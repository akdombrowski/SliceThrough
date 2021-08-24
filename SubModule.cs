using HarmonyLib;

using MCM.Abstractions.Settings.Base.Global;

using System.Collections.Generic;

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SliceThrough
{
    [HarmonyPatch]
    public class SubModule : MBSubModuleBase
    {
        private static bool DEBUG = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mission), "UpdateMomentumRemaining")]
        private static void UpdateMomentumRemaining(
            ref float momentumRemaining,
            Blow b,
            in AttackCollisionData collisionData,
            Agent attacker,
            Agent victim,
            in MissionWeapon attackerWeapon,
            ref bool isCrushThrough)
        {
            int REMAINING_MOMENTUM = GlobalSettings<SliceThroughSettings>.Instance.RemainingMomentum;

            if ((attacker.IsHero || attacker.IsMainAgent || attacker.IsPlayerControlled) &&
                REMAINING_MOMENTUM != 0)
            {
                Harmony.DEBUG = DEBUG;
                List<string> buffer = FileLog.GetBuffer(true);

                buffer.Add("");
                buffer.Add("");
                buffer.Add("UpdateMomentumRemaining");
                buffer.Add("attacker.IsHero: " + attacker.IsHero);
                buffer.Add("attacker.Name: " + attacker.Name);
                buffer.Add("isCrushThrough before: " + isCrushThrough);
                buffer.Add("momentumRemaining before: " + momentumRemaining);
                isCrushThrough = true;
                momentumRemaining = REMAINING_MOMENTUM;
                buffer.Add("isCrushThrough after: " + isCrushThrough);
                buffer.Add("momentumRemaining after: " + momentumRemaining);

                FileLog.LogBuffered(buffer);
                FileLog.FlushBuffer();

                Harmony.DEBUG = false;
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            var harmony = new Harmony("com.SliceThrough.akdombrowski");

            harmony.PatchAll();

            InformationManager.DisplayMessage(new InformationMessage("Loaded 'SliceThrough'.", Color.FromUint(0182999992U)));
        }
    }
}