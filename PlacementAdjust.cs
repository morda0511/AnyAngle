using System.Collections.Generic;
using UnityEngine;

namespace AnyAngle
{
    // Holds the current offset/rotation, reads the keys and draws the key box. Patches.cs applies it to the ghost.
    internal class PlacementAdjust : MonoBehaviour
    {
        // world-space offset in meters; a key press converts the camera direction into a world step,
        // so the offset stays fixed while you walk around the piece
        internal static Vector3 Offset;
        // pitch / yaw / roll in degrees, applied on top of the vanilla rotation
        internal static Vector3 Euler;
        internal static bool Building;

        static readonly float[] PosSteps = { 0.01f, 0.05f, 0.25f, 1f };
        static readonly float[] RotSteps = { 1f, 5f, 15f, 45f };
        static readonly string[] StepNames = { "fine", "medium", "coarse", "large" };
        static int step = 1;

        const float RepeatDelay = 0.4f, RepeatRate = 0.05f;
        float nextRepeat;
        KeyCode held;

        internal static bool Active => Offset != Vector3.zero || Euler != Vector3.zero;

        internal enum Axis { None, Pitch, Yaw, Roll, Depth, Side, Height, Step }
        static Axis shown;
        static float shownUntil;

        internal static Axis Shown => Time.unscaledTime <= shownUntil ? shown : Axis.None;
        internal static string StepName => StepNames[step];

        void LateUpdate() => AxisGizmo.Tick(Plugin.Enabled.Value && Building ? Shown : Axis.None);

        static void CycleStep(int dir) => step = (step + dir + PosSteps.Length) % PosSteps.Length;

        // which axis the mouse wheel controls right now, from the keys you hold
        static Axis HeldAxis()
        {
            if (Input.GetKey(Plugin.WheelDepth.Value)) return Axis.Depth;
            if (Input.GetKey(Plugin.WheelSide.Value)) return Axis.Side;
            if (Input.GetKey(Plugin.WheelHeight.Value)) return Axis.Height;
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            if (shift && ctrl) return Axis.Step;
            if (shift) return Axis.Pitch;
            if (ctrl) return Axis.Roll;
            return Axis.None;
        }

        internal static bool MoveKeyHeld => Building && Plugin.Enabled.Value &&
            (Input.GetKey(Plugin.WheelDepth.Value) || Input.GetKey(Plugin.WheelSide.Value) || Input.GetKey(Plugin.WheelHeight.Value));

        // vanilla must not also turn the piece or zoom the camera while the wheel belongs to us
        internal static bool WheelTaken => Building && Plugin.Enabled.Value && InputFree() && HeldAxis() != Axis.None;

        internal static void Reset()
        {
            Offset = Vector3.zero;
            Euler = Vector3.zero;
            // also the vanilla turn from the plain mouse wheel
            Player player = Player.m_localPlayer;
            if (player != null) HarmonyLib.AccessTools.FieldRefAccess<Player, int>("m_placeRotation")(player) = 0;
            Plugin.Log.LogInfo("Reset offset and rotation");
        }

        void Update()
        {
            if (!Plugin.Enabled.Value || !Building || !InputFree()) { held = KeyCode.None; return; }

            if (Input.GetKeyDown(Plugin.CycleStep.Value)) CycleStep(1);
            if (Input.GetKeyDown(Plugin.StepDown.Value)) CycleStep(-1);
            if (Input.GetKeyDown(Plugin.ResetKey.Value)) Reset();
            if (Plugin.CopyKey.Value.IsDown()) CopyFromTarget();

            float p = PosSteps[step], r = RotSteps[step];
            Try(Plugin.Forward.Value, () => Move(0, 0, p));
            Try(Plugin.Back.Value, () => Move(0, 0, -p));
            Try(Plugin.Right.Value, () => Move(p, 0, 0));
            Try(Plugin.Left.Value, () => Move(-p, 0, 0));
            Try(Plugin.Up.Value, () => Move(0, p, 0));
            Try(Plugin.Down.Value, () => Move(0, -p, 0));

            // mouse wheel: the held modifier picks the axis
            Axis mode = HeldAxis();
            float now = Time.unscaledTime;
            if (mode != Axis.None) { shown = mode; shownUntil = now + 0.8f; }
            int notch = Mathf.RoundToInt(Mathf.Sign(Input.mouseScrollDelta.y) * Mathf.Min(1f, Mathf.Abs(Input.mouseScrollDelta.y)));
            if (notch != 0)
            {
                switch (mode)
                {
                    case Axis.None: shown = Axis.Yaw; shownUntil = now + 1.2f; break;   // vanilla turns the piece
                    case Axis.Pitch: Euler.x += notch * r; break;
                    case Axis.Roll: Euler.z -= notch * r; break;
                    case Axis.Step: CycleStep(notch); break;
                    case Axis.Depth: Move(0, 0, notch * p); break;
                    case Axis.Side: Move(notch * p, 0, 0); break;
                    case Axis.Height: Move(0, notch * p, 0); break;
                }
            }

            float max = Plugin.MaxOffset.Value;
            if (Offset.magnitude > max) Offset = Offset.normalized * max;
            Offset = new Vector3(Round(Offset.x), Round(Offset.y), Round(Offset.z));
        }

