"""Exercise real HUD lifecycle with recording stubs, not a Unity device test."""
import subprocess
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def main():
    stubs = (ROOT / 'stubs.cs').read_text()
    replacements = (
        ('public static bool operator ==(Object a, Object b) { return false; }',
         'public static bool operator ==(Object a,Object b) { return ReferenceEquals(a,b); }'),
        ('public static bool operator !=(Object a, Object b) { return true; }',
         'public static bool operator !=(Object a,Object b) { return !ReferenceEquals(a,b); }'),
        ('public static Event get_current() { return null; }',
         'public static Event TestEvent=new Event(); public static Event get_current() { return TestEvent; }'),
        ('public int get_type() { return 0; }',
         'public int TestType=7; public int get_type() { return TestType; }'),
        ('public void Use() {}', 'public int Uses; public void Use() { Uses++; }'),
        ('public static int get_frameCount() { return 0; }',
         'public static int TestFrame=100; public static int get_frameCount() { return TestFrame; }'),
        ('public static int get_width() { return 0; }', 'public static int get_width() { return 1920; }'),
        ('public static int get_height() { return 0; }', 'public static int get_height() { return 1080; }'),
        ('public static GUISkin get_skin() { return null; }', 'public static GUISkin get_skin() { return new GUISkin(); }'),
        ('public Font get_font() { return null; }', 'public Font get_font() { return new Font(); }'),
        ('public GUIStyleState get_normal() { return null; }',
         'public GUIStyleState get_normal() { return new GUIStyleState(); }'),
        ('public static void Label(Rect r, string t, GUIStyle s) {}',
         'public static int Labels; public static System.Collections.Generic.List<string> Texts=new System.Collections.Generic.List<string>(); public static void Label(Rect r,string t,GUIStyle s) { Labels++; Texts.Add(t); }'),
        ('public Rect(float x, float y, float w, float h) {}',
         'public float x,y,width,height; public Rect(float x,float y,float w,float h) { this.x=x; this.y=y; width=w; height=h; }'),
        ('public bool Contains(Vector2 p) { return false; }',
         'public bool Contains(Vector2 p) { return p.x>=x && p.x<x+width && p.y>=y && p.y<y+height; }'),
        ('public static string systemCopyBuffer { get { return ""; } set {} }',
         'public static string systemCopyBuffer { get; set; }'),
        ('public static void DrawTexture(Rect r, Texture t) {}',
         'public static int Textures; public static void DrawTexture(Rect r,Texture t) { Textures++; }'),
        ('public static int get_touchCount() { return 0; }',
         'public static int Reads; public static int get_touchCount() { Reads++; return 0; }'),
        ('public static UIInGameScene CurrentInGameUIScene() { return null; }',
         'public static UIInGameScene TestScene; public static UIInGameScene CurrentInGameUIScene() { return TestScene; }'),
        ('public UIBigMapController GetBigMap() { return null; }',
         'public UIBigMapController TestMap; public bool ThrowMap; public UIBigMapController GetBigMap() { if(ThrowMap) throw new Exception("map missing"); return TestMap; }'),
        ('public EMapStateType get_MapState() { return EMapStateType.MiniMap; }',
         'public EMapStateType TestState; public bool ThrowState; public EMapStateType get_MapState() { if(ThrowState) throw new Exception("state missing"); return TestState; }'),
        ('public bool GetBigMapActive() { return false; }',
         'public bool TestActive; public bool GetBigMapActive() { return TestActive; }'),
        ('public static Camera get_main() { return null; }',
         'public static int Reads; public static Camera get_main() { Reads++; return null; }'),
        ('public static string GetString(string k, string d) { return d; }',
         'public static System.Collections.Generic.Dictionary<string,string> Values=new System.Collections.Generic.Dictionary<string,string>(); public static string GetString(string k,string d) { return Values.ContainsKey(k)?Values[k]:d; }'),
        ('public static void SetString(string k, string v) {}',
         'public static void SetString(string k,string v) { Values[k]=v; }'),
    )
    for old, new in replacements:
        assert stubs.count(old) == 1, old
        stubs = stubs.replace(old, new, 1)
    harness = r'''
using System;
using System.Reflection;
using UnityEngine;
using COW;
using S=COW.GamePlay.SceneEditBoxSelectTool;
class HudLifecycleTests {
 static void Check(bool value,string message) { if(!value) throw new Exception(message); }
 static void Main() {
  var scene=new UIInGameScene { TestMap=new UIBigMapController() };GameFacade.TestScene=scene;
  scene.TestState=EMapStateType.BigMap;scene.TestMap.TestActive=true;
  PlayerPrefs.SetString("Lam","");
  int draws=GUI.Labels+GUI.Textures;int input=Input.Reads;
  Okmod.Hud();
  Check(S.R7FeatureFrame==101,"features tick while native map open");
  Check(GUI.Labels+GUI.Textures>draws && Input.Reads>input,"native map retains OKMOD rendering/input");
  Check(S.R7Frame==101,"ESP ticks while native map and mod menu open");
  Check(Event.TestEvent.Uses==0,"no unrelated native-map event consumption");
  typeof(Okmod).GetMethod("UpdateFeaturesOnce",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
  int reads=Camera.Reads;
  typeof(Okmod).GetMethod("UpdateFeaturesOnce",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
  Check(Camera.Reads==reads,"feature tick once/frame");
  scene.TestState=EMapStateType.MiniMap;scene.TestMap.TestActive=false;Time.TestFrame++;
  Okmod.Hud();
  Check(S.R7FeatureFrame==102,"features tick with mod menu open");
  Check(S.R7Frame==102,"ESP entry ticks with mod menu open");
  Check(GUI.Labels+GUI.Textures>draws && Input.Reads>input,"menu drawing/input resumes after map closes");
  Event.TestEvent.TestType=0;Time.TestFrame++;Okmod.Hud();Check(S.R7FeatureFrame==102,"no update on mouse event");
  Event.TestEvent.TestType=7;PlayerPrefs.SetString("Lam","X2");Time.TestFrame++;Okmod.Hud();
  Check(S.R7FeatureFrame==104 && S.R7Frame==104,"closed mod menu still updates features/ESP");
  scene.TestState=EMapStateType.BigMap;scene.TestMap.TestActive=true;
  PlayerPrefs.SetString("OKMOD.Move.HitFlyMapButton","X2");Time.TestFrame++;
  int labels=GUI.Labels;Okmod.Hud();
  Check(GUI.Labels>=labels+2,"OKMOD icon and HF MAP drawn on native map");
  Check(S.R7FeatureFrame==105 && S.R7Frame==105,"feature and ESP lifetime unaffected by map");
   PlayerPrefs.SetString("OKMOD.Page.AIM","X2");
   var max=typeof(Okmod).GetMethod("MenuScrollMax",BindingFlags.NonPublic|BindingFlags.Static);
      Check((float)max.Invoke(null,null)==2304f,"AIM list reaches map speed row 35 without loft controls");
   PlayerPrefs.SetString("OKMOD.Page.AIM","");
   Check((float)max.Invoke(null,null)==2232f,"ESP swipe limit preserved");
   var actions=typeof(Okmod).GetMethod("DrawAimLogActions",BindingFlags.NonPublic|BindingFlags.Static);
   S.OkUiScroll=0;GUI.Texts.Clear();
   actions.Invoke(null,new object[]{0,0,new Vector2(),-1,new GUIStyle()});
   Check(GUI.Texts.Contains("LOG") && GUI.Texts.Contains("COPY") && GUI.Texts.Contains("GỬI"),"log actions visible at top without scrolling");
   S.OkUiScroll=2664;S.R7LogOpen=false;
   actions.Invoke(null,new object[]{0,0,new Vector2(480,490),1,new GUIStyle()});
   Check(S.R7LogOpen,"fixed LOG click opens overlay at end of list");
   actions.Invoke(null,new object[]{0,0,new Vector2(590,490),1,new GUIStyle()});
   Check(S.R7AimKillCopyOk && GUIUtility.systemCopyBuffer.Contains("pc=") && GUIUtility.systemCopyBuffer.Contains("online="),"fixed COPY writes diagnostic log");
    var ak=typeof(Okmod).GetMethod("DrawAimKillHeader",BindingFlags.NonPublic|BindingFlags.Static);
    GUI.Texts.Clear();ak.Invoke(null,new object[]{0,0,new Vector2(),-1,new GUIStyle()});
    Check(GUI.Texts.Contains("AK OFF"),"AimKill header visible without scrolling");
    ak.Invoke(null,new object[]{0,0,new Vector2(380,30),1,new GUIStyle()});
    Check(PlayerPrefs.GetString("OKMOD.Aim.AimKill","")=="X2","header turns AimKill ON");
    ak.Invoke(null,new object[]{0,0,new Vector2(380,30),1,new GUIStyle()});
    Check(PlayerPrefs.GetString("OKMOD.Aim.AimKill","")=="","header turns AimKill OFF");
    Console.WriteLine("PASS: HUD lifecycle; scroll; LOG/COPY; visible AimKill header toggles ON/OFF");
 }
}
'''
    with tempfile.TemporaryDirectory(prefix='hud-lifecycle-', dir='/tmp/opencode') as tmp:
        tmp = Path(tmp)
        (tmp/'stubs.cs').write_text(stubs)
        (tmp/'test.cs').write_text(harness)
        subprocess.run(['mcs','-out:'+str(tmp/'test.exe'),str(tmp/'stubs.cs'),
                        str(ROOT/'okmod.cs'),str(ROOT/'esp_sampled.cs'),str(tmp/'test.cs')],check=True)
        subprocess.run(['mono',str(tmp/'test.exe')],check=True)
    source = (ROOT/'okmod.cs').read_text()
    assert 'NativeMapOpen' not in source
    assert 'SetBigMapActive' not in source


if __name__ == '__main__':
    main()
