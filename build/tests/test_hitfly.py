"""Test the actual one-shot C# path with local, recording game stubs.

No network traffic is emitted. This does not validate server acceptance or
replace the IFix/device test. Run build.py first to generate stubs.cs.
"""
import subprocess
import os
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def replace(source, old, new):
    assert source.count(old) == 1, old
    return source.replace(old, new, 1)


def recording_stubs():
    artifacts = Path(os.environ.get('OKMOD_TEST_BUILD_DIR', str(ROOT)))
    stubs = (artifacts / 'stubs.cs').read_text()
    for old, new in (
        ('public static bool operator ==(Object a, Object b) { return false; }',
         'public static bool operator ==(Object a, Object b) { return ReferenceEquals(a,b); }'),
        ('public static bool operator !=(Object a, Object b) { return true; }',
         'public static bool operator !=(Object a, Object b) { return !ReferenceEquals(a,b); }'),
        ('public static float Sqrt(float f) { return 0f; }',
          'public static float Sqrt(float f) { return (float)Math.Sqrt(f); }'),
        ('public static float Clamp(float v, float a, float b) { return v; }',
         'public static float Clamp(float v,float a,float b) { return Math.Max(a,Math.Min(v,b)); }'),
        ('public Rect(float x, float y, float w, float h) {}',
         'public float x,y,width,height; public Rect(float x,float y,float w,float h) { this.x=x;this.y=y;width=w;height=h; }'),
        ('public bool Contains(Vector2 p) { return false; }',
         'public bool Contains(Vector2 p) { return p.x>=x && p.x<x+width && p.y>=y && p.y<y+height; }'),
        ('public GUIStyleState get_normal() { return null; }',
         'public GUIStyleState get_normal() { return new GUIStyleState(); }'),
        ('public static Vector3 Normalize(Vector3 a) { return a; }',
         'public static Vector3 Normalize(Vector3 a) { float n=(float)Math.Sqrt(a.x*a.x+a.y*a.y+a.z*a.z); return n>0 ? new Vector3(a.x/n,a.y/n,a.z/n) : new Vector3(); }'),
        ('public static float get_realtimeSinceStartup() { return 0f; }',
         'public static float TestNow=10; public static float get_realtimeSinceStartup() { return TestNow; }'),
        ('public static float GetFloat(string key, float value) { return value; }',
         'public static System.Collections.Generic.Dictionary<string,float> Floats = new System.Collections.Generic.Dictionary<string,float>(); public static float GetFloat(string key, float value) { return Floats.ContainsKey(key)?Floats[key]:value; }'),
        ('public static void SetFloat(string key, float value) {}',
         'public static void SetFloat(string key, float value) { Floats[key]=value; }'),
        ('public static string GetString(string k, string d) { return d; }',
         'public static System.Collections.Generic.Dictionary<string,string> Strings=new System.Collections.Generic.Dictionary<string,string>(); public static string GetString(string k,string d) { return Strings.ContainsKey(k)?Strings[k]:d; }'),
        ('public static void SetString(string k, string v) {}',
          'public static void SetString(string k,string v) { Strings[k]=v; }'),
        ('public bool IsGrounded() { return false; }',
         'public bool Grounded=true; public bool IsGrounded() { return Grounded; }'),
        ('public Collider get_collider() { return null; }',
         'public Collider Surface; public float Depth; public Collider get_collider() { return Surface; }'),
        ('public float get_distance() { return 0f; }',
         'public float get_distance() { return Depth; }'),
        ('public bool get_isTrigger() { return false; }',
         'public bool Trigger; public bool get_isTrigger() { return Trigger; }'),
        ('public class Physics {',
         'public class Physics { public static bool Loaded=true,Throw; public static float GroundY=20; public static Collider Surface=new Collider();'),
        ('hit = new RaycastHit(); return false;',
         'if(Throw) throw new Exception("ray error"); hit=new RaycastHit { Surface=Surface,Depth=origin.y-GroundY }; return Loaded && direction.y==-1 && hit.Depth>0 && hit.Depth<distance;'),
        ('public static Camera get_main() { return null; }',
         'public static Camera TestCamera; public static Camera get_main() { return TestCamera; }'),
        ('public Vector3 get_position() { return new Vector3(0,0,0); }',
         'public Vector3 TestPosition; public Vector3 get_position() { return TestPosition; }'),
        ('public UnityEngine.Transform get_RootTransform() { return null; }',
         'public UnityEngine.Transform TestRoot; public UnityEngine.Transform get_RootTransform() { return TestRoot; }'),
        ('public Transform get_transform() { return null; }',
         'public Transform TestTransform; public Transform get_transform() { return TestTransform; }'),
        ('public Vector3 get_forward() { return new Vector3(0, 0, 1); }',
         'public Vector3 TestForward=new Vector3(0,0,1); public Vector3 get_forward() { return TestForward; }'),
        ('public static COW.GamePlay.JMAGGLCNGIG CurrentMatch() { return null; }',
         'public static COW.GamePlay.JMAGGLCNGIG TestMatch; public static COW.GamePlay.JMAGGLCNGIG CurrentMatch() { return TestMatch; }'),
        ('public static COW.GamePlay.Player GetLocalPlayerOrObServer() { return null; }',
         'public static COW.GamePlay.Player TestLocal; public static COW.GamePlay.Player GetLocalPlayerOrObServer() { return TestLocal; }'),
        ('public static UIInGameScene CurrentInGameUIScene() { return null; }',
         'public static UIInGameScene TestScene; public static UIInGameScene CurrentInGameUIScene() { return TestScene; }'),
        ('public UIBigMapController GetBigMap() { return null; }',
         'public UIBigMapController TestMap; public UIBigMapController GetBigMap() { return TestMap; }'),
        ('public UIMapContentController CurrentActiveGetMapContentCtrl() { return null; }',
         'public UIMapContentController TestContent; public UIMapContentController CurrentActiveGetMapContentCtrl() { return TestContent; }'),
        ('public bool IsLocalPlayer() { return false; }',
         'public bool IsLocal=true; public bool IsLocalPlayer() { return IsLocal; }'),
        ('public BEADLMGGGGL get_PlayerID() { return new BEADLMGGGGL(); }',
         'public uint TestId=12345; public BEADLMGGGGL get_PlayerID() { return new BEADLMGGGGL { Value=TestId }; }'),
        ('public static uint DEIPJDIGKIP(BEADLMGGGGL value) { return 0; }',
         'public uint Value; public static uint DEIPJDIGKIP(BEADLMGGGGL value) { return value.Value; }'),
        ('public static BJFGLKKKOAN MGOGJCMCIPG(UnityEngine.Vector3 direction) { return null; }',
         'public static UnityEngine.Vector3 LastDirection; public static BJFGLKKKOAN MGOGJCMCIPG(UnityEngine.Vector3 direction) { LastDirection=direction; return new BJFGLKKKOAN(); }'),
        ('public static bool Send(uint id, GCommon.UDPClientMessageBase msg, byte option, bool cache) { return false; }',
          'public static int Calls; public static uint LastId; public static message.NNDOIBJBAJI LastMessage; public static byte LastOption; public static bool LastCache; public static bool Result=true; public static bool Throw; public static bool ThrowNext; public static bool Send(uint id, GCommon.UDPClientMessageBase msg, byte option, bool cache) { Calls++; LastId=id; LastMessage=(message.NNDOIBJBAJI)msg; LastOption=option; LastCache=cache; if(ThrowNext) { ThrowNext=false; throw new Exception("one shot error"); } if(Throw) throw new Exception("test send error"); return Result; }'),
    ):
        stubs = replace(stubs, old, new)
    # Player and vehicle have the same getter; only replace the Player region.
        start = stubs.index('public class Player : UnityEngine.MonoBehaviour')
    end = stubs.index('public class JMAGGLCNGIG', start)
    player = stubs[start:end]
    player = replace(player, 'public int get_CurHP() { return 0; }',
                     'public int TestHp=200; public int get_CurHP() { return TestHp; }')
    stubs = stubs[:start] + player + stubs[end:]
    return stubs


