using System;
using System.Collections;
using UnityEngine;
using S = COW.GamePlay.SceneEditBoxSelectTool;
using Player = COW.GamePlay.Player;

// Only this renderer and its own helper graph are compiled into the candidate.
public class SampledEsp {
  public static void DrawEsp(int w, int h, Texture2D tex, bool box,
      bool name, bool hp, bool distance, bool line, bool fov,
      bool threeD, bool fill, bool arrow, bool link, bool team,
      bool blue, bool headCircle, bool skeleton, bool info, bool armor) {
  SetPhase("entry-event");
    Event ev = Event.get_current();
    if (ev == null || ev.get_type() != 7) { return; }
  SetPhase("entry-cache");
    InitCache();
  SetPhase("entry-frame");
    int frame = Time.get_frameCount();
    if (S.R7Frame == frame + 1) { return; }
    S.R7Frame = frame + 1;
    float now = Time.get_realtimeSinceStartup();
  SetPhase("entry-settings");
    S.R7EnemyColor = LoadColor("OKMOD.Color.Enemy", 1f, 0.353f, 0.431f, 0.95f);
    S.R7TeamColor = LoadColor("OKMOD.Color.Team", 0.239f, 0.839f, 1f, 0.95f);
    S.R7VehColor = LoadColor("OKMOD.Color.Vehicle", 0.2f, 1f, 0.3f, 0.95f);
    S.R7TargetColor = LoadColor("OKMOD.Color.Target", 1f, 0.82f, 0.4f, 0.95f);
    S.R7NameColor = LoadColor("OKMOD.Color.Name", 1f, 1f, 1f, 1f);
    float espFont = PlayerPrefs.GetFloat("OKMOD.Esp.Font.Float", 24f);
    if (!(espFont >= 16f && espFont <= 32f)) { espFont = 24f; }
    S.R7FontSize = espFont;
    S.R8BatchOn = PlayerPrefs.GetString("OKMOD.R8.Batch", "X2") == "X2";
    bool boxField = PlayerPrefs.GetString("OKMOD.Esp.BoxField", "") == "X2";
    bool vehicle = PlayerPrefs.GetString("OKMOD.Esp.Vehicle", "") == "X2";
    bool alert = PlayerPrefs.GetString("OKMOD.Esp.Alert", "") == "X2";
    bool visibility = false; // removed: color-by-obstacle
    bool directOnly = PlayerPrefs.GetString("OKMOD.Esp.DirectOnly", "") == "X2";
    bool viewLine = false; // removed: enemy aim direction
    bool threat = PlayerPrefs.GetString("OKMOD.Esp.Threat", "") == "X2";
    bool radar = PlayerPrefs.GetString("OKMOD.Esp.Radar100", "") == "X2";
    bool offscreen = PlayerPrefs.GetString("OKMOD.Esp.Offscreen", "") == "X2";
    bool segments = false; // removed: 7-segment HP
    bool combat = false; // removed: compact under 35m
    int flags = (box ? 1 : 0) | (name ? 2 : 0) | (hp ? 4 : 0) | (distance ? 8 : 0)
        | (line ? 16 : 0) | (threeD ? 64 : 0) | (fill ? 128 : 0)
        | (arrow ? 256 : 0) | (link ? 512 : 0) | (team ? 1024 : 0)
        | (blue ? 2048 : 0)
        | (boxField ? 8192 : 0) | (vehicle ? 16384 : 0) | (alert ? 32768 : 0)
        | (headCircle ? 65536 : 0) | (skeleton ? 131072 : 0)
        | (info ? 262144 : 0) | (armor ? 524288 : 0)
        | (visibility ? 1048576 : 0) | (directOnly ? 2097152 : 0)
        | (threat ? 8388608 : 0)
        | (radar ? 16777216 : 0) | (offscreen ? 33554432 : 0)
        | (segments ? 67108864 : 0) | (combat ? 134217728 : 0);
    // FOV is independent of all match/player access.
  if (fov) { SetPhase("entry-fov"); DrawFov(w, h, tex); }
  else if (S.R8FovMesh != null) { ReleaseFov(); }
    if (PlayerPrefs.GetString("OKMOD.Esp.Crosshair", "") == "X2") {
      DrawCrosshair(w, h, tex);
    }
    if (flags == 0) {
      ClearContext();
      Telemetry(now);
      return;
    }

    try {
    SetPhase("sample");
  SetPhase("sample-match");
      COW.GamePlay.JMAGGLCNGIG match = COW.GameFacade.CurrentMatch();
  SetPhase("sample-camera");
      Camera camera = Camera.get_main();
  SetPhase("sample-local");
      Player local = COW.GameFacade.GetLocalPlayerOrObServer();
      if (match == null || camera == null || local == null) {
        ClearContext();
        Telemetry(now);
        return;
      }
      if (!System.Object.ReferenceEquals(match, S.R7Match) || camera != S.R7Camera ||
          local != S.R7Local || w != S.R7Width || h != S.R7Height || flags != S.R7Flags) {
        ClearContext();
        S.R7Match = match;
        S.R7Camera = camera;
        S.R7Local = local;
        S.R7Width = w; S.R7Height = h; S.R7Flags = flags;
      }
  SetPhase("sample-list");
      if (now >= S.R7NextSample || now < S.R7SampleStamp) {
        Sample(match, camera, local, w, h, flags, now);
      }
      // Never keep drawing an old scene's or stalled sampler's boxes.
      if (now >= S.R7SampleStamp && now - S.R7SampleStamp <= 0.25f) {
      SetPhase("project");
        ProjectCached(camera, w, h, now);
      SetPhase("GUI");
        Render(tex, flags);
      } else {
        S.R7Count = 0; S.R7WorldCount = 0; S.R7IdsCount = 0;
        S.R7LiveCount = 0; S.R7LiveValid = false;
        S.R7WorldPrevCount = 0; S.R7IdsPrevCount = 0;
        S.R8MeshDirty = true;
      }
    } catch (Exception error) {
      S.R7Count = 0;
      S.R7WorldCount = 0; S.R7IdsCount = 0;
      S.R7LiveCount = 0; S.R7LiveValid = false;
      S.R7NextSample = now + 0.25f;
      RecordError(error, now);
    }
    Telemetry(Time.get_realtimeSinceStartup());
  }

  private static void InitCache() {
    if (S.R7Front != null && S.R7Team != null && S.R7Target != null &&
        S.R7VehFront != null && S.R7VehBack != null &&
        S.R7Prev != null && S.R7Draw != null && S.R7World != null &&
        S.R7WorldPrev != null && S.R7Live != null &&
        S.R7RootTf != null && S.R7HeadTf != null &&
        S.R7Players != null && S.R7BackPlayers != null &&
        S.R7Meters != null && S.R7BackMeters != null && S.R7Ids != null &&
        S.R7Skeleton != null && S.R7Info != null && S.R7HeadScreen != null &&
        S.R7Visibility != null && S.R7BoneTf != null && S.R7BackBoneTf != null) { return; }
    S.R7Front = new float[80]; S.R7Back = new float[80];
    S.R7Meters = new float[16]; S.R7BackMeters = new float[16];
    S.R7Prev = new float[80]; S.R7Draw = new float[80];
    S.R7World = new float[96]; S.R7WorldPrev = new float[96]; S.R7Live = new float[80];
    S.R7Ids = new int[16]; S.R7IdsPrev = new int[16];
    S.R7RootTf = new Transform[16]; S.R7HeadTf = new Transform[16];
    S.R7Players = new Player[16]; S.R7BackPlayers = new Player[16];
    S.R7HeadScreen = new float[48];
    S.R7Skeleton = new float[512]; S.R7SkeletonValid = new bool[256];
    S.R7BoneTf = new Transform[256]; S.R7BackBoneTf = new Transform[256];
    S.R7BonePairs = new int[30];
    int[] pairs = S.R7BonePairs;
    pairs[0] = 0; pairs[1] = 1; pairs[2] = 1; pairs[3] = 2;
    pairs[4] = 2; pairs[5] = 3; pairs[6] = 1; pairs[7] = 4;
    pairs[8] = 4; pairs[9] = 5; pairs[10] = 5; pairs[11] = 6;
    pairs[12] = 1; pairs[13] = 7; pairs[14] = 7; pairs[15] = 8;
    pairs[16] = 8; pairs[17] = 9; pairs[18] = 3; pairs[19] = 10;
    pairs[20] = 10; pairs[21] = 11; pairs[22] = 11; pairs[23] = 12;
    pairs[24] = 3; pairs[25] = 13; pairs[26] = 13; pairs[27] = 14;
    pairs[28] = 14; pairs[29] = 15;
    S.R7Info = new string[16]; S.R7BackInfo = new string[16];
    S.R7InfoBits = new int[16]; S.R7InfoCache = new string[16];
    S.R7Visibility = new int[16]; S.R7BackVisibility = new int[16];
    S.R7Directions = new float[64]; S.R7BackDirections = new float[64];
    S.R7DirectionValid = new bool[16]; S.R7BackDirectionValid = new bool[16];
    S.R7Threat = new bool[16]; S.R7BackThreat = new bool[16];
    S.R7Radar = new float[48]; S.R7BackRadar = new float[48];
    S.R7Bots = new bool[16]; S.R7BackBots = new bool[16];
    S.R7Offscreen = new float[48]; S.R7OffscreenDistance = new string[16];
    S.R7PrevCount = 0; S.R7PrevStamp = 0f; S.R7Smooth = false;
    S.R7WorldCount = 0; S.R7WorldPrevCount = 0;
    S.R7IdsCount = 0; S.R7IdsPrevCount = 0;
    S.R7LiveCount = 0; S.R7LiveStamp = 0f; S.R7LiveValid = false;
    S.R7Names = new string[16]; S.R7BackNames = new string[16];
    S.R7Distances = new string[16]; S.R7BackDistances = new string[16];
    S.R7Knocked = new bool[16]; S.R7BackKnocked = new bool[16];
    S.R7Team = new bool[16]; S.R7BackTeam = new bool[16];
    S.R7Target = new bool[16]; S.R7BackTarget = new bool[16];
    S.R7VehFront = new float[80]; S.R7VehBack = new float[80]; S.R7VehCount = 0;
    S.R7FullNames = new string[16]; S.R7ShortNames = new string[16];
    S.R7DistanceLabels = new string[501];
    S.R7Stats = "R7: chờ lấy mẫu ESP";
    S.R7FontSize = 24f;
    S.R7Thread = System.Threading.Thread.CurrentThread.ManagedThreadId;
  PlayerPrefs.SetString("OKMOD.Diagnostic.Build", "R8-AIM-ESP-v1l");
    PlayerPrefs.SetString("OKMOD.R7.LastError", "");
    PlayerPrefs.SetInt("OKMOD.R7.Errors", 0);
    PlayerPrefs.Save();
  }

  private static void ClearContext() {
  bool hadContext = S.R7Match != null || S.R7Count != 0;
    S.R7Count = 0; S.R7NextSample = 0f;
    S.R7Match = null; S.R7Camera = null; S.R7Local = null;
    S.R7Flags = 0;
    S.R7VehCount = 0;
    S.R7WorldCount = 0; S.R7WorldPrevCount = 0;
    S.R7IdsCount = 0; S.R7IdsPrevCount = 0;
    S.R7LiveCount = 0; S.R7LiveValid = false;
    S.R7ThreatCount = 0;
    for (int i = 0; i < 16; i++) { S.R7RootTf[i] = null; S.R7HeadTf[i] = null; }
    for (int i = 0; i < 16; i++) { S.R7Players[i] = null; S.R7BackPlayers[i] = null; }
    for (int i = 0; i < 16; i++) {
      S.R7Team[i] = false; S.R7BackTeam[i] = false;
      S.R7Target[i] = false; S.R7BackTarget[i] = false;
      S.R7Info[i] = null; S.R7BackInfo[i] = null;
      S.R7Visibility[i] = 0; S.R7BackVisibility[i] = 0;
      S.R7DirectionValid[i] = false; S.R7BackDirectionValid[i] = false;
      S.R7Threat[i] = false; S.R7BackThreat[i] = false;
      S.R7Radar[i * 3 + 2] = 0f; S.R7BackRadar[i * 3 + 2] = 0f;
      S.R7Offscreen[i * 3 + 2] = 0f; S.R7OffscreenDistance[i] = null;
    }
    for (int i = 0; i < 256; i++) {
      S.R7SkeletonValid[i] = false;
      S.R7BoneTf[i] = null; S.R7BackBoneTf[i] = null;
    }
  if (!hadContext) { return; }
  string[] names = S.R7Names, backNames = S.R7BackNames;
  string[] distances = S.R7Distances, backDistances = S.R7BackDistances;
  string[] full = S.R7FullNames, shortNames = S.R7ShortNames;
    for (int i = 0; i < 16; i++) {
    names[i] = null; backNames[i] = null;
    distances[i] = null; backDistances[i] = null;
    full[i] = null; shortNames[i] = null;
    }
  ReleaseBatch();
  }

