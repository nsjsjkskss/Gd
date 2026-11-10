// Stub API game (chi de mcs sinh MemberRef dung ten -> converter tra ra type/extern cua patch)
using System;

namespace UnityEngine {
  public struct Rect {
    public Rect(float x, float y, float w, float h) {}
    public bool Contains(Vector2 p) { return false; }
  }
  public struct Color { public float r, g, b, a; public Color(float r, float g, float b, float a) { this.r = r; this.g = g; this.b = b; this.a = a; } }
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
  public struct Quaternion { public float x, y, z, w;
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
    public int GetInstanceID() { return 0; }
    public static void Destroy(Object o) {}
    public static void DontDestroyOnLoad(Object target) {}
    public string get_name() { return null; }
    public static bool operator ==(Object a, Object b) { return false; }
    public static bool operator !=(Object a, Object b) { return true; }
    public override bool Equals(object obj) { return base.Equals(obj); }
    public override int GetHashCode() { return base.GetHashCode(); }
  }
  public class GameObject : Object {
    public Component GetComponent(Type type) { return null; }
    public Component GetComponent(string type) { return null; }
    public Component GetComponentInChildren(Type type) { return null; }
    public static GameObject Find(string name) { return null; }
    public GameObject(string name) {}
    public Component AddComponent(Type type) { return null; }
    public bool get_activeInHierarchy() { return false; }
  }
  public class Behaviour : Component { }
  public class MonoBehaviour : Behaviour { }
  public class Component : Object {
    public GameObject get_gameObject() { return null; }
    public Transform get_transform() { return null; }
  }
  public class Shader : Object { public static Shader Find(string name) { return null; } }
  public class Material : Object {
    public bool SetPass(int pass) { return true; }
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
    public static float get_realtimeSinceStartup() { return 0f; }
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
    public static void Save() {}
    public static float GetFloat(string key, float value) { return value; }
    public static void SetFloat(string key, float value) {}
    public static int GetInt(string key, int value) { return value; }
    public static void SetInt(string key, int value) {}
    public static string GetString(string k, string d) { return d; }
    public static void SetString(string k, string v) {}
    public static void DeleteKey(string k) {}
  }
  public class Transform : Component {
    public Vector3 get_forward() { return new Vector3(0, 0, 1); }
    public void set_position(Vector3 value) {}
    public int get_childCount() { return 0; }
    public Transform GetChild(int index) { return null; }
    public Vector3 get_position() { return new Vector3(0,0,0); }
    public Quaternion get_rotation() { return new Quaternion(); }
    public void set_rotation(Quaternion q) {}
    public bool IsChildOf(Transform parent) { return false; }
  }
  public struct Bounds { public Vector3 get_center() { return new Vector3(); } }
  public class Collider : Component { public Bounds get_bounds() { return new Bounds(); } public bool get_isTrigger() { return false; } }
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
    public float get_fieldOfView() { return 0f; }
    public void set_fieldOfView(float value) {}
  }
}

