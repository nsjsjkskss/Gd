// OKMOD - player ESP and configurable aim assist for the IFix HUD.
using UnityEngine;
using System.Collections;

public class Okmod {
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
    if (PlayerPrefs.GetInt("OKMOD.Diagnostic.Cleanup", 0) == 7) { return; }
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
    try { PlayerPrefs.SetInt("OKMOD.Diagnostic.Cleanup", 7); } catch (System.Exception) { }
  }

  private static void RecordHudError(System.Exception error) {
    int failures = PlayerPrefs.GetInt("OKMOD.Diagnostic.HudFaults", 0);
    if (failures < 1000000) { PlayerPrefs.SetInt("OKMOD.Diagnostic.HudFaults", failures + 1); }
    PlayerPrefs.SetString("OKMOD.Diagnostic.HudLastError", error.Message);
  }

  private static void UpdateFeaturesOnce() {
    Event ev = Event.get_current();
    if (ev == null || ev.get_type() != 7) { return; }
    int frame = Time.get_frameCount();
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7FeatureFrame == frame + 1) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7FeatureFrame = frame + 1;
    UpdateAimbot(Screen.get_width(), Screen.get_height());
    UpdateAimKill();
    UpdateCombat();
    UpdateJumpBoost();
    UpdateAimKillProbes();
    NativeRecovery();
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest != null &&
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest.get_isDone()) {
      string sendError = "";
      try { sendError = global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest.get_error(); }
      catch (System.Exception) { }
      try { global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest.Dispose(); }
      catch (System.Exception) { }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSending = false;
      if (sendError != null && sendError.Length > 0) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSendOk = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = "send: " + sendError;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSendOk = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = "";
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSentAt = Time.get_realtimeSinceStartup();
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpReq != null &&
        global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpReq.get_isDone()) {
      try { global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpReq.Dispose(); }
      catch (System.Exception) { }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpReq = null;
    }
  }

  private static void DrawEspLayer(int w, int h, Texture2D tex) {
    SampledEsp.DrawEsp(w, h, tex,
        PlayerPrefs.GetString("OKMOD.Esp.Enabled", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.PlayerName", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.Hp", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.Distance", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.AimLine", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.FovRing", "") == "X2",
        false,
        PlayerPrefs.GetString("OKMOD.Esp.Box", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.HeadMark", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.Link", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.Team", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.BlueLines", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.HeadCircle", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.Skeleton", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.Info", "") == "X2",
        PlayerPrefs.GetString("OKMOD.Esp.ArmorFlags", "") == "X2");
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
    bool open = PlayerPrefs.GetString("OKMOD.Ui.Open", "") != "X2";
    float mx = global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX;
    float my = global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY;
    int px = uiW / 2 - 400 + (int)mx;
    int py = uiH / 2 - 340 + (int)my;
    int et = -1;
    if (ReadMenuPointer(open, px, py, uiH, h, menuScale)) { et = 1; }
    Vector2 mp = Event.get_current().get_mousePosition();
    mp.x = global::COW.GamePlay.SceneEditBoxSelectTool.OkUiX;
    mp.y = global::COW.GamePlay.SceneEditBoxSelectTool.OkUiY;
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
        PlayerPrefs.SetString("OKMOD.Ui.Open", "X2");
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured = false;
        open = false;
      }
    } else {
      if (new Rect(10, uiH / 2 - 60, 116, 116).Contains(mp) && et == 1) {
        PlayerPrefs.SetString("OKMOD.Ui.Open", "");
        open = true;
      }
    }

    // ================= cong tac =================
    bool aimPage = PlayerPrefs.GetString("OKMOD.Page.AIM", "") == "X2";
    if (open && et == 1 && new Rect(px + 440, py + 6, 250, 54).Contains(mp)) {
      aimPage = !aimPage;
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll = 0f;
      if (aimPage) { PlayerPrefs.SetString("OKMOD.Page.AIM", "X2"); }
      else { PlayerPrefs.SetString("OKMOD.Page.AIM", ""); }
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
      GUI.Label(new Rect(22, uiH / 2 - 22, 100, 26), "OKMOD R12.30 DEV", bs);
    }

    if (open && global::COW.GamePlay.SceneEditBoxSelectTool.R7LogOpen) {
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
      bool copyRecent = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyAt > 0f &&
          Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyAt < 2f;
      bool copied = copyRecent && global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk;
      bool copyFailed = copyRecent && !global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk;
      GUI.set_color(new Color(0.12f, 0.36f, 0.2f, 0.95f));
      GUI.DrawTexture(copyRect, tex);
      GUI.set_color(new Color(1f, 1f, 1f, 1f));
      GUI.Label(copyRect, copied ? "ĐÃ COPY" : (copyFailed ? "COPY LỖI" : "COPY"), closeStyle);
      bool sentRecent = global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineAt > 0f &&
          Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineAt < 2f;
      GUI.set_color(new Color(0.12f, 0.3f, 0.55f, 0.95f));
      GUI.DrawTexture(sendRect, tex);
      GUI.set_color(new Color(1f, 1f, 1f, 1f));
      int onlineState = global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineState;
      GUI.Label(sendRect, onlineState == 1 ? "ĐANG GỬI" :
          (sentRecent && onlineState == 2 ? "ĐÃ NHẬN" :
          (sentRecent && onlineState < 0 ? "GỬI LỖI" : "GỬI LOG")), closeStyle);
      if (et == 1 && sendRect.Contains(mp)) {
      }
      if (et == 1 && copyRect.Contains(mp)) {
        CopyAimKillLog();
      }
      if (et == 1 && close.Contains(mp)) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LogOpen = false;
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
    GUI.Label(new Rect(px + 18, py + 8, 320, 50), "OKMOD R12.30 DEV", big);

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
        "SC hook: " + global::COW.GamePlay.SceneEditBoxSelectTool.R7ScatterCalls.ToString() +
        " | NR: " + (PlayerPrefs.GetString("OKMOD.Combat.NoRecoil", "") == "X2" ? "ON" : "OFF"),
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
    global::COW.GamePlay.SceneEditBoxSelectTool.OkUiPhase = -1;
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
      if (global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouchFrame == frame + 1) { return false; }
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouchFrame = frame + 1;
      int index = -1;
      if (global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouch) {
        for (int i = 0; i < count; i++) {
          if (Input.GetTouch(i).get_fingerId() == global::COW.GamePlay.SceneEditBoxSelectTool.OkUiFinger) {
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
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouch = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiActive = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiSlider = 0;
        return false;
      }
      Touch touch = Input.GetTouch(index);
      Vector2 position = touch.get_position();
      x = position.x / scale;
      y = ((float)screenH - position.y) / scale;
      int state = (int)touch.get_phase();
      if (state == 0) {
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouch = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiFinger = touch.get_fingerId();
        phase = 0;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouch) {
        if (state == 1 || state == 2) { phase = 1; }
        if (state == 3) { phase = 2; }
        if (state == 4) { phase = 3; }
      }
    } else {
      if (global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouch) {
        // A vanished/cancelled finger is not a successful click.
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouch = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiActive = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiSlider = 0;
      }
      // Suppress Unity's emulated mouse sequence for a touch already processed.
      int touchFrame = global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouchFrame;
      if (touchFrame > 0 && frame <= touchFrame) { return false; }
      Vector2 position = ev.get_mousePosition();
      x = position.x / scale;
      y = position.y / scale;
      if (eventType == 0) { phase = 0; }
      if (eventType == 3) { phase = 1; }
      if (eventType == 1) { phase = 2; }
    }
    if (phase < 0) { return false; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkUiPhase = phase;
    bool inside = false;
    if (open) { inside = new Rect(px, py, 800f, 680f).Contains(new Vector2(x, y)); }
    else {
      Vector2 pointer = new Vector2(x, y);
      inside = new Rect(10f, uiH / 2 - 60f, 116f, 116f).Contains(pointer);
    }
    bool active = global::COW.GamePlay.SceneEditBoxSelectTool.OkUiActive;
    if (phase == 0) {
      active = inside;
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiActive = active;
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiMoved = false;
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiSlider = 0;
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
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiMoved = true;
      }
      if (phase == 1 && global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured &&
          global::COW.GamePlay.SceneEditBoxSelectTool.OkUiMoved) {
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX =
            global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelX + x -
            global::COW.GamePlay.SceneEditBoxSelectTool.MzDownX;
        global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY =
            global::COW.GamePlay.SceneEditBoxSelectTool.MzPanelY + y -
            global::COW.GamePlay.SceneEditBoxSelectTool.MzDownY;
      }
      if (phase == 1 && open && global::COW.GamePlay.SceneEditBoxSelectTool.OkUiMoved &&
          !global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured &&
          global::COW.GamePlay.SceneEditBoxSelectTool.MzPressX >= px + 20f &&
          global::COW.GamePlay.SceneEditBoxSelectTool.MzPressX < px + 420f &&
          global::COW.GamePlay.SceneEditBoxSelectTool.MzPressY >= py + 80f &&
          global::COW.GamePlay.SceneEditBoxSelectTool.MzPressY < py + 584f) {
        float scroll = global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll +
            global::COW.GamePlay.SceneEditBoxSelectTool.MzDownY - y;
        float maxScroll = MenuScrollMax();
        global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll = Mathf.Clamp(scroll, 0f, maxScroll);
      }
      clicked = phase == 2 && inside && !global::COW.GamePlay.SceneEditBoxSelectTool.OkUiMoved &&
          global::COW.GamePlay.SceneEditBoxSelectTool.OkUiSlider == 0;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.MzDownX = x;
    global::COW.GamePlay.SceneEditBoxSelectTool.MzDownY = y;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkUiX = x;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkUiY = y;
    if (phase == 2 || phase == 3) {
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiActive = false;
      global::COW.GamePlay.SceneEditBoxSelectTool.MzPointerCaptured = false;
      if (touchInput) { global::COW.GamePlay.SceneEditBoxSelectTool.OkUiTouch = false; }
      if (phase == 3) { global::COW.GamePlay.SceneEditBoxSelectTool.OkUiSlider = 0; }
    }
    if ((active || inside) && (eventType == 0 || eventType == 1 || eventType == 3)) { ev.Use(); }
    return clicked;
  }

  private static GUIStyle CachedStyle(int slot) {
    GUIStyle[] styles = global::COW.GamePlay.SceneEditBoxSelectTool.OkUiStyles;
    if (styles == null) {
      styles = new GUIStyle[6];
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiStyles = styles;
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
      if (enabled && key == "OKMOD.Move.HighJump") {
        PlayerPrefs.SetString("OKMOD.Move.Hover", "");
      }
      if (enabled && key == "OKMOD.Move.Hover") {
        PlayerPrefs.SetString("OKMOD.Move.HighJump", "");
      }
      if (enabled && key == "OKMOD.Aim.Enabled") {
        PlayerPrefs.SetString("OKMOD.Aim.Silent", "");
        PlayerPrefs.SetString("OKMOD.Aim.AimKill", "");
      }
      if (enabled && key == "OKMOD.Aim.Silent") {
        PlayerPrefs.SetString("OKMOD.Aim.Enabled", "");
        PlayerPrefs.SetString("OKMOD.Aim.AimKill", "");
      }
      if (enabled && key == "OKMOD.Aim.AimKill") {
        PlayerPrefs.SetString("OKMOD.Aim.Enabled", "");
        PlayerPrefs.SetString("OKMOD.Aim.Silent", "");
      }
      if (key == "OKMOD.Aim.AimKill") { SetAimKillEnabled(enabled); }
      if (key == "OKMOD.Aim.Enabled" || key == "OKMOD.Aim.Silent") {
        global::COW.GamePlay.SceneEditBoxSelectTool.OkAimTarget = null;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentFrame = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentAppliedFrame = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentCalls = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentWrites = 0;
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
    if (key == "OKMOD.Move.SpeedScale") { return ReadSpeedScale(); }
    float value = PlayerPrefs.GetFloat(key + ".Float", (float)ReadSetting(key, fallback, min, max));
    if (!(value >= (float)min && value <= (float)max)) {
      value = Mathf.Clamp((float)fallback, (float)min, (float)max);
    }
    return value;
  }

  private static float ReadSpeedScale() {
    float value = PlayerPrefs.GetFloat("OKMOD.Move.SpeedScale.Float", 1f);
    if (value >= 10f && value <= 20f) {
      value = value / 10f;
      PlayerPrefs.SetFloat("OKMOD.Move.SpeedScale.Float", value);
    }
    if (!(value >= 1f && value <= 2f)) { value = 1f; }
    return value;
  }

  private static void SliderRow(float x, float y, Vector2 mp, int et, int id,
      string key, string label, int fallback, int min, int max, float step, GUIStyle style,
      float displayScale = 1f) {
    float value = ReadFloatSetting(key, fallback, min, max);
    int phase = global::COW.GamePlay.SceneEditBoxSelectTool.OkUiPhase;
    if (phase == 0 && global::COW.GamePlay.SceneEditBoxSelectTool.OkUiActive &&
        new Rect(x + 52f, y + 30f, 236f, 52f).Contains(mp)) {
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiSlider = id;
    }
    bool changed = false;
    if (global::COW.GamePlay.SceneEditBoxSelectTool.OkUiSlider == id && phase >= 0 && phase <= 2) {
      float t = Mathf.Clamp((mp.x - x - 64f) / 212f, 0f, 1f);
      value = (float)min + t * (float)(max - min);
      changed = true;
      if (phase == 2) { global::COW.GamePlay.SceneEditBoxSelectTool.OkUiSlider = 0; }
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
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
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
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
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
    float scroll = global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (et == 1) {
      if (new Rect(px + 20, py + 592, 185f, 48f).Contains(mp)) { scroll = scroll - 72f; }
      if (new Rect(px + 215, py + 592, 195f, 48f).Contains(mp)) { scroll = scroll + 72f; }
      global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll = Mathf.Clamp(scroll, 0f, max);
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
    return PlayerPrefs.GetString("OKMOD.Page.AIM", "") == "X2" ? 1656f : 1512f;
  }

  private static void DrawEspMenu(int px, int py, Vector2 mp, int et, GUIStyle row) {
    ListToggle(px, py, 0, mp, et, "OKMOD.Esp.Enabled", "Khung người chơi", "", row);
    ListToggle(px, py, 1, mp, et, "OKMOD.Esp.PlayerName", "Tên người chơi", "", row);
    ListToggle(px, py, 2, mp, et, "OKMOD.Esp.Hp", "Máu", "", row);
    ListToggle(px, py, 3, mp, et, "OKMOD.Esp.Distance", "Khoảng cách", "", row);
    ListToggle(px, py, 4, mp, et, "OKMOD.Esp.AimLine", "Đường ngắm", "", row);
    ListToggle(px, py, 5, mp, et, "OKMOD.Esp.FovRing", "Vòng FOV", "", row);
    ListToggle(px, py, 6, mp, et, "OKMOD.Esp.Box", "Khung đầy", "", row);
    ListToggle(px, py, 7, mp, et, "OKMOD.Esp.HeadMark", "Đánh dấu đầu", "", row);
    ListToggle(px, py, 8, mp, et, "OKMOD.Esp.Link", "Tia nối", "", row);
    ListToggle(px, py, 9, mp, et, "OKMOD.Esp.Team", "ESP đồng đội", "", row);
    ListToggle(px, py, 10, mp, et, "OKMOD.Esp.BlueLines", "Tia xanh", "", row);
    ListToggle(px, py, 11, mp, et, "OKMOD.Esp.BoxField", "Khung đặc", "", row);
    ListToggle(px, py, 12, mp, et, "OKMOD.Esp.Vehicle", "ESP xe", "", row);
    ListToggle(px, py, 13, mp, et, "OKMOD.Esp.Alert", "Cảnh báo địch", "", row);
    ColorRow(px, py, 14, mp, et, "OKMOD.Color.Enemy", "Màu địch", 0, row);
    ColorRow(px, py, 15, mp, et, "OKMOD.Color.Team", "Màu team", 5, row);
    ColorRow(px, py, 16, mp, et, "OKMOD.Color.Vehicle", "Màu xe", 2, row);
    ListToggle(px, py, 17, mp, et, "OKMOD.Esp.HeadCircle", "Vòng đầu", "", row);
    ListToggle(px, py, 18, mp, et, "OKMOD.Esp.Skeleton", "Skeleton (120m)", "", row);
    ListToggle(px, py, 19, mp, et, "OKMOD.Esp.Info", "Trạng thái địch", "", row);
    ListToggle(px, py, 20, mp, et, "OKMOD.Esp.ArmorFlags", "Có giáp / mũ", "", row);
    ColorRow(px, py, 21, mp, et, "OKMOD.Color.Name", "Màu tên", 0, row);
    ListToggle(px, py, 22, mp, et, "OKMOD.Esp.DirectOnly", "Chỉ địch không bị che", "", row);
    ListToggle(px, py, 23, mp, et, "OKMOD.Esp.Threat", "Cảnh báo nhìn về mình", "", row);
    ListToggle(px, py, 24, mp, et, "OKMOD.Esp.Radar100", "Mini radar (100m)", "", row);
    ListToggle(px, py, 25, mp, et, "OKMOD.Esp.Offscreen", "Mũi tên ngoài màn hình", "", row);
    ListToggle(px, py, 26, mp, et, "OKMOD.Esp.Crosshair", "Tâm ngắm riêng", "", row);
    CrosshairStyleRow(px, py, 27, mp, et, row);
    ScrollButtons(px, py, 1512f, mp, et, row);
    SliderRow(px + 440, py + 192, mp, et, 1, "OKMOD.Aim.FOV", "FOV px", 220, 20, 800, 20f, row);
    SliderRow(px + 440, py + 284, mp, et, 4, "OKMOD.Move.SpeedScale", "Tốc chạy", 1, 1, 2, 0.1f, row);
    SliderRow(px + 440, py + 376, mp, et, 5, "OKMOD.Misc.CameraFov", "FOV camera", 90, 60, 120, 5f, row);
    SliderRow(px + 440, py + 560, mp, et, 8, "OKMOD.Esp.Font", "Cỡ chữ ESP", 24, 16, 32, 1f, row);
    row.get_normal().set_textColor(new Color(1f, 0.82f, 0.4f, 1f));
    GUI.Label(new Rect(px + 440, py + 468, 340f, 48f), "Màu A: chạm để đặt lại", row);
    if (et == 1 && new Rect(px + 440, py + 468, 340f, 48f).Contains(mp)) {
      PlayerPrefs.SetFloat("OKMOD.Color.Enemy.R", 1f);
      PlayerPrefs.SetFloat("OKMOD.Color.Enemy.G", 0.353f);
      PlayerPrefs.SetFloat("OKMOD.Color.Enemy.B", 0.431f);
      PlayerPrefs.SetFloat("OKMOD.Color.Enemy.A", 0.95f);
      PlayerPrefs.SetFloat("OKMOD.Color.Team.R", 0.239f);
      PlayerPrefs.SetFloat("OKMOD.Color.Team.G", 0.839f);
      PlayerPrefs.SetFloat("OKMOD.Color.Team.B", 1f);
      PlayerPrefs.SetFloat("OKMOD.Color.Team.A", 0.95f);
      PlayerPrefs.SetFloat("OKMOD.Color.Target.R", 1f);
      PlayerPrefs.SetFloat("OKMOD.Color.Target.G", 0.82f);
      PlayerPrefs.SetFloat("OKMOD.Color.Target.B", 0.4f);
      PlayerPrefs.SetFloat("OKMOD.Color.Target.A", 0.95f);
      PlayerPrefs.SetInt("OKMOD.Color.Name", 0);
      PlayerPrefs.SetFloat("OKMOD.Color.Name.R", 1f);
      PlayerPrefs.SetFloat("OKMOD.Color.Name.G", 1f);
      PlayerPrefs.SetFloat("OKMOD.Color.Name.B", 1f);
      PlayerPrefs.SetFloat("OKMOD.Color.Name.A", 1f);
      PlayerPrefs.Save();
    }
    row.get_normal().set_textColor(new Color(0.75f, 0.75f, 0.8f, 1f));
    GUI.Label(new Rect(px + 440, py + 80, 340f, 48f), "Vuốt danh sách bên trái", row);
    GUI.Label(new Rect(px + 440, py + 136, 340f, 48f), "Kéo tiêu đề để di chuyển", row);
  }
  private static void CrosshairStyleRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int mode = ReadSetting("OKMOD.Esp.CrosshairStyle", 1, 0, 2);
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      mode++;
      if (mode > 2) { mode = 0; }
      PlayerPrefs.SetInt("OKMOD.Esp.CrosshairStyle", mode);
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
    ListToggle(px, py, 0, mp, et, "OKMOD.Aim.Enabled", "Bật aimbot", "", row);
    ListToggle(px, py, 1, mp, et, "OKMOD.Aim.AnyAngle", "Aimbot any-angle", "", row);
    ListToggle(px, py, 2, mp, et, "OKMOD.Aim.Fire", "Khi bắn", "", row);
    ListToggle(px, py, 3, mp, et, "OKMOD.Aim.Visible", "Kiểm tra vật cản", "X2", row);
    ListToggle(px, py, 4, mp, et, "OKMOD.Aim.SkipKnocked", "Bỏ qua knock", "X2", row);
    ListToggle(px, py, 5, mp, et, "OKMOD.Aim.Sticky", "Giữ mục tiêu", "X2", row);
    ListToggle(px, py, 6, mp, et, "OKMOD.Aim.Mark", "Đánh dấu mục tiêu", "X2", row);
    ListToggle(px, py, 7, mp, et, "OKMOD.Esp.FovRing", "Hiện vòng FOV", "", row);
    ListToggle(px, py, 8, mp, et, "OKMOD.Aim.Silent", "Silent Aim", "", row);
    ListToggle(px, py, 9, mp, et, "OKMOD.Aim.Lock", "Aimbot lock", "", row);
    ListToggle(px, py, 10, mp, et, "OKMOD.Combat.NoReload", "No reload", "", row);
    ListToggle(px, py, 11, mp, et, "OKMOD.Combat.FastSwap", "Fast swap", "", row);
    ListToggle(px, py, 12, mp, et, "OKMOD.Misc.Camera90", "Camera 90°", "", row);
    HitFlyValueRow(px, py, 13, mp, et, "OKMOD.Move.SpeedScale", "Tốc chạy x", 1f, 1f, 2f, 0.1f, row);    ListToggle(px, py, 14, mp, et, "OKMOD.Combat.NoRecoil", "No recoil", "", row);
    ListToggle(px, py, 14, mp, et, "OKMOD.Combat.RapidFire", "Rapid fire", "", row);
    ListToggle(px, py, 15, mp, et, "OKMOD.Misc.HighFPS", "High FPS", "", row);
    ListToggle(px, py, 16, mp, et, "OKMOD.Combat.HideDamage", "Hide damage", "", row);
    ListToggle(px, py, 17, mp, et, "OKMOD.Combat.InfiniteAmmo", "Infinite ammo", "", row);
    ListToggle(px, py, 18, mp, et, "OKMOD.Combat.AutoSwap", "Auto swap", "", row);
    ListToggle(px, py, 19, mp, et, "OKMOD.Move.HighJump", "Nhảy cao x3.0", "", row);
    ListToggle(px, py, 20, mp, et, "OKMOD.Move.Hover", "Bay lơ lửng x3.0 + 0.3", "", row);
    ListToggle(px, py, 21, mp, et, "OKMOD.Aim.Scope", "Khi giữ ngắm", "X2", row);
    ListToggle(px, py, 22, mp, et, "OKMOD.Aim.AimKill", "AimKill", "", row);
    AimKillStatusRow(px, py, 23, row);
    AimKillLogRow(px, py, 24, mp, et, row);
    ListToggle(px, py, 25, mp, et, "OKMOD.Diagnostic.NativeRecovery", "Khôi phục gốc: tạm dừng mod", "", row);
    ScrollButtons(px, py, MenuScrollMax(), mp, et, row);

    int point = ReadSetting("OKMOD.Aim.Point", 0, 0, 5);
    if (et == 1 && new Rect(px + 440, py + 80, 340f, 56f).Contains(mp)) {
      point = point + 1;
      if (point > 5) { point = 0; }
      PlayerPrefs.SetInt("OKMOD.Aim.Point", point);
    }
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 440, py + 84, 340f, 52f), AimPointLabel(point), row);

    int priority = ReadSetting("OKMOD.Aim.Priority", 0, 0, 1);
    string priorityLabel = "Ưu tiên: Gần tâm";
    if (priority == 1) { priorityLabel = "Ưu tiên: Gần người"; }
    if (et == 1 && new Rect(px + 440, py + 136, 340f, 56f).Contains(mp)) {
      priority = 1 - priority;
      PlayerPrefs.SetInt("OKMOD.Aim.Priority", priority);
    }
    if (priority == 0) { priorityLabel = "Ưu tiên: Gần tâm"; }
    else { priorityLabel = "Ưu tiên: Gần người"; }
    GUI.Label(new Rect(px + 440, py + 140, 340f, 52f), priorityLabel, row);
    SliderRow(px + 440, py + 192, mp, et, 1, "OKMOD.Aim.FOV", "FOV px", 220, 20, 800, 20f, row);
    SliderRow(px + 440, py + 284, mp, et, 2, "OKMOD.Aim.Range", "Tầm m", 150, 10, 500, 10f, row);
    SliderRow(px + 440, py + 376, mp, et, 3, "OKMOD.Aim.Smooth", "Mượt", 6, 0, 20, 0.1f, row);
    row.get_normal().set_textColor(new Color(0.75f, 0.75f, 0.8f, 1f));
    DrawAimLogActions(px, py, mp, et, row);
    if (et == 1 && new Rect(px + 440, py + 524, 160f, 48f).Contains(mp)) {
      PlayerPrefs.SetFloat("OKMOD.Aim.Smooth.Float", 0f);
      PlayerPrefs.SetInt("OKMOD.Aim.Smooth", 0);
    }
    if (et == 1 && new Rect(px + 610, py + 524, 170f, 48f).Contains(mp)) {
      PlayerPrefs.SetFloat("OKMOD.Aim.Smooth.Float", 6f);
      PlayerPrefs.SetInt("OKMOD.Aim.Smooth", 6);
    }
    GUI.set_color(new Color(0.15f, 0.22f, 0.3f, 1f));
    GUI.DrawTexture(new Rect(px + 440, py + 524, 160f, 48f), Texture2D.get_whiteTexture());
    GUI.DrawTexture(new Rect(px + 610, py + 524, 170f, 48f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 450, py + 528, 150f, 44f), "Trực tiếp", row);
    GUI.Label(new Rect(px + 620, py + 528, 160f, 44f), "Mượt 6", row);
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus;
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7ScatterCalls.ToString();
    if (PlayerPrefs.GetString("OKMOD.Aim.Silent", "") == "X2") {
      int silentStatus = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus;
      int getterFrame = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentFrame;
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
          "S3 · Getter " + global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentCalls.ToString() +
          " / Ghi " + global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentWrites.ToString(), diagnostic);
    } else {
      GUI.Label(new Rect(px + 440, py + 592, 340f, 48f), statusLabel, row);
    }
  }
  private static float HitFlyValue(string key, float fallback, float min, float max) {
    if (key == "OKMOD.Move.SpeedScale") { return ReadSpeedScale(); }
    float value = PlayerPrefs.GetFloat(key + ".Float", fallback);
    if (!(value >= min && value <= max)) { value = fallback; }
    return value;
  }

  private static void HitFlyValueRow(int px, int py, int index, Vector2 mp, int et,
      string key, string label, float fallback, float min, float max, float step, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
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
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    GUI.set_color(new Color(0.11f, 0.13f, 0.17f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    GUI.Label(new Rect(px + 34f, y + 10f, 350f, 44f), label, row);
  }

  private static void HitFlyRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) { QueueHitFly(); }
    GUI.set_color(new Color(0.06f, 0.2f, 0.23f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.3f, 1f, 0.8f, 1f));
    GUI.Label(new Rect(px + 32f, y + 10f, 370f, 48f), "HITFLY: bấm một lần", row);
  }

  private static string HitFlyStatusLabel() {
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus;
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
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapStatus;
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
    if (status == 16) { label = "HF MAP: đáp lệch " + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMiss.ToString("F1") + "m (client)"; }
    if (status == 17) { label = "HF MAP: kết thúc quan sát, xem LOG"; }
#endif
    return label;
  }

  private static void HitFlyStatusRow(int px, int py, int index, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    row.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 3f, 370f, 32f), HitFlyStatusLabel(), row);
    GUI.Label(new Rect(px + 32f, y + 31f, 370f, 32f), "Gửi " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlySent.ToString() + " / thử " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyAttempts.ToString(), row);
  }

  private static string VehAuthStatusLabel() {
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus;
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
    if (status == 15) { return "1146: dmg rejected r=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7VehDmgResult.ToString(); }
    return "1146: ?"; }

  private static void VehAuthStatusRow(int px, int py, int index, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    row.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 1f, 370f, 30f), VehAuthStatusLabel(), row);
    GUI.Label(new Rect(px + 32f, y + 28f, 370f, 30f), "Change " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehCalls.ToString() + " | Push " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehSent.ToString(), row);
    string error = global::COW.GamePlay.SceneEditBoxSelectTool.R7VehError;
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
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehDmgResult = result;
      return result != 0;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehError =
          "dmg: " + error.GetType().Name + ": " + error.Message;
      return false;
    }
  }

  private static void UpdateVehicleAuth() {
    if (PlayerPrefs.GetString("OKMOD.Move.VehAuth", "") != "X2") {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehVehicle = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehTarget = null;
      return;
    }
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7VehNext) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7VehNext = now + 0.1f;

    global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
    global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
    if (local == null || !local.IsLocalPlayer() || local.get_CurHP() <= 0) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 1; return; }
    if (match == null) { global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 2; return; }
    System.Collections.IList list = match.ILPNGCMEFFI();
    if (list == null) { global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 3; return; }

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
    if (vehicle == null) { global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 4; return; }

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
    if (target == null) { global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 5; return; }

    bool justAcquired = vehicle != global::COW.GamePlay.SceneEditBoxSelectTool.R7VehVehicle;
    if (justAcquired || now >= global::COW.GamePlay.SceneEditBoxSelectTool.R7VehReauth) {
      try {
        // Enter=true and driver seat 0: grant local auth on this vehicle.
        vehicle.ChangeState(true, local, false, 0, 0u, false);
        vehicle.SetVisibility(8u, true);
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehVehicle = vehicle;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehTarget = target;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehReauth = now + 0.5f;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehCalls++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 6;
      } catch (System.Exception error) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehError = error.GetType().Name + ": " + error.Message;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 7;
        return;
      }
    }

    UnityEngine.Transform targetRoot = target.get_RootTransform();
    if (targetRoot == null || vehicle.get_transform() == null) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 5; return; }
    Vector3 to = targetRoot.get_position();
    to.y = to.y + 1.0f;
    Vector3 from = vehicle.get_transform().get_position();
    float dist = Vector3.Distance(from, to);
    if (!(dist > 0.05f)) { global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 8; return; }
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
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehSent++;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 9;

      if (now >= global::COW.GamePlay.SceneEditBoxSelectTool.R7VehHitAt) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehHitAt = now + 0.5f;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7VehHpBefore = target.get_CurHP();
        if (DispatchVehicleHit(local, target, vehicle, from, to)) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 14;
        } else {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 15;
        }
      }
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehError = error.GetType().Name + ": " + error.Message;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7VehStatus = 7;
    }
  }

  private static void QueueHitFly() {
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyNext) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = 2;
      return;
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyPending) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyPending = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = 1;
  }


  private static void QueueMapHitFly() {
#if HF_GUIDED_TRIAL
    if (GuidedMapActive()) { EndGuidedMap(6, "guide-user-cancelled"); return; }
#endif
#if HF_SOPHIA_TRIAL
    if (SophiaObservationActive()) { BlockHitFlyMap(15, "map-observation-active-no-repeat"); return; }
#endif
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFNoticeUntil = Time.get_realtimeSinceStartup() + 8f;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapDirty = true;
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapNext) {
      BlockHitFlyMap(2, "map-cooldown"); return;
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapPending) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapPending = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapStatus = 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapReason = "queued";
  }

  private static void BlockHitFlyMap(int status, string reason) {
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapStatus = status;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapReason = reason;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapDirty = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFNoticeUntil = Time.get_realtimeSinceStartup() + 8f;
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState = state;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapReason = reason;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOwner = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMatch = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapDirty = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFNoticeUntil = Time.get_realtimeSinceStartup() + 8f;
  }

  private static void ObserveHitFlyMap() {
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState != 1 &&
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState != 2) { return; }
    try {
      global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOwner;
      if (NativeRecovery() || local == null || local != global::COW.GameFacade.GetLocalPlayerOrObServer() ||
          global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMatch != global::COW.GameFacade.CurrentMatch() ||
          !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        FinishHitFlyMap(6, "context-cancelled"); return;
      }
      float now = Time.get_realtimeSinceStartup();
      if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapSampleAt) { return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapSampleAt = now + 0.1f;
      Transform root = local.get_RootTransform();
      if (root == null) { FinishHitFlyMap(6, "missing-root"); return; }
      Vector3 position = root.get_position();
      if (!ValidVector(position)) { FinishHitFlyMap(6, "invalid-position"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapLast = position;
      if (position.y > global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapApex) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapApex = position.y;
      }
      Vector3 target = global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapTarget;
      float dx = target.x - position.x, dz = target.z - position.z;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMiss = Mathf.Sqrt(dx * dx + dz * dz);
      float elapsed = now - global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapAt;
      bool grounded = local.IsGrounded();
      if (!grounded) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState = 2;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGroundSamples = 0;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState == 2 && elapsed > 0.3f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGroundSamples++;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGroundSamples >= 2) {
          float dy = target.y - position.y;
          bool near = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMiss <= 3f && dy >= -2f && dy <= 2f;
          FinishHitFlyMap(3, near ? "landed-near-client-observation" : "landed-miss-client-observation"); return;
        }
      }
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState == 1 && elapsed > 2f) {
        FinishHitFlyMap(4, "no-takeoff-observed"); return;
      }
      if (elapsed > global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapTime + 5f) {
        FinishHitFlyMap(5, "landing-timeout");
      }
    } catch (System.Exception error) { FinishHitFlyMap(7, error.Message); }
  }

  private static string HitFlyMapLog() {
#if HF_GUIDED_TRIAL
    string mode = "guided-keyed-force-trial";
    string trial = " guideTrial=1 phase=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideState.ToString()
        + " fault=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideFault ? "1" : "0")
        + " owned=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideForceOwned ? "1" : "0")
        + " position=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastPos)
        + " force=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastForce);
#elif HF_SOPHIA_TRIAL
    string mode = "sophia-657-trial";
    string trial = " obs=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState.ToString()
        + " estimate=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaEstimate.ToString("F2")
        + " gravity=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGravity.ToString("F2")
        + " scale=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaScale.ToString("F2")
        + " nearestXZ=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaNearest.ToString("F2")
        + " physState=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaPhysState.ToString()
        + " vel=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaVx.ToString("F2") + ","
        + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaVy.ToString("F2") + ","
        + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaVz.ToString("F2")
        + " missXZ=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMiss.ToString("F2")
        + " rise=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapApex - global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOrigin.y).ToString("F2")
        + " last=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapLast);
#else
    string mode = "bearing-up";
    string trial = "";
#endif
    return "hfMap status=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapStatus.ToString()
        + " mode=" + mode
        + trial
        + " attempts=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapAttempts.ToString()
        + " sent=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapSent.ToString()
        + " error=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapError
        + " reason=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapReason
        + " highJump=" + (HighJumpOn() ? "1" : "0")
        + " hover=" + (HoverModeOn() ? "1" : "0")
        + " t=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapTime.ToString("F2")
        + " xz=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapXZ.ToString("F2")
        + " vy=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapY.ToString("F2")
        + "\nhfMap target=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapTarget)
        + " origin=" + HitFlyCoordinates(global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOrigin)
        + " exactLanding=0 collisionAvoidance=unverified serverAck=unobserved";
  }

  private static string HitFlyCoordinates(Vector3 point) {
    return point.x.ToString("F2") + "," + point.y.ToString("F2") + "," + point.z.ToString("F2");
  }

  private static void HitFlyMapRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
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
    float next = mapMode ? global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapNext :
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyNext;
    if (now < next) {
      if (mapMode) { BlockHitFlyMap(2, "map-cooldown"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = 2; return;
    }
    try {
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      if (global::COW.GameFacade.CurrentMatch() == null || local == null ||
          !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        if (mapMode) { BlockHitFlyMap(3, "no-live-local-match"); return; }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = 3; return;
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGravity = gravity;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaEstimate = estimate;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaScale = scale;
#else
        // Original bearing/impulse route, with a stronger upward launch.
        // Do not infer flight time or an exact landing point from distance.
        duration = 1f;
        horizontal = HitFlyValue("OKMOD.Move.HFMapSpeed", 25f, 5f, 100f);
        vertical = 50f;
#endif
        mapOrigin = origin; mapTarget = mark;
      } else {
        duration = HitFlyValue("OKMOD.Move.HitFlyTime", 1f, 0.2f, 3f);
        horizontal = HitFlyValue("OKMOD.Move.HitFlyXZ", 25f, 0f, 100f);
        vertical = HitFlyValue("OKMOD.Move.HitFlyY", 20f, 0f, 100f);
        Camera camera = Camera.get_main();
        if (camera == null || camera.get_transform() == null) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = 3; return;
        }
        direction = camera.get_transform().get_forward();
      }
      float length = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y + direction.z * direction.z);
      if (playerId == 0 || !ValidVector(direction) || !(length > 0.001f && length < 100f)) {
        if (mapMode) { BlockHitFlyMap(4, "invalid-id-or-direction"); return; }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = 4; return;
      }
      direction = Vector3.Normalize(direction);
      global::message.BJFGLKKKOAN packed = global::message.LMCBHLCPAJD.MGOGJCMCIPG(direction);
      if (packed == null) {
        if (mapMode) { BlockHitFlyMap(4, "direction-pack-null"); return; }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = 4; return;
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapTarget = mapTarget;
        float dx = mapTarget.x - mapOrigin.x, dz = mapTarget.z - mapOrigin.z;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapDistance = Mathf.Sqrt(dx * dx + dz * dz);
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOrigin = mapOrigin;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapTime = duration;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapXZ = horizontal;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapY = vertical;
      }
      float cooldown = msg.DCEMJOGHMEG + 0.25f;
      if (cooldown < 1f) { cooldown = 1f; }
      if (mapMode) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapNext = now + cooldown;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapAttempts++;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyNext = now + cooldown;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyAttempts++;
      }
      bool sent = global::COW.GameFacade.Send(657u, msg, (byte)2, false);
      if (mapMode) {
        if (sent) { global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapSent++; }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapError = "";
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = sent ? 5 : 6;
        if (sent) { global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlySent++; }
#if HF_GUIDED_TRIAL
        if (sent) { global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideManualUntil = now + cooldown; }
#endif
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyError = "";
      }
    } catch (System.Exception error) {
      if (mapMode) {
        BlockHitFlyMap(7, error.Message);
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapError = error.Message;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapNext = now + 1f;
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyStatus = 7;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyError = error.Message;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyNext = now + 1f;
      PlayerPrefs.SetString("OKMOD.Diagnostic.HitFlyError", error.ToString());
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
    int state = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState;
    return state == 1 || state == 2;
  }

  private static void BeginSophiaObservation(global::COW.GamePlay.Player local) {
    float now = Time.get_realtimeSinceStartup();
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOwner = local;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMatch = global::COW.GameFacade.CurrentMatch();
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState = 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapAt = now;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapSampleAt = now;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaTelemetryAt = now + 2f;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapApex = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOrigin.y;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapLast = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOrigin;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMiss = global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapDistance;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaNearest = global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapDistance;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaAirSeen = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaWasGrounded = local.IsGrounded();
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGroundSamples = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapDirty = true;
  }

  private static void EndSophiaObservation(int state, string reason) {
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState = state;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOwner = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMatch = null;
    BlockHitFlyMap(state == 3 ? 16 : 17, reason);
  }

  private static void ObserveSophiaMap() {
    if (!SophiaObservationActive()) { return; }
    try {
      global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOwner;
      if (NativeRecovery() || local == null || local != global::COW.GameFacade.GetLocalPlayerOrObServer() ||
          global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMatch != global::COW.GameFacade.CurrentMatch() ||
          !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        EndSophiaObservation(6, "observation-context-cancelled"); return;
      }
      float now = Time.get_realtimeSinceStartup();
      float elapsed = now - global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapAt;
      if (!(elapsed >= 0f && elapsed <= 35f)) { EndSophiaObservation(5, "observation-clock-or-timeout"); return; }
      if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapSampleAt) { return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapSampleAt = now + 0.1f;
      if (now >= global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaTelemetryAt) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapDirty = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaTelemetryAt = now + 2f;
      }
      Transform root = local.get_RootTransform();
      if (root == null) { EndSophiaObservation(6, "observation-missing-root"); return; }
      Vector3 position = root.get_position();
      if (!ValidVector(position)) { EndSophiaObservation(6, "observation-invalid-position"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaPhysState = (int)local.GetPhysXState();
      Vector3 velocity = local.GetVelocity();
      if (!ValidVector(velocity)) { EndSophiaObservation(6, "observation-invalid-velocity"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaVx = velocity.x;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaVy = velocity.y;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaVz = velocity.z;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapLast = position;
      if (position.y > global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapApex) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapApex = position.y;
      }
      Vector3 target = global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapTarget;
      float dx = target.x - position.x, dz = target.z - position.z;
      float distance = Mathf.Sqrt(dx * dx + dz * dz);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapMiss = distance;
      if (distance < global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaNearest) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaNearest = distance;
      }
      bool grounded = local.IsGrounded() && global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaPhysState != 5;
      if (!grounded) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaAirSeen = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapState = 2;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGroundSamples = 0;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaAirSeen && elapsed > 0.5f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGroundSamples++;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapGroundSamples >= 3) {
          EndSophiaObservation(3, global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaWasGrounded ?
              "landed-client-observation-unattributed" : "landed-airborne-start-unattributed"); return;
        }
      }
      if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7HFSophiaAirSeen && elapsed > 3f) {
        EndSophiaObservation(4, "no-airborne-sample-observed"); return;
      }
    } catch (System.Exception error) { EndSophiaObservation(7, "observation-error: " + error.Message); }
  }
