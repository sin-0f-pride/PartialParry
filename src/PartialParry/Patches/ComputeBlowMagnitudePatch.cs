using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace PartialParry.Patches
{
    [HarmonyPatch(typeof(MissionCombatMechanicsHelper), "ComputeBlowMagnitude")]
    public static class ComputeBlowMagnitudePatch
    {
        // here we reduce the magnitude which his the damage before armor so that parry will not always parry everything
        public static void Postfix(AttackCollisionData acd, AttackInformation attackInformation, MissionWeapon weapon, ref float specialMagnitude)
        {
            if (!acd.AttackBlockedWithShield && (acd.CollisionResult == CombatCollisionResult.Parried || acd.CollisionResult == CombatCollisionResult.Blocked))
            {
                float parryMagnitude = acd.CollisionResult != CombatCollisionResult.Parried ? Settings.Current.ParryBaseMagnitude : Settings.Current.PerfectParryMagnitude;
                WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
                if (currentUsageItem != null)
                {
                    if (currentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
                    {
                        parryMagnitude *= Settings.Current.BonusAgainstShieldMalus;
                    }
                    int attackerWeaponValue = GetValueForWeaponClass(currentUsageItem.WeaponClass);
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
                    int attackerSkillLevel = 0;
                    int victimSkillLevel = 0;
                    if (currentUsageItem != null)
                    {
                        attackerSkillLevel = attackInformation.AttackerAgentCharacter.GetSkillValue(currentUsageItem.RelevantSkill);
                    }
                    if (victimWeapon != null)
                    {
                        victimSkillLevel = attackInformation.VictimAgentCharacter.GetSkillValue(victimWeapon.RelevantSkill);
                    }
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
                float newMagnitude = MathF.Max(0f, specialMagnitude - (specialMagnitude * parryMagnitude));
                if (Settings.Current.Logging && (attackInformation.IsAttackerPlayer || attackInformation.IsVictimPlayer))
                {
                    SubModule.Log("Before=" + specialMagnitude + ", After=" + newMagnitude);
                }
                specialMagnitude = newMagnitude;
            }
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