  private static void Sample(COW.GamePlay.JMAGGLCNGIG match, Camera camera,
      Player local, int w, int h, int flags, float now) {
  SetPhase("sample");
    float started = Time.get_realtimeSinceStartup();
    // Backoff is installed before any native read, including list retrieval.
    S.R7NextSample = now + 0.10f;
    int drawn = 0; int visited = 0;
    int skeletonCount = 0;
    for (int k = 0; k < 256; k++) { S.R7BackBoneTf[k] = null; }
    IList players = match.ILPNGCMEFFI();
    if (players == null) {
      S.R7Count = 0; S.R7WorldCount = 0; S.R7IdsCount = 0;
      S.R7LiveCount = 0; S.R7LiveValid = false;
      return;
    }
SetPhase("sample-local-root");
    Transform localRoot = local.get_RootTransform();
    Vector3 origin = new Vector3(0f, 0f, 0f);
    bool hasOrigin = localRoot != null;
    if (hasOrigin) { origin = localRoot.get_position(); hasOrigin = Valid(origin); }
    Transform cameraTransform = camera.get_transform();
    Vector3 cameraOrigin = origin;
    if (cameraTransform != null) { cameraOrigin = cameraTransform.get_position(); }
    Vector3 localForward = new Vector3(0f, 0f, 1f);
    if ((flags & 16777216) != 0) { localForward = AimForward(local.GetAimRotation()); }
    int limit = players.Count;
    if (limit > 128) { limit = 128; }
    bool wantTeam = (flags & 1024) != 0;
int start = 0;
  int worldPrev = S.R7WorldCount;
  if (worldPrev > 16) { worldPrev = 16; }
  if (worldPrev > 0) {
    for (int k = 0; k < worldPrev * 6; k++) { S.R7WorldPrev[k] = S.R7World[k]; }
    for (int k = 0; k < worldPrev; k++) { S.R7IdsPrev[k] = S.R7Ids[k]; }
  }
  S.R7WorldPrevCount = worldPrev;
  S.R7IdsPrevCount = worldPrev;
  S.R7WorldCount = 0;
  S.R7IdsCount = 0;
  // Resolve dynamic IFix storage once per sample, not once per field/player.
  float[] records = S.R7Back;
  string[] recordNames = S.R7BackNames, recordDistances = S.R7BackDistances;
  bool[] recordKnocked = S.R7BackKnocked;
  bool[] recordTeam = S.R7BackTeam, recordTarget = S.R7BackTarget;
   string[] fullNames = S.R7FullNames, shortNames = S.R7ShortNames;
   string[] distanceLabels = S.R7DistanceLabels;
   for (int clear = 0; clear < 16; clear++) {
     recordNames[clear] = null; recordDistances[clear] = null;
     fullNames[clear] = null; shortNames[clear] = null;
     S.R7BackInfo[clear] = null; S.R7InfoCache[clear] = null; S.R7InfoBits[clear] = -1;
     recordTeam[clear] = false; recordTarget[clear] = false; recordKnocked[clear] = false;
     S.R7BackVisibility[clear] = 0; S.R7BackDirectionValid[clear] = false;
     S.R7BackThreat[clear] = false; S.R7BackBots[clear] = false;
     S.R7BackPlayers[clear] = null;
   }
for (int step = 0; step < limit && drawn < 16; step++) {
  int i = start + step;
  if (i >= limit) { i -= limit; }
      // Cooperative deadline: cannot interrupt one slow native call.
      if (visited > 0 && Time.get_realtimeSinceStartup() - started >= 0.012f) { break; }
      visited++;
      try {
    SetPhase("player-read-list");
        if (i >= players.Count) { break; }
        Player player = players[i] as Player;
        if (player == null || player == local) { continue; }
    SetPhase("player-read-object");
        GameObject owner = player.get_gameObject();
        if (owner == null || !owner.get_activeInHierarchy()) { continue; }
        if (player.IsLocalPlayer()) { continue; }
        bool teammate = player.IsLocalTeammate(false) || player.IsLocalTeammate(true);
        if (teammate && !wantTeam) { continue; }
    SetPhase("player-read-team-hp");
        int currentHp = player.get_CurHP();
        if (currentHp <= 0) { continue; }
    SetPhase("player-read-bones");
        Transform root = player.get_RootTransform();
        Transform head = player.GetHeadTF();
        if (root == null || head == null) { continue; }
        Vector3 feet = root.get_position();
        Vector3 top = head.get_position() + new Vector3(0f, 0.2f, 0f);
        if (!Valid(feet) || !Valid(top)) { continue; }
        float meters = 0f;
        if (hasOrigin) {
          meters = Vector3.Distance(origin, feet);
          if (!(meters >= 0f && meters <= 150f)) { continue; }
        }
        S.R7BackVisibility[drawn] = 0;
        S.R7BackDirectionValid[drawn] = false;
        S.R7BackThreat[drawn] = false;
        S.R7BackRadar[drawn * 3 + 2] = 0f;
        S.R7BackBots[drawn] = player.IsClientBot;
        // Unknown stays unknown when the optional ray budget is exhausted.
        if ((flags & (1048576 | 2097152)) != 0 &&
            Time.get_realtimeSinceStartup() - started < 0.008f) {
          S.R7BackVisibility[drawn] = SampleVisibility(cameraOrigin,
              new Vector3(top.x, top.y - 0.2f, top.z), root, localRoot);
        }
        if ((flags & 8388608) != 0 &&
            Time.get_realtimeSinceStartup() - started < 0.009f) {
          try {
            Vector3 forward = AimForward(player.GetAimRotation());
            Vector3 startPoint = new Vector3(top.x, top.y - 0.2f, top.z);
            Vector3 endPoint = new Vector3(startPoint.x + forward.x * 4f,
                startPoint.y + forward.y * 4f, startPoint.z + forward.z * 4f);
            Vector3 fromScreen = camera.WorldToScreenPoint(startPoint);
            Vector3 toScreen = camera.WorldToScreenPoint(endPoint);
            if (Valid(fromScreen) && Valid(toScreen) && fromScreen.z > 0.1f && toScreen.z > 0.1f) {
              S.R7BackDirections[drawn * 4] = fromScreen.x;
              S.R7BackDirections[drawn * 4 + 1] = (float)h - fromScreen.y;
              S.R7BackDirections[drawn * 4 + 2] = toScreen.x;
              S.R7BackDirections[drawn * 4 + 3] = (float)h - toScreen.y;
              S.R7BackDirectionValid[drawn] = true;
            }
            if (!teammate && hasOrigin && meters <= 100f && meters > 1f) {
              Vector3 toLocal = Vector3.Normalize(new Vector3(cameraOrigin.x - startPoint.x,
                  cameraOrigin.y - startPoint.y, cameraOrigin.z - startPoint.z));
              float dot = forward.x * toLocal.x + forward.y * toLocal.y + forward.z * toLocal.z;
              S.R7BackThreat[drawn] = dot >= 0.985f;
            }
          } catch (Exception error) { RecordError(error, now); }
        }
        if ((flags & 16777216) != 0 && hasOrigin) {
          float dx = feet.x - origin.x, dz = feet.z - origin.z;
          float horizontal = Mathf.Sqrt(localForward.x * localForward.x + localForward.z * localForward.z);
          if (horizontal > 0.001f) {
            float rightX = localForward.z / horizontal, rightZ = -localForward.x / horizontal;
            S.R7BackRadar[drawn * 3] = dx * rightX + dz * rightZ;
            S.R7BackRadar[drawn * 3 + 1] = (dx * localForward.x + dz * localForward.z) / horizontal;
            S.R7BackRadar[drawn * 3 + 2] = 1f;
          }
        }
    SetPhase("player-read-projection");
        float x = 0f, y = 0f, width = 0f, height = 0f;
        Vector3 sf = camera.WorldToScreenPoint(feet);
        Vector3 sh = camera.WorldToScreenPoint(top);
        if (Valid(sf) && Valid(sh) && sf.z > 0.1f && sh.z > 0.1f) {
          float bottom = (float)h - sf.y;
          height = (sh.y - sf.y) * 1.0909f;
          if (height > 0f && height < (float)h * 4f) {
            width = height * 0.5f;
            x = (sf.x + sh.x) * 0.5f - width * 0.5f;
            y = bottom - height;
            // Keep offscreen players cached; the per-frame projector will
            // place them as soon as they enter view.
            if (x > w || x + width < 0f || y > h || bottom < 0f) {
              x = 0f; y = 0f; width = 0f; height = 0f;
            }
          }
        }
        bool knocked = player.get_IsDieing();
        if (S.R7BackDirectionValid[drawn]) {
          if (width > 2f && height > 2f) {
            int d = drawn * 4;
            S.R7BackDirections[d] = (S.R7BackDirections[d] - x) / width;
            S.R7BackDirections[d + 1] = (S.R7BackDirections[d + 1] - y) / height;
            S.R7BackDirections[d + 2] = (S.R7BackDirections[d + 2] - x) / width;
            S.R7BackDirections[d + 3] = (S.R7BackDirections[d + 3] - y) / height;
          } else { S.R7BackDirectionValid[drawn] = false; }
        }
        float ratio = 0f;
        if ((flags & 4) != 0 && !knocked) {
          int maximum = player.get_MaxHP();
          if (maximum > 0) { ratio = Mathf.Clamp((float)currentHp / maximum, 0f, 1f); }
        }
        string label = null;
        if ((flags & 2) != 0) {
          string full = player.get_NickName();
          if (full == null) { full = "Unknown"; }
          if (player.IsClientBot) { full = "[BOT] " + full; }
        if (fullNames[drawn] != full) {
          fullNames[drawn] = full;
          shortNames[drawn] = (full.Length > 24 ? full.Substring(0, 24) : full).ToUpper();
          }
        label = shortNames[drawn];
        }
        string distanceLabel = null;
        if ((flags & 8) != 0 && hasOrigin) {
          int d = (int)meters;
        distanceLabel = distanceLabels[d];
          if (distanceLabel == null) {
            distanceLabel = d.ToString() + " m";
          distanceLabels[d] = distanceLabel;
          }
        }
        // Publish a complete record only after every player call has succeeded.
        S.R7BackInfo[drawn] = null;
        if ((flags & (262144 | 524288)) != 0) {
          int bits = 0;
          try {
            if ((flags & 262144) != 0) {
              if (player.IsFiring()) { bits |= 1; }
              if (player.IsReloading()) { bits |= 2; }
              if (player.IsCrouching()) { bits |= 4; }
              if (knocked) { bits |= 8; }
            }
            if ((flags & 524288) != 0) {
              if (player.get_HasVest()) { bits |= 16; }
              if (player.get_HasHelmet()) { bits |= 32; }
            }
            if (S.R7InfoCache[drawn] == null || S.R7InfoBits[drawn] != bits) {
              string detail = "";
              if ((bits & 1) != 0) { detail += "FIRE "; }
              if ((bits & 2) != 0) { detail += "RELOAD "; }
              if ((bits & 4) != 0) { detail += "CROUCH "; }
              if ((bits & 8) != 0) { detail += "KNOCK "; }
              if ((bits & 16) != 0) { detail += "VEST "; }
              if ((bits & 32) != 0) { detail += "HELMET"; }
              S.R7InfoBits[drawn] = bits; S.R7InfoCache[drawn] = detail;
            }
            S.R7BackInfo[drawn] = S.R7InfoCache[drawn];
          } catch (Exception error) { RecordError(error, now); }
        }
        // Optional bone reads remain in the scheduled sampler, never Render.
        if ((flags & 131072) != 0 && skeletonCount < 8 && meters <= 120f &&
            width > 2f && height > 2f && Time.get_realtimeSinceStartup() - started < 0.008f) {
          SampleSkeleton(player, head, drawn);
          skeletonCount++;
        }
        int at = drawn * 5;
      records[at] = x; records[at + 1] = y;
      records[at + 2] = width; records[at + 3] = height;
      records[at + 4] = ratio;
      recordNames[drawn] = label;
      recordDistances[drawn] = distanceLabel;
      recordKnocked[drawn] = knocked;
      recordTeam[drawn] = teammate;
      recordTarget[drawn] = player == S.OkAimTarget;
      S.R7BackMeters[drawn] = meters;
      S.R7Ids[drawn] = player.GetInstanceID();
      S.R7RootTf[drawn] = root; S.R7HeadTf[drawn] = head;
      S.R7BackPlayers[drawn] = player;
      int worldAt = drawn * 6;
      S.R7World[worldAt] = feet.x; S.R7World[worldAt + 1] = feet.y; S.R7World[worldAt + 2] = feet.z;
      S.R7World[worldAt + 3] = top.x; S.R7World[worldAt + 4] = top.y; S.R7World[worldAt + 5] = top.z;
        drawn++;
      } catch (Exception error) { RecordError(error, now); }
    }
    SampleVehicles(match, camera, origin, hasOrigin, h, flags, now);
    for (int i = drawn; i < 16; i++) {
    recordNames[i] = null; recordDistances[i] = null;
    fullNames[i] = null; shortNames[i] = null;
    S.R7RootTf[i] = null; S.R7HeadTf[i] = null; S.R7BackPlayers[i] = null;
    S.R7Ids[i] = 0; S.R7BackMeters[i] = 0f;
    S.R7BackKnocked[i] = false; S.R7BackTeam[i] = false; S.R7BackTarget[i] = false;
    int staleAt = i * 5;
    for (int k = 0; k < 5; k++) { records[staleAt + k] = 0f; }
    int staleWorld = i * 6;
    for (int k = 0; k < 6; k++) { S.R7World[staleWorld + k] = 0f; }
    }
    if (drawn <= 0) {
      S.R7Smooth = false; S.R7PrevCount = 0; S.R7WorldPrevCount = 0;
      S.R7IdsPrevCount = 0;
    }
    int prevCopy = S.R7Count;
    if (prevCopy > 16) { prevCopy = 16; }
    if (prevCopy > 0) {
      for (int k = 0; k < prevCopy * 5; k++) { S.R7Prev[k] = S.R7Front[k]; }
    }
    S.R7PrevCount = prevCopy;
    S.R7PrevStamp = S.R7SampleStamp;
    S.R7Smooth = true;
    float[] old = S.R7Front; S.R7Front = S.R7Back; S.R7Back = old;
    string[] names = S.R7Names; S.R7Names = S.R7BackNames; S.R7BackNames = names;
    string[] distances = S.R7Distances; S.R7Distances = S.R7BackDistances; S.R7BackDistances = distances;
    bool[] knockedBuffer = S.R7Knocked; S.R7Knocked = S.R7BackKnocked; S.R7BackKnocked = knockedBuffer;
    bool[] teamBuffer = S.R7Team; S.R7Team = S.R7BackTeam; S.R7BackTeam = teamBuffer;
    bool[] targetBuffer = S.R7Target; S.R7Target = S.R7BackTarget; S.R7BackTarget = targetBuffer;
    float[] metersBuffer = S.R7Meters; S.R7Meters = S.R7BackMeters; S.R7BackMeters = metersBuffer;
    Transform[] boneTfBuffer = S.R7BoneTf; S.R7BoneTf = S.R7BackBoneTf; S.R7BackBoneTf = boneTfBuffer;
    string[] infoBuffer = S.R7Info; S.R7Info = S.R7BackInfo; S.R7BackInfo = infoBuffer;
    int[] visibilityBuffer = S.R7Visibility; S.R7Visibility = S.R7BackVisibility; S.R7BackVisibility = visibilityBuffer;
    float[] directionBuffer = S.R7Directions; S.R7Directions = S.R7BackDirections; S.R7BackDirections = directionBuffer;
    bool[] directionValid = S.R7DirectionValid; S.R7DirectionValid = S.R7BackDirectionValid; S.R7BackDirectionValid = directionValid;
    bool[] threatBuffer = S.R7Threat; S.R7Threat = S.R7BackThreat; S.R7BackThreat = threatBuffer;
    float[] radarBuffer = S.R7Radar; S.R7Radar = S.R7BackRadar; S.R7BackRadar = radarBuffer;
    bool[] botBuffer = S.R7Bots; S.R7Bots = S.R7BackBots; S.R7BackBots = botBuffer;
    Player[] playerBuffer = S.R7Players; S.R7Players = S.R7BackPlayers; S.R7BackPlayers = playerBuffer;
    float[] vehicleBuffer = S.R7VehFront; S.R7VehFront = S.R7VehBack; S.R7VehBack = vehicleBuffer;
    for (int i = drawn; i < 16; i++) { S.R7Team[i] = false; S.R7Target[i] = false; }
    S.R7Count = drawn; S.R7WorldCount = drawn; S.R7IdsCount = drawn;
    S.R7Visited = visited; S.R7SampleStamp = now;
  S.R8MeshDirty = true; S.R8ColorsDirty = true;
    float elapsed = Time.get_realtimeSinceStartup() - started;
    S.R7SampleMs = elapsed * 1000f;
    if (S.R7Samples < 1000000000) { S.R7Samples++; }
    float delay = 0.04f;
    if (elapsed > 0.012f) { delay = 0.08f; }
    else if (elapsed > 0.008f) { delay = 0.06f; }
    if (delay < S.R7HeapDelay) { delay = S.R7HeapDelay; }
    S.R7NextSample = Time.get_realtimeSinceStartup() + delay;
  }

