using HarmonyLib;

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SliceThrough
{
  public class SubModule : MBSubModuleBase
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Mission), "DecideWeaponCollisionReaction")]
    protected static void DecideWeaponCollisionReaction(
      Blow registeredBlow,
      in AttackCollisionData collisionData,
      Agent attacker,
      Agent defender,
      in MissionWeapon attackerWeapon,
      bool isFatalHit,
      bool isShruggedOff,
      out MeleeCollisionReaction colReaction, Mission __instance)
    {
      Harmony.DEBUG = false;
      Debug.Print("");
      FileLog.Log("");

      // original code
      if (collisionData.IsColliderAgent && collisionData.StrikeType == 1 && collisionData.CollisionHitResultFlags.HasAnyFlag<CombatHitResultFlags>(CombatHitResultFlags.HitWithStartOfTheAnimation))
        colReaction = MeleeCollisionReaction.Staggered;
      else if (!collisionData.IsColliderAgent && collisionData.PhysicsMaterialIndex != -1 && PhysicsMaterial.GetFromIndex(collisionData.PhysicsMaterialIndex).GetFlags().HasAnyFlag<PhysicsMaterialFlags>(PhysicsMaterialFlags.AttacksCanPassThrough))
        colReaction = MeleeCollisionReaction.SlicedThrough;
      else if (!collisionData.IsColliderAgent || registeredBlow.InflictedDamage <= 0)
        colReaction = MeleeCollisionReaction.Bounced;
      else if (collisionData.StrikeType == 1 && attacker.IsDoingPassiveAttack)
        colReaction = MissionGameModels.Current.AgentApplyDamageModel.DecidePassiveAttackCollisionReaction(attacker, defender, isFatalHit);
      else
      {
        WeaponClass weaponClass = !attackerWeapon.IsEmpty ? attackerWeapon.CurrentUsageItem.WeaponClass : WeaponClass.Undefined;
        int num1 = attackerWeapon.IsEmpty ? 0 : (!isFatalHit ? 1 : 0);
        int num2 = isShruggedOff ? 1 : 0;
        colReaction = (num1 & num2) != 0 || attackerWeapon.IsEmpty && defender != null && defender.IsHuman && !collisionData.IsAlternativeAttack && (collisionData.VictimHitBodyPart == BoneBodyPartType.Chest || collisionData.VictimHitBodyPart == BoneBodyPartType.ShoulderLeft || (collisionData.VictimHitBodyPart == BoneBodyPartType.ShoulderRight || collisionData.VictimHitBodyPart == BoneBodyPartType.Abdomen) || collisionData.VictimHitBodyPart == BoneBodyPartType.Legs) ? MeleeCollisionReaction.Bounced : ((weaponClass == WeaponClass.OneHandedAxe || weaponClass == WeaponClass.TwoHandedAxe) && (!isFatalHit && (double)collisionData.InflictedDamage < (double)defender.HealthLimit * 0.5) || attackerWeapon.IsEmpty && !collisionData.IsAlternativeAttack && collisionData.AttackDirection == Agent.UsageDirection.AttackUp || collisionData.ThrustTipHit && (sbyte)collisionData.DamageType == (sbyte)1 && (!attackerWeapon.IsEmpty && defender.CanThrustAttackStickToBone(collisionData.CollisionBoneIndex)) ? MeleeCollisionReaction.Stuck : MeleeCollisionReaction.SlicedThrough);
        if (!collisionData.AttackBlockedWithShield && !collisionData.CollidedWithShieldOnBack || colReaction != MeleeCollisionReaction.SlicedThrough)
          return;
        colReaction = MeleeCollisionReaction.Bounced;
      }
      // original code

      if (attacker.IsMine)
      {
        Debug.Print("attacker: " + attacker.Name);
        FileLog.Log("attacker: " + attacker.Name);
        Debug.Print("Collision Reaction: SlicedThrough");
        FileLog.Log("Collision Reaction: SlicedThrough");
        colReaction = MeleeCollisionReaction.SlicedThrough;
      }

      Debug.Print("");
      FileLog.Log("");
      Harmony.DEBUG = false;
    }

    protected override void OnBeforeInitialModuleScreenSetAsRoot()
    {
      base.OnBeforeInitialModuleScreenSetAsRoot();
      var harmony = new Harmony("com.DontInterruptMe.akdombrowski");

      harmony.PatchAll();

      InformationManager.DisplayMessage(new InformationMessage("Loaded 'AttackInteruptModifier'.", Color.FromUint(0182999992U)));
    }
  }
}