"""Exercise actual C# geometry using instrumented, local-only Unity stubs.

This does not emulate Unity/IFix or replace the on-device test.
Run build.py first so stubs.cs includes current injected fields.
"""
from pathlib import Path
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[1]


def replace_once(source, old, new):
    assert source.count(old) == 1, old
    return source.replace(old, new, 1)


def main():
    stubs = (ROOT / "stubs.cs").read_text()
    for old, new in (
        ("public static float Sin(float f) { return 0f; }", "public static float Sin(float f) { return (float)Math.Sin(f); }"),
        ("public static float Sqrt(float f) { return 0f; }", "public static float Sqrt(float f) { return (float)Math.Sqrt(f); }"),
        ("public static float Clamp(float v, float a, float b) { return v; }", "public static float Clamp(float v, float a, float b) { return Math.Max(a, Math.Min(b, v)); }"),
        ('public static float Distance(Vector3 a, Vector3 b) { return 0f; }', 'public static float Distance(Vector3 a, Vector3 b) { return (float)Math.Sqrt((a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y)+(a.z-b.z)*(a.z-b.z)); }'),
        ('public static Vector3 operator -(Vector3 a, Vector3 b) { return a; }', 'public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.x-b.x,a.y-b.y,a.z-b.z); }'),
        ('public static Vector3 Normalize(Vector3 a) { return a; }', 'public static Vector3 Normalize(Vector3 a) { float n=(float)Math.Sqrt(a.x*a.x+a.y*a.y+a.z*a.z); return n>0 ? new Vector3(a.x/n,a.y/n,a.z/n) : new Vector3(); }'),
        ("public static bool operator ==(Object a, Object b) { return false; }", "public static bool operator ==(Object a, Object b) { return ReferenceEquals(a, b); }"),
        ("public static bool operator !=(Object a, Object b) { return true; }", "public static bool operator !=(Object a, Object b) { return !ReferenceEquals(a, b); }"),
        ('public static Shader Find(string name) { return null; }', 'public static Shader Find(string name) { return new Shader(); }'),
        ('public GUIStyleState get_normal() { return null; }', 'public GUIStyleState get_normal() { return new GUIStyleState(); }'),
        ('public void set_vertices(Vector3[] v) {}', 'public Vector3[] vertices; public int uploads; public void set_vertices(Vector3[] v) { vertices = (Vector3[])v.Clone(); uploads++; }'),
        ('public void set_colors(Color[] v) {}', 'public Color[] colors; public void set_colors(Color[] v) { colors = (Color[])v.Clone(); }'),
        ('public void set_triangles(int[] v) {}', 'public int[] triangles; public void set_triangles(int[] v) { triangles = (int[])v.Clone(); }'),
        ('public Transform get_transform() { return null; }', 'public Transform TestTransform; public Transform get_transform() { return TestTransform; }'),
        ('public Collider get_collider() { return null; }', 'public Collider get_collider() { return Physics.TestCollider; }'),
        ('public class Physics {', 'public class Physics { public static bool TestHit; public static Collider TestCollider;'),
        ('hit = new RaycastHit(); return false;', 'hit = new RaycastHit(); return TestHit;'),
    ):
        stubs = replace_once(stubs, old, new)

    harness = r'''
using System;
using System.Reflection;
using UnityEngine;
using S = COW.GamePlay.SceneEditBoxSelectTool;
class VisualTests {
  static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
  static object Call(string name, params object[] args) {
    return typeof(SampledEsp).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, args);
  }
  static void Main() {
    Call("InitCache");
    Check(S.R7Skeleton.Length == 512 && S.R7SkeletonValid.Length == 256, "bone buffers");
    Check(S.R7BonePairs.Length == 30, "15 skeleton edges");
    foreach (int i in S.R7BonePairs) Check(i >= 0 && i < 16, "bone index");
    Call("DrawFov", 1920, 1080, null);
    Mesh ring = S.R8FovMesh;
    Check(ring != null && ring.vertices.Length == 384 && ring.triangles.Length == 576, "FOV topology");
    for (int i = 0; i < ring.vertices.Length; i++) {
      Vector3 v = ring.vertices[i];
      double r = Math.Sqrt((v.x-960)*(v.x-960)+(v.y-540)*(v.y-540));
      double expected = (i % 4 == 0 || i % 4 == 3) ? 218 : 222;
      Check(Math.Abs(r-expected) < 0.01, "FOV circular radius and 4px thickness");
    }
    int uploaded = ring.uploads;
    Call("DrawFov", 1920, 1080, null);
    Check(ring.uploads == uploaded, "FOV mesh reused");
    Call("DrawFov", 2560, 1440, null);
    Check(ring.uploads == uploaded + 1, "FOV viewport rebuild");
    Console.WriteLine("PASS: FOV circle, thickness, closure, cache and viewport rebuild");

    S.R7Width=1920; S.R7Height=1080;
    S.R7EnemyColor=new Color(1,0.3f,0.4f,1);
    S.R7TargetColor=new Color(1,0.8f,0.4f,1);
    float[] records = new float[80];
    bool[] knocked=new bool[16], team=new bool[16], target=new bool[16];
    for(int i=0;i<16;i++) {
      int a=i*5;
      records[a]=200+i*20; records[a+1]=200; records[a+2]=80; records[a+3]=180; records[a+4]=0.5f;
      S.R7Names[i]="TEST";
      S.R7HeadScreen[i*3]=records[a]+40; S.R7HeadScreen[i*3+1]=210; S.R7HeadScreen[i*3+2]=12;
      for(int j=0;j<16;j++) {
        int bone=i*16+j;
        S.R7Skeleton[bone*2]=(float)(j%4)/4;
        S.R7Skeleton[bone*2+1]=(float)(j/4)/4;
        S.R7SkeletonValid[bone]=true;
      }
    }
    S.R8MeshDirty=true;
    Check((bool)Call("DrawBatch",records,knocked,team,target,1,16),"line batch");
    Check(S.R8QuadCount == 1,"top line count");
    Vector3[] line = S.R8Mesh.vertices;
    Check(Math.Abs((line[0].x+line[3].x)*0.5f-960)<0.01,"line top center x");
    Check(Math.Abs((line[0].y+line[3].y)*0.5f)<0.01,"line top center y");
    Check(Math.Abs((line[1].y+line[2].y)*0.5f-200)<0.01,"line enemy endpoint");
    Console.WriteLine("PASS: ESP line starts at top-center, not bottom");

    S.R8MeshDirty=true;
    Check((bool)Call("DrawBatch",records,knocked,team,target,1,131072),"skeleton batch");
    Check(S.R8QuadCount==15,"all 15 skeleton edges");
    S.R8MeshDirty=true;
    Check((bool)Call("DrawBatch",records,knocked,team,target,1,65536),"head circle batch");
    Check(S.R8QuadCount==20,"head circle 20 segments");
    Console.WriteLine("PASS: cached skeleton edges and head-circle geometry");

    Quaternion q = new Quaternion(); q.w=1;
    Vector3 forward=(Vector3)Call("AimForward",q);
    Check(Math.Abs(forward.x)<0.001 && Math.Abs(forward.z-1)<0.001,"identity forward");
    q.y=(float)Math.Sqrt(0.5); q.w=q.y;
    forward=(Vector3)Call("AimForward",q);
    Check(Math.Abs(forward.x-1)<0.001 && Math.Abs(forward.z)<0.001,"90-degree forward");
    q.y*=2; q.w*=2;
    forward=(Vector3)Call("AimForward",q);
    Check(Math.Abs(forward.x-1)<0.001,"normalized quaternion");
    Vector2 radar=(Vector2)Call("RadarPoint",0f,100f,100f,100f,82f);
    Check(Math.Abs(radar.x-100)<0.001 && Math.Abs(radar.y-18)<0.001,"radar ahead");
    radar=(Vector2)Call("RadarPoint",1000f,0f,100f,100f,82f);
    Check(Math.Abs(radar.x-182)<0.001,"radar range clamp");
    Vector2 arrow=(Vector2)Call("ArrowAnchor",1920f,1080f,960f,2000f);
    Check(Math.Abs(arrow.x-960)<0.001 && Math.Abs(arrow.y-1052)<0.001,"behind/down arrow");
    Console.WriteLine("PASS: quaternion aim direction, radar projection/clamp, edge-arrow anchor");

    Transform enemyRoot=new Transform();
    Physics.TestHit=false;
    Check((int)Call("SampleVisibility",new Vector3(),new Vector3(0,0,10),enemyRoot,null)==1,"unobstructed ray");
    Physics.TestHit=true; Physics.TestCollider=new Collider(); Physics.TestCollider.TestTransform=new Transform();
    Check((int)Call("SampleVisibility",new Vector3(),new Vector3(0,0,10),enemyRoot,null)==-1,"wall ray");
    Physics.TestCollider.TestTransform=enemyRoot;
    Check((int)Call("SampleVisibility",new Vector3(),new Vector3(0,0,10),enemyRoot,null)==1,"target collider ray");
    Physics.TestCollider=null;
    Check((int)Call("SampleVisibility",new Vector3(),new Vector3(0,0,10),enemyRoot,null)==0,"invalid hit remains unknown");
    Console.WriteLine("PASS: line-of-sight clear/blocked/target/unknown states");

    S.R7VisibleColor=new Color(0,1,0,1); S.R7HiddenColor=new Color(1,0,0,1);
    S.R7Visibility[0]=1;
    Color visible=(Color)Call("EspColor",0,1048576,false,false,false);
    Check(visible.g==1 && visible.r==0,"visible palette");
    S.R7Visibility[0]=-1;
    Color hidden=(Color)Call("EspColor",0,1048576,false,false,false);
    Check(hidden.r==1 && hidden.g==0,"occluded palette");
    S.R8MeshDirty=true;
    Check((bool)Call("DrawBatch",records,knocked,team,target,1,16|2097152),"direct-only batch");
    Check(S.R8QuadCount==0,"occluded excluded");
    S.R7Visibility[0]=0; S.R8MeshDirty=true;
    Call("DrawBatch",records,knocked,team,target,1,16|2097152);
    Check(S.R8QuadCount==0,"unknown excluded");
    S.R7Visibility[0]=1; S.R8MeshDirty=true;
    Call("DrawBatch",records,knocked,team,target,1,16|2097152);
    Check(S.R8QuadCount==1,"clear enemy retained");
    records[4]=1; S.R8MeshDirty=true;
    Call("DrawBatch",records,knocked,team,target,1,4); int hpQuads=S.R8QuadCount;
    S.R8MeshDirty=true;
    Call("DrawBatch",records,knocked,team,target,1,4|67108864);
    Check(S.R8QuadCount==hpQuads+6,"7 HP segments use six separators");
    S.R7DirectionValid[0]=true;
    S.R7Directions[0]=0.5f; S.R7Directions[1]=0.1f; S.R7Directions[2]=1f; S.R7Directions[3]=0.4f;
    S.R8MeshDirty=true;
    Call("DrawBatch",records,knocked,team,target,1,4194304);
    Check(S.R8QuadCount==1,"enemy view line from cached points");
    Console.WriteLine("PASS: visibility colors, direct-only tri-state filter, HP separators, cached view line");

    S.R8MeshDirty=true;
    int all = 1|4|16|64|128|256|512|2048|4096|8192|65536|131072;
    Check((bool)Call("DrawBatch",records,knocked,team,target,16,all),"combined batch");
    Check(S.R8QuadCount <= 1024,"mesh capacity");
    foreach(int i in S.R8Mesh.triangles) Check(i >= 0 && i < S.R8Mesh.vertices.Length,"triangle bounds");
    Call("ClearContext");
    foreach(bool valid in S.R7SkeletonValid) Check(!valid,"skeleton clear");
    Console.WriteLine("PASS: combined feature capacity, triangle bounds, context cleanup");
  }
}
'''
    with tempfile.TemporaryDirectory(prefix="esp-visual-tests-", dir="/tmp/opencode") as tmp:
        tmp = Path(tmp)
        (tmp / "stubs.cs").write_text(stubs)
        (tmp / "test.cs").write_text(harness)
        subprocess.run(["mcs", "-out:" + str(tmp / "tests.exe"), str(tmp / "stubs.cs"),
                        str(ROOT / "esp_sampled.cs"), str(tmp / "test.cs")], check=True)
        subprocess.run(["mono", str(tmp / "tests.exe")], check=True)

    source = (ROOT / "okmod.cs").read_text()
    assert 'if (!open) {\n      SampledEsp.DrawEsp' not in source
    assert source.index('DrawEspLayer(w, h, tex);') < source.index('// ================= menu =================')
    assert 'OKMOD.Color.Name' in source
    assert 'return PlayerPrefs.GetString("OKMOD.Page.AIM", "") == "X2" ? 2304f : 2232f;' in source
    renderer = (ROOT / "esp_sampled.cs").read_text()
    assert 'StartsWith("[BOT]")' not in renderer
    assert 'Color nameText = S.R7NameColor;' in renderer
    assert 'player.IsVisible()' not in renderer
    assert 'CompactProjectedSlot(i, live);' in renderer
    assert 'S.R7Names[to] = S.R7Names[from];' in renderer
    assert 'S.R7LiveCount = live;' in renderer
    assert 'slot-based' not in renderer
    print("PASS: ESP behind menu, independent name color and scroll limits")


if __name__ == "__main__":
    main()