  private static Transform SkeletonTransform(Player player, Transform head, int bone) {
    if (bone == 0) { return head; }
    if (bone == 1) { return player.get_NeckBone(); }
    if (bone == 2) { return player.get_SpineBoneTransform(); }
    if (bone == 3) { return player.GetHipTF(); }
    if (bone == 4) { return player.get_ArmBoneLeft(); }
    if (bone == 5) { return player.get_LeftForeArmTF(); }
    if (bone == 6) { return player.get_LeftHandTF(); }
    if (bone == 7) { return player.get_ArmBoneRight(); }
    if (bone == 8) { return player.get_RightForeArmTF(); }
    if (bone == 9) { return player.get_RightHandTF(); }
    if (bone == 10) { return player.get_LegBoneLeft(); }
    if (bone == 11) { return player.GetLeftAnkleTF(); }
    if (bone == 12) { return player.get_FootBoneLeft(); }
    if (bone == 13) { return player.get_LegBoneRight(); }
    if (bone == 14) { return player.GetRightAnkleTF(); }
    return player.get_FootBoneRight();
  }

  private static Vector3 AimForward(Quaternion q) {
    float norm = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
    if (!(norm > 0.0001f && norm < 100f)) { return new Vector3(0f, 0f, 0f); }
    float scale = 2f / norm;
    return new Vector3(scale * (q.x * q.z + q.w * q.y),
        scale * (q.y * q.z - q.w * q.x), 1f - scale * (q.x * q.x + q.y * q.y));
  }

  private static int SampleVisibility(Vector3 origin, Vector3 point,
      Transform targetRoot, Transform localRoot) {
    try {
      if (!Valid(origin) || !Valid(point)) { return 0; }
      for (int pass = 0; pass < 3; pass++) {
        float distance = Vector3.Distance(origin, point);
        if (!(distance > 0.001f)) { return 0; }
        Vector3 direction = Vector3.Normalize(point - origin);
        RaycastHit hit;
        if (!Physics.Raycast(origin, direction, out hit, distance, -1,
            QueryTriggerInteraction.Ignore)) { return 1; }
        Collider collider = hit.get_collider();
        if (collider == null) { return 0; }
        Transform transform = collider.get_transform();
        if (transform == null) { return 0; }
        if (transform == targetRoot || transform.IsChildOf(targetRoot)) { return 1; }
        if (localRoot == null || !(transform == localRoot || transform.IsChildOf(localRoot))) { return -1; }
        float advance = hit.get_distance() + 0.02f;
        if (!(advance > 0f && advance < distance)) { return 0; }
        origin = new Vector3(origin.x + direction.x * advance,
            origin.y + direction.y * advance, origin.z + direction.z * advance);
      }
    } catch (Exception) { return 0; }
    return 0;
  }

  private static Color EspColor(int slot, int flags, bool knocked, bool team, bool target) {
    if (target) { return S.R7TargetColor; }
    if (knocked) { return new Color(1f, 0.18f, 0.18f, 0.95f); }
    if (team) { return S.R7TeamColor; }
    if ((flags & 1048576) != 0) {
      if (S.R7Visibility[slot] == 1) { return S.R7VisibleColor; }
      if (S.R7Visibility[slot] == -1) { return S.R7HiddenColor; }
    }
    return S.R7EnemyColor;
  }

  private static void SampleSkeleton(Player player, Transform head, int slot) {
    float started = Time.get_realtimeSinceStartup();
    for (int bone = 0; bone < 16; bone++) {
      // A separate small budget prevents optional ESP from monopolizing a frame.
      if (Time.get_realtimeSinceStartup() - started >= 0.002f) { break; }
      int index = slot * 16 + bone;
      try {
        Transform transform = SkeletonTransform(player, head, bone);
        if (transform == null) { continue; }
        Vector3 position = transform.get_position();
        if (!Valid(position)) { continue; }
        S.R7BackBoneTf[index] = transform;
      } catch (Exception) { S.R7BackBoneTf[index] = null; }
    }
  }

  private static void SampleVehicles(COW.GamePlay.JMAGGLCNGIG match, Camera camera,
      Vector3 origin, bool hasOrigin, int h, int flags, float now) {
    S.R7VehCount = 0;
    if ((flags & 16384) == 0) { return; }
    try {
      SetPhase("vehicles");
      System.Collections.IDictionary vehicles = match.LGNHODMMCMJ;
      if (vehicles == null) { return; }
      float[] output = S.R7VehBack;
      int count = 0;
      foreach (object value in vehicles.Values) {
        if (count >= 16) { break; }
        COW.GamePlay.LevelVehicle vehicle = value as COW.GamePlay.LevelVehicle;
        if (vehicle == null) { continue; }
        Transform transform = vehicle.get_transform();
        if (transform == null) { continue; }
        Vector3 position = transform.get_position();
        if (!Valid(position)) { continue; }
        Vector3 screen = camera.WorldToScreenPoint(position);
        if (!Valid(screen) || screen.z <= 0.1f) { continue; }
        float distance = hasOrigin ? Vector3.Distance(origin, position) : 0f;
        if (!(distance >= 0f && distance <= 150f)) { continue; }
        int at = count * 5;
        output[at] = screen.x; output[at + 1] = (float)h - screen.y;
        output[at + 2] = distance;
        int maxHp = vehicle.get_MaxHP();
        output[at + 3] = maxHp > 0 ? Mathf.Clamp((float)vehicle.get_CurHP() / maxHp, 0f, 1f) : 0f;
        output[at + 4] = 0f;
        count++;
      }
      S.R7VehCount = count;
    } catch (Exception error) {
      S.R7VehCount = 0;
      RecordError(error, now);
    }
  }