def main():
    stubs = recording_stubs()
    harness = r'''
using System;
using System.Reflection;
using UnityEngine;
using COW;
using COW.GamePlay;
using S=COW.GamePlay.SceneEditBoxSelectTool;
class HitFlyTests {
 static void Check(bool ok,string text) { if(!ok) throw new Exception(text); }
  static void Call(string name) { typeof(Okmod).GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null); }
 static string Label() { return (string)typeof(Okmod).GetMethod("HitFlyStatusLabel",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null); }
  static void Next() { Time.TestNow+=10; }
 static bool Solve(Vector3 start,Vector3 end,float speed,float height,float gravity,out float t,out float xz,out float vy) {
   object[] args={start,end,speed,height,gravity,0f,0f,0f};
   bool ok=(bool)typeof(Okmod).GetMethod("SolveHitFlyMapArc",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);
   t=(float)args[5];xz=(float)args[6];vy=(float)args[7];return ok;
 }
 static void CancelFlight() { PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");Call("UpdateHitFly");PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");Time.TestNow+=30; }
  static void HistoricalTrajectoryTests() {
   IPBGFJNGLOH.AMMOCNCMBBG=-9.8f;
  GameFacade.TestMatch=new JMAGGLCNGIG();
  GameFacade.TestLocal=new Player();
  Camera.TestCamera=new Camera { TestTransform=new Transform() };
  Camera.TestCamera.TestTransform.TestForward=new Vector3(0,0,2);
  Call("QueueHitFly"); Call("QueueHitFly"); Call("UpdateHitFly");
  Check(GameFacade.Calls==1,"one queue => one send");
  Check(GameFacade.LastId==657 && GameFacade.LastOption==2 && !GameFacade.LastCache,"BS4484 transport values");
  var m=GameFacade.LastMessage;
  Check(m.BEADLMGGGGL==12345,"unwrapped local player ID");
  Check(m.DCEMJOGHMEG==1 && m.KJGOMDJCKEB==25 && m.DPJAIECLHGL==20,"BS4484 defaults");
  Check(m.MGJNBIJNLIJ!=null && m.JBEHMJFBEMG=="" && m.IMAOCDIIIHI==0,"packet defaults");
  Check(Math.Abs(message.LMCBHLCPAJD.LastDirection.z-1)<0.001,"normalized camera direction");
  Call("UpdateHitFly"); Call("QueueHitFly"); Call("UpdateHitFly");
  Check(GameFacade.Calls==1 && S.R7HitFlyStatus==2,"no per-frame send, cooldown");
  Next(); PlayerPrefs.SetFloat("OKMOD.Move.HitFlyTime.Float",2);
  PlayerPrefs.SetFloat("OKMOD.Move.HitFlyXZ.Float",80); PlayerPrefs.SetFloat("OKMOD.Move.HitFlyY.Float",35);
  Call("QueueHitFly"); Call("UpdateHitFly");
  Check(GameFacade.Calls==2 && GameFacade.LastMessage.DCEMJOGHMEG==2 && GameFacade.LastMessage.KJGOMDJCKEB==80,"custom parameters");
  Next(); GameFacade.TestLocal.TestId=0; Call("QueueHitFly");Call("UpdateHitFly");
  Check(GameFacade.Calls==2 && S.R7HitFlyStatus==4 && !S.R7HitFlyPending,"invalid ID consumed without send");
  GameFacade.TestLocal.TestId=12345; Camera.TestCamera.TestTransform.TestForward=new Vector3(float.NaN,0,1);
  Call("QueueHitFly");Call("UpdateHitFly"); Check(GameFacade.Calls==2,"NaN direction rejected");
  Camera.TestCamera.TestTransform.TestForward=new Vector3(0,0,1);
  GameFacade.TestLocal.IsLocal=false;Call("QueueHitFly");Call("UpdateHitFly"); Check(GameFacade.Calls==2,"observer target rejected");
  GameFacade.TestLocal.IsLocal=true;GameFacade.TestLocal.TestHp=0;Call("QueueHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==2,"dead local rejected");
  GameFacade.TestLocal.TestHp=200;GameFacade.Result=false;Call("QueueHitFly");Call("UpdateHitFly");Check(S.R7HitFlyStatus==6,"false Send status");
  Next();GameFacade.Result=true;GameFacade.Throw=true;Call("QueueHitFly");Call("UpdateHitFly");
  Check(S.R7HitFlyStatus==7 && !S.R7HitFlyPending && S.R7HitFlyError.Length>0,"exception consumed and diagnosed");
  int calls=GameFacade.Calls;Call("UpdateHitFly");Check(GameFacade.Calls==calls,"exception does not retry");
   Console.WriteLine("PASS: Hitfly packet, local ID, direction, params, one-shot/cooldown, invalid inputs, false-send and exceptions");

  Next();GameFacade.Throw=false;
  GameFacade.TestLocal.TestRoot=new Transform { TestPosition=new Vector3(10,20,30) };
  GameFacade.TestScene=new UIInGameScene { TestMap=new UIBigMapController { TestContent=new UIMapContentController() } };
  int before=GameFacade.Calls;
  Call("QueueMapHitFly");Call("UpdateHitFly");
  Check(GameFacade.Calls==before && S.R7HitFlyStatus==8,"no marker => no send, no camera fallback");
  var content=GameFacade.TestScene.TestMap.TestContent;
   content.m_HasLocalMapMarkRealPos=true;content.m_LocalMapMarkRealPos=new Vector3(110,999,30);
   Physics.Loaded=false;Call("QueueMapHitFly");Call("UpdateHitFly");
   Check(GameFacade.Calls==before && S.R7HitFlyStatus==10,"unloaded surface rejected without marker-Y fallback");
   Physics.Loaded=true;Camera.TestCamera=null;
   Call("QueueMapHitFly");Call("UpdateHitFly");
  Check(GameFacade.Calls==before+1,"map request sent once");
  Check(Math.Abs(message.LMCBHLCPAJD.LastDirection.x-1)<0.001 && message.LMCBHLCPAJD.LastDirection.y==0 && Math.Abs(message.LMCBHLCPAJD.LastDirection.z)<0.001,"map bearing ignores camera and layer height");
   Check(Math.Abs(S.R7HitFlyMapDistance-100)<0.001 && Math.Abs(S.R7HitFlyMapTarget.y-20.2f)<0.001,"map distance and loaded surface height, not marker Y");
   m=GameFacade.LastMessage;
   Check(m.DCEMJOGHMEG>4 && m.KJGOMDJCKEB<=25 && m.DPJAIECLHGL>20,"loft increases time instead of overshooting horizontal speed");
   Check(Math.Abs(m.KJGOMDJCKEB*m.DCEMJOGHMEG-100)<0.01 && Math.Abs(20+m.DPJAIECLHGL*m.DCEMJOGHMEG-4.9*m.DCEMJOGHMEG*m.DCEMJOGHMEG-20.2)<0.01,"packet reaches destination in explicit ballistic model");
   Check(m.DPJAIECLHGL-9.8*m.DCEMJOGHMEG<0,"arrival on descending branch");
   Call("QueueMapHitFly");Call("QueueHitFly");Call("UpdateHitFly");
   Check(GameFacade.Calls==before+1,"active flight rejects both requeue paths");
   GameFacade.TestLocal.Grounded=false;Time.TestNow+=0.2f;
   GameFacade.TestLocal.TestRoot.TestPosition=new Vector3(40,50,30);Call("UpdateHitFly");
   Check(S.R7HFMapState==2 && S.R7HFMapApex==50,"airborne and apex observed without transform writes");
   content.m_LocalMapMarkRealPos=new Vector3(-900,0,-900);
   GameFacade.TestLocal.TestRoot.TestPosition=new Vector3(111,20.2f,30);GameFacade.TestLocal.Grounded=true;
   Time.TestNow+=1;Call("UpdateHitFly");Time.TestNow+=0.2f;Call("UpdateHitFly");
   Check(S.R7HFMapState==3 && Math.Abs(S.R7HFMapMiss-1)<0.01 && S.R7HFMapOwner==null,"two stable grounded samples observe miss against original destination");
   Check(S.R7HFMapReason=="landed-near-client-observation","near landing labeled observation, not server ACK");
   Check(GameFacade.Calls==before+1,"tracking does not send gameplay corrections");
   GameFacade.TestLocal.TestRoot.TestPosition=new Vector3(10,20,30);
  Next();content.m_LocalMapMarkRealPos=new Vector3(10,0,30);Call("QueueMapHitFly");Call("UpdateHitFly");
  Check(GameFacade.Calls==before+1 && S.R7HitFlyStatus==9,"near target => no send");
  content.m_LocalMapMarkRealPos=new Vector3(float.NaN,0,30);Call("QueueMapHitFly");Call("UpdateHitFly");
   Check(GameFacade.Calls==before+1,"NaN map target rejected");
   Console.WriteLine("PASS: marker/surface required, ballistic packet, camera-independent MAP, active queue isolation, landing observation");

   float t,xz,vy;
   int models=0;
   foreach(float distance in new float[]{2,10,30,100,300,800,2000})
    foreach(float dy in new float[]{-100,-10,0,10,100})
     foreach(float speed in new float[]{5,25,100})
      foreach(float height in new float[]{10,30,200}) {
       if(!Solve(new Vector3(0,0,0),new Vector3(distance,dy,0),speed,height,9.8f,out t,out xz,out vy)) continue;
       Check(t<=20 && vy<=100 && xz<=speed+0.001,"bounded model");
       Check(Math.Abs(xz*t-distance)<0.01 && Math.Abs(vy*t-4.9*t*t-dy)<0.01,"model endpoint equality");
       Check(vy-9.8*t<0 && vy*vy/19.6>=Math.Max(0,dy)+height-0.01,"descending arrival and requested minimum loft");
       models++;
      }
   Check(models>100,"parameter sweep covers many reachable arcs");
   foreach(float bad in new float[]{float.NaN,float.PositiveInfinity,-1,0,101})
    Check(!Solve(new Vector3(),new Vector3(100,0,0),bad,30,9.8f,out t,out xz,out vy),"invalid speed rejected");
   Check(!Solve(new Vector3(),new Vector3(100,float.NaN,0),25,30,9.8f,out t,out xz,out vy),"invalid endpoint rejected");
   Check(!Solve(new Vector3(),new Vector3(2000,0,0),5,30,9.8f,out t,out xz,out vy),"time out of bound rejected, not clamped into miss");
   Console.WriteLine("PASS: "+models+" ballistic-model cases, bounds/NaN/unreachable rejection (not native physics validation)");

   Next();content.m_LocalMapMarkRealPos=new Vector3(310,0,30);
   PlayerPrefs.SetFloat("OKMOD.Move.HFMapSpeed.Float",50);PlayerPrefs.SetFloat("OKMOD.Move.HFMapHeight.Float",10);
   Call("QueueMapHitFly");Call("UpdateHitFly");m=GameFacade.LastMessage;
   Check(Math.Abs(m.DCEMJOGHMEG-6)<0.01 && m.KJGOMDJCKEB==50,"independent adjustable map speed, old manual params ignored");
   CancelFlight();Call("QueueMapHitFly");Call("UpdateHitFly");Time.TestNow+=2.1f;Call("UpdateHitFly");
   Check(S.R7HFMapState==4 && S.R7HFMapReason=="no-takeoff-observed","send true not mistaken for actual takeoff");
   Next();PlayerPrefs.SetString("OKMOD.Move.Hover","X2");int n=GameFacade.Calls;
    Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n && S.R7HitFlyStatus==12,"hover conflict rejected without altering toggle");
    Check(S.R7HFMapReason=="hover-enabled" && Label().Contains("Bay lơ lửng") && S.R7HFNoticeUntil>Time.TestNow && S.R7HFMapDirty,"hover rejection visible and queued for telemetry even with AK off");
    Check(PlayerPrefs.GetString("OKMOD.Move.Hover","")=="X2","blocked launch does not disable user's hover setting");
   PlayerPrefs.SetString("OKMOD.Move.Hover","");PlayerPrefs.SetString("OKMOD.Move.HighJump","X2");
    Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n,"high jump conflict rejected");
    Check(S.R7HFMapReason=="high-jump-enabled" && Label().Contains("Nhảy cao"),"high jump exact actionable reason");
   PlayerPrefs.SetString("OKMOD.Move.HighJump","");GameFacade.TestLocal.Grounded=false;
    Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n,"airborne launch rejected");GameFacade.TestLocal.Grounded=true;
    Check(S.R7HFMapReason=="not-grounded" && Label().Contains("mặt đất"),"grounding failure distinct from gravity modes");
   Physics.Surface.Trigger=true;Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n,"trigger is not landing surface");Physics.Surface.Trigger=false;
   Physics.GroundY=2000;Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n,"surface outside ray bound rejected");Physics.GroundY=20;
   Physics.Throw=true;Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n && S.R7HitFlyStatus==7,"surface exception consumed without gameplay send");Physics.Throw=false;
   CancelFlight();GameFacade.Result=false;Call("QueueMapHitFly");Call("UpdateHitFly");
   Check(S.R7HFMapState==4 && S.R7HFMapOwner==null,"false send does not track flight");GameFacade.Result=true;
   Next();Call("QueueMapHitFly");Call("UpdateHitFly");GameFacade.TestLocal.Grounded=false;Time.TestNow+=0.2f;Call("UpdateHitFly");
   Time.TestNow+=S.R7HFMapTime+6;Call("UpdateHitFly");Check(S.R7HFMapState==5 && S.R7HFMapOwner==null,"airborne flight timeout bounded");
   GameFacade.TestLocal.Grounded=true;Next();Call("QueueMapHitFly");Call("UpdateHitFly");GameFacade.TestMatch=new JMAGGLCNGIG();Time.TestNow+=0.2f;Call("UpdateHitFly");
   Check(S.R7HFMapState==6 && S.R7HFMapOwner==null,"match transition cancels observation");
   Next();Call("QueueMapHitFly");Call("UpdateHitFly");GameFacade.TestLocal.TestHp=0;Time.TestNow+=0.2f;Call("UpdateHitFly");
   Check(S.R7HFMapState==6,"death cancels observation");GameFacade.TestLocal.TestHp=200;
   Next();Call("QueueMapHitFly");PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");n=GameFacade.Calls;Call("UpdateHitFly");
   Check(GameFacade.Calls==n && !S.R7HitFlyPending && !S.R7HitFlyMapPending && S.R7HitFlyStatus==13,"recovery consumes pending queue without delayed surprise launch");
   PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");
   Next();Call("QueueMapHitFly");Call("UpdateHitFly");
   var owner=GameFacade.TestLocal;GameFacade.TestLocal=new Player();Time.TestNow+=0.2f;Call("UpdateHitFly");
   Check(S.R7HFMapState==6,"local player replacement cancels observation");GameFacade.TestLocal=owner;
   Next();Call("QueueMapHitFly");Call("UpdateHitFly");GameFacade.TestLocal.TestRoot.TestPosition=new Vector3(float.NaN,0,0);
   Time.TestNow+=0.2f;Call("UpdateHitFly");Check(S.R7HFMapState==6,"NaN tracking position cancels");
   GameFacade.TestLocal.TestRoot.TestPosition=new Vector3(10,20,30);Next();GameFacade.Throw=true;
   Call("QueueMapHitFly");Call("UpdateHitFly");Check(S.R7HFMapState==7 && !S.R7HitFlyPending,"map send exception stops observation");GameFacade.Throw=false;
   CancelFlight();S.R7GravityApplied=1;int blocked=GameFacade.Calls;Call("QueueMapHitFly");Call("UpdateHitFly");
    Check(GameFacade.Calls==blocked && S.R7HitFlyStatus==12,"pending gravity restoration blocks launch");S.R7GravityApplied=0;
    Check(S.R7HFMapReason=="gravity-restoring" && Label().Contains("phục hồi"),"gravity restoration has own reason");
    Call("QueueMapHitFly");Call("UpdateHitFly");
    Check(GameFacade.Calls==blocked+1 && S.R7HFMapState==1,"same button launches once conflicts are cleared; no hidden disable latch");
    CancelFlight();
   var valueRow=typeof(Okmod).GetMethod("HitFlyValueRow",BindingFlags.NonPublic|BindingFlags.Static);
   PlayerPrefs.SetFloat("OKMOD.Move.HFMapSpeed.Float",25);PlayerPrefs.SetFloat("OKMOD.Move.HFMapHeight.Float",30);
   S.OkUiScroll=2376;
   valueRow.Invoke(null,new object[]{0,0,35,new Vector2(380,250),1,"OKMOD.Move.HFMapSpeed","MAP tốc tối đa m/s",25f,5f,100f,5f,new GUIStyle()});
   Check(PlayerPrefs.GetFloat("OKMOD.Move.HFMapSpeed.Float",0)==30,"visible MAP speed plus click changes own preference");
   PlayerPrefs.SetFloat("OKMOD.Move.HFMapSpeed.Float",100);
   valueRow.Invoke(null,new object[]{0,0,35,new Vector2(380,250),1,"OKMOD.Move.HFMapSpeed","MAP tốc tối đa m/s",25f,5f,100f,5f,new GUIStyle()});
   Check(PlayerPrefs.GetFloat("OKMOD.Move.HFMapSpeed.Float",0)==100,"speed UI clamps at max");
   valueRow.Invoke(null,new object[]{0,0,36,new Vector2(380,330),1,"OKMOD.Move.HFMapHeight","MAP độ vồng m",30f,10f,200f,10f,new GUIStyle()});
   Check(PlayerPrefs.GetFloat("OKMOD.Move.HFMapHeight.Float",0)==40 && PlayerPrefs.GetFloat("OKMOD.Move.HitFlyXZ.Float",0)==80,"loft UI changes independently of manual HITFLY speed");
   string report=(string)typeof(Okmod).GetMethod("BuildAimKillLog",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
   Check(report.IndexOf("hfMap status=")<500 && report.Contains("predictedOnly=1 serverAck=unobserved"),"HF telemetry near front survives online truncation and labels uncertainty");
   Console.WriteLine("PASS: map speed/loft UI, config isolation, no-takeoff/timeout/transition/death/recovery and surface errors");

 }
 static void Main() {
   GameFacade.TestMatch=new JMAGGLCNGIG();
   var local=new Player { TestRoot=new Transform { TestPosition=new Vector3(10,20,30) } };
   GameFacade.TestLocal=local;
   Camera.TestCamera=new Camera { TestTransform=new Transform { TestForward=new Vector3(0,0,2) } };
   Call("QueueHitFly");Call("QueueHitFly");Call("UpdateHitFly");
   Check(GameFacade.Calls==1 && GameFacade.LastId==657 && GameFacade.LastOption==2 && !GameFacade.LastCache,"one-shot original transport");
   var m=GameFacade.LastMessage;
   Check(m.DCEMJOGHMEG==1 && m.KJGOMDJCKEB==25 && m.DPJAIECLHGL==20 && m.BEADLMGGGGL==12345,"ordinary HITFLY default unchanged");
   Check(message.LMCBHLCPAJD.LastDirection.z==1 && m.IMAOCDIIIHI==0 && m.MGJNBIJNLIJ!=null,"packer and packet flags unchanged");
   Call("QueueHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==1,"cooldown retained");
   Next();PlayerPrefs.SetFloat("OKMOD.Move.HitFlyTime.Float",2);PlayerPrefs.SetFloat("OKMOD.Move.HitFlyXZ.Float",80);PlayerPrefs.SetFloat("OKMOD.Move.HitFlyY.Float",35);
   Call("QueueHitFly");Call("UpdateHitFly");m=GameFacade.LastMessage;
   Check(m.DCEMJOGHMEG==2 && m.KJGOMDJCKEB==80 && m.DPJAIECLHGL==35,"ordinary manual settings unchanged");
   Next();GameFacade.TestScene=new UIInGameScene { TestMap=new UIBigMapController { TestContent=new UIMapContentController() } };
   int n=GameFacade.Calls;Call("QueueMapHitFly");Call("UpdateHitFly");
    Check(GameFacade.Calls==n && S.R7HFMapStatus==8 && S.R7HFMapReason=="missing-marker","marker required without camera fallback");
   var content=GameFacade.TestScene.TestMap.TestContent;
   content.m_HasLocalMapMarkRealPos=true;content.m_LocalMapMarkRealPos=new Vector3(110,999,30);
   // Old blockers and experimental options must not affect the simplified route.
   PlayerPrefs.SetString("OKMOD.Move.HighJump","X2");PlayerPrefs.SetString("OKMOD.Move.Hover","X2");
   PlayerPrefs.SetFloat("OKMOD.Move.HFMapHeight.Float",200);S.R7GravityApplied=1;
   local.Grounded=false;local.InSnowSlideWayDashing=true;Physics.Throw=true;Physics.Loaded=false;Camera.TestCamera=null;
   Call("QueueMapHitFly");Call("QueueMapHitFly");Call("UpdateHitFly");m=GameFacade.LastMessage;
    Check(GameFacade.Calls==n+1 && S.R7HFMapStatus==5,"simple map launch no terrain/grounding/hover/jump/camera dependency");
   Check(m.DCEMJOGHMEG==1 && m.KJGOMDJCKEB==25 && m.DPJAIECLHGL==50,"short fixed impulse and upward boost, no ballistic time");
   Check(message.LMCBHLCPAJD.LastDirection.x==1 && message.LMCBHLCPAJD.LastDirection.y==0 && message.LMCBHLCPAJD.LastDirection.z==0,"map bearing uses marker XZ independently of camera and marker Y");
   Check(S.R7HFMapReason=="sent-bearing-up" && S.R7HFNoticeUntil>Time.TestNow,"accurate send status and visible notice");
   Check(PlayerPrefs.GetString("OKMOD.Move.Hover","")=="X2" && PlayerPrefs.GetString("OKMOD.Move.HighJump","")=="X2" && S.R7GravityApplied==1,"unrelated movement configuration not silently changed");
   Call("UpdateHitFly");Check(GameFacade.Calls==n+1,"no per-frame retries");
   foreach(float speed in new float[]{5,25,100}) {
     Next();PlayerPrefs.SetFloat("OKMOD.Move.HFMapSpeed.Float",speed);
     content.m_LocalMapMarkRealPos=new Vector3(6010,-9999,30);
     Call("QueueMapHitFly");Call("UpdateHitFly");m=GameFacade.LastMessage;
     Check(m.KJGOMDJCKEB==speed && m.DCEMJOGHMEG==1 && m.DPJAIECLHGL==50,"distance does not change speed/time/upward impulse");
   }
   foreach(float bad in new float[]{float.NaN,float.PositiveInfinity,0,101}) {
     Next();PlayerPrefs.SetFloat("OKMOD.Move.HFMapSpeed.Float",bad);Call("QueueMapHitFly");Call("UpdateHitFly");
     Check(GameFacade.LastMessage.KJGOMDJCKEB==25,"bad saved MAP speed falls back to25");
   }
   Next();n=GameFacade.Calls;content.m_LocalMapMarkRealPos=new Vector3(10,0,30);Call("QueueMapHitFly");Call("UpdateHitFly");
    Check(GameFacade.Calls==n && S.R7HFMapStatus==9,"near map marker rejected");
   content.m_LocalMapMarkRealPos=new Vector3(float.NaN,0,30);Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n,"NaN marker rejected");
    content.m_LocalMapMarkRealPos=new Vector3(110,0,30);local.TestId=0;Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n && S.R7HFMapStatus==4,"invalid player ID rejected");local.TestId=12345;
   local.IsLocal=false;Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n,"observer rejected");local.IsLocal=true;
   local.TestHp=0;Call("QueueMapHitFly");Call("UpdateHitFly");Check(GameFacade.Calls==n,"dead local rejected");local.TestHp=200;
    GameFacade.Result=false;Call("QueueMapHitFly");Call("UpdateHitFly");Check(S.R7HFMapStatus==6 && S.R7HFMapReason=="send-false","false Send diagnosed");GameFacade.Result=true;
    Next();GameFacade.Throw=true;Call("QueueMapHitFly");Call("UpdateHitFly");n=GameFacade.Calls;Call("UpdateHitFly");Check(S.R7HFMapStatus==7 && GameFacade.Calls==n && !S.R7HitFlyMapPending,"exception consumed without retry");GameFacade.Throw=false;
   Next();Call("QueueMapHitFly");PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");n=GameFacade.Calls;Call("UpdateHitFly");
    Check(GameFacade.Calls==n && S.R7HFMapStatus==13 && !S.R7HitFlyPending && !S.R7HitFlyMapPending,"recovery cancels pending launch");
   PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");Call("UpdateHitFly");Check(GameFacade.Calls==n,"no surprise launch after recovery off");
   var row=typeof(Okmod).GetMethod("HitFlyValueRow",BindingFlags.NonPublic|BindingFlags.Static);
   S.OkUiScroll=2304;PlayerPrefs.SetFloat("OKMOD.Move.HFMapSpeed.Float",25);
   row.Invoke(null,new object[]{0,0,35,new Vector2(380,320),1,"OKMOD.Move.HFMapSpeed","MAP tốc bay",25f,5f,100f,5f,new GUIStyle()});
   Check(PlayerPrefs.GetFloat("OKMOD.Move.HFMapSpeed.Float",0)==30,"speed plus button reachable");
    Check(PlayerPrefs.GetFloat("OKMOD.Move.HitFlyXZ.Float",0)==80,"MAP UI isolated from ordinary HITFLY");
    // Both queues survive either order; each status/cooldown/counter is independent.
    Camera.TestCamera=new Camera { TestTransform=new Transform { TestForward=new Vector3(0,0,1) } };
    Next();S.R7HitFlyNext=Time.TestNow+100;S.R7HFMapNext=0;
    int hfStatus=S.R7HitFlyStatus,hfAttempts=S.R7HitFlyAttempts;float hfNext=S.R7HitFlyNext;
    Call("QueueMapHitFly");Call("UpdateHitFly");
    Check(S.R7HFMapStatus==5 && S.R7HitFlyStatus==hfStatus && S.R7HitFlyAttempts==hfAttempts && S.R7HitFlyNext==hfNext,"manual cooldown/status/counter does not block or change MAP");
    S.R7HFMapNext=Time.TestNow+100;S.R7HitFlyNext=0;
    int mapStatus=S.R7HFMapStatus,mapAttempts=S.R7HFMapAttempts;float mapNext=S.R7HFMapNext;
    Call("QueueHitFly");Call("UpdateHitFly");
    Check(S.R7HitFlyStatus==5 && S.R7HFMapStatus==mapStatus && S.R7HFMapAttempts==mapAttempts && S.R7HFMapNext==mapNext,"MAP cooldown/status/counter does not block or change manual HF");
    foreach(bool mapFirst in new bool[]{true,false}) {
      S.R7HitFlyNext=0;S.R7HFMapNext=0;n=GameFacade.Calls;
      if(mapFirst) { Call("QueueMapHitFly");Call("QueueHitFly"); } else { Call("QueueHitFly");Call("QueueMapHitFly"); }
      Check(S.R7HitFlyPending && S.R7HitFlyMapPending,"both queues preserved");Call("UpdateHitFly");
      Check(GameFacade.Calls==n && !S.R7HitFlyPending && !S.R7HitFlyMapPending,"simultaneous motion requests consumed without stacking impulses");
      Check(S.R7HitFlyStatus==14 && S.R7HFMapStatus==14 && S.R7HitFlyNext==0 && S.R7HFMapNext==0,"explicit conflict leaves independent deadlines unchanged");
    }
    S.R7HitFlyNext=0;S.R7HFMapNext=0;Camera.TestCamera=null;hfAttempts=S.R7HitFlyAttempts;mapAttempts=S.R7HFMapAttempts;
    Call("QueueHitFly");Call("UpdateHitFly");Call("QueueMapHitFly");Call("UpdateHitFly");
    Check(S.R7HitFlyStatus==3 && S.R7HFMapStatus==5 && S.R7HFMapAttempts==mapAttempts+1 && S.R7HitFlyAttempts==hfAttempts,"manual camera error cannot suppress queued MAP");
    S.R7HitFlyNext=0;S.R7HFMapNext=0;Camera.TestCamera=new Camera { TestTransform=new Transform() };content.m_HasLocalMapMarkRealPos=false;
    Call("QueueMapHitFly");Call("UpdateHitFly");Call("QueueHitFly");Call("UpdateHitFly");
    Check(S.R7HFMapStatus==8 && S.R7HitFlyStatus==5,"missing MAP marker cannot suppress queued manual HF");
    S.R7HitFlyNext=0;S.R7HFMapNext=0;content.m_HasLocalMapMarkRealPos=true;GameFacade.ThrowNext=true;
    mapStatus=S.R7HFMapStatus;mapNext=S.R7HFMapNext;mapAttempts=S.R7HFMapAttempts;
    Call("QueueHitFly");Call("UpdateHitFly");
    Check(S.R7HitFlyStatus==7 && S.R7HFMapStatus==mapStatus && S.R7HFMapNext==mapNext && S.R7HFMapAttempts==mapAttempts,"manual exception cannot change MAP state/deadline/counter");
    Call("QueueMapHitFly");Call("UpdateHitFly");Check(S.R7HFMapStatus==5,"MAP still works after ordinary HF exception");
    S.R7HitFlyNext=0;S.R7HFMapNext=0;GameFacade.ThrowNext=true;hfStatus=S.R7HitFlyStatus;hfNext=S.R7HitFlyNext;hfAttempts=S.R7HitFlyAttempts;
    Call("QueueMapHitFly");Call("UpdateHitFly");
    Check(S.R7HFMapStatus==7 && S.R7HitFlyStatus==hfStatus && S.R7HitFlyNext==hfNext && S.R7HitFlyAttempts==hfAttempts,"MAP exception cannot change manual state/deadline/counter");
    Call("QueueHitFly");Call("UpdateHitFly");Check(S.R7HitFlyStatus==5,"manual HF still works after MAP exception");
    S.R7HitFlyNext=0;S.R7HFMapNext=0;Call("QueueHitFly");Call("QueueMapHitFly");PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");Call("UpdateHitFly");
    Check(S.R7HitFlyStatus==13 && S.R7HFMapStatus==13 && !S.R7HitFlyPending && !S.R7HitFlyMapPending,"global recovery cancels both independent queues");
    PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");
   string log=(string)typeof(Okmod).GetMethod("BuildAimKillLog",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
   Check(log.Contains("mode=bearing-up") && log.Contains("exactLanding=0") && !log.Contains("gModel="),"no obsolete predicted landing claims");
    Console.WriteLine("PASS: simple impulse regression; independent HF/MAP state and settings; simultaneous sends rejected in both orders; errors and recovery. Exact landing NOT validated.");
 }
}
'''
    with tempfile.TemporaryDirectory(prefix='hitfly-tests-', dir='/tmp/opencode') as tmp:
        tmp = Path(tmp)
        (tmp/'stubs.cs').write_text(stubs)
        (tmp/'test.cs').write_text(harness)
        subprocess.run(['mcs','-out:'+str(tmp/'test.exe'),str(tmp/'stubs.cs'),
                        str(ROOT/'okmod.cs'),str(ROOT/'esp_sampled.cs'),str(tmp/'test.cs')],check=True)
        subprocess.run(['mono',str(tmp/'test.exe')],check=True)
    source = (ROOT/'okmod.cs').read_text()
    assert 'IfixBaseTakeDamage' not in source and 'NoFallDamage(' not in source
    for removed in ('SetBigMapActive', 'UpdateHitFlyMapUI', 'HitFlyMapOpenPending', 'CHON MAP', 'Chọn map'):
        assert removed not in source, removed
    assert 'Rect fly = new Rect(px + 20f, y, 390f, 64f);' in source
    assert 'new Rect(140f, uiH / 2 + 84f, 620f, 52f), HitFlyMapStatusLabel()' in source
    print('PASS: no map-open/close controls, HF MAP full-row hitbox retained')
    print('PASS: rejected damage hook absent from candidate source')


if __name__ == '__main__':
    main()
