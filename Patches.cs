using HarmonyLib;
using UnityEngine;

namespace AnyAngle
{
    // Flow per frame: the ghost is nudged (position + rotation) right before vanilla looks for snap points,
    // so it snaps against nudged neighbours exactly like it would against normal pieces.
    internal static class Nudge
    {
        internal const string OffsetKey = "vbb_off";
        internal const string RotKey = "vbb_rot";

        internal static bool Applied;          // nudge was applied before the snap search
        internal static Vector3 PreSnapPos;    // ghost position after the nudge, before vanilla snapped
        internal static Piece SnapTarget;
        internal static Piece HitPiece;        // piece under the crosshair, its own nudge is already in the hit point

        internal static bool Ready => Plugin.Enabled.Value && PlacementAdjust.Active;

        internal static void CameraAxes(Player player, out Vector3 right, out Vector3 fwd)
        {
            Transform cam = GameCamera.instance != null ? GameCamera.instance.transform : player.transform;
            fwd = Vector3.ProjectOnPlane(cam.forward, Vector3.up);
            if (fwd.sqrMagnitude < 0.0001f) fwd = player.transform.forward;
            fwd.Normalize();
            right = Vector3.Cross(Vector3.up, fwd);
        }

        internal static Vector3 HitPoint;      // where the crosshair ray hit

        // Turn the ghost around the point you aim at (where vanilla put the selected snap point),
        // not around its own origin, so the snap points stay under the crosshair
        internal static void Rotate(Transform ghost)
        {
            Quaternion oldRot = ghost.rotation;
            Quaternion newRot = oldRot * Quaternion.Euler(PlacementAdjust.Euler);
            Vector3 local = Quaternion.Inverse(oldRot) * (HitPoint - ghost.position);
            ghost.rotation = newRot;
            ghost.position = HitPoint - newRot * local;
        }

        internal static Vector3 OffsetOf(Piece piece)
        {
            ZDO zdo = ZdoOf(piece);
            return zdo != null ? zdo.GetVec3(OffsetKey, Vector3.zero) : Vector3.zero;
        }

        internal static ZDO ZdoOf(Piece piece)
        {
            ZNetView nview = piece != null ? piece.GetComponent<ZNetView>() : null;
            return nview != null && nview.IsValid() ? nview.GetZDO() : null;
        }
    }

    [HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
    internal static class GhostPatch
    {
        static readonly AccessTools.FieldRef<Player, GameObject> Ghost =
            AccessTools.FieldRefAccess<Player, GameObject>("m_placementGhost");
        static readonly AccessTools.FieldRef<Player, Player.PlacementStatus> Status =
            AccessTools.FieldRefAccess<Player, Player.PlacementStatus>("m_placementStatus");
        static readonly System.Reflection.MethodInfo SetValid =
            AccessTools.Method(typeof(Player), "SetPlacementGhostValid");

        static void Prefix()
        {
            Nudge.Applied = false;
            Nudge.SnapTarget = null;
            Nudge.HitPiece = null;
        }

        static void Postfix(Player __instance)
        {
            GameObject ghost = Ghost(__instance);
            PlacementAdjust.Building = ghost != null && __instance == Player.m_localPlayer;
            if (!PlacementAdjust.Building || !Nudge.Ready || !ghost.activeSelf) return;

            Vector3 world = PlacementAdjust.Offset;
            if (!Nudge.Applied)
            {
                // snapping was off (Alt) or never ran: nudge the finished ghost
                Nudge.Rotate(ghost.transform);
                ghost.transform.position += world -Nudge.OffsetOf(Nudge.HitPiece);
            }
            else if ((ghost.transform.position - Nudge.PreSnapPos).sqrMagnitude > 1e-6f)
            {
                // vanilla pulled the ghost onto a snap point; a neighbour that carries a different nudge
                // is compensated so the offset stays relative to the normal grid
                ghost.transform.position += world - Nudge.OffsetOf(Nudge.SnapTarget);
            }

            Player.PlacementStatus status = Status(__instance);
            Vector3 pos = ghost.transform.position;
            Piece piece = ghost.GetComponent<Piece>();
            PrivateArea ward = piece != null ? piece.GetComponent<PrivateArea>() : null;

            // the nudge may have moved the piece into a protected area
            if (Location.IsInsideNoBuildLocation(pos))
                status = Player.PlacementStatus.NoBuildZone;
            else if (!PrivateArea.CheckAccess(pos, ward ? ward.m_radius : 0f, false, ward != null))
                status = Player.PlacementStatus.PrivateZone;
            else if (Plugin.IgnoreInvalid.Value && status == Player.PlacementStatus.Invalid)
                status = Player.PlacementStatus.Valid;

            Status(__instance) = status;
            SetValid.Invoke(__instance, new object[] { status == Player.PlacementStatus.Valid });
        }
    }

    // vanilla puts the ghost against the surface it hits; a nudged piece's surface already carries its offset
    [HarmonyPatch(typeof(Player), "PieceRayTest")]
    internal static class RayPatch
    {
        static void Postfix(bool __result, Piece piece, Vector3 point)
        {
            Nudge.HitPiece = __result ? piece : null;
            Nudge.HitPoint = point;
        }
    }

    [HarmonyPatch(typeof(Player), "FindClosestSnapPoints")]
    internal static class SnapPatch
    {
        static void Prefix(Transform ghost)
        {
            if (!Nudge.Ready || Player.m_localPlayer == null) return;
            Nudge.Rotate(ghost);
            ghost.position += PlacementAdjust.Offset - Nudge.OffsetOf(Nudge.HitPiece);
            Nudge.PreSnapPos = ghost.position;
            Nudge.Applied = true;
        }

        static void Postfix(bool __result, Transform b)
        {
            if (__result && b != null) Nudge.SnapTarget = b.GetComponentInParent<Piece>();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
    internal static class PlacePatch
    {
        internal static bool Placing;

        static void Prefix() => Placing = Nudge.Ready;
        static void Postfix()
        {
            Placing = false;
            if (Plugin.ResetAfterPlace.Value) PlacementAdjust.Reset();
        }
    }

    // remember the nudge on the new piece so later pieces can compensate for it
    [HarmonyPatch(typeof(Piece), nameof(Piece.SetCreator))]
    internal static class StoreNudgePatch
    {
        static void Postfix(Piece __instance)
        {
            if (!PlacePatch.Placing) return;
            ZDO zdo = Nudge.ZdoOf(__instance);
            if (zdo == null) return;
            zdo.Set(Nudge.OffsetKey, PlacementAdjust.Offset);
            zdo.Set(Nudge.RotKey, PlacementAdjust.Euler);
        }
    }
}
