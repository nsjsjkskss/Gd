"""Test staged force controller against explicit models, never native movement."""
import os
from pathlib import Path
import subprocess
import tempfile

from test_hitfly import recording_stubs, replace, ROOT


def main():
    stubs = recording_stubs()
    for old, new in (
        ('public static Vector3 operator +(Vector3 a, Vector3 b) { return a; }',
         'public static Vector3 operator +(Vector3 a,Vector3 b) { return new Vector3(a.x+b.x,a.y+b.y,a.z+b.z); }'),
        ('public UnityEngine.Vector3 GetVelocity() { return new UnityEngine.Vector3(); }',
         'public UnityEngine.Vector3 Baseline; public UnityEngine.Vector3 GetVelocity() { return Baseline; }'),
        ('public UnityEngine.Vector3 GetForceMoveVelocity() { return new UnityEngine.Vector3(); }',
         'public UnityEngine.Vector3 OtherForce,OwnForce; public UnityEngine.Vector3 GetForceMoveVelocity() { return new UnityEngine.Vector3(OtherForce.x+OwnForce.x,OtherForce.y+OwnForce.y,OtherForce.z+OwnForce.z); }'),
        ('public void SetForceMoveVelocity(UnityEngine.Vector3 value, uint key) {}',
         'public int ForceCalls; public bool ThrowApply,ThrowCleanup; public uint LastKey; public void SetForceMoveVelocity(UnityEngine.Vector3 value,uint key) { ForceCalls++;LastKey=key;bool zero=value.x==0 && value.y==0 && value.z==0;if(zero && ThrowCleanup)throw new Exception("cleanup");OwnForce=value;if(!zero && ThrowApply)throw new Exception("partial apply"); }'),
        ('public int GetPhysXState() { return 0; }',
         'public int MotionState; public int GetPhysXState() { return MotionState; }'),
        ('public static bool Loaded=true,Throw;',
         'public static bool Loaded=true,Throw,Obstacle;'),
        ('return Loaded && direction.y==-1 && hit.Depth>0 && hit.Depth<distance;',
         'return Obstacle || (Loaded && direction.y==-1 && hit.Depth>0 && hit.Depth<distance);'),
    ):
        stubs = replace(stubs, old, new)
    harness = r'''
using System;
using System.Reflection;
using UnityEngine;
using COW;
using COW.GamePlay;
using S=COW.GamePlay.SceneEditBoxSelectTool;
class GuidedTests {
 static void Check(bool ok,string text) { if(!ok)throw new Exception(text+" state="+S.R7HFGuideState+" reason="+S.R7HFMapReason); }
 static void Call(string name) { typeof(Okmod).GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null); }
 static Player local;
 static UIMapContentController content;
 static float Norm(Vector3 v) { return (float)Math.Sqrt(v.x*v.x+v.y*v.y+v.z*v.z); }
 static void Reset(float distance=60) {
   S.R7HFGuideForceOwned=false;S.R7HFGuideFault=false;S.R7HFGuideState=0;S.R7HFGuideOwner=null;S.R7HFGuideManualUntil=0;
   S.R7HFMapNext=0;S.R7HitFlyNext=0;S.R7HitFlyPending=false;S.R7HitFlyMapPending=false;
   PlayerPrefs.Strings.Clear();PlayerPrefs.Floats.Clear();Time.TestNow+=100;
   local=new Player { TestRoot=new Transform { TestPosition=new Vector3(0,20.2f,0) },Grounded=true };
   GameFacade.TestLocal=local;GameFacade.TestMatch=new JMAGGLCNGIG();
   content=new UIMapContentController { m_HasLocalMapMarkRealPos=true,m_LocalMapMarkRealPos=new Vector3(distance,999,0) };
   GameFacade.TestScene=new UIInGameScene { TestMap=new UIBigMapController { TestContent=content } };
   Physics.Throw=false;Physics.Obstacle=false;Physics.Loaded=true;Physics.GroundY=20;Physics.Surface=new Collider();
   Camera.TestCamera=new Camera { TestTransform=new Transform() };GameFacade.Throw=false;GameFacade.Result=true;
   S.R7GravityApplied=0;
 }
 static void Start() { Call("QueueMapHitFly");Call("UpdateHitFly");Check(S.R7HFGuideState==1,"guided start"); }
 static void Tick(float dt=0.04f) { Time.TestNow+=dt;Call("TickGuidedMap"); }
 static void Simulate(float speed,float distance,float rate) {
   Reset(distance);PlayerPrefs.SetFloat("OKMOD.Move.HFMapSpeed.Float",speed);
   local.Baseline=new Vector3(0,-1,0);local.OtherForce=new Vector3(0.3f,0,0.1f);
   int sends=GameFacade.Calls;Start();bool highBeforeTransit=false,braked=false,descended=false;
   float dt=1f/rate;
   for(int i=0;i<rate*90 && S.R7HFGuideState>=1 && S.R7HFGuideState<=4;i++) {
     var p=local.TestRoot.TestPosition;
     Time.TestNow+=dt;Call("TickGuidedMap");
     var f=local.OwnForce;Check(Norm(f)<=100.01f,"bounded force");
     var v=new Vector3(local.Baseline.x+local.OtherForce.x+f.x,local.Baseline.y+local.OtherForce.y+f.y,local.Baseline.z+local.OtherForce.z+f.z);
     if(S.R7HFGuideState==2)highBeforeTransit|=p.y>=54;
     if(S.R7HFGuideState==3)braked=true;
     if(S.R7HFGuideState==4)descended=true;
     local.TestRoot.TestPosition=new Vector3(p.x+v.x*dt,p.y+v.y*dt,p.z+v.z*dt);
     p=local.TestRoot.TestPosition;
     if(p.y<=20.2f) { p.y=20.2f;local.Grounded=true; } else local.Grounded=false;
     local.TestRoot.TestPosition=p;
   }
   Check(S.R7HFGuideState==5,"modeled flight completes");
   Check(highBeforeTransit && braked && descended,"ascent/transit/brake/descent phases");
   Check(Math.Abs(local.TestRoot.TestPosition.x-distance)<1 && Math.Abs(local.TestRoot.TestPosition.z)<1,"modeled endpoint tolerance");
   Check(Norm(local.OwnForce)==0 && !S.R7HFGuideForceOwned && S.R7HFGuideOwner==null,"own force released on modeled landing");
   Check(local.OtherForce.x==0.3f && local.OtherForce.z==0.1f,"other force untouched");
   Check(GameFacade.Calls==sends,"guided MAP emits no657 or fake sync");
 }
 static void Main() {
   int flights=0;
   foreach(float speed in new float[]{5,25,100})foreach(float distance in new float[]{8,60,150})foreach(float hz in new float[]{30,60}) {
     Simulate(speed,distance,hz);flights++;
   }
   Reset();Start();Tick();PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");Call("UpdateHitFly");
   Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"recovery releases captured force");
   Reset();Start();Tick();local.TestHp=0;Tick();Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"death cleanup");
   Reset();Start();Tick();var old=local;GameFacade.TestLocal=new Player();Tick();Check(old.OwnForce.y==0 && S.R7HFGuideOwner==null,"replacement cleans original player only");
   Reset();Start();Tick();GameFacade.TestMatch=new JMAGGLCNGIG();Tick();Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"match transition cleanup");
   Reset();Start();Tick();Time.TestNow+=0.5f;Call("TickGuidedMap");Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"stalled frame cleanup");
   Reset();Start();Tick();Time.TestNow-=1f;Call("TickGuidedMap");Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"clock reversal cleanup");
   Reset();Start();Tick();local.TestRoot.TestPosition=new Vector3(500,500,500);Tick();Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"position correction cancels");
   Reset();Start();local.MotionState=5;Tick();Check(S.R7HFGuideState==6,"game hitfly mode cancels trial");
   Reset();Start();Tick();Call("QueueMapHitFly");Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0 && !S.R7HitFlyMapPending,"second MAP press cancels not relaunches");
   Reset();Start();Tick();content.m_LocalMapMarkRealPos=new Vector3(900,999,900);Tick();Check(S.R7HitFlyMapTarget.x==60 && S.R7HitFlyMapTarget.z==0,"moving marker cannot retarget in-flight trip");
   Reset();Start();Tick();PlayerPrefs.SetString("OKMOD.Move.Hover","X2");Tick();Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"new gravity mode cancels and releases");
   Reset();Start();Tick();local.Baseline=new Vector3(float.NaN,0,0);Tick();Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"NaN native velocity never reaches setter");
   Reset();Start();Tick();Physics.Obstacle=true;Tick();Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"obstruction cancels rather than pushing through");
   Reset();Start();Tick();Physics.Throw=true;Tick();Check(S.R7HFGuideState==7 && Norm(local.OwnForce)==0,"physics binding exception releases own force");
   Reset();Start();for(int i=0;i<90 && S.R7HFGuideState==1;i++)Tick();Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"unresponsive actuator no-progress cancellation");
   Reset();Start();Tick();S.R7HFGuideState=4;Physics.Loaded=false;Tick();Check(S.R7HFGuideState==6 && Norm(local.OwnForce)==0,"lost landing surface cancels descent");
   Reset();Start();local.ThrowApply=true;Tick();Check(S.R7HFGuideState==7 && Norm(local.OwnForce)==0 && !S.R7HFGuideFault,"partial apply cleaned");
   Reset();Start();Tick();local.ThrowCleanup=true;PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","X2");Call("UpdateHitFly");
   Check(S.R7HFGuideFault && S.R7HFGuideState==9,"cleanup failure latched");int calls=local.ForceCalls;
   PlayerPrefs.SetString("OKMOD.Diagnostic.NativeRecovery","");Call("QueueMapHitFly");Call("UpdateHitFly");
   Check(local.ForceCalls==calls && S.R7HFGuideFault,"faulted cleanup never automatically retried or replaced");
   Reset();Physics.Loaded=false;Call("QueueMapHitFly");Call("UpdateHitFly");Check(S.R7HFMapStatus==10 && local.ForceCalls==0,"unloaded landing surface rejected");
   Reset();local.Grounded=false;Call("QueueMapHitFly");Call("UpdateHitFly");Check(local.ForceCalls==0 && S.R7HFMapStatus==15,"airborne start rejected");
   Reset();Start();int hs=S.R7HFMapSent;Call("QueueHitFly");Call("UpdateHitFly");Check(S.R7HitFlyStatus==15 && S.R7HFMapSent==hs,"manual HF cannot overwrite guided owner");
   Reset();Call("QueueHitFly");Call("UpdateHitFly");Call("QueueMapHitFly");Call("UpdateHitFly");Check(S.R7HFGuideState==0 && S.R7HFMapStatus==15,"manual impulse lifetime blocks guided takeover");
   Console.WriteLine("PASS: "+flights+" IDEAL-ADDITIVE-MODEL flights + phase/cleanup/fault/ownership regressions. NOT native/device/server validation.");
 }
}
'''
    with tempfile.TemporaryDirectory(prefix='guided-map-', dir='/tmp/opencode') as tmp:
        tmp = Path(tmp)
        (tmp/'stubs.cs').write_text(stubs)
        (tmp/'test.cs').write_text(harness)
        subprocess.run(['mcs','-define:HF_GUIDED_TRIAL','-out:'+str(tmp/'test.exe'),str(tmp/'stubs.cs'),
                        str(ROOT/'okmod.cs'),str(ROOT/'esp_sampled.cs'),str(tmp/'test.cs')],check=True)
        subprocess.run(['mono',str(tmp/'test.exe')],check=True)


if __name__ == '__main__':
    main()
