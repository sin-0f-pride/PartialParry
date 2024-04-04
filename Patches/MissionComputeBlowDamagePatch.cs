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
                float parryMagnitude = attackCollisionData.CollisionResult != CombatCollisionResult.Parried ? Settings.Current.ParryBaseMagnitude : Settings.Current.PerfectParryMagnitude;
                if (attackerWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
                {
                    parryMagnitude *= Settings.Current.BonusAgainstShieldMalus;
                }
                int attackerWeaponValue = GetValueForWeaponClass(attackerWeapon.WeaponClass);
                if (attackerWeaponValue == 0)
                {
                    parryMagnitude *= Settings.Current.TwoHandedParryMalus;
                }
                else if (attackerWeaponValue == 1)
                {
                    parryMagnitude *= Settings.Current.DaggerParryBonus;
                }
                int victimWeaponValue = GetValueForWeaponClass(attackInformation.VictimMainHandWeapon.CurrentUsageItem.WeaponClass);
                if (victimWeaponValue == 0)
                {
                    parryMagnitude *= Settings.Current.TwoHandedParryBonus;
                }
                else if (victimWeaponValue == 1)
                {
                    parryMagnitude *= Settings.Current.DaggerParryMalus;
                }
                if (Settings.Current.SkillLevelMagnitude)
                {
                    int attackerSkill = attackInformation.AttackerAgentCharacter.GetSkillValue(attackerWeapon.RelevantSkill);
                    int victimSkill = attackInformation.VictimAgentCharacter.GetSkillValue(attackInformation.VictimMainHandWeapon.CurrentUsageItem.RelevantSkill);
                    if (attackerSkill > victimSkill)
                    {
                        parryMagnitude /= (victimSkill - attackerSkill) / 100f;
                    }
                    else if (victimSkill > attackerSkill)
                    {
                        parryMagnitude *= (victimSkill - attackerSkill) / 100f;
                    }
                }
                float newMagnitude = MathF.Max(0f, magnitude - (magnitude * parryMagnitude));
                if (Settings.Current.Logging && (attackInformation.IsAttackerPlayer || attackInformation.IsVictimPlayer))
                {
                    InformationManager.DisplayMessage(new InformationMessage(string.Format("magnitude before parry:{0} after:{1} ", magnitude, newMagnitude)));
                }
                magnitude = newMagnitude;
            }
            return true;
        }

        private static int GetValueForWeaponClass(WeaponClass weaponClass)
        {
            if (weaponClass == WeaponClass.TwoHandedAxe || weaponClass == WeaponClass.TwoHandedSword || weaponClass == WeaponClass.TwoHandedMace || weaponClass == WeaponClass.TwoHandedPolearm)
            {
                return 0;
            }
            if (weaponClass == WeaponClass.Dagger)
            {
                return 1;
            }
            return -1;
        }
    }
}