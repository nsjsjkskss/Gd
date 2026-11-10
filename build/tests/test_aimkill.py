"""Recording-stub regression tests for native recovery and log transport."""
from pathlib import Path
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[1]


def main():
    stubs = (ROOT/'stubs.cs').read_text()
    replacements = (
        ('public static bool operator ==(Object a, Object b) { return false; }',
         'public static bool operator ==(Object a,Object b) { return ReferenceEquals(a,b); }'),
        ('public static bool operator !=(Object a, Object b) { return true; }',
         'public static bool operator !=(Object a,Object b) { return !ReferenceEquals(a,b); }'),
        ('public static float Distance(Vector3 a, Vector3 b) { return 0f; }',
         'public static float Distance(Vector3 a,Vector3 b) { return (float)Math.Sqrt((a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y)+(a.z-b.z)*(a.z-b.z)); }'),
        ('public static Vector3 operator -(Vector3 a, Vector3 b) { return a; }',
         'public static Vector3 operator -(Vector3 a,Vector3 b) { return new Vector3(a.x-b.x,a.y-b.y,a.z-b.z); }'),
        ('public static Vector3 Normalize(Vector3 a) { return a; }',
         'public static Vector3 Normalize(Vector3 a) { float n=Distance(a,new Vector3()); return n>0 ? new Vector3(a.x/n,a.y/n,a.z/n) : new Vector3(); }'),
        ('public static Quaternion LookRotation(Vector3 forward) { return new Quaternion(); }',
         'public static Vector3 LastLook; public static Quaternion LookRotation(Vector3 forward) { LastLook=forward; return new Quaternion(); }'),
        ('public static Quaternion Slerp(Quaternion a, Quaternion b, float t) { return b; }',
         'public static int Slerps; public static Quaternion Slerp(Quaternion a,Quaternion b,float t) { Slerps++; return b; }'),
        ('public static float get_unscaledDeltaTime() { return 0f; }',
         'public static float get_unscaledDeltaTime() { return 0.016f; }'),
        ('public static float Exp(float value) { return 0f; }',
         'public static float Exp(float value) { return (float)Math.Exp(value); }'),
        ('public static float get_realtimeSinceStartup() { return 0f; }',
         'public static float Now=10; public static float get_realtimeSinceStartup() { return Now; }'),
        ('public static Camera get_main() { return null; }',
         'public static Camera Main=new Camera(); public static Camera get_main() { return Main; }'),
        ('public static int get_width() { return 0; }',
         'public static int get_width() { return 1920; }'),
        ('public static int get_height() { return 0; }',
         'public static int get_height() { return 1080; }'),
        ('public Vector3 WorldToScreenPoint(Vector3 p) { return new Vector3(0,0,0); }',
         'public Vector3 WorldToScreenPoint(Vector3 p) { return new Vector3(960+p.x*10,540,1); }'),
        ('public static Event get_current() { return null; }',
         'public static Event get_current() { return new Event(); }'),
        ('public int get_type() { return 0; }',
         'public int get_type() { return 7; }'),
        ('public static string GetString(string k, string d) { return d; }',
         'public static System.Collections.Generic.Dictionary<string,string> Values=new System.Collections.Generic.Dictionary<string,string>(); public static string GetString(string k,string d) { return Values.ContainsKey(k)?Values[k]:d; }'),
        ('public static void SetString(string k, string v) {}',
         'public static void SetString(string k,string v) { Values[k]=v; }'),
        ('public static int GetInt(string key, int value) { return value; }',
         'public static System.Collections.Generic.Dictionary<string,int> Ints=new System.Collections.Generic.Dictionary<string,int>(); public static int GetInt(string key,int value) { return Ints.ContainsKey(key)?Ints[key]:value; }'),
        ('public static void SetInt(string key, int value) {}',
         'public static void SetInt(string key,int value) { Ints[key]=value; }'),
        ('public static float GetFloat(string key, float value) { return value; }',
         'public static System.Collections.Generic.Dictionary<string,float> Floats=new System.Collections.Generic.Dictionary<string,float>(); public static float GetFloat(string key,float value) { return Floats.ContainsKey(key)?Floats[key]:value; }'),
        ('public static void SetFloat(string key, float value) {}',
         'public static void SetFloat(string key,float value) { Floats[key]=value; }'),
        ('public float FireIntervalScale { get { return 1f; } set {} }',
         'public float FireIntervalScale { get; set; }'),
        ('public bool IsAmmoFree { get { return false; } set {} }',
         'public bool IsAmmoFree { get; set; }'),
        ('public GameObject get_gameObject() { return null; }',
         'public GameObject Go=new GameObject("test"); public GameObject get_gameObject() { return Go; }'),
        ('public Transform get_transform() { return null; }',
         'public Transform Tf; public Transform get_transform() { return Tf; }'),
        ('public bool get_activeInHierarchy() { return false; }',
         'public bool Active=true; public bool get_activeInHierarchy() { return Active; }'),
        ('public Vector3 get_position() { return new Vector3(0,0,0); }',
         'public Vector3 Position; public Vector3 get_position() { return Position; }'),
        ('public bool IsChildOf(Transform parent) { return false; }',
         'public Transform Parent; public bool IsChildOf(Transform parent) { return Parent==parent; }'),
        ('public Collider get_collider() { return null; }',
         'public Collider Collider; public Collider get_collider() { return Collider; }'),
        ('public float get_distance() { return 0f; }',
         'public float Distance; public float get_distance() { return Distance; }'),
        ('public class Physics {',
         'public class Physics { public static Collider Obstacle;'),
        ('hit = new RaycastHit(); return false;',
         'hit = new RaycastHit { Collider=Obstacle, Distance=0.1f }; return Obstacle!=null;'),
        ('public Bounds get_bounds() { return new Bounds(); }',
         'public Bounds get_bounds() { return new Bounds { Center=Tf.Position }; }'),
        ('public Vector3 get_center() { return new Vector3(); }',
         'public Vector3 Center; public Vector3 get_center() { return Center; }'),
        ('public bool get_isTrigger() { return false; }',
         'public bool Trigger; public bool get_isTrigger() { return Trigger; }'),
        ('public bool IsLocalPlayer() { return false; }',
         'public bool Local; public bool IsLocalPlayer() { return Local; }'),
        ('public bool IsLocalTeammate(bool includeDead) { return false; }',
         'public bool Team; public bool IsLocalTeammate(bool includeDead) { return Team; }'),
        ('public bool get_IsDieing() { return false; }',
         'public bool Knocked; public bool get_IsDieing() { return Knocked; }'),
        ('public bool IsFiring() { return false; }',
         'public bool Firing; public bool IsFiring() { return Firing; }'),
        ('public bool IsReloading() { return false; }',
         'public bool Reloading; public bool IsReloading() { return Reloading; }'),
        ('public UnityEngine.Transform get_RootTransform() { return null; }',
         'public UnityEngine.Transform Root; public UnityEngine.Transform get_RootTransform() { return Root; }'),
        ('public UnityEngine.Transform GetHeadTF() { return null; }',
         'public UnityEngine.Transform Head; public UnityEngine.Transform GetHeadTF() { return Head; }'),
        ('public UnityEngine.Collider get_HeadCollider() { return null; }',
         'public UnityEngine.Collider HeadCollider; public UnityEngine.Collider get_HeadCollider() { return HeadCollider; }'),
        ('public HBIBDMMOOOK GetWeaponOnHand() { return null; }',
         'public HBIBDMMOOOK Weapon; public HBIBDMMOOOK GetWeaponOnHand() { return Weapon; }'),
        ('public PlayerAttributes get_Attributes() { return null; }',
         'public PlayerAttributes Attr=new PlayerAttributes(); public PlayerAttributes get_Attributes() { return Attr; }'),
        ('public virtual void StartFiring(HBIBDMMOOOK weapon) {}',
          'public int Starts,Stops,Aims; public bool ThrowStart,ThrowStop; public HBIBDMMOOOK StartedWeapon,StoppedWeapon,SwapOnStart; public virtual void StartFiring(HBIBDMMOOOK weapon) { Starts++; StartedWeapon=weapon; Firing=true; Trace.Steps+="start;"; Okmod.SilentAimingInfo(this); if(SwapOnStart!=null) Weapon=SwapOnStart; if(ThrowStart) throw new Exception("start failure"); }'),
        ('public virtual void StopFire(HBIBDMMOOOK weapon) {}',
         'public virtual void StopFire(HBIBDMMOOOK weapon) { Stops++; StoppedWeapon=weapon; Firing=false; Trace.Steps+="stop;"; if(ThrowStop) throw new Exception("stop failure"); }'),
        ('public virtual void FullWeaponAmmo() {}',
         'public int AmmoFills; public virtual void FullWeaponAmmo() { AmmoFills++; }'),
        ('public bool IsGroundedByRayCastHit() { return false; }',
         'public bool GroundedValue; public bool IsGroundedByRayCastHit() { return GroundedValue; }'),
        ('public void SetAimRotation(UnityEngine.Quaternion rotation, bool force) {}',
         'public void SetAimRotation(UnityEngine.Quaternion rotation,bool force) { Aims++; Trace.Steps+="aim;"; }'),
        ('public int LHNJLFOINDO(CGKJLKPMGDJ hit) { return 0; }',
         'public int FallbackHits; public bool ThrowHit; public CGKJLKPMGDJ LastHit; public int LHNJLFOINDO(CGKJLKPMGDJ hit) { FallbackHits++; LastHit=hit; Trace.Steps+="fbhit;"; if(ThrowHit) throw new Exception("test fallback failure"); return 17; }'),
        ('public int CIHHEOKFADJ(CGKJLKPMGDJ hit, UnityEngine.MonoBehaviour target, float param) { return 0; }',
         'public int Hits; public int PcHits; public bool ThrowPc; public Player PcTarget; public int CIHHEOKFADJ(CGKJLKPMGDJ hit, UnityEngine.MonoBehaviour target, float param) { Hits++; PcHits++; PcTarget=(Player)target; LastHit=hit; Trace.Steps+="hit;"; if(ThrowPc) throw new Exception("test pc failure"); return 23; }'),
        ('public int KBIFKMJODNA() { return 0; }',
         'public int KBIFCalls; public int KBIFKMJODNA() { KBIFCalls++; return 99; }'),
        ('public float LDKCJEMDAMC() { return 0f; }',
         'public float NativeInterval=0.2f; public bool ThrowCadence; public float LDKCJEMDAMC() { if(ThrowCadence) throw new Exception("cadence unavailable"); return NativeInterval; }'),
        ('public float BDHMJJIADFG() { return 0f; }',
         'public float RepeatInterval=0.4f; public float BDHMJJIADFG() { return RepeatInterval; }'),
        ('public bool KENANOGLPJO() { return false; }',
         'public bool Repeat; public bool KENANOGLPJO() { return Repeat; }'),
        ('public uint MJCENIGKEPH() { return 0; }',
         'public uint MJCENIGKEPH() { return 4242; }'),
        ('public uint GetId() { return 0; }',
         'public uint GetId() { return 815; }'),
        ('public interface OGKFEMJNCGM { UnityEngine.GameObject get_gameObject(); }',
         'public interface OGKFEMJNCGM { UnityEngine.GameObject get_gameObject(); } public class TestAttackable : OGKFEMJNCGM { public UnityEngine.GameObject Go; public UnityEngine.GameObject get_gameObject() { return Go; } }'),
        ('public OGKFEMJNCGM JGIEOGCGFEP(CGKJLKPMGDJ hit) { return null; }',
         'public CGKJLKPMGDJ ResolvedHit; public UnityEngine.GameObject WrongResolved; public OGKFEMJNCGM JGIEOGCGFEP(CGKJLKPMGDJ hit) { ResolvedHit=hit; return new TestAttackable { Go=WrongResolved ?? ((Player)COW.GameFacade.Match.Players[1]).Go }; }'),
        ('public bool IsInvincible() { return false; }',
         'public bool Invincible; public bool IsInvincible() { return Invincible; }'),
        ('public System.Collections.Generic.List<float> NPLMKLBKJPE(OGKFEMJNCGM target, UnityEngine.Collider collider, bool flag, HNLIMBLIANL info) { return null; }',
         'public HNLIMBLIANL PreparedInfo; public System.Collections.Generic.List<float> NPLMKLBKJPE(OGKFEMJNCGM target, UnityEngine.Collider collider, bool flag, HNLIMBLIANL info) { PreparedInfo=info; if(info!=null) info.OCOCPPNPHKN=77; return new System.Collections.Generic.List<float>(); }'),
        ('public int TakeDamage(int damage, BEADLMGGGGL playerId, HNLIMBLIANL info,\n        int weaponId, UnityEngine.Vector3 firePos, UnityEngine.Vector3 targetPos,\n        System.Collections.Generic.List<float> checks, PIAEKELILCJ dynamicInfo, uint vehicleId) { return 0; }',
          'public static int ExplicitCalls; public int TakeDamage(int damage, BEADLMGGGGL playerId, HNLIMBLIANL info,\n        int weaponId, UnityEngine.Vector3 firePos, UnityEngine.Vector3 targetPos,\n        System.Collections.Generic.List<float> checks, PIAEKELILCJ dynamicInfo, uint vehicleId) { ExplicitCalls++; return 23; }'),
        ('public float OEOBPHDFJMO() { return 0f; }',
         'public float Range=100; public float OEOBPHDFJMO() { return Range; }'),
        ('public static COW.GamePlay.JMAGGLCNGIG CurrentMatch() { return null; }',
         'public static COW.GamePlay.JMAGGLCNGIG Match; public static COW.GamePlay.JMAGGLCNGIG CurrentMatch() { return Match; }'),
        ('public static COW.GamePlay.Player GetLocalPlayerOrObServer() { return null; }',
         'public static COW.GamePlay.Player Local; public static COW.GamePlay.Player GetLocalPlayerOrObServer() { return Local; }'),
        ('public System.Collections.IList ILPNGCMEFFI() { return null; }',
         'public System.Collections.IList Players; public System.Collections.IList ILPNGCMEFFI() { return Players; }'),
        ('public static bool Send(uint id, GCommon.UDPClientMessageBase msg, byte option, bool cache) { return false; }',
          'public static bool Sent; public static uint SentId; public static GCommon.UDPClientMessageBase SentMsg; public static int SentCount; public static bool SendResult=true,ThrowSend; public static byte SentOption; public static bool SentCache; public static bool Send(uint id, GCommon.UDPClientMessageBase msg, byte option, bool cache) { Sent=true; SentId=id; SentMsg=msg; SentCount++; SentOption=option; SentCache=cache; Trace.Steps+="send108;"; if(ThrowSend) throw new Exception("send108 failure"); return SendResult; }'),
        ('public static uint DEIPJDIGKIP(BEADLMGGGGL value) { return 0; }',
         'public static uint DEIPJDIGKIP(BEADLMGGGGL value) { return 7; }'),
        ('public static float Sqrt(float f) { return 0f; }',
         'public static float Sqrt(float f) { return (float)Math.Sqrt(f); }'),
        ('public static BJFGLKKKOAN MGOGJCMCIPG(UnityEngine.Vector3 direction) { return null; }',
         'public static BJFGLKKKOAN MGOGJCMCIPG(UnityEngine.Vector3 direction) { return new BJFGLKKKOAN(); }'),
        ('public UnityWebRequest(string url) {}',
         'public static UnityWebRequest Last; public string Url; public bool Done,Disposed; public long Code; public string Error; public UnityWebRequest(string url) { Url=url; Last=this; }'),
        ('public bool get_isDone() { return false; }',
         'public bool get_isDone() { return Done; }'),
        ('public long get_responseCode() { return 0; }',
         'public long get_responseCode() { return Code; }'),
        ('public string get_error() { return null; }',
         'public string get_error() { return Error; }'),
        ('public void Dispose() {}',
         'public void Dispose() { Disposed=true; }'),
    )
    for old,new in replacements:
        assert stubs.count(old)==1,old
        stubs=stubs.replace(old,new,1)
    start=stubs.index('public class Player : UnityEngine.MonoBehaviour')
    end=stubs.index('public class JMAGGLCNGIG',start)
    part=stubs[start:end].replace('public int get_CurHP() { return 0; }',
                                'public int Hp=200; public int get_CurHP() { return Hp; }')
    part=part.replace('public int get_MaxHP() { return 0; }',
                      'public int MaxHp=200; public int get_MaxHP() { return MaxHp; }')
    stubs=stubs[:start]+part+stubs[end:]
    harness=r'''
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using COW;
using COW.GamePlay;
using message;
using S=COW.GamePlay.SceneEditBoxSelectTool;
public static class Trace { public static string Steps=""; }
class AimKillTests {
 static void Check(bool ok,string text) { if(!ok) throw new Exception(text); }
 static void Raw() { typeof(Okmod).GetMethod("UpdateAimKill",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null); }
 static void Tick() { Time.Now+=1; Raw(); }
  static PlayerNetwork Enemy(float z) {
   var p=new PlayerNetwork { Root=new Transform(),Head=new Transform { Position=new Vector3(0,1,z) } };
  p.HeadCollider=new Collider { Tf=p.Head };p.Head.Parent=p.Root;return p;
 }
 static void LegacyMain() {
   var local=new PlayerNetwork();local.Root=new Transform();local.Head=new Transform { Position=new Vector3(0,1,0) };local.HeadCollider=new Collider { Tf=local.Head };local.Head.Parent=local.Root;local.Local=true;local.Weapon=new HBIBDMMOOOK();local.Weapon.ODFDGMOCJLL=4242;local.Weapon.PIILKNEHNLB=new IOKEALMHBKN();GameFacade.Local=local;
   var near=Enemy(10);var far=Enemy(30);var team=Enemy(2);team.Team=true;
   var knocked=Enemy(3);knocked.Knocked=true;
   var inactive=Enemy(4);inactive.Go.Active=false;
  GameFacade.Match=new JMAGGLCNGIG { Players=new ArrayList { local,far,team,knocked,inactive,near } };
   Tick();Check(local.Starts==0,"default off");
  Check(!GameFacade.Sent,"no inventory packet when disabled");
  PlayerPrefs.SetString("OKMOD.Aim.AimKillSource","");
  PlayerPrefs.SetString("OKMOD.Aim.AimKill","X2");
  PlayerPrefs.SetString("OKMOD.Aim.Enabled","X2");PlayerPrefs.SetString("OKMOD.Aim.Silent","X2");
   Tick();Check(PlayerPrefs.GetString("OKMOD.Aim.Enabled","")=="" && PlayerPrefs.GetString("OKMOD.Aim.Silent","")=="","mode exclusion");
   Check(local.Weapon.KBIFCalls==1 && local.Starts==1 && local.Stops==0,"single target, one damage dispatch per tick");
  Check(local.AmmoFills>=1,"clip kept fed while aimkill is on");
   Check(Trace.Steps=="start;fbhit;","sequence");
  var h=local.Weapon.LastHit;
  Check(object.ReferenceEquals(h.JINDNOFABKB,near.HeadCollider),"nearest valid enemy, excludes team/knocked/inactive");
  Check(h.DKPDBLLJEAA==LKOOALMKJND.Head && Math.Abs(h.CFIPDNFBKHK-10)<0.001,"hit group/distance");
  Check(Math.Abs(h.HLDECMJFKJK.z-1)<0.001 && h.ONMKMEGMHOL.y==1,"direction/origin");
   Check(S.R7AimKillStatus==4 && S.R7AimKillResult==23 && S.R7AimKillPC==1 && local.Weapon.KBIFCalls==1 && S.R7AimKillChainStage==9,"full PC damage chain dispatched");
  Check(S.R7AimKillGroup==1,"head group recorded");
  typeof(Okmod).GetMethod("UpdateAimKill",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
  Check(local.Weapon.KBIFCalls==1,"rate limit");
   local.Firing=true;Tick();Check(local.Weapon.KBIFCalls==1 && S.R7AimKillManualSkips>0,"manual fire is not hijacked");local.Firing=false;
   local.Reloading=true;Tick();Check(local.Weapon.KBIFCalls==1,"reload stops dispatch");local.Reloading=false;
   local.Weapon.Range=5;Tick();Check(local.Weapon.KBIFCalls==1,"actual weapon range");local.Weapon.Range=100;
   near.HeadCollider.Trigger=true;Tick();Check(local.Weapon.KBIFCalls==1,"trigger collider rejected");near.HeadCollider.Trigger=false;
  int stopsBeforeThrow=local.Stops;
  local.Weapon.ThrowPc=true;local.Weapon.ThrowHit=true;Tick();Check(S.R7AimKillStatus==5 && local.Stops==stopsBeforeThrow+1,"exception stops its shot");
   int hits=local.Weapon.KBIFCalls;typeof(Okmod).GetMethod("UpdateAimKill",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
   Check(local.Weapon.KBIFCalls==hits,"exception does not retry per frame");
  local.Weapon.ThrowPc=false;local.Weapon.ThrowHit=false;near.Hp=0;far.Hp=0;Tick();Check(S.R7AimKillStatus==2 && local.Weapon.KBIFCalls==hits,"no target no fire");
   local.Local=false;Tick();Check(local.Weapon.KBIFCalls==hits,"observer/nonlocal rejected");

  // Per-shot mode: every dispatch starts and stops its own fire.
  local.Local=true;near.Hp=200;Time.Now=500;S.R7AimKillNext=0;Raw();
   Check(local.Stops>0,"trigger released after the shot window");

  PlayerPrefs.SetString("OKMOD.Aim.AimKill","");Tick();Check(S.R7AimKillStatus==0,"off status");

  // All directions: pick the nearest enemy even when it is off-axis.
   var side=Enemy(5);side.Head.Position=new Vector3(8,1,5);((ArrayList)GameFacade.Match.Players).Add(side);
  local.Local=true;near.Hp=200;near.Head.Position=new Vector3(0,1,10);
  PlayerPrefs.SetString("OKMOD.Aim.AimKill","X2");
  PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay",0);Time.Now=50;S.R7AimKillNext=0;Raw();
  Check(object.ReferenceEquals(local.Weapon.LastHit.JINDNOFABKB,side.HeadCollider),"all-direction pick takes nearest");
  side.Hp=0;

  // Delay cadence modes 1 (20 ms) and 3 (80 ms).
  local.Local=true;near.Hp=200;near.Head.Position=new Vector3(0,1,10);
  PlayerPrefs.SetString("OKMOD.Aim.AimKill","X2");
  PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay",0);
  Time.Now=100;S.R7AimKillNext=0;Raw();
   int baseHits=local.Weapon.KBIFCalls;
   Time.Now=100.01f;Raw();Check(local.Weapon.KBIFCalls==baseHits,"80 ms blocks 10 ms");
   Time.Now=100.09f;Raw();Check(local.Weapon.KBIFCalls==baseHits+1,"80 ms allows 90 ms");
  PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay",2);
  Time.Now=200;S.R7AimKillNext=0;Raw();
   int base80=local.Weapon.KBIFCalls;
   Time.Now=200.04f;Raw();Check(local.Weapon.KBIFCalls==base80,"200 ms blocks 40 ms");
   Time.Now=200.21f;Raw();Check(local.Weapon.KBIFCalls==base80+1,"200 ms allows 210 ms");
  var log=(string)typeof(Okmod).GetMethod("BuildAimKillLog",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
  Check(log!=null && log.Contains("stop=") && log.Contains("ammoFill="),"diagnostic log builds");

 }
  static void ConfigurablePresetHistoricalTests() {
   var local=Enemy(0);local.Local=true;local.Weapon=new HBIBDMMOOOK();GameFacade.Local=local;
   var hit=new CGKJLKPMGDJ();hit.OLGEMBBDBID=123;local.FDMIEDDNCEC=hit;
   GameFacade.Match=new JMAGGLCNGIG { Players=new ArrayList { local,Enemy(10) } };
   PlayerPrefs.SetString("OKMOD.Aim.AimKill","X2");
   PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");
   PlayerPrefs.SetString("OKMOD.Aim.Enabled","X2");PlayerPrefs.SetString("OKMOD.Aim.Silent","X2");
   S.R7AimKillFiring=true;S.R7AimFireUntil=9999;
   for(int i=0;i<500;i++) { local.Firing=i%2==0;Tick(); }
   Check(local.Starts==0 && local.Stops==0 && local.AmmoFills==0,"recovery never seizes trigger or refills ammo");
   Check(local.Weapon.KBIFCalls==0 && S.R7AimKillDispatches==0 && S.R7AimKillCalls==0,"no synthetic dispatches");
   Check(object.ReferenceEquals(local.FDMIEDDNCEC,hit) && hit.OLGEMBBDBID==123,"native hit-info unmodified");
   Check(S.R7AimKillStatus==8 && !S.R7AimKillFiring,"recovery explicit in status");
    Check(PlayerPrefs.GetString("OKMOD.Aim.Enabled","")=="X2" && PlayerPrefs.GetString("OKMOD.Aim.Silent","")=="X2","recovery preserves feature preferences");
   PlayerPrefs.SetString("OKMOD.Combat.NoReload","X2");PlayerPrefs.SetString("OKMOD.Combat.RapidFire","X2");
   S.R7SpeedOwner=local.Attr;S.R7SpeedApplied=true;S.R7SpeedOld=1f;local.Attr.RunSpeedUpScale=1.8f;
   S.R7NoReloadOwner=local.Attr;S.R7NoReloadApplied=true;S.R7NoReloadOld=false;local.Attr.ShootNoReload=true;
   typeof(Okmod).GetMethod("UpdateCombat",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
   Check(local.Attr.RunSpeedUpScale==1f && !local.Attr.ShootNoReload,"applied speed/reload restored");
   PlayerPrefs.SetString("OKMOD.Move.HighJump","X2");PlayerPrefs.SetString("OKMOD.Move.Hover","X2");
   S.R7GravityOld=-9.8f;S.R7GravityApplied=1;IPBGFJNGLOH.AMMOCNCMBBG=-0.3f;
   typeof(Okmod).GetMethod("UpdateJumpBoost",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
   Check(Math.Abs(IPBGFJNGLOH.AMMOCNCMBBG+9.8f)<0.001 && S.R7GravityApplied==0,"global gravity restored once");
   Check(Math.Abs(Okmod.CustomGravity(local)+9.8f)<0.001,"normal native gravity during recovery");
    PlayerPrefs.SetString("OKMOD.Aim.AimKill","");
    PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");Tick();
     Check(S.R7AimKillStatus==0 && local.Stops==0,"recovery off and AimKill off preserve manual fire");
    var combat=typeof(Okmod).GetMethod("UpdateCombat",BindingFlags.NonPublic|BindingFlags.Static);
    var speed=typeof(Okmod).GetMethod("ReadSpeedScale",BindingFlags.NonPublic|BindingFlags.Static);
    PlayerPrefs.SetFloat("OKMOD.Move.SpeedScale.Float",15f);
    Check(Math.Abs((float)speed.Invoke(null,null)-1.5f)<0.001,"old ESP speed units normalized");
    local.Attr.FireIntervalScale=1f;
    PlayerPrefs.SetString("OKMOD.Combat.InfiniteAmmo","X2");
    combat.Invoke(null,null);
    Check(local.Attr.ShootNoReload && local.Attr.IsAmmoFree && local.Attr.FireIntervalScale==0.5f && local.Attr.RunSpeedUpScale==1.5f,"features resume independently after recovery off");
    PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");combat.Invoke(null,null);
    Check(!local.Attr.ShootNoReload && !local.Attr.IsAmmoFree && local.Attr.FireIntervalScale==1f && local.Attr.RunSpeedUpScale==1f,"recovery on restores native values");
    PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");combat.Invoke(null,null);
    Check(local.Attr.ShootNoReload && local.Attr.IsAmmoFree && local.Attr.RunSpeedUpScale==1.5f,"features reapply after second recovery toggle");
    PlayerPrefs.SetString("OKMOD.Combat.NoReload","");PlayerPrefs.SetString("OKMOD.Combat.InfiniteAmmo","");PlayerPrefs.SetString("OKMOD.Combat.RapidFire","");
    PlayerPrefs.SetFloat("OKMOD.Move.SpeedScale.Float",1f);combat.Invoke(null,null);
     Check(!local.Attr.ShootNoReload && !local.Attr.IsAmmoFree && local.Attr.FireIntervalScale==1f && local.Attr.RunSpeedUpScale==1f,"individual feature toggles restore values");
   var enabled=typeof(Okmod).GetMethod("SetAimKillEnabled",BindingFlags.NonPublic|BindingFlags.Static);
   var probes=typeof(Okmod).GetMethod("UpdateAimKillProbes",BindingFlags.NonPublic|BindingFlags.Static);
    local.Firing=false;local.Weapon.PIILKNEHNLB=new IOKEALMHBKN { JAOECECKADN=999, MHMGCLILKGJ=new IJHHJLDNMGH() };
   enabled.Invoke(null,new object[]{true});Tick();
    Check(S.R7AimKillDispatches==1 && S.R7AimKillPC==1 && S.R7AimKillChainStage==9,"AimKill ON reaches real damage chain");
     Check(local.Weapon.FallbackHits==1 && PlayerNetwork.ExplicitCalls==0,"single engine-hit entry, no duplicate explicit damage");
     Check(S.R7AimKillStartResult==17 && S.R7AimKillResult==17 && S.R7AimKillCheckCount==0,"engine return and unobserved intermediates recorded");
     Check(local.Starts==1 && local.Stops==1 && local.AmmoFills==0 && !local.Firing,"owned shot starts/stops synchronously without refill");
      Check(Trace.Steps.EndsWith("aim;start;fbhit;stop;aim;"),"reference order and aim restoration");
      Check(S.R7ManualHitSamples==0,"synthetic StartFiring is not counted as manual input");
    Check(local.Weapon.LastHit!=null && !object.ReferenceEquals(local.Weapon.LastHit,hit) && local.FDMIEDDNCEC.OLGEMBBDBID==123,"engine dispatch uses isolated hit");
   int dispatched=S.R7AimKillDispatches;
   Time.Now+=0.1f;Raw();Check(S.R7AimKillDispatches==dispatched,"cadence throttles repeated HUD passes");
    float deadline=S.R7ProbeAt[0];
    Time.Now+=0.15f;Raw();
    Check(S.R7AimKillDispatches==dispatched+1,"pending HP observation no longer blocks cadence");
    Check(S.R7ProbeAt[0]==deadline && S.R7ProbeAt[1]==0,"repeated hit preserves one target observation deadline");
    dispatched=S.R7AimKillDispatches;
    var cadence=typeof(Okmod).GetMethod("AimKillWeaponDelay",BindingFlags.NonPublic|BindingFlags.Static);
    local.Weapon.NativeInterval=0.8f;
    Check(Math.Abs((float)cadence.Invoke(null,new object[]{local.Weapon})-0.2f)<0.001 && Math.Abs(S.R7AimKillNativeDelay-0.8f)<0.001,"slow native interval is observed, not imposed");
    local.Weapon.Repeat=true;
    Check(Math.Abs((float)cadence.Invoke(null,new object[]{local.Weapon})-0.2f)<0.001 && Math.Abs(S.R7AimKillNativeDelay-0.4f)<0.001,"repeat native observation does not change selected delay");
    local.Weapon.Repeat=false;local.Weapon.NativeInterval=float.NaN;
    Check(Math.Abs((float)cadence.Invoke(null,new object[]{local.Weapon})-0.2f)<0.001 && S.R7AimKillNativeDelay==-1f,"invalid native observation does not add fallback wait");
    local.Weapon.ThrowCadence=true;
    Check(Math.Abs((float)cadence.Invoke(null,new object[]{local.Weapon})-0.2f)<0.001 && S.R7AimKillNativeDelay==-1f,"native getter exception does not block selected cadence");
    local.Weapon.ThrowCadence=false;
     local.Weapon.NativeInterval=0.2f;
     PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay",4);local.Weapon.NativeInterval=0.065f;
     Check(Math.Abs((float)cadence.Invoke(null,new object[]{local.Weapon})-0.08f)<0.001,"80ms selectable for faster native weapon");
     local.Weapon.NativeInterval=0.13f;
     Check(Math.Abs((float)cadence.Invoke(null,new object[]{local.Weapon})-0.08f)<0.001,"80ms selection is not raised to 130ms");
     PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay",1);local.Weapon.NativeInterval=0.2f;
    probes.Invoke(null,null);int priorStops=local.Stops;local.Firing=true;Time.Now+=2f;Raw();
    Check(S.R7AimKillDispatches==dispatched && S.R7AimKillStatus==7 && local.Stops==priorStops,"manual fire takes priority without StopFire");
   local.Firing=false;Physics.Obstacle=new Collider { Tf=new Transform() };Time.Now+=2;Raw();
   Check(S.R7AimKillDispatches==dispatched,"near wall blocks damage before dispatch");
   Physics.Obstacle=null;Time.Now+=2;Raw();
    Check(S.R7AimKillDispatches==dispatched+1,"visible target resumes dispatch once without catch-up burst");
    enabled.Invoke(null,new object[]{false});Time.Now+=2;Raw();
    Check(S.R7AimKillStatus==0 && S.R7AimKillDispatches==dispatched+1 && local.Stops==local.Starts,"AimKill OFF stops dispatch only, no deferred cleanup");
   enabled.Invoke(null,new object[]{true});Time.Now+=2;Raw();
   Check(S.R7AimKillDispatches==dispatched+2,"AimKill re-enables");
    enabled.Invoke(null,new object[]{false});
    enabled.Invoke(null,new object[]{true});GameFacade.Match=null;Time.Now+=2;Raw();
    Check(S.R7AimKillStop==2 && S.R7AimKillLocalReason=="no-match" && S.R7AimKillPC==0,"no-match does not retain successful pc flag");
    enabled.Invoke(null,new object[]{false});
    var target=Enemy(10);GameFacade.Match=new JMAGGLCNGIG { Players=new ArrayList { local,target } };
    target.Attr.Invincible=true;enabled.Invoke(null,new object[]{true});Time.Now+=2;Raw();
    int beforeGuard=S.R7AimKillDispatches;
    Check(S.R7AimKillInvincibleSkips>0 && S.R7AimKillChainError.Contains("invincible"),"invincible target diagnosed");
      target.Attr.Invincible=false;local.Weapon.ThrowHit=true;Time.Now+=2;Raw();
      Check(S.R7AimKillDispatches==beforeGuard && S.R7AimKillChainError.Contains("failure") && local.Stops==local.Starts && !local.Firing,"engine exception cleans owned trigger, no fallback");
     local.Weapon.ThrowHit=false;Time.Now+=2;Raw();
    Check(S.R7AimKillDispatches==beforeGuard+1 && S.R7AimKillImmediateBefore==200 && S.R7AimKillImmediateAfter==200,"immediate HP logged independently from return code");
    target.Hp=220;Time.Now+=2;probes.Invoke(null,null);
    Check(S.R7ProbeHealed>0 && S.R7ProbeLastAfter==220,"HP recovery separately recorded");
    PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");Raw();
    Check(S.R7AimKillRecoveryWasOn,"recovery transition tracked");
    PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");GameFacade.Match=null;Raw();
    Check(!S.R7AimKillRecoveryWasOn && S.R7AimKillChainError=="" && S.R7AimKillChainStage==0,"recovery error not retained after resume");
     enabled.Invoke(null,new object[]{false});
     enabled.Invoke(null,new object[]{true});
     PlayerPrefs.SetString("OKMOD.Aim.Silent","");local.Firing=true;
     hit.OLGEMBBDBID=37;hit.CFIPDNFBKHK=10;hit.GABPHAKNADH=8;
     int starts=local.Starts, stops=local.Stops, engineHits=local.Weapon.FallbackHits;
     var returned=Okmod.SilentAimingInfo(local);
     Check(object.ReferenceEquals(returned,hit) && hit.OLGEMBBDBID==37 && hit.GABPHAKNADH==8,"manual observer is a read-only passthrough");
     Check(S.R7ManualHitSamples==1 && S.R7ManualHitSnapshot.Contains("damage=37") && S.R7ManualHitSnapshot.Contains("layer=8"),"native firing sample recorded");
     Okmod.SilentAimingInfo(local);Check(S.R7ManualHitSamples==1,"manual observer rate limited");
     Check(local.Starts==starts && local.Stops==stops && local.Weapon.FallbackHits==engineHits,"manual observation performs no firing or hit calls");
     local.Firing=false;Time.Now+=1;Okmod.SilentAimingInfo(local);
     Check(S.R7ManualHitSamples==1,"no sample outside real firing");
      enabled.Invoke(null,new object[]{false});
      var execute=typeof(Okmod).GetMethod("ExecuteAimKillShot",BindingFlags.NonPublic|BindingFlags.Static);
      var originalWeapon=local.Weapon;var otherWeapon=new HBIBDMMOOOK();
      GameFacade.Match=new JMAGGLCNGIG { Players=new ArrayList { local,target } };
       local.Firing=false;local.SwapOnStart=otherWeapon;
       int beforeSwap=local.Weapon.FallbackHits;
       execute.Invoke(null,new object[]{local,originalWeapon,target,hit,new Vector3(0,1,0),new Vector3(0,1,10)});
       Check(object.ReferenceEquals(local.StoppedWeapon,originalWeapon),"cleanup uses captured weapon after native swap");
       Check(originalWeapon.FallbackHits==beforeSwap && S.R7AimShotAborted>0,"no hit on stale weapon after native start swaps it");
      local.SwapOnStart=null;local.Weapon=originalWeapon;local.ThrowStart=true;
      int nStops=local.Stops;execute.Invoke(null,new object[]{local,originalWeapon,target,hit,new Vector3(0,1,0),new Vector3(0,1,10)});
      Check(local.Stops==nStops+1 && !local.Firing,"partial start exception still gets exactly one stop");
      local.ThrowStart=false;local.ThrowStop=true;
      execute.Invoke(null,new object[]{local,originalWeapon,target,hit,new Vector3(0,1,0),new Vector3(0,1,10)});
      Check(S.R7AimShotFaulted && S.R7AimShotCleanupError.Contains("stop failure"),"cleanup fault latched");
      int nStarts=local.Starts;Tick();Check(local.Starts==nStarts,"cleanup fault blocks automated retries");
      local.ThrowStop=false;
      var record=typeof(Okmod).GetMethod("RecordAimKillProbe",BindingFlags.NonPublic|BindingFlags.Static);
      var clear=typeof(Okmod).GetMethod("ClearAimKillProbes",BindingFlags.NonPublic|BindingFlags.Static);
      clear.Invoke(null,null);target.Hp=200;record.Invoke(null,new object[]{target,200});
      int okBefore=S.R7ProbeOk;S.R7ManualHitSamples++;target.Hp=150;Time.Now+=1;probes.Invoke(null,null);
       Check(S.R7ProbeMixed>0 && S.R7ProbeOk==okBefore,"manual-contaminated HP windows not counted as clean observations");
       Check(GameFacade.SentCount==0,"sync108 is default off");
       S.R7AimShotFaulted=false;local.Firing=false;
       originalWeapon.ODFDGMOCJLL=987654u;local.Weapon=originalWeapon;
       enabled.Invoke(null,new object[]{true});PlayerPrefs.SetString("OKMOD.Aim.Sync108","X2");Time.Now+=2;
       bool shot=(bool)execute.Invoke(null,new object[]{local,originalWeapon,target,hit,new Vector3(0,1,0),new Vector3(0,1,10)});
       var packet=GameFacade.SentMsg as message.EOBFCLNNIJH;
       Check(shot && GameFacade.SentCount==1 && GameFacade.SentId==108 && packet!=null,"owned successful shot sends one 108");
       Check(packet.BEADLMGGGGL==7 && packet.FIKKBFDEJOK==987654u && packet.FIKKBFDEJOK!=4242u,"packet uses player id and instance uid, not weapon data id");
       Check(GameFacade.SentOption==2 && !GameFacade.SentCache && Trace.Steps.EndsWith("stop;aim;send108;"),"send happens after cleanup with reference options");
       var sync=typeof(Okmod).GetMethod("TrySyncInventory108",BindingFlags.NonPublic|BindingFlags.Static);
       sync.Invoke(null,new object[]{local,originalWeapon,987654u});Check(GameFacade.SentCount==1,"no queued burst under 250ms");
       Time.Now+=0.3f;GameFacade.SendResult=false;sync.Invoke(null,new object[]{local,originalWeapon,987654u});
       Check(S.R7Sync108Failed==1 && S.R7Sync108Sent==1,"false send not recorded as success");
       Time.Now+=0.3f;GameFacade.ThrowSend=true;sync.Invoke(null,new object[]{local,originalWeapon,987654u});
       Check(S.R7Sync108Failed==2 && S.R7Sync108Error.Contains("send108 failure"),"send exception stays separate from hit error");
       GameFacade.ThrowSend=false;GameFacade.SendResult=true;Time.Now+=0.3f;originalWeapon.ODFDGMOCJLL=999u;
       int sendCount=GameFacade.SentCount;sync.Invoke(null,new object[]{local,originalWeapon,987654u});
       Check(GameFacade.SentCount==sendCount,"changed unique id prevents stale sync");
       originalWeapon.ODFDGMOCJLL=987654u;local.ThrowStop=true;Time.Now+=0.3f;
       execute.Invoke(null,new object[]{local,originalWeapon,target,hit,new Vector3(0,1,0),new Vector3(0,1,10)});
       Check(GameFacade.SentCount==sendCount,"failed cleanup sends no 108");
        local.ThrowStop=false;PlayerPrefs.SetString("OKMOD.Aim.Sync108","");
        S.R7AimShotFaulted=false;S.R7AimKillNext=0;S.R7AimKillNextHit=0;
        S.R7AimCadenceWeapon=originalWeapon;S.R7AimCadenceLast=Time.Now;
         originalWeapon.NativeInterval=1.82f;local.Weapon=originalWeapon;local.Firing=false;
        PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay",4);
        float scheduledLast=Time.Now;int deadlineHits=S.R7AimKillDispatches;
         Time.Now=scheduledLast+0.06f;Raw();
         Check(S.R7AimKillStatus==10 && Math.Abs(S.R7AimKillNext-(scheduledLast+0.08f))<0.001,"wake at chosen 80ms deadline despite native 1820ms");
         Time.Now=scheduledLast+0.081f;Raw();
         Check(S.R7AimKillDispatches==deadlineHits+1,"hit on first eligible frame after deadline");
         Raw();Check(S.R7AimKillDispatches==deadlineHits+1,"no extra hit on repeated pass at same timestamp");
        var fastWeapon=new HBIBDMMOOOK { NativeInterval=0.065f, ODFDGMOCJLL=444u };
         local.Weapon=fastWeapon;Time.Now+=0.06f;Raw();
        Check(object.ReferenceEquals(S.R7AimCadenceWeapon,fastWeapon) && S.R7AimKillDispatches==deadlineHits+2,"new weapon does not inherit old weapon cooldown");
        // Config isolation: old aimbot values must not leak into either AK branch.
        var akRange=typeof(Okmod).GetMethod("AimKillRange",BindingFlags.NonPublic|BindingFlags.Static);
        var akFov=typeof(Okmod).GetMethod("AimKillFov",BindingFlags.NonPublic|BindingFlags.Static);
        var akPoint=typeof(Okmod).GetMethod("AimKillPointMode",BindingFlags.NonPublic|BindingFlags.Static);
        PlayerPrefs.SetFloat("OKMOD.Aim.Range.Float",500f);PlayerPrefs.SetFloat("OKMOD.Aim.FOV.Float",800f);
        PlayerPrefs.SetInt("OKMOD.Aim.Point",3);PlayerPrefs.SetFloat("OKMOD.Aim.Smooth.Float",20f);
        Check((float)akRange.Invoke(null,null)==150f && (float)akFov.Invoke(null,null)==220f && (int)akPoint.Invoke(null,null)==0,"AK defaults independent of aimbot config");
        PlayerPrefs.SetFloat("OKMOD.AK.Range.Float",75f);PlayerPrefs.SetFloat("OKMOD.AK.FOV.Float",333f);
        Check((float)akRange.Invoke(null,null)==75f && (float)akFov.Invoke(null,null)==333f,"AK own settings read");
        local.Weapon=fastWeapon;local.Firing=true;Time.Now+=1f;
        int manualStarts=local.Starts, manualStops=local.Stops, manualHits=S.R7AimKillDispatches, manualPackets=GameFacade.SentCount, manualAims=local.Aims;
        Raw();
        Check(S.R7AKBranch=="manual-native" && local.Aims>manualAims && S.R7AKManualAims>0,"held trigger activates manual native aim branch");
        Check(local.Starts==manualStarts && local.Stops==manualStops && S.R7AimKillDispatches==manualHits && GameFacade.SentCount==manualPackets,"manual branch no synthetic firing/hit/sync calls");
        Check(object.ReferenceEquals(local.FDMIEDDNCEC,hit),"manual branch preserves native hit object");
        int slerps=Quaternion.Slerps;Time.Now+=0.06f;Raw();Check(Quaternion.Slerps==slerps,"aimbot smoothing does not affect AK manual aim");
        PlayerPrefs.SetFloat("OKMOD.AK.ManualSmooth.Float",6f);Time.Now+=0.06f;Raw();
        Check(Quaternion.Slerps>slerps,"AK manual smoothing independently applied");
        var applyManual=typeof(Okmod).GetMethod("ApplyAimKillManualAim",BindingFlags.NonPublic|BindingFlags.Static);
        Physics.Obstacle=new Collider { Tf=new Transform() };manualAims=local.Aims;
        applyManual.Invoke(null,null);Check(local.Aims==manualAims && S.R7AKManualTarget==null,"new cover invalidates cached manual target immediately");
        Physics.Obstacle=null;Time.Now+=0.06f;Raw();
        local.Reloading=true;manualAims=local.Aims;Time.Now+=0.06f;Raw();
        Check(local.Aims==manualAims && S.R7AKManualTarget==null && local.Stops==manualStops,"reload suppresses manual assistance without releasing native trigger");
        local.Reloading=false;Time.Now+=0.06f;Raw();
        local.Firing=false;Raw();Check(S.R7AKBranch=="handoff" && S.R7AimKillDispatches==manualHits,"release handoff does not immediately seize trigger");
        Time.Now+=0.1f;Raw();Check(S.R7AimKillDispatches==manualHits,"handoff grace preserved");
        Time.Now+=0.06f;Raw();Check(S.R7AimKillDispatches==manualHits+1,"auto resumes after native release grace");
        PlayerPrefs.SetString("OKMOD.AK.Auto","");Time.Now+=1;Raw();int autoOff=S.R7AimKillDispatches;
        local.Firing=true;Time.Now+=0.06f;manualAims=local.Aims;Raw();
        Check(local.Aims>manualAims && S.R7AimKillDispatches==autoOff,"manual assistance works with auto disabled");
        PlayerPrefs.SetString("OKMOD.AK.Manual","");Time.Now+=0.06f;manualAims=local.Aims;Raw();
        Check(local.Aims==manualAims && local.Stops==manualStops+1,"manual toggle off does not stop player's firing");
        PlayerPrefs.SetString("OKMOD.AK.Manual","X2");Time.Now+=0.06f;Raw();
        target.Head.Position=new Vector3(30,1,10);PlayerPrefs.SetFloat("OKMOD.AK.FOV.Float",20f);
        PlayerPrefs.SetString("OKMOD.Aim.AimKillFov","X2");Time.Now+=0.06f;manualAims=local.Aims;Raw();
        Check(local.Aims==manualAims,"AK own FOV excludes target despite aimbot FOV800");
        PlayerPrefs.SetFloat("OKMOD.AK.FOV.Float",400f);Time.Now+=0.06f;Raw();
        Check(local.Aims>manualAims,"AK own FOV change applies");
        PlayerPrefs.SetString("OKMOD.Aim.AimKillFov","");PlayerPrefs.SetFloat("OKMOD.AK.Range.Float",10f);
        Time.Now+=0.06f;manualAims=local.Aims;Raw();
        Check(local.Aims==manualAims,"AK own range excludes target despite aimbot range500");
        target.Head.Position=new Vector3(0,1,10);PlayerPrefs.SetFloat("OKMOD.AK.Range.Float",75f);Time.Now+=0.06f;Raw();
        PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");manualAims=local.Aims;
        applyManual.Invoke(null,null);Check(local.Aims==manualAims && local.Stops==manualStops+1,"recovery never changes held native trigger");
        PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");Time.Now+=0.06f;Raw();
        local.Weapon=new HBIBDMMOOOK();manualAims=local.Aims;applyManual.Invoke(null,null);
        Check(local.Aims==manualAims && S.R7AKManualTarget==null,"weapon switch rejects manual cache without changing fire state");
        local.Weapon=fastWeapon;Time.Now+=0.06f;Raw();Time.Now+=0.21f;manualAims=local.Aims;applyManual.Invoke(null,null);
        Check(local.Aims==manualAims && S.R7AKManualTarget==null,"stale manual cache rejected");
        Time.Now+=0.06f;Raw();GameFacade.Match=null;manualAims=local.Aims;applyManual.Invoke(null,null);
        Check(local.Aims==manualAims && S.R7AKManualTarget==null,"match transition rejects manual cache");
        GameFacade.Match=new JMAGGLCNGIG { Players=new ArrayList { local,target } };
        Time.Now+=0.06f;Raw();target.Hp=0;manualAims=local.Aims;applyManual.Invoke(null,null);
        Check(local.Aims==manualAims && S.R7AKManualTarget==null,"dead target rejected without stopping held trigger");
        local.Firing=false;enabled.Invoke(null,new object[]{false});
        int beforeTransport=S.R7AimKillDispatches;
   var send=typeof(Okmod).GetMethod("SendAimKillOnline",BindingFlags.NonPublic|BindingFlags.Static);
   var poll=typeof(Okmod).GetMethod("PollAimKillOnline",BindingFlags.NonPublic|BindingFlags.Static);
   S.R7OnlineReqA=null;S.R7OnlineCount=0;
   send.Invoke(null,new object[]{"man",true});
   var req=UnityEngine.Networking.UnityWebRequest.Last;
   Check(req.Url.StartsWith("https://okmod.sbs/aimlog?aimkill=1&"),"direct nginx transport");
   Check(S.R7OnlineState==1 && S.R7OnlineCount==0,"started is not acknowledged");
   send.Invoke(null,new object[]{"man",true});
   Check(object.ReferenceEquals(req,UnityEngine.Networking.UnityWebRequest.Last) && !req.Disposed,"in-flight request preserved");
    req.Done=true;req.Code=204;poll.Invoke(null,null);
   Check(S.R7OnlineState==2 && S.R7OnlineCount==1 && req.Disposed && S.R7OnlineReqA==null,"204 acknowledges and disposes");
    Time.Now+=2;send.Invoke(null,new object[]{"man",true});req=UnityEngine.Networking.UnityWebRequest.Last;
   req.Done=true;req.Code=500;req.Error="server failure";string prior=S.R7AimKillError;poll.Invoke(null,null);
   Check(S.R7OnlineState==-2 && S.R7OnlineCount==1 && S.R7AimKillError==prior,"transport error does not acknowledge or overwrite game error");
   var encoded=(string)typeof(Okmod).GetMethod("OnlineEncode",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{"lỗi +&😀"});
   Check(Uri.UnescapeDataString(encoded.Replace("+"," "))=="lỗi +&😀","UTF-8 query roundtrip");
     Check(S.R7AimKillDispatches==beforeTransport,"transport does not dispatch damage");
     Check(PlayerNetwork.ExplicitCalls==0,"no direct TakeDamage duplicates in engine mode");
     Console.WriteLine("PASS: BS4484/PC engine-entry flow, native trigger preservation, recovery, cover, probes and HTTPS transport");
 }
 static void Main() {
   var local=Enemy(0);local.Local=true;local.Weapon=new HBIBDMMOOOK { NativeInterval=1.82f,ODFDGMOCJLL=444 };
   var enemy=Enemy(100);GameFacade.Local=local;
   GameFacade.Match=new JMAGGLCNGIG { Players=new ArrayList { local,enemy } };
   var nativeHit=new CGKJLKPMGDJ { OLGEMBBDBID=37 };local.FDMIEDDNCEC=nativeHit;
   var set=typeof(Okmod).GetMethod("SetAimKillEnabled",BindingFlags.NonPublic|BindingFlags.Static);
   var range=typeof(Okmod).GetMethod("AimKillRange",BindingFlags.NonPublic|BindingFlags.Static);
   var point=typeof(Okmod).GetMethod("AimKillPointMode",BindingFlags.NonPublic|BindingFlags.Static);
   var cadence=typeof(Okmod).GetMethod("AimKillWeaponDelay",BindingFlags.NonPublic|BindingFlags.Static);
   PlayerPrefs.SetString("OKMOD.AK.Auto","");PlayerPrefs.SetString("OKMOD.AK.Manual","");
   PlayerPrefs.SetFloat("OKMOD.AK.Range.Float",10);PlayerPrefs.SetFloat("OKMOD.AK.FOV.Float",20);
   PlayerPrefs.SetInt("OKMOD.AK.Point",3);PlayerPrefs.SetFloat("OKMOD.AK.ManualSmooth.Float",20);
   PlayerPrefs.SetString("OKMOD.Aim.AimKillFov","X2");PlayerPrefs.SetString("OKMOD.Aim.Sync108","X2");
   PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay",3);PlayerPrefs.SetInt("OKMOD.Aim.Point",3);
   PlayerPrefs.SetFloat("OKMOD.Aim.Range.Float",10);PlayerPrefs.SetFloat("OKMOD.Aim.Smooth.Float",20);
   Check((float)range.Invoke(null,null)==500 && (int)point.Invoke(null,null)==0,"fixed head/range override stale options without mutating them");
   Check(Math.Abs((float)cadence.Invoke(null,new object[]{local.Weapon})-0.08f)<0.001,"fixed 80ms despite old delay and native observation");
   set.Invoke(null,new object[]{true});Tick();
   Check(S.R7AimKillDispatches==1 && local.Starts==1 && local.Stops==1 && !local.Firing,"one toggle activates owned auto shot");
   Check(local.Weapon.LastHit.DKPDBLLJEAA==LKOOALMKJND.Head,"fixed head hit despite old body preference");
   Time.Now+=0.06f;Raw();Check(S.R7AimKillDispatches==1,"no early repeated hit");
   Time.Now+=0.021f;Raw();Check(S.R7AimKillDispatches==2,"80ms deadline without native floor or probe gate");
   int hits=S.R7AimKillDispatches, starts=local.Starts, stops=local.Stops;
   local.Firing=true;Time.Now+=0.06f;int aims=local.Aims;Raw();
   Check(local.Aims>aims && S.R7AKBranch=="manual-native","fixed direct manual assistance despite old manual toggle off");
   Check(local.Starts==starts && local.Stops==stops && S.R7AimKillDispatches==hits && Quaternion.Slerps==0,"held trigger not seized and old smoothing ignored");
   Check(object.ReferenceEquals(local.FDMIEDDNCEC,nativeHit) && nativeHit.OLGEMBBDBID==37,"native hit remains unchanged");
   Physics.Obstacle=new Collider { Tf=new Transform() };Time.Now+=0.06f;aims=local.Aims;Raw();
   Check(local.Aims==aims && S.R7AKManualTarget==null,"cover still blocks manual aim");
   Physics.Obstacle=null;local.Firing=false;Raw();Time.Now+=0.1f;Raw();
   Check(S.R7AimKillDispatches==hits,"handoff retained");
   Time.Now+=0.06f;Raw();Check(S.R7AimKillDispatches==hits+1,"auto resumes after release");
   var syncPacket=GameFacade.SentMsg as message.EOBFCLNNIJH;
   Check(GameFacade.SentCount>=1 && GameFacade.SentId==108 && syncPacket!=null &&
         syncPacket.BEADLMGGGGL==7 && syncPacket.FIKKBFDEJOK==444 &&
         PlayerNetwork.ExplicitCalls==0,"reference 108 after cleanup, no explicit damage duplicates");
   Check(Trace.Steps.Contains("stop;aim;send108;"),"108 sent only after StopFire and aim restore");
   Check(S.R7Sync108Sent==1,"108 is once per weapon instance");
   PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");Tick();hits=S.R7AimKillDispatches;
   Tick();Check(S.R7AimKillDispatches==hits && S.R7AimKillStatus==8,"recovery still pauses fixed preset");
   PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");set.Invoke(null,new object[]{false});Tick();
   Check(S.R7AimKillStatus==0 && S.R7AimKillDispatches==hits,"one toggle off stops auto");
   Check(PlayerPrefs.GetInt("OKMOD.AK.Point",0)==3 && PlayerPrefs.GetFloat("OKMOD.AK.Range.Float",0)==10,"old settings preserved, not silently overwritten");
   set.Invoke(null,new object[]{true});Time.Now+=1;local.ThrowStart=true;
   int stopped=local.Stops;Raw();
   Check(local.Stops==stopped+1 && !local.Firing,"partial start still cleaned under fixed preset");
   local.ThrowStart=false;local.Weapon.ThrowHit=true;Time.Now+=2;stopped=local.Stops;Raw();
   Check(local.Stops==stopped+1 && !local.Firing,"engine error still cleaned under fixed preset");
   local.Weapon.ThrowHit=false;local.ThrowStop=true;Time.Now+=2;Raw();
   Check(S.R7AimShotFaulted,"cleanup fault remains latched");
   int started=local.Starts;Time.Now+=2;Raw();Check(local.Starts==started,"no automated retries after cleanup fault");
   var encoded=(string)typeof(Okmod).GetMethod("OnlineEncode",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{"lỗi +&😀"});
   Check(Uri.UnescapeDataString(encoded.Replace("+"," "))=="lỗi +&😀","UTF-8 transport unchanged");
   Console.WriteLine("PASS: single preset ignores old settings; head/500m/360/direct/80ms; native manual fire, cover, handoff, ref108-after and recovery");
 }
}
'''
    with tempfile.TemporaryDirectory(prefix='aimkill-tests-',dir='/tmp/opencode') as tmp:
        tmp=Path(tmp);(tmp/'stubs.cs').write_text(stubs);(tmp/'test.cs').write_text(harness)
        subprocess.run(['mcs','-out:'+str(tmp/'test.exe'),str(tmp/'stubs.cs'),str(ROOT/'okmod.cs'),
                        str(ROOT/'esp_sampled.cs'),str(tmp/'test.cs')],check=True)
        subprocess.run(['mono',str(tmp/'test.exe')],check=True)
    source=(ROOT/'okmod.cs').read_text()
    menu=source[source.index('private static void DrawAimMenu'):source.index('private static float HitFlyValue')]
    for removed in ('AimKillDelayRow(', 'AimKillPointRow(', 'OKMOD.AK.', 'OKMOD.Aim.Sync108', 'OKMOD.Aim.AimKillFov', 'JumpLogRow('):
        assert removed not in menu, removed


if __name__=='__main__':
    main()
