// OKMOD - player ESP and configurable aim assist for the IFix HUD.
using UnityEngine;
using System.Collections;

public class Zx9466c8e {
  private static bool ZxBoot;
  public static void Hud() {
    Event current = Event.get_current();
    if (current == null) { return; }
    Matrix4x4 savedMatrix = GUI.get_matrix();
    Color savedColor = GUI.get_color();
    try {
      if (!ZxBoot) { ZxBoot = true; try { CleanupLegacyResidue(); } catch (System.Exception) { } }
      // Features must never suppress the panel itself.
      try { UpdateFeaturesOnce(); }
      catch (System.Exception error) { RecordHudError(error); }
      try { HudBody(); }
      catch (System.Exception error) { RecordHudError(error); }
    } finally {
      try { GUI.set_matrix(savedMatrix); } catch (System.Exception) { }
      try { GUI.set_color(savedColor); } catch (System.Exception) { }
    }
  }

  private static int HexNibble(char c) {
    if (c >= '0' && c <= '9') { return c - '0'; }
    if (c >= 'A' && c <= 'F') { return c - 'A' + 10; }
    if (c >= 'a' && c <= 'f') { return c - 'a' + 10; }
    return -1;
  }

  private static void CleanupLegacyResidue() {
    if (PlayerPrefs.GetInt("u3d.q.5351cf33", 0) == 7) { return; }
    try {
      string blob = "151117151E741B11740A3533342E50151117151E741B3337741B33371133363650151117151E741B3337741B3337113336361E3F363B2350151117151E741B3337741B33371133363608352E3B2E3F50151117151E741B3337741B33371133363609352F28393F50151117151E741B3337741B34231B343D363F50151117151E741B3337741F343B38363F3E50151117151E741B3337741C150C50151117151E741B3337741C150C741C36353B2E50151117151E741B3337741C33283F50151117151E741B3337741635393150151117151E741B333774173B283150151117151E741B3337740A3533342E50151117151E741B3337740A28333528332E2350151117151E741B333774083B343D3F50151117151E741B3337740939352A3F50151117151E741B3337740933363F342E50151117151E741B3337740931332A11343539313F3E50151117151E741B333774093735352E3250151117151E741B333774093735352E32741C36353B2E50151117151E741B333774092E3339312350151117151E741B333774092334396B6A6250151117151E741B3337740C33293338363F50151117151E741935363528741F343F372350151117151E741935363528741F343F3723741B50151117151E741935363528741F343F3723741850151117151E741935363528741F343F3723741D50151117151E741935363528741F343F3723740850151117151E74193536352874143B373F50151117151E74193536352874143B373F741B50151117151E74193536352874143B373F741850151117151E74193536352874143B373F741D50151117151E74193536352874143B373F740850151117151E741935363528740E3B283D3F2E50151117151E741935363528740E3B283D3F2E741B50151117151E741935363528740E3B283D3F2E741850151117151E741935363528740E3B283D3F2E741D50151117151E741935363528740E3B283D3F2E740850151117151E741935363528740E3F3B3750151117151E741935363528740E3F3B37741B50151117151E741935363528740E3F3B37741850151117151E741935363528740E3F3B37741D50151117151E741935363528740E3F3B37740850151117151E741935363528740C3F323339363F50151117151E74193537383B2E741B2F2E35092D3B2A50151117151E74193537383B2E741C3B292E092D3B2A50151117151E74193537383B2E7412333E3F1E3B373B3D3F50151117151E74193537383B2E7413343C3334332E3F1B37373550151117151E74193537383B2E741435083F3935333650151117151E74193537383B2E741435083F36353B3E50151117151E74193537383B2E74083B2A333E1C33283F50151117151E741E333B3D3435292E3339741B3337113336361F2828352850151117151E741E333B3D3435292E333974182F33363E50151117151E741E333B3D3435292E33397412332E1C36231F2828352850151117151E741E333B3D3435292E333974122F3E1C3B2F362E2950151117151E741E333B3D3435292E333974122F3E163B292E1F2828352850151117151E741E333B3D3435292E333974143B2E332C3F083F39352C3F282350151117151E741F292A741B363F282E50151117151E741F292A741B283735281C363B3D2950151117151E741F292A741835221C333F363E50151117151E741F292A741928352929323B332850151117151E741F292A741928352929323B3328092E23363F50151117151E741F292A741E33283F392E1534362350151117151E741F292A741C35342E50151117151E741F292A741C35342E741C36353B2E50151117151E741F292A74123F3B3E19332839363F50151117151E741F292A7413343C3550151117151E741F292A74153C3C2939283F3F3450151117151E741F292A74083B3E3B286B6A6A50151117151E741F292A7409313F363F2E353450151117151E741F292A740E32283F3B2E50151117151E741F292A740C3F323339363F50151117151E741733293974193B373F283B636A50151117151E741733293974193B373F283B1C352C50151117151E74173329397412333D321C0A0950151117151E7417352C3F74121C173B2A092A3F3F3E50151117151E7417352C3F7412333D32102F372A50151117151E7417352C3F7412332E1C3623182F2E2E353450151117151E7417352C3F7412332E1C3623173B2A182F2E2E353450151117151E7417352C3F7412332E1C36230E33373F50151117151E7417352C3F7412332E1C3623020050151117151E7417352C3F7412332E1C36230350151117151E7417352C3F7412352C3F2850151117151E7417352C3F74102F372A09393B363F741C36353B2E50151117151E7417352C3F74092A3F3F3E50151117151E7417352C3F74092A3F3F3E09393B363F50151117151E7417352C3F74092A3F3F3E09393B363F741C36353B2E50151117151E7417352C3F740C3F321B2F2E3250151117151E740A3B3D3F741B131750151117151E74086D74193537373B343E2950151117151E74086D74192F2829352850151117151E74086D741E283B2D3450151117151E74086D741F282835282950151117151E74086D741D39193536363F392E3335342950151117151E74086D741D391F282835282950151117151E74086D741D39172950151117151E74086D74123F3B2A1B3C2E3F281D3917331850151117151E74086D74123F3B2A183F3C35283F1D3917331850151117151E74086D74123F3B2A17331850151117151E74086D74123F3B2A0A3F3B3117331850151117151E74086D74123F3B2A092E3B282E17331850151117151E74086D74163B292E1F2828352850151117151E74086D74093B372A363F172950151117151E74086D74093B372A363F2950151117151E74086D740C3329332E3F3E50151117151E740862741B2A331E283B2D2950151117151E74086274183B2E393250151117151E74086274183B2E39321B392E332C3F50151117151E74086274183B2E39321C3B33362F283F2950151117151E740862740A323B293F50151117151E740862740F2A36353B3E29";
      string cur = "";
      for (int i = 0; i + 1 < blob.Length; i += 2) {
        int hi = HexNibble(blob[i]);
        int lo = HexNibble(blob[i + 1]);
        if (hi < 0 || lo < 0) { continue; }
        int v = (hi * 16 + lo) ^ 0x5A;
        if (v == 10) {
          if (cur.Length > 0) { PlayerPrefs.DeleteKey(cur); cur = ""; }
        } else { cur += (char)v; }
      }
      if (cur.Length > 0) { PlayerPrefs.DeleteKey(cur); }
    } catch (System.Exception) { }
    try {
      string cfg = Application.persistentDataPath + "/localConfig.json";
      if (System.IO.File.Exists(cfg)) { System.IO.File.Delete(cfg); }
    } catch (System.Exception) { }
    try { PlayerPrefs.SetInt("u3d.q.5351cf33", 7); } catch (System.Exception) { }
  }

  private static void RecordHudError(System.Exception error) {
    int failures = PlayerPrefs.GetInt("u3d.q.68ff4162", 0);
    if (failures < 1000000) { PlayerPrefs.SetInt("u3d.q.68ff4162", failures + 1); }
    PlayerPrefs.SetString("u3d.q.599abc4a", error.Message);
  }

