using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

using HarmonyLib;

namespace PartialParry.Patches
{

    [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "ComputeBlowDamage")]
    public static class MissionComputeBlowDamagePatch
    {
        // here we reduce the magnitude which his the damage before armor so that parry will not always parry everything
        public static bool Prefix(AttackInformation attackInformation, AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, ref float magnitude)
        {
            if (!attackCollisionData.AttackBlockedWithShield && (attackCollisionData.CollisionResult == CombatCollisionResult.Parried || attackCollisionData.CollisionResult == CombatCollisionResult.Blocked))
            {
                float baseParryMagnitude = Settings.Current.baseMagnitudeParried;
                if (attackCollisionData.CollisionResult == CombatCollisionResult.Parried)
                {
                    baseParryMagnitude *= Settings.Current.perfectParryBonus / 100f;
                }
                if (attackerWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
                {
                    baseParryMagnitude *= Settings.Current.shieldBreakWeaponDefendMalus / 100f;
                }
                WeaponClass weaponClass = attackInformation.VictimMainHandWeapon.CurrentUsageItem.WeaponClass;
                if (weaponClass == WeaponClass.TwoHandedAxe || weaponClass == WeaponClass.TwoHandedSword || weaponClass == WeaponClass.TwoHandedMace || weaponClass == WeaponClass.TwoHandedPolearm)
                {
                    baseParryMagnitude *= Settings.Current.twoHandedWeaponParryBonus / 100f;
                }
                if (weaponClass == WeaponClass.Dagger)
                {
                    baseParryMagnitude *= Settings.Current.littleWeaponParryMalus / 100f;
                }
                float oldMagnitude = magnitude;
                magnitude -= baseParryMagnitude;
                magnitude = MathF.Max(0f, magnitude);
                if (Settings.Current.showLog && (attackInformation.IsAttackerPlayer || attackInformation.IsVictimPlayer))
                {
                    InformationManager.DisplayMessage(new InformationMessage(string.Format("magnitude before parry:{0} after:{1} ", oldMagnitude, magnitude)));
                }
            }
            return true;
        }
    }
}