namespace COW.GamePlay {
  public class Player : UnityEngine.MonoBehaviour {
    public UnityEngine.Transform GetHipTF() { return null; }
    public UnityEngine.Transform get_ArmBoneLeft() { return null; }
    public UnityEngine.Transform get_ArmBoneRight() { return null; }
    public UnityEngine.Transform get_LeftForeArmTF() { return null; }
    public UnityEngine.Transform get_RightForeArmTF() { return null; }
    public UnityEngine.Transform get_LeftHandTF() { return null; }
    public UnityEngine.Transform get_RightHandTF() { return null; }
    public UnityEngine.Transform get_LegBoneLeft() { return null; }
    public UnityEngine.Transform get_LegBoneRight() { return null; }
    public UnityEngine.Transform GetLeftAnkleTF() { return null; }
    public UnityEngine.Transform GetRightAnkleTF() { return null; }
    public UnityEngine.Transform get_FootBoneLeft() { return null; }
    public UnityEngine.Transform get_FootBoneRight() { return null; }
    public bool get_HasVest() { return false; }
    public bool get_HasHelmet() { return false; }
    public bool IsCrouching() { return false; }
    public bool IsReloading() { return false; }
    public BEADLMGGGGL get_PlayerID() { return new BEADLMGGGGL(); }
    public virtual void StartFiring(HBIBDMMOOOK weapon) {}
    public virtual void StartWholeBodyFiring(HBIBDMMOOOK weapon) {}
    public OGKFEMJNCGM JGIEOGCGFEP(CGKJLKPMGDJ hit) { return null; }
    public virtual void StopFire(HBIBDMMOOOK weapon) {}
    public virtual void FullWeaponAmmo() {}
    public bool IsGroundedByRayCastHit() { return false; }
    public bool IsGrounded() { return false; }
    public float get_LastStartJumpTime() { return 0f; }
    public UnityEngine.Vector3 GetVelocity() { return new UnityEngine.Vector3(); }
    public void SetForceMoveVelocity(UnityEngine.Vector3 value, uint key) {}
    public UnityEngine.Vector3 GetForceMoveVelocity() { return new UnityEngine.Vector3(); }
    public int GetPhysXState() { return 0; }
    public HBIBDMMOOOK GetWeaponOnHand() { return null; }
    public bool m_GroundHitResult;
    public bool InSnowSlideWayDashing;
    public float SnowSlideWayRisingGravity;
    public float SnowSlideWayFallingGravity;
    public float get_CustomGravity() { return 0f; }
    public float get_FallingGravity() { return 0f; }
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
    public bool IsInVehicle() { return false; }
    public COW.GamePlay.Vehicle VehicleIAmIn() { return null; }
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
    public PlayerAttributes get_Attributes() { return null; }
    public void ResetSwapWeaponTime() {}
    public void AutoSwitchWeapon(bool a, bool b) {}

  }
  public class JMAGGLCNGIG { public System.Collections.IList ILPNGCMEFFI() { return null; } public System.Collections.IDictionary LGNHODMMCMJ; }
  public class LevelVehicle : UnityEngine.Component { public int get_CurHP() { return 0; } public int get_MaxHP() { return 0; } }
  public class PlayerAttributes : UnityEngine.Component { public bool IsInvincible() { return false; } public bool ShootNoReload; public float RunSpeedUpScale; public float SkillScatterRate { get { return 0f; } set {} } public float SkillScatterRateSighting { get { return 0f; } set {} } public float FireIntervalScale { get { return 1f; } set {} } public bool IsAmmoFree { get { return false; } set {} } public float BuffWeaponReloadScale; public float ForceSetRunAndDashSpeed; }
  public enum LKOOALMKJND { Default, Head, Body }
  public class CGKJLKPMGDJ {
    public UnityEngine.Vector3 ONMKMEGMHOL;
    public short CLOLKHNMNEK;
    public object MGBCGEBLKJB;
    public string ANJGNHKHOKO;
    public UnityEngine.GameObject JHAINOBIFMA;
    public UnityEngine.Collider JINDNOFABKB;
    public UnityEngine.Vector3 HPCOJHGFEEG, DOJDNHCGFGK, HLDECMJFKJK, EKPLMDDKOGB;
    public LKOOALMKJND DKPDBLLJEAA;
    public bool EFMBNPNFGHM, CEOKOILBODN;
    public int OLGEMBBDBID;
    public float CFIPDNFBKHK;
    public int GABPHAKNADH;
  }
  public class IEFJGCGOMID { public uint ODFDGMOCJLL; public uint MJCENIGKEPH() { return 0; } }
  public class HBIBDMMOOOK : IEFJGCGOMID {
    public IOKEALMHBKN PIILKNEHNLB;
    public int KBIFKMJODNA() { return 0; }
    public float LDKCJEMDAMC() { return 0f; }
    public float BDHMJJIADFG() { return 0f; }
    public bool KENANOGLPJO() { return false; }
    public System.Collections.Generic.List<float> NPLMKLBKJPE(OGKFEMJNCGM target, UnityEngine.Collider collider, bool flag, HNLIMBLIANL info) { return null; }
    public int LHNJLFOINDO(CGKJLKPMGDJ hit) { return 0; }
    public int CIHHEOKFADJ(CGKJLKPMGDJ hit, UnityEngine.MonoBehaviour target, float param) { return 0; }
    public float OEOBPHDFJMO() { return 0f; }
    public COW.WeaponGunFeelProfileData ILGIAJHNNDP() { return null; }
    public void GENFKNDFIJH(UnityEngine.Vector3 from, UnityEngine.Vector3 to,
                           UnityEngine.GameObject target, short traceType) {}
  }
  public class SceneEditBoxSelectTool : UnityEngine.Component {
    public static UnityEngine.GameObject MzDriver;
    public static bool MzGuiRunning;
    public static int Zx4c1ce7a, Zx6b5ca42, Zx5f3a853;
    public static System.Single[] ZxFront;
    public static System.Single[] ZxBack;
    public static System.Single[] ZxPrev;
    public static System.Single[] ZxDraw;
    public static System.Single[] ZxWorld;
    public static System.Single[] ZxWorldPrev;
    public static System.Single[] ZxLive;
    public static System.Single[] ZxMeters;
    public static System.Single[] ZxBackMeters;
    public static System.Int32 ZxPrevCount;
    public static System.Int32 ZxWorldCount;
    public static System.Int32 ZxWorldPrevCount;
    public static System.Int32 ZxLiveCount;
    public static System.Int32 ZxIdsCount;
    public static System.Int32 ZxIdsPrevCount;
    public static System.Int32[] ZxIds;
    public static System.Int32[] ZxIdsPrev;
    public static UnityEngine.LineRenderer[] Zx5992bd4;
    public static UnityEngine.LineRenderer[] Zxc8b6421;
    public static UnityEngine.Material Zx9dd2f81;
    public static System.Int32 Zxa606551;
    public static COW.GamePlay.Player Zxa4ac39a;
    public static COW.GamePlay.Player Zx6472cf9;
    public static COW.GamePlay.JMAGGLCNGIG Zxb414436;
    public static System.Int32 Zxb44a5e1;
    public static System.Int32 Zx513f0e7;
    public static System.Int32 Zx6bd394c;
    public static System.Int32 Zxf5ba39d;
    public static System.Int32 Zxdc58154;
    public static System.Int32 Zxad02e3d;
    public static System.Int32 Zxe61c424;
    public static COW.GamePlay.CGKJLKPMGDJ Zx2a72a6f;
    public static UnityEngine.GameObject Zxa463246;
    public static UnityEngine.GameObject Zxfa9a523;
    public static UnityEngine.Collider Zx167cbcd;
    public static UnityEngine.Collider Zx4882118;
    public static UnityEngine.Vector3 Zx9d4a2d3;
    public static UnityEngine.Vector3 Zx8176230;
    public static UnityEngine.Vector3 Zx6674dbe;
    public static UnityEngine.Vector3 Zxcfd54c0;
    public static UnityEngine.Vector3 Zx0126c95;
    public static COW.GamePlay.LKOOALMKJND Zx4ef8a62;
    public static COW.GamePlay.LKOOALMKJND Zx39781aa;
    public static System.Boolean Zx5d825f6;
    public static System.Boolean Zxf3457a1;
    public static System.Boolean Zxc0a4b9d;
    public static System.Boolean Zxc5c2fc9;
    public static UnityEngine.GUIStyle[] Zx76cb2b6;
    public static System.Boolean Zxeed3b1e;
    public static System.Boolean Zxd35d8b7;
    public static System.Int32 Zx65f7d8f;
    public static System.Int32 Zx447d912;
    public static System.Single Zx1d8a279;
    public static System.Single Zx635b5cc;
    public static System.Int32 Zx1e1fdc1;
    public static System.Int32 Zxd28db42;
    public static System.Single Zx60cb4f4;
    public static UnityEngine.Transform[] ZxRootTf;
    public static UnityEngine.Transform[] ZxHeadTf;
    public static System.Single[] Zx9fd1fea;
    public static System.Boolean[] Zx9916ba5;
    public static UnityEngine.Transform[] Zxa13badb;
    public static UnityEngine.Transform[] Zxcd78bd5;
    public static System.Int32[] Zxf3d93d7;
    public static System.String[] Zxe3a3dfd;
    public static System.String[] Zxdeb10c2;
    public static System.Int32[] Zx7fa6a53;
    public static System.String[] Zx9f021cb;
    public static UnityEngine.Color Zx3c4a82a;
    public static System.Single[] Zx96dd77c;
    public static UnityEngine.Mesh R8FovMesh;
    public static UnityEngine.Material R8FovMaterial;
    public static System.Single R8FovRadius;
    public static System.Int32 R8FovWidth;
    public static System.Int32 R8FovHeight;
    public static System.Int32[] Zx7e03505;
    public static System.Int32[] Zx6ee14f8;
    public static System.Single[] Zx3b0e144;
    public static System.Single[] Zx41c39ab;
    public static System.Boolean[] Zxa659ff4;
    public static System.Boolean[] Zx857fa6c;
    public static System.Boolean[] Zxfa3386b;
    public static System.Boolean[] Zx054c30e;
    public static System.Single[] Zx6d86575;
    public static System.Single[] Zx838d60d;
    public static System.Boolean[] Zx46ee197;
    public static System.Boolean[] Zx30eaf81;
    public static COW.GamePlay.Player[] Zxcb50f6d;
    public static COW.GamePlay.Player[] Zx3836571;
    public static System.Single[] Zx0eda687;
    public static System.String[] Zxec27a7e;
    public static UnityEngine.Color Zx8d0a841;
    public static UnityEngine.Color Zx1d1640d;
    public static System.Int32 Zx4de822b;
    public static System.Boolean Zxf4ca405;
    public static System.Single Zxd743c61;
    public static System.Int32 Zx1619ebc;
    public static System.Int32 Zx5b461d5;
    public static System.Int32 Zxd16e43d;
    public static System.String Zx58d8e8b;
    public static System.Boolean Zx4f44d88;
    public static UnityEngine.Vector3 Zx51a4eaa;
    public static System.Single Zx27385ef;
    public static COW.GamePlay.Player Zx5aedd00;
    public static COW.GamePlay.JMAGGLCNGIG Zxc0040fc;
    public static UnityEngine.Vector3 Zxaaebcd5;
    public static UnityEngine.Vector3 Zx04a45e0;
    public static System.Single Zx324e1d0;
    public static System.Single Zx41d811b;
    public static System.Single Zx7146047;
    public static System.Single Zx0545665;
    public static System.Single Zxeb2d06c;
    public static System.Single Zx5998847;
    public static System.Single Zx5e922fb;
    public static System.Single Zx04df5b3;
    public static System.Int32 Zxbf365f8;
    public static System.Int32 Zx1a282ff;
    public static System.Boolean Zxb0bf803;
    public static System.String Zxba0dd78;
    public static System.Single ZxHFNoticeUntil;
    public static System.Single Zx92648bd;
    public static System.Int32 Zxa7465f3;
    public static System.Int32 Zx994d756;
    public static System.Int32 Zxe208c69;
    public static System.String Zx25cb7bd;
    public static COW.GamePlay.Player Zx633e561;
    public static COW.GamePlay.JMAGGLCNGIG Zx0b81acf;
    public static System.Int32 Zxcdd5a3a;
    public static System.Boolean Zx6cf04c7;
    public static System.Boolean Zx6910825;
    public static UnityEngine.Vector3 Zx27a70e8;
    public static UnityEngine.Vector3 Zx97406cf;
    public static System.Single Zx41dd9fc;
    public static System.Single Zxa5fc5e7;
    public static System.Single Zx8c2e64e;
    public static System.Single Zxf5f6bcc;
    public static System.Single Zxaadaf57;
    public static System.Single Zxdc4dc6d;
    public static System.Single Zxee5f1e7;
    public static System.Single Zx123707a;
    public static System.Single Zx51c6116;
    public static System.Single Zx4835f14;
    public static System.Single Zx8f27331;
    public static System.Boolean Zx64c6109;
    public static System.Boolean Zx9b7df00;
    public static System.Single Zx4f494d8;
    public static System.Int32 Zx8495afa;
    public static System.Single Zxf99b46d;
    public static System.Single Zxaa6699a;
    public static System.Single Zx85fe3a2;
    public static System.Int32 ZxFeatureFrame;
    public static System.Int32 Zx01d4522;
    public static System.Boolean Zxcc8d803;
    public static System.Int32 Zxa8c09ba;
    public static System.String Zx4d50001;
    public static System.Boolean Zx725515d;
    public static UnityEngine.Networking.UnityWebRequest Zx3c924df;
    public static System.Int32 Zxdeb7952;
    public static System.Boolean Zxac13af9;
    public static UnityEngine.Networking.UnityWebRequest Zx742e4f2;
    public static System.Boolean Zx3f0a2e2;
    public static System.Int32 Zxeb06d54;
    public static UnityEngine.Networking.UnityWebRequest Zx8920732;
    public static System.Int32 Zx0c284aa;
    public static System.String Zx02b572c;
    public static UnityEngine.Networking.UnityWebRequest Zxefde87b;
    public static System.Int32 Zx4f78091;
    public static System.String Zxdf32c93;
    public static UnityEngine.Networking.UnityWebRequest Zxae1b147;
    public static System.Int32 Zx4d1d440;
    public static System.Boolean Zxa87a45e;
    public static System.String Zx6592044;
    public static System.String Zx6fa5c15;
    public static UnityEngine.Networking.UnityWebRequest Zx0fda3c3;
    public static System.Int32 Zx214fac9;
    public static System.String Zx99087db;
    public static System.Single Zx6045a8e;
    public static System.Int32 Zx0082d3a;
    public static System.Int32 Zx4ab5f41;
    public static System.Int32 Zx3440801;
    public static System.Int32 Zx689a5a3;
    public static System.Int32 Zx0365f1c;
    public static System.Int32 Zx931a875;
    public static System.Int32 Zx8f3ce9a;
    public static System.Int32 Zx57b65d1;
    public static System.Int32 Zx72c422d;
    public static System.Int32 Zxccc4567;
    public static System.Single Zx66e6ece;
    public static System.Single Zx925b72b;
    public static System.Single Zx5245c77;
    public static System.Int32 Zx847ed24;
    public static System.Int32 Zxc2144e5;
    public static System.Single Zx74bb9a1;
    public static System.Int32 Zx8827a2e;
    public static System.Int32 Zx748c7f9;
    public static System.Int32 Zx6784c20;
    public static System.Int32 Zx93c4570;
    public static System.String Zx3daf7a7;
    public static System.String Zxda45310;
    public static System.String Zx9b8c19f;
    public static System.String Zx30ab0df;
    public static System.String Zxda9c714;
    public static UnityEngine.Networking.UnityWebRequest Zxbfe2278;
    public static System.Single Zxb0c5deb;
    public static System.Single Zx7cd2b77;
    public static System.Single Zx94a6b6a;
    public static System.Int32 Zxd869a67;
    public static COW.GamePlay.Player Zxd06ff6f;
    public static System.Single Zx316270d;
    public static COW.GamePlay.PlayerAttributes Zxfd35f49;
    public static System.Boolean Zx54c4855;
    public static System.Boolean Zxe240989;
    public static System.Single Zx24da377;
    public static System.Single Zx17e9bf3;
    public static System.Single Zx577f981;
    public static System.Int32 Zx21571fa;
    public static System.Int32 Zxe418f63;
    public static System.Int32 Zxeefa15e;
    public static System.Int32 Zx2623aa9;
    public static System.Boolean Zx558a20f;
    public static System.Single Zx59631bb;
    public static System.UInt32 Zxcd36d2e;
    public static System.Single Zxc7b6d43;
    public static System.Int32 Zxb6e8183;
    public static System.Int32 Zx16fc72e;
    public static System.Int32 Zx825a403;
    public static System.Int32 Zx70e9f3a;
    public static System.Int32 Zxf0498e2;
    public static System.Int32 Zx86f4aae;
    public static System.Single Zx240eed4;
    public static COW.GamePlay.Player Zx9792f89;
    public static COW.GamePlay.Player Zxfd4df51;
    public static COW.GamePlay.Player Zx56b69c0;
    public static COW.GamePlay.JMAGGLCNGIG Zx7f23289;
    public static COW.GamePlay.HBIBDMMOOOK Zx1619c92;
    public static System.Single Zx82c9c5c;
    public static System.Single Zx88dcc5a;
    public static System.Boolean Zxbd7113e;
    public static System.Int32 Zx897adec;
    public static System.String Zx5569989;
    public static System.String Zx1ddcddd;
    public static COW.GamePlay.Player[] Zxfa9179c;
    public static System.Int32[] Zxab0dd60;
    public static System.Single[] Zx8edff5a;
    public static System.Int32 Zx19dccf8;
    public static System.Int32 Zx89ff375;
    public static System.Int32 Zx99746dd;
    public static System.Int32 Zx4969a60;
    public static System.Int32 Zxe332667;
    public static System.Boolean Zxf49526f;
    public static System.Int32 Zxf48e82a;
    public static System.Single Zxe502b16;
    public static System.Boolean Zxc4ebb60;
    public static System.Single Zx11e14e7;
    public static System.Boolean Zx8db27de;
    public static System.Single Zx13f260f;
    public static System.Boolean Zxe8d2f5b;
    public static System.Int32[] Zx792c226;
    public static System.Int32[] Zx583761b;
    public static System.Int32[] Zx0b983fd;
    public static System.Int32 Zx55d7b96;
    public static System.Int32 Zx0099695;
    public static System.Int32 Zx3575d3d;
    public static System.Int32 Zx0a8bff9;
    public static System.String Zx5a3e469;
    public static System.Int32 Zxd265c96;
    public static System.Int32 Zxda4736a;
    public static System.String Zx2753b33;
    public static System.Int32 Zxe2e9f09;
    public static System.Int32 Zx34efd7c;
    public static System.Int32 Zxe54ff61;
    public static System.Int32 Zxf906f1e;
    public static System.Int32 Zx44c0115;
    public static System.Int32 Zxc93449b;
    public static System.Int32 Zx547c35e;
    public static System.Single Zx73b7a69;
    public static System.Int32 Zx99b3be4;
    public static System.String Zx801408c;
    public static System.UInt32 Zxc4cc996;
    public static System.Single Zxa454328;
    public static System.Single Zxbac7a40;
    public static System.Single Zxf302889;
    public static COW.GamePlay.HBIBDMMOOOK Zxf4ca292;
    public static System.Single Zx3c16b4d;
    public static System.Single Zx3d7d0cc;
    public static System.Single Zxa3804d7;
    public static System.Single Zxf011053;
    public static System.Single Zx85ff0ad;
    public static System.Single Zxb091c0b;
    public static System.Single Zx31e5d10;
    public static System.Int32 Zx028ff7b;
    public static System.Single Zxa7dbda9;
    public static System.Int32 Zxe317b04;
    public static System.String Zxa5e3ae5;
    public static System.String Zxc2db594;
    public static System.Int32 Zx6318eaa;
    public static System.Int32 Zx82d65f6;
    public static System.String Zx4c5532b;
    public static System.Boolean Zx2487c50;
    public static System.Int32 Zx77f3f72;
    public static System.Int32 Zx5a0676a;
    public static System.Int32[] Zx23c3e3e;
    public static System.Single Zxe02a56c;
    public static System.Int32 Zxc1c359d;
    public static System.Int32 Zxa3a059f;
    public static System.Int32 Zx9d368fe;
    public static System.Int32 Zxa8041cb;
    public static System.UInt32 Zxd6e24b3;
    public static System.String Zxfb38810;
    public static System.String Zx53a2ee1;
    public static System.Boolean Zx89c3772;
    public static System.Int32 Zxe0a6680;
    public static System.Int32 Zx85aba33;
    public static System.Int32 Zxd48ca0a;
    public static System.Int32 Zx6983b27;
    public static System.Single Zxdec9f99;
    public static System.Int32 Zx14970e5;
    public static System.Int32 Zx6775f79;
    public static System.String Zx13554af;
    public static System.String Zxe0f54c0;
    public static System.Int64 Zxa955bc1;
    public static UnityEngine.Networking.UnityWebRequest Zxa69426c;
    public static System.Single Zxa7737bd;
    public static System.Boolean Zx9af4ccb;
    public static System.Boolean Zx47c075a;
    public static UnityEngine.Networking.UnityWebRequest Zx8afdb74;
    public static System.Boolean Zx9ef7abd;
    public static System.Single Zxbd51549;
    public static System.Boolean Zx300a80a;
    public static System.String Zx6f3f293;
    public static System.Int32 Zx0674c58;
    public static System.Single Zx095eaf9;
    public static System.Int32 Zx5442eaa;
    public static System.Int32 Zxde221c0;
    public static System.Int32 Zx904ac7d;
    public static System.Int32 Zxc22150c;
    public static System.Single Zx44ffec0;
    public static System.Single Zx56ccd1a;
    public static System.String Zx660c3b1;
    public static COW.GamePlay.Vehicle Zx2b4538a;
    public static COW.GamePlay.Player Zxf1604d9;
    public static System.Single ZxPrevStamp;
    public static System.Single ZxLiveStamp;
    public static System.Boolean ZxSmooth;
    public static System.Boolean ZxLiveValid;
    public static System.String[] ZxNames;
    public static System.String[] ZxBackNames;
    public static System.String[] ZxDistances;
    public static System.String[] ZxBackDistances;
    public static System.String[] ZxFullNames;
    public static System.String[] ZxShortNames;
    public static System.String[] ZxDistanceLabels;
    public static System.Boolean[] ZxKnocked;
    public static System.Boolean[] ZxBackKnocked;
    public static System.Int32 ZxFrame;
    public static System.Int32 ZxWidth;
    public static System.Int32 ZxHeight;
    public static System.Int32 ZxFlags;
    public static System.Int32 ZxCount;
    public static System.Int32 ZxVisited;
    public static System.Int32 ZxSamples;
    public static System.Int32 ZxDrawBudget;
    public static System.Int32 ZxDraws;
    public static System.Int32 ZxErrors;
    public static System.Int32 ZxHeapMiB;
    public static System.Int32 ZxCursor;
    public static System.Single ZxNextSample;
    public static System.Single ZxSampleStamp;
    public static System.Single ZxSampleMs;
    public static System.Single ZxHeapDelay;
    public static System.Single ZxErrorAfter;
    public static System.Single ZxSaveAfter;
    public static System.Single ZxHeapAfter;
    public static System.Single ZxReportAfter;
    public static System.Int64 Zx9bd4c39;
    public static System.String Zx65541d1;
    public static System.String Zx09c9fc4;
    public static UnityEngine.GUIStyle Zx1cc5c48;
    public static System.Single Zxe331a2b;
    public static COW.GamePlay.JMAGGLCNGIG Zx2f1848f;
    public static UnityEngine.Camera Zxfb417c8;
    public static COW.GamePlay.Player Zxf36379d;
    public static System.Int32 ZxThread;
    public static System.Int32 ZxCollections;
    public static System.Int32 ZxGcErrors;
    public static System.Int64 ZxHeapStart;
    public static System.Int64 ZxHeapPeak;
    public static System.Int64 ZxHeapBefore;
    public static System.Int64 ZxHeapAfterCollect;
    public static System.Single ZxCollectAfter;
    public static System.Single ZxGcMs;
    public static System.String ZxGcStats;
    public static UnityEngine.Mesh R8Mesh;
    public static UnityEngine.Material R8Material;
    public static UnityEngine.Vector3[] R8Vertices;
    public static UnityEngine.Color[] R8Colors;
    public static System.Int32[] R8Indices;
    public static System.Boolean R8MeshDirty;
    public static System.Boolean R8ColorsDirty;
    public static System.Boolean R8BatchOn;
    public static System.Boolean R8BatchActive;
    public static System.Single R8BatchRetry;
    public static System.Int32 R8QuadCount;
    public static System.Int32 R8ApiDraws;
    public static System.Int32 R8Uploads;
    public static System.Int32 R8BatchFailures;
    public static System.String R8Stats;
    public static System.String R8Phase;
    public static System.Boolean[] Zx3de8320;
    public static System.Boolean[] Zx5b0b2a5;
    public static System.Boolean[] Zx8c4b660;
    public static System.Boolean[] Zxe63a668;
    public static System.Single[] Zx9df99a6;
    public static System.Single[] Zxbc88301;
    public static System.Int32 Zx649d8fa;
    public static UnityEngine.Color Zxaeb6ade;
    public static UnityEngine.Color Zxfc90704;
    public static UnityEngine.Color Zx84f3f18;
    public static UnityEngine.Color Zx3b07a6a;
    public static COW.GamePlay.PlayerAttributes Zxfefb633;
    public static System.Boolean Zx4b2c801;
    public static System.Boolean Zxff3d85e;
    public static COW.GamePlay.PlayerAttributes Zx54899b4;
    public static System.String Zx06ef0b8;
    public static System.Single Zx8ef9e60;
    public static System.Boolean Zx39038d4;
    public static System.Single Zx7eb83a6;
    public static System.Boolean Zx2d99817;
    public static System.Int32 Zx9045c73;
    public static COW.WeaponGunFeelProfileData Zxd7bf0bd;
    public static System.Boolean Zx88c5083;
    public static System.Boolean Zx357fec0;
    public static System.Boolean Zxbe529ad;
    public static COW.GamePlay.PlayerAttributes Zx03407a4;
    public static System.Single Zx1e83147;
    public static System.Boolean Zx5a3f1a5;
    public static System.Boolean Zxb96978c;
    public static System.Boolean Zx56a65a5;
    public static System.Boolean Zxcee1755;
    public static COW.GamePlay.PlayerAttributes Zx238f04e;
    public static System.Boolean Zxdee11d6;
    public static System.Boolean Zxd6c366c;
    public static System.Boolean Zxee85210;
    public static System.Boolean Zxbf3cddc;
    public static System.Single Zx8f1be81;
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
  public class WeaponGunFeelProfileData { public bool enableRecoil; public bool enableVisualRecoil; }
  public static class GameSettingData { public static bool m_IsHighFramerate; public static bool m_SmoothHighFrame; public static bool GetAutoSwitchWeapon() { return false; } public static void SetAutoSwitchWeapon(bool v) {} }
  public class GameFacade {
    public static UIInGameScene CurrentInGameUIScene() { return null; }
    public static GCommon.MatchGameClient CurretOnlineMatchClient() { return null; }
    public static COW.MatchGame CurrentMatchGame;
    public static bool Send(uint id, GCommon.UDPClientMessageBase msg, byte option, bool cache) { return false; }
    public static COW.GamePlay.JMAGGLCNGIG CurrentMatch() { return null; }
    public static COW.GamePlay.Player GetLocalPlayerOrObServer() { return null; }
  }
}

namespace UnityEngine.Networking {
  public class UnityWebRequestAsyncOperation { }
  public class UnityWebRequest {
    public UnityWebRequest(string url) {}
    public static UnityWebRequest Get(string url) { return null; }
    public long get_responseCode() { return 0; }
    public void set_timeout(int seconds) {}
    public UnityWebRequestAsyncOperation SendWebRequest() { return null; }
    public bool get_isDone() { return false; }
    public string get_error() { return null; }
    public void Dispose() {}
  }
}

namespace UnityEngine {
  public class Mesh : Object {
    public Mesh() {}
    public void MarkDynamic() {}
    public void set_vertices(Vector3[] v) {}
    public void set_colors(Color[] v) {}
    public void set_triangles(int[] v) {}
  }
  public static class GL {
    public static void PushMatrix() {}
    public static void PopMatrix() {}
    public static void LoadPixelMatrix(float left, float right, float bottom, float top) {}
  }
  public static class Graphics { public static void DrawMeshNow(Mesh m, Matrix4x4 matrix) {} }
}

namespace GCommon {
  public class UDPClientMessageBase { }
}
namespace message {
  public class EOBFCLNNIJH : GCommon.UDPClientMessageBase {
    public uint BEADLMGGGGL, FIKKBFDEJOK;
  }
  public class BJFGLKKKOAN : GCommon.UDPClientMessageBase { }
  public class NNDOIBJBAJI : GCommon.UDPClientMessageBase {
    public uint BEADLMGGGGL;
    public float DCEMJOGHMEG, KJGOMDJCKEB, DPJAIECLHGL;
    public BJFGLKKKOAN MGJNBIJNLIJ;
    public string JBEHMJFBEMG;
    public byte IMAOCDIIIHI;
  }
  public static class LMCBHLCPAJD {
    public static BJFGLKKKOAN MGOGJCMCIPG(UnityEngine.Vector3 direction) { return null; }
    public static LDCJOLOBGGH DJNIOABIFCA(UnityEngine.Vector3 value) { return null; }
  }
}
namespace COW.GamePlay {
  public struct BEADLMGGGGL {
    public static uint DEIPJDIGKIP(BEADLMGGGGL value) { return 0; }
  }
}
namespace COW {
  public class UIInGameScene {
    public UIBigMapController GetBigMap() { return null; }
    public EMapStateType get_MapState() { return EMapStateType.MiniMap; }
  }
  public class UIMapBaseController {
    public UIMapContentController CurrentActiveGetMapContentCtrl() { return null; }
  }
  public class UIBigMapController : UIMapBaseController {
    public bool GetBigMapActive() { return false; }
  }
  public class UIMapContentController {
    public bool m_HasLocalMapMarkRealPos;
    public UnityEngine.Vector3 m_LocalMapMarkRealPos;
    public UIHudPlayerMarkController m_LocalMapMarkController;
  }
  public class UIHudPlayerMarkController : UnityEngine.Component { public UnityEngine.Vector3 m_pos; }
}
public enum EMapStateType { MiniMap = 0, BigMap = 1, SideMap = 2 }

namespace COW.GamePlay {
  public interface OGKFEMJNCGM { UnityEngine.GameObject get_gameObject(); }
  public enum EHGMCNBHNMD { None = 0, Head = 1, Body = 2, Limb = 3 }
  public class IJHHJLDNMGH { public uint GetId() { return 0; } }
  public class IOKEALMHBKN { public uint JAOECECKADN; public IJHHJLDNMGH MHMGCLILKGJ; }
  public class HNLIMBLIANL {
    public int KOPMDJJKLKH;
    public EHGMCNBHNMD BGMOAOCFPGM;
    public BEADLMGGGGL BIHCFIINIBK;
    public HBIBDMMOOOK HBIBDMMOOOK;
    public int NEGABPENICF;
    public UnityEngine.Vector3 HOOMBLGOEPL, KEOAMDFAOLM, DOJDNHCGFGK;
    public short CLOLKHNMNEK;
    public bool FDJNCEIGHGB;
    public int OCOCPPNPHKN;
  }
  public class PIAEKELILCJ {
    public float BCGCKCKIFJE, BGPCJENGPBM, GGLOGCIBGFA, MMMHNBGEGNK,
                 HMGMDJJJCAA, OPCDFPPCIOB, FIPDEHJALEE, KLAJFPICFFJ, IFJNGGDLGMD;
    public int IEIDAILNCCE, JOGBMDLHJPH;
  }
  public class PlayerNetwork : Player {
    public int TakeDamage(int damage, BEADLMGGGGL playerId, HNLIMBLIANL info,
        int weaponId, UnityEngine.Vector3 firePos, UnityEngine.Vector3 targetPos,
        System.Collections.Generic.List<float> checks, PIAEKELILCJ dynamicInfo, uint vehicleId) { return 0; }
  }
}

namespace message {
  public class LDCJOLOBGGH {
    public int ALNGACCIEFF;
    public int CHHBFMKPGDC;
    public int JJPBHGNHGDN;
  }
  public class APCOOODFCGL {
    public uint FALECEKOBKD;
    public LDCJOLOBGGH CHBDLAKCLKD;
    public LPEKLBNOBEK GFJPEIEIFHC;
    public byte IMNOFNCGNBB;
    public BJFGLKKKOAN IEEIGMJLPEL;
  }
  public class LPEKLBNOBEK {
    public short ALNGACCIEFF;
    public short CHHBFMKPGDC;
    public short JJPBHGNHGDN;
    public short FAAFHAAKNGI;
  }
}
namespace COW.GamePlay {
  public class Vehicle : UnityEngine.MonoBehaviour {
    public void ChangeState(bool isEnter, Player player, bool flag, int seat, uint timestamp, bool force) {}
    public void PushSyncState(message.APCOOODFCGL msg, uint timestamp) {}
    public void SetVisibility(uint flags, bool visible) {}
    public void SetVelocity(UnityEngine.Vector3 value) {}
    public uint GetAttackableID() { return 0; }
  }
  public class VehicleCollision : UnityEngine.MonoBehaviour {
    public Vehicle attachVehicle;
    public bool isKillCollision;
    public void OnTriggerEnter(UnityEngine.Collider collider) {}
  }
}

namespace COW.GamePlay { public class IPBGFJNGLOH { public static float AMMOCNCMBBG; } }

namespace UnityEngine { public static class GUIUtility { public static string systemCopyBuffer { get { return ""; } set {} } } }

namespace GCommon { public class NetworkMessageDispatcher {  public System.Type GetCmdType(uint cmd) { return null; } } public class NetworkClient {  public GCommon.NetworkMessageDispatcher m_MessageDispatcher; } public class MatchGameClient : GCommon.NetworkClient {  public System.Collections.IList m_CurFrameHandleMsgCmdList;  public System.Collections.IList m_LastFrameHandleMsgCmdList;  public System.Collections.IList m_HandleMsgPacketList;  public System.Collections.IList m_RecvUDPMsgList;  public System.Collections.IList m_UnSerializedMsgList;  public GCommon.FastBinaryReader m_BinaryReader;  public GCommon.INetworkSession m_UDPSession; } public class UDPMsgPacket { public byte MsgKey; public byte CRC7;  public byte SendOption; public ushort Cmd; public ushort OrderID;  public byte Flags; public ushort Length; public byte[] Data; } public class FastBinaryReader { public byte[] _ioBuffer; public int _ioIndex;  public int _available; } public class TeaDecTempBuffer { public byte[] dec_iv_pre; public byte[] dec_iv_cur;  public uint[] dec_k; } public interface IUDPSession { byte[] GetPayloadKey(ushort cmd);  byte[] GetFallbackPayloadKey(ushort cmd); } public interface INetworkSession : GCommon.IUDPSession { } public class MUDPSession : GCommon.INetworkSession { public string m_SessionKey;  public byte[] m_TargetSecretKey; public byte[] m_SessionSecretKey;  public byte[] m_TargetProxySecretKey; public GCommon.TeaDecTempBuffer m_DecBuffer;  public byte[] GetPayloadKey(ushort cmd) { return null; }  public byte[] GetFallbackPayloadKey(ushort cmd) { return null; } } public class UDPSession : GCommon.INetworkSession { public string m_SessionKey;  public byte[] m_SessionSecretKey; public GCommon.TeaDecTempBuffer m_DecBuffer;  public byte[] GetPayloadKey(ushort cmd) { return null; }  public byte[] GetFallbackPayloadKey(ushort cmd) { return null; } } public class MultiNetworkSession { public System.Object m_Sessions;  public byte[] m_TargetSecretKey; public int[] m_TargetHeadSecretKey;  public byte[] m_TargetProxySecretKey; public byte[] m_SessionSecretKey; } public static class UDPParameters { public static byte[] SECRET_KEY;  public static int[] HEAD_SECRET; } public static class NetworkCryptologyUtil { public static int SALT_LEN;  public static int ZERO_LEN; public static uint DELTA; public static int ROUNDS;  public static int LOG_ROUNDS; public static byte[] TestKey; } public class ReplicationData { public int GroupID;  public GCommon.ReplicationDataValue Value; } public struct ReplicationDataValue { public float m_Single; public int m_Int32;  public uint m_UInt32; public short m_Int16; public ushort m_UInt16; } public class ReplicationDataPool { public GCommon.ReplicationData[] m_Datas;  public float GetDataFloat32(uint varID) { return 0f; }  public int GetDataInt32(uint varID) { return 0; }  public uint GetDataUInt32(uint varID) { return 0; }  public ushort GetDataUInt16(uint varID) { return 0; }  public short GetDataInt16(uint varID) { return 0; }  public byte GetDataUInt8(uint varID) { return 0; }  public bool GetDataBool(uint varID) { return false; } } public class ReplicationEntity { public System.Object m_PRIDataPool;  public uint GetReplicationID() { return 0; } } }

namespace COW { public class MatchGame { public System.Object m_ReplicationEntitis; } }
