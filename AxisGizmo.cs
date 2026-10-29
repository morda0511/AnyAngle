using UnityEngine;

namespace AnyAngle
{
    // Three axis arrows with cones and three rings at the snap point of the ghost, like the Unity editor handles.
    // The axis the mouse wheel controls right now is drawn bright, the others dimmed.
    internal static class AxisGizmo
    {
        static GameObject root;
        static readonly LineRenderer[] shafts = new LineRenderer[3];
        static readonly LineRenderer[] rings = new LineRenderer[3];
        static readonly Transform[] cones = new Transform[3];
        static readonly Material[] coneMats = new Material[3];
        static Mesh coneMesh;
        static Material lineMat;

        const int RingPoints = 64;
        static readonly Color[] AxisColors =
        {
            new Color(1f, 0.25f, 0.2f), new Color(0.4f, 1f, 0.3f), new Color(0.3f, 0.55f, 1f)
        };
        static readonly Color Ring = new Color(1f, 0.9f, 0.1f);

        static Vector3 anchor;
        static string label;
        static Color labelColor;
        static bool visible;

        static Material NewMaterial()
        {
            Shader shader = Shader.Find("UI/Default") ?? Shader.Find("Sprites/Default") ?? Shader.Find("Hidden/Internal-Colored");
            var m = new Material(shader) { renderQueue = 4000 };
            m.SetInt("unity_GUIZTestMode", 8);   // always on top, so the piece never hides it
            return m;
        }

        static Mesh MakeCone()
        {
            const int n = 20;
            var verts = new Vector3[n + 2];
            var tris = new int[n * 6];
            for (int i = 0; i < n; i++)
            {
                float a = i * Mathf.PI * 2f / n;
                verts[i] = new Vector3(Mathf.Cos(a) * 0.5f, Mathf.Sin(a) * 0.5f, 0f);
            }
            verts[n] = new Vector3(0, 0, 1f);   // tip
            verts[n + 1] = Vector3.zero;        // base centre
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                tris[i * 6] = i; tris[i * 6 + 1] = n; tris[i * 6 + 2] = j;
                tris[i * 6 + 3] = j; tris[i * 6 + 4] = n + 1; tris[i * 6 + 5] = i;
            }
            var mesh = new Mesh { vertices = verts, triangles = tris };
            mesh.RecalculateBounds();
            return mesh;
        }