  private static void ProjectCached(Camera camera, int w, int h, float now) {
    S.R7LiveValid = false;
    int count = S.R7WorldCount;
    S.R7LiveCount = 0;
    if (camera == null || count <= 0) { return; }
    try {
      int live = 0;
      for (int i = 0; i < count && i < 16; i++) {
        S.R7Offscreen[i * 3 + 2] = 0f;
        S.R7OffscreenDistance[i] = null;
        int outAt = i * 5;
        S.R7Live[outAt] = 0f; S.R7Live[outAt + 1] = 0f;
        S.R7Live[outAt + 2] = 0f; S.R7Live[outAt + 3] = 0f;
        S.R7Live[outAt + 4] = S.R7Front[outAt + 4];
        S.R7HeadScreen[i * 3 + 2] = 0f;
        for (int b = 0; b < 16; b++) { S.R7SkeletonValid[i * 16 + b] = false; }
        Player tracked = S.R7Players[i];
        if (tracked == null) { continue; }
        try {
          if (tracked.get_CurHP() <= 0) { continue; }
          GameObject trackedObject = tracked.get_gameObject();
          if (trackedObject == null || !trackedObject.get_activeInHierarchy()) { continue; }
          S.R7Bots[i] = tracked.IsClientBot;
        } catch (Exception) { continue; }
        Transform rt = S.R7RootTf[i];
        Transform ht = S.R7HeadTf[i];
        if (rt == null || ht == null) { continue; }
        float fx, fy, fz, tx, ty, tz;
        try {
          Vector3 lf = rt.get_position();
          Vector3 lt = ht.get_position() + new Vector3(0f, 0.2f, 0f);
          if (!Valid(lf) || !Valid(lt)) { continue; }
          fx = lf.x; fy = lf.y; fz = lf.z;
          tx = lt.x; ty = lt.y; tz = lt.z;
        } catch (Exception) { continue; }
        Vector3 sf = camera.WorldToScreenPoint(new Vector3(fx, fy, fz));
        Vector3 sh = camera.WorldToScreenPoint(new Vector3(tx, ty, tz));
        if ((S.R7Flags & 33554432) != 0 && Valid(sh) &&
            (sh.z <= 0.1f || sh.x < 0f || sh.x > w || sh.y < 0f || sh.y > h)) {
          float dx = sh.x - (float)w * 0.5f;
          float dy = (float)h * 0.5f - sh.y;
          if (sh.z <= 0.1f) { dx = -dx; dy = -dy; }
          if (Mathf.Abs(dx) + Mathf.Abs(dy) < 1f) { dy = (float)h; }
          S.R7Offscreen[i * 3] = (float)w * 0.5f + dx;
          S.R7Offscreen[i * 3 + 1] = (float)h * 0.5f + dy;
          S.R7Offscreen[i * 3 + 2] = 1f;
          int distance = (int)Mathf.Clamp(S.R7Meters[i], 0f, 150f);
          if (S.R7DistanceLabels[distance] == null) { S.R7DistanceLabels[distance] = distance.ToString() + " m"; }
          S.R7OffscreenDistance[i] = S.R7DistanceLabels[distance];
        }
        if (!Valid(sf) || !Valid(sh) || sf.z <= 0.1f || sh.z <= 0.1f) { continue; }
        float bottom = (float)h - sf.y;
        float height = (sh.y - sf.y) * 1.0909f;
        if (!(height > 0f && height < (float)h * 4f)) { continue; }
        float width = height * 0.5f;
        S.R7Live[outAt] = (sf.x + sh.x) * 0.5f - width * 0.5f;
        S.R7Live[outAt + 1] = bottom - height;
        S.R7Live[outAt + 2] = width;
        S.R7Live[outAt + 3] = height;
        if ((S.R7Flags & 131072) != 0) {
          for (int b = 0; b < 16; b++) {
            Transform boneTf = S.R7BoneTf[i * 16 + b];
            if (boneTf == null) { continue; }
            try {
              Vector3 boneWorld = boneTf.get_position();
              if (!Valid(boneWorld)) { continue; }
              Vector3 boneScreen = camera.WorldToScreenPoint(boneWorld);
              if (!Valid(boneScreen) || boneScreen.z <= 0.1f) { continue; }
              int si = (i * 16 + b) * 2;
              S.R7Skeleton[si] = boneScreen.x;
              S.R7Skeleton[si + 1] = (float)h - boneScreen.y;
              S.R7SkeletonValid[i * 16 + b] = true;
            } catch (Exception) { }
          }
        }
        if ((S.R7Flags & 65536) != 0) {
          Vector3 center = camera.WorldToScreenPoint(new Vector3(tx, ty - 0.2f, tz));
          if (Valid(center) && center.z > 0.1f) {
            S.R7HeadScreen[i * 3] = center.x;
            S.R7HeadScreen[i * 3 + 1] = (float)h - center.y;
            S.R7HeadScreen[i * 3 + 2] = Mathf.Clamp(width * 0.22f, 4f, 26f);
          }
        }
        // Projection can reject slots (behind camera/invalid depth). Compact
        // every surviving record and all parallel metadata before Render reads
        // the live arrays. Without this, liveCount N could render slot 0 while
        // the first valid player was slot 1, carrying another player's name,
        // team flag or skeleton into the visible record.
        CompactProjectedSlot(i, live);
        live++;
      }
      S.R7LiveCount = live;
      S.R7LiveStamp = now;
      // If the projector ran, its zeroed invalid slots are authoritative;
      // never fall back to stale screen rectangles from an earlier camera.
      S.R7LiveValid = live > 0;
    } catch (Exception error) {
      S.R7LiveCount = 0;
      S.R7LiveValid = false;
      S.R7Count = 0; S.R7WorldCount = 0; S.R7IdsCount = 0;
      RecordError(error, now);
    }
  }

  private static void CompactProjectedSlot(int from, int to) {
    if (from == to) { return; }
    int src = from * 5, dst = to * 5;
    for (int k = 0; k < 5; k++) { S.R7Live[dst + k] = S.R7Live[src + k]; }
    S.R7Names[to] = S.R7Names[from]; S.R7Distances[to] = S.R7Distances[from];
    S.R7Knocked[to] = S.R7Knocked[from]; S.R7Team[to] = S.R7Team[from];
    S.R7Target[to] = S.R7Target[from]; S.R7Meters[to] = S.R7Meters[from];
    S.R7Info[to] = S.R7Info[from]; S.R7InfoBits[to] = S.R7InfoBits[from];
    S.R7InfoCache[to] = S.R7InfoCache[from]; S.R7Ids[to] = S.R7Ids[from];
    S.R7Visibility[to] = S.R7Visibility[from]; S.R7Threat[to] = S.R7Threat[from];
    S.R7Bots[to] = S.R7Bots[from]; S.R7DirectionValid[to] = S.R7DirectionValid[from];
    S.R7OffscreenDistance[to] = S.R7OffscreenDistance[from];
    for (int k = 0; k < 4; k++) { S.R7Directions[to * 4 + k] = S.R7Directions[from * 4 + k]; }
    for (int k = 0; k < 3; k++) {
      S.R7Radar[to * 3 + k] = S.R7Radar[from * 3 + k];
      S.R7Offscreen[to * 3 + k] = S.R7Offscreen[from * 3 + k];
      S.R7HeadScreen[to * 3 + k] = S.R7HeadScreen[from * 3 + k];
    }
    for (int k = 0; k < 32; k++) { S.R7Skeleton[to * 32 + k] = S.R7Skeleton[from * 32 + k]; }
    for (int k = 0; k < 16; k++) { S.R7SkeletonValid[to * 16 + k] = S.R7SkeletonValid[from * 16 + k]; }
    for (int k = 0; k < 16; k++) { S.R7BoneTf[to * 16 + k] = S.R7BoneTf[from * 16 + k]; }
    S.R7RootTf[to] = S.R7RootTf[from]; S.R7HeadTf[to] = S.R7HeadTf[from];
    S.R7Players[to] = S.R7Players[from];
  }

