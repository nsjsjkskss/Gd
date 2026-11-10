"""Compare C# packet parameters with the pinned Sophia VM transcription.

No network or physics execution. Observation tests manually set positions.
"""
from pathlib import Path
import sys
import subprocess
import tempfile

from test_hitfly import recording_stubs, replace, ROOT

sys.path.insert(0, str(ROOT.parent / 'buildkit/audit'))
from sophia_hitfly_reference import model, collect


def main():
    collect(Path('/opt/okmod-1300/var/runtime.bundle'))
    stubs = recording_stubs()
    stubs = stubs.replace('public int GetPhysXState() { return 0; }',
        'public int MotionState; public int GetPhysXState() { return MotionState; }', 1)
    stubs = stubs.replace(
        'public float SnowSlideWayFallingGravity;',
        'public float SnowSlideWayFallingGravity; public float NativeGravity=-50; public bool ThrowGravity;', 1)
    stubs = stubs.replace(
        'public float get_FallingGravity() { return 0f; }',
        'public float get_FallingGravity() { if(ThrowGravity)throw new Exception("gravity");return NativeGravity; }', 1)
    cases = []
    for distance in (2, 8, 100, 300, 900):
        for dy in (-100, 0, 40, 300):
            for g in (5, 9.8, 50, 160):
                for scale in (0.5, 1, 1.5):
                    expected = model(distance, dy, g)
                    cases.append(f'Case({distance}f,{dy}f,{g}f,{scale}f,{expected["estimated_time"]:.9f}f,{expected["packet_time"]:.9f}f,{expected["packet_xz"]*scale:.9f}f);')
    harness = r'''
using System;
using System.Reflection;
using UnityEngine;
using COW;
using COW.GamePlay;
using S=COW.GamePlay.SceneEditBoxSelectTool;
class SophiaTests {
 static void Check(bool ok,string text) { if(!ok)throw new Exception(text+" status="+S.R7HFMapStatus+" reason="+S.R7HFMapReason); }
 static void Call(string name) { typeof(Okmod).GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null); }
 static void Case(float d,float dy,float g,float scale,float t,float duration,float xz) {
   object[] args={new Vector3(),new Vector3(d,dy,0),g,scale,0f,0f,0f};
   bool ok=(bool)typeof(Okmod).GetMethod("SophiaMapParameters",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);
   Check(ok==(xz<=500),"explicit application bound, no silent clamp");
   Check(Math.Abs((float)args[4]-t)<0.001 && Math.Abs((float)args[5]-duration)<0.001 && Math.Abs((float)args[6]-xz)<0.002,"C# vs pinned VM formula");
 }
 static Player local;static UIMapContentController content;
 static void Reset() {
   Time.TestNow+=100;PlayerPrefs.Strings.Clear();PlayerPrefs.Floats.Clear();
   GameFacade.TestMatch=new JMAGGLCNGIG();
   local=new Player { TestRoot=new Transform { TestPosition=new Vector3(10,20,30) } };GameFacade.TestLocal=local;
   content=new UIMapContentController { m_HasLocalMapMarkRealPos=true,m_LocalMapMarkRealPos=new Vector3(310,20,30) };
   GameFacade.TestScene=new UIInGameScene { TestMap=new UIBigMapController { TestContent=content } };
   Camera.TestCamera=new Camera { TestTransform=new Transform { TestForward=new Vector3(0,0,1) } };
   S.R7HFMapState=0;S.R7HFMapNext=0;S.R7HitFlyNext=0;S.R7HitFlyPending=false;S.R7HitFlyMapPending=false;
   GameFacade.Throw=false;GameFacade.ThrowNext=false;GameFacade.Result=true;Physics.Throw=true;
 }
 static void Shot() { Call("QueueMapHitFly");Call("UpdateHitFly"); }
 static void StopObservation() { PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");Call("UpdateHitFly");PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");Time.TestNow+=10; }
 static void Main() {
   __CASES__
   Reset();int n=GameFacade.Calls,hs=S.R7HitFlyStatus,ha=S.R7HitFlyAttempts;
   PlayerPrefs.SetFloat("OKMOD.Move.HitFlyTime.Float",3);PlayerPrefs.SetFloat("OKMOD.Move.HitFlyXZ.Float",1);PlayerPrefs.SetFloat("OKMOD.Move.HitFlyY.Float",1);
   PlayerPrefs.SetFloat("OKMOD.Move.HFMapSpeed.Float",5);PlayerPrefs.SetFloat("OKMOD.Move.HFMapHeight.Float",200);
   Camera.TestCamera=null;Shot();var m=GameFacade.LastMessage;
   Check(GameFacade.Calls==n+1 && m.DCEMJOGHMEG==6 && Math.Abs(m.KJGOMDJCKEB-132)<0.01 && m.DPJAIECLHGL==150,"Sophia D300 g50 reference packet with fixed x1.1");
   Check(m.BEADLMGGGGL==12345 && m.MGJNBIJNLIJ!=null && GameFacade.LastId==657 && GameFacade.LastOption==2 && !GameFacade.LastCache,"typed ID/packer/transport");
   Check(m.JBEHMJFBEMG==null && m.IMAOCDIIIHI==0,"MAP optional fields stay constructor defaults");
   Check(S.R7HitFlyStatus==hs && S.R7HitFlyAttempts==ha,"MAP does not write manual counters/status");
   Check(message.LMCBHLCPAJD.LastDirection.x==1 && message.LMCBHLCPAJD.LastDirection.y==0,"horizontal direction");
   Shot();Check(GameFacade.Calls==n+1,"observation prevents repeated map launches");
   local.Grounded=false;local.TestRoot.TestPosition=new Vector3(100,80,30);Time.TestNow+=0.2f;Call("UpdateHitFly");
   Check(S.R7HFMapState==2 && S.R7HFMapApex==80,"read-only airborne/apex sample");
   content.m_LocalMapMarkRealPos=new Vector3(-999,999,-999);
   local.TestRoot.TestPosition=new Vector3(312,20,30);local.Grounded=true;
   for(int i=0;i<4;i++){Time.TestNow+=0.2f;Call("UpdateHitFly");}
   Check(S.R7HFMapState==3 && Math.Abs(S.R7HFMapMiss-2)<0.01 && S.R7HitFlyMapTarget.x==310,"frozen destination and stable landing miss");
   Check(GameFacade.Calls==n+1 && S.R7HFMapOwner==null,"observation sends no gameplay correction");
   Reset();local.NativeGravity=-9.8f;Shot();Check(GameFacade.LastMessage.DCEMJOGHMEG==6 && GameFacade.LastMessage.KJGOMDJCKEB>0 && GameFacade.LastMessage.DPJAIECLHGL==150,"native gravity9.8 accepted with fixed x1.1");
   foreach(float bad in new float[]{0,float.NaN,float.PositiveInfinity,200}) {
     Reset();local.NativeGravity=bad;Shot();Check(S.R7HFMapGravity==50,"invalid gravity fallback50");
   }
   Reset();local.ThrowGravity=true;Shot();Check(S.R7HFMapGravity==50 && S.R7HFMapStatus==5,"gravity exception isolated with fallback");
   Reset();PlayerPrefs.SetFloat("OKMOD.Move.HFMapScale.Float",0.5f);Shot();Check(Math.Abs(GameFacade.LastMessage.KJGOMDJCKEB-132)<0.01,"obsolete speed multiplier ignored, fixed x1.1");
   Reset();PlayerPrefs.SetFloat("OKMOD.Move.HFMapScale.Float",float.NaN);Shot();Check(Math.Abs(GameFacade.LastMessage.KJGOMDJCKEB-132)<0.01,"bad multiplier ignored, fixed x1.1");
   Reset();content.m_HasLocalMapMarkRealPos=false;content.m_LocalMapMarkController=new UIHudPlayerMarkController { m_pos=new Vector3(310,20,30) };Shot();Check(S.R7HFMapStatus==5,"native marker controller fallback");
   Reset();content.m_HasLocalMapMarkRealPos=false;n=GameFacade.Calls;Shot();Check(GameFacade.Calls==n && S.R7HFMapStatus==8,"no marker never falls back to unintended camera launch");
   Reset();content.m_LocalMapMarkRealPos=new Vector3(float.NaN,20,30);n=GameFacade.Calls;Shot();Check(GameFacade.Calls==n,"invalid marker rejected");
   Reset();content.m_LocalMapMarkRealPos=new Vector3(9000,20,30);n=GameFacade.Calls;Shot();Check(GameFacade.Calls==n && S.R7HFMapStatus==9,"over500 XZ application bound rejected");
   Reset();GameFacade.Result=false;Shot();Check(!SophiaActive() && S.R7HFMapStatus==6,"false send does not start observation");
   Reset();GameFacade.Throw=true;Shot();n=GameFacade.Calls;Call("UpdateHitFly");Check(S.R7HFMapStatus==7 && GameFacade.Calls==n,"throw consumed without repeat");
    Reset();Shot();Time.TestNow+=3.2f;Call("UpdateHitFly");Check(S.R7HFMapState==4 && S.R7HFMapReason=="no-airborne-sample-observed","Send true not mistaken for takeoff");
    Reset();Shot();local.MotionState=5;local.Grounded=true;local.TestRoot.TestPosition=new Vector3(10,40,30);Time.TestNow+=0.2f;Call("UpdateHitFly");
    Check(S.R7HFMapState==2 && S.R7HFSophiaAirSeen && S.R7HFSophiaPhysState==5,"native HitFly state counts as airborne when grounded getter lags");
   Reset();Shot();GameFacade.TestMatch=new JMAGGLCNGIG();Time.TestNow+=0.2f;Call("UpdateHitFly");Check(S.R7HFMapState==6,"match cancellation");
   Reset();Shot();local.TestHp=0;Call("UpdateHitFly");Check(S.R7HFMapState==6,"death cancellation");
   Reset();Shot();local.Grounded=false;Time.TestNow+=0.2f;Call("UpdateHitFly");Time.TestNow+=36;Call("UpdateHitFly");Check(S.R7HFMapState==5,"bounded observation timeout");
   Reset();Shot();Time.TestNow-=1;Call("UpdateHitFly");Check(S.R7HFMapState==5,"clock reversal");
   Reset();Shot();Camera.TestCamera=new Camera { TestTransform=new Transform() };S.R7HitFlyNext=0;Call("QueueHitFly");Call("UpdateHitFly");
   Check(S.R7HFMapState==6 && S.R7HFMapReason=="observation-mixed-manual-hf" && S.R7HitFlyStatus==5,"manual HF works separately, invalidates attribution only");
   Reset();Camera.TestCamera=new Camera { TestTransform=new Transform() };Call("QueueHitFly");Call("UpdateHitFly");m=GameFacade.LastMessage;
   Check(m.DCEMJOGHMEG==1 && m.KJGOMDJCKEB==25 && m.DPJAIECLHGL==20 && m.JBEHMJFBEMG=="","ordinary HITFLY unchanged");
   Console.WriteLine("PASS: 240 C# vs pinned-Sophia parameter cases; packet/marker/gravity/speed isolation and read-only observation lifecycle. NOT device physics validation.");
 }
 static bool SophiaActive() { return (bool)typeof(Okmod).GetMethod("SophiaObservationActive",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null); }
}
'''.replace('__CASES__', '\n'.join(cases))
    with tempfile.TemporaryDirectory(prefix='sophia-map-', dir='/tmp/opencode') as tmp:
        tmp = Path(tmp)
        (tmp/'stubs.cs').write_text(stubs)
        (tmp/'test.cs').write_text(harness)
        subprocess.run(['mcs','-define:HF_SOPHIA_TRIAL','-out:'+str(tmp/'test.exe'),str(tmp/'stubs.cs'),
                        str(ROOT/'okmod.cs'),str(ROOT/'esp_sampled.cs'),str(tmp/'test.cs')],check=True)
        subprocess.run(['mono',str(tmp/'test.exe')],check=True)


if __name__ == '__main__':
    main()
