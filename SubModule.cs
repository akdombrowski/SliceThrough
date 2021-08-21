using HarmonyLib;

using System;
using System.Collections.Generic;

using TaleWorlds.Core;
using TaleWorlds.Engine;
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

            if (attacker.IsHero || attacker.IsMainAgent || attacker.IsPlayerControlled)
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
                momentumRemaining = 1000;
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