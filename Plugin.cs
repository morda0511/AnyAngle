using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace AnyAngle
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string ModGuid = "com.morda.anyangle";
        public const string ModName = "AnyAngle";
        public const string ModVersion = "0.1.0";

        internal static BepInEx.Logging.ManualLogSource Log;

        internal static ConfigEntry<bool> Enabled, IgnoreInvalid, ResetAfterPlace, ShowHud;
        internal static ConfigEntry<float> MaxOffset;
        internal static ConfigEntry<KeyCode> Forward, Back, Left, Right, Up, Down;
        internal static ConfigEntry<KeyCode> PitchUp, PitchDown, RollLeft, RollRight, YawLeft, YawRight;
        internal static ConfigEntry<KeyCode> CycleStep, ResetKey;
        internal static ConfigEntry<KeyboardShortcut> CopyKey;

        private void Awake()
        {
            Log = Logger;
            Enabled = Config.Bind("General", "Enabled", true, "Allow nudging and rotating the build ghost before placing.");
            IgnoreInvalid = Config.Bind("General", "IgnoreInvalid", true,
                "While a nudge/rotation is active, a ghost that Valheim marks invalid (clipping, slope, ...) can still be placed. " +
                "Ward zones and no-build zones are always respected.");
            ResetAfterPlace = Config.Bind("General", "ResetAfterPlace", false,
                "Reset offset and rotation after each placed piece. Off = the same offset/rotation stays for the next pieces, " +
                "which still snap to the (nudged) pieces already built.");
            MaxOffset = Config.Bind("General", "MaxOffset", 3f, "Maximum distance in meters the ghost can be moved away from its normal spot.");
            ShowHud = Config.Bind("General", "ShowHud", true, "Show offset/rotation/step on screen while building.");

            const string k = "Keys (hold to repeat)";
            Forward = Config.Bind(k, "Forward", KeyCode.UpArrow, "Move the piece away from you.");
            Back = Config.Bind(k, "Back", KeyCode.DownArrow, "Move the piece towards you.");
            Left = Config.Bind(k, "Left", KeyCode.LeftArrow, "Move the piece left.");
            Right = Config.Bind(k, "Right", KeyCode.RightArrow, "Move the piece right.");
            Up = Config.Bind(k, "Up", KeyCode.PageUp, "Move the piece up.");
            Down = Config.Bind(k, "Down", KeyCode.PageDown, "Move the piece down.");
            PitchUp = Config.Bind(k, "PitchUp", KeyCode.Keypad2, "Tilt the piece forward.");
            PitchDown = Config.Bind(k, "PitchDown", KeyCode.Keypad8, "Tilt the piece back.");
            RollLeft = Config.Bind(k, "RollLeft", KeyCode.Keypad4, "Roll the piece to the left.");
            RollRight = Config.Bind(k, "RollRight", KeyCode.Keypad6, "Roll the piece to the right.");
            YawLeft = Config.Bind(k, "YawLeft", KeyCode.Keypad7, "Extra turn left (on top of the vanilla mouse wheel).");
            YawRight = Config.Bind(k, "YawRight", KeyCode.Keypad9, "Extra turn right.");
            CycleStep = Config.Bind(k, "CycleStep", KeyCode.KeypadPlus, "Switch between the fine / medium / coarse / large step sizes.");
            ResetKey = Config.Bind(k, "Reset", KeyCode.Keypad5, "Reset offset and rotation.");

            CopyKey = Config.Bind(k, "CopyNudge", new KeyboardShortcut(KeyCode.C, KeyCode.LeftAlt),
                "Look at a piece you built with this mod and press this to take over its offset and rotation.");

            gameObject.AddComponent<PlacementAdjust>();
            new Harmony(ModGuid).PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo(ModName + " v" + ModVersion + " loaded.");
        }
    }
}