#endif

#if HF_GUIDED_TRIAL
  // Staged experiment: this key is owned only by the guided MAP controller.
  private const uint GuidedMapForceKey = 1330333766u;

  private static bool GuidedMapActive() {
    int state = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideState;
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideFault = true;
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideForceOwned) {
      try {
        global::COW.GamePlay.Player owner = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideOwner;
        if (owner != null) { owner.SetForceMoveVelocity(new Vector3(), GuidedMapForceKey); }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideForceOwned = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastForce = new Vector3();
      } catch (System.Exception error) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideState = 9;
        BlockHitFlyMap(7, "guide-cleanup-fault: " + error.Message);
        return;
      }
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideFault = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideState = state;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideOwner = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideMatch = null;
    BlockHitFlyMap(state == 5 ? 16 : 17, reason);
  }

  private static void StartGuidedMap() {
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideFault) { BlockHitFlyMap(7, "guide-fault-restart-required"); return; }
    if (GuidedMapActive()) { BlockHitFlyMap(15, "guide-already-active"); return; }
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapNext) { BlockHitFlyMap(2, "map-cooldown"); return; }
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideManualUntil) { BlockHitFlyMap(15, "manual-hf-in-progress"); return; }
    try {
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
      if (local == null || match == null || !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        BlockHitFlyMap(3, "no-live-local-match"); return;
      }
      // Do not compete with an existing game hitfly/vehicle/airborne state.
      if (!local.IsGrounded() || local.GetPhysXState() != 0 || HighJumpOn() || HoverModeOn() ||
          global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityApplied != 0) {
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
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideOwner = local;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideMatch = match;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapTarget = target;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOrigin = origin;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastPos = origin;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideCruiseY = (origin.y > target.y ? origin.y : target.y) + 35f;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideSpeed = HitFlyValue("OKMOD.Move.HFMapSpeed", 25f, 5f, 100f);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideAt = now;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideTickAt = now;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideProgressAt = now;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideGroundAt = 0f;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideMetric = 100000f;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastForce = new Vector3();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideState = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapNext = now + 1f;
      BlockHitFlyMap(18, "trial-ascent");
    } catch (System.Exception error) { EndGuidedMap(7, "guide-start-error: " + error.Message); }
  }

  private static void TickGuidedMap() {
    if (!GuidedMapActive() || global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideFault) { return; }
    try {
      global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideOwner;
      if (NativeRecovery() || local == null || local != global::COW.GameFacade.GetLocalPlayerOrObServer() ||
          global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideMatch != global::COW.GameFacade.CurrentMatch() ||
          !local.IsLocalPlayer() || local.get_IsDieing() || local.get_CurHP() <= 0) {
        EndGuidedMap(6, "guide-context-cancelled"); return;
      }
      if (HighJumpOn() || HoverModeOn() || local.GetPhysXState() > 1) {
        EndGuidedMap(6, "guide-other-motion-mode"); return;
      }
      float now = Time.get_realtimeSinceStartup();
      float dt = now - global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideTickAt;
      if (dt < 0.02f && dt >= 0f) { return; }
      if (!(dt > 0f && dt <= 0.25f) || now - global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideAt > 90f) {
        EndGuidedMap(6, "guide-clock-or-flight-timeout"); return;
      }
      Transform root = local.get_RootTransform();
      if (root == null) { EndGuidedMap(6, "guide-missing-root"); return; }
      Vector3 position = root.get_position();
      Vector3 previous = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastPos;
      if (!ValidVector(position)) { EndGuidedMap(6, "guide-invalid-position"); return; }
      Vector3 observed = new Vector3((position.x - previous.x) / dt, (position.y - previous.y) / dt, (position.z - previous.z) / dt);
      float moved = Mathf.Sqrt(observed.x * observed.x + observed.y * observed.y + observed.z * observed.z) * dt;
      if (moved > 150f * dt + 3f) { EndGuidedMap(6, "guide-position-correction"); return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideTickAt = now;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastPos = position;
      Vector3 target = global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapTarget;
      float dx = target.x - position.x, dz = target.z - position.z;
      float distance = Mathf.Sqrt(dx * dx + dz * dz);
      int state = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideState;
      float cruise = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideCruiseY;
      float metric = distance;
      Vector3 desired = new Vector3();
      if (state == 1) {
        metric = cruise - position.y;
        desired.x = Mathf.Clamp((global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOrigin.x - position.x) * 2f, -5f, 5f);
        desired.z = Mathf.Clamp((global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapOrigin.z - position.z) * 2f, -5f, 5f);
        desired.y = Mathf.Clamp(metric * 1.5f, -5f, 15f);
        if (metric < 1f && observed.y < 3f) { state = 2; }
      } else if (state == 2 || state == 3) {
        float speed = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideSpeed;
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HitFlyMapTarget = target;
        desired.x = Mathf.Clamp(dx * 2f, -3f, 3f); desired.z = Mathf.Clamp(dz * 2f, -3f, 3f);
        float height = position.y - target.y;
        metric = height;
        desired.y = -Mathf.Clamp(height * 1.5f, 0.5f, 6f);
        if (local.IsGrounded() && distance < 1f && height > -1f && height < 1.5f) {
          if (global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideGroundAt == 0f) {
            global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideGroundAt = now;
          } else if (now - global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideGroundAt >= 0.2f) {
            EndGuidedMap(5, "guide-landed-client-observation"); return;
          }
        } else { global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideGroundAt = 0f; }
      }
      if (state != global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideState) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideProgressAt = now;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideMetric = 100000f;
      } else if (metric < global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideMetric - 0.25f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideProgressAt = now;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideMetric = metric;
      }
      if (now - global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideProgressAt > 3f) {
        EndGuidedMap(6, "guide-no-progress"); return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideState = state;
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
      Vector3 own = global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastForce;
      if (!ValidVector(baseline) || !ValidVector(forces)) { EndGuidedMap(6, "guide-invalid-velocity"); return; }
      Vector3 command = LimitGuidedVector(new Vector3(desired.x - baseline.x - forces.x + own.x,
          desired.y - baseline.y - forces.y + own.y, desired.z - baseline.z - forces.z + own.z), 100f);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideForceOwned = true;
      local.SetForceMoveVelocity(command, GuidedMapForceKey);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFGuideLastForce = command;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7HFMapReason = "guided-force-trial-state" + state.ToString();
    } catch (System.Exception error) { EndGuidedMap(7, "guide-tick-error: " + error.Message); }
  }
#endif

  private static void UpdateCombat() {
    if (Event.get_current().get_type() != 7) { return; }
    try {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7CombatError = "";
      bool recovery = NativeRecovery();
      bool wantCamera = PlayerPrefs.GetString("OKMOD.Misc.Camera90", "") == "X2";
      Camera camera = Camera.get_main();
      if (camera != null) {
        if (wantCamera) {
          if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7CamFovApplied) {
            global::COW.GamePlay.SceneEditBoxSelectTool.R7CamFovOld = camera.get_fieldOfView();
            global::COW.GamePlay.SceneEditBoxSelectTool.R7CamFovApplied = true;
          }
          camera.set_fieldOfView(ReadFloatSetting("OKMOD.Misc.CameraFov", 90, 60, 120));
        } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7CamFovApplied) {
          camera.set_fieldOfView(global::COW.GamePlay.SceneEditBoxSelectTool.R7CamFovOld);
          global::COW.GamePlay.SceneEditBoxSelectTool.R7CamFovApplied = false;
        }
      }
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      if (local == null) { return; }
      if (!recovery && PlayerPrefs.GetString("OKMOD.Combat.FastSwap", "") == "X2") {
        local.ResetSwapWeaponTime();
      }
      bool wantNoReload = !recovery && PlayerPrefs.GetString("OKMOD.Combat.NoReload", "") == "X2";
      global::COW.GamePlay.PlayerAttributes attributes = local.get_Attributes();
      if (attributes == null) { return; }
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadOwner != attributes) {
        global::COW.GamePlay.PlayerAttributes previous =
            global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadOwner;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadApplied && previous != null) {
          previous.ShootNoReload = global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadOld;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadOwner = attributes;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadApplied = false;
      }
      if (wantNoReload) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadApplied) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadOld = attributes.ShootNoReload;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadApplied = true;
        }
        attributes.ShootNoReload = true;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadApplied) {
        attributes.ShootNoReload = global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadOld;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadApplied = false;
      }
      // Speed is a slider now: 1.0 = off, up to 2.0. RunSpeedUpScale feeds
      // PlayerAttributes.GetSpeedScaleBySpeedType, so it also raises the
      // airborne steering speed (air movement uses GetSpeedByType too).
      // The old X2 toggle is migrated once to 1.3x.
      if (PlayerPrefs.GetString("OKMOD.Move.Speed", "") == "X2") {
        PlayerPrefs.SetString("OKMOD.Move.Speed", "");
        PlayerPrefs.SetFloat("OKMOD.Move.SpeedScale.Float", 1.3f);
      }
      float speedScale = ReadSpeedScale();
      bool wantSpeed = !recovery && speedScale > 1.01f;
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedOwner != attributes) {
        global::COW.GamePlay.PlayerAttributes previousSpeed =
            global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedOwner;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedApplied && previousSpeed != null) {
          previousSpeed.RunSpeedUpScale = global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedOld;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedOwner = attributes;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedApplied = false;
      }
      if (wantSpeed) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedApplied) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedOld = attributes.RunSpeedUpScale;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedApplied = true;
        }
        attributes.RunSpeedUpScale = speedScale;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedApplied) {
        attributes.RunSpeedUpScale = global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedOld;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedApplied = false;
      }
      bool wantNoRecoil = !recovery && PlayerPrefs.GetString("OKMOD.Combat.NoRecoil", "") == "X2";
      COW.WeaponGunFeelProfileData profile = null;
      global::COW.GamePlay.HBIBDMMOOOK weapon = local.GetActiveWeapon();
      if (weapon != null) { profile = weapon.ILGIAJHNNDP(); }
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelOwner != profile) {
        COW.WeaponGunFeelProfileData previousProfile =
            global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelOwner;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelApplied && previousProfile != null) {
          previousProfile.enableRecoil = global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelRecoilOld;
          previousProfile.enableVisualRecoil = global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelVisualOld;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelOwner = profile;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelApplied = false;
      }
      if (wantNoRecoil && profile != null) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelApplied) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelRecoilOld = profile.enableRecoil;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelVisualOld = profile.enableVisualRecoil;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelApplied = true;
        }
        profile.enableRecoil = false;
        profile.enableVisualRecoil = false;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelApplied && profile != null) {
        profile.enableRecoil = global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelRecoilOld;
        profile.enableVisualRecoil = global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelVisualOld;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7GunFeelApplied = false;
      }
      bool wantRapid = !recovery && PlayerPrefs.GetString("OKMOD.Combat.RapidFire", "") == "X2";
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidOwner != attributes) {
        global::COW.GamePlay.PlayerAttributes previousRapid =
            global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidOwner;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidApplied && previousRapid != null) {
          previousRapid.FireIntervalScale = global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidOld;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidOwner = attributes;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidApplied = false;
      }
      if (wantRapid) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidApplied) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidOld = attributes.FireIntervalScale;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidApplied = true;
        }
        attributes.FireIntervalScale = 0.5f;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidApplied) {
        attributes.FireIntervalScale = global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidOld;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidApplied = false;
      }
      bool wantHighFps = PlayerPrefs.GetString("OKMOD.Misc.HighFPS", "") == "X2";
      if (wantHighFps) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7FpsApplied) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7FpsOld = COW.GameSettingData.m_IsHighFramerate;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7FpsSmoothOld = COW.GameSettingData.m_SmoothHighFrame;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7FpsApplied = true;
        }
        COW.GameSettingData.m_IsHighFramerate = true;
        COW.GameSettingData.m_SmoothHighFrame = true;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7FpsApplied) {
        COW.GameSettingData.m_IsHighFramerate = global::COW.GamePlay.SceneEditBoxSelectTool.R7FpsOld;
        COW.GameSettingData.m_SmoothHighFrame = global::COW.GamePlay.SceneEditBoxSelectTool.R7FpsSmoothOld;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7FpsApplied = false;
      }
      bool wantAmmo = !recovery && PlayerPrefs.GetString("OKMOD.Combat.InfiniteAmmo", "") == "X2";
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoOwner != attributes) {
        global::COW.GamePlay.PlayerAttributes previousAmmo =
            global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoOwner;
        if (global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoApplied && previousAmmo != null) {
          previousAmmo.IsAmmoFree = global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoOld;
        }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoOwner = attributes;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoApplied = false;
      }
      if (wantAmmo) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoApplied) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoOld = attributes.IsAmmoFree;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoApplied = true;
        }
        attributes.IsAmmoFree = true;
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoApplied) {
        attributes.IsAmmoFree = global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoOld;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoApplied = false;
      }
      bool wantAutoSwap = !recovery && PlayerPrefs.GetString("OKMOD.Combat.AutoSwap", "") == "X2";
      if (wantAutoSwap) {
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7AutoSwapApplied) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AutoSwapOld = COW.GameSettingData.GetAutoSwitchWeapon();
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AutoSwapApplied = true;
        }
        COW.GameSettingData.SetAutoSwitchWeapon(true);
        float swapNow = Time.get_realtimeSinceStartup();
        if (local.IsFiring() && swapNow >= global::COW.GamePlay.SceneEditBoxSelectTool.R7AutoSwapNext) {
          local.AutoSwitchWeapon(false, false);
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AutoSwapNext = swapNow + 0.40f;
        }
      } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7AutoSwapApplied) {
        COW.GameSettingData.SetAutoSwitchWeapon(global::COW.GamePlay.SceneEditBoxSelectTool.R7AutoSwapOld);
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AutoSwapApplied = false;
      }
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7CombatError = error.Message;
    }
  }

  // Hook body for PlayerAttributes.GetScatterRate.
  // Off: reproduce the native formula (1.0 + sighting + skill, min 0.1).
  // On: local player gets 0.1; other players keep their normal value.
  public static float ScatterRate(global::COW.GamePlay.PlayerAttributes self) {
    int calls = global::COW.GamePlay.SceneEditBoxSelectTool.R7ScatterCalls;
    if (calls < 1000000000) { global::COW.GamePlay.SceneEditBoxSelectTool.R7ScatterCalls = calls + 1; }
    if (self == null) { return 0.1f; }
    if (!NativeRecovery() && PlayerPrefs.GetString("OKMOD.Combat.NoRecoil", "") == "X2") { return 0.1f; }
    float value = 1f + self.SkillScatterRateSighting + self.SkillScatterRate;
    if (value < 0.1f) { value = 0.1f; }
    return value;
  }

  public static bool ShowDamageNum(global::COW.GamePlay.Player self) {
    return PlayerPrefs.GetString("OKMOD.Combat.HideDamage", "") != "X2";
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7GodCalls =
        global::COW.GamePlay.SceneEditBoxSelectTool.R7GodCalls + 1;
    if (Time.get_frameCount() < global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpBoostUntil) {
      int hits = global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagHits;
      if (hits < 100000) { global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagHits = hits + 1; }
      float boosted = gravity / JumpScale();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7GodLast = boosted;
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7RiseCalls =
        global::COW.GamePlay.SceneEditBoxSelectTool.R7RiseCalls + 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7RiseLast = value;
    return value;
  }

  public static float FallingGravity(global::COW.GamePlay.Player self) {
    if (NativeRecovery()) {
      return self != null && self.InSnowSlideWayDashing ? self.SnowSlideWayFallingGravity : VanillaGravity();
    }
    if (self == null) { return VanillaGravity(); }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7FallCalls =
        global::COW.GamePlay.SceneEditBoxSelectTool.R7FallCalls + 1;
    float hover = HoverValue();
    float value;
    if (hover < 9.75f) { value = -hover; }
    else if (self.InSnowSlideWayDashing) { value = self.SnowSlideWayFallingGravity; }
    else { value = VanillaGravity(); }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7FallLast = value;
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
    return PlayerPrefs.GetString("OKMOD.Move.HighJump", "") == "X2";
  }
  private static bool HoverModeOn() {
    return PlayerPrefs.GetString("OKMOD.Move.Hover", "") == "X2";
  }
  private static float JumpScale() {
    return (HighJumpOn() || HoverModeOn()) ? 3f : 1f;
  }

  private static string H(float value) {
    return ((int)(value * 100f)).ToString();
  }

  private static void RecordJump() {
    string line = "x" + H(global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpScaleAt / 100f)
        + " h" + H(global::COW.GamePlay.SceneEditBoxSelectTool.R7ApexY
            - global::COW.GamePlay.SceneEditBoxSelectTool.R7StartY)
        + " v" + H(global::COW.GamePlay.SceneEditBoxSelectTool.R7PeakCur)
        + " god" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7GodCalls
            - global::COW.GamePlay.SceneEditBoxSelectTool.R7GodBase).ToString()
        + " rise" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7RiseCalls
            - global::COW.GamePlay.SceneEditBoxSelectTool.R7RiseBase).ToString()
        + " fall" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7FallCalls
            - global::COW.GamePlay.SceneEditBoxSelectTool.R7FallBase).ToString()
        + " st" + global::COW.GamePlay.SceneEditBoxSelectTool.R7StateLast.ToString();
    int slot = global::COW.GamePlay.SceneEditBoxSelectTool.R7JIdx;
    if (slot == 0) { global::COW.GamePlay.SceneEditBoxSelectTool.R7J0 = line; }
    else if (slot == 1) { global::COW.GamePlay.SceneEditBoxSelectTool.R7J1 = line; }
    else if (slot == 2) { global::COW.GamePlay.SceneEditBoxSelectTool.R7J2 = line; }
    else if (slot == 3) { global::COW.GamePlay.SceneEditBoxSelectTool.R7J3 = line; }
    else { global::COW.GamePlay.SceneEditBoxSelectTool.R7J4 = line; }
    slot = slot + 1;
    if (slot >= 5) { slot = 0; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7JIdx = slot;
  }


  private static void UpdateJumpBoost() {
    if (NativeRecovery() || (!HighJumpOn() && !HoverModeOn())) {
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityApplied != 0) {
        global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG = global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityOld;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityApplied = 0;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpBoostUntil = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpActive = 0;
      return;
    }
    global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
    if (local == null) { return; }
    float stamp = local.get_LastStartJumpTime();
    if (stamp != global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpStamp) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpStamp = stamp;
    }
    int state = local.GetPhysXState();
    int grounded = local.IsGrounded() ? 1 : 0;
    float speedY = local.GetVelocity().y;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7StateLast = state;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7GroundedLast = grounded;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagVy = (int)(speedY * 100f);

    if (grounded == 0 && speedY > 1f &&
        global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpActive == 0) {
      // takeoff: start a new automatic log entry
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpActive = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpScaleAt = (int)(JumpScale() * 100f);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7PeakCur = 0f;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7GodBase = global::COW.GamePlay.SceneEditBoxSelectTool.R7GodCalls;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7RiseBase = global::COW.GamePlay.SceneEditBoxSelectTool.R7RiseCalls;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7FallBase = global::COW.GamePlay.SceneEditBoxSelectTool.R7FallCalls;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagHits = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpBoostUntil = Time.get_frameCount() + 30;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7StartY = local.GetAttackableCenterWS().y;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ApexY = local.GetAttackableCenterWS().y;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpEvents =
          global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpEvents + 1;
    } else if (grounded == 0) {
      if (speedY > global::COW.GamePlay.SceneEditBoxSelectTool.R7PeakCur) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7PeakCur = speedY;
      }
      float yPos = local.GetAttackableCenterWS().y;
      if (yPos > global::COW.GamePlay.SceneEditBoxSelectTool.R7ApexY) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7ApexY = yPos;
      }
    } else if (global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpActive != 0) {
      RecordJump();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpActive = 0;
    }

    int left = global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpBoostUntil - Time.get_frameCount();
    if (left < 0) { left = 0; }
    if (left > 999) { left = 999; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagArm = left;

    // Both ascent components ultimately read IPBGFJNGLOH.AMMOCNCMBBG:
    // get_CustomGravity() returns it, and JLPJMAFLADI adds it inline at
    // 0x5785104. Writing the constant scales them together, so the apex scales
    // exactly with the slider (previously only the 77% CustomGravity share moved,
    // which capped the gain at ~4.4x).
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityApplied == 0) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityOld =
          global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityApplied = 1;
    }
    float target = JumpScale();
    if (target > 1.001f && left > 0) {
      global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG =
          global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityOld / target;
    } else {
      global::COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG =
          global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityOld;
    }
  }

  private static string BuildJumpDiag() {
    return "OKMOD R12.30 DEV jump log (values x100)\n"
        + "jumpScale=" + H(JumpScale()) + " bay=" + H(HoverValue())
        + " speed=" + H(ReadSpeedScale()) + "\n"
        + "jumps=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpEvents.ToString()
        + " active=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpActive.ToString()
        + " stamp=" + ((int)global::COW.GamePlay.SceneEditBoxSelectTool.R7JumpStamp).ToString() + "\n"
        + "state=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7StateLast.ToString()
        + " grounded=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7GroundedLast.ToString()
        + " vy=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagVy.ToString()
        + " arm=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagArm.ToString() + "\n"
        + "customCalls=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7GodCalls.ToString()
        + " risingCalls=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7RiseCalls.ToString()
        + " fallingCalls=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7FallCalls.ToString() + "\n"
        + "--- history (oldest last) ---\n"
        + (global::COW.GamePlay.SceneEditBoxSelectTool.R7J0 == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.R7J0 + "\n")
        + (global::COW.GamePlay.SceneEditBoxSelectTool.R7J1 == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.R7J1 + "\n")
        + (global::COW.GamePlay.SceneEditBoxSelectTool.R7J2 == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.R7J2 + "\n")
        + (global::COW.GamePlay.SceneEditBoxSelectTool.R7J3 == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.R7J3 + "\n")
        + (global::COW.GamePlay.SceneEditBoxSelectTool.R7J4 == null ? "" : global::COW.GamePlay.SceneEditBoxSelectTool.R7J4 + "\n");
  }

  private static void JumpDiagRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    float value = PlayerPrefs.GetFloat("OKMOD.Move.JumpScale.Float", 1f);
    if (!(value >= 1f && value <= 3f)) { value = 1f; }
    if (et == 1) {
      if (new Rect(px + 20f, y, 52f, 64f).Contains(mp)) {
        value = value - 0.2f;
        if (value < 1f) { value = 1f; }
        PlayerPrefs.SetFloat("OKMOD.Move.JumpScale.Float", value);
      } else if (new Rect(px + 358f, y, 52f, 64f).Contains(mp)) {
        value = value + 0.2f;
        if (value > 3f) { value = 3f; }
        PlayerPrefs.SetFloat("OKMOD.Move.JumpScale.Float", value);
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
        "v" + global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagVy.ToString()
        + " a" + global::COW.GamePlay.SceneEditBoxSelectTool.R7DiagArm.ToString()
        + " | chạm: copy log", row);
    GUI.Label(new Rect(px + 372f, y + 10f, 40f, 48f), "+", row);
  }

  private static void JumpLogRow(int px, int py, int index, string text, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
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
    if (PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") == "X2") { return false; }
    // Migrate the old master+silent combination deterministically, including a closed menu.
    bool silent = PlayerPrefs.GetString("OKMOD.Aim.Silent", "") == "X2";
    if (silent && PlayerPrefs.GetString("OKMOD.Aim.Enabled", "") == "X2") {
      PlayerPrefs.SetString("OKMOD.Aim.Enabled", "");
    }
    return silent;
  }

  private static void SetAimKillEnabled(bool enabled) {
    PlayerPrefs.SetString("OKMOD.Aim.AimKill", enabled ? "X2" : "");
    if (enabled) {
      PlayerPrefs.SetString("OKMOD.Aim.Enabled", "");
      PlayerPrefs.SetString("OKMOD.Aim.Silent", "");
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNext = 0f;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNextHit = 0f;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetPlayer = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = "";
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "";
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = enabled ? 1 : 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLocalReason = "";
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
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int point = AimKillPointMode();
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      point++;
      if (point > 3) { point = 0; }
      PlayerPrefs.SetInt("OKMOD.AK.Point", point);
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualTarget = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualWeapon = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualAt = 0f;
  }

  private static void SampleAimKillInput() {
    bool enabled = PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") == "X2" && !NativeRecovery();
    global::COW.GamePlay.Player local = enabled ? global::COW.GameFacade.GetLocalPlayerOrObServer() : null;
    global::COW.GamePlay.JMAGGLCNGIG match = enabled ? global::COW.GameFacade.CurrentMatch() : null;
    if (local != global::COW.GamePlay.SceneEditBoxSelectTool.R7AKInputOwner ||
        match != global::COW.GamePlay.SceneEditBoxSelectTool.R7AKInputMatch) {
      ClearAimKillManualTarget();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AKInputOwner = local;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AKInputMatch = match;
    }
    bool holding = local != null && match != null && local.IsLocalPlayer() &&
        local.get_CurHP() > 0 && !local.get_IsDieing() && local.IsFiring();
    float now = Time.get_realtimeSinceStartup();
    if (holding || global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualHolding) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AKResumeAt = now + 0.15f;
    }
    if (holding != global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualHolding) {
      ClearAimKillManualTarget();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNext = 0f;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualHolding = holding;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AKBranch = !enabled ? "off" :
        (holding ? "manual-native" : (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7AKResumeAt ? "handoff" : "auto"));
    if (!holding) { ClearAimKillManualTarget(); }
  }

  private static void ApplyAimKillManualAim() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualHolding || NativeRecovery() ||
        PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") != "X2" ||
        !AimKillManualEnabled()) { return; }
    global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.R7AKInputOwner;
    global::COW.GamePlay.Player enemy = global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualTarget;
    if (local == null || enemy == null || local != global::COW.GameFacade.GetLocalPlayerOrObServer() ||
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AKInputMatch != global::COW.GameFacade.CurrentMatch() ||
        Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualAt > 0.2f ||
        !local.IsLocalPlayer() || local.get_CurHP() <= 0 || local.get_IsDieing() ||
        !local.IsFiring() || local.IsReloading() ||
        local.GetWeaponOnHand() != global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualWeapon) { ClearAimKillManualTarget(); return; }
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
    float range = global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualWeapon.OEOBPHDFJMO();
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualAims++;
  }

  private static void DrawAimKillHeader(int px, int py, Vector2 mp, int et, GUIStyle style) {
    Rect button = new Rect(px + 345f, py + 8f, 86f, 48f);
    bool enabled = PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") == "X2";
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
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogOpen = true;
    }
    GUI.set_color(new Color(0.12f, 0.10f, 0.08f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.75f, 0.3f, 1f));
    GUI.Label(new Rect(px + 30f, y + 4f, 370f, 28f), "AimKill: tạm ngừng", row);
    GUI.Label(new Rect(px + 30f, y + 32f, 370f, 28f), "Chạm xem log; tính năng khác riêng", row);
  }

  private static void AimKillStatusRow(int px, int py, int index, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int status = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus;
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCalls.ToString() + " | kết quả " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult.ToString() + " | " +
        (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillGroup == 1 ? "đầu" : "thân") +
        (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring ? " | giữ cò" : ""), row);
  }

  private static void ClearAimKillProbes() {
    global::COW.GamePlay.Player[] targets = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeTargets;
    float[] ats = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeAt;
    if (targets != null && ats != null && targets.Length == 8 && ats.Length == 8) {
      for (int i = 0; i < 8; i++) { targets[i] = null; ats[i] = 0f; }
    }
  }


  private static void RecordAimKillProbe(global::COW.GamePlay.Player enemy, int hpBefore) {
    if (enemy == null || hpBefore <= 0) { return; }
    global::COW.GamePlay.Player[] targets = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeTargets;
    int[] hps = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeHp;
    float[] ats = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeAt;
    if (targets == null || hps == null || ats == null ||
        targets.Length != 8 || hps.Length != 8 || ats.Length != 8) {
      targets = new global::COW.GamePlay.Player[8];
      hps = new int[8];
      ats = new float[8];
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeTargets = targets;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeHp = hps;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeAt = ats;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeCursor = 0;
    }
    // One observation window per target; repeated hits don't move its deadline.
    int cursor = -1;
    for (int i = 0; i < 8; i++) {
      if (ats[i] > 0f && targets[i] == enemy) { return; }
      if (ats[i] <= 0f && cursor < 0) { cursor = i; }
    }
    if (cursor < 0) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeDropped++;
      return;
    }
    targets[cursor] = enemy;
    hps[cursor] = hpBefore;
    ats[cursor] = Time.get_realtimeSinceStartup() + 0.7f;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeCursor = (cursor + 1) & 7;
    int[] manual = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeManualBaseline;
    if (manual == null || manual.Length != 8) {
      manual = new int[8];
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeManualBaseline = manual;
    }
    manual[cursor] = global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitSamples;
  }

  private static void UpdateAimKillProbes() {
    global::COW.GamePlay.Player[] targets = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeTargets;
    int[] hps = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeHp;
    float[] ats = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeAt;
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeLastBefore = hp;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeLastAfter = after;
        int[] manual = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeManualBaseline;
        if (manual != null && manual.Length == 8 &&
            manual[i] != global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitSamples) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeMixed++;
          continue;
        }
        if (after < hp) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeOk++;
        } else {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeFail++;
          if (after > hp) { global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeHealed++; }
        }
      } catch (System.Exception) { }
    }
  }

  private static void StartAimKillTest() {
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestActive = true;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestPhase = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestArmed = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestWaitStart = Time.get_realtimeSinceStartup();
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestNext = 0f;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestSavedDelay =
        ReadSetting("OKMOD.Aim.AimKillDelay", 1, 0, 3);
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestRotFlag =
        PlayerPrefs.GetString("OKMOD.Aim.AimKillRotate", "") == "X2";
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestSourceFlag =
        PlayerPrefs.GetString("OKMOD.Aim.AimKillSource", "") == "X2";
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestWeaponRange =
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillWeaponRange;
    ClearAimKillProbes();
    int[] ok = new int[3]; int[] fail = new int[3]; int[] calls = new int[3];
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestOk = ok;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestFail = fail;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestCalls = calls;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestSummary = "";
    PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay", 1);
  }

  private static void FinishAimKillTest() {
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestActive = false;
    PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay",
        global::COW.GamePlay.SceneEditBoxSelectTool.R7TestSavedDelay);
    BuildAimKillTestSummary();
    SendAimKillTestSummary();
  }

  private static void UpdateAimKillTest() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7TestActive) { return; }
    float now = Time.get_realtimeSinceStartup();
    int phase = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestPhase;
    int[] ok = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestOk;
    int[] fail = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestFail;
    int[] calls = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestCalls;
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7TestArmed) {
      if (now - global::COW.GamePlay.SceneEditBoxSelectTool.R7TestWaitStart < 10f) { return; }
      if (ok != null && fail != null && calls != null && phase >= 0 && phase < 3) {
        ok[phase] = 0; fail[phase] = 0; calls[phase] = 0;
      }
      phase = phase + 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7TestPhase = phase;
      if (phase >= 3) { FinishAimKillTest(); return; }
      ClearAimKillProbes();
      PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay", phase + 1);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7TestWaitStart = now;
      return;
    }
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7TestNext) { return; }
    if (ok != null && fail != null && calls != null && phase >= 0 && phase < 3) {
      ok[phase] = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeOk - global::COW.GamePlay.SceneEditBoxSelectTool.R7TestBaseOk;
      fail[phase] = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeFail - global::COW.GamePlay.SceneEditBoxSelectTool.R7TestBaseFail;
      calls[phase] = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCalls - global::COW.GamePlay.SceneEditBoxSelectTool.R7TestBaseCalls;
    }
    phase = phase + 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestPhase = phase;
    if (phase >= 3) { FinishAimKillTest(); return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestArmed = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestWaitStart = now;
    ClearAimKillProbes();
    PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay", phase + 1);
  }

  private static void BuildAimKillTestSummary() {
    int[] ok = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestOk;
    int[] fail = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestFail;
    int[] calls = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestCalls;
    if (ok == null || fail == null || calls == null) { return; }
    string s = "AimKill test v11.64";
    s = s + " rot=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7TestRotFlag ? "1" : "0");
    s = s + " wr=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7TestWeaponRange.ToString();
    s = s + " src=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7TestSourceFlag ? "1" : "0");
    s = s + " | 120 ok=" + ok[0].ToString() + " fail=" + fail[0].ToString() + " calls=" + calls[0].ToString();
    s = s + " | 200 ok=" + ok[1].ToString() + " fail=" + fail[1].ToString() + " calls=" + calls[1].ToString();
    s = s + " | 300 ok=" + ok[2].ToString() + " fail=" + fail[2].ToString() + " calls=" + calls[2].ToString();
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestSummary = s;
  }

  private static void SendAimKillTestSummary() {
    int[] ok = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestOk;
    int[] fail = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestFail;
    int[] calls = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestCalls;
    if (ok == null || fail == null || calls == null) { return; }
    string url = "https://okmod.sbs/aimlog?v=11.64&test=1";
    url = url + "&rot=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7TestRotFlag ? "1" : "0");
    url = url + "&wr=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7TestWeaponRange.ToString();
    url = url + "&src=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7TestSourceFlag ? "1" : "0");
    url = url + "&ok0=" + ok[0].ToString() + "&fail0=" + fail[0].ToString() + "&calls0=" + calls[0].ToString();
    url = url + "&ok1=" + ok[1].ToString() + "&fail1=" + fail[1].ToString() + "&calls1=" + calls[1].ToString();
    url = url + "&ok2=" + ok[2].ToString() + "&fail2=" + fail[2].ToString() + "&calls2=" + calls[2].ToString();
    StartLogRequest(url);
  }

  private static void StartLogRequest(string url) {
    try {
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest != null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest.Dispose();
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest = null;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest =
          new UnityEngine.Networking.UnityWebRequest(url);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogRequest.SendWebRequest();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSending = true;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSentAt = 0f;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSending = false;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = "send: " + error.Message;
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineReqA;
    if (request == null) { return; }
    try {
      if (!request.get_isDone()) { return; }
      long code = request.get_responseCode();
      string error = request.get_error();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineHttp = code;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineError = error == null ? "" : error;
      if (code >= 200 && code < 300 && (error == null || error.Length == 0)) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineState = 2;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineCount =
            global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineCount + 1;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineState = -2;
      }
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineState = -2;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineError = error.Message;
    }
    try { request.Dispose(); } catch (System.Exception) { }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineReqA = null;
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
      global::COW.GamePlay.SceneEditBoxSelectTool.R7CapBuf =
          global::COW.GamePlay.SceneEditBoxSelectTool.R7CapBuf
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
    string error = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError;
    if (error == null) { error = ""; }
    string log = "OKMOD R12.30 DEV AimKill log";
    log = log + "\n" + HitFlyMapLog();
    log = log + "\nmode=split-auto/manual reference=BS4484/PC aimkill="
        + (PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") == "X2" ? "on" : "off");
    log = log + "\nmanualSamples=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitSamples.ToString()
        + " manualAt=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitAt.ToString();
    log = log + "\nmanualHit=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitSnapshot;
    log = log + "\nmanualError=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitError;
    log = log + "\nrecovery=" + (NativeRecovery() ? "on" : "off")
        + " speedApplied=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7SpeedApplied ? "1" : "0")
        + " gravityApplied=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7GravityApplied.ToString();
    log = log + "\nspeedSetting=" + ReadSpeedScale().ToString()
        + " rapidApplied=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7RapidApplied ? "1" : "0")
        + " ammoApplied=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7AmmoApplied ? "1" : "0")
        + " reloadApplied=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7NoReloadApplied ? "1" : "0");
    log = log + "\ncombatError=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7CombatError;
    log = log + "\nstop=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop.ToString();
    log = log + "\nstatus=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus.ToString();
    log = log + "\nlocalReason=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLocalReason;
    log = log + "\nlastDispatchAt=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLastDispatchAt.ToString()
        + " dataId=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillDataId.ToString();
    log = log + "\nimmediateHp=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillImmediateBefore.ToString()
        + "->" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillImmediateAfter.ToString()
        + " invincibleSkips=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillInvincibleSkips.ToString();
    log = log + "\ntarget=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetId.ToString();
    log = log + "\ndist=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetDist.ToString();
    log = log + "\nzeros=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillZeros.ToString();
    log = log + "\nreloads=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillReloads.ToString();
    log = log + "\nammoFill=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillAmmoFills.ToString();
    log = log + "\nammoFail=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillAmmoFails.ToString();
    log = log + "\ncalls=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCalls.ToString();
    log = log + "\nresult=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult.ToString();
    log = log + "\npc=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC.ToString();
    log = log + "\nchainStage=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage.ToString()
        + " dispatched=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillDispatches.ToString();
    log = log + "\nbaseDamage=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillBaseDamage.ToString()
        + " checks=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCheckCount.ToString()
        + " startResult=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStartResult.ToString();
    log = log + "\npath=LHNJLFOINDO explicitTakeDamage=0 engineInternals=unobserved";
    float cooldownRemaining = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimCooldownUntil - Time.get_realtimeSinceStartup();
    if (cooldownRemaining < 0f) { cooldownRemaining = 0f; }
    log = log + "\ncooldownRemainingMs=" + ((int)(cooldownRemaining * 1000f)).ToString();
    log = log + "\nshotStarts=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotStarts.ToString()
        + " shotStops=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotStops.ToString()
        + " shotFault=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotFaulted ? "1" : "0");
    log = log + "\ncleanupError=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotCleanupError;
    log = log + "\nsync108=disabled-preset"
        + " attempts=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Attempts.ToString()
        + " sent=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Sent.ToString()
        + " failed=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Failed.ToString()
        + " skipped=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Skipped.ToString();
    log = log + "\nsync108State=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State
        + " uniqueId=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Weapon.ToString()
        + " error=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Error;
    log = log + "\nshotAborted=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotAborted.ToString()
        + " probeMixed=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeMixed.ToString();
    log = log + "\ncadenceMs user=" + H(global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillUserDelay * 10f)
        + " native=" + H(global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNativeDelay * 10f)
        + " effective=" + H(global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillEffectiveDelay * 10f)
        + " lastGap=" + H(global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLastGap * 10f);
    log = log + "\ncadencePolicy=selected-delay nativeIsDiagnostic=1";
    log = log + "\nworkMs call=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCallMs.ToString()
        + " scan=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillScanMs.ToString()
        + " probeDropped=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeDropped.ToString();
    log = log + "\nscanned=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillScanned.ToString()
        + " blocked=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillBlocked.ToString()
        + " manualSkips=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillManualSkips.ToString()
        + " effectiveRange=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillEffectiveRange.ToString();
    log = log + "\nchainError=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError;
    log = log + "\ngroup=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillGroup.ToString();
    log = log + "\nfire=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring ? "held" : "released");
    log = log + "\nweapon=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillWeaponId.ToString();
    log = log + "\npreset=head-500m-360-direct-80ms delayMs=80";
    log = log + "\nrange=" + AimKillRange().ToString();
    log = log + "\npointMode=" + AimKillPointMode().ToString();
    log = log + "\nakConfig fov=" + AimKillFov().ToString()
        + " fovLimit=" + (AimKillFovLimited() ? "1" : "0")
        + " manualSmooth=" + AimKillManualSmooth().ToString();
    log = log + "\ninputBranch=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AKBranch
        + " manualAims=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualAims.ToString()
        + " autoEnabled=" + (AimKillAutoEnabled() ? "1" : "0")
        + " manualEnabled=" + (AimKillManualEnabled() ? "1" : "0");
    log = log + "\nmanualAimError=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualError;
    log = log + "\nweaponRange=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillWeaponRange.ToString();
    log = log + "\nrotate=" + (PlayerPrefs.GetString("OKMOD.Aim.AimKillRotate", "") == "X2" ? "1" : "0");
    log = log + "\nsource=" + (PlayerPrefs.GetString("OKMOD.Aim.AimKillSource", "") == "X2" ? "1" : "0");
    log = log + "\nprobeOk=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeOk.ToString();
    log = log + "\nprobeFail=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeFail.ToString();
    log = log + "\nprobeHealed=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeHealed.ToString()
        + " hpObservationOnly=1";
    log = log + "\nprobeLast=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeLastBefore.ToString()
        + "->" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeLastAfter.ToString();
    string onlineTag = global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineTag;
    log = log + "\nonline=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineState.ToString()
        + " sent=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineCount.ToString()
        + " tag=" + (onlineTag == null ? "" : onlineTag);
    log = log + "\nhttp=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineHttp.ToString()
        + " onlineError=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7OnlineError;
    string testSummary = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestSummary;
    if (testSummary != null && testSummary.Length > 0) { log = log + "\n" + testSummary; }
    log = log + "\nerror=" + error;
    log = log + "\ntime=" + Time.get_realtimeSinceStartup().ToString();
    return log;
  }

  private static void CopyAimKillLog() {
    try {
      string snapshot = BuildAimKillLog();
      UnityEngine.GUIUtility.systemCopyBuffer = snapshot;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk =
          UnityEngine.GUIUtility.systemCopyBuffer == snapshot;
    } catch (System.Exception) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk = false;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyAt = Time.get_realtimeSinceStartup();
  }

  private static void DrawAimLogActions(int px, int py, Vector2 mp, int et, GUIStyle row) {
    Rect log = new Rect(px + 440f, py + 468f, 106f, 48f);
    Rect copy = new Rect(px + 554f, py + 468f, 106f, 48f);
    Rect send = new Rect(px + 668f, py + 468f, 112f, 48f);
    if (et == 1) {
      if (log.Contains(mp)) { global::COW.GamePlay.SceneEditBoxSelectTool.R7LogOpen = true; }
      if (copy.Contains(mp)) { CopyAimKillLog(); }
    }
    GUI.set_color(new Color(0.10f, 0.22f, 0.36f, 1f));
    GUI.DrawTexture(log, Texture2D.get_whiteTexture());
    GUI.DrawTexture(copy, Texture2D.get_whiteTexture());
    GUI.DrawTexture(send, Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(1f, 0.85f, 0.1f, 1f));
    bool recent = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyAt > 0f &&
        Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyAt < 2f;
    string copyLabel = "COPY";
    if (recent) { copyLabel = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk ? "ĐÃ COPY" : "LỖI COPY"; }
    GUI.Label(new Rect(px + 448f, py + 472f, 98f, 44f), "LOG", row);
    GUI.Label(new Rect(px + 560f, py + 472f, 100f, 44f), copyLabel, row);
    GUI.Label(new Rect(px + 676f, py + 472f, 104f, 44f), "GỬI", row);
  }

  private static void AimKillCopyRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    bool copyRecent = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyAt > 0f &&
        Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyAt < 2f;
    bool copied = copyRecent && global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk;
    bool copyFailed = copyRecent && !global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk;
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      try {
        string log = BuildAimKillLog();
        UnityEngine.GUIUtility.systemCopyBuffer = log;
        string check = UnityEngine.GUIUtility.systemCopyBuffer;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk = check == log;
        if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = "copy readback mismatch";
        }
      } catch (System.Exception error) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyOk = false;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = "copy: " + error.Message;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCopyAt = Time.get_realtimeSinceStartup();
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
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LogOpen = true;
    }
    GUI.set_color(new Color(0.10f, 0.12f, 0.20f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.7f, 0.8f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 6f, 360f, 28f), "Xem log", row);
    GUI.Label(new Rect(px + 32f, y + 32f, 360f, 24f), "Mở toàn màn hình để đọc / chụp", row);
  }

  private static void AimKillSendLogRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    bool sending = global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSending;
    bool sendRecent = !sending && global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSentAt > 0f &&
        Time.get_realtimeSinceStartup() - global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSentAt < 3f;
    bool sent = sendRecent && global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSendOk;
    bool sendFailed = sendRecent && !global::COW.GamePlay.SceneEditBoxSelectTool.R7LogSendOk;
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      string url = "https://okmod.sbs/aimlog?v=11.57";
        url = url + "&stop=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop.ToString();
        url = url + "&status=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus.ToString();
        url = url + "&target=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetId.ToString();
        url = url + "&dist=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetDist.ToString();
        url = url + "&zeros=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillZeros.ToString();
        url = url + "&reloads=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillReloads.ToString();
        url = url + "&af=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillAmmoFills.ToString();
        url = url + "&ax=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillAmmoFails.ToString();
        url = url + "&calls=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCalls.ToString();
        url = url + "&res=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult.ToString();
        url = url + "&group=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillGroup.ToString();
        url = url + "&fire=" + (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring ? "1" : "0");
        url = url + "&weapon=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillWeaponId.ToString();
        url = url + "&delay=" + ReadSetting("OKMOD.Aim.AimKillDelay", 1, 0, 3).ToString();
        url = url + "&range=" + ReadFloatSetting("OKMOD.Aim.Range", 150, 10, 500).ToString();
        url = url + "&point=" + ReadSetting("OKMOD.Aim.Point", 0, 0, 5).ToString();
        url = url + "&wr=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillWeaponRange.ToString();
        url = url + "&rot=" + (PlayerPrefs.GetString("OKMOD.Aim.AimKillRotate", "") == "X2" ? "1" : "0");
        url = url + "&pok=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeOk.ToString();
        url = url + "&pfail=" + global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeFail.ToString();
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
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    bool active = global::COW.GamePlay.SceneEditBoxSelectTool.R7TestActive;
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      if (!active) { StartAimKillTest(); }
    }
    GUI.set_color(new Color(0.14f, 0.10f, 0.22f, 0.95f));
    GUI.DrawTexture(new Rect(px + 20f, y, 390f, 64f), Texture2D.get_whiteTexture());
    GUI.set_color(new Color(1f, 1f, 1f, 1f));
    row.get_normal().set_textColor(new Color(0.85f, 0.7f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y + 6f, 360f, 28f),
        active ? ((global::COW.GamePlay.SceneEditBoxSelectTool.R7TestArmed
                   ? ("Đang test " + (global::COW.GamePlay.SceneEditBoxSelectTool.R7TestPhase + 1).ToString() + "/3...")
                   : "Đang chờ mục tiêu..."))
               : "Test tự động (120/200/300)", row);
    GUI.Label(new Rect(px + 32f, y + 32f, 360f, 24f), active ? "Mỗi phase bắt đầu khi có hit, 5 giây" : "3 mức delay, mỗi mức 5s sau hit đầu", row);
  }

  private static void AimKillDebugRow(int px, int py, int index, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    string error = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError;
    if (error == null || error.Length == 0) { error = "-"; }
    row.get_normal().set_textColor(new Color(0.7f, 0.9f, 1f, 1f));
    GUI.Label(new Rect(px + 32f, y, 370f, 30f), "T " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetId.ToString() + " | " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetDist.ToString() + "m | 0:" +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillZeros.ToString() + " | stop " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop.ToString(), row);
    GUI.Label(new Rect(px + 32f, y + 29f, 370f, 30f), "nạp " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillReloads.ToString() + " | ammo " +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillAmmoFills.ToString() + "/" +
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillAmmoFails.ToString() + " | " + error, row);
  }

  private static void AimKillDelayRow(int px, int py, int index, Vector2 mp, int et, GUIStyle row) {
    float y = py + 80f + (float)index * 72f - global::COW.GamePlay.SceneEditBoxSelectTool.OkUiScroll;
    if (y < py + 80f || y + 64f > py + 584f) { return; }
    int mode = ReadSetting("OKMOD.Aim.AimKillDelay", 1, 0, 4);
    if (et == 1 && new Rect(px + 20f, y, 390f, 64f).Contains(mp)) {
      mode = mode + 1;
      if (mode > 4) { mode = 0; }
      PlayerPrefs.SetInt("OKMOD.Aim.AimKillDelay", mode);
    }
    string label = "AimKill delay: 200 ms";
    if (mode == 0) { label = "AimKill delay: 120 ms"; }
    if (mode == 2) { label = "AimKill delay: 300 ms"; }
    if (mode == 3) { label = "AimKill delay: 500 ms"; }
    if (mode == 4) { label = "AimKill delay: 80 ms"; }
    label = label + " / lịch " + ((int)(global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillEffectiveDelay * 1000f)).ToString() + " ms";
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillBaseDamage = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCheckCount = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "";
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 1;
    try {
      global::COW.GamePlay.PlayerAttributes targetAttributes = enemy.get_Attributes();
      if (targetAttributes == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "target attributes unavailable";
        return false;
      }
      if (targetAttributes.IsInvincible()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillInvincibleSkips++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "target invincible; no damage submitted";
        return false;
      }
      global::COW.GamePlay.PlayerNetwork target = enemy as global::COW.GamePlay.PlayerNetwork;
      if (target == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "target is not PlayerNetwork";
        return false;
      }
      global::COW.GamePlay.IOKEALMHBKN weaponData = weapon.PIILKNEHNLB;
      if (weaponData == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "weapon data missing";
        return false;
      }
      // Keep native trigger/ammo state untouched. Do not combine the engine's
      // hit-effect entry with the explicit damage submission below.
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStartResult = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 4;
      int damage = weapon.KBIFKMJODNA();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillBaseDamage = damage;
      uint weaponId = weapon.MJCENIGKEPH();
      global::COW.GamePlay.IJHHJLDNMGH definition = weaponData.MHMGCLILKGJ;
      if (definition == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "weapon definition missing";
        return false;
      }
      uint dataId = definition.GetId();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillDataId = dataId;
      if (damage <= 0 || weaponId > 2147483647u || dataId > 2147483647u) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "invalid base damage / weapon id";
        return false;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillWeaponId = (int)weaponId;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 5;
      global::COW.GamePlay.OGKFEMJNCGM attackable = local.JGIEOGCGFEP(hit);
      if (attackable == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "JGIEOGCGFEP returned null";
        return false;
      }
      if (attackable.get_gameObject() != enemy.get_gameObject()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "hit resolves to a different target";
        return false;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 6;
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
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 7;
      System.Collections.Generic.List<float> checks = weapon.NPLMKLBKJPE(attackable, collider, false, info);
      if (checks == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "NPLMKLBKJPE returned null";
        return false;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCheckCount = checks.Count;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 8;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillImmediateBefore = enemy.get_CurHP();
      int result = target.TakeDamage(damage, playerId, info, (int)weaponId,
          origin, hitPoint, checks, dynamicInfo, 0u);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillImmediateAfter = enemy.get_CurHP();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult = result;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillDispatches++;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLastDispatchAt = Time.get_realtimeSinceStartup();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 9;
      return true;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = error.Message;
      return false;
    }
  }

  private static bool DispatchEngineHit(global::COW.GamePlay.Player enemy,
      global::COW.GamePlay.HBIBDMMOOOK weapon, global::COW.GamePlay.CGKJLKPMGDJ hit) {
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 1;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "";
    // Engine-owned intermediates are not observed by this caller.
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCheckCount = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillDataId = 0;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillBaseDamage = 0;
    try {
      global::COW.GamePlay.PlayerAttributes attributes = enemy.get_Attributes();
      if (attributes == null || attributes.IsInvincible()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillInvincibleSkips++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "target attributes missing / invincible";
        return false;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillWeaponId = (int)weapon.MJCENIGKEPH();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillBaseDamage = weapon.KBIFKMJODNA();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillImmediateBefore = enemy.get_CurHP();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 3;
      float callStarted = Time.get_realtimeSinceStartup();
      int result = weapon.LHNJLFOINDO(hit);
      float completed = Time.get_realtimeSinceStartup();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCallMs = (completed - callStarted) * 1000f;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult = result;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStartResult = result;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillImmediateAfter = enemy.get_CurHP();
      float previous = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLastDispatchAt;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLastGap = previous > 0f ? completed - previous : 0f;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLastDispatchAt = completed;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimCadenceLast = completed;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillDispatches++;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 9;
      return true;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = error.Message;
      return false;
    }
  }

  private static void TrySyncInventory108(global::COW.GamePlay.Player local,
      global::COW.GamePlay.HBIBDMMOOOK weapon, uint expectedUniqueId) {
    if (PlayerPrefs.GetString("OKMOD.Aim.Sync108", "") != "X2") {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "off";
      return;
    }
    try {
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotFaulted ||
          NativeRecovery() || PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") != "X2" ||
          local == null || weapon == null || global::COW.GameFacade.CurrentMatch() == null ||
          global::COW.GameFacade.GetLocalPlayerOrObServer() != local ||
          !local.IsLocalPlayer() || local.get_CurHP() <= 0 || local.get_IsDieing() ||
          local.IsFiring() || local.IsReloading() || local.GetWeaponOnHand() != weapon) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Skipped++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "stale-or-busy";
        return;
      }
      uint playerId = global::COW.GamePlay.BEADLMGGGGL.DEIPJDIGKIP(local.get_PlayerID());
      uint uniqueId = weapon.ODFDGMOCJLL;
      if (playerId == 0u || uniqueId == 0u || uniqueId != expectedUniqueId) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Skipped++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "zero-id";
        return;
      }
      float now = Time.get_realtimeSinceStartup();
      if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Next) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Skipped++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "rate-limited";
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Next = now + 0.25f;
      global::message.EOBFCLNNIJH packet = new global::message.EOBFCLNNIJH();
      packet.BEADLMGGGGL = playerId;
      packet.FIKKBFDEJOK = uniqueId;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Attempts++;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Weapon = uniqueId;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Error = "";
      bool sent = global::COW.GameFacade.Send(108u, packet, (byte)2, false);
      if (sent) { global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Sent++; }
      else { global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Failed++; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = sent ? "send-returned-true" : "send-returned-false";
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Failed++;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Error = error.Message;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "exception";
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
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Skipped++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "stale-or-missing";
        return;
      }
      uint playerId = global::COW.GamePlay.BEADLMGGGGL.DEIPJDIGKIP(local.get_PlayerID());
      uint weaponId = weapon.MJCENIGKEPH();
      uint uniqueId = weapon.ODFDGMOCJLL;
      uint weaponField = weaponId == 1u ? 0u : uniqueId;
      // 108 is a reload message. Sending it after every 80 ms shot makes the
      // next shot land in reload, so sync once per weapon instance only.
      if (weaponField == global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Weapon) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "already-synced";
        return;
      }
      if (playerId == 0u) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Skipped++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "zero-player-id";
        return;
      }
      global::message.EOBFCLNNIJH packet = new global::message.EOBFCLNNIJH();
      packet.BEADLMGGGGL = playerId;
      packet.FIKKBFDEJOK = weaponField;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Attempts++;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Error = "";
      bool sent = global::COW.GameFacade.Send(108u, packet, (byte)2, false);
      if (sent) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Weapon = weaponField;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Sent++;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Failed++;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State =
          sent ? "ref108-after-sent" : "ref108-after-false";
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Failed++;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108Error = error.Message;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7Sync108State = "ref108-after-exception";
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotCleanupError = "";
    try {
      if (local.IsFiring() || local.IsReloading() || local.GetWeaponOnHand() != weapon) { return false; }
      global::COW.GamePlay.PlayerAttributes attributes = enemy.get_Attributes();
      if (attributes == null || attributes.IsInvincible()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillInvincibleSkips++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "target attributes missing / invincible";
        return false;
      }
      if (!ClearAimKillSight(origin, point, enemy.get_RootTransform(), local.get_RootTransform())) { return false; }
      original = local.GetAimRotation();
      applied = Quaternion.LookRotation(point + new Vector3(0f, 0.1f, 0f) - origin);
      aimed = true;
      local.SetAimRotation(applied, false);
      // Mark ownership before a native start that could partially act then throw.
      owned = true;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring = true;
      local.StartFiring(weapon);
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotStarts++;
      if (local.GetWeaponOnHand() != weapon || !local.IsLocalPlayer() ||
          local.get_CurHP() <= 0 || local.get_IsDieing() ||
          enemy.get_CurHP() <= 0 || enemy.get_IsDieing() ||
          !ClearAimKillSight(origin, point, enemy.get_RootTransform(), local.get_RootTransform())) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotAborted++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "native start changed weapon/target/LOS; hit skipped";
        return false;
      }
      submitted = DispatchEngineHit(enemy, weapon, hit);
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = error.Message;
    } finally {
      if (owned) {
        try {
          local.StopFire(weapon);
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotStops++;
        } catch (System.Exception error) {
          cleaned = false;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotCleanupError = "stop: " + error.Message;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotFaulted = true;
        }
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring = false;
      if (aimed) {
        try {
          if (Quaternion.Angle(local.GetAimRotation(), applied) < 0.1f) {
            local.SetAimRotation(original, false);
          }
        } catch (System.Exception error) {
          cleaned = false;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotCleanupError =
              global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotCleanupError + " restore: " + error.Message;
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotFaulted = true;
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
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillGroup = (int)hit.DKPDBLLJEAA;
    hit.EFMBNPNFGHM = false; hit.CEOKOILBODN = false; hit.CLOLKHNMNEK = 0;
    // BS4484 wipes the hit object from offset 0x10 before filling it, which also
    // clears the two collections the constructor creates (0x88/0x90).
    hit.MGBCGEBLKJB = null; hit.ANJGNHKHOKO = null;
    try {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCalls++;
      int probeHp = 0;
      try { probeHp = enemy.get_CurHP(); } catch (System.Exception) { }
      // Both supplied references name LHNJLFOINDO as the engine-hit entry.
      // Do not combine it with a second explicit damage submission.
      if (!ExecuteAimKillShot(local, weapon, enemy, hit, origin, hitPoint)) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 5;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 5;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError =
            "stage " + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage.ToString()
            + ": " + global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError;
        return false;
      }
      int result = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult;
      RecordAimKillProbe(enemy, probeHp);
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7TestActive &&
          !global::COW.GamePlay.SceneEditBoxSelectTool.R7TestArmed) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7TestArmed = true;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7TestNext = Time.get_realtimeSinceStartup() + 5f;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7TestBaseOk = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeOk;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7TestBaseFail = global::COW.GamePlay.SceneEditBoxSelectTool.R7ProbeFail;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7TestBaseCalls = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillCalls;
        ClearAimKillProbes();
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLastTarget = Time.get_realtimeSinceStartup();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillResult = result;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetPlayer = enemy;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetId =
          global::COW.GamePlay.BEADLMGGGGL.DEIPJDIGKIP(enemy.get_PlayerID());
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetDist = hitDistance;
      if (result <= 0) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillZeros++;
      } else {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillZeros = 0;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 4;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = "";
      return true;
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = error.Message;
      return false;
    }
  }

  // Releases the trigger once the post-shot window elapses.
  private static void ServiceAimFire() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring) { return; }
    if (Time.get_realtimeSinceStartup() < global::COW.GamePlay.SceneEditBoxSelectTool.R7AimFireUntil) {
      return;
    }
    ReleaseAimKillFire();
  }

  private static float AimKillDelaySeconds() {
    return 0.08f;
  }

  private static bool NativeRecovery() {
    return PlayerPrefs.GetString("OKMOD.Diagnostic.NativeRecovery", "") == "X2";
  }

  private static float AimKillWeaponDelay(global::COW.GamePlay.HBIBDMMOOOK weapon) {
    float user = AimKillDelaySeconds();
    float native = -1f;
    // Native cadence is diagnostic only, not a gate on direct hit attempts.
    try {
      native = weapon.KENANOGLPJO() ? weapon.BDHMJJIADFG() : weapon.LDKCJEMDAMC();
      if (!(native > 0f && native < 60f)) { native = -1f; }
    } catch (System.Exception) { native = -1f; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillUserDelay = user;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNativeDelay = native;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillEffectiveDelay = user;
    return user;
  }

  private static void UpdateAimKill() {
    ReleaseAimKillAmmo();
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimFireUntil = 0f;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7TestActive = false;
    try { SampleAimKillInput(); }
    catch (System.Exception error) {
      ClearAimKillManualTarget();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AKBranch = "input-error";
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualError = error.Message;
      return;
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimShotFaulted) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "shot cleanup failed; restart game before retry";
      return;
    }
    if (NativeRecovery()) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillRecoveryWasOn = true;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 8;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 8;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "paused by recovery switch";
      return;
    }
    if (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillRecoveryWasOn) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillRecoveryWasOn = false;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "";
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = "";
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainStage = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNext = 0f;
      ClearAimKillProbes();
    }
    UpdateAimKillLegacy();
    try {
      ApplyAimKillManualAim();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualError = "";
    } catch (System.Exception error) {
      ClearAimKillManualTarget();
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualError = error.Message;
    }
  }

  private static void UpdateAimKillLegacy() {
    if (PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") != "X2") {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillTargetPlayer = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 1;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC = 0;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillChainError = "";
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 0; return;
    }
    ResolveAimMode();
    float now = Time.get_realtimeSinceStartup();
    if (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNext) { return; }
    // Install the cadence before native calls; an exception must not retry per frame.
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNext = now + 0.05f;
    try {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 1;
      global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
      global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
      if (match == null || local == null || !local.IsLocalPlayer() || local.get_CurHP() <= 0 || local.get_IsDieing()) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 2;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillPC = 0;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget = null;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLocalReason = match == null ? "no-match" :
            (local == null ? "no-local" : (!local.IsLocalPlayer() ? "observer" :
            (local.get_CurHP() <= 0 ? "local-hp-zero" : "local-knocked")));
        ClearAimKillProbes();
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLocalReason = "ready";
      bool manual = global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualHolding;
      if (manual && !AimKillManualEnabled()) {
        ClearAimKillManualTarget();
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillManualSkips++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 7;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 7;
        return;
      }
      if (!manual && (now < global::COW.GamePlay.SceneEditBoxSelectTool.R7AKResumeAt ||
          !AimKillAutoEnabled())) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 11;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 11;
        return;
      }
      if (local.IsReloading()) {
        ClearAimKillManualTarget();
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillReloads++;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 3;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 6;
        return;
      }
      Camera camera = Camera.get_main();
      Transform root = local.get_RootTransform();
      Transform head = local.GetHeadTF();
      if (camera == null || root == null || head == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 2;
        return;
      }
      global::COW.GamePlay.HBIBDMMOOOK weapon = local.GetWeaponOnHand();
      if (weapon == null) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 2;
        return;
      }
      float range = weapon.OEOBPHDFJMO();
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7AimCadenceWeapon != weapon) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimCadenceWeapon = weapon;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimCadenceLast = 0f;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNextHit = 0f;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget = null;
      }
      float hitDelay = AimKillWeaponDelay(weapon);
      float lastHit = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimCadenceLast;
      float due = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNextHit;
      if (lastHit > 0f && due < lastHit + hitDelay) { due = lastHit + hitDelay; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimCooldownUntil = due;
      if (!manual && now < due) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 10;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 10;
        // Wake on the first eligible frame instead of adding another 50ms
        // poll quantum to the selected deadline. Never submit a catch-up burst.
        if (due < global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNext) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNext = due;
        }
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillWeaponRange = range;
      if (!(range > 0f && range < 10000f)) { return; }
      float configured = AimKillRange();
      if (range > configured) { range = configured; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillEffectiveRange = range;
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
      global::COW.GamePlay.Player pendingLock = global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget;
      if (pendingLock != null && now > global::COW.GamePlay.SceneEditBoxSelectTool.R7LockUntil) {
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
            if (lockOk) { best = pendingLock; lockUsed = true; global::COW.GamePlay.SceneEditBoxSelectTool.R7LockUntil = now + 3f; }
          }
        }
        if (!lockUsed) { global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget = null; }
      }
      float closest = range + 1f;
      int bestSlot = 0;
      float scanStarted = Time.get_realtimeSinceStartup();
      int count = players.Count;
      int cursor = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillScanCursor;
      if (cursor < 0 || cursor >= count) { cursor = 0; }
      int budget = count;
      if (budget > 12) { budget = 12; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 2;
      for (int scan = 0; scan < budget && !lockUsed; scan++) {
        if (scan > 0 && Time.get_realtimeSinceStartup() - scanStarted > 0.0025f) { break; }
        int i = cursor;
        cursor++;
        if (cursor >= count) { cursor = 0; }
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillScanCursor = cursor;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillScanned++;
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
             global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillBlocked++;
             continue;
           }
          if (!(distance < closest)) { continue; }
          best = enemy; closest = distance; bestSlot = i & 63;
        } catch (System.Exception) { }
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillScanMs =
          (Time.get_realtimeSinceStartup() - scanStarted) * 1000f;
      if (best == null) {
        ClearAimKillManualTarget();
        if (now - global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillLastTarget > 0.4f) {
          global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 4;
        }
        return;
      }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 3;
      if (!lockUsed) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LockTarget = best;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7LockUntil = now + 3f;
      }
      if (manual) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualTarget = best;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualWeapon = weapon;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AKManualAt = now;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 7;
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 7;
        return;
      }
      // Reserve before native work; no catch-up loop after missed frames.
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNextHit = now + hitDelay;
      bool submitted = StartAimKillXNEW(local, weapon, best, origin, mode, range, visible, bestSlot);
      if (!submitted && global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus == 5 && hitDelay < 1f) {
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNextHit = now + 1f;
      }
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStop = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillStatus = 5;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillError = error.Message;
      PlayerPrefs.SetString("OKMOD.Diagnostic.AimKillError", error.ToString());
      global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillNext = now + 1f;
    }
  }

  private static void ReleaseAimKillAmmo() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7AimAmmoApplied) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimAmmoApplied = false;
    global::COW.GamePlay.PlayerAttributes owner =
        global::COW.GamePlay.SceneEditBoxSelectTool.R7AimAmmoOwner;
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimAmmoOwner = null;
    if (owner == null) { return; }
    try { owner.IsAmmoFree = global::COW.GamePlay.SceneEditBoxSelectTool.R7AimAmmoOld; }
    catch (System.Exception) { }
  }

  private static void ReleaseAimKillFire() {
    if (!global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring = false;
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
    if (PlayerPrefs.GetString("OKMOD.Aim.Silent", "") == "X2") { target = SilentPoint(candidate, point); }
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
    if (global::COW.GamePlay.SceneEditBoxSelectTool.OkAimFrame == frame + 1) { return; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimFrame = frame + 1;
    RefreshAimTarget(w, h);
    global::COW.GamePlay.Player best = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimTarget;
    if (best == null || PlayerPrefs.GetString("OKMOD.Aim.Silent", "") == "X2") { return; }
    global::COW.GamePlay.Player local = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOwner;
    Camera cam = Camera.get_main();
    if (local == null || cam == null) { return; }
    Transform aimCamera = local.MainCameraTransform;
    if (aimCamera == null) { aimCamera = cam.get_transform(); }
    if (aimCamera == null) { return; }
    Vector3 origin = aimCamera.get_position();
    int point = ReadSetting("OKMOD.Aim.Point", 0, 0, 5);
    Vector3 selectedPoint = AimPoint(best, point);
    if (!ValidVector(selectedPoint) || !ValidVector(origin)) { return; }
    Vector3 direction = selectedPoint - origin;
    Quaternion rotation = Quaternion.LookRotation(direction);
    float smooth = ReadFloatSetting("OKMOD.Aim.Smooth", 6, 0, 20);
    Quaternion current = local.GetAimRotation();
    float error = Quaternion.Angle(current, rotation);
    if (smooth > 0) {
      float dt = Time.get_unscaledDeltaTime();
      if (!(dt > 0f)) {
        global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 8;
        return;
      }
      if (dt > 0.1f) { dt = 0.1f; }
      float alpha = 1f - Mathf.Exp(-dt * 60f / smooth);
      if (error > 0.05f) { rotation = Quaternion.Slerp(current, rotation, alpha); }
    }
    local.SetAimRotation(rotation, false);
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 6;
    if (smooth == 0 || error <= 0.05f) { global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 7; }
  }

  // Called synchronously by both HUD and the getter; never reads the getter itself.
  private static void RefreshAimTarget(int w, int h) {
    global::COW.GamePlay.Player previous = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimTarget;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimTarget = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 0;
    bool silent = ResolveAimMode();
    if (NativeRecovery() || PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") == "X2") { return; }
    bool anyAngle = !silent && PlayerPrefs.GetString("OKMOD.Aim.AnyAngle", "") == "X2";
    if (!silent && PlayerPrefs.GetString("OKMOD.Aim.Enabled", "") != "X2") {
      global::COW.GamePlay.SceneEditBoxSelectTool.OkAimMatch = null;
      global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOwner = null;
      return;
    }
    global::COW.GamePlay.JMAGGLCNGIG match = global::COW.GameFacade.CurrentMatch();
    global::COW.GamePlay.Player local = global::COW.GameFacade.GetLocalPlayerOrObServer();
    if (match != global::COW.GamePlay.SceneEditBoxSelectTool.OkAimMatch ||
        local != global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOwner) { previous = null; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimMatch = match;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOwner = local;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 1;
    if (match == null || local == null || local.get_CurHP() <= 0 || local.get_IsDieing()) { return; }
    GameObject localObject = local.get_gameObject();
    if (localObject == null || !localObject.get_activeInHierarchy()) { return; }
    bool scope = PlayerPrefs.GetString("OKMOD.Aim.Scope", "X2") == "X2";
    bool fire = PlayerPrefs.GetString("OKMOD.Aim.Fire", "") == "X2";
    bool lockAlways = PlayerPrefs.GetString("OKMOD.Aim.Lock", "") == "X2";
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 2;
    // Scope/fire activation config belongs to camera aim. Silent activates when firing.
    if (!silent && !lockAlways && !anyAngle && (scope || fire) &&
        !((scope && local.get_IsSighting()) || (fire && local.IsFiring()))) { return; }
    if (anyAngle && !lockAlways && !local.IsFiring()) { return; }
    Camera cam = Camera.get_main();
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 3;
    if (cam == null || cam.get_transform() == null) { return; }
    IList players = match.ILPNGCMEFFI();
    if (players == null) { return; }
    Transform aimCamera = local.MainCameraTransform;
    if (aimCamera == null) { aimCamera = cam.get_transform(); }
    // Use the view origin, not the offset weapon/shoulder origin (parallax).
    Vector3 origin = aimCamera.get_position();
    Vector3 sightOrigin = origin;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 8;
    if (!ValidVector(origin) || !ValidVector(sightOrigin)) { return; }
    int point = ReadSetting("OKMOD.Aim.Point", 0, 0, 5);
    float radius = ReadFloatSetting("OKMOD.Aim.FOV", 220, 20, 800);
    float range = ReadFloatSetting("OKMOD.Aim.Range", 150, 10, 500);
    int priority = ReadSetting("OKMOD.Aim.Priority", 0, 0, 1);
    bool skipKnocked = PlayerPrefs.GetString("OKMOD.Aim.SkipKnocked", "X2") == "X2";
    bool visible = PlayerPrefs.GetString("OKMOD.Aim.Visible", "X2") == "X2";
    bool sticky = PlayerPrefs.GetString("OKMOD.Aim.Sticky", "X2") == "X2";
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
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 4;
    if (best == null) {
      if (missingBone) { global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 9; }
      if (occluded) { global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 5; }
      return;
    }
    Vector3 selectedPoint = AimPoint(best, point);
    if (silent) { selectedPoint = SilentPoint(best, point); }
    if (!ValidVector(selectedPoint)) {
      global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 9;
      return;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimTarget = best;
    if (silent) { global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus = 10; }
  }

  private static bool SameVector(Vector3 a, Vector3 b) {
    return a.x == b.x && a.y == b.y && a.z == b.z;
  }

  private static void RestoreSilent(global::COW.GamePlay.CGKJLKPMGDJ hit) {
    // Never restore a different/shared object or overwrite game updates.
    if (hit != null && hit == global::COW.GamePlay.SceneEditBoxSelectTool.OkAimHit &&
        hit.JHAINOBIFMA == global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedObject &&
        hit.JINDNOFABKB == global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedCollider &&
        SameVector(hit.HPCOJHGFEEG, global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedPoint) &&
        SameVector(hit.DOJDNHCGFGK, new Vector3(0f, 1f, 0f)) &&
        SameVector(hit.HLDECMJFKJK, global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedDirection) &&
        hit.DKPDBLLJEAA == global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedGroup &&
        hit.EFMBNPNFGHM == global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedIgnore && !hit.CEOKOILBODN) {
      hit.JHAINOBIFMA = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldObject;
      hit.JINDNOFABKB = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldCollider;
      hit.HPCOJHGFEEG = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldPoint;
      hit.DOJDNHCGFGK = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldNormal;
      hit.HLDECMJFKJK = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldDirection;
      hit.DKPDBLLJEAA = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldGroup;
      hit.EFMBNPNFGHM = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldIgnore;
      hit.CEOKOILBODN = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldBlocked;
    }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimHit = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedObject = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedCollider = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldObject = null;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldCollider = null;
  }

  private static void ObserveManualHit(global::COW.GamePlay.Player player,
      global::COW.GamePlay.CGKJLKPMGDJ hit) {
    // Only read a naturally requested native hit during real firing. Do not
    // invoke a fire method, alter this object, or retain game object references.
    if (hit == null || global::COW.GamePlay.SceneEditBoxSelectTool.R7AimKillFiring ||
        NativeRecovery() || PlayerPrefs.GetString("OKMOD.Aim.AimKill", "") != "X2") { return; }
    try {
      if (!player.IsLocalPlayer() || !player.IsFiring()) { return; }
      float now = Time.get_realtimeSinceStartup();
      if (global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitSamples > 0 &&
          now - global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitAt < 0.25f) { return; }
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitAt = now;
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
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitSnapshot = snapshot;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitSamples++;
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitError = "";
    } catch (System.Exception error) {
      global::COW.GamePlay.SceneEditBoxSelectTool.R7ManualHitError = error.Message;
    }
  }

  // Replaces the existing getter fix; no recursive getter call or native pointers.
  public static global::COW.GamePlay.CGKJLKPMGDJ SilentAimingInfo(global::COW.GamePlay.Player player) {
    if (player == null) { return null; }
    global::COW.GamePlay.CGKJLKPMGDJ hit = player.FDMIEDDNCEC;
    ObserveManualHit(player, hit);
    // Disabled mode must be a field-only passthrough, except pending restoration.
    if (PlayerPrefs.GetString("OKMOD.Aim.Silent", "") != "X2" &&
        global::COW.GamePlay.SceneEditBoxSelectTool.OkAimHit == null) { return hit; }
    if (!player.IsLocalPlayer()) { return hit; }
    RestoreSilent(hit);
    if (!ResolveAimMode() || player != global::COW.GameFacade.GetLocalPlayerOrObServer()) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentFrame = Time.get_frameCount() + 1;
    int calls = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentCalls;
    if (calls < 1000000000) { global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentCalls = calls + 1; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = 11;
    if (hit == null) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = 12;
    if (!player.IsFiring()) { return hit; }
    RefreshAimTarget(Screen.get_width(), Screen.get_height());
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimStatus;
    global::COW.GamePlay.Player target = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimTarget;
    if (target == null) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = 14;
    int mode = ReadSetting("OKMOD.Aim.Point", 0, 0, 5);
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
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = 15;
    if (!ValidVector(origin) || !ValidVector(point) || !(distance > 0.001f) ||
        distance > ReadFloatSetting("OKMOD.Aim.Range", 150, 10, 500)) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = 16;
    if (PlayerPrefs.GetString("OKMOD.Aim.Visible", "X2") == "X2" &&
        !ClearSight(origin, point, target.get_RootTransform(), player.get_RootTransform())) { return hit; }
    Vector3 direction = Vector3.Normalize(point - origin);
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = 15;
    if (!ValidVector(direction)) { return hit; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldObject = hit.JHAINOBIFMA;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldCollider = hit.JINDNOFABKB;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldPoint = hit.HPCOJHGFEEG;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldNormal = hit.DOJDNHCGFGK;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldDirection = hit.HLDECMJFKJK;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldGroup = hit.DKPDBLLJEAA;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldIgnore = hit.EFMBNPNFGHM;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimOldBlocked = hit.CEOKOILBODN;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimHit = hit;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedObject = targetObject;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedCollider = collider;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedPoint = point;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedDirection = direction;
    global::COW.GamePlay.LKOOALMKJND group = global::COW.GamePlay.LKOOALMKJND.Body;
    if (mode == 0) { group = global::COW.GamePlay.LKOOALMKJND.Head; }
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedGroup = group;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimAppliedIgnore = mode == 0;
    // BS4484 hit-object semantics; preserve native origin, distance and damage.
    hit.JHAINOBIFMA = targetObject;
    hit.JINDNOFABKB = collider;
    hit.HPCOJHGFEEG = point;
    hit.DOJDNHCGFGK = new Vector3(0f, 1f, 0f);
    hit.HLDECMJFKJK = direction;
    hit.DKPDBLLJEAA = group;
    hit.EFMBNPNFGHM = mode == 0;
    hit.CEOKOILBODN = false;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentStatus = 17;
    global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentAppliedFrame = Time.get_frameCount() + 1;
    int writes = global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentWrites;
    if (writes < 1000000000) { global::COW.GamePlay.SceneEditBoxSelectTool.OkAimSilentWrites = writes + 1; }
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
        global::COW.GamePlay.SceneEditBoxSelectTool.OkEspEffectFrame != frame + 1;
    if (updateEffects) {
      global::COW.GamePlay.SceneEditBoxSelectTool.OkEspEffectFrame = frame + 1;
      DisableLines(global::COW.GamePlay.SceneEditBoxSelectTool.OkEspBlueLines);
      DisableLines(global::COW.GamePlay.SceneEditBoxSelectTool.OkEspGrenadeLines);
    }
    if (espFov) {
      GUI.set_color(new Color(1f, 0.85f, 0.1f, 0.6f));
      float centerX = (float)w * 0.5f;
      float centerY = (float)h * 0.5f;
      float radius = ReadFloatSetting("OKMOD.Aim.FOV", 220, 20, 800);
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
        if (pl == global::COW.GamePlay.SceneEditBoxSelectTool.OkAimTarget &&
            PlayerPrefs.GetString("OKMOD.Aim.Mark", "X2") == "X2") {
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
    if (grenade) { lines = global::COW.GamePlay.SceneEditBoxSelectTool.OkEspGrenadeLines; }
    else { lines = global::COW.GamePlay.SceneEditBoxSelectTool.OkEspBlueLines; }
    if (lines == null) {
      lines = new LineRenderer[128];
      if (grenade) { global::COW.GamePlay.SceneEditBoxSelectTool.OkEspGrenadeLines = lines; }
      else { global::COW.GamePlay.SceneEditBoxSelectTool.OkEspBlueLines = lines; }
    }
    LineRenderer line = lines[index];
    if (line == null) {
      Material material = global::COW.GamePlay.SceneEditBoxSelectTool.OkEspLineMaterial;
      if (material == null) {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        if (shader == null) { return; }
        material = new Material(shader);
        material.SetInt("_SrcBlend", 5);
        material.SetInt("_DstBlend", 10);
        material.SetInt("_Cull", 0);
        material.SetInt("_ZWrite", 0);
        material.SetInt("_ZTest", 8);
        global::COW.GamePlay.SceneEditBoxSelectTool.OkEspLineMaterial = material;
      }
      GameObject owner = new GameObject("OKMOD");
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