  private static void Render(Texture2D tex, int flags) {
    SetPhase("GUI");
    if (S.R7Style == null) {
      S.R7Style = new GUIStyle();
      S.R7Style.set_fontSize(18); S.R7Style.set_alignment(1);
      S.R7Style.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
    }
    bool live = S.R7LiveValid;
    float[] records = live ? S.R7Live : S.R7Front;
    string[] names = S.R7Names, distances = S.R7Distances;
    bool[] knocked = S.R7Knocked, team = S.R7Team, target = S.R7Target;
    GUIStyle style = S.R7Style;
    float renderNow = Time.get_realtimeSinceStartup();
    int count = live ? S.R7LiveCount : 0, commands = 0, apiDraws = 0;
    if (live) { S.R8MeshDirty = true; }
    bool batched = false;
    if (count > 0 && S.R8BatchOn) {
      batched = DrawBatch(records, knocked, team, target, count, flags);
      if (batched && S.R8QuadCount > 0) { apiDraws = 1; }
    }
    S.R8BatchActive = batched;
    float targetPulse = 0.86f + 0.14f * Mathf.Sin(renderNow * 5.2f);
    int threatCount = 0;
    for (int i = 0; i < count && i < 16; i++) {
      if ((flags & 2097152) != 0 && S.R7Visibility[i] != 1) { continue; }
      if ((flags & 8388608) != 0 && S.R7Threat[i]) { threatCount++; }
      if ((flags & 33554432) != 0 && S.R7Offscreen[i * 3 + 2] > 0f && !team[i]) {
        Color arrowColor = EspColor(i, flags, knocked[i], team[i], target[i]);
        float pulse = 0.8f + 0.2f * Mathf.Sin(renderNow * 4f);
        arrowColor = new Color(arrowColor.r, arrowColor.g, arrowColor.b, pulse);
        int arrows = DrawOffscreenArrow((float)S.R7Width, (float)S.R7Height,
            S.R7Offscreen[i * 3], S.R7Offscreen[i * 3 + 1], arrowColor, tex);
        commands += arrows; apiDraws += arrows;
        DrawArrowDistance(i, style);
      }
    }
    S.R7ThreatCount = threatCount;
    if (threatCount > 0) {
      GUI.set_color(new Color(1f, 1f, 1f, 1f));
      style.set_fontSize((int)S.R7FontSize); style.set_alignment(1);
      style.get_normal().set_textColor(new Color(1f, 0.3f, 0.25f, 1f));
      GUI.Label(new Rect((float)S.R7Width * 0.5f - 260f, 44f, 520f, 40f),
          "NHIN VE BAN (~10deg): " + threatCount.ToString(), style);
      commands++; apiDraws++;
    }
    if ((flags & 16777216) != 0) { DrawRadar(tex, count, team, knocked, target, flags, style, records); }
    for (int i = 0; i < count && i < 16; i++) {
      if ((flags & 2097152) != 0 && S.R7Visibility[i] != 1) { continue; }
      int at = i * 5;
      float x = records[at]; float y = records[at + 1];
      float width = records[at + 2]; float height = records[at + 3];
      float ratio = records[at + 4];
      if (!(width > 0f) || !(height > 0f)) { continue; }
      float cx = x + width * 0.5f;
      float bottom = y + height;
      float meters = S.R7Meters[i];
      bool hideInfo = (flags & 134217728) != 0 && meters < 35f;
      float fade = 1f;
      if (meters > 60f) { fade = 1f - Mathf.Clamp((meters - 60f) / 90f, 0f, 0.25f); }
      Color color = EspColor(i, flags, knocked[i], team[i], target[i]);
      color = new Color(color.r, color.g, color.b, color.a * fade);
      if (target[i]) { color = new Color(color.r, color.g, color.b, color.a * targetPulse); }
      float sw = (float)S.R7Width, sh = (float)S.R7Height;
      if (x + width < 0f || x > sw || y + height < 0f || y > sh) {
        continue;
      }
      GUI.set_color(color);
      if (!batched) {
        if ((flags & 8192) != 0) {
          GUI.set_color(new Color(color.r, color.g, color.b, 0.22f));
          GUI.DrawTexture(new Rect(x, y, width, height), tex); commands++; apiDraws++;
        }
        if ((flags & 128) != 0) {
          GUI.set_color(new Color(0f, 0f, 0f, 0.32f));
          GUI.DrawTexture(new Rect(x, y, width, height), tex); commands++; apiDraws++;
        }
        if ((flags & (1 | 64 | 128 | 8192)) != 0 || team[i]) {
          GuiBox(x, y, width, height, 2.0f, color, tex); commands += 4; apiDraws += 4;
        }
        if ((flags & 64) != 0) {
          float depth = width * 0.25f;
          GuiBox(x - depth, y - depth, width, height, 1.2f, color, tex); commands += 4; apiDraws += 4;
          GuiLine(x, y, x - depth, y - depth, 1.2f, color, tex);
          GuiLine(x + width, y, x + width - depth, y - depth, 1.2f, color, tex);
          GuiLine(x, bottom, x - depth, bottom - depth, 1.2f, color, tex);
          GuiLine(x + width, bottom, x + width - depth, bottom - depth, 1.2f, color, tex);
          commands += 4; apiDraws += 4;
        }
        if ((flags & 16) != 0) {
          GuiLine((float)S.R7Width * 0.5f, 0f, cx, y, 2.0f, color, tex); commands++; apiDraws++;
        }
        if ((flags & 65536) != 0) {
          float headR = S.R7HeadScreen[i * 3 + 2];
          float hcx = S.R7HeadScreen[i * 3], hcy = S.R7HeadScreen[i * 3 + 1];
          float hpx = hcx + headR; float hpy = hcy;
          for (int seg = 1; seg <= 16; seg++) {
            float ang = seg * 0.39269908f;
            float hnx = hcx + Mathf.Sin(ang + 1.570796327f) * headR;
            float hny = hcy + Mathf.Sin(ang) * headR;
            GuiLine(hpx, hpy, hnx, hny, 2.6f, color, tex);
            hpx = hnx; hpy = hny;
          }
          commands += 16; apiDraws += 16;
        }
        if ((flags & 131072) != 0) {
          int edges = GuiSkeleton(i, color, tex);
          commands += edges; apiDraws += edges;
        }
        if ((flags & 256) != 0 && !knocked[i]) {
          Color mark = new Color(1f, 0.15f, 0.15f, 0.95f);
          GuiLine(cx - 6f, y - 16f, cx + 6f, y - 16f, 1.2f, mark, tex);
          GuiLine(cx - 6f, y - 16f, cx, y - 4f, 1.2f, mark, tex);
          GuiLine(cx + 6f, y - 16f, cx, y - 4f, 1.2f, mark, tex);
          commands += 3; apiDraws += 3;
        }
        if ((flags & 512) != 0) {
          GuiLine((float)S.R7Width * 0.5f, (float)S.R7Height * 0.5f, cx, bottom, 3.0f,
              new Color(1f, 1f, 1f, 0.35f), tex); commands++; apiDraws++;
        }
        if ((flags & 2048) != 0) {
          GuiLine((float)S.R7Width * 0.5f, (float)S.R7Height, cx, bottom, 3.2f,
              new Color(0.15f, 0.5f, 1f, 0.95f), tex); commands++; apiDraws++;
        }
        if ((flags & 4) != 0 && !knocked[i]) {
          GUI.set_color(new Color(0f, 0f, 0f, 0.6f));
          GUI.DrawTexture(new Rect(x - 7f, y, 3f, height), tex); apiDraws++;
          GUI.set_color(new Color(0.15f, 0.95f, 0.25f, 0.95f));
          if (ratio > 0f) { GUI.DrawTexture(new Rect(x - 7f, y + height - height * ratio, 3f, height * ratio), tex); apiDraws++; }
          commands += 2;
          if ((flags & 67108864) != 0) {
            for (int segment = 1; segment < 7; segment++) {
              float sy = y + height * (float)segment / 7f;
              GuiLine(x - 8f, sy, x - 3f, sy, 1f, new Color(0f, 0f, 0f, 0.8f), tex);
            }
          }
        }
      }
      GUI.set_color(new Color(1f, 1f, 1f, 1f));
      int nameFont = (int)S.R7FontSize;
      if (nameFont < 16) { nameFont = 24; }
      int distFont = nameFont - 4;
      if (distFont < 12) { distFont = 12; }
      int hpFont = nameFont - 6;
      if (hpFont < 12) { hpFont = 12; }
      Color nameText = S.R7NameColor;
      if (!hideInfo && (flags & 2) != 0 && names[i] != null) {
        style.set_fontSize(nameFont); style.set_alignment(1);
        float pillH = (float)nameFont + 8f;
        float pillW = Mathf.Clamp((float)names[i].Length * (float)nameFont * 0.58f + 18f, 60f, 340f);
        float pillX = cx - pillW * 0.5f;
        if (pillX < 4f) { pillX = 4f; }
        if (pillX + pillW > sw - 4f) { pillX = sw - 4f - pillW; }
        float pillY = y - pillH - 6f;
        if (pillY < 2f) { pillY = 2f; }
        GUI.set_color(new Color(color.r, color.g, color.b, 0.30f));
        GUI.DrawTexture(new Rect(pillX - 1f, pillY - 1f, pillW + 2f, pillH + 2f), tex);
        GUI.set_color(new Color(0.02f, 0.03f, 0.06f, 0.72f));
        GUI.DrawTexture(new Rect(pillX, pillY, pillW, pillH), tex);
        GUI.set_color(new Color(1f, 1f, 1f, 1f));
        style.get_normal().set_textColor(new Color(0f, 0f, 0f, 0.9f));
        GUI.Label(new Rect(pillX + 1f, pillY + 1f, pillW, pillH), names[i], style);
        style.get_normal().set_textColor(nameText);
        GUI.Label(new Rect(pillX, pillY, pillW, pillH), names[i], style);
        commands += 4; apiDraws += 4;
      }
      if (!hideInfo && (flags & 8) != 0 && distances[i] != null) {
        style.set_fontSize(distFont); style.set_alignment(1);
        float pillH = (float)distFont + 8f;
        float pillW = Mathf.Clamp((float)distances[i].Length * (float)distFont * 0.45f + 16f, 46f, 150f);
        float pillX = cx - pillW * 0.5f;
        if (pillX < 4f) { pillX = 4f; }
        if (pillX + pillW > sw - 4f) { pillX = sw - 4f - pillW; }
        float pillY = bottom + 4f;
        if (pillY + pillH > sh - 2f) { pillY = bottom - height - pillH - 3f; }
        GUI.set_color(new Color(color.r, color.g, color.b, 0.26f));
        GUI.DrawTexture(new Rect(pillX - 1f, pillY - 1f, pillW + 2f, pillH + 2f), tex);
        GUI.set_color(new Color(0.02f, 0.03f, 0.06f, 0.72f));
        GUI.DrawTexture(new Rect(pillX, pillY, pillW, pillH), tex);
        GUI.set_color(new Color(1f, 1f, 1f, 1f));
        style.get_normal().set_textColor(new Color(0f, 0f, 0f, 0.9f));
        GUI.Label(new Rect(pillX + 1f, pillY + 1f, pillW, pillH), distances[i], style);
        style.get_normal().set_textColor(target[i] ? new Color(1f, 0.86f, 0.45f, 1f)
            : new Color(0.92f, 0.96f, 1f, 1f));
        GUI.Label(new Rect(pillX, pillY, pillW, pillH), distances[i], style);
        commands += 4; apiDraws += 4;
      }
      if (!hideInfo && (flags & 4) != 0 && !knocked[i] && (target[i] || ratio <= 0.5f)) {
        style.set_fontSize(hpFont); style.set_alignment(1);
        string hpText = ((int)(ratio * 100f)).ToString();
        float hpY = y + height - (float)hpFont - 3f;
        style.get_normal().set_textColor(new Color(0f, 0f, 0f, 0.9f));
        GUI.Label(new Rect(x + 3f, hpY + 1f, 56f, (float)hpFont + 4f), hpText, style);
        style.get_normal().set_textColor(target[i] ? new Color(1f, 0.86f, 0.45f, 1f)
            : new Color(1f, 1f, 1f, 1f));
        GUI.Label(new Rect(x + 2f, hpY, 56f, (float)hpFont + 4f), hpText, style);
        commands += 2; apiDraws += 2;
      }
      if (!hideInfo && (flags & (262144 | 524288)) != 0 && S.R7Info[i] != null && S.R7Info[i].Length > 0) {
        style.set_fontSize(distFont); style.set_alignment(1);
        float infoY = bottom + (float)distFont + 16f;
        if ((flags & 8) == 0) { infoY = bottom + 4f; }
        if (infoY + (float)distFont + 4f < sh) {
          GUI.set_color(new Color(1f, 1f, 1f, 1f));
          style.get_normal().set_textColor(new Color(0f, 0f, 0f, 0.95f));
          GUI.Label(new Rect(cx - 161f, infoY + 1f, 322f, (float)distFont + 4f), S.R7Info[i], style);
          style.get_normal().set_textColor(new Color(0.9f, 0.96f, 1f, 1f));
          GUI.Label(new Rect(cx - 162f, infoY, 322f, (float)distFont + 4f), S.R7Info[i], style);
          commands += 2; apiDraws += 2;
        }
      }
    }
    int hudFont = (int)S.R7FontSize;
    if (hudFont < 16) { hudFont = 24; }
    style.set_fontSize(hudFont - 4); style.set_alignment(1);
    if ((flags & 16384) != 0 && S.R7VehCount > 0) {
      float[] vehicles = S.R7VehFront;
      for (int i = 0; i < S.R7VehCount && i < 16; i++) {
        int at = i * 5;
        float vx = vehicles[at], vy = vehicles[at + 1], vd = vehicles[at + 2];
        float ratio = vehicles[at + 3];
        GuiLine((float)S.R7Width * 0.5f, (float)S.R7Height, vx, vy, 1.4f, S.R7VehColor, tex);
        style.get_normal().set_textColor(S.R7VehColor);
        GUI.Label(new Rect(vx - 110f, vy - 34f, 220f, 30f),
            "Xe " + ((int)vd).ToString() + "m", style);
        GUI.set_color(new Color(0f, 0f, 0f, 0.65f));
        GUI.DrawTexture(new Rect(vx - 20f, vy + 4f, 40f, 4f), tex);
        GUI.set_color(new Color(0.2f, 1f, 0.3f, 0.95f));
        GUI.DrawTexture(new Rect(vx - 20f, vy + 4f, 40f * ratio, 4f), tex);
        commands += 4;
      }
    }
    if ((flags & 32768) != 0 && count > 0) {
      int enemies = 0;
      for (int i = 0; i < count && i < 16; i++) {
        if ((flags & 2097152) != 0 && S.R7Visibility[i] != 1) { continue; }
        if (records[i * 5 + 2] > 0f && !team[i] && !knocked[i]) { enemies++; }
      }
      if (enemies > 0) {
        style.set_fontSize(hudFont); style.set_alignment(1);
        style.get_normal().set_textColor(new Color(1f, 0.25f, 0.25f, 1f));
        GUI.Label(new Rect((float)S.R7Width * 0.5f - 200f, 10f, 400f, 28f),
            "CANH BAO: " + enemies.ToString() + " DICH", style);
        commands++;
      }
    }
    S.R7Draws = commands; S.R7DrawBudget = 128 - commands; S.R8ApiDraws = apiDraws;
  }

  private static Color LoadColor(string key, float r, float g, float b, float a) {
    return new Color(PlayerPrefs.GetFloat(key + ".R", r),
        PlayerPrefs.GetFloat(key + ".G", g),
        PlayerPrefs.GetFloat(key + ".B", b),
        PlayerPrefs.GetFloat(key + ".A", a));
  }