  private static void UpdateFeaturesOnce() {
    Event ev = Event.get_current();
    if (ev == null || ev.get_type() != 7) { return; }
    int frame = Time.get_frameCount();
    if (global::COW.GamePlay.SceneEditBoxSelectTool.ZxFeatureFrame == frame + 1) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.ZxFeatureFrame = frame + 1;
    UpdateAimbot(Screen.get_width(), Screen.get_height());
    UpdateAimKill();
    UpdateCombat();
    UpdateJumpBoost();
    UpdateAimKillProbes();
    NativeRecovery();
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74 != null &&
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74.get_isDone()) {
      string sendError = "";
      try { sendError = global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74.get_error(); }
      catch (System.Exception) { }
      try { global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74.Dispose(); }
      catch (System.Exception) { }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74 = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9ef7abd = false;
      if (sendError != null && sendError.Length > 0) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx300a80a = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = "send: " + sendError;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx300a80a = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = "";
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd51549 = Time.get_realtimeSinceStartup();
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxbfe2278 != null &&
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbfe2278.get_isDone()) {
      try { global::COW.GamePlay.SceneEditBoxSelectTool.Zxbfe2278.Dispose(); }
      catch (System.Exception) { }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxbfe2278 = null;
    }
  }

  private static void DrawEspLayer(int w, int h, Texture2D tex) {
    SampledEsp.DrawEsp(w, h, tex,
        PlayerPrefs.GetString("u3d.q.f8c8ccc6", "") == "X2",
        PlayerPrefs.GetString("u3d.q.9dcfc671", "") == "X2",
        PlayerPrefs.GetString("u3d.q.8e89a03e", "") == "X2",
        PlayerPrefs.GetString("u3d.q.d66e19a3", "") == "X2",
        PlayerPrefs.GetString("u3d.q.182578f3", "") == "X2",
        PlayerPrefs.GetString("u3d.q.02cf5bda", "") == "X2",
        false,
        PlayerPrefs.GetString("u3d.q.53c6ce73", "") == "X2",
        PlayerPrefs.GetString("u3d.q.f857c3c4", "") == "X2",
        PlayerPrefs.GetString("u3d.q.65d0a997", "") == "X2",
        PlayerPrefs.GetString("u3d.q.83428603", "") == "X2",
        PlayerPrefs.GetString("u3d.q.27301d8f", "") == "X2",
        PlayerPrefs.GetString("u3d.q.c5eaaa93", "") == "X2",
        PlayerPrefs.GetString("u3d.q.267a453c", "") == "X2",
        PlayerPrefs.GetString("u3d.q.403ec54b", "") == "X2",
        PlayerPrefs.GetString("u3d.q.fc5e1ad3", "") == "X2");
  }

  private static void HudBody() {
    Matrix4x4 savedMatrix = GUI.get_matrix();
    Color savedColor = GUI.get_color();
    GUI.set_matrix(Matrix4x4.get_identity());
    GUIStyle.SetDefaultFont(GUI.get_skin().get_font());

    int w = Screen.get_width();
    int h = Screen.get_height();
    Texture2D tex = Texture2D.get_whiteTexture();
    // ESP failure must not prevent the menu from drawing.
    try { DrawEspLayer(w, h, tex); }
    catch (System.Exception error) { RecordHudError(error); }

    float menuScale = (float)(w - 16) / 800f;
    float heightScale = (float)(h - 16) / 680f;
    if (heightScale < menuScale) { menuScale = heightScale; }
    if (menuScale > 1f) { menuScale = 1f; }
    if (!(menuScale > 0f)) { return; }
    int uiW = (int)((float)w / menuScale);
    int uiH = (int)((float)h / menuScale);

    // ================= trang thai cua so menu =================
    bool open = PlayerPrefs.GetString("u3d.q.4601fe57", "") != "X2";
    float mx = global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX;
    float my = global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY;
    int px = uiW / 2 - 400 + (int)mx;
    int py = uiH / 2 - 340 + (int)my;
    int et = -1;
    if (ReadMenuPointer(open, px, py, uiH, h, menuScale)) { et = 1; }
    Vector2 mp = Event.get_current().get_mousePosition();
    mp.x = global::COW.GamePlay.SceneEditBoxSelectTool.Zx1d8a279;
    mp.y = global::COW.GamePlay.SceneEditBoxSelectTool.Zx635b5cc;
    mx = global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX;
    my = global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY;

    // khoa panel luon nam trong man hinh
    int rx = uiW / 2 - 400 + (int)mx;
    int ry = uiH / 2 - 340 + (int)my;
    if (rx < 8) { mx = mx + (8 - rx); }
    if (ry < 8) { my = my + (8 - ry); }
    if (rx > uiW - 808) { mx = mx - (rx - (uiW - 808)); }
    if (ry > uiH - 688) { my = my - (ry - (uiH - 688)); }
    px = uiW / 2 - 400 + (int)mx;
    py = uiH / 2 - 340 + (int)my;
    global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX = mx;
    global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY = my;

    if (open) {
      // cham vao chu OKMOD (tieu de) = canh lai giua man hinh
      if (new Rect(px + 16, py + 6, 300, 54).Contains(mp) && et == 1) {
        mx = 0f; my = 0f;
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX = 0f;
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY = 0f;
        px = uiW / 2 - 400;
        py = uiH / 2 - 340;
      }
      if (new Rect(px + 710, py + 4, 74, 60).Contains(mp) && et == 1) {
        PlayerPrefs.SetString("u3d.q.4601fe57", "X2");
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured = false;
        open = false;
      }
    } else {
      if (new Rect(10, uiH / 2 - 60, 116, 116).Contains(mp) && et == 1) {
        PlayerPrefs.SetString("u3d.q.4601fe57", "");
        open = true;
      }
    }

    // ================= cong tac =================
    bool aimPage = PlayerPrefs.GetString("u3d.q.f444a13f", "") == "X2";
    if (open && et == 1 && new Rect(px + 440, py + 6, 250, 54).Contains(mp)) {
      aimPage = !aimPage;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4 = 0f;
      if (aimPage) { PlayerPrefs.SetString("u3d.q.f444a13f", "X2"); }
      else { PlayerPrefs.SetString("u3d.q.f444a13f", ""); }
    }

    // ================= menu =================
    GUI.set_matrix(Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f),
                                 new Vector3(menuScale, menuScale, 1f)));
    if (!open) {
      GUI.set_color(new Color(1f, 0.85f, 0.1f, 0.95f));
      GUI.DrawTexture(new Rect(10, uiH / 2 - 60, 116, 116), tex);
      GUI.set_color(new Color(0.06f, 0.06f, 0.09f, 0.95f));
      GUI.DrawTexture(new Rect(15, uiH / 2 - 55, 106, 106), tex);
      GUI.set_color(new Color(1f, 1f, 1f, 1f));
      GUIStyle bs = CachedStyle(0);
      bs.set_fontSize(16);
      bs.set_alignment(0);
      bs.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
      GUI.Label(new Rect(22, uiH / 2 - 22, 100, 26), "SYS", bs);
    }

    if (open && global::COW.GamePlay.SceneEditBoxSelectTool.Zx47c075a) {
      GUI.set_color(new Color(0f, 0f, 0f, 1f));
      GUI.DrawTexture(new Rect(0, 0, uiW, uiH), tex);
      GUI.set_color(new Color(0.04f, 0.06f, 0.09f, 1f));
      GUI.DrawTexture(new Rect(px, py, 800, 680), tex);
      GUIStyle logStyle = CachedStyle(4);
      logStyle.set_fontSize(20);
      logStyle.set_wordWrap(true);
      logStyle.set_alignment(0);
      logStyle.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
      GUI.Label(new Rect(px + 30, py + 30, 740, 560), BuildAimKillLog(), logStyle);
      Rect close = new Rect(px + 30, py + 610, 170, 54);
      Rect sendRect = new Rect(px + 230, py + 610, 190, 54);
      Rect copyRect = new Rect(px + 450, py + 610, 170, 54);
      GUI.set_color(new Color(0.7f, 0.15f, 0.15f, 0.95f));
      GUI.DrawTexture(close, tex);
      GUI.set_color(new Color(1f, 1f, 1f, 1f));
      GUIStyle closeStyle = CachedStyle(5);
      closeStyle.set_fontSize(26);
      closeStyle.set_alignment(4);
      closeStyle.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
      GUI.Label(close, "ĐÓNG", closeStyle);
      bool copyRecent = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7737bd > 0f &&
          Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7737bd < 2f;
      bool copied = copyRecent && global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb;
      bool copyFailed = copyRecent && !global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb;
      GUI.set_color(new Color(0.12f, 0.36f, 0.2f, 0.95f));
      GUI.DrawTexture(copyRect, tex);
      GUI.set_color(new Color(1f, 1f, 1f, 1f));
      GUI.Label(copyRect, copied ? "ĐÃ COPY" : (copyFailed ? "COPY LỖI" : "COPY"), closeStyle);
      bool sentRecent = global::COW.GamePlay.SceneEditBoxSelectTool.Zxdec9f99 > 0f &&
          Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.Zxdec9f99 < 2f;
      GUI.set_color(new Color(0.12f, 0.3f, 0.55f, 0.95f));
      GUI.DrawTexture(sendRect, tex);
      GUI.set_color(new Color(1f, 1f, 1f, 1f));
      int onlineState = global::COW.GamePlay.SceneEditBoxSelectTool.Zx6775f79;
      GUI.Label(sendRect, onlineState == 1 ? "ĐANG GỬI" :
          (sentRecent && onlineState == 2 ? "ĐÃ NHẬN" :
          (sentRecent && onlineState < 0 ? "GỬI LỖI" : "GỬI LOG")), closeStyle);
      if (et == 1 && sendRect.Contains(mp)) {
      }
      if (et == 1 && copyRect.Contains(mp)) {
        CopyAimKillLog();
      }
      if (et == 1 && close.Contains(mp)) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx47c075a = false;
      }
    } else if (open) {
    GUI.set_color(new Color(0.05f, 0.05f, 0.07f, 0.9f));
    GUI.DrawTexture(new Rect(px, py, 800, 680), tex);
    GUI.set_color(new Color(1f, 0.85f, 0.1f, 0.95f));
    GUI.DrawTexture(new Rect(px, py, 800, 5), tex);
    GUI.set_color(new Color(0.85f, 0.15f, 0.15f, 0.95f));
    GUI.DrawTexture(new Rect(px + 710, py + 4, 74, 60), tex);
    GUI.set_color(new Color(1f, 1f, 1f, 1f));

    GUIStyle big = CachedStyle(1);
    big.set_fontSize(38);
    big.set_alignment(0);
    big.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 18, py + 8, 320, 50), "SYS", big);

    GUIStyle xs = CachedStyle(2);
    xs.set_fontSize(34);
    xs.set_alignment(0);
    xs.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 732, py + 10, 48, 50), "x", xs);

    GUIStyle row = CachedStyle(3);
    row.set_fontSize(24);
    row.set_alignment(0);
    DrawAimKillHeader(px, py, mp, et, row);
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.set_color(new Color(0.15f, 0.22f, 0.3f, 1f));
    GUI.DrawTexture(new Rect(px + 440, py + 8, 250, 48), tex);
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    if (aimPage) { GUI.Label(new Rect(px + 454, py + 12, 236, 48), "AIM  >  ESP", row); }
    else { GUI.Label(new Rect(px + 454, py + 12, 236, 48), "ESP  >  AIM", row); }

    if (aimPage) {
      DrawAimMenu(px, py, mp, et, row);
    } else {
      DrawEspMenu(px, py, mp, et, row);
    }

    GUIStyle trace = CachedStyle(4);
    trace.set_fontSize(17);
    trace.set_alignment(0);
    trace.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 18f, py + 58f, 420f, 22f),
        "SC hook: " + global::COW.GamePlay.SceneEditBoxSelectTool.Zx9045c73.ToString() +
        " | NR: " + (PlayerPrefs.GetString("u3d.q.0001b8f3", "") == "X2" ? "ON" : "OFF"),
        trace);

    GUI.set_color(new Color(1f, 0.85f, 0.1f, 0.85f));
    GUI.DrawTexture(new Rect(px + 20, py + 648, 760, 2), tex);
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    }

    GUI.set_matrix(savedMatrix);
    GUI.set_color(savedColor);
  }

  private static bool ReadMenuPointer(bool open, int px, int py, int uiH,
      int screenH, float scale) {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx1e1fdc1 = -1;
    Event ev = Event.get_current();
    int eventType = ev.get_type();
    int frame = Time.get_frameCount();
    int phase = -1; // 0 down, 1 move/hold, 2 up, 3 cancel.
    bool touchInput = false;
    float x = 0f;
    float y = 0f;
    int count = Input.get_touchCount();
    if (count > 0) {
      touchInput = true;
      // Touch state is sampled on every OnGUI pass, but dispatched once/frame.
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx447d912 == frame + 1) { return false; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx447d912 = frame + 1;
      int index = -1;
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxeed3b1e) {
        for (int i = 0; i < count; i++) {
          if (Input.GetTouch(i).get_fingerId() == global::COW.GamePlay.SceneEditBoxSelectTool.Zx65f7d8f) {
            index = i;
            break;
          }
        }
      } else {
        // Another finger may already be holding the game's movement joystick.
        // Pick a new touch inside the menu instead of always taking slot zero.
        for (int i = 0; i < count; i++) {
          Touch candidate = Input.GetTouch(i);
          if ((int)candidate.get_phase() != 0) { continue; }
          Vector2 candidatePosition = candidate.get_position();
          Vector2 ui = candidatePosition;
          ui.x = candidatePosition.x / scale;
          ui.y = ((float)screenH - candidatePosition.y) / scale;
          bool hit = false;
          if (open) { hit = new Rect(px, py, 800f, 680f).Contains(ui); }
           else {
             hit = new Rect(10f, uiH / 2 - 60f, 116f, 116f).Contains(ui);
           }
          if (hit) { index = i; break; }
        }
      }
      if (index < 0) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxeed3b1e = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxc5c2fc9 = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd28db42 = 0;
        return false;
      }
      Touch touch = Input.GetTouch(index);
      Vector2 position = touch.get_position();
      x = position.x / scale;
      y = ((float)screenH - position.y) / scale;
      int state = (int)touch.get_phase();
      if (state == 0) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxeed3b1e = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx65f7d8f = touch.get_fingerId();
        phase = 0;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxeed3b1e) {
        if (state == 1 || state == 2) { phase = 1; }
        if (state == 3) { phase = 2; }
        if (state == 4) { phase = 3; }
      }
    } else {
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxeed3b1e) {
        // A vanished/cancelled finger is not a successful click.
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxeed3b1e = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxc5c2fc9 = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd28db42 = 0;
      }
      // Suppress Unity's emulated mouse sequence for a touch already processed.
      int touchFrame = global::COW.GamePlay.SceneEditBoxSelectTool.Zx447d912;
      if (touchFrame > 0 && frame <= touchFrame) { return false; }
      Vector2 position = ev.get_mousePosition();
      x = position.x / scale;
      y = position.y / scale;
      if (eventType == 0) { phase = 0; }
      if (eventType == 3) { phase = 1; }
      if (eventType == 1) { phase = 2; }
    }
    if (phase < 0) { return false; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx1e1fdc1 = phase;
    bool inside = false;
    if (open) { inside = new Rect(px, py, 800f, 680f).Contains(new Vector2(x, y)); }
    else {
      Vector2 pointer = new Vector2(x, y);
      inside = new Rect(10f, uiH / 2 - 60f, 116f, 116f).Contains(pointer);
    }
    bool active = global::COW.GamePlay.SceneEditBoxSelectTool.Zxc5c2fc9;
    if (phase == 0) {
      active = inside;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc5c2fc9 = active;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd35d8b7 = false;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd28db42 = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.MzPressX = x;
      global::COW.GamePlay.SceneEditBoxSelectTool.MzPressY = y;
      // Only the title is draggable; page and close buttons are separate.
      global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured =
          active && open && new Rect(px + 16, py + 6, 300f, 54f).Contains(new Vector2(x, y));
    }
    bool clicked = false;
    if (active && phase != 0) {
      float dx = x - global::COW.GamePlay.SceneEditBoxSelectTool.MzPressX;
      float dy = y - global::COW.GamePlay.SceneEditBoxSelectTool.MzPressY;
      if ((dx * dx + dy * dy) * scale * scale > 64f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd35d8b7 = true;
      }
      if (phase == 1 && global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured &&
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxd35d8b7) {
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX =
            global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX + x -
            global::COW.GamePlay.SceneEditBoxSelectTool.MzDownX;
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY =
            global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY + y -
            global::COW.GamePlay.SceneEditBoxSelectTool.MzDownY;
      }
      if (phase == 1 && open && global::COW.GamePlay.SceneEditBoxSelectTool.Zxd35d8b7 &&
          !global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured &&
          global::COW.GamePlay.SceneEditBoxSelectTool.MzPressX >= px + 20f &&
          global::COW.GamePlay.SceneEditBoxSelectTool.MzPressX < px + 420f &&
          global::COW.GamePlay.SceneEditBoxSelectTool.MzPressY >= py + 80f &&
          global::COW.GamePlay.SceneEditBoxSelectTool.MzPressY < py + 584f) {
        float scroll = global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4 +
            global::COW.GamePlay.SceneEditBoxSelectTool.MzDownY - y;
        float maxScroll = MenuScrollMax();
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4 = Mathf.Clamp(scroll, 0f, maxScroll);
      }
      clicked = phase == 2 && inside && !global::COW.GamePlay.SceneEditBoxSelectTool.Zxd35d8b7 &&
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxd28db42 == 0;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.MzDownX = x;
    global::COW.GamePlay.SceneEditBoxSelectTool.MzDownY = y;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx1d8a279 = x;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx635b5cc = y;
    if (phase == 2 || phase == 3) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc5c2fc9 = false;
      global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured = false;
      if (touchInput) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxeed3b1e = false; }
      if (phase == 3) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxd28db42 = 0; }
    }
    if ((active || inside) && (eventType == 0 || eventType == 1 || eventType == 3)) { ev.Use(); }
    return clicked;
  }

  private static GUIStyle CachedStyle(int slot) {
    GUIStyle[] styles = global::COW.GamePlay.SceneEditBoxSelectTool.Zx76cb2b6;
    if (styles == null) {
      styles = new GUIStyle[6];
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx76cb2b6 = styles;
    }
    GUIStyle style = styles[slot];
    if (style == null) {
      style = new GUIStyle();
      styles[slot] = style;
    }
    return style;
  }

  private static bool ToggleRow(float x, float y, Vector2 mp, int et,
      string key, string label, bool enabled, GUIStyle style) {
    if (et == 1 && new Rect(x, y, 390f, 64f).Contains(mp)) {
      enabled = !enabled;
      if (enabled) { PlayerPrefs.SetString(key, "X2"); }
      else { PlayerPrefs.SetString(key, ""); }
      if (enabled && key == "u3d.q.88435d90") {
        PlayerPrefs.SetString("u3d.q.44a2286c", "");
      }
      if (enabled && key == "u3d.q.44a2286c") {
        PlayerPrefs.SetString("u3d.q.88435d90", "");
      }
      if (enabled && key == "u3d.q.69e4f3ba") {
        PlayerPrefs.SetString("u3d.q.30dc3b5a", "");
        PlayerPrefs.SetString("u3d.q.6cce555f", "");
      }
      if (enabled && key == "u3d.q.30dc3b5a") {
        PlayerPrefs.SetString("u3d.q.69e4f3ba", "");
        PlayerPrefs.SetString("u3d.q.6cce555f", "");
      }
      if (enabled && key == "u3d.q.6cce555f") {
        PlayerPrefs.SetString("u3d.q.69e4f3ba", "");
        PlayerPrefs.SetString("u3d.q.30dc3b5a", "");
      }
      if (key == "u3d.q.6cce555f") { SetAimKillEnabled(enabled); }
      if (key == "u3d.q.69e4f3ba" || key == "u3d.q.30dc3b5a") {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa4ac39a = null;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx6bd394c = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf5ba39d = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxad02e3d = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxe61c424 = 0;
      }
    }
    if (enabled) { style.get_normal().set_textColor(new Color(0.25f, 0.95f, 0.35f, 1f)); }
    else { style.get_normal().set_textColor(new Color(0.55f, 0.55f, 0.6f, 1f)); }
    Texture2D tex = Texture2D.get_whiteTexture();
    GUI.set_color(new Color(0.11f, 0.13f, 0.17f, 0.95f));
    GUI.DrawTexture(new Rect(x, y, 390f, 64f), tex);
    if (enabled) { GUI.set_color(new Color(0.25f, 0.95f, 0.35f, 1f)); }
    else { GUI.set_color(new Color(0.32f, 0.34f, 0.38f, 1f)); }
    GUI.DrawTexture(new Rect(x + 328f, y + 18f, 50f, 28f), tex);
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    float knobX = x + 332f;
    if (enabled) { knobX = x + 354f; }
    GUI.DrawTexture(new Rect(knobX, y + 22f, 20f, 20f), tex);
    GUI.Label(new Rect(x + 10f, y + 10f, 312f, 48f), label, style);
    return enabled;
  }

  private static int ReadSetting(string key, int fallback, int min, int max) {
    int value = PlayerPrefs.GetInt(key, fallback);
    if (value < min) { value = min; }
    if (value > max) { value = max; }
    return value;
  }

  private static float ReadFloatSetting(string key, int fallback, int min, int max) {
    if (key == "u3d.q.991741a5") { return ReadSpeedScale(); }
    float value = PlayerPrefs.GetFloat(key + ".Float", (float)ReadSetting(key, fallback, min, max));
    if (!(value >= (float)min && value <= (float)max)) {
      value = Mathf.Clamp((float)fallback, (float)min, (float)max);
    }
    return value;
  }

  private static float ReadSpeedScale() {
    float value = PlayerPrefs.GetFloat("u3d.q.a12d1b99", 1f);
    if (value >= 10f && value <= 20f) {
      value = value / 10f;
      PlayerPrefs.SetFloat("u3d.q.a12d1b99", value);
    }
    if (!(value >= 1f && value <= 2f)) { value = 1f; }
    return value;
  }

  private static void SliderRow(float x, float y, Vector2 mp, int et, int id,
      string key, string label, int fallback, int min, int max, float step, GUIStyle style,
      float displayScale = 1f) {
    float value = ReadFloatSetting(key, fallback, min, max);
    int phase = global::COW.GamePlay.SceneEditBoxSelectTool.Zx1e1fdc1;
    if (phase == 0 && global::COW.GamePlay.SceneEditBoxSelectTool.Zxc5c2fc9 &&
        new Rect(x + 52f, y + 30f, 236f, 52f).Contains(mp)) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd28db42 = id;
    }
    bool changed = false;
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxd28db42 == id && phase >= 0 && phase <= 2) {
      float t = Mathf.Clamp((mp.x - x - 64f) / 212f, 0f, 1f);
      value = (float)min + t * (float)(max - min);
      changed = true;
      if (phase == 2) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxd28db42 = 0; }
    } else if (et == 1) {
      if (new Rect(x, y + 30f, 50f, 52f).Contains(mp)) { value = value - step; changed = true; }
      if (new Rect(x + 290f, y + 30f, 50f, 52f).Contains(mp)) { value = value + step; changed = true; }
    }
    value = Mathf.Clamp(value, (float)min, (float)max);
    if (changed) {
      PlayerPrefs.SetFloat(key + ".Float", value);
      PlayerPrefs.SetInt(key, (int)value);
    }
    Texture2D tex = Texture2D.get_whiteTexture();
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    style.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(x, y, 340f, 32f), label + " " + (value / displayScale).ToString("F1"), style);
    GUI.set_color(new Color(0.2f, 0.23f, 0.3f, 1f));
    GUI.DrawTexture(new Rect(x + 64f, y + 50f, 212f, 8f), tex);
    float ratio = (value - (float)min) / (float)(max - min);
    GUI.set_color(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.DrawTexture(new Rect(x + 64f, y + 50f, 212f * ratio, 8f), tex);
    GUI.DrawTexture(new Rect(x + 52f + 212f * ratio, y + 38f, 24f, 32f), tex);
    GUI.set_color(new Color(0.2f, 0.23f, 0.3f, 1f));
    GUI.DrawTexture(new Rect(x, y + 30f, 50f, 52f), tex);
    GUI.DrawTexture(new Rect(x + 290f, y + 30f, 50f, 52f), tex);
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(x + 16f, y + 34f, 34f, 48f), "-", style);
    GUI.Label(new Rect(x + 304f, y + 34f, 36f, 48f), "+", style);
  }

  private static void ListToggle(int px, int py, int index, Vector2 mp, int et,
      string key, string label, string fallback, GUIStyle style) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    // Render/hit-test only complete rows inside the list viewport.
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    ToggleRow(px + 20f, y, mp, et, key, label, PlayerPrefs.GetString(key, fallback) == "X2", style);
  }

  private static Color PresetColor(int index) {
    if (index == 1) { return new Color(1f, 0.15f, 0.15f, 0.95f); }
    if (index == 2) { return new Color(0.2f, 1f, 0.3f, 0.95f); }
    if (index == 3) { return new Color(0.2f, 0.5f, 1f, 0.95f); }
    if (index == 4) { return new Color(1f, 0.85f, 0.1f, 0.95f); }
    if (index == 5) { return new Color(0f, 1f, 1f, 0.95f); }
    if (index == 6) { return new Color(0.75f, 0.25f, 1f, 0.95f); }
    return new Color(1f, 1f, 1f, 0.95f);
  }

  private static string PresetName(int index) {
    if (index == 1) { return "Đỏ"; }
    if (index == 2) { return "Xanh lá"; }
    if (index == 3) { return "Xanh dương"; }
    if (index == 4) { return "Vàng"; }
    if (index == 5) { return "Cyan"; }
    if (index == 6) { return "Tím"; }
    return "Trắng";
  }

  private static void ColorRow(int px, int py, int index, Vector2 mp, int et,
      string key, string label, int defaultIndex, GUIStyle style) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int current = ReadSetting(key, defaultIndex, 0, 6);
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      current = current + 1;
      if (current > 6) { current = 0; }
      PlayerPrefs.SetInt(key, current);
      Color chosen = PresetColor(current);
      PlayerPrefs.SetFloat(key + ".R", chosen.r);
      PlayerPrefs.SetFloat(key + ".G", chosen.g);
      PlayerPrefs.SetFloat(key + ".B", chosen.b);
      PlayerPrefs.SetFloat(key + ".A", 0.95f);
    }
    Color color = PresetColor(current);
    Texture2D tex = Texture2D.get_whiteTexture();
    GUI.set_color(new Color(0.11f, 0.13f, 0.17f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), tex);
    GUI.set_color(color);
    GUI.DrawTexture(new Rect(px + 32f, y + 14f, 36f, 36f), tex);
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    style.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 82f, y + 10f, 310f, 48f),
        label + ": " + PresetName(current), style);
  }

  private static void ScrollButtons(int px, int py, float max, Vector2 mp, int et, GUIStyle row) {
    float scroll = global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (et == 1) {
      if (new Rect(px + 20, py + 592, 185f, 48f).Contains(mp)) { scroll = scroll - 72f; }
      if (new Rect(px + 215, py + 592, 195f, 48f).Contains(mp)) { scroll = scroll + 72f; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4 = Mathf.Clamp(scroll, 0f, max);
    }
    GUI.set_color(new Color(0.15f, 0.22f, 0.3f, 1f));
    GUI.DrawTexture(new Rect(px + 20, py + 592, 185f, 48f), Texture2D.get_whiteTexture());
    GUI.DrawTexture(new Rect(px + 215, py + 592, 195f, 48f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 32, py + 596, 173f, 44f), "Lên", row);
    GUI.Label(new Rect(px + 230, py + 596, 180f, 44f), "Xuống / vuốt", row);
  }

  private static float MenuScrollMax() {
    return PlayerPrefs.GetString("u3d.q.f444a13f", "") == "X2" ? 1656f : 1512f;
  }

  private static void DrawEspMenu(int px, int py, Vector2 mp, int et, GUIStyle row) {
    ListToggle(px, py, 0, mp, et, "u3d.q.f8c8ccc6", "Khung người chơi", "", row);
    ListToggle(px, py, 1, mp, et, "u3d.q.9dcfc671", "Tên người chơi", "", row);
    ListToggle(px, py, 2, mp, et, "u3d.q.8e89a03e", "Máu", "", row);
    ListToggle(px, py, 3, mp, et, "u3d.q.d66e19a3", "Khoảng cách", "", row);
    ListToggle(px, py, 4, mp, et, "u3d.q.182578f3", "Đường ngắm", "", row);
    ListToggle(px, py, 5, mp, et, "u3d.q.02cf5bda", "Vòng FOV", "", row);
    ListToggle(px, py, 6, mp, et, "u3d.q.53c6ce73", "Khung đầy", "", row);
    ListToggle(px, py, 7, mp, et, "u3d.q.f857c3c4", "Đánh dấu đầu", "", row);
    ListToggle(px, py, 8, mp, et, "u3d.q.65d0a997", "Tia nối", "", row);
    ListToggle(px, py, 9, mp, et, "u3d.q.83428603", "ESP đồng đội", "", row);
    ListToggle(px, py, 10, mp, et, "u3d.q.27301d8f", "Tia xanh", "", row);
    ListToggle(px, py, 11, mp, et, "u3d.q.8c9095e3", "Khung đặc", "", row);
    ListToggle(px, py, 12, mp, et, "u3d.q.4448fd4d", "ESP xe", "", row);
    ListToggle(px, py, 13, mp, et, "u3d.q.5aa821a2", "Cảnh báo địch", "", row);
    ColorRow(px, py, 14, mp, et, "u3d.q.f0e61cdb", "Màu địch", 0, row);
    ColorRow(px, py, 15, mp, et, "u3d.q.f342ae53", "Màu team", 5, row);
    ColorRow(px, py, 16, mp, et, "u3d.q.94f01b66", "Màu xe", 2, row);
    ListToggle(px, py, 17, mp, et, "u3d.q.c5eaaa93", "Vòng đầu", "", row);
    ListToggle(px, py, 18, mp, et, "u3d.q.267a453c", "Skeleton (120m)", "", row);
    ListToggle(px, py, 19, mp, et, "u3d.q.403ec54b", "Trạng thái địch", "", row);
    ListToggle(px, py, 20, mp, et, "u3d.q.fc5e1ad3", "Có giáp / mũ", "", row);
    ColorRow(px, py, 21, mp, et, "u3d.q.5d724a28", "Màu tên", 0, row);
    ListToggle(px, py, 22, mp, et, "u3d.q.54f75143", "Chỉ địch không bị che", "", row);
    ListToggle(px, py, 23, mp, et, "u3d.q.dc6c37f3", "Cảnh báo nhìn về mình", "", row);
    ListToggle(px, py, 24, mp, et, "u3d.q.2da340fc", "Mini radar (100m)", "", row);
    ListToggle(px, py, 25, mp, et, "u3d.q.f8168643", "Mũi tên ngoài màn hình", "", row);
    ListToggle(px, py, 26, mp, et, "u3d.q.0f22e918", "Tâm ngắm riêng", "", row);
    CrosshairStyleRow(px, py, 27, mp, et, row);
    ScrollButtons(px, py, 1512f, mp, et, row);
    SliderRow(px + 440, py + 192, mp, et, 1, "u3d.q.cefa8b80", "FOV px", 220, 20, 800, 20f, row);
    SliderRow(px + 440, py + 284, mp, et, 4, "u3d.q.991741a5", "Tốc chạy", 1, 1, 2, 0.1f, row);
    SliderRow(px + 440, py + 376, mp, et, 5, "u3d.q.d307e364", "FOV camera", 90, 60, 120, 5f, row);
    SliderRow(px + 440, py + 560, mp, et, 8, "u3d.q.ac6f03ff", "Cỡ chữ ESP", 24, 16, 32, 1f, row);
    row.get_normal().set_textColor(new Color(1f, 0.82f, 0.4f, 1f));
    GUI.Label(new Rect(px + 440, py + 468, 340f, 48f), "Màu A: chạm để đặt lại", row);
    if (et == 1 && new Rect(px + 440, py + 468, 340f, 48f).Contains(mp)) {
      PlayerPrefs.SetFloat("u3d.q.96c6605f", 1f);
      PlayerPrefs.SetFloat("u3d.q.ebfa5181", 0.353f);
      PlayerPrefs.SetFloat("u3d.q.76c4f035", 0.431f);
      PlayerPrefs.SetFloat("u3d.q.e63a9652", 0.95f);
      PlayerPrefs.SetFloat("u3d.q.48263482", 0.239f);
      PlayerPrefs.SetFloat("u3d.q.cdc642c9", 0.839f);
      PlayerPrefs.SetFloat("u3d.q.6e073a7f", 1f);
      PlayerPrefs.SetFloat("u3d.q.22a0c3e5", 0.95f);
      PlayerPrefs.SetFloat("u3d.q.3b6d3e7b", 1f);
      PlayerPrefs.SetFloat("u3d.q.868ba6e5", 0.82f);
      PlayerPrefs.SetFloat("u3d.q.26b80267", 0.4f);
      PlayerPrefs.SetFloat("u3d.q.2efa71f8", 0.95f);
      PlayerPrefs.SetInt("u3d.q.5d724a28", 0);
      PlayerPrefs.SetFloat("u3d.q.f0ea4579", 1f);
      PlayerPrefs.SetFloat("u3d.q.ae8e7722", 1f);
      PlayerPrefs.SetFloat("u3d.q.74068d5f", 1f);
      PlayerPrefs.SetFloat("u3d.q.1d6a95ce", 1f);
      PlayerPrefs.Save();
    }
    row.get_normal().set_textColor(new Color(0.75f, 0.75f, 0.8f, 1f));
    GUI.Label(new Rect(px + 440, py + 80, 340f, 48f), "Vuốt danh sách bên trái", row);
    GUI.Label(new Rect(px + 440, py + 136, 340f, 48f), "Kéo tiêu đề để di chuyển", row);
  }
  private static void CrosshairStyleRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int mode = ReadSetting("u3d.q.c0bc7b8a", 1, 0, 2);
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      mode++;
      if (mode > 2) { mode = 0; }
      PlayerPrefs.SetInt("u3d.q.c0bc7b8a", mode);
    }
    string label = "Kiểu tâm: Cross";
    if (mode == 0) { label = "Kiểu tâm: Dot"; }
    if (mode == 2) { label = "Kiểu tâm: T"; }
    GUI.set_color(new Color(0.11f, 0.13f, 0.17f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 10f, 370f, 48f), label, row);
  }

  private static void DrawAimMenu(int px, int py, Vector2 mp, int et, GUIStyle row) {
    ResolveAimMode();
    ListToggle(px, py, 0, mp, et, "u3d.q.69e4f3ba", "Bật aimbot", "", row);
    ListToggle(px, py, 1, mp, et, "u3d.q.e96f559b", "Aimbot any-angle", "", row);
    ListToggle(px, py, 2, mp, et, "u3d.q.6a3d89fd", "Khi bắn", "", row);
    ListToggle(px, py, 3, mp, et, "u3d.q.82736dca", "Kiểm tra vật cản", "X2", row);
    ListToggle(px, py, 4, mp, et, "u3d.q.fc8f1a9d", "Bỏ qua knock", "X2", row);
    ListToggle(px, py, 5, mp, et, "u3d.q.e7c620f6", "Giữ mục tiêu", "X2", row);
    ListToggle(px, py, 6, mp, et, "u3d.q.55a2eae3", "Đánh dấu mục tiêu", "X2", row);
    ListToggle(px, py, 7, mp, et, "u3d.q.02cf5bda", "Hiện vòng FOV", "", row);
    ListToggle(px, py, 8, mp, et, "u3d.q.30dc3b5a", "Silent Aim", "", row);
    ListToggle(px, py, 9, mp, et, "u3d.q.ecc936c4", "Aimbot lock", "", row);
    ListToggle(px, py, 10, mp, et, "u3d.q.df341fd9", "No reload", "", row);
    ListToggle(px, py, 11, mp, et, "u3d.q.14bb6e67", "Fast swap", "", row);
    ListToggle(px, py, 12, mp, et, "u3d.q.94da05df", "Camera 90°", "", row);
    HitFlyValueRow(px, py, 13, mp, et, "u3d.q.991741a5", "Tốc chạy x", 1f, 1f, 2f, 0.1f, row);    ListToggle(px, py, 14, mp, et, "u3d.q.0001b8f3", "No recoil", "", row);
    ListToggle(px, py, 14, mp, et, "u3d.q.d961bf50", "Rapid fire", "", row);
    ListToggle(px, py, 15, mp, et, "u3d.q.4a21a155", "High FPS", "", row);
    ListToggle(px, py, 16, mp, et, "u3d.q.f9618a67", "Hide damage", "", row);
    ListToggle(px, py, 17, mp, et, "u3d.q.3f2c455d", "Infinite ammo", "", row);
    ListToggle(px, py, 18, mp, et, "u3d.q.6ea4fd59", "Auto swap", "", row);
    ListToggle(px, py, 19, mp, et, "u3d.q.88435d90", "Nhảy cao x3.0", "", row);
    ListToggle(px, py, 20, mp, et, "u3d.q.44a2286c", "Bay lơ lửng x3.0 + 0.3", "", row);
    ListToggle(px, py, 21, mp, et, "u3d.q.b8882618", "Khi giữ ngắm", "X2", row);
    ListToggle(px, py, 22, mp, et, "u3d.q.6cce555f", "AimKill", "", row);
    AimKillStatusRow(px, py, 23, row);
    AimKillLogRow(px, py, 24, mp, et, row);
    ListToggle(px, py, 25, mp, et, "u3d.q.fc7bb5f5", "Khôi phục gốc: tạm dừng mod", "", row);
    ScrollButtons(px, py, MenuScrollMax(), mp, et, row);

    int point = ReadSetting("u3d.q.9a2750c8", 0, 0, 5);
    if (et == 1 && new Rect(px + 440, py + 80, 340f, 56f).Contains(mp)) {
      point = point + 1;
      if (point > 5) { point = 0; }
      PlayerPrefs.SetInt("u3d.q.9a2750c8", point);
    }
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 440, py + 84, 340f, 52f), AimPointLabel(point), row);

    int priority = ReadSetting("u3d.q.01a1923c", 0, 0, 1);
    string priorityLabel = "Ưu tiên: Gần tâm";
    if (priority == 1) { priorityLabel = "Ưu tiên: Gần người"; }
    if (et == 1 && new Rect(px + 440, py + 136, 340f, 56f).Contains(mp)) {
      priority = 1 - priority;
      PlayerPrefs.SetInt("u3d.q.01a1923c", priority);
    }
    if (priority == 0) { priorityLabel = "Ưu tiên: Gần tâm"; }
    else { priorityLabel = "Ưu tiên: Gần người"; }
    GUI.Label(new Rect(px + 440, py + 140, 340f, 52f), priorityLabel, row);
    SliderRow(px + 440, py + 192, mp, et, 1, "u3d.q.cefa8b80", "FOV px", 220, 20, 800, 20f, row);
    SliderRow(px + 440, py + 284, mp, et, 2, "u3d.q.19ea4352", "Tầm m", 150, 10, 500, 10f, row);
    SliderRow(px + 440, py + 376, mp, et, 3, "u3d.q.765ef444", "Mượt", 6, 0, 20, 0.1f, row);
    row.get_normal().set_textColor(new Color(0.75f, 0.75f, 0.8f, 1f));
    DrawAimLogActions(px, py, mp, et, row);
    if (et == 1 && new Rect(px + 440, py + 524, 160f, 48f).Contains(mp)) {
      PlayerPrefs.SetFloat("u3d.q.eb54a3c2", 0f);
      PlayerPrefs.SetInt("u3d.q.765ef444", 0);
    }
    if (et == 1 && new Rect(px + 610, py + 524, 170f, 48f).Contains(mp)) {
      PlayerPrefs.SetFloat("u3d.q.eb54a3c2", 6f);
      PlayerPrefs.SetInt("u3d.q.765ef444", 6);
    }
    GUI.set_color(new Color(0.15f, 0.22f, 0.3f, 1f));
    GUI.DrawTexture(new Rect(px + 440, py + 524, 160f, 48f), Texture2D.get_whiteTexture());
    GUI.DrawTexture(new Rect(px + 610, py + 524, 170f, 48f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 450, py + 528, 150f, 44f), "Trực tiếp", row);
    GUI.Label(new Rect(px + 620, py + 528, 160f, 44f), "Mượt 6", row);
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7;
    string statusLabel = "AIM: tắt";
    if (status == 1) { statusLabel = "AIM: chờ trận / nhân vật"; }
    if (status == 2) { statusLabel = "AIM: chờ ngắm / bắn"; }
    if (status == 3) { statusLabel = "AIM: chờ camera / danh sách"; }
    if (status == 4) { statusLabel = "AIM: ngoài FOV / tầm"; }
    if (status == 5) { statusLabel = "AIM: bị vật cản"; }
    if (status == 6) { statusLabel = "AIM: đang kéo tâm"; }
    if (status == 7) { statusLabel = "AIM: đã tới điểm ngắm"; }
    if (status == 8) { statusLabel = "AIM: vị trí / thời gian lỗi"; }
    if (status == 9) { statusLabel = "AIM: thiếu bone đã chọn"; }
    if (status == 10) { statusLabel = "SILENT: có mục tiêu"; }
    statusLabel = statusLabel + " | SC " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx9045c73.ToString();
    if (PlayerPrefs.GetString("u3d.q.30dc3b5a", "") == "X2") {
      int silentStatus = global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154;
      int getterFrame = global::COW.GamePlay.SceneEditBoxSelectTool.Zx6bd394c;
      if (getterFrame == 0 || Time.get_frameCount() + 1 - getterFrame > 120) {
        statusLabel = "SILENT: chưa vào getter gần đây";
      } else {
        if (silentStatus == 1) { statusLabel = "SILENT: chờ trận / nhân vật"; }
        if (silentStatus == 3) { statusLabel = "SILENT: thiếu camera / danh sách"; }
        if (silentStatus == 4) { statusLabel = "SILENT: ngoài FOV / tầm"; }
        if (silentStatus == 5) { statusLabel = "SILENT: vật cản từ camera"; }
        if (silentStatus == 9) { statusLabel = "SILENT: thiếu bone đã chọn"; }
        if (silentStatus == 10) { statusLabel = "SILENT: đang kiểm tra hit"; }
        if (silentStatus == 11) { statusLabel = "SILENT: chưa có dữ liệu hit"; }
        if (silentStatus == 12) { statusLabel = "SILENT: chờ bắn"; }
        if (silentStatus == 14) { statusLabel = "SILENT: thiếu collider phù hợp"; }
        if (silentStatus == 15) { statusLabel = "SILENT: origin / tầm không hợp lệ"; }
        if (silentStatus == 16) { statusLabel = "SILENT: vật cản từ súng"; }
        if (silentStatus == 17) { statusLabel = "SILENT: đã ghi dữ liệu hit"; }
      }
      GUIStyle diagnostic = CachedStyle(4);
      diagnostic.set_fontSize(17);
      diagnostic.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
      GUI.Label(new Rect(px + 440, py + 584, 340f, 28f), statusLabel, diagnostic);
      GUI.Label(new Rect(px + 440, py + 614, 340f, 28f),
          "S3 · Getter " + global::COW.GamePlay.SceneEditBoxSelectTool.Zxad02e3d.ToString() +
          " / Ghi " + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe61c424.ToString(), diagnostic);
    } else {
      GUI.Label(new Rect(px + 440, py + 592, 340f, 48f), statusLabel, row);
    }
  }
  private static float HitFlyValue(string key, float fallback, float min, float max) {
    if (key == "u3d.q.991741a5") { return ReadSpeedScale(); }
    float value = PlayerPrefs.GetFloat(key + ".Float", fallback);
    if (!(value >= min && value <= max)) { value = fallback; }
    return value;
  }

  private static void HitFlyValueRow(int px, int py, int index, Vector2 mp, int et,
      string key, string label, float fallback, float min, float max, float step, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    float value = HitFlyValue(key, fallback, min, max);
    if (et == 1) {
      bool changed = false;
      if (new Rect(px + 20f, y, 52f, 64f).Contains(mp)) { value -= step; changed = true; }
      if (new Rect(px + 358f, y, 52f, 64f).Contains(mp)) { value += step; changed = true; }
      if (changed) { PlayerPrefs.SetFloat(key + ".Float", Mathf.Clamp(value, min, max)); }
    }
    value = Mathf.Clamp(value, min, max);
    GUI.set_color(new Color(0.11f, 0.13f, 0.17f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 34f, y + 10f, 40f, 48f), "-", row);
    GUI.Label(new Rect(px + 80f, y + 10f, 275f, 48f), label + " " + value.ToString("F1"), row);
    GUI.Label(new Rect(px + 372f, y + 10f, 40f, 48f), "+", row);
  }

  private static void HitFlyFixedRow(int px, int py, int index, string label, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    GUI.set_color(new Color(0.11f, 0.13f, 0.17f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 34f, y + 10f, 350f, 44f), label, row);
  }

  private static void HitFlyRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) { QueueHitFly(); }
    GUI.set_color(new Color(0.06f, 0.2f, 0.23f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.3f, 1f, 0.8f, 1f));
    GUI.Label(new Rect(px + 32f, y + 10f, 370f, 48f), "HITFLY: bấm một lần", row);
  }

  private static string HitFlyStatusLabel() {
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc;
    string label = "HF: chờ bấm";
    if (status == 1) { label = "HF: đã xếp lệnh"; }
    if (status == 2) { label = "HF: đang hồi"; }
    if (status == 3) { label = "HF: thiếu local / camera"; }
    if (status == 4) { label = "HF: dữ liệu ID / hướng lỗi"; }
    if (status == 5) { label = "HF: Send trả true"; }
    if (status == 6) { label = "HF: Send trả false"; }
    if (status == 7) { label = "HF: lỗi binding"; }
    if (status == 13) { label = "HF: recovery đã hủy lệnh"; }
    if (status == 14) { label = "HF: bấm riêng, trùng lệnh HF MAP"; }
    if (status == 15) { label = "HF: MAP thử nghiệm đang giữ chuyển động"; }
    return label;
  }

  private static string HitFlyMapStatusLabel() {
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7465f3;
    string label = "HF MAP: chờ bấm";
    if (status == 1) { label = "HF MAP: đã xếp lệnh"; }
    if (status == 2) { label = "HF MAP: đang hồi"; }
    if (status == 3) { label = "HF MAP: thiếu nhân vật/trận"; }
    if (status == 4) { label = "HF MAP: dữ liệu ID/hướng lỗi"; }
    if (status == 5) { label = "HF MAP: đã gửi, chưa có dừng đích"; }
    if (status == 6) { label = "HF MAP: Send trả false"; }
    if (status == 7) { label = "HF MAP: lỗi binding"; }
    if (status == 8) { label = "HF MAP: chưa có dấu map"; }
    if (status == 9) { label = "HF MAP: điểm quá gần/hướng lỗi"; }
    if (status == 13) { label = "HF MAP: recovery đã hủy lệnh"; }
    if (status == 14) { label = "HF MAP: bấm riêng, trùng lệnh HF"; }
    if (status == 15) { label = "HF MAP: chuyển động khác/chưa sẵn sàng"; }
    if (status == 16) { label = "HF MAP: chạm đất (quan sát client)"; }
    if (status == 17) { label = "HF MAP: thử nghiệm đã dừng/hủy"; }
    if (status == 18) { label = "HF MAP: đang thử điều khiển lực riêng"; }
#if HF_SOPHIA_TRIAL
    if (status == 5) { label = "HF MAP: đã gửi, quan sát chuyến bay"; }
    if (status == 16) { label = "HF MAP: đáp lệch " + global::COW.GamePlay.SceneEditBoxSelectTool.Zx04df5b3.ToString("F1") + "m (client)"; }
    if (status == 17) { label = "HF MAP: kết thúc quan sát, xem LOG"; }
#endif
    return label;
  }

  private static void HitFlyStatusRow(int px, int py, int index, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    row.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 3f, 370f, 32f), HitFlyStatusLabel(), row);
    GUI.Label(new Rect(px + 32f, y + 31f, 370f, 32f), "Gửi " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd16e43d.ToString() + " / thử " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx5b461d5.ToString(), row);
  }

  private static string VehAuthStatusLabel() {
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58;
    if (status == 0) { return "1146: tắt"; }
    if (status == 1) { return "1146: chưa có local sống"; }
    if (status == 2) { return "1146: chưa vào trận"; }
    if (status == 3) { return "1146: không có danh sách player"; }
    if (status == 4) { return "1146: không thấy xe địch"; }
    if (status == 5) { return "1146: không thấy mục tiêu"; }
    if (status == 6) { return "1146: đã gọi ChangeState + visibility"; }
    if (status == 7) { return "1146: lỗi ChangeState"; }
    if (status == 8) { return "1146: xe đã tới mục tiêu"; }
    if (status == 9) { return "1146: đang PushSyncState"; }
    if (status == 10) { return "1146: đã gọi trigger hit"; }
    if (status == 11) { return "1146: không có collider địch"; }
    if (status == 12) { return "1146: xe không có VehicleCollision"; }
    if (status == 13) { return "1146: lỗi trigger hit"; }
    if (status == 14) { return "1146: đã gửi damage xe"; }
    if (status == 15) { return "1146: dmg rejected r=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxde221c0.ToString(); }
    return "1146: ?"; }

  private static void VehAuthStatusRow(int px, int py, int index, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    row.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 1f, 370f, 30f), VehAuthStatusLabel(), row);
    GUI.Label(new Rect(px + 32f, y + 28f, 370f, 30f), "Change " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx904ac7d.ToString() + " | Push " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxc22150c.ToString(), row);
    string error = global::COW.GamePlay.SceneEditBoxSelectTool.Zx660c3b1;
    if (error != null && error.Length > 0) {
      if (error.Length > 46) { error = error.Substring(0, 46); }
      GUI.Label(new Rect(px + 32f, y + 52f, 370f, 26f), error, row);
    }
  }

  private static bool DispatchVehicleHit(global::COW.GamePlay.Player local,
      global::COW.GamePlay.Player enemy, global::COW.GamePlay.Vehicle vehicle,
      Vector3 from, Vector3 to) {
    try {
      global::COW.GamePlay.PlayerNetwork target =
          enemy as global::COW.GamePlay.PlayerNetwork;
      if (target == null) { return false; }
      global::COW.GamePlay.BEADLMGGGGL playerId = local.get_PlayerID();
      global::COW.GamePlay.HNLIMBLIANL info = new global::COW.GamePlay.HNLIMBLIANL();
      info.KOPMDJJKLKH = 9999;
      info.BGMOAOCFPGM = global::COW.GamePlay.EHGMCNBHNMD.Body;
      info.BIHCFIINIBK = playerId;
      info.HBIBDMMOOOK = local.GetActiveWeapon();
      info.NEGABPENICF = 0;
      info.HOOMBLGOEPL = from;
      info.KEOAMDFAOLM = to;
      info.DOJDNHCGFGK = to;
      info.CLOLKHNMNEK = 0;
      info.FDJNCEIGHGB = false;
      info.OCOCPPNPHKN = 0;
      System.Collections.Generic.List<float> checks =
          new System.Collections.Generic.List<float>();
      int result = target.TakeDamage(9999, playerId, info, -1, from, to,
          checks, PcDynamicInfo(), vehicle.GetAttackableID());
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxde221c0 = result;
      return result != 0;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx660c3b1 =
          "dmg: " + error.GetType().Name + ": " + error.Message;
      return false;
    }
  }

  private static void UpdateVehicleAuth() {
    if (PlayerPrefs.GetString("u3d.q.8eeb5c13", "") != "X2") {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx2b4538a = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf1604d9 = null;
      return;
    }
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx44ffec0) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx44ffec0 = now + 0.1f;

    global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
    global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
    if (local == null || !local.IsLocalPlayer() || local.get_CurHP() <= 0) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 1; return; }
    if (match == null) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 2; return; }
    System.Collections.IList list = match.ILPNGCMEFFI();
    if (list == null) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 3; return; }

    UnityEngine.Transform localRoot = local.get_RootTransform();
    Vector3 localPos = localRoot == null ? new Vector3(0f, 0f, 0f) : localRoot.get_position();
    global::COW.GamePlay.Vehicle vehicle = null;
    global::COW.GamePlay.Player rider = null;
    float bestVehicle = 999999f;
    int count = list.Count;
    if (count > 128) { count = 128; }
    for (int i = 0; i < count; i++) {
      global::COW.GamePlay.Player other = list[i] as global::COW.GamePlay.Player;
      if (other == null || other == local || other.IsLocalPlayer()) { continue; }
      if (other.IsLocalTeammate(false) || other.IsLocalTeammate(true)) { continue; }
      if (other.get_IsDieing() || other.get_CurHP() <= 0) { continue; }
      UnityEngine.Transform tr = other.get_RootTransform();
      if (tr == null || !other.IsInVehicle()) { continue; }
      global::COW.GamePlay.Vehicle candidate = other.VehicleIAmIn();
      if (candidate == null) { continue; }
      float d = Vector3.Distance(localPos, tr.get_position());
      if (d < bestVehicle) { bestVehicle = d; vehicle = candidate; rider = other; }
    }
    if (vehicle == null && local.IsInVehicle()) {
      vehicle = local.VehicleIAmIn();
      rider = local;
    }
    if (vehicle == null) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 4; return; }

    global::COW.GamePlay.Player target = null;
    float bestTarget = 999999f;
    for (int i = 0; i < count; i++) {
      global::COW.GamePlay.Player other = list[i] as global::COW.GamePlay.Player;
      if (other == null || other == local || other == rider || other.IsLocalPlayer()) { continue; }
      if (other.IsLocalTeammate(false) || other.IsLocalTeammate(true)) { continue; }
      if (other.get_IsDieing() || other.get_CurHP() <= 0) { continue; }
      UnityEngine.Transform tr = other.get_RootTransform();
      if (tr == null) { continue; }
      float d = Vector3.Distance(localPos, tr.get_position());
      if (d < bestTarget) { bestTarget = d; target = other; }
    }
    if (target == null) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 5; return; }

    bool justAcquired = vehicle != global::COW.GamePlay.SceneEditBoxSelectTool.Zx2b4538a;
    if (justAcquired || now >= global::COW.GamePlay.SceneEditBoxSelectTool.Zx56ccd1a) {
      try {
        // Enter=true and driver seat 0: grant local auth on this vehicle.
        vehicle.ChangeState(true, local, false, 0, 0u, false);
        vehicle.SetVisibility(8u, true);
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2b4538a = vehicle;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf1604d9 = target;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx56ccd1a = now + 0.5f;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx904ac7d++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 6;
      } catch (System.Exception error) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx660c3b1 = error.GetType().Name + ": " + error.Message;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 7;
        return;
      }
    }

    UnityEngine.Transform targetRoot = target.get_RootTransform();
    if (targetRoot == null || vehicle.get_transform() == null) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 5; return; }
    Vector3 to = targetRoot.get_position();
    to.y = to.y + 1.0f;
    Vector3 from = vehicle.get_transform().get_position();
    float dist = Vector3.Distance(from, to);
    if (!(dist > 0.05f)) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 8; return; }
    float speed = 45f;
    Vector3 delta = new Vector3(to.x - from.x, to.y - from.y, to.z - from.z);
    Vector3 velocity = new Vector3(delta.x * speed / dist, delta.y * speed / dist, delta.z * speed / dist);

    try {
      global::message.APCOOODFCGL packet = new global::message.APCOOODFCGL();
      packet.FALECEKOBKD = vehicle.GetAttackableID();
      packet.CHBDLAKCLKD = global::message.LMCBHLCPAJD.DJNIOABIFCA(to);
      packet.GFJPEIEIFHC = new global::message.LPEKLBNOBEK();
      packet.GFJPEIEIFHC.ALNGACCIEFF = 0;
      packet.GFJPEIEIFHC.CHHBFMKPGDC = 0;
      packet.GFJPEIEIFHC.JJPBHGNHGDN = 0;
      packet.GFJPEIEIFHC.FAAFHAAKNGI = 1000;
      packet.IMNOFNCGNBB = 0;
      packet.IEEIGMJLPEL = global::message.LMCBHLCPAJD.MGOGJCMCIPG(velocity);
      uint timestamp = (uint)(now * 1000f);
      vehicle.PushSyncState(packet, timestamp);
      vehicle.get_transform().set_position(to);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc22150c++;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 9;

      if (now >= global::COW.GamePlay.SceneEditBoxSelectTool.Zx095eaf9) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx095eaf9 = now + 0.5f;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx5442eaa = target.get_CurHP();
        if (DispatchVehicleHit(local, target, vehicle, from, to)) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 14;
        } else {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 15;
        }
      }
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx660c3b1 = error.GetType().Name + ": " + error.Message;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx0674c58 = 7;
    }
  }

  private static void QueueHitFly() {
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zxd743c61) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = 2;
      return;
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxf4ca405) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf4ca405 = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = 1;
  }


  private static void QueueMapHitFly() {
#if HF_GUIDED_TRIAL
    if (GuidedMapActive()) { EndGuidedMap(6, "guide-user-cancelled"); return; }
#endif
#if HF_SOPHIA_TRIAL
    if (SophiaObservationActive()) { BlockHitFlyMap(15, "map-observation-active-no-repeat"); return; }
#endif
    global::COW.GamePlay.SceneEditBoxSelectTool.ZxHFNoticeUntil = Time.get_realtimeSinceStartup() + 8f;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxb0bf803 = true;
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx92648bd) {
      BlockHitFlyMap(2, "map-cooldown"); return;
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx4f44d88) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx4f44d88 = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7465f3 = 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxba0dd78 = "queued";
  }

  private static void BlockHitFlyMap(int status, string reason) {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7465f3 = status;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxba0dd78 = reason;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxb0bf803 = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.ZxHFNoticeUntil = Time.get_realtimeSinceStartup() + 8f;
  }

  private static bool TryHitFlyMapTarget(out Vector3 target) {
    target = new Vector3();
    global::COW.UIInGameScene scene = global::COW.GameFacade.CurrentInGameUIScene();
    if (scene == null) { return false; }
    global::COW.UIBigMapController map = scene.GetBigMap();
    if (map == null) { return false; }
    global::COW.UIMapContentController content = map.CurrentActiveGetMapContentCtrl();
    if (content == null) { return false; }
#if HF_SOPHIA_TRIAL
    if (!content.m_HasLocalMapMarkRealPos) {
      global::COW.UIHudPlayerMarkController marker = content.m_LocalMapMarkController;
      if (marker == null) { return false; }
      target = marker.m_pos;
      return ValidVector(target);
    }
#else
    if (!content.m_HasLocalMapMarkRealPos) { return false; }
#endif
    target = content.m_LocalMapMarkRealPos;
    return ValidVector(target);
  }

  private static Vector3 HitFlyMapDirection(Vector3 origin, Vector3 target) {
    // Map Y can refer to another map layer; only horizontal bearing is used.
    if (!ValidVector(origin) || !ValidVector(target)) { return new Vector3(); }
    float dx = target.x - origin.x, dz = target.z - origin.z;
    float distance = Mathf.Sqrt(dx * dx + dz * dz);
    if (!(distance > 1f && distance <= 10000f)) { return new Vector3(); }
    return new Vector3(dx / distance, 0f, dz / distance);
  }

  // Historical R12.16 experiment, excluded from the active compiled graph.
  private static bool HitFlyMapSurface(Vector3 origin, Vector3 mark, out Vector3 target) {
    target = new Vector3();
    if (!ValidVector(origin) || !ValidVector(mark)) { return false; }
    // Marker Y can be a UI/map-layer value. Resolve a loaded solid surface at XZ.
    Vector3 top = new Vector3(mark.x, origin.y + 1024f, mark.z);
    RaycastHit hit;
    if (!Physics.Raycast(top, new Vector3(0f, -1f, 0f), out hit,
        2048f, -1, QueryTriggerInteraction.Ignore)) { return false; }
    Collider surface = hit.get_collider();
    float depth = hit.get_distance();
    if (surface == null || surface.get_isTrigger() || !(depth > 0f && depth < 2048f)) { return false; }
    target = new Vector3(mark.x, top.y - depth + 0.2f, mark.z);
    return ValidVector(target);
  }

  private static bool SolveHitFlyMapArc(Vector3 origin, Vector3 target, float speed,
      float height, float gravity, out float duration, out float horizontal, out float vertical) {
    duration = 0f; horizontal = 0f; vertical = 0f;
    if (!ValidVector(origin) || !ValidVector(target) || !(speed >= 5f && speed <= 100f) ||
        !(height >= 10f && height <= 200f) || !(gravity >= 1f && gravity <= 30f)) { return false; }
    float dx = target.x - origin.x, dz = target.z - origin.z;
    float distance = Mathf.Sqrt(dx * dx + dz * dz);
    if (!(distance > 1f && distance <= 2000f)) { return false; }
    float dy = target.y - origin.y;
    float apex = height;
    if (dy > 0f) { apex = apex + dy; }
    // Choose the descending solution with an apex above both endpoints.
    float loftTime = Mathf.Sqrt(2f * apex / gravity) + Mathf.Sqrt(2f * (apex - dy) / gravity);
    duration = distance / speed;
    if (duration < loftTime) { duration = loftTime; }
    vertical = dy / duration + 0.5f * gravity * duration;
    horizontal = distance / duration;
    // Do not clamp an unreachable shot into one that silently misses the mark.
    return duration >= 0.2f && duration <= 20f && horizontal > 0f && horizontal <= 100f &&
        vertical > 0f && vertical <= 100f && vertical - gravity * duration < 0f;
  }

  private static void FinishHitFlyMap(int state, string reason) {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 = state;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxba0dd78 = reason;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5aedd00 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxc0040fc = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxb0bf803 = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.ZxHFNoticeUntil = Time.get_realtimeSinceStartup() + 8f;
  }

  private static void ObserveHitFlyMap() {
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 != 1 &&
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 != 2) { return; }
    try {
      global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.Zx5aedd00;
      if (NativeRecovery() || local == null || local != global::COW.GameFacade.GetLocalPlayerOrObServer() ||
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxc0040fc != global::COW.GameFacade.CurrentMatch() ||
          !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        FinishHitFlyMap(6, "context-cancelled"); return;
      }
      float now = Time.get_realtimeSinceStartup();
      if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx5998847) { return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx5998847 = now + 0.1f;
      Transform root = local.get_RootTransform();
      if (root == null) { FinishHitFlyMap(6, "missing-root"); return; }
      Vector3 position = root.get_position();
      if (!ValidVector(position)) { FinishHitFlyMap(6, "invalid-position"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx04a45e0 = position;
      if (position.y > global::COW.GamePlay.SceneEditBoxSelectTool.Zx5e922fb) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx5e922fb = position.y;
      }
      Vector3 target = global::COW.GamePlay.SceneEditBoxSelectTool.Zx51a4eaa;
      float dx = target.x - position.x, dz = target.z - position.z;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx04df5b3 = Mathf.Sqrt(dx * dx + dz * dz);
      float elapsed = now - global::COW.GamePlay.SceneEditBoxSelectTool.Zxeb2d06c;
      bool grounded = local.IsGrounded();
      if (!grounded) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 = 2;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1a282ff = 0;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 == 2 && elapsed > 0.3f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1a282ff++;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx1a282ff >= 2) {
          float dy = target.y - position.y;
          bool near = global::COW.GamePlay.SceneEditBoxSelectTool.Zx04df5b3 <= 3f && dy >= -2f && dy <= 2f;
          FinishHitFlyMap(3, near ? "landed-near-client-observation" : "landed-miss-client-observation"); return;
        }
      }
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 == 1 && elapsed > 2f) {
        FinishHitFlyMap(4, "no-takeoff-observed"); return;
      }
      if (elapsed > global::COW.GamePlay.SceneEditBoxSelectTool.Zx324e1d0 + 5f) {
        FinishHitFlyMap(5, "landing-timeout");
      }
    } catch (System.Exception error) { FinishHitFlyMap(7, error.Message); }
  }

  private static string HitFlyMapLog() {
#if HF_GUIDED_TRIAL
    string mode = "guided-keyed-force-trial";
    string trial = " guideTrial=1 phase=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxcdd5a3a.ToString()
        + " fault=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx6cf04c7 ? "1" : "0")
        + " owned=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx6910825 ? "1" : "0")
        + " position=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.Zx97406cf)
        + " force=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.Zx27a70e8);
#elif HF_SOPHIA_TRIAL
    string mode = "sophia-657-trial";
    string trial = " obs=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8.ToString()
        + " estimate=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx51c6116.ToString("F2")
        + " gravity=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx0545665.ToString("F2")
        + " scale=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx4835f14.ToString("F2")
        + " nearestXZ=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f27331.ToString("F2")
        + " physState=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx8495afa.ToString()
        + " vel=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxf99b46d.ToString("F2") + ","
        + global::COW.GamePlay.SceneEditBoxSelectTool.Zxaa6699a.ToString("F2") + ","
        + global::COW.GamePlay.SceneEditBoxSelectTool.Zx85fe3a2.ToString("F2")
        + " missXZ=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx04df5b3.ToString("F2")
        + " rise=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx5e922fb - global::COW.GamePlay.SceneEditBoxSelectTool.Zxaaebcd5.y).ToString("F2")
        + " last=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.Zx04a45e0);
#else
    string mode = "bearing-up";
    string trial = "";
#endif
    return "hfMap status=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7465f3.ToString()
        + " mode=" + mode
        + trial
        + " attempts=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx994d756.ToString()
        + " sent=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe208c69.ToString()
        + " error=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx25cb7bd
        + " reason=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxba0dd78
        + " highJump=" + (HighJumpOn() ? "1" : "0")
        + " hover=" + (HoverModeOn() ? "1" : "0")
        + " t=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx324e1d0.ToString("F2")
        + " xz=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx41d811b.ToString("F2")
        + " vy=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx7146047.ToString("F2")
        + "\nhfMap target=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.Zx51a4eaa)
        + " origin=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.Zxaaebcd5)
        + " exactLanding=0 collisionAvoidance=unverified serverAck=unobserved";
  }

  private static string HitFlyCoordinates(Vector3 point) {
    return point.x.ToString("F2") + "," + point.y.ToString("F2") + "," + point.z.ToString("F2");
  }

  private static void HitFlyMapRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    Rect fly = new Rect(px + 20f, y, 390f, 64f);
    if (et == 1 && fly.Contains(mp)) { QueueMapHitFly(); }
    GUI.set_color(new Color(0.06f, 0.2f, 0.23f, 0.95f));
    GUI.DrawTexture(fly, Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.4f, 1f, 0.8f, 1f));
