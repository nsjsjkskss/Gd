// Stub API game (chi de mcs sinh MemberRef dung ten -> converter tra ra type/extern cua patch)
using System;

namespace UnityEngine {
  public struct Rect {
    public Rect(float x, float y, float w, float h) {}
    public bool Contains(Vector2 p) { return false; }
  }
  public struct Color { public Color(float r, float g, float b, float a) {} }
  public struct Vector2 { public float x, y; public Vector2(float x, float y) { this.x = x; this.y = y; } }
  public struct Vector3 {
    public float x, y, z;
    public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
    public static float Distance(Vector3 a, Vector3 b) { return 0f; }
    public static Vector3 operator -(Vector3 a, Vector3 b) { return a; }
    public static Vector3 operator +(Vector3 a, Vector3 b) { return a; }
    public static Vector3 Normalize(Vector3 a) { return a; }
    public static Vector3 get_zero() { return new Vector3(0,0,0); }
    public static Vector3 get_one() { return new Vector3(1,1,1); }
  }
  public struct Matrix4x4 {
    public static Matrix4x4 TRS(Vector3 p, Quaternion q, Vector3 s) { return new Matrix4x4(); }
    public static Matrix4x4 get_identity() { return new Matrix4x4(); }
  }
  public struct Quaternion {
    public static float Angle(Quaternion a, Quaternion b) { return 0f; }
    public static Quaternion LookRotation(Vector3 forward) { return new Quaternion(); }
    public static Quaternion Slerp(Quaternion a, Quaternion b, float t) { return b; }
    public static Quaternion get_identity() { return new Quaternion(); }
    public static Quaternion Euler(float x, float y, float z) { return new Quaternion(); }
  }
  public class Mathf {
    public static float Exp(float value) { return 0f; }
    public static float Sin(float f) { return 0f; }
    public static float Sqrt(float f) { return 0f; }
    public static float Atan2(float y, float x) { return 0f; }
    public static float Abs(float f) { return f; }
    public static float Clamp(float v, float a, float b) { return v; }
  }
  public class Object {
    public string get_name() { return null; }
    public static bool operator ==(Object a, Object b) { return false; }
    public static bool operator !=(Object a, Object b) { return true; }
    public override bool Equals(object obj) { return base.Equals(obj); }
    public override int GetHashCode() { return base.GetHashCode(); }
  }
  public class GameObject : Object {
    public GameObject(string name) {}
    public Component AddComponent(Type type) { return null; }
    public bool get_activeInHierarchy() { return false; }
  }
  public class Component : Object {
    public GameObject get_gameObject() { return null; }
    public Transform get_transform() { return null; }
  }
  public class Shader : Object { public static Shader Find(string name) { return null; } }
  public class Material : Object {
    public Material(Shader shader) {}
    public void SetInt(string name, int value) {}
  }
  public class Renderer : Component {
    public void set_enabled(bool value) {}
    public void set_sharedMaterial(Material value) {}
  }
  public class LineRenderer : Renderer {
    public void set_positionCount(int value) {}
    public void set_useWorldSpace(bool value) {}
    public void set_startWidth(float value) {}
    public void set_endWidth(float value) {}
    public void set_startColor(Color value) {}
    public void set_endColor(Color value) {}
    public void SetPosition(int index, Vector3 value) {}
  }
  public class GUIStyle {
    public GUIStyle() {}
    public void set_fontSize(int v) {}
    public void set_alignment(int v) {}
    public void set_wordWrap(bool v) {}
    public GUIStyleState get_normal() { return null; }
    public static void SetDefaultFont(Font f) {}
  }
  public class GUISkin { public Font get_font() { return null; } }
  public class Font {}
  public class GUIStyleState { public void set_textColor(Color c) {} }
  public class GUI {
    public static GUISkin get_skin() { return null; }
    public static void Label(Rect r, string t, GUIStyle s) {}
    public static Matrix4x4 get_matrix() { return new Matrix4x4(); }
    public static void set_matrix(Matrix4x4 m) {}
    public static void set_color(Color c) {}
    public static Color get_contentColor() { return new Color(1,1,1,1); }
    public static void set_contentColor(Color c) {}
    public static Color get_color() { return new Color(1,1,1,1); }
    public static void DrawTexture(Rect r, Texture t) {}
  }
  public class Texture {}
  public class Texture2D : Texture { public static Texture2D get_whiteTexture() { return null; } }
  public class Time {
    public static float get_unscaledDeltaTime() { return 0f; }
    public static int get_frameCount() { return 0; }
    public static void set_timeScale(float v) {}
    public static float get_timeScale() { return 1f; }
  }
  public class Screen {
    public static int get_width() { return 0; }
    public static int get_height() { return 0; }
  }
  public class Event {
    public void Use() {}
    public static Event get_current() { return null; }
    public Vector2 get_mousePosition() { return new Vector2(0, 0); }
    public int get_type() { return 0; }
  }
  public enum TouchPhase { Began, Moved, Stationary, Ended, Canceled }
  public struct Touch {
    public int get_fingerId() { return 0; }
    public Vector2 get_position() { return new Vector2(); }
    public TouchPhase get_phase() { return TouchPhase.Began; }
  }
  public class Input {
    public static int get_touchCount() { return 0; }
    public static Touch GetTouch(int index) { return new Touch(); }
  }
  public class Application { public static string persistentDataPath { get { return "."; } } }
  public class PlayerPrefs {
    public static float GetFloat(string key, float value) { return value; }
    public static void SetFloat(string key, float value) {}
    public static int GetInt(string key, int value) { return value; }
    public static void SetInt(string key, int value) {}
    public static string GetString(string k, string d) { return d; }
    public static void SetString(string k, string v) {}
    public static void DeleteKey(string k) {}
  }
  public class Transform : Component {
    public Vector3 get_position() { return new Vector3(0,0,0); }
    public Quaternion get_rotation() { return new Quaternion(); }
    public void set_rotation(Quaternion q) {}
    public bool IsChildOf(Transform parent) { return false; }
  }
  public struct Bounds { public Vector3 get_center() { return new Vector3(); } }
  public class Collider : Component { public Bounds get_bounds() { return new Bounds(); } }
  public struct RaycastHit {
    public Collider get_collider() { return null; }
    public float get_distance() { return 0f; }
  }
  public enum QueryTriggerInteraction { UseGlobal, Ignore, Collide }
  public class Physics {
    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit,
        float distance, int mask, QueryTriggerInteraction triggers) {
      hit = new RaycastHit(); return false;
    }
  }
  public class Camera : Component {
    public static Camera get_main() { return null; }
    public Vector3 WorldToScreenPoint(Vector3 p) { return new Vector3(0,0,0); }
  }
}