  private static void RecordError(Exception error, float now) {
    if (S.R7Errors < 1000000000) { S.R7Errors++; }
    if (now < S.R7ErrorAfter) { return; }
    S.R7ErrorAfter = now + 1f;
  string phase = S.R8Phase == null ? "unknown" : S.R8Phase;
  PlayerPrefs.SetString("OKMOD.R7.LastError", "phase=" + phase + " | " + error.ToString());
  PlayerPrefs.SetString("OKMOD.R8.Phase", phase);
    PlayerPrefs.SetInt("OKMOD.R7.Errors", S.R7Errors);
    // Error persistence is throttled; never flush on every player/frame.
    if (now >= S.R7SaveAfter) { PlayerPrefs.Save(); S.R7SaveAfter = now + 5f; }
  }

private static bool DrawBatch(float[] records, bool[] knocked, bool[] team,
    bool[] target, int count, int flags) {
  float now = Time.get_realtimeSinceStartup();
  if (now < S.R8BatchRetry) { return false; }
  try {
    SetPhase("mesh-build");
    Mesh mesh = S.R8Mesh;
    Material material = S.R8Material;
    if (mesh == null || material == null) {
      ReleaseBatch();
      Shader shader = Shader.Find("Hidden/Internal-Colored");
      if (shader == null) { S.R8BatchRetry = now + 5f; return false; }
      material = new Material(shader); S.R8Material = material;
      material.SetInt("_SrcBlend", 5); material.SetInt("_DstBlend", 10);
      material.SetInt("_Cull", 0); material.SetInt("_ZWrite", 0); material.SetInt("_ZTest", 8);
      mesh = new Mesh(); S.R8Mesh = mesh; mesh.MarkDynamic();
      if (S.R8Vertices == null || S.R8Indices == null || S.R8Colors == null ||
          S.R8Vertices.Length < 4096 || S.R8Indices.Length < 6144 ||
          S.R8Colors.Length < 4096) {
        S.R8Vertices = new Vector3[4096]; S.R8Colors = new Color[4096]; S.R8Indices = new int[6144];
        int[] triangles = S.R8Indices;
        for (int i = 0; i < 1024; i++) {
          int v = i * 4, t = i * 6;
          triangles[t] = v; triangles[t + 1] = v + 1; triangles[t + 2] = v + 2;
          triangles[t + 3] = v; triangles[t + 4] = v + 2; triangles[t + 5] = v + 3;
        }
      }
      mesh.set_vertices(S.R8Vertices); mesh.set_triangles(S.R8Indices);
      S.R8MeshDirty = true; S.R8ColorsDirty = true;
    }
    if (S.R8MeshDirty) {
      Vector3[] vertices = S.R8Vertices;
      Color[] colors = S.R8Colors;
      int quads = 0;
      float screenW = (float)S.R7Width, screenH = (float)S.R7Height;
      for (int i = 0; i < count && i < 16; i++) {
        if ((flags & 2097152) != 0 && S.R7Visibility[i] != 1) { continue; }
        int at = i * 5;
        float x = records[at], y = records[at + 1], w = records[at + 2], h = records[at + 3];
        if (!(w > 0f) || !(h > 0f)) { continue; }
        float cx = x + w * 0.5f;
        float bottom = y + h;
        float meters = S.R7Meters[i];
        float fade = 1f;
        if (meters > 60f) { fade = 1f - Mathf.Clamp((meters - 60f) / 90f, 0f, 0.25f); }
        Color c = EspColor(i, flags, knocked[i], team[i], target[i]);
        c = new Color(c.r, c.g, c.b, c.a * fade);
        if (target[i]) {
          float pulse = 0.86f + 0.14f * Mathf.Sin(now * 5.2f);
          c = new Color(c.r, c.g, c.b, c.a * pulse);
        }
        float corner = w * 0.26f;
        if (corner > h * 0.20f) { corner = h * 0.20f; }
        if (corner < 8f) { corner = 8f; }
        if (corner > 30f) { corner = 30f; }
        if ((flags & 1) != 0 || team[i]) {
          if (target[i]) {
            MeshCorners(vertices, colors, ref quads, x, y, w, h, corner, 6f,
                new Color(c.r, c.g, c.b, 0.25f));
          } else {
            MeshCorners(vertices, colors, ref quads, x + 1f, y + 1f, w, h, corner, 5.4f,
                new Color(0f, 0f, 0f, 0.6f));
          }
          MeshBox(vertices, colors, ref quads, x, y, w, h, 1.4f,
              new Color(c.r, c.g, c.b, 0.30f));
          MeshCorners(vertices, colors, ref quads, x, y, w, h, corner, 3.6f, c);
        }
        if ((flags & 128) != 0) {
          MeshBox(vertices, colors, ref quads, x + 1f, y + 1f, w, h, 5.4f,
              new Color(0f, 0f, 0f, 0.5f));
          MeshBox(vertices, colors, ref quads, x, y, w, h, 3.2f, c);
        }
        if ((flags & 8192) != 0) {
          Quad(vertices, colors, quads++, x, y, w, h,
              new Color(c.r, c.g, c.b, 0.24f));
        }
        if ((flags & 64) != 0) {
          float depth = w * 0.22f;
          MeshBox(vertices, colors, ref quads, x - depth, y - depth, w, h, 1.6f, c);
          MeshLine(vertices, colors, ref quads, x, y, x - depth, y - depth, 1.6f, c);
          MeshLine(vertices, colors, ref quads, x + w, y, x + w - depth, y - depth, 1.6f, c);
          MeshLine(vertices, colors, ref quads, x, bottom, x - depth, bottom - depth, 1.6f, c);
          MeshLine(vertices, colors, ref quads, x + w, bottom, x + w - depth, bottom - depth, 1.6f, c);
        }
        bool onScreen = x + w >= 0f && x <= screenW && y + h >= 0f && y <= screenH;
        if ((flags & 16) != 0 && onScreen) {
          if (target[i]) {
            MeshLine(vertices, colors, ref quads, screenW * 0.5f, 0f, cx, y, 3.0f, c);
          } else {
            MeshLine(vertices, colors, ref quads, screenW * 0.5f, 0f, cx, y, 2.2f,
                new Color(c.r, c.g, c.b, 0.45f));
          }
        }
        if ((flags & 256) != 0 && !knocked[i]) {
          Quad(vertices, colors, quads++, cx - 3f, y - 11f, 6f, 6f, c);
          MeshLine(vertices, colors, ref quads, cx - 7f, y - 8f, cx + 7f, y - 8f, 1.2f, c);
          MeshLine(vertices, colors, ref quads, cx, y - 15f, cx, y - 1f, 1.2f, c);
        }
        if ((flags & 65536) != 0) {
          float headR = S.R7HeadScreen[i * 3 + 2];
          if (headR > 0f) {
            MeshCircle(vertices, colors, ref quads, S.R7HeadScreen[i * 3],
                S.R7HeadScreen[i * 3 + 1], headR, 20, 2.8f, c);
          }
        }
        if ((flags & 131072) != 0 && onScreen) {
          MeshSkeleton(vertices, colors, ref quads, i, c);
        }
        if (target[i]) {
          float nameTop = y - S.R7FontSize - 16f;
          MeshLine(vertices, colors, ref quads, cx - 6f, nameTop - 9f, cx, nameTop - 2f, 2.2f, c);
          MeshLine(vertices, colors, ref quads, cx + 6f, nameTop - 9f, cx, nameTop - 2f, 2.2f, c);
        }
        if ((flags & 512) != 0) {
          MeshLine(vertices, colors, ref quads, screenW * 0.5f, screenH * 0.5f, cx, bottom, 3.0f,
              new Color(1f, 1f, 1f, 0.35f));
        }
        if ((flags & 2048) != 0) {
          MeshLine(vertices, colors, ref quads, screenW * 0.5f, screenH, cx, bottom, 3.2f,
              new Color(0.15f, 0.5f, 1f, 0.95f));
        }
        if ((flags & 4) != 0 && !knocked[i]) {
          float barX = x + w - 8f;
          float barW = 6f;
          Quad(vertices, colors, quads++, barX - 1f, y - 1f, barW + 2f, h + 2f,
              new Color(0f, 0f, 0f, 0.62f));
          float ratio = records[at + 4];
          if (ratio > 0f) {
            float filled = h * ratio;
            float visibleTop = y + h - filled;
            MeshHpBand(vertices, colors, ref quads, barX, y + h * 0.75f, y + h, visibleTop, y + h, barW,
                new Color(1f, 0.353f, 0.431f, 0.98f));
            MeshHpBand(vertices, colors, ref quads, barX, y + h * 0.5f, y + h * 0.75f, visibleTop, y + h, barW,
                new Color(1f, 0.82f, 0.4f, 0.98f));
            MeshHpBand(vertices, colors, ref quads, barX, y, y + h * 0.5f, visibleTop, y + h, barW,
                new Color(0.259f, 0.902f, 0.584f, 0.98f));
          }
          if ((flags & 67108864) != 0) {
            for (int segment = 1; segment < 7; segment++) {
              float sy = y + h * (float)segment / 7f;
              MeshLine(vertices, colors, ref quads, barX, sy, barX + barW, sy, 1f,
                  new Color(0f, 0f, 0f, 0.8f));
            }
          }
        }
      }
      // Unused fixed triangles reference degenerate quads, never stale geometry.
      if (quads > 1024) { quads = 1024; }
      for (int i = quads * 4; i < 4096; i++) { vertices[i] = new Vector3(0f, 0f, 0f); }
      mesh.set_vertices(vertices);
      // Topology can change between samples when a live slot goes behind the
      // camera; pulse also changes alpha. Keep vertex/color indices in sync.
      mesh.set_colors(colors); S.R8ColorsDirty = false;
      S.R8QuadCount = quads; S.R8MeshDirty = false;
      S.R8Uploads++;
    }
    if (!material.SetPass(0)) {
      ReleaseBatch(); S.R8BatchRetry = now + 5f; S.R8BatchFailures++;
      return false;
    }
    SetPhase("mesh-draw");
    GL.PushMatrix();
    try {
      GL.LoadPixelMatrix(0f, (float)S.R7Width, (float)S.R7Height, 0f);
      Graphics.DrawMeshNow(mesh, Matrix4x4.get_identity());
    } finally { GL.PopMatrix(); }
    return true;
  } catch (Exception error) {
    ReleaseBatch(); S.R8BatchRetry = now + 5f; S.R8BatchFailures++;
    RecordError(error, now);
    return false;
  }
}

private static void Quad(Vector3[] vertices, Color[] colors, int index,
    float x, float y, float w, float h, Color color) {
  if (index < 0 || index >= 1024) { return; }
  int v = index * 4;
  vertices[v] = new Vector3(x, y, 0f); vertices[v + 1] = new Vector3(x + w, y, 0f);
  vertices[v + 2] = new Vector3(x + w, y + h, 0f); vertices[v + 3] = new Vector3(x, y + h, 0f);
  colors[v] = color; colors[v + 1] = color; colors[v + 2] = color; colors[v + 3] = color;
}

private static void MeshLine(Vector3[] vertices, Color[] colors, ref int index,
    float x1, float y1, float x2, float y2, float thickness, Color color) {
  if (index < 0 || index >= 1024) { return; }
  float dx = x2 - x1, dy = y2 - y1;
  float length = Mathf.Sqrt(dx * dx + dy * dy);
  if (!(length > 0f)) { return; }
  float nx = -dy / length * thickness * 0.5f;
  float ny = dx / length * thickness * 0.5f;
  int v = index * 4;
  vertices[v] = new Vector3(x1 + nx, y1 + ny, 0f);
  vertices[v + 1] = new Vector3(x2 + nx, y2 + ny, 0f);
  vertices[v + 2] = new Vector3(x2 - nx, y2 - ny, 0f);
  vertices[v + 3] = new Vector3(x1 - nx, y1 - ny, 0f);
  colors[v] = color; colors[v + 1] = color; colors[v + 2] = color; colors[v + 3] = color;
  index++;
}

private static void MeshBox(Vector3[] vertices, Color[] colors, ref int index,
    float x, float y, float w, float h, float thickness, Color color) {
  MeshLine(vertices, colors, ref index, x, y, x + w, y, thickness, color);
  MeshLine(vertices, colors, ref index, x + w, y, x + w, y + h, thickness, color);
  MeshLine(vertices, colors, ref index, x + w, y + h, x, y + h, thickness, color);
  MeshLine(vertices, colors, ref index, x, y + h, x, y, thickness, color);
}

private static void MeshCorners(Vector3[] vertices, Color[] colors, ref int index,
    float x, float y, float w, float h, float corner, float thickness, Color color) {
  MeshLine(vertices, colors, ref index, x, y, x + corner, y, thickness, color);
  MeshLine(vertices, colors, ref index, x, y, x, y + corner, thickness, color);
  MeshLine(vertices, colors, ref index, x + w - corner, y, x + w, y, thickness, color);
  MeshLine(vertices, colors, ref index, x + w, y, x + w, y + corner, thickness, color);
  MeshLine(vertices, colors, ref index, x, y + h - corner, x, y + h, thickness, color);
  MeshLine(vertices, colors, ref index, x, y + h, x + corner, y + h, thickness, color);
  MeshLine(vertices, colors, ref index, x + w, y + h - corner, x + w, y + h, thickness, color);
  MeshLine(vertices, colors, ref index, x + w - corner, y + h, x + w, y + h, thickness, color);
}

private static void MeshCircle(Vector3[] vertices, Color[] colors, ref int index,
    float cx, float cy, float radius, int segments, float thickness, Color color) {
  float px = cx + radius;
  float py = cy;
  for (int i = 1; i <= segments; i++) {
    float angle = (float)i * 6.2831853f / (float)segments;
    float nx = cx + Mathf.Sin(angle + 1.570796327f) * radius;
    float ny = cy + Mathf.Sin(angle) * radius;
    if (i == segments) { nx = cx + radius; ny = cy; }
    MeshLine(vertices, colors, ref index, px, py, nx, ny, thickness, color);
    px = nx; py = ny;
  }
}

private static void MeshSkeleton(Vector3[] vertices, Color[] colors, ref int index,
    int slot, Color color) {
  int[] pairs = S.R7BonePairs;
  for (int edge = 0; edge < pairs.Length; edge += 2) {
    int a = slot * 16 + pairs[edge];
    int b = slot * 16 + pairs[edge + 1];
    if (!S.R7SkeletonValid[a] || !S.R7SkeletonValid[b]) { continue; }
    float ax = S.R7Skeleton[a * 2];
    float ay = S.R7Skeleton[a * 2 + 1];
    float bx = S.R7Skeleton[b * 2];
    float by = S.R7Skeleton[b * 2 + 1];
    MeshLine(vertices, colors, ref index, ax, ay, bx, by, 3.0f, color);
  }
}

private static void MeshHpBand(Vector3[] vertices, Color[] colors, ref int index,
    float x, float top, float bottom, float visibleTop, float fullBottom,
    float width, Color color) {
  float a = top;
  if (a < visibleTop) { a = visibleTop; }
  float b = bottom;
  if (b > fullBottom) { b = fullBottom; }
  if (b > a) { Quad(vertices, colors, index++, x, a, width, b - a, color); }
}

private static void MeshCorner(Vector3[] vertices, Color[] colors, ref int index,
    float x, float y, float w, float h, float corner, Color color) {
  MeshLine(vertices, colors, ref index, x, y, x + corner, y, 2.4f, color);
  MeshLine(vertices, colors, ref index, x, y, x, y + corner, 2.4f, color);
  MeshLine(vertices, colors, ref index, x + w - corner, y, x + w, y, 2.4f, color);
  MeshLine(vertices, colors, ref index, x + w, y, x + w, y + corner, 2.4f, color);
  MeshLine(vertices, colors, ref index, x, y + h - corner, x, y + h, 2.4f, color);
  MeshLine(vertices, colors, ref index, x, y + h, x + corner, y + h, 2.4f, color);
  MeshLine(vertices, colors, ref index, x + w, y + h - corner, x + w, y + h, 2.4f, color);
  MeshLine(vertices, colors, ref index, x + w - corner, y + h, x + w, y + h, 2.4f, color);
}

private static void GuiLine(float x1, float y1, float x2, float y2,
    float thickness, Color color, Texture2D tex) {
  float dx = x2 - x1, dy = y2 - y1;
  float length = Mathf.Sqrt(dx * dx + dy * dy);
  if (!(length > 0f)) { return; }
  Matrix4x4 saved = GUI.get_matrix();
  Color old = GUI.get_color();
  GUI.set_color(color);
  GUI.set_matrix(Matrix4x4.TRS(new Vector3(x1, y1, 0f),
      Quaternion.Euler(0f, 0f, Mathf.Atan2(dy, dx) * 57.29578f),
      new Vector3(1f, 1f, 1f)));
  GUI.DrawTexture(new Rect(0f, -thickness * 0.5f, length, thickness), tex);
  GUI.set_matrix(saved);
  GUI.set_color(old);
}

private static int DrawOffscreenArrow(float sw, float sh, float cx, float cy,
    Color color, Texture2D tex) {
  float dx = cx - sw * 0.5f;
  float dy = cy - sh * 0.5f;
  float len = Mathf.Sqrt(dx * dx + dy * dy);
  if (!(len > 1f)) { return 0; }
  float ux = dx / len, uy = dy / len;
  float ax = Mathf.Abs(ux), ay = Mathf.Abs(uy);
  if (ax < 0.001f) { ax = 0.001f; }
  if (ay < 0.001f) { ay = 0.001f; }
  float tx = (sw * 0.5f - 28f) / ax;
  float ty = (sh * 0.5f - 28f) / ay;
  float t = tx < ty ? tx : ty;
  if (t < 0f) { t = 0f; }
  float ex = sw * 0.5f + ux * t;
  float ey = sh * 0.5f + uy * t;
  float px = -uy, py = ux;
  GuiLine(ex, ey, ex - ux * 14f + px * 7f, ey - uy * 14f + py * 7f, 2f, color, tex);
  GuiLine(ex, ey, ex - ux * 14f - px * 7f, ey - uy * 14f - py * 7f, 2f, color, tex);
  return 2;
}

private static void DrawArrowDistance(int slot, GUIStyle style) {
  if (S.R7OffscreenDistance[slot] == null) { return; }
  float sw = (float)S.R7Width, sh = (float)S.R7Height;
  Vector2 point = ArrowAnchor(sw, sh, S.R7Offscreen[slot * 3], S.R7Offscreen[slot * 3 + 1]);
  float dx = point.x - sw * 0.5f, dy = point.y - sh * 0.5f;
  float len = Mathf.Sqrt(dx * dx + dy * dy);
  if (len > 1f) { point.x -= dx / len * 30f; point.y -= dy / len * 30f; }
  GUI.set_color(new Color(1f, 1f, 1f, 1f));
  style.set_fontSize(18); style.set_alignment(1);
  style.get_normal().set_textColor(new Color(0f, 0f, 0f, 1f));
  GUI.Label(new Rect(point.x - 45f, point.y - 11f, 92f, 26f), S.R7OffscreenDistance[slot], style);
  style.get_normal().set_textColor(new Color(1f, 1f, 1f, 1f));
  GUI.Label(new Rect(point.x - 46f, point.y - 12f, 92f, 26f), S.R7OffscreenDistance[slot], style);
}

private static Vector2 ArrowAnchor(float sw, float sh, float x, float y) {
  float dx = x - sw * 0.5f, dy = y - sh * 0.5f;
  float len = Mathf.Sqrt(dx * dx + dy * dy);
  if (!(len > 1f)) { return new Vector2(sw * 0.5f, sh - 28f); }
  float ux = dx / len, uy = dy / len;
  float ax = Mathf.Abs(ux), ay = Mathf.Abs(uy);
  if (ax < 0.001f) { ax = 0.001f; }
  if (ay < 0.001f) { ay = 0.001f; }
  float tx = (sw * 0.5f - 28f) / ax, ty = (sh * 0.5f - 28f) / ay;
  float t = tx < ty ? tx : ty;
  return new Vector2(sw * 0.5f + ux * t, sh * 0.5f + uy * t);
}

private static Vector2 RadarPoint(float right, float forward, float cx, float cy, float radius) {
  float distance = Mathf.Sqrt(right * right + forward * forward);
  float scale = radius / 100f;
  if (distance > 100f) { scale = radius / distance; }
  return new Vector2(cx + right * scale, cy - forward * scale);
}

private static void GuiCircle(float cx, float cy, float radius, float thickness,
    Color color, Texture2D tex) {
  const int seg = 20;
  float px = cx + radius, py = cy;
  for (int s = 1; s <= seg; s++) {
    float a = (float)s * 0.31415927f;
    float nx = cx + Mathf.Sin(a + 1.570796327f) * radius;
    float ny = cy + Mathf.Sin(a) * radius;
    GuiLine(px, py, nx, ny, thickness, color, tex);
    px = nx; py = ny;
  }
}

private static void DrawRadar(Texture2D tex, int count, bool[] team, bool[] knocked,
    bool[] target, int flags, GUIStyle style, float[] records) {
  float sw = (float)S.R7Width, sh = (float)S.R7Height;
  float size = 214f;
  if (size > sw * 0.46f) { size = sw * 0.46f; }
  // top-centre: keeps the movement joystick and fire buttons clear
  float left = (sw - size) * 0.5f;
  float top = 54f;
  if (top + size > sh - 90f) { top = sh - size - 90f; }
  if (top < 12f) { top = 12f; }
  float cx = left + size * 0.5f, cy = top + size * 0.5f;
  float radius = size * 0.5f - 13f;

  // panel + border
  GUI.set_color(new Color(0.02f, 0.03f, 0.06f, 0.68f));
  GUI.DrawTexture(new Rect(left, top, size, size), tex);
  GuiBox(left, top, size, size, 1.4f, new Color(0.35f, 0.6f, 0.95f, 0.85f), tex);

  // 50m / 100m rings + crosshair
  Color ring = new Color(0.45f, 0.65f, 0.95f, 0.32f);
  GuiCircle(cx, cy, radius, 1f, ring, tex);
  GuiCircle(cx, cy, radius * 0.5f, 1f, new Color(0.45f, 0.65f, 0.95f, 0.18f), tex);
  GuiLine(left + 4f, cy, left + size - 4f, cy, 1f, new Color(1f, 1f, 1f, 0.12f), tex);
  GuiLine(cx, top + 4f, cx, top + size - 4f, 1f, new Color(1f, 1f, 1f, 0.12f), tex);

  int inRange = 0, beyond = 0;
  for (int i = 0; i < count && i < 16; i++) {
    if (S.R7Radar[i * 3 + 2] <= 0f) { continue; }
    if ((flags & 2097152) != 0 && S.R7Visibility[i] != 1) { continue; }
    if (team[i]) { continue; }  // enemies only
    float right = S.R7Radar[i * 3], forward = S.R7Radar[i * 3 + 1];
    float dist = Mathf.Sqrt(right * right + forward * forward);
    bool far = dist > 100f;
    if (far) { beyond++; } else { inRange++; }
    Vector2 point = RadarPoint(right, forward, cx, cy, radius);
    float hpRatio = 1f;
    int at = i * 5 + 4;
    if (records != null && at < records.Length) { hpRatio = records[at]; }
    Color c = EspColor(i, flags, knocked[i], team[i], target[i]);
    if (!knocked[i] && !target[i]) {
      if (hpRatio <= 0.35f && hpRatio > 0f) { c = new Color(1f, 0.22f, 0.18f, 1f); }
      else if (hpRatio <= 0.7f && hpRatio > 0f) { c = new Color(1f, 0.62f, 0.12f, 1f); }
    }
    if (S.R7Bots[i] && !target[i]) { c = new Color(0.78f, 0.62f, 1f, 1f); }
    if (far) { c = new Color(c.r, c.g, c.b, 0.45f); }
    GUI.set_color(c);
    float dot = target[i] ? 9f : (far ? 5f : 7f);
    GUI.DrawTexture(new Rect(point.x - dot * 0.5f, point.y - dot * 0.5f, dot, dot), tex);
  }

  // local player arrow (up = aim direction)
  GUI.set_color(new Color(1f, 1f, 1f, 1f));
  GUI.DrawTexture(new Rect(cx - 3f, cy - 3f, 6f, 6f), tex);
  GuiLine(cx - 6f, cy + 1f, cx, cy - 12f, 2f, new Color(1f, 1f, 1f, 1f), tex);
  GuiLine(cx + 6f, cy + 1f, cx, cy - 12f, 2f, new Color(1f, 1f, 1f, 1f), tex);

  style.set_fontSize(14); style.set_alignment(1);
  style.get_normal().set_textColor(new Color(1f, 1f, 1f, 0.95f));
  string label = "ĐỊCH " + inRange.ToString() + " | 100m";
  if (beyond > 0) { label += "  +" + beyond.ToString() + ">"; }
  GUI.Label(new Rect(left, top + size + 2f, size, 20f), label, style);
}

private static void DrawCrosshair(int w, int h, Texture2D tex) {
  int mode = PlayerPrefs.GetInt("OKMOD.Esp.CrosshairStyle", 1);
  float cx = (float)w * 0.5f, cy = (float)h * 0.5f;
  Color color = new Color(0.35f, 1f, 0.7f, 1f);
  if (mode == 0) {
    GUI.set_color(new Color(0f, 0f, 0f, 0.9f));
    GUI.DrawTexture(new Rect(cx - 4f, cy - 4f, 8f, 8f), tex);
    GUI.set_color(color);
    GUI.DrawTexture(new Rect(cx - 2f, cy - 2f, 4f, 4f), tex);
    return;
  }
  GuiLine(cx - 17f, cy, cx - 6f, cy, 4f, new Color(0f, 0f, 0f, 0.9f), tex);
  GuiLine(cx + 6f, cy, cx + 17f, cy, 4f, new Color(0f, 0f, 0f, 0.9f), tex);
  GuiLine(cx, cy + 6f, cx, cy + 17f, 4f, new Color(0f, 0f, 0f, 0.9f), tex);
  GuiLine(cx - 17f, cy, cx - 6f, cy, 2f, color, tex);
  GuiLine(cx + 6f, cy, cx + 17f, cy, 2f, color, tex);
  GuiLine(cx, cy + 6f, cx, cy + 17f, 2f, color, tex);
  if (mode != 2) {
    GuiLine(cx, cy - 17f, cx, cy - 6f, 4f, new Color(0f, 0f, 0f, 0.9f), tex);
    GuiLine(cx, cy - 17f, cx, cy - 6f, 2f, color, tex);
  }
}

private static int GuiSkeleton(int slot, Color color, Texture2D tex) {
  int drawn = 0;
  for (int edge = 0; edge < S.R7BonePairs.Length; edge += 2) {
    int a = slot * 16 + S.R7BonePairs[edge];
    int b = slot * 16 + S.R7BonePairs[edge + 1];
    if (!S.R7SkeletonValid[a] || !S.R7SkeletonValid[b]) { continue; }
    GuiLine(S.R7Skeleton[a * 2], S.R7Skeleton[a * 2 + 1],
        S.R7Skeleton[b * 2], S.R7Skeleton[b * 2 + 1], 2.6f, color, tex);
    drawn++;
  }
  return drawn;
}

private static void GuiBox(float x, float y, float w, float h,
    float thickness, Color color, Texture2D tex) {
  GuiLine(x, y, x + w, y, thickness, color, tex);
  GuiLine(x + w, y, x + w, y + h, thickness, color, tex);
  GuiLine(x + w, y + h, x, y + h, thickness, color, tex);
  GuiLine(x, y + h, x, y, thickness, color, tex);
}

private static void ReleaseBatch() {
  Mesh mesh = S.R8Mesh; Material material = S.R8Material;
  S.R8Mesh = null; S.R8Material = null; S.R8BatchActive = false;
  S.R8MeshDirty = true; S.R8ColorsDirty = true;
  S.R8QuadCount = 0;
  if (mesh != null) { UnityEngine.Object.Destroy(mesh); }
  if (material != null) { UnityEngine.Object.Destroy(material); }
}

