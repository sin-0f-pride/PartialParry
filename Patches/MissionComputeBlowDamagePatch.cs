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
                if (attackerWeapon != null)
                {
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
                }
                WeaponComponentData victimWeapon = attackInformation.VictimMainHandWeapon.CurrentUsageItem;
                if (victimWeapon != null)
                {
                    int victimWeaponValue = GetValueForWeaponClass(victimWeapon.WeaponClass);
                    if (victimWeaponValue == 0)
                    {
                        parryMagnitude *= Settings.Current.TwoHandedParryBonus;
                    }
                    else if (victimWeaponValue == 1)
                    {
                        parryMagnitude *= Settings.Current.DaggerParryMalus;
                    }
                }
                if (Settings.Current.SkillLevelMagnitude)
                {
                    SkillObject attackerSkill = attackerWeapon.RelevantSkill;
                    SkillObject victimSkill = victimWeapon.RelevantSkill;
                    if (attackerSkill != null && victimSkill != null)
                    {
                        int attackerSkillLevel = attackInformation.AttackerAgentCharacter.GetSkillValue(attackerSkill);
                        int victimSkillLevel = attackInformation.VictimAgentCharacter.GetSkillValue(victimWeapon.RelevantSkill);
                        float skillMagnitude = attackerSkillLevel - victimSkillLevel;
                        if (skillMagnitude > 0)
                        {
                            parryMagnitude /= skillMagnitude;
                        }
                        else
                        {
                            parryMagnitude *= MathF.Abs(skillMagnitude);
                        }
                        if (Settings.Current.Logging)
                        {
                            SubModule.Log("Attacker Skill=" + attackerSkillLevel + ", Victim Skill=" + victimSkillLevel + ", SkillMagnitude=" + skillMagnitude);
                        }
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