        static void Move(float right, float up, float fwd)
        {
            Player player = Player.m_localPlayer;
            if (player == null) return;
            // along the piece's own axes, so "forward" is always the same direction of the piece
            GameObject ghost = AccessTools_Ghost(player);
            if (ghost == null) return;
            Nudge.MoveAxes(ghost.transform, out Vector3 r, out Vector3 u, out Vector3 f);
            Offset += r * right + u * up + f * fwd;
        }

        static void CopyFromTarget()
        {
            Player player = Player.m_localPlayer;
            if (player == null || GameCamera.instance == null) return;
            Transform cam = GameCamera.instance.transform;
            if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 50f, LayerMask.GetMask("piece", "piece_nonsolid"))) return;
            ZDO zdo = Nudge.ZdoOf(hit.collider.GetComponentInParent<Piece>());
            if (zdo == null) return;

            Offset = zdo.GetVec3(Nudge.OffsetKey, Vector3.zero);
            Euler = zdo.GetVec3(Nudge.RotKey, Vector3.zero);
            player.Message(MessageHud.MessageType.TopLeft, "Offset and rotation copied");
        }

        // fire once on press, then repeat while held
        void Try(KeyCode key, System.Action act)
        {
            if (key == KeyCode.None) return;
            if (Input.GetKeyDown(key))
            {
                held = key;
                nextRepeat = Time.unscaledTime + RepeatDelay;
                act();
            }
            else if (held == key && Input.GetKey(key) && Time.unscaledTime >= nextRepeat)
            {
                nextRepeat = Time.unscaledTime + RepeatRate;
                act();
            }
        }

        static float Round(float v) => Mathf.Round(v * 1000f) / 1000f;

        static bool InputFree()
        {
            if (Chat.instance != null && Chat.instance.HasFocus()) return false;
            return !(Console.IsVisible() || TextInput.IsVisible() || Menu.IsVisible() || InventoryGui.IsVisible() || Hud.IsPieceSelectionVisible());
        }

        // ------------------------------------------------------------------ key box (bottom right)

        static Texture2D panelTex, chipTex;
        static GUIStyle titleStyle, labelStyle, chipStyle, footStyle;

        static Texture2D Solid(Color c)
        {
            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        static void InitStyles()
        {
            if (panelTex != null) return;
            panelTex = Solid(new Color(0.08f, 0.07f, 0.06f, 0.78f));
            chipTex = Solid(new Color(0.93f, 0.89f, 0.80f, 0.95f));
            titleStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, richText = true };
            labelStyle = new GUIStyle(GUI.skin.label) { richText = true };
            footStyle = new GUIStyle(GUI.skin.label) { richText = true };
            chipStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                wordWrap = false
            };
            chipStyle.normal.background = chipTex;
            chipStyle.normal.textColor = new Color(0.12f, 0.1f, 0.08f);
        }

        static string Name(KeyCode k)
        {
            switch (k)
            {
                case KeyCode.UpArrow: return "Up";
                case KeyCode.DownArrow: return "Down";
                case KeyCode.LeftArrow: return "Left";
                case KeyCode.RightArrow: return "Right";
                case KeyCode.PageUp: return "PgUp";
                case KeyCode.PageDown: return "PgDn";
                case KeyCode.KeypadPlus: return "Num +";
                case KeyCode.KeypadMinus: return "Num -";
                case KeyCode.None: return "-";
            }
            string s = k.ToString();
            return s.StartsWith("Keypad") ? "Num " + s.Substring(6) : s;
        }

        static string Pair(KeyCode a, KeyCode b) => Name(a) + " / " + Name(b);

        void OnGUI()
        {
            if (!Plugin.ShowHud.Value || !Plugin.Enabled.Value || !Building) return;
            InitStyles();

            float s = Mathf.Clamp(Screen.height / 1080f, 0.7f, 2f);
            int font = Mathf.RoundToInt(15 * s);
            titleStyle.fontSize = labelStyle.fontSize = chipStyle.fontSize = footStyle.fontSize = font;
            titleStyle.normal.textColor = new Color(1f, 0.82f, 0.35f);
            labelStyle.normal.textColor = Color.white;
            footStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            var rows = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Wheel", "Turn (Y)"),
                new KeyValuePair<string, string>("Shift + Wheel", "Tilt (X)"),
                new KeyValuePair<string, string>("Ctrl + Wheel", "Roll (Z)"),
                new KeyValuePair<string, string>(Name(Plugin.WheelDepth.Value) + " + Wheel", "Move forward / back"),
                new KeyValuePair<string, string>(Name(Plugin.WheelSide.Value) + " + Wheel", "Move left / right"),
                new KeyValuePair<string, string>(Name(Plugin.WheelHeight.Value) + " + Wheel", "Move up / down"),
                new KeyValuePair<string, string>("Shift + Ctrl + Wheel", "Step size: " + StepNames[step]),
                new KeyValuePair<string, string>(Pair(Plugin.CycleStep.Value, Plugin.StepDown.Value), "Step size up / down"),
                new KeyValuePair<string, string>("Arrows, PgUp, PgDn", "Move with keys"),
                new KeyValuePair<string, string>(Name(Plugin.ResetKey.Value), "Reset"),
                new KeyValuePair<string, string>(Plugin.CopyKey.Value.ToString().Replace(" + ", "+"), "Copy from piece"),
            };

            float rowH = 28 * s, pad = 12 * s, chipW = 190 * s, width = 440 * s;
            float height = pad * 2 + rowH * (rows.Count + 2);
            var box = new Rect(Screen.width - width - 24 * s, Screen.height - height - 110 * s, width, height);
            GUI.DrawTexture(box, panelTex);

            Piece piece = SelectedPiece();
            string title = piece != null ? Localization.instance.Localize(piece.m_name) : "Build";
            float y = box.y + pad;
            GUI.Label(new Rect(box.x + pad, y, width - pad * 2, rowH), title, titleStyle);
            y += rowH;

            foreach (var row in rows)
            {
                GUI.Label(new Rect(box.x + pad, y, chipW, rowH - 4 * s), row.Key, chipStyle);
                GUI.Label(new Rect(box.x + pad * 2 + chipW, y - 2 * s, width - chipW - pad * 3, rowH), row.Value, labelStyle);
                y += rowH;
            }

            string foot = string.Format("Offset up {0:0.00}  side {1:0.00} m   Rot {2:0.#} / {3:0.#} / {4:0.#}",
                Offset.y, new Vector2(Offset.x, Offset.z).magnitude, Euler.x, Euler.y, Euler.z);
            GUI.Label(new Rect(box.x + pad, y + 2 * s, width - pad * 2, rowH), foot, footStyle);

            AxisGizmo.DrawLabel(s, titleStyle);
        }


        static Piece SelectedPiece()
        {
            Player p = Player.m_localPlayer;
            if (p == null) return null;
            GameObject ghost = AccessTools_Ghost(p);
            return ghost != null ? ghost.GetComponent<Piece>() : null;
        }

        internal static GameObject AccessTools_Ghost(Player p) => HarmonyLib.AccessTools.FieldRefAccess<Player, GameObject>("m_placementGhost")(p);
    }
}