  private static void Telemetry(float now) {
    if (now >= S.R7HeapAfter) {
      S.R7HeapAfter = now + 0.5f;
      long bytes = GC.GetTotalMemory(false);
      if (S.R7HeapStart == 0) {
        S.R7HeapStart = bytes;
        S.R7CollectAfter = now + 10f;
      }
      if (bytes > S.R7HeapPeak) { S.R7HeapPeak = bytes; }
      if (S.R7HeapBase == 0 || bytes < S.R7HeapBase) { S.R7HeapBase = bytes; }
      if (S.R7Flags != 0 && bytes - S.R7HeapBase >= 16777216 && now >= S.R7CollectAfter &&
          System.Threading.Thread.CurrentThread.ManagedThreadId == S.R7Thread) {
        // Same growth/cooldown limits as the reference, never once per frame.
        S.R7CollectAfter = now + 10f;
        S.R7HeapBefore = bytes;
        float gcStarted = Time.get_realtimeSinceStartup();
        try {
        SetPhase("GC");
          GC.Collect();
          bytes = GC.GetTotalMemory(false);
          S.R7HeapAfterCollect = bytes;
          S.R7HeapBase = bytes;
          if (S.R7Collections < 1000000000) { S.R7Collections++; }
        } catch (Exception error) {
          if (S.R7GcErrors < 1000000000) { S.R7GcErrors++; }
          RecordError(error, now);
        }
        S.R7GcMs = (Time.get_realtimeSinceStartup() - gcStarted) * 1000f;
      }
      S.R7HeapMiB = (int)(bytes / 1048576);
      long growth = bytes - S.R7HeapBase;
      S.R7HeapDelay = growth >= 67108864 ? 0.10f : growth >= 33554432 ? 0.075f : 0.04f;
      S.R7Stats = "R7 mẫu " + S.R7SampleMs.ToString("F1") + " ms | đọc " + S.R7Visited.ToString() +
          " | vẽ " + S.R7Count.ToString() + "/16 | lệnh " + S.R7Draws.ToString() + "/128";
      S.R7Stats2 = "Heap " + S.R7HeapMiB.ToString() + " MiB | lỗi player " + S.R7Errors.ToString() +
          " | mẫu " + S.R7Samples.ToString();
      S.R7GcStats = "GC " + S.R7Collections.ToString() + " | trước/sau " +
          ((int)(S.R7HeapBefore / 1048576)).ToString() + "/" +
          ((int)(S.R7HeapAfterCollect / 1048576)).ToString() + " MiB | " + S.R7GcMs.ToString("F1") + " ms";
    S.R8Stats = "API vẽ " + S.R8ApiDraws.ToString() + " | mesh upload " + S.R8Uploads.ToString() +
        " | fallback " + S.R8BatchFailures.ToString();
    }
    if (now >= S.R7ReportAfter) {
      S.R7ReportAfter = now + 2f;
      PlayerPrefs.SetInt("OKMOD.R7.Samples", S.R7Samples);
      PlayerPrefs.SetInt("OKMOD.R7.Visited", S.R7Visited);
  PlayerPrefs.SetInt("OKMOD.R7.Cursor", S.R7Cursor);
      PlayerPrefs.SetInt("OKMOD.R7.Drawn", S.R7Count);
      PlayerPrefs.SetInt("OKMOD.R7.Commands", S.R7Draws);
      PlayerPrefs.SetInt("OKMOD.R7.HeapMiB", S.R7HeapMiB);
      PlayerPrefs.SetInt("OKMOD.R7.Errors", S.R7Errors);
      PlayerPrefs.SetFloat("OKMOD.R7.SampleMs", S.R7SampleMs);
      PlayerPrefs.SetInt("OKMOD.R7.HeapStartMiB", (int)(S.R7HeapStart / 1048576));
      PlayerPrefs.SetInt("OKMOD.R7.HeapPeakMiB", (int)(S.R7HeapPeak / 1048576));
      PlayerPrefs.SetInt("OKMOD.R7.HeapBeforeGcMiB", (int)(S.R7HeapBefore / 1048576));
      PlayerPrefs.SetInt("OKMOD.R7.HeapAfterGcMiB", (int)(S.R7HeapAfterCollect / 1048576));
      PlayerPrefs.SetInt("OKMOD.R7.GcCollections", S.R7Collections);
      PlayerPrefs.SetInt("OKMOD.R7.GcErrors", S.R7GcErrors);
      PlayerPrefs.SetFloat("OKMOD.R7.GcMs", S.R7GcMs);
    PlayerPrefs.SetInt("OKMOD.R8.ApiDraws", S.R8ApiDraws);
    PlayerPrefs.SetInt("OKMOD.R8.Uploads", S.R8Uploads);
    PlayerPrefs.SetInt("OKMOD.R8.BatchFailures", S.R8BatchFailures);
    PlayerPrefs.SetInt("OKMOD.R8.BatchActive", S.R8BatchActive ? 1 : 0);
      if (now >= S.R7SaveAfter) { PlayerPrefs.Save(); S.R7SaveAfter = now + 10f; }
    }
  }