#if HF_GUIDED_TRIAL
    GUI.Label(new Rect(px + 32f, y + 3f, 370f, 30f), "HF MAP DEV: bấm chạy/hủy", row);
#elif HF_SOPHIA_TRIAL
    GUI.Label(new Rect(px + 32f, y + 3f, 370f, 30f), "HF MAP DEV: Sophia 657", row);
#else
    GUI.Label(new Rect(px + 32f, y + 3f, 370f, 30f), "HF MAP: hướng dấu + đẩy lên", row);
#endif
    GUIStyle statusStyle = CachedStyle(4);
    statusStyle.set_fontSize(17);
    statusStyle.get_normal().set_textColor(new Color(0.4f, 1f, 0.8f, 1f));
    GUI.Label(new Rect(px + 32f, y + 32f, 370f, 30f), HitFlyMapStatusLabel(), statusStyle);
  }



  private static void DispatchHitFly(bool mapMode) {
    // Consume before any game calls: a failure never retries every frame.
    float now = Time.get_realtimeSinceStartup();
    float next = mapMode ? global::COW.GamePlay.SceneEditBoxSelectTool.Zx92648bd :
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd743c61;
    if (now < next) {
      if (mapMode) { BlockHitFlyMap(2, "map-cooldown"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = 2; return;
    }
    try {
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      if (global::COW.GameFacade.CurrentMatch() == null || local == null ||
          !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        if (mapMode) { BlockHitFlyMap(3, "no-live-local-match"); return; }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = 3; return;
      }
      uint playerId = global::COW.GamePlay.BEADLMGGGGL.DEIPJDIGKIP(local.get_PlayerID());
      Vector3 direction = new Vector3();
        float duration = 1f, horizontal = 25f, vertical = 50f;
      Vector3 mapOrigin = new Vector3(), mapTarget = new Vector3();
      if (mapMode) {
        Vector3 mark;
        if (!TryHitFlyMapTarget(out mark)) {
          BlockHitFlyMap(8, "missing-marker"); return;
        }
        Transform root = local.get_RootTransform();
        if (root == null) { BlockHitFlyMap(9, "missing-root"); return; }
        Vector3 origin = root.get_position();
        direction = HitFlyMapDirection(origin, mark);
        if (!(direction.x * direction.x + direction.z * direction.z > 0.9f)) {
          BlockHitFlyMap(9, "invalid-map-bearing"); return;
        }
#if HF_SOPHIA_TRIAL
        float gravity = SophiaMapGravity(local);
        float estimate;
        float scale = 1.1f;
        if (!SophiaMapParameters(origin, mark, gravity, scale, out estimate, out duration, out horizontal)) {
          BlockHitFlyMap(9, "invalid-sophia-parameters"); return;
        }
        vertical = 150f;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx0545665 = gravity;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx51c6116 = estimate;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx4835f14 = scale;
#else
        // Original bearing/impulse route, with a stronger upward launch.
        // Do not infer flight time or an exact landing point from distance.
        duration = 1f;
        horizontal = HitFlyValue("u3d.q.52472132", 25f, 5f, 100f);
        vertical = 50f;
#endif
        mapOrigin = origin; mapTarget = mark;
      } else {
        duration = HitFlyValue("u3d.q.f6bfd105", 1f, 0.2f, 3f);
        horizontal = HitFlyValue("u3d.q.ad35e585", 25f, 0f, 100f);
        vertical = HitFlyValue("u3d.q.0d5df123", 20f, 0f, 100f);
        Camera camera = Camera.get_main();
        if (camera == null || camera.get_transform() == null) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = 3; return;
        }
        direction = camera.get_transform().get_forward();
      }
      float length = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y + direction.z * direction.z);
      if (playerId == 0 || !ValidVector(direction) || !(length > 0.001f && length < 100f)) {
        if (mapMode) { BlockHitFlyMap(4, "invalid-id-or-direction"); return; }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = 4; return;
      }
      direction = Vector3.Normalize(direction);
      global::message.BJFGLKKKOAN packed = global::message.LMCBHLCPAJD.MGOGJCMCIPG(direction);
      if (packed == null) {
        if (mapMode) { BlockHitFlyMap(4, "direction-pack-null"); return; }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = 4; return;
      }
      global::message.NNDOIBJBAJI msg = new global::message.NNDOIBJBAJI();
      msg.BEADLMGGGGL = playerId;
      msg.DCEMJOGHMEG = duration;
      msg.KJGOMDJCKEB = horizontal;
      msg.DPJAIECLHGL = vertical;
      msg.MGJNBIJNLIJ = packed;
#if HF_SOPHIA_TRIAL
      // Sophia leaves these optional fields at their constructor defaults.
      if (!mapMode) { msg.JBEHMJFBEMG = ""; msg.IMAOCDIIIHI = 0; }
#else
      msg.JBEHMJFBEMG = ""; msg.IMAOCDIIIHI = 0;
#endif
      if (mapMode) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx51a4eaa = mapTarget;
        float dx = mapTarget.x - mapOrigin.x, dz = mapTarget.z - mapOrigin.z;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx27385ef = Mathf.Sqrt(dx * dx + dz * dz);
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxaaebcd5 = mapOrigin;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx324e1d0 = duration;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx41d811b = horizontal;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx7146047 = vertical;
      }
      float cooldown = msg.DCEMJOGHMEG + 0.25f;
      if (cooldown < 1f) { cooldown = 1f; }
      if (mapMode) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx92648bd = now + cooldown;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx994d756++;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd743c61 = now + cooldown;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx5b461d5++;
      }
      bool sent = global::COW.GameFacade.Send(657u, msg, (byte)2, false);
      if (mapMode) {
        if (sent) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxe208c69++; }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx25cb7bd = "";
#if HF_SOPHIA_TRIAL
        BlockHitFlyMap(sent ? 5 : 6, sent ? "sent-sophia-657" : "send-false");
        if (sent) {
          try { BeginSophiaObservation(local); }
          catch (System.Exception error) { EndSophiaObservation(7, "observation-start-error: " + error.Message); }
        }
#else
        BlockHitFlyMap(sent ? 5 : 6, sent ? "sent-bearing-up" : "send-false");
#endif
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = sent ? 5 : 6;
        if (sent) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxd16e43d++; }
#if HF_GUIDED_TRIAL
        if (sent) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx123707a = now + cooldown; }
#endif
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx58d8e8b = "";
      }
    } catch (System.Exception error) {
      if (mapMode) {
        BlockHitFlyMap(7, error.Message);
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx25cb7bd = error.Message;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx92648bd = now + 1f;
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619ebc = 7;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx58d8e8b = error.Message;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd743c61 = now + 1f;
      PlayerPrefs.SetString("u3d.q.1b5a182e", error.ToString());
    }
  }

#if HF_SOPHIA_TRIAL
  private static float SophiaMapGravity(global::COW.GamePlay.Player local) {
    float gravity = 50f;
    try {
      float native = local.get_FallingGravity();
      if (native < 0f) { native = -native; }
      if (native >= 5f && native <= 160f) { gravity = native; }
    } catch (System.Exception) { }
    return gravity;
  }

  private static bool SophiaMapParameters(Vector3 origin, Vector3 target, float gravity, float scale,
      out float estimate, out float duration, out float horizontal) {
    estimate = 0f; duration = 0f; horizontal = 0f;
    if (!ValidVector(origin) || !ValidVector(target) || !(gravity >= 5f && gravity <= 160f) ||
        !(scale >= 0.5f && scale <= 1.5f)) { return false; }
    float dx = target.x - origin.x, dz = target.z - origin.z, dy = target.y - origin.y;
    float distance = Mathf.Sqrt(dx * dx + dz * dz);
    if (!(distance > 1f && distance <= 10000f)) { return false; }
    float discriminant = 22500f - 2f * gravity * dy;
    if (discriminant < 0f) { discriminant = 0f; }
    estimate = Mathf.Clamp((150f + Mathf.Sqrt(discriminant)) / gravity, 2f, 15f);
    float factor = Mathf.Clamp(2.1f + distance / 1000f, 2.15f, 2.55f);
    horizontal = distance / estimate * factor * scale;
    duration = Mathf.Clamp(estimate, 1.8f, 6f);
    // Application bound only. Never silently clamp to100 and change the reference.
    return horizontal > 0f && horizontal <= 500f;
  }

  private static bool SophiaObservationActive() {
    int state = global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8;
    return state == 1 || state == 2;
  }

  private static void BeginSophiaObservation(global::COW.GamePlay.Player local) {
    float now = Time.get_realtimeSinceStartup();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5aedd00 = local;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxc0040fc = global::COW.GameFacade.CurrentMatch();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 = 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxeb2d06c = now;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5998847 = now;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx4f494d8 = now + 2f;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5e922fb = global::COW.GamePlay.SceneEditBoxSelectTool.Zxaaebcd5.y;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx04a45e0 = global::COW.GamePlay.SceneEditBoxSelectTool.Zxaaebcd5;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx04df5b3 = global::COW.GamePlay.SceneEditBoxSelectTool.Zx27385ef;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f27331 = global::COW.GamePlay.SceneEditBoxSelectTool.Zx27385ef;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx64c6109 = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx9b7df00 = local.IsGrounded();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx1a282ff = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxb0bf803 = true;
  }

  private static void EndSophiaObservation(int state, string reason) {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 = state;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5aedd00 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxc0040fc = null;
    BlockHitFlyMap(state == 3 ? 16 : 17, reason);
  }

  private static void ObserveSophiaMap() {
    if (!SophiaObservationActive()) { return; }
    try {
      global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.Zx5aedd00;
      if (NativeRecovery() || local == null || local != global::COW.GameFacade.GetLocalPlayerOrObServer() ||
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxc0040fc != global::COW.GameFacade.CurrentMatch() ||
          !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        EndSophiaObservation(6, "observation-context-cancelled"); return;
      }
      float now = Time.get_realtimeSinceStartup();
      float elapsed = now - global::COW.GamePlay.SceneEditBoxSelectTool.Zxeb2d06c;
      if (!(elapsed >= 0f && elapsed <= 35f)) { EndSophiaObservation(5, "observation-clock-or-timeout"); return; }
      if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx5998847) { return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx5998847 = now + 0.1f;
      if (now >= global::COW.GamePlay.SceneEditBoxSelectTool.Zx4f494d8) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxb0bf803 = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx4f494d8 = now + 2f;
      }
      Transform root = local.get_RootTransform();
      if (root == null) { EndSophiaObservation(6, "observation-missing-root"); return; }
      Vector3 position = root.get_position();
      if (!ValidVector(position)) { EndSophiaObservation(6, "observation-invalid-position"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx8495afa = (int)local.GetPhysXState();
      Vector3 velocity = local.GetVelocity();
      if (!ValidVector(velocity)) { EndSophiaObservation(6, "observation-invalid-velocity"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf99b46d = velocity.x;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxaa6699a = velocity.y;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx85fe3a2 = velocity.z;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx04a45e0 = position;
      if (position.y > global::COW.GamePlay.SceneEditBoxSelectTool.Zx5e922fb) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx5e922fb = position.y;
      }
      Vector3 target = global::COW.GamePlay.SceneEditBoxSelectTool.Zx51a4eaa;
      float dx = target.x - position.x, dz = target.z - position.z;
      float distance = Mathf.Sqrt(dx * dx + dz * dz);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx04df5b3 = distance;
      if (distance < global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f27331) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f27331 = distance;
      }
      bool grounded = local.IsGrounded() && global::COW.GamePlay.SceneEditBoxSelectTool.Zx8495afa != 5;
      if (!grounded) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx64c6109 = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf365f8 = 2;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1a282ff = 0;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx64c6109 && elapsed > 0.5f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1a282ff++;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx1a282ff >= 3) {
          EndSophiaObservation(3, global::COW.GamePlay.SceneEditBoxSelectTool.Zx9b7df00 ?
              "landed-client-observation-unattributed" : "landed-airborne-start-unattributed"); return;
        }
      }
      if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zx64c6109 && elapsed > 3f) {
        EndSophiaObservation(4, "no-airborne-sample-observed"); return;
      }
    } catch (System.Exception error) { EndSophiaObservation(7, "observation-error: " + error.Message); }
  }