        static LineRenderer NewLine(string name, int points, bool loop)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root.transform, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.sharedMaterial = lineMat;
            lr.useWorldSpace = true;
            lr.loop = loop;
            lr.positionCount = points;
            lr.numCapVertices = 4;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            return lr;
        }

        static void Build()
        {
            root = new GameObject("AnyAngle gizmo");
            Object.DontDestroyOnLoad(root);
            lineMat = NewMaterial();
            coneMesh = MakeCone();
            for (int i = 0; i < 3; i++)
            {
                shafts[i] = NewLine("shaft" + i, 2, false);
                rings[i] = NewLine("ring" + i, RingPoints, true);
                var go = new GameObject("cone" + i);
                go.transform.SetParent(root.transform, false);
                go.AddComponent<MeshFilter>().sharedMesh = coneMesh;
                var mr = go.AddComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                coneMats[i] = NewMaterial();
                mr.sharedMaterial = coneMats[i];
                cones[i] = go.transform;
            }
        }

        static void SetLine(LineRenderer lr, Color c, float width)
        {
            lr.startColor = lr.endColor = c;
            lr.startWidth = lr.endWidth = width;
        }

        // the three axes (colour order red, green, blue) for the mode, and which one is active (-1 = none)
        static void Basis(PlacementAdjust.Axis mode, Transform ghost, Player player, out Vector3[] axes, out int active)
        {
            axes = new[] { ghost.right, ghost.up, ghost.forward };
            if (mode == PlacementAdjust.Axis.Side || mode == PlacementAdjust.Axis.Height || mode == PlacementAdjust.Axis.Depth)
            {
                Nudge.MoveAxes(ghost, out Vector3 r, out Vector3 u, out Vector3 f);
                axes = new[] { r, u, f };
                active = mode == PlacementAdjust.Axis.Side ? 0 : mode == PlacementAdjust.Axis.Height ? 1 : 2;
                return;
            }
            active = mode == PlacementAdjust.Axis.Pitch ? 0 : mode == PlacementAdjust.Axis.Yaw ? 1 : mode == PlacementAdjust.Axis.Roll ? 2 : -1;
        }

        internal static void Tick(PlacementAdjust.Axis mode)
        {
            Player player = Player.m_localPlayer;
            GameObject ghost = player != null ? PlacementAdjust.AccessTools_Ghost(player) : null;
            Camera cam = Camera.main;
            visible = mode != PlacementAdjust.Axis.None && ghost != null && ghost.activeSelf && cam != null;
            if (!visible) { if (root != null && root.activeSelf) root.SetActive(false); return; }
            if (root == null) Build();
            root.SetActive(true);

            // anchor: the ghost's snap point at the aim point (where the yellow marker is)
            Vector3 center = ghost.transform.position;
            Vector3 aim = Nudge.HitPoint;
            if ((aim - center).sqrMagnitude < 36f)
            {
                float best = float.MaxValue;
                foreach (Transform child in ghost.GetComponentsInChildren<Transform>())
                {
                    if (!child.CompareTag("snappoint")) continue;
                    float d = (child.position - aim).sqrMagnitude;
                    if (d < best) { best = d; center = child.position; }
                }
                if (best == float.MaxValue) center = aim;
            }
            anchor = center;

            float dist = Vector3.Distance(cam.transform.position, center);
            float len = Mathf.Clamp(dist * 0.14f, 0.5f, 1.6f);
            float thin = len * 0.025f, thick = len * 0.05f;

            Basis(mode, ghost.transform, player, out Vector3[] axes, out int active);
            bool turning = mode == PlacementAdjust.Axis.Pitch || mode == PlacementAdjust.Axis.Yaw || mode == PlacementAdjust.Axis.Roll;
            for (int i = 0; i < 3; i++)
            {
                // only what you use: the ring of the axis you turn, or the arrow of the axis you move on
                bool on = i == active;
                rings[i].enabled = on && turning;
                shafts[i].enabled = on && !turning;
                cones[i].gameObject.SetActive(on && !turning);
                Color c = AxisColors[i];
                if (!on) c.a = active < 0 ? 0.9f : 0.3f;

                Vector3 dir = axes[i];
                float coneLen = len * 0.28f, coneW = len * (on ? 0.16f : 0.12f);
                Vector3 tip = center + dir * len;

                shafts[i].SetPosition(0, center);
                shafts[i].SetPosition(1, tip - dir * coneLen * 0.5f);
                SetLine(shafts[i], c, on ? thick : thin);

                cones[i].position = tip - dir * coneLen;
                cones[i].rotation = Quaternion.LookRotation(dir);
                cones[i].localScale = new Vector3(coneW, coneW, coneLen);
                coneMats[i].color = c;

                // ring around axis i: lies in the plane of the other two axes
                Vector3 u = axes[(i + 1) % 3], v = axes[(i + 2) % 3];
                float r = len * 0.8f;
                for (int k = 0; k < RingPoints; k++)
                {
                    float a = k * Mathf.PI * 2f / RingPoints;
                    rings[i].SetPosition(k, center + (u * Mathf.Cos(a) + v * Mathf.Sin(a)) * r);
                }
                Color rc = Ring;
                if (!on) rc.a = active < 0 ? 0.55f : 0.25f;
                SetLine(rings[i], rc, on ? thick : thin);
            }

            switch (mode)
            {
                case PlacementAdjust.Axis.Pitch: label = "Tilt (X)"; labelColor = AxisColors[0]; break;
                case PlacementAdjust.Axis.Yaw: label = "Turn (Y)"; labelColor = AxisColors[1]; break;
                case PlacementAdjust.Axis.Roll: label = "Roll (Z)"; labelColor = AxisColors[2]; break;
                case PlacementAdjust.Axis.Depth: label = "Forward / back"; labelColor = AxisColors[2]; break;
                case PlacementAdjust.Axis.Side: label = "Left / right"; labelColor = AxisColors[0]; break;
                case PlacementAdjust.Axis.Height: label = "Up / down"; labelColor = AxisColors[1]; break;
                default: label = "Step: " + PlacementAdjust.StepName; labelColor = Color.white; break;
            }
        }

        static GUIStyle labelStyle;

        internal static void DrawLabel(float s, GUIStyle unused)
        {
            if (!visible || Event.current.type != EventType.Repaint) return;
            Camera cam = Camera.main;
            if (cam == null) return;
            Vector3 v = cam.WorldToScreenPoint(anchor + Vector3.up * 0.1f);
            if (v.z < 0.05f) return;
            float x = v.x / cam.pixelWidth * Screen.width;
            float y = Screen.height - v.y / cam.pixelHeight * Screen.height - 70 * s;

            if (labelStyle == null) labelStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            labelStyle.fontSize = Mathf.RoundToInt(18 * s);
            var rect = new Rect(x - 140 * s, y - 14 * s, 280 * s, 28 * s);
            Color old = GUI.color;
            labelStyle.normal.textColor = Color.black;
            GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), label, labelStyle);
            labelStyle.normal.textColor = labelColor;
            GUI.Label(rect, label, labelStyle);
            GUI.color = old;
        }
    }
}