  public static void StatusMenu(int px, int py, Vector2 pointer, int eventType, GUIStyle style) {
  GUI.Label(new Rect(px + 20, py + 80, 740f, 48f), "R8-AIM-ESP-v1", style);
    GUI.Label(new Rect(px + 20, py + 152, 740f, 48f), S.R7Stats, style);
    GUI.Label(new Rect(px + 20, py + 224, 740f, 48f), S.R7Stats2, style);
    GUI.Label(new Rect(px + 20, py + 296, 740f, 48f), "Aimbot / Silent chưa nối", style);
    GUI.Label(new Rect(px + 20, py + 368, 740f, 48f), S.R7GcStats, style);
  bool batch = PlayerPrefs.GetString("OKMOD.R8.Batch", "X2") == "X2";
  if (eventType == 1 && new Rect(px + 20, py + 440, 740f, 48f).Contains(pointer)) {
    batch = !batch;
    PlayerPrefs.SetString("OKMOD.R8.Batch", batch ? "X2" : "");
    S.R8BatchOn = batch;
    if (!batch) { ReleaseBatch(); }
  }
  GUI.Label(new Rect(px + 20, py + 440, 740f, 48f), batch ? "Mesh batch: BẬT (chạm để đổi)" : "Mesh batch: TẮT (chạm để đổi)", style);
  GUI.Label(new Rect(px + 20, py + 512, 740f, 48f), S.R8Stats, style);
  }

private static void SetPhase(string value) { S.R8Phase = value; }

  private static bool Valid(Vector3 v) {
    return v.x > -1000000f && v.x < 1000000f && v.y > -1000000f &&
        v.y < 1000000f && v.z > -1000000f && v.z < 1000000f;
  }

  private static void DrawFov(int w, int h, Texture2D tex) {
    float radius = PlayerPrefs.GetFloat("OKMOD.Aim.FOV.Float", (float)PlayerPrefs.GetInt("OKMOD.Aim.FOV", 220));
    if (!(radius >= 20f && radius <= 800f)) { radius = 220f; }
    float cx = w * 0.5f; float cy = h * 0.5f;
    try {
      if (S.R8FovMesh == null || S.R8FovMaterial == null) {
        ReleaseFov();
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        if (shader == null) { throw new Exception("FOV shader unavailable"); }
        S.R8FovMaterial = new Material(shader);
        S.R8FovMaterial.SetInt("_SrcBlend", 5); S.R8FovMaterial.SetInt("_DstBlend", 10);
        S.R8FovMaterial.SetInt("_Cull", 0); S.R8FovMaterial.SetInt("_ZWrite", 0);
        S.R8FovMaterial.SetInt("_ZTest", 8);
        S.R8FovMesh = new Mesh();
        S.R8FovRadius = -1f;
      }
      if (radius != S.R8FovRadius || w != S.R8FovWidth || h != S.R8FovHeight) {
        // A cached annulus uses one draw call, not dozens of GUI rotations.
        Vector3[] vertices = new Vector3[384];
        Color[] colors = new Color[384];
        int[] triangles = new int[576];
        float inner = radius - 2f, outer = radius + 2f;
        Color ringColor = new Color(1f, 0.86f, 0.3f, 0.95f);
        for (int i = 0; i < 96; i++) {
          float a = (float)i * 6.2831853f / 96f;
          float b = (float)(i + 1) * 6.2831853f / 96f;
          float ax = Mathf.Sin(a + 1.570796327f), ay = Mathf.Sin(a);
          float bx = Mathf.Sin(b + 1.570796327f), by = Mathf.Sin(b);
          if (i == 95) { bx = 1f; by = 0f; }
          int v = i * 4, t = i * 6;
          vertices[v] = new Vector3(cx + ax * inner, cy + ay * inner, 0f);
          vertices[v + 1] = new Vector3(cx + ax * outer, cy + ay * outer, 0f);
          vertices[v + 2] = new Vector3(cx + bx * outer, cy + by * outer, 0f);
          vertices[v + 3] = new Vector3(cx + bx * inner, cy + by * inner, 0f);
          colors[v] = ringColor; colors[v + 1] = ringColor;
          colors[v + 2] = ringColor; colors[v + 3] = ringColor;
          triangles[t] = v; triangles[t + 1] = v + 1; triangles[t + 2] = v + 2;
          triangles[t + 3] = v; triangles[t + 4] = v + 2; triangles[t + 5] = v + 3;
        }
        S.R8FovMesh.set_vertices(vertices); S.R8FovMesh.set_colors(colors);
        S.R8FovMesh.set_triangles(triangles);
        S.R8FovRadius = radius; S.R8FovWidth = w; S.R8FovHeight = h;
      }
      if (!S.R8FovMaterial.SetPass(0)) { throw new Exception("FOV material unavailable"); }
      GL.PushMatrix();
      try {
        GL.LoadPixelMatrix(0f, (float)w, (float)h, 0f);
        Graphics.DrawMeshNow(S.R8FovMesh, Matrix4x4.get_identity());
      } finally { GL.PopMatrix(); }
      return;
    } catch (Exception error) {
      ReleaseFov();
      RecordError(error, Time.get_realtimeSinceStartup());
    }
    // Keep a round, thick fallback on devices without the mesh shader.
    float px = cx + radius; float py = cy;
    for (int i = 1; i <= 64; i++) {
      float angle = i * 0.09817477f;
      float nx = cx + Mathf.Sin(angle + 1.570796327f) * radius;
      float ny = cy + Mathf.Sin(angle) * radius;
      if (i == 64) { nx = cx + radius; ny = cy; }
      GuiLine(px, py, nx, ny, 4f, new Color(1f, 0.86f, 0.3f, 0.95f), tex);
      px = nx; py = ny;
    }
  }

  private static void ReleaseFov() {
    if (S.R8FovMesh != null) { UnityEngine.Object.Destroy(S.R8FovMesh); }
    if (S.R8FovMaterial != null) { UnityEngine.Object.Destroy(S.R8FovMaterial); }
    S.R8FovMesh = null; S.R8FovMaterial = null; S.R8FovRadius = -1f;
  }
}