#endif

#if HF_GUIDED_TRIAL
  // Staged experiment: this key is owned only by the guided MAP controller.
  private const uint GuidedMapForceKey = 1330333766u;

  private static bool GuidedMapActive() {
    int state = global::COW.GamePlay.SceneEditBoxSelectTool.Zxcdd5a3a;
    return state >= 1 && state <= 4;
  }

  private static Vector3 LimitGuidedVector(Vector3 value, float limit) {
    float length = Mathf.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
    if (length > limit) { return new Vector3(value.x * limit / length, value.y * limit / length, value.z * limit / length); }
    return value;
  }

  private static bool GuidedSurface(Vector3 origin, Vector3 mark, out Vector3 target) {
    target = new Vector3();
    if (!ValidVector(origin) || !ValidVector(mark)) { return false; }
    RaycastHit hit;
    Vector3 top = new Vector3(mark.x, origin.y + 512f, mark.z);
    if (!Physics.Raycast(top, new Vector3(0f, -1f, 0f), out hit, 1024f, -1, QueryTriggerInteraction.Ignore)) { return false; }
    float depth = hit.get_distance();
    Collider surface = hit.get_collider();
    if (surface == null || surface.get_isTrigger() || !(depth > 0f && depth < 1024f)) { return false; }
    target = new Vector3(mark.x, top.y - depth + 0.2f, mark.z);
    return ValidVector(target);
  }

  private static void EndGuidedMap(int state, string reason) {
    // Latch first. Even a partial native setter failure must not be retried as a
    // fresh flight. Never touch any other force key or the transform position.
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx6cf04c7 = true;
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx6910825) {
      try {
        global::COW.GamePlay.Player owner = global::COW.GamePlay.SceneEditBoxSelectTool.Zx633e561;
        if (owner != null) { owner.SetForceMoveVelocity(new Vector3(), GuidedMapForceKey); }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx6910825 = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx27a70e8 = new Vector3();
      } catch (System.Exception error) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxcdd5a3a = 9;
        BlockHitFlyMap(7, "guide-cleanup-fault: " + error.Message);
        return;
      }
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx6cf04c7 = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxcdd5a3a = state;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx633e561 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx0b81acf = null;
    BlockHitFlyMap(state == 5 ? 16 : 17, reason);
  }

  private static void StartGuidedMap() {
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx6cf04c7) { BlockHitFlyMap(7, "guide-fault-restart-required"); return; }
    if (GuidedMapActive()) { BlockHitFlyMap(15, "guide-already-active"); return; }
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx92648bd) { BlockHitFlyMap(2, "map-cooldown"); return; }
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx123707a) { BlockHitFlyMap(15, "manual-hf-in-progress"); return; }
    try {
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
      if (local == null || match == null || !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        BlockHitFlyMap(3, "no-live-local-match"); return;
      }
      // Do not compete with an existing game hitfly/vehicle/airborne state.
      if (!local.IsGrounded() || local.GetPhysXState() != 0 || HighJumpOn() || HoverModeOn() ||
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxd869a67 != 0) {
        BlockHitFlyMap(15, "trial-requires-grounded-walking-and-normal-gravity"); return;
      }
      Transform root = local.get_RootTransform();
      Vector3 mark, target;
      if (root == null || !TryHitFlyMapTarget(out mark)) { BlockHitFlyMap(8, "missing-root-or-marker"); return; }
      Vector3 origin = root.get_position();
      if (!GuidedSurface(origin, mark, out target)) { BlockHitFlyMap(10, "trial-no-loaded-surface"); return; }
      float dx = target.x - origin.x, dz = target.z - origin.z;
      float distance = Mathf.Sqrt(dx * dx + dz * dz);
      if (!(distance > 2f && distance <= 1000f)) { BlockHitFlyMap(9, "trial-distance-out-of-range"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx633e561 = local;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx0b81acf = match;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx51a4eaa = target;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxaaebcd5 = origin;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx97406cf = origin;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxaadaf57 = (origin.y > target.y ? origin.y : target.y) + 35f;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc4dc6d = HitFlyValue("u3d.q.52472132", 25f, 5f, 100f);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx41dd9fc = now;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa5fc5e7 = now;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx8c2e64e = now;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxee5f1e7 = 0f;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf5f6bcc = 100000f;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx27a70e8 = new Vector3();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxcdd5a3a = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx92648bd = now + 1f;
      BlockHitFlyMap(18, "trial-ascent");
    } catch (System.Exception error) { EndGuidedMap(7, "guide-start-error: " + error.Message); }
  }

  private static void TickGuidedMap() {
    if (!GuidedMapActive() || global::COW.GamePlay.SceneEditBoxSelectTool.Zx6cf04c7) { return; }
    try {
      global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.Zx633e561;
      if (NativeRecovery() || local == null || local != global::COW.GameFacade.GetLocalPlayerOrObServer() ||
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx0b81acf != global::COW.GameFacade.CurrentMatch() ||
          !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        EndGuidedMap(6, "guide-context-cancelled"); return;
      }
      if (HighJumpOn() || HoverModeOn() || local.GetPhysXState() > 1) {
        EndGuidedMap(6, "guide-other-motion-mode"); return;
      }
      float now = Time.get_realtimeSinceStartup();
      float dt = now - global::COW.GamePlay.SceneEditBoxSelectTool.Zxa5fc5e7;
      if (dt < 0.02f && dt >= 0f) { return; }
      if (!(dt > 0f && dt <= 0.25f) || now - global::COW.GamePlay.SceneEditBoxSelectTool.Zx41dd9fc > 90f) {
        EndGuidedMap(6, "guide-clock-or-flight-timeout"); return;
      }
      Transform root = local.get_RootTransform();
      if (root == null) { EndGuidedMap(6, "guide-missing-root"); return; }
      Vector3 position = root.get_position();
      Vector3 previous = global::COW.GamePlay.SceneEditBoxSelectTool.Zx97406cf;
      if (!ValidVector(position)) { EndGuidedMap(6, "guide-invalid-position"); return; }
      Vector3 observed = new Vector3((position.x - previous.x) / dt, (position.y - previous.y) / dt, (position.z - previous.z) / dt);
      float moved = Mathf.Sqrt(observed.x * observed.x + observed.y * observed.y + observed.z * observed.z) * dt;
      if (moved > 150f * dt + 3f) { EndGuidedMap(6, "guide-position-correction"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa5fc5e7 = now;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx97406cf = position;
      Vector3 target = global::COW.GamePlay.SceneEditBoxSelectTool.Zx51a4eaa;
      float dx = target.x - position.x, dz = target.z - position.z;
      float distance = Mathf.Sqrt(dx * dx + dz * dz);
      int state = global::COW.GamePlay.SceneEditBoxSelectTool.Zxcdd5a3a;
      float cruise = global::COW.GamePlay.SceneEditBoxSelectTool.Zxaadaf57;
      float metric = distance;
      Vector3 desired = new Vector3();
      if (state == 1) {
        metric = cruise - position.y;
        desired.x = Mathf.Clamp((global::COW.GamePlay.SceneEditBoxSelectTool.Zxaaebcd5.x - position.x) * 2f, -5f, 5f);
        desired.z = Mathf.Clamp((global::COW.GamePlay.SceneEditBoxSelectTool.Zxaaebcd5.z - position.z) * 2f, -5f, 5f);
        desired.y = Mathf.Clamp(metric * 1.5f, -5f, 15f);
        if (metric < 1f && observed.y < 3f) { state = 2; }
      } else if (state == 2 || state == 3) {
        float speed = global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc4dc6d;
        float approach = Mathf.Sqrt(2f * 12f * distance);
        if (speed > approach) { speed = approach; }
        if (speed > distance * 2f) { speed = distance * 2f; }
        if (distance > 0.05f) { desired.x = dx / distance * speed; desired.z = dz / distance * speed; }
        desired.y = Mathf.Clamp((cruise - position.y) * 2f, -8f, 8f);
        if (distance < 12f) { state = 3; }
        float lateral = Mathf.Sqrt(observed.x * observed.x + observed.z * observed.z);
        if (distance < 0.75f && lateral < 0.75f) { state = 4; }
      } else {
        Vector3 surface;
        if (!GuidedSurface(position, target, out surface)) { EndGuidedMap(6, "guide-surface-unloaded"); return; }
        target.y = surface.y;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx51a4eaa = target;
        desired.x = Mathf.Clamp(dx * 2f, -3f, 3f); desired.z = Mathf.Clamp(dz * 2f, -3f, 3f);
        float height = position.y - target.y;
        metric = height;
        desired.y = -Mathf.Clamp(height * 1.5f, 0.5f, 6f);
        if (local.IsGrounded() && distance < 1f && height > -1f && height < 1.5f) {
          if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxee5f1e7 == 0f) {
            global::COW.GamePlay.SceneEditBoxSelectTool.Zxee5f1e7 = now;
          } else if (now - global::COW.GamePlay.SceneEditBoxSelectTool.Zxee5f1e7 >= 0.2f) {
            EndGuidedMap(5, "guide-landed-client-observation"); return;
          }
        } else { global::COW.GamePlay.SceneEditBoxSelectTool.Zxee5f1e7 = 0f; }
      }
      if (state != global::COW.GamePlay.SceneEditBoxSelectTool.Zxcdd5a3a) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx8c2e64e = now;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf5f6bcc = 100000f;
      } else if (metric < global::COW.GamePlay.SceneEditBoxSelectTool.Zxf5f6bcc - 0.25f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx8c2e64e = now;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf5f6bcc = metric;
      }
      if (now - global::COW.GamePlay.SceneEditBoxSelectTool.Zx8c2e64e > 3f) {
        EndGuidedMap(6, "guide-no-progress"); return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxcdd5a3a = state;
      // A short swept ray is only a center-line obstruction check, not capsule
      // clearance. It prevents continuing to push into a known obstruction.
      float speedLength = Mathf.Sqrt(desired.x * desired.x + desired.y * desired.y + desired.z * desired.z);
      if (speedLength > 0.01f && state != 4) {
        RaycastHit obstruction;
        if (Physics.Raycast(position + new Vector3(0f, 1f, 0f), Vector3.Normalize(desired), out obstruction,
            speedLength * dt + 1f, -1, QueryTriggerInteraction.Ignore)) {
          Collider collider = obstruction.get_collider();
          Transform tf = collider == null ? null : collider.get_transform();
          if (tf == null || (!tf.IsChildOf(root) && tf != root)) { EndGuidedMap(6, "guide-path-obstructed"); return; }
        }
      }
      Vector3 baseline = local.GetVelocity();
      Vector3 forces = local.GetForceMoveVelocity();
      Vector3 own = global::COW.GamePlay.SceneEditBoxSelectTool.Zx27a70e8;
      if (!ValidVector(baseline) || !ValidVector(forces)) { EndGuidedMap(6, "guide-invalid-velocity"); return; }
      Vector3 command = LimitGuidedVector(new Vector3(desired.x - baseline.x - forces.x + own.x,
          desired.y - baseline.y - forces.y + own.y, desired.z - baseline.z - forces.z + own.z), 100f);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6910825 = true;
      local.SetForceMoveVelocity(command, GuidedMapForceKey);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx27a70e8 = command;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxba0dd78 = "guided-force-trial-state" + state.ToString();
    } catch (System.Exception error) { EndGuidedMap(7, "guide-tick-error: " + error.Message); }
  }
#endif

  private static void UpdateCombat() {
    if (Event.get_current().get_type() != 7) { return; }
    try {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx06ef0b8 = "";
      bool recovery = NativeRecovery();
      bool wantCamera = PlayerPrefs.GetString("u3d.q.94da05df", "") == "X2";
      Camera camera = Camera.get_main();
      if (camera != null) {
        if (wantCamera) {
          if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zx2d99817) {
            global::COW.GamePlay.SceneEditBoxSelectTool.Zx7eb83a6 = camera.get_fieldOfView();
            global::COW.GamePlay.SceneEditBoxSelectTool.Zx2d99817 = true;
          }
          camera.set_fieldOfView(ReadFloatSetting("u3d.q.d307e364", 90, 60, 120));
        } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx2d99817) {
          camera.set_fieldOfView(global::COW.GamePlay.SceneEditBoxSelectTool.Zx7eb83a6);
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx2d99817 = false;
        }
      }
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      if (local == null) { return; }
      if (!recovery && PlayerPrefs.GetString("u3d.q.14bb6e67", "") == "X2") {
        local.ResetSwapWeaponTime();
      }
      bool wantNoReload = !recovery && PlayerPrefs.GetString("u3d.q.df341fd9", "") == "X2";
      global::COW.GamePlay.PlayerAttributes attributes = local.get_Attributes();
      if (attributes == null) { return; }
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxfefb633 != attributes) {
        global::COW.GamePlay.PlayerAttributes previous =
            global::COW.GamePlay.SceneEditBoxSelectTool.Zxfefb633;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxff3d85e && previous != null) {
          previous.ShootNoReload = global::COW.GamePlay.SceneEditBoxSelectTool.Zx4b2c801;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfefb633 = attributes;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxff3d85e = false;
      }
      if (wantNoReload) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxff3d85e) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx4b2c801 = attributes.ShootNoReload;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxff3d85e = true;
        }
        attributes.ShootNoReload = true;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxff3d85e) {
        attributes.ShootNoReload = global::COW.GamePlay.SceneEditBoxSelectTool.Zx4b2c801;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxff3d85e = false;
      }
      // Speed is a slider now: 1.0 = off, up to 2.0. RunSpeedUpScale feeds
      // PlayerAttributes.GetSpeedScaleBySpeedType, so it also raises the
      // airborne steering speed (air movement uses GetSpeedByType too).
      // The old X2 toggle is migrated once to 1.3x.
      if (PlayerPrefs.GetString("u3d.q.f2ed3b05", "") == "X2") {
        PlayerPrefs.SetString("u3d.q.f2ed3b05", "");
        PlayerPrefs.SetFloat("u3d.q.a12d1b99", 1.3f);
      }
      float speedScale = ReadSpeedScale();
      bool wantSpeed = !recovery && speedScale > 1.01f;
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx54899b4 != attributes) {
        global::COW.GamePlay.PlayerAttributes previousSpeed =
            global::COW.GamePlay.SceneEditBoxSelectTool.Zx54899b4;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx39038d4 && previousSpeed != null) {
          previousSpeed.RunSpeedUpScale = global::COW.GamePlay.SceneEditBoxSelectTool.Zx8ef9e60;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx54899b4 = attributes;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx39038d4 = false;
      }
      if (wantSpeed) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zx39038d4) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx8ef9e60 = attributes.RunSpeedUpScale;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx39038d4 = true;
        }
        attributes.RunSpeedUpScale = speedScale;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx39038d4) {
        attributes.RunSpeedUpScale = global::COW.GamePlay.SceneEditBoxSelectTool.Zx8ef9e60;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx39038d4 = false;
      }
      bool wantNoRecoil = !recovery && PlayerPrefs.GetString("u3d.q.0001b8f3", "") == "X2";
      COW.WeaponGunFeelProfileData profile = null;
      global::COW.GamePlay.HBIBDMMOOOK weapon = local.GetActiveWeapon();
      if (weapon != null) { profile = weapon.ILGIAJHNNDP(); }
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxd7bf0bd != profile) {
        COW.WeaponGunFeelProfileData previousProfile =
            global::COW.GamePlay.SceneEditBoxSelectTool.Zxd7bf0bd;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxbe529ad && previousProfile != null) {
          previousProfile.enableRecoil = global::COW.GamePlay.SceneEditBoxSelectTool.Zx88c5083;
          previousProfile.enableVisualRecoil = global::COW.GamePlay.SceneEditBoxSelectTool.Zx357fec0;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd7bf0bd = profile;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbe529ad = false;
      }
      if (wantNoRecoil && profile != null) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxbe529ad) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx88c5083 = profile.enableRecoil;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx357fec0 = profile.enableVisualRecoil;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxbe529ad = true;
        }
        profile.enableRecoil = false;
        profile.enableVisualRecoil = false;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxbe529ad && profile != null) {
        profile.enableRecoil = global::COW.GamePlay.SceneEditBoxSelectTool.Zx88c5083;
        profile.enableVisualRecoil = global::COW.GamePlay.SceneEditBoxSelectTool.Zx357fec0;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbe529ad = false;
      }
      bool wantRapid = !recovery && PlayerPrefs.GetString("u3d.q.d961bf50", "") == "X2";
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx03407a4 != attributes) {
        global::COW.GamePlay.PlayerAttributes previousRapid =
            global::COW.GamePlay.SceneEditBoxSelectTool.Zx03407a4;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3f1a5 && previousRapid != null) {
          previousRapid.FireIntervalScale = global::COW.GamePlay.SceneEditBoxSelectTool.Zx1e83147;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx03407a4 = attributes;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3f1a5 = false;
      }
      if (wantRapid) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3f1a5) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx1e83147 = attributes.FireIntervalScale;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3f1a5 = true;
        }
        attributes.FireIntervalScale = 0.5f;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3f1a5) {
        attributes.FireIntervalScale = global::COW.GamePlay.SceneEditBoxSelectTool.Zx1e83147;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3f1a5 = false;
      }
      bool wantHighFps = PlayerPrefs.GetString("u3d.q.4a21a155", "") == "X2";
      if (wantHighFps) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxb96978c) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx56a65a5 = COW.GameSettingData.m_IsHighFramerate;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxcee1755 = COW.GameSettingData.m_SmoothHighFrame;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxb96978c = true;
        }
        COW.GameSettingData.m_IsHighFramerate = true;
        COW.GameSettingData.m_SmoothHighFrame = true;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxb96978c) {
        COW.GameSettingData.m_IsHighFramerate = global::COW.GamePlay.SceneEditBoxSelectTool.Zx56a65a5;
        COW.GameSettingData.m_SmoothHighFrame = global::COW.GamePlay.SceneEditBoxSelectTool.Zxcee1755;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxb96978c = false;
      }
      bool wantAmmo = !recovery && PlayerPrefs.GetString("u3d.q.3f2c455d", "") == "X2";
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx238f04e != attributes) {
        global::COW.GamePlay.PlayerAttributes previousAmmo =
            global::COW.GamePlay.SceneEditBoxSelectTool.Zx238f04e;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6c366c && previousAmmo != null) {
          previousAmmo.IsAmmoFree = global::COW.GamePlay.SceneEditBoxSelectTool.Zxdee11d6;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx238f04e = attributes;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6c366c = false;
      }
      if (wantAmmo) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6c366c) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxdee11d6 = attributes.IsAmmoFree;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6c366c = true;
        }
        attributes.IsAmmoFree = true;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6c366c) {
        attributes.IsAmmoFree = global::COW.GamePlay.SceneEditBoxSelectTool.Zxdee11d6;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6c366c = false;
      }
      bool wantAutoSwap = !recovery && PlayerPrefs.GetString("u3d.q.6ea4fd59", "") == "X2";
      if (wantAutoSwap) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf3cddc) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxee85210 = COW.GameSettingData.GetAutoSwitchWeapon();
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf3cddc = true;
        }
        COW.GameSettingData.SetAutoSwitchWeapon(true);
        float swapNow = Time.get_realtimeSinceStartup();
        if (local.IsFiring() && swapNow >= global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f1be81) {
          local.AutoSwitchWeapon(false, false);
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f1be81 = swapNow + 0.40f;
        }
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf3cddc) {
        COW.GameSettingData.SetAutoSwitchWeapon(global::COW.GamePlay.SceneEditBoxSelectTool.Zxee85210);
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbf3cddc = false;
      }
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx06ef0b8 = error.Message;
    }
  }

  // Hook body for PlayerAttributes.GetScatterRate.
  // Off: reproduce the native formula (1.0 + sighting + skill, min 0.1).
  // On: local player gets 0.1; other players keep their normal value.
  public static float ScatterRate(global::COW.GamePlay.PlayerAttributes self) {
    int calls = global::COW.GamePlay.SceneEditBoxSelectTool.Zx9045c73;
    if (calls < 1000000000) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx9045c73 = calls + 1; }
    if (self == null) { return 0.1f; }
    if (!NativeRecovery() && PlayerPrefs.GetString("u3d.q.0001b8f3", "") == "X2") { return 0.1f; }
    float value = 1f + self.SkillScatterRateSighting + self.SkillScatterRate;
    if (value < 0.1f) { value = 0.1f; }
    return value;
  }

  public static bool ShowDamageNum(global::COW.GamePlay.Player self) {
    return PlayerPrefs.GetString("u3d.q.f9618a67", "") != "X2";
  }

  // ---- movement gravity ------------------------------------------------
  // Player.HADKJKALGOL (0x568BBAC) applies vy += gravity * dt at 0x568D02C.
  // Which getter it uses depends on InSnowSlideWayDashing (Player+0x2080):
  //
  //   0x568CE60  cbz  [Player+0x2080], 0x568CFA4   ; not snow-slide
  //   0x568CE80       get_FallingGravity            ; vy < 0, snow-slide only
  //   0x568CF90       get_RisingGravity             ; vy >= 0, snow-slide only
  //   0x568CFFC       get_CustomGravity             ; normal fall (the common path)
  //
  // So normal falling goes through get_CustomGravity, NOT FallingGravity.
  // All three are leaf patch targets (no <>iFixBaseProxy_*), so the OFF path
  // must reproduce the original: in normal matches every one of them collapses
  // to COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG (-9.8f).
  //
  // get_RisingGravity is also the gravity term behind the jump launch speed:
  // Player.ELGCCFOEIID (0x57746BC) does v0 = sqrt(max(h * -2 * RisingGravity, 0)).
  // It therefore must keep returning the vanilla constant while the toggle is
  // on, otherwise the jump shrinks to a hop.
  private static float HoverValue() {
    return HoverModeOn() ? 0.3f : 9.8f;
  }
  private static float VanillaGravity() {
    return global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG;
  }

  public static float CustomGravity(global::COW.GamePlay.Player self) {
    if (NativeRecovery()) { return VanillaGravity(); }
    if (self == null) { return VanillaGravity(); }
    float hover = HoverValue();
    float gravity = (hover < 9.75f) ? -hover : VanillaGravity();
    // Jump boost: for a short window right after the jump starts, report a
    // weaker gravity. The launch speed already happened at vanilla strength, so
    // a weaker ascent deceleration raises the apex without touching the fall.
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx0365f1c =
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx0365f1c + 1;
    if (Time.get_frameCount() < global::COW.GamePlay.SceneEditBoxSelectTool.Zx0082d3a) {
      int hits = global::COW.GamePlay.SceneEditBoxSelectTool.Zx689a5a3;
      if (hits < 100000) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx689a5a3 = hits + 1; }
      float boosted = gravity / JumpScale();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx66e6ece = boosted;
      return boosted;
    }
    return gravity;
  }

  public static float RisingGravity(global::COW.GamePlay.Player self) {
    if (NativeRecovery()) {
      return self != null && self.InSnowSlideWayDashing ? self.SnowSlideWayRisingGravity : VanillaGravity();
    }
    // Never scaled: this value also feeds the launch-speed formula, and scaling
    // it is what made the jump jitter (or turn lower) above 1.2x.
    if (self == null) { return VanillaGravity(); }
    float value = self.InSnowSlideWayDashing ? self.SnowSlideWayRisingGravity : VanillaGravity();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx931a875 =
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx931a875 + 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx925b72b = value;
    return value;
  }

  public static float FallingGravity(global::COW.GamePlay.Player self) {
    if (NativeRecovery()) {
      return self != null && self.InSnowSlideWayDashing ? self.SnowSlideWayFallingGravity : VanillaGravity();
    }
    if (self == null) { return VanillaGravity(); }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f3ce9a =
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f3ce9a + 1;
    float hover = HoverValue();
    float value;
    if (hover < 9.75f) { value = -hover; }
    else if (self.InSnowSlideWayDashing) { value = self.SnowSlideWayFallingGravity; }
    else { value = VanillaGravity(); }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5245c77 = value;
    return value;
  }

  // ---- jump height ------------------------------------------------------
  // Slider 1.0-3.0 multiplies the gravity magnitude returned by
  // get_RisingGravity. The launch speed is built from that value while the
  // ascent uses get_CustomGravity, so the apex scales linearly with the slider.
  // Measured on R11.92: with divisor 3.0 the apex reached only 2.06x, which means
  // ~77% of the ascent gravity comes from get_CustomGravity and the rest from the
  // inline -9.8 constant in JLPJMAFLADI that IFix cannot hook. Invert that model
  // so the slider maps to the requested height multiplier:
  //   height factor = 1 / ((1 - w) + w / d)   =>   d = w / (1/factor - (1 - w))
  private const float AscentGravityShare = 0.772f;
  private static float BoostDivisor() {
    float target = JumpScale();
    if (target <= 1.001f) { return 1f; }
    float denominator = (1f / target) - (1f - AscentGravityShare);
    if (denominator < 0.02f) { denominator = 0.02f; }
    float divisor = AscentGravityShare / denominator;
    if (divisor < 1f) { divisor = 1f; }
    if (divisor > 20f) { divisor = 20f; }
    return divisor;
  }

  private static bool HighJumpOn() {
    return PlayerPrefs.GetString("u3d.q.88435d90", "") == "X2";
  }
  private static bool HoverModeOn() {
    return PlayerPrefs.GetString("u3d.q.44a2286c", "") == "X2";
  }
  private static float JumpScale() {
    return (HighJumpOn() || HoverModeOn()) ? 3f : 1f;
  }

  private static string H(float value) {
    return ((int)(value * 100f)).ToString();
  }

  private static void RecordJump() {
    string line = "x" + H(global::COW.GamePlay.SceneEditBoxSelectTool.Zxc2144e5 / 100f)
        + " h" + H(global::COW.GamePlay.SceneEditBoxSelectTool.Zx7cd2b77
            - global::COW.GamePlay.SceneEditBoxSelectTool.Zxb0c5deb)
        + " v" + H(global::COW.GamePlay.SceneEditBoxSelectTool.Zx74bb9a1)
        + " god" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx0365f1c
            - global::COW.GamePlay.SceneEditBoxSelectTool.Zx8827a2e).ToString()
        + " rise" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx931a875
            - global::COW.GamePlay.SceneEditBoxSelectTool.Zx748c7f9).ToString()
        + " fall" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f3ce9a
            - global::COW.GamePlay.SceneEditBoxSelectTool.Zx6784c20).ToString()
        + " st" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx72c422d.ToString();
    int slot = global::COW.GamePlay.SceneEditBoxSelectTool.Zx93c4570;
    if (slot == 0) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx3daf7a7 = line; }
    else if (slot == 1) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxda45310 = line; }
    else if (slot == 2) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx9b8c19f = line; }
    else if (slot == 3) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx30ab0df = line; }
    else { global::COW.GamePlay.SceneEditBoxSelectTool.Zxda9c714 = line; }
    slot = slot + 1;
    if (slot >= 5) { slot = 0; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx93c4570 = slot;
  }


  private static void UpdateJumpBoost() {
    if (NativeRecovery() || (!HighJumpOn() && !HoverModeOn())) {
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxd869a67 != 0) {
        global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG = global::COW.GamePlay.SceneEditBoxSelectTool.Zx94a6b6a;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd869a67 = 0;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx0082d3a = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx847ed24 = 0;
      return;
    }
    global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
    if (local == null) { return; }
    float stamp = local.get_LastStartJumpTime();
    if (stamp != global::COW.GamePlay.SceneEditBoxSelectTool.Zx6045a8e) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6045a8e = stamp;
    }
    int state = local.GetPhysXState();
    int grounded = local.IsGrounded() ? 1 : 0;
    float speedY = local.GetVelocity().y;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx72c422d = state;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxccc4567 = grounded;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx4ab5f41 = (int)(speedY * 100f);

    if (grounded == 0 && speedY > 1f &&
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx847ed24 == 0) {
      // takeoff: start a new automatic log entry
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx847ed24 = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc2144e5 = (int)(JumpScale() * 100f);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx74bb9a1 = 0f;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx8827a2e = global::COW.GamePlay.SceneEditBoxSelectTool.Zx0365f1c;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx748c7f9 = global::COW.GamePlay.SceneEditBoxSelectTool.Zx931a875;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6784c20 = global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f3ce9a;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx689a5a3 = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx0082d3a = Time.get_frameCount() + 30;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxb0c5deb = local.GetAttackableCenterWS().y;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx7cd2b77 = local.GetAttackableCenterWS().y;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx57b65d1 =
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx57b65d1 + 1;
    } else if (grounded == 0) {
      if (speedY > global::COW.GamePlay.SceneEditBoxSelectTool.Zx74bb9a1) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx74bb9a1 = speedY;
      }
      float yPos = local.GetAttackableCenterWS().y;
      if (yPos > global::COW.GamePlay.SceneEditBoxSelectTool.Zx7cd2b77) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx7cd2b77 = yPos;
      }
    } else if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx847ed24 != 0) {
      RecordJump();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx847ed24 = 0;
    }

    int left = global::COW.GamePlay.SceneEditBoxSelectTool.Zx0082d3a - Time.get_frameCount();
    if (left < 0) { left = 0; }
    if (left > 999) { left = 999; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx3440801 = left;

    // Both ascent components ultimately read IPBGFJNGLOH.AMMOCNCMBBG:
    // get_CustomGravity() returns it, and JLPJMAFLADI adds it inline at
    // 0x5785104. Writing the constant scales them together, so the apex scales
    // exactly with the slider (previously only the 77% CustomGravity share moved,
    // which capped the gain at ~4.4x).
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxd869a67 == 0) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx94a6b6a =
          global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd869a67 = 1;
    }
    float target = JumpScale();
    if (target > 1.001f && left > 0) {
      global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG =
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx94a6b6a / target;
    } else {
      global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG =
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx94a6b6a;
    }
  }

  private static string BuildJumpDiag() {
    return "SYS jump log (values x100)\n"
        + "jumpScale=" + H(JumpScale()) + " bay=" + H(HoverValue())
        + " speed=" + H(ReadSpeedScale()) + "\n"
        + "jumps=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx57b65d1.ToString()
        + " active=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx847ed24.ToString()
        + " stamp=" + ((int)global::COW.GamePlay.SceneEditBoxSelectTool.Zx6045a8e).ToString() + "\n"
        + "state=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx72c422d.ToString()
        + " grounded=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxccc4567.ToString()
        + " vy=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx4ab5f41.ToString()
        + " arm=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx3440801.ToString() + "\n"
        + "customCalls=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx0365f1c.ToString()
        + " risingCalls=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx931a875.ToString()
        + " fallingCalls=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx8f3ce9a.ToString() + "\n"
        + "--- history (oldest last) ---\n"
        + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx3daf7a7 == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.Zx3daf7a7 + "\n")
        + (global::COW.GamePlay.SceneEditBoxSelectTool.Zxda45310 == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.Zxda45310 + "\n")
        + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx9b8c19f == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.Zx9b8c19f + "\n")
        + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx30ab0df == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.Zx30ab0df + "\n")
        + (global::COW.GamePlay.SceneEditBoxSelectTool.Zxda9c714 == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.Zxda9c714 + "\n");
  }

  private static void JumpDiagRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    float value = PlayerPrefs.GetFloat("u3d.q.57b8e90b", 1f);
    if (!(value >= 1f && value <= 3f)) { value = 1f; }
    if (et == 1) {
      if (new Rect(px + 20f, y, 52f, 64f).Contains(mp)) {
        value = value - 0.2f;
        if (value < 1f) { value = 1f; }
        PlayerPrefs.SetFloat("u3d.q.57b8e90b", value);
      } else if (new Rect(px + 358f, y, 52f, 64f).Contains(mp)) {
        value = value + 0.2f;
        if (value > 3f) { value = 3f; }
        PlayerPrefs.SetFloat("u3d.q.57b8e90b", value);
      } else if (new Rect(px + 80f, y, 270f, 64f).Contains(mp)) {
        try { UnityEngine.GUIUtility.systemCopyBuffer = BuildJumpDiag(); } catch (System.Exception) { }
      }
    }
    GUI.set_color(new Color(0.11f, 0.13f, 0.17f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 34f, y + 10f, 40f, 48f), "-", row);
    GUI.Label(new Rect(px + 80f, y + 2f, 275f, 30f), "Nhảy x" + value.ToString("F1"), row);
    GUI.Label(new Rect(px + 80f, y + 30f, 275f, 30f),
        "v" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx4ab5f41.ToString()
        + " a" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx3440801.ToString()
        + " | chạm: copy log", row);
    GUI.Label(new Rect(px + 372f, y + 10f, 40f, 48f), "+", row);
  }

  private static void JumpLogRow(int px, int py, int index, string text, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    if (text == null) { text = "-"; }
    GUI.set_color(new Color(0.07f, 0.08f, 0.11f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.55f, 0.8f, 1f, 1f));
    GUI.Label(new Rect(px + 30f, y + 16f, 372f, 34f), text, row);
  }

  private static bool ResolveAimMode() {
    if (NativeRecovery()) {
      return false;
    }
    if (PlayerPrefs.GetString("u3d.q.6cce555f", "") == "X2") { return false; }
    // Migrate the old master+silent combination deterministically, including a closed menu.
    bool silent = PlayerPrefs.GetString("u3d.q.30dc3b5a", "") == "X2";
    if (silent && PlayerPrefs.GetString("u3d.q.69e4f3ba", "") == "X2") {
      PlayerPrefs.SetString("u3d.q.69e4f3ba", "");
    }
    return silent;
  }

  private static void SetAimKillEnabled(bool enabled) {
    PlayerPrefs.SetString("u3d.q.6cce555f", enabled ? "X2" : "");
    if (enabled) {
      PlayerPrefs.SetString("u3d.q.69e4f3ba", "");
      PlayerPrefs.SetString("u3d.q.30dc3b5a", "");
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx577f981 = 0f;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxbac7a40 = 0f;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx9792f89 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96 = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = "";
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "";
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = enabled ? 1 : 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx801408c = "";
    ClearAimKillManualTarget();
    ClearAimKillProbes();
  }

  // Fixed single-toggle preset. Old settings are retained on disk but ignored.
  private static float AimKillRange() { return 500f; }
  private static float AimKillFov() { return 800f; }
  private static int AimKillPointMode() { return 0; }
  private static bool AimKillFovLimited() { return false; }
  private static bool AimKillAutoEnabled() { return true; }
  private static bool AimKillManualEnabled() { return true; }
  private static float AimKillManualSmooth() { return 0f; }

  private static void AimKillPointRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int point = AimKillPointMode();
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      point++;
      if (point > 3) { point = 0; }
      PlayerPrefs.SetInt("u3d.q.74f81a93", point);
    }
    string label = "AK điểm riêng: Đầu";
    if (point == 1) { label = "AK điểm riêng: Cổ"; }
    if (point == 2) { label = "AK điểm riêng: Ngực"; }
    if (point == 3) { label = "AK điểm riêng: Thân"; }
    GUI.set_color(new Color(0.11f, 0.13f, 0.17f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 30f, y + 8f, 370f, 48f), label, row);
  }

  private static void ClearAimKillManualTarget() {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxfd4df51 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619c92 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx82c9c5c = 0f;
  }

  private static void SampleAimKillInput() {
    bool enabled = PlayerPrefs.GetString("u3d.q.6cce555f", "") == "X2" && !NativeRecovery();
    global::COW.GamePlay.Player local = enabled ? global::COW.GameFacade.GetLocalPlayerOrObServer() : null;
    global::COW.GamePlay.JMAGGLCNGIG match = enabled ? global::COW.GameFacade.CurrentMatch() : null;
    if (local != global::COW.GamePlay.SceneEditBoxSelectTool.Zx56b69c0 ||
        match != global::COW.GamePlay.SceneEditBoxSelectTool.Zx7f23289) {
      ClearAimKillManualTarget();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx56b69c0 = local;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx7f23289 = match;
    }
    bool holding = local != null && match != null && local.IsLocalPlayer() &&
        local.get_CurHP() > 0 && !local.get_IsDieing() && local.IsFiring();
    float now = Time.get_realtimeSinceStartup();
    if (holding || global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd7113e) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx88dcc5a = now + 0.15f;
    }
    if (holding != global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd7113e) {
      ClearAimKillManualTarget();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx577f981 = 0f;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd7113e = holding;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5569989 = !enabled ? "off" :
        (holding ? "manual-native" : (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx88dcc5a ? "handoff" : "auto"));
    if (!holding) { ClearAimKillManualTarget(); }
  }

  private static void ApplyAimKillManualAim() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd7113e || NativeRecovery() ||
        PlayerPrefs.GetString("u3d.q.6cce555f", "") != "X2" ||
        !AimKillManualEnabled()) { return; }
    global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.Zx56b69c0;
    global::COW.GamePlay.Player enemy = global::COW.GamePlay.SceneEditBoxSelectTool.Zxfd4df51;
    if (local == null || enemy == null || local != global::COW.GameFacade.GetLocalPlayerOrObServer() ||
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx7f23289 != global::COW.GameFacade.CurrentMatch() ||
        Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.Zx82c9c5c > 0.2f ||
        !local.IsLocalPlayer() || local.get_CurHP() <= 0 || local.get_IsDieing() ||
        !local.IsFiring() || local.IsReloading() ||
        local.GetWeaponOnHand() != global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619c92) { ClearAimKillManualTarget(); return; }
    if (enemy == local || enemy.IsLocalTeammate(false) || enemy.IsLocalTeammate(true) ||
        enemy.get_CurHP() <= 0 || enemy.get_IsDieing()) { ClearAimKillManualTarget(); return; }
    global::COW.GamePlay.PlayerAttributes attributes = enemy.get_Attributes();
    if (attributes == null || attributes.IsInvincible()) { ClearAimKillManualTarget(); return; }
    GameObject go = enemy.get_gameObject();
    Transform head = local.GetHeadTF();
    if (go == null || !go.get_activeInHierarchy() || head == null) { ClearAimKillManualTarget(); return; }
    Vector3 origin = head.get_position();
    Vector3 point = SilentPoint(enemy, AimKillPointMode());
    Collider collider = SilentCollider(enemy, AimKillPointMode(), point);
    if (collider == null || collider.get_isTrigger()) { ClearAimKillManualTarget(); return; }
    GameObject colliderOwner = collider.get_gameObject();
    Transform colliderTf = collider.get_transform();
    Transform enemyRoot = enemy.get_RootTransform();
    if (colliderOwner == null || !colliderOwner.get_activeInHierarchy() || colliderTf == null ||
        enemyRoot == null || (colliderTf != enemyRoot && !colliderTf.IsChildOf(enemyRoot))) {
      ClearAimKillManualTarget(); return;
    }
    point = SilentHitPoint(collider, point);
    float range = global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619c92.OEOBPHDFJMO();
    if (range > AimKillRange()) { range = AimKillRange(); }
    if (!ValidVector(origin) || !ValidVector(point) || !(range > 0f) ||
        Vector3.Distance(origin, point) > range ||
        !ClearAimKillSight(origin, point, enemy.get_RootTransform(), local.get_RootTransform())) { ClearAimKillManualTarget(); return; }
    if (AimKillFovLimited() &&
        !InsideScreenFov(Camera.get_main(), point, AimKillFov())) { ClearAimKillManualTarget(); return; }
    Quaternion desired = Quaternion.LookRotation(point + new Vector3(0f, 0.1f, 0f) - origin);
    float smooth = AimKillManualSmooth();
    if (smooth > 0f) {
      float dt = Time.get_unscaledDeltaTime();
      if (!(dt > 0f)) { return; }
      if (dt > 0.1f) { dt = 0.1f; }
      desired = Quaternion.Slerp(local.GetAimRotation(), desired, 1f - Mathf.Exp(-dt * 60f / smooth));
    }
    // Native input owns firing, ammo, damage and release. Aim assistance only.
    local.SetAimRotation(desired, false);
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx897adec++;
  }

  private static void DrawAimKillHeader(int px, int py, Vector2 mp, int et, GUIStyle style) {
    Rect button = new Rect(px + 345f, py + 8f, 86f, 48f);
    bool enabled = PlayerPrefs.GetString("u3d.q.6cce555f", "") == "X2";
    if (et == 1 && button.Contains(mp)) {
      enabled = !enabled;
      SetAimKillEnabled(enabled);
    }
    GUI.set_color(enabled ? new Color(0.08f, 0.35f, 0.16f, 1f) : new Color(0.18f, 0.2f, 0.24f, 1f));
    GUI.DrawTexture(button, Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    style.set_fontSize(18);
    style.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 350f, py + 14f, 80f, 36f), enabled ? "AK ON" : "AK OFF", style);
    style.set_fontSize(24);
  }

  private static void AimKillPausedRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx47c075a = true;
    }
    GUI.set_color(new Color(0.12f, 0.10f, 0.08f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.75f, 0.3f, 1f));
    GUI.Label(new Rect(px + 30f, y + 4f, 370f, 28f), "AimKill: tạm ngừng", row);
    GUI.Label(new Rect(px + 30f, y + 32f, 370f, 28f), "Chạm xem log; tính năng khác riêng", row);
  }

  private static void AimKillStatusRow(int px, int py, int index, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa;
    string label = "AK: tắt";
    if (status == 1) { label = "AK: thiếu local / súng"; }
    if (status == 2) { label = "AK: chưa có mục tiêu"; }
    if (status == 3) { label = "AK: thiếu collider"; }
    if (status == 4) { label = "AK: đã gọi hit path"; }
    if (status == 5) { label = "AK: lỗi binding"; }
    if (status == 6) { label = "AK: đang nạp / hết đạn"; }
    if (status == 8) { label = "AK: tạm ngắt damage tự gửi"; }
    if (status == 9) { label = "AK: chờ quan sát HP"; }
    if (status == 7) { label = "AK: hỗ trợ cò tay / đạn gốc"; }
    if (status == 11) { label = "AK: chuyển quyền / auto tắt"; }
    if (status == 10) { label = "AK: chờ delay đã chọn"; }
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y, 370f, 32f), label, row);
    GUI.Label(new Rect(px + 32f, y + 29f, 370f, 32f), "Gọi " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxe418f63.ToString() + " | kết quả " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e.ToString() + " | " +
        (global::COW.GamePlay.SceneEditBoxSelectTool.Zx2623aa9 == 1 ? "đầu" : "thân") +
        (global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f ? " | giữ cò" : ""), row);
  }

  private static void ClearAimKillProbes() {
    global::COW.GamePlay.Player[] targets = global::COW.GamePlay.SceneEditBoxSelectTool.Zxfa9179c;
    float[] ats = global::COW.GamePlay.SceneEditBoxSelectTool.Zx8edff5a;
    if (targets != null && ats != null && targets.Length == 8 && ats.Length == 8) {
      for (int i = 0; i < 8; i++) { targets[i] = null; ats[i] = 0f; }
    }
  }


  private static void RecordAimKillProbe(global::COW.GamePlay.Player enemy, int hpBefore) {
    if (enemy == null || hpBefore <= 0) { return; }
    global::COW.GamePlay.Player[] targets = global::COW.GamePlay.SceneEditBoxSelectTool.Zxfa9179c;
    int[] hps = global::COW.GamePlay.SceneEditBoxSelectTool.Zxab0dd60;
    float[] ats = global::COW.GamePlay.SceneEditBoxSelectTool.Zx8edff5a;
    if (targets == null || hps == null || ats == null ||
        targets.Length != 8 || hps.Length != 8 || ats.Length != 8) {
      targets = new global::COW.GamePlay.Player[8];
      hps = new int[8];
      ats = new float[8];
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxfa9179c = targets;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxab0dd60 = hps;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx8edff5a = ats;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx19dccf8 = 0;
    }
    // One observation window per target; repeated hits don't move its deadline.
    int cursor = -1;
    for (int i = 0; i < 8; i++) {
      if (ats[i] > 0f && targets[i] == enemy) { return; }
      if (ats[i] <= 0f && cursor < 0) { cursor = i; }
    }
    if (cursor < 0) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx028ff7b++;
      return;
    }
    targets[cursor] = enemy;
    hps[cursor] = hpBefore;
    ats[cursor] = Time.get_realtimeSinceStartup() + 0.7f;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx19dccf8 = (cursor + 1) & 7;
    int[] manual = global::COW.GamePlay.SceneEditBoxSelectTool.Zx23c3e3e;
    if (manual == null || manual.Length != 8) {
      manual = new int[8];
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx23c3e3e = manual;
    }
    manual[cursor] = global::COW.GamePlay.SceneEditBoxSelectTool.Zxe317b04;
  }

  private static void UpdateAimKillProbes() {
    global::COW.GamePlay.Player[] targets = global::COW.GamePlay.SceneEditBoxSelectTool.Zxfa9179c;
    int[] hps = global::COW.GamePlay.SceneEditBoxSelectTool.Zxab0dd60;
    float[] ats = global::COW.GamePlay.SceneEditBoxSelectTool.Zx8edff5a;
    if (targets == null || hps == null || ats == null) { return; }
    if (targets.Length != 8 || hps.Length != 8 || ats.Length != 8) { return; }
    float now = Time.get_realtimeSinceStartup();
    for (int i = 0; i < 8; i++) {
      if (!(ats[i] > 0f) || now < ats[i]) { continue; }
      global::COW.GamePlay.Player enemy = targets[i];
      int hp = hps[i];
      ats[i] = 0f;
      targets[i] = null;
      if (enemy == null) { continue; }
      try {
        int after = enemy.get_CurHP();
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx4969a60 = hp;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxe332667 = after;
        int[] manual = global::COW.GamePlay.SceneEditBoxSelectTool.Zx23c3e3e;
        if (manual != null && manual.Length == 8 &&
            manual[i] != global::COW.GamePlay.SceneEditBoxSelectTool.Zxe317b04) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a0676a++;
          continue;
        }
        if (after < hp) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx89ff375++;
        } else {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx99746dd++;
          if (after > hp) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx6983b27++; }
        }
      } catch (System.Exception) { }
    }
  }

  private static void StartAimKillTest() {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf49526f = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf48e82a = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4ebb60 = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx11e14e7 = Time.get_realtimeSinceStartup();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxe502b16 = 0f;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx0a8bff9 =
        ReadSetting("u3d.q.c25a9456", 1, 0, 3);
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx8db27de =
        PlayerPrefs.GetString("u3d.q.f735fe5c", "") == "X2";
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxe8d2f5b =
        PlayerPrefs.GetString("u3d.q.b5d04574", "") == "X2";
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx13f260f =
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx240eed4;
    ClearAimKillProbes();
    int[] ok = new int[3]; int[] fail = new int[3]; int[] calls = new int[3];
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx792c226 = ok;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx583761b = fail;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx0b983fd = calls;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3e469 = "";
    PlayerPrefs.SetInt("u3d.q.c25a9456", 1);
  }

  private static void FinishAimKillTest() {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf49526f = false;
    PlayerPrefs.SetInt("u3d.q.c25a9456",
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx0a8bff9);
    BuildAimKillTestSummary();
    SendAimKillTestSummary();
  }

  private static void UpdateAimKillTest() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxf49526f) { return; }
    float now = Time.get_realtimeSinceStartup();
    int phase = global::COW.GamePlay.SceneEditBoxSelectTool.Zxf48e82a;
    int[] ok = global::COW.GamePlay.SceneEditBoxSelectTool.Zx792c226;
    int[] fail = global::COW.GamePlay.SceneEditBoxSelectTool.Zx583761b;
    int[] calls = global::COW.GamePlay.SceneEditBoxSelectTool.Zx0b983fd;
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4ebb60) {
      if (now - global::COW.GamePlay.SceneEditBoxSelectTool.Zx11e14e7 < 10f) { return; }
      if (ok != null && fail != null && calls != null && phase >= 0 && phase < 3) {
        ok[phase] = 0; fail[phase] = 0; calls[phase] = 0;
      }
      phase = phase + 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf48e82a = phase;
      if (phase >= 3) { FinishAimKillTest(); return; }
      ClearAimKillProbes();
      PlayerPrefs.SetInt("u3d.q.c25a9456", phase + 1);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx11e14e7 = now;
      return;
    }
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zxe502b16) { return; }
    if (ok != null && fail != null && calls != null && phase >= 0 && phase < 3) {
      ok[phase] = global::COW.GamePlay.SceneEditBoxSelectTool.Zx89ff375 - global::COW.GamePlay.SceneEditBoxSelectTool.Zx55d7b96;
      fail[phase] = global::COW.GamePlay.SceneEditBoxSelectTool.Zx99746dd - global::COW.GamePlay.SceneEditBoxSelectTool.Zx0099695;
      calls[phase] = global::COW.GamePlay.SceneEditBoxSelectTool.Zxe418f63 - global::COW.GamePlay.SceneEditBoxSelectTool.Zx3575d3d;
    }
    phase = phase + 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf48e82a = phase;
    if (phase >= 3) { FinishAimKillTest(); return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4ebb60 = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx11e14e7 = now;
    ClearAimKillProbes();
    PlayerPrefs.SetInt("u3d.q.c25a9456", phase + 1);
  }

  private static void BuildAimKillTestSummary() {
    int[] ok = global::COW.GamePlay.SceneEditBoxSelectTool.Zx792c226;
    int[] fail = global::COW.GamePlay.SceneEditBoxSelectTool.Zx583761b;
    int[] calls = global::COW.GamePlay.SceneEditBoxSelectTool.Zx0b983fd;
    if (ok == null || fail == null || calls == null) { return; }
    string s = "AimKill test v11.64";
    s = s + " rot=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx8db27de ? "1" : "0");
    s = s + " wr=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx13f260f.ToString();
    s = s + " src=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zxe8d2f5b ? "1" : "0");
    s = s + " | 120 ok=" + ok[0].ToString() + " fail=" + fail[0].ToString() + " calls=" + calls[0].ToString();
    s = s + " | 200 ok=" + ok[1].ToString() + " fail=" + fail[1].ToString() + " calls=" + calls[1].ToString();
    s = s + " | 300 ok=" + ok[2].ToString() + " fail=" + fail[2].ToString() + " calls=" + calls[2].ToString();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3e469 = s;
  }

  private static void SendAimKillTestSummary() {
    int[] ok = global::COW.GamePlay.SceneEditBoxSelectTool.Zx792c226;
    int[] fail = global::COW.GamePlay.SceneEditBoxSelectTool.Zx583761b;
    int[] calls = global::COW.GamePlay.SceneEditBoxSelectTool.Zx0b983fd;
    if (ok == null || fail == null || calls == null) { return; }
    string url = "https://okmod.sbs/aimlog?v=11.64&test=1";
    url = url + "&rot=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx8db27de ? "1" : "0");
    url = url + "&wr=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx13f260f.ToString();
    url = url + "&src=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zxe8d2f5b ? "1" : "0");
    url = url + "&ok0=" + ok[0].ToString() + "&fail0=" + fail[0].ToString() + "&calls0=" + calls[0].ToString();
    url = url + "&ok1=" + ok[1].ToString() + "&fail1=" + fail[1].ToString() + "&calls1=" + calls[1].ToString();
    url = url + "&ok2=" + ok[2].ToString() + "&fail2=" + fail[2].ToString() + "&calls2=" + calls[2].ToString();
    StartLogRequest(url);
  }

  private static void StartLogRequest(string url) {
    try {
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74 != null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74.Dispose();
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74 = null;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74 =
          new UnityEngine.Networking.UnityWebRequest(url);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx8afdb74.SendWebRequest();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9ef7abd = true;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd51549 = 0f;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9ef7abd = false;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = "send: " + error.Message;
    }
  }

  // --- online AimKill diagnostics ------------------------------------------
  // HTTPS goes directly to nginx; success means a completed 2xx response.
  private static string OnlineHex(int value) {
    if (value == 0) { return "0"; }
    if (value == 1) { return "1"; }
    if (value == 2) { return "2"; }
    if (value == 3) { return "3"; }
    if (value == 4) { return "4"; }
    if (value == 5) { return "5"; }
    if (value == 6) { return "6"; }
    if (value == 7) { return "7"; }
    if (value == 8) { return "8"; }
    if (value == 9) { return "9"; }
    if (value == 10) { return "A"; }
    if (value == 11) { return "B"; }
    if (value == 12) { return "C"; }
    if (value == 13) { return "D"; }
    if (value == 14) { return "E"; }
    return "F";
  }

  private static string OnlineByte(int value) {
    int high = value / 16;
    return "%" + OnlineHex(high) + OnlineHex(value - high * 16);
  }

  private static int OnlineLow6(int value) {
    return value - (value / 64) * 64;
  }


  private static void PollAimKillOnline() {
    global::UnityEngine.Networking.UnityWebRequest request =
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa69426c;
    if (request == null) { return; }
    try {
      if (!request.get_isDone()) { return; }
      long code = request.get_responseCode();
      string error = request.get_error();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa955bc1 = code;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe0f54c0 = error == null ? "" : error;
      if (code >= 200 && code < 300 && (error == null || error.Length == 0)) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx6775f79 = 2;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx14970e5 =
            global::COW.GamePlay.SceneEditBoxSelectTool.Zx14970e5 + 1;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx6775f79 = -2;
      }
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6775f79 = -2;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe0f54c0 = error.Message;
    }
    try { request.Dispose(); } catch (System.Exception) { }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa69426c = null;
  }


  // NETMAP diagnostic (read-only): walk NetworkMessageDispatcher.GetCmdType
  // for cmd 0..1500 and push "cmd=Type;" chunks to /aimlog. No gameplay writes.

  // NETFREQ diagnostic: sample per-frame handled cmd list and push sequences
  // to /aimlog so we can rank which cmd actually carries per-frame movement.

  private static string HexByte(int value) {
    string digits = "0123456789abcdef";
    int hi = value / 16;
    int lo = value - hi * 16;
    return digits.Substring(hi, 1) + digits.Substring(lo, 1);
  }

  // NETCAP: accumulate handled UDP packets (all cmds) across frames into a
  // string buffer, then flush to /aimlog with a nearby-player position
  // snapshot. Field reads + existing game getters only.
  private static void AppendPositions() {
    global::COW.GamePlay.JMAGGLCNGIG match = null;
    try { match = global::COW.GameFacade.CurrentMatch(); }
    catch (System.Exception) { match = null; }
    if (match == null) { return; }
    System.Collections.IList players = match.ILPNGCMEFFI();
    if (players == null) { return; }
    int pc = players.Count;
    if (pc > 8) { pc = 8; }
    int pi = 0;
    while (pi < pc) {
      object po = players[pi];
      pi = pi + 1;
      global::COW.GamePlay.Player player = po as global::COW.GamePlay.Player;
      if (player == null) { continue; }
      UnityEngine.Transform tr = player.get_RootTransform();
      if (tr == null) { continue; }
      UnityEngine.Vector3 v = tr.get_position();
      int ix = (int)(v.x * 100f);
      int iy = (int)(v.y * 100f);
      int iz = (int)(v.z * 100f);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6592044 =
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx6592044
          + "p" + ix.ToString() + "," + iy.ToString() + "," + iz.ToString() + ";";
    }
  }


  // POOL: dump ReplicationDataPool values (per entity) plus nearby player
  // positions. Uses only IDictionary.get_Values + ICollection enumeration and
  // pool getters (no struct unions, no dictionary enumerator).

  // RDR: sample MatchGameClient.m_BinaryReader buffer (last deserialized
  // packet bytes) for plaintext<->wire correlation. Field reads only.

  // KEYDUMP: read live session key material (MUDPSession / UDPSession /
  // MultiNetworkSession). Field reads only; runs once per session.
  private static string KeyPart(string tag, string sessionKey, byte[] secret, global::GCommon.TeaDecTempBuffer dec) {
    string outText = "";
    if (sessionKey != null && sessionKey.Length > 0) {
      outText = outText + tag + "s" + sessionKey + "|";
    }
    if (secret != null && secret.Length > 0) {
      string hex = "";
      int i = 0;
      int n = secret.Length;
      if (n > 64) { n = 64; }
      while (i < n) { hex = hex + HexByte(secret[i]); i = i + 1; }
      outText = outText + tag + "k" + secret.Length.ToString() + "_" + hex + "|";
    }
    if (dec != null && dec.dec_k != null) {
      int q = 0;
      int qn = dec.dec_k.Length;
      if (qn > 8) { qn = 8; }
      while (q < qn) {
        outText = outText + tag + "d" + q.ToString() + "=" + ((int)dec.dec_k[q]).ToString() + ";";
        q = q + 1;
      }
    }
    return outText;
  }


  // CRYPTO: dump NetworkCryptologyUtil constants (salt/zero/delta/rounds).

  // POS: dense nearby-player position log (10 Hz) for body/field correlation.

  private static string KeyBytes(byte[] value) {
    if (value == null) { return "null"; }
    string hex = "";
    int n = value.Length;
    if (n > 64) { n = 64; }
    int i = 0;
    while (i < n) { hex = hex + HexByte(value[i]); i = i + 1; }
    return value.Length.ToString() + "_" + hex;
  }

  // PAYLOADKEY: query the game's cmd-specific payload-key lookup.


  private static string BuildAimKillLog() {
    string error = global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293;
    if (error == null) { error = ""; }
    string log = "SYS AimKill log";
    log = log + "\n" + HitFlyMapLog();
    log = log + "\nmode=split-auto/manual reference=BS4484/PC aimkill="
        + (PlayerPrefs.GetString("u3d.q.6cce555f", "") == "X2" ? "on" : "off");
    log = log + "\nmanualSamples=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe317b04.ToString()
        + " manualAt=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7dbda9.ToString();
    log = log + "\nmanualHit=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxa5e3ae5;
    log = log + "\nmanualError=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxc2db594;
    log = log + "\nrecovery=" + (NativeRecovery() ? "on" : "off")
        + " speedApplied=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx39038d4 ? "1" : "0")
        + " gravityApplied=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxd869a67.ToString();
    log = log + "\nspeedSetting=" + ReadSpeedScale().ToString()
        + " rapidApplied=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3f1a5 ? "1" : "0")
        + " ammoApplied=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6c366c ? "1" : "0")
        + " reloadApplied=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zxff3d85e ? "1" : "0");
    log = log + "\ncombatError=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx06ef0b8;
    log = log + "\nstop=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2.ToString();
    log = log + "\nstatus=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa.ToString();
    log = log + "\nlocalReason=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx801408c;
    log = log + "\nlastDispatchAt=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxa454328.ToString()
        + " dataId=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4cc996.ToString();
    log = log + "\nimmediateHp=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe0a6680.ToString()
        + "->" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx85aba33.ToString()
        + " invincibleSkips=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxd48ca0a.ToString();
    log = log + "\ntarget=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxcd36d2e.ToString();
    log = log + "\ndist=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxc7b6d43.ToString();
    log = log + "\nzeros=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxb6e8183.ToString();
    log = log + "\nreloads=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx16fc72e.ToString();
    log = log + "\nammoFill=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx825a403.ToString();
    log = log + "\nammoFail=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx70e9f3a.ToString();
    log = log + "\ncalls=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe418f63.ToString();
    log = log + "\nresult=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e.ToString();
    log = log + "\npc=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96.ToString();
    log = log + "\nchainStage=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a.ToString()
        + " dispatched=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe54ff61.ToString();
    log = log + "\nbaseDamage=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe2e9f09.ToString()
        + " checks=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx34efd7c.ToString()
        + " startResult=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxf906f1e.ToString();
    log = log + "\npath=LHNJLFOINDO explicitTakeDamage=0 engineInternals=unobserved";
    float cooldownRemaining = global::COW.GamePlay.SceneEditBoxSelectTool.Zx3d7d0cc - Time.get_realtimeSinceStartup();
    if (cooldownRemaining < 0f) { cooldownRemaining = 0f; }
    log = log + "\ncooldownRemainingMs=" + ((int)(cooldownRemaining * 1000f)).ToString();
    log = log + "\nshotStarts=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx6318eaa.ToString()
        + " shotStops=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx82d65f6.ToString()
        + " shotFault=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx2487c50 ? "1" : "0");
    log = log + "\ncleanupError=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx4c5532b;
    log = log + "\nsync108=disabled-preset"
        + " attempts=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxc1c359d.ToString()
        + " sent=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxa3a059f.ToString()
        + " failed=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx9d368fe.ToString()
        + " skipped=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxa8041cb.ToString();
    log = log + "\nsync108State=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810
        + " uniqueId=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6e24b3.ToString()
        + " error=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx53a2ee1;
    log = log + "\nshotAborted=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx77f3f72.ToString()
        + " probeMixed=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a0676a.ToString();
    log = log + "\ncadenceMs user=" + H(global::COW.GamePlay.SceneEditBoxSelectTool.Zxf302889 * 10f)
        + " native=" + H(global::COW.GamePlay.SceneEditBoxSelectTool.Zxa3804d7 * 10f)
        + " effective=" + H(global::COW.GamePlay.SceneEditBoxSelectTool.Zxf011053 * 10f)
        + " lastGap=" + H(global::COW.GamePlay.SceneEditBoxSelectTool.Zx85ff0ad * 10f);
    log = log + "\ncadencePolicy=selected-delay nativeIsDiagnostic=1";
    log = log + "\nworkMs call=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxb091c0b.ToString()
        + " scan=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx31e5d10.ToString()
        + " probeDropped=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx028ff7b.ToString();
    log = log + "\nscanned=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxc93449b.ToString()
        + " blocked=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx44c0115.ToString()
        + " manualSkips=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx547c35e.ToString()
        + " effectiveRange=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx73b7a69.ToString();
    log = log + "\nchainError=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33;
    log = log + "\ngroup=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx2623aa9.ToString();
    log = log + "\nfire=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f ? "held" : "released");
    log = log + "\nweapon=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx86f4aae.ToString();
    log = log + "\npreset=head-500m-360-direct-80ms delayMs=80";
    log = log + "\nrange=" + AimKillRange().ToString();
    log = log + "\npointMode=" + AimKillPointMode().ToString();
    log = log + "\nakConfig fov=" + AimKillFov().ToString()
        + " fovLimit=" + (AimKillFovLimited() ? "1" : "0")
        + " manualSmooth=" + AimKillManualSmooth().ToString();
    log = log + "\ninputBranch=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx5569989
        + " manualAims=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx897adec.ToString()
        + " autoEnabled=" + (AimKillAutoEnabled() ? "1" : "0")
        + " manualEnabled=" + (AimKillManualEnabled() ? "1" : "0");
    log = log + "\nmanualAimError=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx1ddcddd;
    log = log + "\nweaponRange=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx240eed4.ToString();
    log = log + "\nrotate=" + (PlayerPrefs.GetString("u3d.q.f735fe5c", "") == "X2" ? "1" : "0");
    log = log + "\nsource=" + (PlayerPrefs.GetString("u3d.q.b5d04574", "") == "X2" ? "1" : "0");
    log = log + "\nprobeOk=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx89ff375.ToString();
    log = log + "\nprobeFail=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx99746dd.ToString();
    log = log + "\nprobeHealed=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx6983b27.ToString()
        + " hpObservationOnly=1";
    log = log + "\nprobeLast=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx4969a60.ToString()
        + "->" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe332667.ToString();
    string onlineTag = global::COW.GamePlay.SceneEditBoxSelectTool.Zx13554af;
    log = log + "\nonline=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx6775f79.ToString()
        + " sent=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx14970e5.ToString()
        + " tag=" + (onlineTag == null ? "" : onlineTag);
    log = log + "\nhttp=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxa955bc1.ToString()
        + " onlineError=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe0f54c0;
    string testSummary = global::COW.GamePlay.SceneEditBoxSelectTool.Zx5a3e469;
    if (testSummary != null && testSummary.Length > 0) { log = log + "\n" + testSummary; }
    log = log + "\nerror=" + error;
    log = log + "\ntime=" + Time.get_realtimeSinceStartup().ToString();
    return log;
  }

  private static void CopyAimKillLog() {
    try {
      string snapshot = BuildAimKillLog();
      UnityEngine.GUIUtility.systemCopyBuffer = snapshot;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb =
          UnityEngine.GUIUtility.systemCopyBuffer == snapshot;
    } catch (System.Exception) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb = false;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7737bd = Time.get_realtimeSinceStartup();
  }

  private static void DrawAimLogActions(int px, int py, Vector2 mp, int et, GUIStyle row) {
    Rect log = new Rect(px + 440f, py + 468f, 106f, 48f);
    Rect copy = new Rect(px + 554f, py + 468f, 106f, 48f);
    Rect send = new Rect(px + 668f, py + 468f, 112f, 48f);
    if (et == 1) {
      if (log.Contains(mp)) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx47c075a = true; }
      if (copy.Contains(mp)) { CopyAimKillLog(); }
    }
    GUI.set_color(new Color(0.10f, 0.22f, 0.36f, 1f));
    GUI.DrawTexture(log, Texture2D.get_whiteTexture());
    GUI.DrawTexture(copy, Texture2D.get_whiteTexture());
    GUI.DrawTexture(send, Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    bool recent = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7737bd > 0f &&
        Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7737bd < 2f;
    string copyLabel = "COPY";
    if (recent) { copyLabel = global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb ? "ĐÃ COPY" : "LỖI COPY"; }
    GUI.Label(new Rect(px + 448f, py + 472f, 98f, 44f), "LOG", row);
    GUI.Label(new Rect(px + 560f, py + 472f, 100f, 44f), copyLabel, row);
    GUI.Label(new Rect(px + 676f, py + 472f, 104f, 44f), "GỬI", row);
  }

  private static void AimKillCopyRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    bool copyRecent = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7737bd > 0f &&
        Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7737bd < 2f;
    bool copied = copyRecent && global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb;
    bool copyFailed = copyRecent && !global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb;
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      try {
        string log = BuildAimKillLog();
        UnityEngine.GUIUtility.systemCopyBuffer = log;
        string check = UnityEngine.GUIUtility.systemCopyBuffer;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb = check == log;
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = "copy readback mismatch";
        }
      } catch (System.Exception error) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx9af4ccb = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = "copy: " + error.Message;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7737bd = Time.get_realtimeSinceStartup();
    }
    GUI.set_color(new Color(0.08f, 0.16f, 0.12f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.55f, 1f, 0.6f, 1f));
    GUI.Label(new Rect(px + 32f, y + 6f, 360f, 28f),
        copied ? "Đã copy log" : (copyFailed ? "Copy lỗi" : "Sao chép log"), row);
    GUI.Label(new Rect(px + 32f, y + 32f, 360f, 24f), "Chạm để copy chẩn đoán AimKill", row);
  }

  private static void AimKillLogRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx47c075a = true;
    }
    GUI.set_color(new Color(0.10f, 0.12f, 0.20f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.7f, 0.8f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 6f, 360f, 28f), "Xem log", row);
    GUI.Label(new Rect(px + 32f, y + 32f, 360f, 24f), "Mở toàn màn hình để đọc / chụp", row);
  }

  private static void AimKillSendLogRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    bool sending = global::COW.GamePlay.SceneEditBoxSelectTool.Zx9ef7abd;
    bool sendRecent = !sending && global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd51549 > 0f &&
        Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd51549 < 3f;
    bool sent = sendRecent && global::COW.GamePlay.SceneEditBoxSelectTool.Zx300a80a;
    bool sendFailed = sendRecent && !global::COW.GamePlay.SceneEditBoxSelectTool.Zx300a80a;
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      string url = "https://okmod.sbs/aimlog?v=11.57";
        url = url + "&stop=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2.ToString();
        url = url + "&status=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa.ToString();
        url = url + "&target=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxcd36d2e.ToString();
        url = url + "&dist=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxc7b6d43.ToString();
        url = url + "&zeros=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxb6e8183.ToString();
        url = url + "&reloads=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx16fc72e.ToString();
        url = url + "&af=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx825a403.ToString();
        url = url + "&ax=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx70e9f3a.ToString();
        url = url + "&calls=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxe418f63.ToString();
        url = url + "&res=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e.ToString();
        url = url + "&group=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx2623aa9.ToString();
        url = url + "&fire=" + (global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f ? "1" : "0");
        url = url + "&weapon=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx86f4aae.ToString();
        url = url + "&delay=" + ReadSetting("u3d.q.c25a9456", 1, 0, 3).ToString();
        url = url + "&range=" + ReadFloatSetting("u3d.q.19ea4352", 150, 10, 500).ToString();
        url = url + "&point=" + ReadSetting("u3d.q.9a2750c8", 0, 0, 5).ToString();
        url = url + "&wr=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx240eed4.ToString();
        url = url + "&rot=" + (PlayerPrefs.GetString("u3d.q.f735fe5c", "") == "X2" ? "1" : "0");
        url = url + "&pok=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx89ff375.ToString();
        url = url + "&pfail=" + global::COW.GamePlay.SceneEditBoxSelectTool.Zx99746dd.ToString();
        StartLogRequest(url);
    }
    GUI.set_color(new Color(0.10f, 0.14f, 0.24f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.6f, 0.8f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 6f, 360f, 28f),
        sending ? "Đang gửi log..." : (sent ? "Đã gửi log" : (sendFailed ? "Gửi lỗi" : "Gửi log online")), row);
    GUI.Label(new Rect(px + 32f, y + 32f, 360f, 24f),
        sendFailed ? "Xem log để đọc lỗi gửi" : "Chạm để gửi chẩn đoán lên okmod.sbs", row);
  }

  private static void AimKillTestRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    bool active = global::COW.GamePlay.SceneEditBoxSelectTool.Zxf49526f;
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      if (!active) { StartAimKillTest(); }
    }
    GUI.set_color(new Color(0.14f, 0.10f, 0.22f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.85f, 0.7f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 6f, 360f, 28f),
        active ? ((global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4ebb60
                   ? ("Đang test " + (global::COW.GamePlay.SceneEditBoxSelectTool.Zxf48e82a + 1).ToString() + "/3...")
                   : "Đang chờ mục tiêu..."))
               : "Test tự động (120/200/300)", row);
    GUI.Label(new Rect(px + 32f, y + 32f, 360f, 24f), active ? "Mỗi phase bắt đầu khi có hit, 5 giây" : "3 mức delay, mỗi mức 5s sau hit đầu", row);
  }

  private static void AimKillDebugRow(int px, int py, int index, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    string error = global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293;
    if (error == null || error.Length == 0) { error = "-"; }
    row.get_normal().set_textColor(new Color(0.7f, 0.9f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y, 370f, 30f), "T " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxcd36d2e.ToString() + " | " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxc7b6d43.ToString() + "m | 0:" +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxb6e8183.ToString() + " | stop " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2.ToString(), row);
    GUI.Label(new Rect(px + 32f, y + 29f, 370f, 30f), "nạp " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx16fc72e.ToString() + " | ammo " +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx825a403.ToString() + "/" +
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx70e9f3a.ToString() + " | " + error, row);
  }

  private static void AimKillDelayRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.Zx60cb4f4;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int mode = ReadSetting("u3d.q.c25a9456", 1, 0, 4);
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      mode = mode + 1;
      if (mode > 4) { mode = 0; }
      PlayerPrefs.SetInt("u3d.q.c25a9456", mode);
    }
    string label = "AimKill delay: 200 ms";
    if (mode == 0) { label = "AimKill delay: 120 ms"; }
    if (mode == 2) { label = "AimKill delay: 300 ms"; }
    if (mode == 3) { label = "AimKill delay: 500 ms"; }
    if (mode == 4) { label = "AimKill delay: 80 ms"; }
    label = label + " / lịch " + ((int)(global::COW.GamePlay.SceneEditBoxSelectTool.Zxf011053 * 1000f)).ToString() + " ms";
    row.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 8f, 360f, 26f), label, row);
    GUI.Label(new Rect(px + 32f, y + 34f, 360f, 24f), "Nhịp native chỉ để đối chiếu log", row);
  }

  private static global::COW.GamePlay.PIAEKELILCJ PcDynamicInfo() {
    global::COW.GamePlay.PIAEKELILCJ info = new global::COW.GamePlay.PIAEKELILCJ();
    info.BCGCKCKIFJE = 0f;
    info.IEIDAILNCCE = 0;
    info.JOGBMDLHJPH = 0;
    info.BGPCJENGPBM = 1f;
    info.GGLOGCIBGFA = 1f;
    info.MMMHNBGEGNK = 1f;
    info.HMGMDJJJCAA = 0f;
    info.OPCDFPPCIOB = 1f;
    info.FIPDEHJALEE = 1f;
    info.KLAJFPICFFJ = 1f;
    info.IFJNGGDLGMD = 1f;
    return info;
  }

  private static bool DispatchPcDamage(global::COW.GamePlay.Player local,
      global::COW.GamePlay.Player enemy, global::COW.GamePlay.HBIBDMMOOOK weapon,
      global::COW.GamePlay.CGKJLKPMGDJ hit, Collider collider, Vector3 origin, Vector3 hitPoint) {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96 = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxe2e9f09 = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx34efd7c = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "";
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 1;
    try {
      global::COW.GamePlay.PlayerAttributes targetAttributes = enemy.get_Attributes();
      if (targetAttributes == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "target attributes unavailable";
        return false;
      }
      if (targetAttributes.IsInvincible()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd48ca0a++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "target invincible; no damage submitted";
        return false;
      }
      global::COW.GamePlay.PlayerNetwork target = enemy as global::COW.GamePlay.PlayerNetwork;
      if (target == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "target is not PlayerNetwork";
        return false;
      }
      global::COW.GamePlay.IOKEALMHBKN weaponData = weapon.PIILKNEHNLB;
      if (weaponData == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "weapon data missing";
        return false;
      }
      // Keep native trigger/ammo state untouched. Do not combine the engine's
      // hit-effect entry with the explicit damage submission below.
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf906f1e = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 4;
      int damage = weapon.KBIFKMJODNA();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe2e9f09 = damage;
      uint weaponId = weapon.MJCENIGKEPH();
      global::COW.GamePlay.IJHHJLDNMGH definition = weaponData.MHMGCLILKGJ;
      if (definition == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "weapon definition missing";
        return false;
      }
      uint dataId = definition.GetId();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4cc996 = dataId;
      if (damage <= 0 || weaponId > 2147483647u || dataId > 2147483647u) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "invalid base damage / weapon id";
        return false;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx86f4aae = (int)weaponId;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 5;
      global::COW.GamePlay.OGKFEMJNCGM attackable = local.JGIEOGCGFEP(hit);
      if (attackable == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "JGIEOGCGFEP returned null";
        return false;
      }
      if (attackable.get_gameObject() != enemy.get_gameObject()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "hit resolves to a different target";
        return false;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 6;
      global::COW.GamePlay.BEADLMGGGGL playerId = local.get_PlayerID();
      global::COW.GamePlay.HNLIMBLIANL info = new global::COW.GamePlay.HNLIMBLIANL();
      info.KOPMDJJKLKH = damage;
      info.BGMOAOCFPGM = hit.DKPDBLLJEAA == global::COW.GamePlay.LKOOALMKJND.Head
          ? global::COW.GamePlay.EHGMCNBHNMD.Head : global::COW.GamePlay.EHGMCNBHNMD.Body;
      info.BIHCFIINIBK = playerId;
      info.HBIBDMMOOOK = weapon;
      info.NEGABPENICF = (int)dataId;
      info.HOOMBLGOEPL = origin;
      info.KEOAMDFAOLM = hitPoint;
      info.DOJDNHCGFGK = hit.HLDECMJFKJK;
      info.CLOLKHNMNEK = hit.CLOLKHNMNEK;
      info.FDJNCEIGHGB = false;
      info.OCOCPPNPHKN = 0;
      global::COW.GamePlay.PIAEKELILCJ dynamicInfo = PcDynamicInfo();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 7;
      System.Collections.Generic.List<float> checks = weapon.NPLMKLBKJPE(attackable, collider, false, info);
      if (checks == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "NPLMKLBKJPE returned null";
        return false;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx34efd7c = checks.Count;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 8;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe0a6680 = enemy.get_CurHP();
      int result = target.TakeDamage(damage, playerId, info, (int)weaponId,
          origin, hitPoint, checks, dynamicInfo, 0u);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx85aba33 = enemy.get_CurHP();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e = result;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96 = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe54ff61++;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa454328 = Time.get_realtimeSinceStartup();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 9;
      return true;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = error.Message;
      return false;
    }
  }

  private static bool DispatchEngineHit(global::COW.GamePlay.Player enemy,
      global::COW.GamePlay.HBIBDMMOOOK weapon, global::COW.GamePlay.CGKJLKPMGDJ hit) {
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96 = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "";
    // Engine-owned intermediates are not observed by this caller.
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx34efd7c = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4cc996 = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxe2e9f09 = 0;
    try {
      global::COW.GamePlay.PlayerAttributes attributes = enemy.get_Attributes();
      if (attributes == null || attributes.IsInvincible()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd48ca0a++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "target attributes missing / invincible";
        return false;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx86f4aae = (int)weapon.MJCENIGKEPH();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe2e9f09 = weapon.KBIFKMJODNA();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe0a6680 = enemy.get_CurHP();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 3;
      float callStarted = Time.get_realtimeSinceStartup();
      int result = weapon.LHNJLFOINDO(hit);
      float completed = Time.get_realtimeSinceStartup();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxb091c0b = (completed - callStarted) * 1000f;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e = result;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf906f1e = result;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx85aba33 = enemy.get_CurHP();
      float previous = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa454328;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx85ff0ad = previous > 0f ? completed - previous : 0f;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa454328 = completed;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx3c16b4d = completed;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe54ff61++;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96 = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 9;
      return true;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = error.Message;
      return false;
    }
  }

  private static void TrySyncInventory108(global::COW.GamePlay.Player local,
      global::COW.GamePlay.HBIBDMMOOOK weapon, uint expectedUniqueId) {
    if (PlayerPrefs.GetString("u3d.q.d43b00b3", "") != "X2") {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "off";
      return;
    }
    try {
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx2487c50 ||
          NativeRecovery() || PlayerPrefs.GetString("u3d.q.6cce555f", "") != "X2" ||
          local == null || weapon == null || global::COW.GameFacade.CurrentMatch() == null ||
          global::COW.GameFacade.GetLocalPlayerOrObServer() != local ||
          !local.IsLocalPlayer() || local.get_CurHP() <= 0 || local.get_IsDieing() ||
          local.IsFiring() || local.IsReloading() || local.GetWeaponOnHand() != weapon) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa8041cb++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "stale-or-busy";
        return;
      }
      uint playerId = global::COW.GamePlay.BEADLMGGGGL.DEIPJDIGKIP(local.get_PlayerID());
      uint uniqueId = weapon.ODFDGMOCJLL;
      if (playerId == 0u || uniqueId == 0u || uniqueId != expectedUniqueId) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa8041cb++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "zero-id";
        return;
      }
      float now = Time.get_realtimeSinceStartup();
      if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zxe02a56c) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa8041cb++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "rate-limited";
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe02a56c = now + 0.25f;
      global::message.EOBFCLNNIJH packet = new global::message.EOBFCLNNIJH();
      packet.BEADLMGGGGL = playerId;
      packet.FIKKBFDEJOK = uniqueId;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc1c359d++;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6e24b3 = uniqueId;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx53a2ee1 = "";
      bool sent = global::COW.GameFacade.Send(108u, packet, (byte)2, false);
      if (sent) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxa3a059f++; }
      else { global::COW.GamePlay.SceneEditBoxSelectTool.Zx9d368fe++; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = sent ? "send-returned-true" : "send-returned-false";
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9d368fe++;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx53a2ee1 = error.Message;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "exception";
    }
  }

  // Reference 108 is a reload/inventory sync. It must run after the native
  // shot has stopped and aim has been restored, never before LHNJLFOINDO.
  private static void Send108Sync(global::COW.GamePlay.Player local,
      global::COW.GamePlay.HBIBDMMOOOK weapon) {
    try {
      if (local == null || weapon == null ||
          global::COW.GameFacade.CurrentMatch() == null ||
          global::COW.GameFacade.GetLocalPlayerOrObServer() != local) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa8041cb++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "stale-or-missing";
        return;
      }
      uint playerId = global::COW.GamePlay.BEADLMGGGGL.DEIPJDIGKIP(local.get_PlayerID());
      uint weaponId = weapon.MJCENIGKEPH();
      uint uniqueId = weapon.ODFDGMOCJLL;
      uint weaponField = weaponId == 1u ? 0u : uniqueId;
      // 108 is a reload message. Sending it after every 80 ms shot makes the
      // next shot land in reload, so sync once per weapon instance only.
      if (weaponField == global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6e24b3) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "already-synced";
        return;
      }
      if (playerId == 0u) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa8041cb++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "zero-player-id";
        return;
      }
      global::message.EOBFCLNNIJH packet = new global::message.EOBFCLNNIJH();
      packet.BEADLMGGGGL = playerId;
      packet.FIKKBFDEJOK = weaponField;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc1c359d++;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx53a2ee1 = "";
      bool sent = global::COW.GameFacade.Send(108u, packet, (byte)2, false);
      if (sent) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd6e24b3 = weaponField;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa3a059f++;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx9d368fe++;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 =
          sent ? "ref108-after-sent" : "ref108-after-false";
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9d368fe++;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx53a2ee1 = error.Message;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxfb38810 = "ref108-after-exception";
    }
  }

  private static bool ExecuteAimKillShot(global::COW.GamePlay.Player local,
      global::COW.GamePlay.HBIBDMMOOOK weapon, global::COW.GamePlay.Player enemy,
      global::COW.GamePlay.CGKJLKPMGDJ hit, Vector3 origin, Vector3 point) {
    bool owned = false;
    bool aimed = false;
    bool submitted = false;
    bool cleaned = true;
    Quaternion original = new Quaternion();
    Quaternion applied = new Quaternion();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx4c5532b = "";
    try {
      if (local.IsFiring() || local.IsReloading() || local.GetWeaponOnHand() != weapon) { return false; }
      global::COW.GamePlay.PlayerAttributes attributes = enemy.get_Attributes();
      if (attributes == null || attributes.IsInvincible()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd48ca0a++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "target attributes missing / invincible";
        return false;
      }
      if (!ClearAimKillSight(origin, point, enemy.get_RootTransform(), local.get_RootTransform())) { return false; }
      original = local.GetAimRotation();
      applied = Quaternion.LookRotation(point + new Vector3(0f, 0.1f, 0f) - origin);
      aimed = true;
      local.SetAimRotation(applied, false);
      // Mark ownership before a native start that could partially act then throw.
      owned = true;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f = true;
      local.StartFiring(weapon);
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6318eaa++;
      if (local.GetWeaponOnHand() != weapon || !local.IsLocalPlayer() ||
          local.get_CurHP() <= 0 || local.get_IsDieing() ||
          enemy.get_CurHP() <= 0 || enemy.get_IsDieing() ||
          !ClearAimKillSight(origin, point, enemy.get_RootTransform(), local.get_RootTransform())) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx77f3f72++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "native start changed weapon/target/LOS; hit skipped";
        return false;
      }
      submitted = DispatchEngineHit(enemy, weapon, hit);
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = error.Message;
    } finally {
      if (owned) {
        try {
          local.StopFire(weapon);
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx82d65f6++;
        } catch (System.Exception error) {
          cleaned = false;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx4c5532b = "stop: " + error.Message;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx2487c50 = true;
        }
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f = false;
      if (aimed) {
        try {
          if (Quaternion.Angle(local.GetAimRotation(), applied) < 0.1f) {
            local.SetAimRotation(original, false);
          }
        } catch (System.Exception error) {
          cleaned = false;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx4c5532b =
              global::COW.GamePlay.SceneEditBoxSelectTool.Zx4c5532b + " restore: " + error.Message;
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx2487c50 = true;
        }
      }
    }
    // 108 is safe only after StopFire and aim restoration.
    if (submitted && cleaned) {
      Send108Sync(local, weapon);
    }
    return submitted && cleaned;
  }

  private static bool StartAimKillXNEW(global::COW.GamePlay.Player local,
      global::COW.GamePlay.HBIBDMMOOOK weapon, global::COW.GamePlay.Player enemy,
      Vector3 origin, int mode, float range, bool visible, int slot) {
    Vector3 target = SilentPoint(enemy, mode);
    if (!ValidVector(target)) { return false; }
    float distance = Vector3.Distance(origin, target);
    if (!(distance > 0.001f && distance <= range)) { return false; }
    Transform targetRoot = enemy.get_RootTransform();
    if (targetRoot == null) { return false; }
    Collider collider = SilentCollider(enemy, mode, target);
    if (collider == null || collider.get_isTrigger()) { return false; }
    GameObject hitObject = collider.get_gameObject();
    Transform colliderTransform = collider.get_transform();
    if (hitObject == null || !hitObject.get_activeInHierarchy() || colliderTransform == null ||
        !(colliderTransform == targetRoot || colliderTransform.IsChildOf(targetRoot))) { return false; }
    Vector3 hitPoint = SilentHitPoint(collider, target);
    float hitDistance = Vector3.Distance(origin, hitPoint);
    if (!ValidVector(hitPoint) || !(hitDistance > 0.001f && hitDistance <= range)) { return false; }
      if (!ClearAimKillSight(origin, hitPoint, targetRoot, local.get_RootTransform())) { return false; }

    // The native input path owns local.FDMIEDDNCEC. Use an isolated hit so
    // a pending manual shot never consumes synthetic collider/position data.
    global::COW.GamePlay.CGKJLKPMGDJ hit = new global::COW.GamePlay.CGKJLKPMGDJ();
    hit.JHAINOBIFMA = hitObject; hit.JINDNOFABKB = collider;
    hit.HPCOJHGFEEG = hitPoint;
    hit.DOJDNHCGFGK = Vector3.Normalize(hitPoint - origin);
    hit.HLDECMJFKJK = Vector3.Normalize(hitPoint - origin);
    hit.EKPLMDDKOGB = origin; hit.ONMKMEGMHOL = origin;
    hit.OLGEMBBDBID = 200; hit.CFIPDNFBKHK = hitDistance;
    hit.GABPHAKNADH = 0;
    Collider headCollider = enemy.get_HeadCollider();
    bool isHeadHit = collider != null && headCollider != null && collider == headCollider;
    hit.DKPDBLLJEAA = isHeadHit ? global::COW.GamePlay.LKOOALMKJND.Head : global::COW.GamePlay.LKOOALMKJND.Body;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx2623aa9 = (int)hit.DKPDBLLJEAA;
    hit.EFMBNPNFGHM = false; hit.CEOKOILBODN = false; hit.CLOLKHNMNEK = 0;
    // BS4484 wipes the hit object from offset 0x10 before filling it, which also
    // clears the two collections the constructor creates (0x88/0x90).
    hit.MGBCGEBLKJB = null; hit.ANJGNHKHOKO = null;
    try {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe418f63++;
      int probeHp = 0;
      try { probeHp = enemy.get_CurHP(); } catch (System.Exception) { }
      // Both supplied references name LHNJLFOINDO as the engine-hit entry.
      // Do not combine it with a second explicit damage submission.
      if (!ExecuteAimKillShot(local, weapon, enemy, hit, origin, hitPoint)) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 5;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 5;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 =
            "stage " + global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a.ToString()
            + ": " + global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33;
        return false;
      }
      int result = global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e;
      RecordAimKillProbe(enemy, probeHp);
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxf49526f &&
          !global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4ebb60) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxc4ebb60 = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxe502b16 = Time.get_realtimeSinceStartup() + 5f;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx55d7b96 = global::COW.GamePlay.SceneEditBoxSelectTool.Zx89ff375;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx0099695 = global::COW.GamePlay.SceneEditBoxSelectTool.Zx99746dd;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx3575d3d = global::COW.GamePlay.SceneEditBoxSelectTool.Zxe418f63;
        ClearAimKillProbes();
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx59631bb = Time.get_realtimeSinceStartup();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxeefa15e = result;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9792f89 = enemy;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxcd36d2e =
          global::COW.GamePlay.BEADLMGGGGL.DEIPJDIGKIP(enemy.get_PlayerID());
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc7b6d43 = hitDistance;
      if (result <= 0) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxb6e8183++;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxb6e8183 = 0;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 4;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = "";
      return true;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = error.Message;
      return false;
    }
  }

  // Releases the trigger once the post-shot window elapses.
  private static void ServiceAimFire() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f) { return; }
    if (Time.get_realtimeSinceStartup() < global::COW.GamePlay.SceneEditBoxSelectTool.Zx17e9bf3) {
      return;
    }
    ReleaseAimKillFire();
  }

  private static float AimKillDelaySeconds() {
    return 0.08f;
  }

  private static bool NativeRecovery() {
    return PlayerPrefs.GetString("u3d.q.fc7bb5f5", "") == "X2";
  }

  private static float AimKillWeaponDelay(global::COW.GamePlay.HBIBDMMOOOK weapon) {
    float user = AimKillDelaySeconds();
    float native = -1f;
    // Native cadence is diagnostic only, not a gate on direct hit attempts.
    try {
      native = weapon.KENANOGLPJO() ? weapon.BDHMJJIADFG() : weapon.LDKCJEMDAMC();
      if (!(native > 0f && native < 60f)) { native = -1f; }
    } catch (System.Exception) { native = -1f; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf302889 = user;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa3804d7 = native;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf011053 = user;
    return user;
  }

  private static void UpdateAimKill() {
    ReleaseAimKillAmmo();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx17e9bf3 = 0f;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf49526f = false;
    try { SampleAimKillInput(); }
    catch (System.Exception error) {
      ClearAimKillManualTarget();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx5569989 = "input-error";
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx1ddcddd = error.Message;
      return;
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx2487c50) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "shot cleanup failed; restart game before retry";
      return;
    }
    if (NativeRecovery()) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx89c3772 = true;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 8;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 8;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96 = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "paused by recovery switch";
      return;
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zx89c3772) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx89c3772 = false;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "";
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = "";
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxda4736a = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx577f981 = 0f;
      ClearAimKillProbes();
    }
    UpdateAimKillLegacy();
    try {
      ApplyAimKillManualAim();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx1ddcddd = "";
    } catch (System.Exception error) {
      ClearAimKillManualTarget();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx1ddcddd = error.Message;
    }
  }

  private static void UpdateAimKillLegacy() {
    if (PlayerPrefs.GetString("u3d.q.6cce555f", "") != "X2") {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx9792f89 = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96 = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx2753b33 = "";
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 0; return;
    }
    ResolveAimMode();
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx577f981) { return; }
    // Install the cadence before native calls; an exception must not retry per frame.
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx577f981 = now + 0.05f;
    try {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 1;
      global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      if (match == null || local == null || !local.IsLocalPlayer() || local.get_CurHP() <= 0 || local.get_IsDieing()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 2;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd265c96 = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f = null;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx801408c = match == null ? "no-match" :
            (local == null ? "no-local" : (!local.IsLocalPlayer() ? "observer" :
            (local.get_CurHP() <= 0 ? "local-hp-zero" : "local-knocked")));
        ClearAimKillProbes();
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx801408c = "ready";
      bool manual = global::COW.GamePlay.SceneEditBoxSelectTool.Zxbd7113e;
      if (manual && !AimKillManualEnabled()) {
        ClearAimKillManualTarget();
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx547c35e++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 7;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 7;
        return;
      }
      if (!manual && (now < global::COW.GamePlay.SceneEditBoxSelectTool.Zx88dcc5a ||
          !AimKillAutoEnabled())) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 11;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 11;
        return;
      }
      if (local.IsReloading()) {
        ClearAimKillManualTarget();
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx16fc72e++;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 3;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 6;
        return;
      }
      Camera camera = Camera.get_main();
      Transform root = local.get_RootTransform();
      Transform head = local.GetHeadTF();
      if (camera == null || root == null || head == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 2;
        return;
      }
      global::COW.GamePlay.HBIBDMMOOOK weapon = local.GetWeaponOnHand();
      if (weapon == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 2;
        return;
      }
      float range = weapon.OEOBPHDFJMO();
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxf4ca292 != weapon) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf4ca292 = weapon;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx3c16b4d = 0f;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbac7a40 = 0f;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f = null;
      }
      float hitDelay = AimKillWeaponDelay(weapon);
      float lastHit = global::COW.GamePlay.SceneEditBoxSelectTool.Zx3c16b4d;
      float due = global::COW.GamePlay.SceneEditBoxSelectTool.Zxbac7a40;
      if (lastHit > 0f && due < lastHit + hitDelay) { due = lastHit + hitDelay; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx3d7d0cc = due;
      if (!manual && now < due) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 10;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 10;
        // Wake on the first eligible frame instead of adding another 50ms
        // poll quantum to the selected deadline. Never submit a catch-up burst.
        if (due < global::COW.GamePlay.SceneEditBoxSelectTool.Zx577f981) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zx577f981 = due;
        }
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx240eed4 = range;
      if (!(range > 0f && range < 10000f)) { return; }
      float configured = AimKillRange();
      if (range > configured) { range = configured; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx73b7a69 = range;
      Vector3 origin = head.get_position();
      if (!ValidVector(origin)) { return; }
      int mode = AimKillPointMode();
      // AimKill always requires a clear line of sight, regardless of the aimbot toggle.
      bool visible = true;
      bool fovOn = AimKillFovLimited();
      float fovRadius = AimKillFov();
      IList players = match.ILPNGCMEFFI();
      if (players == null) { return; }
      // Scan the full candidate list and run one validated sequence for the
      // nearest enemy in any direction (no crosshair/FOV restriction).
      global::COW.GamePlay.Player best = null;
      bool lockUsed = false;
      global::COW.GamePlay.Player pendingLock = global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f;
      if (pendingLock != null && now > global::COW.GamePlay.SceneEditBoxSelectTool.Zx316270d) {
        pendingLock = null;
      }
      if (pendingLock != null) {
        bool lockOk = pendingLock != local && !pendingLock.IsLocalPlayer() &&
            !pendingLock.IsLocalTeammate(false) && !pendingLock.IsLocalTeammate(true) &&
            !pendingLock.get_IsDieing() && pendingLock.get_CurHP() > 0;
        Transform lockRoot = null;
        if (lockOk) {
          GameObject lockOwner = pendingLock.get_gameObject();
          lockRoot = pendingLock.get_RootTransform();
          lockOk = lockOwner != null && lockOwner.get_activeInHierarchy() && lockRoot != null;
        }
        if (lockOk) {
          Vector3 lockPoint = SilentPoint(pendingLock, mode);
          Collider lockCollider = SilentCollider(pendingLock, mode, lockPoint);
          Vector3 lockHit = SilentHitPoint(lockCollider, lockPoint);
          lockOk = ValidVector(lockHit);
          if (lockOk) {
            float lockDistance = Vector3.Distance(origin, lockHit);
            lockOk = lockDistance > 0.001f && lockDistance <= range;
            if (lockOk && fovOn) { lockOk = InsideScreenFov(camera, lockHit, fovRadius); }
            if (lockOk) { lockOk = ClearAimKillSight(origin, lockHit, lockRoot, root); }
            if (lockOk) { best = pendingLock; lockUsed = true; global::COW.GamePlay.SceneEditBoxSelectTool.Zx316270d = now + 3f; }
          }
        }
        if (!lockUsed) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f = null; }
      }
      float closest = range + 1f;
      int bestSlot = 0;
      float scanStarted = Time.get_realtimeSinceStartup();
      int count = players.Count;
      int cursor = global::COW.GamePlay.SceneEditBoxSelectTool.Zx99b3be4;
      if (cursor < 0 || cursor >= count) { cursor = 0; }
      int budget = count;
      if (budget > 12) { budget = 12; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 2;
      for (int scan = 0; scan < budget && !lockUsed; scan++) {
        if (scan > 0 && Time.get_realtimeSinceStartup() - scanStarted > 0.0025f) { break; }
        int i = cursor;
        cursor++;
        if (cursor >= count) { cursor = 0; }
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx99b3be4 = cursor;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxc93449b++;
        try {
          global::COW.GamePlay.Player enemy = players[i] as global::COW.GamePlay.Player;
          if (enemy == null || enemy == local || enemy.IsLocalPlayer() || enemy.IsLocalTeammate(false) ||
              enemy.IsLocalTeammate(true) || enemy.get_IsDieing() || enemy.get_CurHP() <= 0) { continue; }
          GameObject owner = enemy.get_gameObject();
          Transform enemyRoot = enemy.get_RootTransform();
          if (owner == null || !owner.get_activeInHierarchy() || enemyRoot == null) { continue; }
          Vector3 point = SilentPoint(enemy, mode);
          if (!ValidVector(point)) { continue; }
          // Test line of sight against the same collider centre that will be
          // submitted as the hit point, not the bone position (the bone can sit
          // behind an edge while the visible collider is exposed).
          Collider sightCollider = SilentCollider(enemy, mode, point);
          Vector3 sightPoint = SilentHitPoint(sightCollider, point);
          if (!ValidVector(sightPoint)) { continue; }
          float distance = Vector3.Distance(origin, sightPoint);
          if (!(distance > 0.001f && distance <= range)) { continue; }
          if (fovOn && !InsideScreenFov(camera, sightPoint, fovRadius)) { continue; }
           if (visible && !ClearAimKillSight(origin, sightPoint, enemyRoot, root)) {
             global::COW.GamePlay.SceneEditBoxSelectTool.Zx44c0115++;
             continue;
           }
          if (!(distance < closest)) { continue; }
          best = enemy; closest = distance; bestSlot = i & 63;
        } catch (System.Exception) { }
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx31e5d10 =
          (Time.get_realtimeSinceStartup() - scanStarted) * 1000f;
      if (best == null) {
        ClearAimKillManualTarget();
        if (now - global::COW.GamePlay.SceneEditBoxSelectTool.Zx59631bb > 0.4f) {
          global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 4;
        }
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 3;
      if (!lockUsed) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxd06ff6f = best;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx316270d = now + 3f;
      }
      if (manual) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfd4df51 = best;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx1619c92 = weapon;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx82c9c5c = now;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 7;
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 7;
        return;
      }
      // Reserve before native work; no catch-up loop after missed frames.
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxbac7a40 = now + hitDelay;
      bool submitted = StartAimKillXNEW(local, weapon, best, origin, mode, range, visible, bestSlot);
      if (!submitted && global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa == 5 && hitDelay < 1f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxbac7a40 = now + 1f;
      }
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxf0498e2 = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx21571fa = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6f3f293 = error.Message;
      PlayerPrefs.SetString("u3d.q.c8782668", error.ToString());
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx577f981 = now + 1f;
    }
  }

  private static void ReleaseAimKillAmmo() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zxe240989) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxe240989 = false;
    global::COW.GamePlay.PlayerAttributes owner =
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxfd35f49;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxfd35f49 = null;
    if (owner == null) { return; }
    try { owner.IsAmmoFree = global::COW.GamePlay.SceneEditBoxSelectTool.Zx54c4855; }
    catch (System.Exception) { }
  }

  private static void ReleaseAimKillFire() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f = false;
    try {
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      if (local == null) { return; }
      global::COW.GamePlay.HBIBDMMOOOK weapon = local.GetWeaponOnHand();
      if (weapon != null) { local.StopFire(weapon); }
    } catch (System.Exception) { }
  }

  private static string AimPointLabel(int point) {
    if (point == 1) { return "Điểm ngắm: Cổ"; }
    if (point == 2) { return "Điểm ngắm: Vai"; }
    if (point == 3) { return "Điểm ngắm: Ngực trên"; }
    if (point == 4) { return "Điểm ngắm: Ngực"; }
    if (point == 5) { return "Điểm ngắm: Thân"; }
    return "Điểm ngắm: Đầu";
  }

  private static Vector3 AimPoint(global::COW.GamePlay.Player player, int point) {
    Transform head = player.GetHeadTF();
    if (point == 0) {
      // BS4484 ProcessAimbot: head transform + GetRotationToLocation's y_bias.
      if (head != null) { return head.get_position() + new Vector3(0f, 0.1f, 0f); }
      Collider headCollider = player.get_HeadCollider();
      if (headCollider != null) {
        Vector3 center = headCollider.get_bounds().get_center();
        if (ValidVector(center)) { return center; }
      }
      // An invalid head must not silently become a neck/body target.
      return new Vector3(10000000f, 10000000f, 10000000f);
    }
    if (point == 1) {
      Transform neck = player.get_NeckBone();
      if (neck != null) { return neck.get_position(); }
    }
    if (point == 2) {
      Transform shoulder = player.get_ShoulderBoneTransform();
      if (shoulder != null) { return shoulder.get_position(); }
    }
    if (point == 3) {
      Transform spineU = player.get_SpineBoneTransform();
      if (spineU != null) { return spineU.get_position() + new Vector3(0f, 0.20f, 0f); }
    }
    if (point == 4) {
      Transform spine = player.get_SpineBoneTransform();
      if (spine != null) { return spine.get_position(); }
    }
    if (point == 5) {
      Transform root = player.get_RootTransform();
      if (root != null) { return root.get_position() + new Vector3(0f, 1.0f, 0f); }
    }
    // Missing selected anatomy must not silently redirect aim to another part.
    return new Vector3(10000000f, 10000000f, 10000000f);
  }

  private static bool ValidVector(Vector3 value) {
    // Comparisons reject NaN and infinities without introducing new System APIs.
    return value.x > -1000000f && value.x < 1000000f &&
           value.y > -1000000f && value.y < 1000000f &&
           value.z > -1000000f && value.z < 1000000f;
  }

  private static Vector3 SilentPoint(global::COW.GamePlay.Player player, int mode) {
    // Silent uses the selected bone itself, without the camera-only head bias.
    if (mode != 0) { return AimPoint(player, mode); }
    Transform head = player.GetHeadTF();
    if (head != null) { return head.get_position(); }
    return new Vector3(10000000f, 10000000f, 10000000f);
  }

  private static Collider SilentCollider(global::COW.GamePlay.Player player, int mode, Vector3 point) {
    Collider head = player.get_HeadCollider();
    if (mode == 0 && head != null) { return head; }
    Transform root = player.get_RootTransform();
    if (root == null) { return null; }
    IList colliders = player.get_FireColliders();
    if (colliders == null) { return head; }
    Collider best = null;
    float closest = mode == 0 ? 0.75f : 1.25f;
    int count = colliders.Count;
    if (count > 64) { count = 64; }
    for (int i = 0; i < count; i++) {
      Collider collider = colliders[i] as Collider;
      if (collider == null || collider == head) { continue; }
      if (collider.get_isTrigger()) { continue; }
      Transform transform = collider.get_transform();
      GameObject owner = collider.get_gameObject();
      if (transform == null || owner == null || !owner.get_activeInHierarchy() ||
          !(transform == root || transform.IsChildOf(root))) { continue; }
      Vector3 center = collider.get_bounds().get_center();
      if (!ValidVector(center)) { continue; }
      float distance = Vector3.Distance(center, point);
      if (distance < closest) { closest = distance; best = collider; }
    }
    if (best == null && mode == 0) { return head; }
    return best;
  }

  // The hit point must be inside the actual collider. Bone transforms can sit
  // outside the hit capsule, which makes silent shots miss even when the
  // collider/group is correct.
  private static Vector3 SilentHitPoint(Collider collider, Vector3 fallback) {
    if (collider == null) { return fallback; }
    Vector3 center = collider.get_bounds().get_center();
    if (ValidVector(center)) { return center; }
    return fallback;
  }

  private static bool InsideScreenFov(Camera cam, Vector3 world, float radius) {
    if (cam == null || !ValidVector(world)) { return false; }
    Vector3 screen = cam.WorldToScreenPoint(world);
    if (!(screen.z > 0.1f)) { return false; }
    float dx = screen.x - (float)Screen.get_width() * 0.5f;
    float dy = screen.y - (float)Screen.get_height() * 0.5f;
    return dx * dx + dy * dy <= radius * radius;
  }

  private static bool ClearSight(Vector3 origin, Vector3 point, Transform targetRoot,
      Transform localRoot) {
    if (!ValidVector(origin) || !ValidVector(point)) { return false; }
    // A third-person camera may first hit the local player's own body/weapon.
    // Advance only past confirmed local hits, then continue testing for walls.
    for (int pass = 0; pass < 8; pass++) {
      float distance = Vector3.Distance(origin, point);
      if (!(distance > 0.001f)) { return false; }
      Vector3 direction = Vector3.Normalize(point - origin);
      RaycastHit hit;
      if (!Physics.Raycast(origin, direction, out hit,
                           distance, -1, QueryTriggerInteraction.Ignore)) { return true; }
      Collider collider = hit.get_collider();
      if (collider == null) { return false; }
      Transform transform = collider.get_transform();
      if (transform == null) { return false; }
      if (transform == targetRoot || transform.IsChildOf(targetRoot)) { return true; }
      bool selfHit = localRoot != null && (transform == localRoot || transform.IsChildOf(localRoot));
      if (!selfHit) {
        // Own weapon/arms/camera can sit outside localRoot and block the ray at
        // close range; ignore only very close hits, not real cover.
        if (hit.get_distance() >= 0.9f) { return false; }
        selfHit = true;
      }
      float advance = hit.get_distance() + 0.02f;
      if (!(advance > 0f && advance < distance)) { return false; }
      origin = origin + new Vector3(direction.x * advance, direction.y * advance, direction.z * advance);
    }
    return false;
  }

  private static bool ClearAimKillSight(Vector3 origin, Vector3 point, Transform targetRoot,
      Transform localRoot) {
    if (!ValidVector(origin) || !ValidVector(point)) { return false; }
    for (int pass = 0; pass < 4; pass++) {
      float distance = Vector3.Distance(origin, point);
      if (!(distance > 0.001f)) { return false; }
      Vector3 direction = Vector3.Normalize(point - origin);
      RaycastHit hit;
      if (!Physics.Raycast(origin, direction, out hit, distance, -1, QueryTriggerInteraction.Ignore)) {
        return true;
      }
      Collider collider = hit.get_collider();
      if (collider == null) { return false; }
      Transform transform = collider.get_transform();
      if (transform == null) { return false; }
      if (transform == targetRoot || transform.IsChildOf(targetRoot)) { return true; }
      if (localRoot == null || (transform != localRoot && !transform.IsChildOf(localRoot))) { return false; }
      float advance = hit.get_distance() + 0.02f;
      if (!(advance > 0f && advance < distance)) { return false; }
      origin = origin + new Vector3(direction.x * advance, direction.y * advance, direction.z * advance);
    }
    return false;
  }

  private static float AimScore(global::COW.GamePlay.Player candidate,
      global::COW.GamePlay.Player local, Camera cam, Vector3 origin, Vector3 sightOrigin,
      int w, int h, int point, float radius, float range, int priority,
      bool skipKnocked, bool visible) {
    if (candidate == null || candidate == local || candidate.IsLocalPlayer()) { return -1f; }
    if (candidate.IsLocalTeammate(false) || candidate.IsLocalTeammate(true)) { return -1f; }
    GameObject go = candidate.get_gameObject();
    if (go == null || !go.get_activeInHierarchy() || candidate.get_CurHP() <= 0) { return -1f; }
    if (skipKnocked && candidate.get_IsDieing()) { return -1f; }
    Transform root = candidate.get_RootTransform();
    if (root == null) { return -1f; }
    Vector3 target = AimPoint(candidate, point);
    if (PlayerPrefs.GetString("u3d.q.30dc3b5a", "") == "X2") { target = SilentPoint(candidate, point); }
    if (!ValidVector(target)) { return -3f; }
    float distance = Vector3.Distance(origin, target);
    if (!(distance > 0.001f && distance <= range)) { return -1f; }
    Vector3 screen = cam.WorldToScreenPoint(target);
    if (!(screen.z > 0.1f && screen.x >= 0f && screen.x <= (float)w &&
          screen.y >= 0f && screen.y <= (float)h)) { return -1f; }
    float dx = screen.x - (float)w * 0.5f;
    float dy = screen.y - (float)h * 0.5f;
    float screenDistance = dx * dx + dy * dy;
    if (!(screenDistance <= radius * radius)) { return -1f; }
    if (visible && !ClearSight(sightOrigin, target, root, local.get_RootTransform())) { return -2f; }
    if (priority == 1) { return distance; }
    return screenDistance;
  }

  private static float AimScoreAny(global::COW.GamePlay.Player candidate,
      global::COW.GamePlay.Player local, Camera cam, Vector3 origin, Vector3 sightOrigin,
      int point, float range, bool skipKnocked, bool visible) {
    if (candidate == null || candidate == local || candidate.IsLocalPlayer()) { return -1f; }
    if (candidate.IsLocalTeammate(false) || candidate.IsLocalTeammate(true)) { return -1f; }
    GameObject go = candidate.get_gameObject();
    if (go == null || !go.get_activeInHierarchy() || candidate.get_CurHP() <= 0) { return -1f; }
    if (skipKnocked && candidate.get_IsDieing()) { return -1f; }
    Transform root = candidate.get_RootTransform();
    if (root == null) { return -1f; }
    Vector3 target = AimPoint(candidate, point);
    if (!ValidVector(target)) { return -3f; }
    float distance = Vector3.Distance(origin, target);
    if (!(distance > 0.001f && distance <= range)) { return -1f; }
    if (visible && !ClearSight(sightOrigin, target, root, local.get_RootTransform())) { return -2f; }
    return distance;
  }

  private static void UpdateAimbot(int w, int h) {
    if (Event.get_current().get_type() != 7) { return; }
    int frame = Time.get_frameCount();
    if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxb44a5e1 == frame + 1) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxb44a5e1 = frame + 1;
    RefreshAimTarget(w, h);
    global::COW.GamePlay.Player best = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa4ac39a;
    if (best == null || PlayerPrefs.GetString("u3d.q.30dc3b5a", "") == "X2") { return; }
    global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.Zx6472cf9;
    Camera cam = Camera.get_main();
    if (local == null || cam == null) { return; }
    Transform aimCamera = local.MainCameraTransform;
    if (aimCamera == null) { aimCamera = cam.get_transform(); }
    if (aimCamera == null) { return; }
    Vector3 origin = aimCamera.get_position();
    int point = ReadSetting("u3d.q.9a2750c8", 0, 0, 5);
    Vector3 selectedPoint = AimPoint(best, point);
    if (!ValidVector(selectedPoint) || !ValidVector(origin)) { return; }
    Vector3 direction = selectedPoint - origin;
    Quaternion rotation = Quaternion.LookRotation(direction);
    float smooth = ReadFloatSetting("u3d.q.765ef444", 6, 0, 20);
    Quaternion current = local.GetAimRotation();
    float error = Quaternion.Angle(current, rotation);
    if (smooth > 0) {
      float dt = Time.get_unscaledDeltaTime();
      if (!(dt > 0f)) {
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 8;
        return;
      }
      if (dt > 0.1f) { dt = 0.1f; }
      float alpha = 1f - Mathf.Exp(-dt * 60f / smooth);
      if (error > 0.05f) { rotation = Quaternion.Slerp(current, rotation, alpha); }
    }
    local.SetAimRotation(rotation, false);
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 6;
    if (smooth == 0 || error <= 0.05f) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 7; }
  }

  // Called synchronously by both HUD and the getter; never reads the getter itself.
  private static void RefreshAimTarget(int w, int h) {
    global::COW.GamePlay.Player previous = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa4ac39a;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa4ac39a = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 0;
    bool silent = ResolveAimMode();
    if (NativeRecovery() || PlayerPrefs.GetString("u3d.q.6cce555f", "") == "X2") { return; }
    bool anyAngle = !silent && PlayerPrefs.GetString("u3d.q.e96f559b", "") == "X2";
    if (!silent && PlayerPrefs.GetString("u3d.q.69e4f3ba", "") != "X2") {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxb414436 = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx6472cf9 = null;
      return;
    }
    global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
    global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
    if (match != global::COW.GamePlay.SceneEditBoxSelectTool.Zxb414436 ||
        local != global::COW.GamePlay.SceneEditBoxSelectTool.Zx6472cf9) { previous = null; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxb414436 = match;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx6472cf9 = local;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 1;
    if (match == null || local == null || local.get_CurHP() <= 0 || local.get_IsDieing()) { return; }
    GameObject localObject = local.get_gameObject();
    if (localObject == null || !localObject.get_activeInHierarchy()) { return; }
    bool scope = PlayerPrefs.GetString("u3d.q.b8882618", "X2") == "X2";
    bool fire = PlayerPrefs.GetString("u3d.q.6a3d89fd", "") == "X2";
    bool lockAlways = PlayerPrefs.GetString("u3d.q.ecc936c4", "") == "X2";
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 2;
    // Scope/fire activation config belongs to camera aim. Silent activates when firing.
    if (!silent && !lockAlways && !anyAngle && (scope || fire) &&
        !((scope && local.get_IsSighting()) || (fire && local.IsFiring()))) { return; }
    if (anyAngle && !lockAlways && !local.IsFiring()) { return; }
    Camera cam = Camera.get_main();
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 3;
    if (cam == null || cam.get_transform() == null) { return; }
    IList players = match.ILPNGCMEFFI();
    if (players == null) { return; }
    Transform aimCamera = local.MainCameraTransform;
    if (aimCamera == null) { aimCamera = cam.get_transform(); }
    // Use the view origin, not the offset weapon/shoulder origin (parallax).
    Vector3 origin = aimCamera.get_position();
    Vector3 sightOrigin = origin;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 8;
    if (!ValidVector(origin) || !ValidVector(sightOrigin)) { return; }
    int point = ReadSetting("u3d.q.9a2750c8", 0, 0, 5);
    float radius = ReadFloatSetting("u3d.q.cefa8b80", 220, 20, 800);
    float range = ReadFloatSetting("u3d.q.19ea4352", 150, 10, 500);
    int priority = ReadSetting("u3d.q.01a1923c", 0, 0, 1);
    bool skipKnocked = PlayerPrefs.GetString("u3d.q.fc8f1a9d", "X2") == "X2";
    bool visible = PlayerPrefs.GetString("u3d.q.82736dca", "X2") == "X2";
    bool sticky = PlayerPrefs.GetString("u3d.q.e7c620f6", "X2") == "X2";
    global::COW.GamePlay.Player best = null;
    float bestScore = 1000000000f;
    bool occluded = false;
    bool missingBone = false;
    int n = players.Count;
    if (n > 128) { n = 128; }
    for (int i = 0; i < n; i++) {
      global::COW.GamePlay.Player candidate = players[i] as global::COW.GamePlay.Player;
      float score = anyAngle
          ? AimScoreAny(candidate, local, cam, origin, sightOrigin,
                        point, range, skipKnocked, visible)
          : AimScore(candidate, local, cam, origin, sightOrigin, w, h,
                     point, radius, range, priority, skipKnocked, visible);
      if (score == -2f) { occluded = true; }
      if (score == -3f) { missingBone = true; }
      if (score < 0f) { continue; }
      if (sticky && candidate == previous) { best = candidate; break; }
      if (score < bestScore) { bestScore = score; best = candidate; }
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 4;
    if (best == null) {
      if (missingBone) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 9; }
      if (occluded) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 5; }
      return;
    }
    Vector3 selectedPoint = AimPoint(best, point);
    if (silent) { selectedPoint = SilentPoint(best, point); }
    if (!ValidVector(selectedPoint)) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 9;
      return;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa4ac39a = best;
    if (silent) { global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7 = 10; }
  }

  private static bool SameVector(Vector3 a, Vector3 b) {
    return a.x == b.x && a.y == b.y && a.z == b.z;
  }

  private static void RestoreSilent(global::COW.GamePlay.CGKJLKPMGDJ hit) {
    // Never restore a different/shared object or overwrite game updates.
    if (hit != null && hit == global::COW.GamePlay.SceneEditBoxSelectTool.Zx2a72a6f &&
        hit.JHAINOBIFMA == global::COW.GamePlay.SceneEditBoxSelectTool.Zxfa9a523 &&
        hit.JINDNOFABKB == global::COW.GamePlay.SceneEditBoxSelectTool.Zx4882118 &&
        SameVector(hit.HPCOJHGFEEG, global::COW.GamePlay.SceneEditBoxSelectTool.Zxcfd54c0) &&
        SameVector(hit.DOJDNHCGFGK, new Vector3(0f, 1f, 0f)) &&
        SameVector(hit.HLDECMJFKJK, global::COW.GamePlay.SceneEditBoxSelectTool.Zx0126c95) &&
        hit.DKPDBLLJEAA == global::COW.GamePlay.SceneEditBoxSelectTool.Zx39781aa &&
        hit.EFMBNPNFGHM == global::COW.GamePlay.SceneEditBoxSelectTool.Zx5d825f6 && !hit.CEOKOILBODN) {
      hit.JHAINOBIFMA = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa463246;
      hit.JINDNOFABKB = global::COW.GamePlay.SceneEditBoxSelectTool.Zx167cbcd;
      hit.HPCOJHGFEEG = global::COW.GamePlay.SceneEditBoxSelectTool.Zx9d4a2d3;
      hit.DOJDNHCGFGK = global::COW.GamePlay.SceneEditBoxSelectTool.Zx8176230;
      hit.HLDECMJFKJK = global::COW.GamePlay.SceneEditBoxSelectTool.Zx6674dbe;
      hit.DKPDBLLJEAA = global::COW.GamePlay.SceneEditBoxSelectTool.Zx4ef8a62;
      hit.EFMBNPNFGHM = global::COW.GamePlay.SceneEditBoxSelectTool.Zxf3457a1;
      hit.CEOKOILBODN = global::COW.GamePlay.SceneEditBoxSelectTool.Zxc0a4b9d;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx2a72a6f = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxfa9a523 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx4882118 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa463246 = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx167cbcd = null;
  }

  private static void ObserveManualHit(global::COW.GamePlay.Player player,
      global::COW.GamePlay.CGKJLKPMGDJ hit) {
    // Only read a naturally requested native hit during real firing. Do not
    // invoke a fire method, alter this object, or retain game object references.
    if (hit == null || global::COW.GamePlay.SceneEditBoxSelectTool.Zx558a20f ||
        NativeRecovery() || PlayerPrefs.GetString("u3d.q.6cce555f", "") != "X2") { return; }
    try {
      if (!player.IsLocalPlayer() || !player.IsFiring()) { return; }
      float now = Time.get_realtimeSinceStartup();
      if (global::COW.GamePlay.SceneEditBoxSelectTool.Zxe317b04 > 0 &&
          now - global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7dbda9 < 0.25f) { return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa7dbda9 = now;
      string snapshot = "damage=" + hit.OLGEMBBDBID.ToString()
          + " distance=" + hit.CFIPDNFBKHK.ToString() + " layer=" + hit.GABPHAKNADH.ToString()
          + " group=" + ((int)hit.DKPDBLLJEAA).ToString()
          + " blocked=" + (hit.CEOKOILBODN ? "1" : "0")
          + " ignore=" + (hit.EFMBNPNFGHM ? "1" : "0");
      snapshot = snapshot + " collider=" + (hit.JINDNOFABKB == null ? "0" : hit.JINDNOFABKB.GetInstanceID().ToString())
          + " special=" + hit.CLOLKHNMNEK.ToString()
          + " normalY=" + hit.DOJDNHCGFGK.y.ToString()
          + " origin=" + hit.EKPLMDDKOGB.x.ToString() + "," + hit.EKPLMDDKOGB.y.ToString() + "," + hit.EKPLMDDKOGB.z.ToString()
          + " pos=" + hit.HPCOJHGFEEG.x.ToString() + "," + hit.HPCOJHGFEEG.y.ToString() + "," + hit.HPCOJHGFEEG.z.ToString();
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa5e3ae5 = snapshot;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxe317b04++;
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc2db594 = "";
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxc2db594 = error.Message;
    }
  }

  // Replaces the existing getter fix; no recursive getter call or native pointers.
  public static global::COW.GamePlay.CGKJLKPMGDJ SilentAimingInfo(global::COW.GamePlay.Player player) {
    if (player == null) { return null; }
    global::COW.GamePlay.CGKJLKPMGDJ hit = player.FDMIEDDNCEC;
    ObserveManualHit(player, hit);
    // Disabled mode must be a field-only passthrough, except pending restoration.
    if (PlayerPrefs.GetString("u3d.q.30dc3b5a", "") != "X2" &&
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx2a72a6f == null) { return hit; }
    if (!player.IsLocalPlayer()) { return hit; }
    RestoreSilent(hit);
    if (!ResolveAimMode() || player != global::COW.GameFacade.GetLocalPlayerOrObServer()) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx6bd394c = Time.get_frameCount() + 1;
    int calls = global::COW.GamePlay.SceneEditBoxSelectTool.Zxad02e3d;
    if (calls < 1000000000) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxad02e3d = calls + 1; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = 11;
    if (hit == null) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = 12;
    if (!player.IsFiring()) { return hit; }
    RefreshAimTarget(Screen.get_width(), Screen.get_height());
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = global::COW.GamePlay.SceneEditBoxSelectTool.Zx513f0e7;
    global::COW.GamePlay.Player target = global::COW.GamePlay.SceneEditBoxSelectTool.Zxa4ac39a;
    if (target == null) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = 14;
    int mode = ReadSetting("u3d.q.9a2750c8", 0, 0, 5);
    Vector3 point = SilentPoint(target, mode);
    if (!ValidVector(point)) { return hit; }
    Collider collider = SilentCollider(target, mode, point);
    if (collider == null) { return hit; }
    point = SilentHitPoint(collider, point);
    if (!ValidVector(point)) { return hit; }
    GameObject targetObject = collider.get_gameObject();
    if (targetObject == null || !targetObject.get_activeInHierarchy()) { return hit; }
    Vector3 origin = hit.EKPLMDDKOGB;
    float distance = Vector3.Distance(origin, point);
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = 15;
    if (!ValidVector(origin) || !ValidVector(point) || !(distance > 0.001f) ||
        distance > ReadFloatSetting("u3d.q.19ea4352", 150, 10, 500)) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = 16;
    if (PlayerPrefs.GetString("u3d.q.82736dca", "X2") == "X2" &&
        !ClearSight(origin, point, target.get_RootTransform(), player.get_RootTransform())) { return hit; }
    Vector3 direction = Vector3.Normalize(point - origin);
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = 15;
    if (!ValidVector(direction)) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxa463246 = hit.JHAINOBIFMA;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx167cbcd = hit.JINDNOFABKB;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx9d4a2d3 = hit.HPCOJHGFEEG;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx8176230 = hit.DOJDNHCGFGK;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx6674dbe = hit.HLDECMJFKJK;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx4ef8a62 = hit.DKPDBLLJEAA;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf3457a1 = hit.EFMBNPNFGHM;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxc0a4b9d = hit.CEOKOILBODN;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx2a72a6f = hit;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxfa9a523 = targetObject;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx4882118 = collider;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxcfd54c0 = point;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx0126c95 = direction;
    global::COW.GamePlay.LKOOALMKJND group = global::COW.GamePlay.LKOOALMKJND.Body;
    if (mode == 0) { group = global::COW.GamePlay.LKOOALMKJND.Head; }
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx39781aa = group;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zx5d825f6 = mode == 0;
    // BS4484 hit-object semantics; preserve native origin, distance and damage.
    hit.JHAINOBIFMA = targetObject;
    hit.JINDNOFABKB = collider;
    hit.HPCOJHGFEEG = point;
    hit.DOJDNHCGFGK = new Vector3(0f, 1f, 0f);
    hit.HLDECMJFKJK = direction;
    hit.DKPDBLLJEAA = group;
    hit.EFMBNPNFGHM = mode == 0;
    hit.CEOKOILBODN = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxdc58154 = 17;
    global::COW.GamePlay.SceneEditBoxSelectTool.Zxf5ba39d = Time.get_frameCount() + 1;
    int writes = global::COW.GamePlay.SceneEditBoxSelectTool.Zxe61c424;
    if (writes < 1000000000) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxe61c424 = writes + 1; }
    return hit;
  }

  private static void DrawEsp(int w, int h, Texture2D tex, bool espBox,
      bool espName, bool espHp, bool espDist, bool espLine, bool espFov,
      bool esp3d, bool espFill, bool espArrow, bool espLink,
      bool espTeam, bool espBlue, bool espGrenade) {
    // No player reads or drawing on layout/input events.
    bool updateEffects = Event.get_current().get_type() == 7;
    if (!updateEffects) { return; }
    int frame = Time.get_frameCount();
    bool fireFrame = updateEffects &&
        global::COW.GamePlay.SceneEditBoxSelectTool.Zxa606551 != frame + 1;
    if (updateEffects) {
      global::COW.GamePlay.SceneEditBoxSelectTool.Zxa606551 = frame + 1;
      DisableLines(global::COW.GamePlay.SceneEditBoxSelectTool.Zx5992bd4);
      DisableLines(global::COW.GamePlay.SceneEditBoxSelectTool.Zxc8b6421);
    }
    if (espFov) {
      GUI.set_color(new Color(1f, 0.85f, 0.1f, 0.6f));
      float centerX = (float)w * 0.5f;
      float centerY = (float)h * 0.5f;
      float radius = ReadFloatSetting("u3d.q.cefa8b80", 220, 20, 800);
      float firstX = centerX + radius;
      float firstY = centerY;
      float prevX = firstX;
      float prevY = firstY;
      for (int k = 1; k <= 36; k++) {
        float a = (float)k * 0.174532925f;
        float nextX = centerX + Mathf.Sin(a + 1.570796327f) * radius;
        float nextY = centerY + Mathf.Sin(a) * radius;
        // Close at exactly the first endpoint, avoiding a floating-point seam.
        if (k == 36) { nextX = firstX; nextY = firstY; }
        DrawLine(prevX, prevY, nextX, nextY, 1.2f, tex);
        prevX = nextX;
        prevY = nextY;
      }
    }

    // FOV is screen-space only. Do not touch match/player/camera state for it.
    if (!(espBox || espName || espHp || espDist || espLine || esp3d || espFill ||
          espArrow || espLink || espTeam || espBlue || espGrenade)) { return; }

    global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
    Camera cam = Camera.get_main();
    if (match == null || cam == null) { return; }
    IList list = match.ILPNGCMEFFI();
    if (list == null) { return; }

    global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
    Transform localRoot = null;
    Transform localHead = null;
    if (local != null) {
      localRoot = local.get_RootTransform();
      localHead = local.GetHeadTF();
    }

    GUIStyle small = CachedStyle(5);
    small.set_fontSize(18);
    small.set_alignment(1); // UpperCenter: text is centered on cx.

    int n = list.Count;
    if (n > 128) { n = 128; }
    for (int i = 0; i < n; i++) {
      global::COW.GamePlay.Player pl = list[i] as global::COW.GamePlay.Player;
      if (pl == null || pl.IsLocalPlayer()) { continue; }
      bool teammate = pl.IsLocalTeammate(false) || pl.IsLocalTeammate(true);
      if (teammate && !espTeam) { continue; }
      GameObject go = pl.get_gameObject();
      if (go != null && go.get_activeInHierarchy() && pl.get_CurHP() > 0) {
        bool knocked = pl.get_IsDieing();
        Transform trf = pl.get_RootTransform();
        Transform trh = pl.GetHeadTF();
        if (teammate) {
          if (trf != null) { DrawTeammate(pl, trf, localRoot, cam, w, h, tex, small); }
          continue;
        }
        if (trf == null || trh == null) { continue; }

        Vector3 feet = trf.get_position();
        Vector3 head = trh.get_position() + new Vector3(0f, 0.2f, 0f);
        Vector3 sf = cam.WorldToScreenPoint(feet);
        Vector3 sh = cam.WorldToScreenPoint(head);
        if (!(sf.z > 0.1f && sh.z > 0.1f)) { continue; }

        float fy = (float)h - sf.y;
        float hy = (float)h - sh.y;
        float bh = (fy - hy) * 1.0909f;
        // Also rejects degenerate and NaN projections without hiding close enemies.
        if (!(bh > 0f)) { continue; }
        float bw = bh * 0.50f;
        float cx = (sf.x + sh.x) * 0.5f;
        float gx = cx - bw * 0.5f;
        float gy = fy - bh;

        Color enemyColor = new Color(1f, 1f, 1f, 0.95f);
        if (knocked) { enemyColor = new Color(1f, 0.15f, 0.15f, 0.95f); }
        if (pl == global::COW.GamePlay.SceneEditBoxSelectTool.Zxa4ac39a &&
            PlayerPrefs.GetString("u3d.q.55a2eae3", "X2") == "X2") {
          enemyColor = new Color(1f, 0.85f, 0.1f, 0.95f);
        }

        if (espFill) {
          GUI.set_color(new Color(0f, 0f, 0f, 0.32f));
          GUI.DrawTexture(new Rect(gx, gy, bw, bh), tex);
        }
        GUI.set_color(enemyColor);
        if (espBox || espFill || esp3d) {
          DrawBox(gx, gy, bw, bh, 1.2f, tex);
        }
        if (espFill) {
          float corner = bw * 0.25f;
          // Four L-shaped corner highlights over the thin outline.
          DrawLine(gx, gy, gx + corner, gy, 2.4f, tex);
          DrawLine(gx, gy, gx, gy + corner, 2.4f, tex);
          DrawLine(gx + bw - corner, gy, gx + bw, gy, 2.4f, tex);
          DrawLine(gx + bw, gy, gx + bw, gy + corner, 2.4f, tex);
          DrawLine(gx, fy - corner, gx, fy, 2.4f, tex);
          DrawLine(gx, fy, gx + corner, fy, 2.4f, tex);
          DrawLine(gx + bw, fy - corner, gx + bw, fy, 2.4f, tex);
          DrawLine(gx + bw - corner, fy, gx + bw, fy, 2.4f, tex);
        }
        if (esp3d) {
          float depth = bw * 0.25f;
          DrawBox(gx - depth, gy - depth, bw, bh, 1.2f, tex);
          DrawLine(gx, gy, gx - depth, gy - depth, 1.2f, tex);
          DrawLine(gx + bw, gy, gx + bw - depth, gy - depth, 1.2f, tex);
          DrawLine(gx, fy, gx - depth, fy - depth, 1.2f, tex);
          DrawLine(gx + bw, fy, gx + bw - depth, fy - depth, 1.2f, tex);
        }
        if (espLine) {
          DrawLine((float)w * 0.5f, 0f, cx, gy, 1.2f, tex);
        }
        if (local != null && updateEffects) {
          if (espLink && fireFrame) {
            global::COW.GamePlay.HBIBDMMOOOK weapon = local.GetActiveWeapon();
            if (weapon != null) {
              weapon.GENFKNDFIJH(local.GetAttackableCenterWS(),
                  pl.GetAttackableCenterWS(), go, (short)0);
            }
          }
          if (espBlue) {
            SetEffectLine(i, false, local.GetAttackableCenterWS(),
                pl.GetAttackableCenterWS(), new Color(0.15f, 0.5f, 1f, 0.95f));
          }
          if (espGrenade && localHead != null) {
            SetEffectLine(i, true, localHead.get_position(), trh.get_position(),
                new Color(1f, 0.15f, 0.15f, 0.95f));
          }
        }
        if (espArrow && !knocked) {
          GUI.set_color(new Color(1f, 0.15f, 0.15f, 0.95f));
          // Filled downward triangle: a wide top tapering to its bottom tip.
          for (int row = 0; row < 12; row++) {
            float halfWidth = 6f * (1f - (float)row / 12f);
            GUI.DrawTexture(new Rect(cx - halfWidth, gy - 16f + (float)row,
                                     halfWidth * 2f, 1f), tex);
          }
          DrawLine(cx - 6f, gy - 16f, cx + 6f, gy - 16f, 1f, tex);
          DrawLine(cx - 6f, gy - 16f, cx, gy - 4f, 1f, tex);
          DrawLine(cx + 6f, gy - 16f, cx, gy - 4f, 1f, tex);
        }
        if (espHp && !knocked) {
          float maxHp = (float)pl.get_MaxHP();
          float hp = 0f;
          if (maxHp > 0f) { hp = (float)pl.get_CurHP() / maxHp; }
          if (hp > 1f) { hp = 1f; }
          if (hp < 0f) { hp = 0f; }
          GUI.set_color(new Color(0f, 0f, 0f, 0.6f));
          GUI.DrawTexture(new Rect(gx - 7f, gy, 3f, bh), tex);
          if (hp > 0.5f) { GUI.set_color(new Color(0.15f, 0.95f, 0.25f, 0.95f)); }
          else if (hp > 0.25f) { GUI.set_color(new Color(0.95f, 0.9f, 0.15f, 0.95f)); }
          else { GUI.set_color(new Color(1f, 0.15f, 0.15f, 0.95f)); }
          if (hp > 0f) {
            GUI.DrawTexture(new Rect(gx - 7f, gy + bh - bh * hp, 3f, bh * hp), tex);
          }
        }

        // GUI.color multiplies style textColor; use white for accurate text colors.
        GUI.set_color(new Color(1f, 1f, 1f, 1f));
        small.get_normal().set_textColor(enemyColor);
        if (espName) {
          string nickname = pl.get_NickName();
          if (nickname == null) { nickname = "Unknown"; }
          if (pl.IsClientBot) { nickname = "[BOT] " + nickname; }
          GUI.Label(new Rect(cx - 150f, gy - 22f, 300f, 22f), nickname, small);
        }
        if (espDist && localRoot != null) {
          float dd = Vector3.Distance(localRoot.get_position(), feet);
          int d_i = (int)dd;
          string txt = d_i.ToString() + " m";
          GUI.Label(new Rect(cx - 100f, fy + 2f, 200f, 22f), txt, small);
        }
      }
    }
  }

  private static void DrawTeammate(global::COW.GamePlay.Player pl, Transform root,
      Transform localRoot, Camera cam, int w, int h, Texture2D tex, GUIStyle style) {
    Vector3 feet = root.get_position();
    Vector3 sf = cam.WorldToScreenPoint(feet);
    Vector3 sh = cam.WorldToScreenPoint(feet + new Vector3(0f, 1.7f, 0f));
    if (!(sf.z > 0.1f && sh.z > 0.1f)) { return; }
    float height = sh.y - sf.y;
    if (height < 0f) { height = -height; }
    height = height * 1.1f;
    if (!(height > 0f)) { return; }
    float width = height * 0.45f;
    float x = sh.x - width * 0.5f;
    float y = (float)h - sh.y;
    Color cyan = new Color(0f, 1f, 1f, 0.95f);
    GUI.set_color(cyan);
    DrawBox(x, y, width, height, 1f, tex);
    DrawLine((float)w * 0.5f, 0f, sh.x, y + height, 0.8f, tex);
    string name = pl.get_NickName();
    if (name == null) { name = "Teammate"; }
    if (pl.IsClientBot) { name = "[BOT] " + name; }
    if (localRoot != null) {
      int distance = (int)Vector3.Distance(localRoot.get_position(), feet);
      name = name + " | " + distance.ToString() + " m";
    }
    GUI.set_color(new Color(0f, 0f, 0f, 0.43f));
    GUI.DrawTexture(new Rect(sh.x - 150f, y - 29f, 300f, 26f), tex);
    GUI.set_color(cyan);
    GUI.DrawTexture(new Rect(sh.x - 150f, y - 29f, 4f, 26f), tex);
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    style.get_normal().set_textColor(cyan);
    GUI.Label(new Rect(sh.x - 144f, y - 26f, 288f, 22f), name, style);
  }

  private static void DisableLines(LineRenderer[] lines) {
    if (lines == null) { return; }
    for (int i = 0; i < lines.Length; i++) {
      LineRenderer line = lines[i];
      if (line != null) { line.set_enabled(false); }
    }
  }

  private static void SetEffectLine(int index, bool grenade, Vector3 from,
      Vector3 to, Color color) {
    LineRenderer[] lines;
    if (grenade) { lines = global::COW.GamePlay.SceneEditBoxSelectTool.Zxc8b6421; }
    else { lines = global::COW.GamePlay.SceneEditBoxSelectTool.Zx5992bd4; }
    if (lines == null) {
      lines = new LineRenderer[128];
      if (grenade) { global::COW.GamePlay.SceneEditBoxSelectTool.Zxc8b6421 = lines; }
      else { global::COW.GamePlay.SceneEditBoxSelectTool.Zx5992bd4 = lines; }
    }
    LineRenderer line = lines[index];
    if (line == null) {
      Material material = global::COW.GamePlay.SceneEditBoxSelectTool.Zx9dd2f81;
      if (material == null) {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        if (shader == null) { return; }
        material = new Material(shader);
        material.SetInt("_SrcBlend", 5);
        material.SetInt("_DstBlend", 10);
        material.SetInt("_Cull", 0);
        material.SetInt("_ZWrite", 0);
        material.SetInt("_ZTest", 8);
        global::COW.GamePlay.SceneEditBoxSelectTool.Zx9dd2f81 = material;
      }
      GameObject owner = new GameObject("UI");
      line = owner.AddComponent(typeof(LineRenderer)) as LineRenderer;
      if (line == null) { return; }
      line.set_sharedMaterial(material);
      line.set_useWorldSpace(true);
      line.set_positionCount(2);
      line.set_startWidth(0.02f);
      line.set_endWidth(0.02f);
      lines[index] = line;
    }
    line.set_startColor(color);
    line.set_endColor(color);
    line.SetPosition(0, from);
    line.SetPosition(1, to);
    line.set_enabled(true);
  }

  private static void DrawBox(float x, float y, float width, float height,
      float thickness, Texture2D tex) {
    DrawLine(x, y, x + width, y, thickness, tex);
    DrawLine(x + width, y, x + width, y + height, thickness, tex);
    DrawLine(x + width, y + height, x, y + height, thickness, tex);
    DrawLine(x, y + height, x, y, thickness, tex);
  }

  private static void DrawLine(float x1, float y1, float x2, float y2,
      float thickness, Texture2D tex) {
    float dx = x2 - x1;
    float dy = y2 - y1;
    float length = Mathf.Sqrt(dx * dx + dy * dy);
    if (!(length > 0f)) { return; }
    Matrix4x4 saved = GUI.get_matrix();
    GUI.set_matrix(Matrix4x4.TRS(new Vector3(x1, y1, 0f),
        Quaternion.Euler(0f, 0f, Mathf.Atan2(dy, dx) * 57.29578f),
        new Vector3(1f, 1f, 1f)));
    GUI.DrawTexture(new Rect(0f, -thickness * 0.5f, length, thickness), tex);
    GUI.set_matrix(saved);
  }
}