namespace COW.GamePlay {
  public class Player : UnityEngine.Component {
    public CGKJLKPMGDJ FDMIEDDNCEC;
    public UnityEngine.Transform MainCameraTransform;
    public bool IsClientBot;
    public bool IsLocalPlayer() { return false; }
    public bool IsLocalTeammate(bool includeDead) { return false; }
    public bool get_IsDieing() { return false; }
    public bool IsInStealth() { return false; }
    public int get_CurHP() { return 0; }
    public int get_MaxHP() { return 0; }
    public string get_NickName() { return null; }
    public UnityEngine.Transform get_RootTransform() { return null; }
    public UnityEngine.Transform GetHeadTF() { return null; }
    public UnityEngine.Collider get_HeadCollider() { return null; }
    public System.Collections.Generic.List<UnityEngine.Collider> get_FireColliders() { return null; }
    public UnityEngine.Quaternion GetAimRotation() { return new UnityEngine.Quaternion(); }
    public UnityEngine.Transform get_NeckBone() { return null; }
    public UnityEngine.Transform get_SpineBoneTransform() { return null; }
    public UnityEngine.Transform get_ShoulderBoneTransform() { return null; }
    public UnityEngine.Transform get_BoneRootTransform() { return null; }
    public bool IsFiring() { return false; }
    public bool get_IsSighting() { return false; }
    public UnityEngine.Vector3 get_AimStartPostion() { return new UnityEngine.Vector3(); }
    public void SetAimRotation(UnityEngine.Quaternion rotation, bool force) {}
    public HBIBDMMOOOK GetActiveWeapon() { return null; }
    public UnityEngine.Vector3 GetAttackableCenterWS() { return new UnityEngine.Vector3(); }
  }
  public class JMAGGLCNGIG { public System.Collections.IList ILPNGCMEFFI() { return null; } }
  public enum LKOOALMKJND { Default, Head, Body }
  public class CGKJLKPMGDJ {
    public UnityEngine.GameObject JHAINOBIFMA;
    public UnityEngine.Collider JINDNOFABKB;
    public UnityEngine.Vector3 HPCOJHGFEEG, DOJDNHCGFGK, HLDECMJFKJK, EKPLMDDKOGB;
    public LKOOALMKJND DKPDBLLJEAA;
    public bool EFMBNPNFGHM, CEOKOILBODN;
    public int OLGEMBBDBID;
    public float CFIPDNFBKHK;
    public int GABPHAKNADH;
  }
  public class HBIBDMMOOOK {
    public void GENFKNDFIJH(UnityEngine.Vector3 from, UnityEngine.Vector3 to,
                           UnityEngine.GameObject target, short traceType) {}
  }
  public class SceneEditBoxSelectTool : UnityEngine.Component {
    public static UnityEngine.GameObject MzDriver;
    public static bool MzGuiRunning;
    public static int OkUiAccepted, OkUiRejected, OkUiReentry;
    public static UnityEngine.GUIStyle MzLabelStyle;
    public static byte[] MzSwitch;
    public static float MzPanelX;
    public static float MzPanelY;
    public static float MzDownX;
    public static float MzDownY;
    public static float MzPressX;
    public static float MzPressY;
    public static bool MzPointerCaptured;
    public static bool MzOpen;
  }
}

namespace COW {
  public class GameFacade {
    public static COW.GamePlay.JMAGGLCNGIG CurrentMatch() { return null; }
    public static COW.GamePlay.Player GetLocalPlayerOrObServer() { return null; }
  }
}

namespace UnityEngine.Networking {
  public class UnityWebRequestAsyncOperation { }
  public class UnityWebRequest {
    public UnityWebRequest(string url) {}
    public UnityWebRequestAsyncOperation SendWebRequest() { return null; }
    public bool get_isDone() { return false; }
    public string get_error() { return null; }
    public void Dispose() {}
  }
}
