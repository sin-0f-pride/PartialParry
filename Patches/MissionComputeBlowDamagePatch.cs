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
                    float skillMagnitude = attackerSkill - victimSkill;
                    if (Settings.Current.Logging)
                    {
                        SubModule.Log("Attacker Skill=" + attackerSkill + ", Victim Skill=" + victimSkill + ", SkillMagnitude=" + skillMagnitude);
                    }
                    if (skillMagnitude > 0)
                    {
                        parryMagnitude /= skillMagnitude;
                    }
                    else
                    {
                        parryMagnitude *= MathF.Abs(skillMagnitude);
                    }
                }
                float newMagnitude = MathF.Max(0f, magnitude - (magnitude * parryMagnitude));
                if (Settings.Current.Logging && (attackInformation.IsAttackerPlayer || attackInformation.IsVictimPlayer))
                {
                    SubModule.Log("Before=" + magnitude + ", After=" + newMagnitude);
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