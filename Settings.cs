using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace PartialParry
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public Settings() 
        {
            //Rmpty
        }

        private float _parryBaseMagnitude = 0.3f;
        private float _perfectParryMagnitude = 0.6f;
        private float _daggerParryBonus = 1.25f;
        private float _twoHandedParryBonus = 2f;
        private float _bonusAgainstShieldMalus = 0.9f;
        private float _daggerParryMalus = 0.8f;
        private float _twoHandedParryMalus = 0.5f;
        private bool _skillLevelMagnitude = false;
        private bool _logging = false;

        public override string Id => "PartialParry";
        public override string DisplayName => $"Partial Parry";
        public override string FolderName => "PartialParry";
        public override string FormatType => "json2";

        [SettingPropertyFloatingInteger("Parry base magnitude", 0f, 1f, "#0%", RequireRestart = false, HintText = "Base damage to be blocked by parry before crushing through.")]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public float ParryBaseMagnitude
        {
            get => _parryBaseMagnitude;
            set
            {
                if (_parryBaseMagnitude != value)
                {
                    _parryBaseMagnitude = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyFloatingInteger("Perfect parry magnitude", 0f, 1f, "#0%", RequireRestart = false, HintText = "Damage to be blocked by perfectly timed parry before crushing through.")]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public float PerfectParryMagnitude
        {
            get => _perfectParryMagnitude;
            set
            {
                if (_perfectParryMagnitude != value)
                {
                    _perfectParryMagnitude = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyBool("Skill Level Magnitude (EXPERIMENTAL)", RequireRestart = false, HintText = "Damage bonuses & maluses based upon the skill level of the attackers skill in their weapon, as well as the skill level of the defenders skill in their parrying weapon.")]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public bool SkillLevelMagnitude
        {
            get => _skillLevelMagnitude;
            set
            {
                if (_skillLevelMagnitude != value)
                {
                    _skillLevelMagnitude = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyFloatingInteger("Parry bonus vs Daggers", 0f, 1f, RequireRestart = false, HintText = "Damage decrease when parrying against a dagger.")]
        [SettingPropertyGroup("Bonus", GroupOrder = 10)]
        public float DaggerParryBonus
        {
            get => _daggerParryBonus;
            set
            {
                if (_daggerParryBonus != value)
                {
                    _daggerParryBonus = value;
                    OnPropertyChanged();
                }
            }
        }
        [SettingPropertyFloatingInteger("Parry bonus using Two handed weapons", 0f, 1f, RequireRestart = false, HintText = "Damage decrease when parrying with a two handed weapon.")]
        [SettingPropertyGroup("Bonus", GroupOrder = 10)]
        public float TwoHandedParryBonus
        {
            get => _twoHandedParryBonus;
            set
            {
                if (_twoHandedParryBonus != value)
                {
                    _twoHandedParryBonus = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyFloatingInteger("Parry malus vs Bonus shield damage weapons", 0f, 1f, RequireRestart = false, HintText = "Damage increase when parrying against a weapon that has bonus damage to shield, such as an axe.")]
        [SettingPropertyGroup("Malus", GroupOrder = 20)]
        public float BonusAgainstShieldMalus
        {
            get => _bonusAgainstShieldMalus;
            set
            {
                if (_bonusAgainstShieldMalus != value)
                {
                    _bonusAgainstShieldMalus = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyFloatingInteger("Parry malus using Daggers", 0f, 1f, RequireRestart = false, HintText = "Damage increase when parrying with a dagger.")]
        [SettingPropertyGroup("Malus", GroupOrder = 20)]
        public float DaggerParryMalus
        {
            get => _daggerParryMalus;
            set
            {
                if (_daggerParryMalus != value)
                {
                    _daggerParryMalus = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyFloatingInteger("Parry malus vs Two handed weapons", 0f, 1f, RequireRestart = false, HintText = "Damage increase when parrying against a two handed weapon.")]
        [SettingPropertyGroup("Malus", GroupOrder = 20)]
        public float TwoHandedParryMalus
        {
            get => _twoHandedParryMalus;
            set
            {
                if (_twoHandedParryMalus != value)
                {
                    _twoHandedParryMalus = value;
                    OnPropertyChanged();
                }
            }
        }
        [SettingPropertyBool("Logging", RequireRestart = false, HintText = "Displays before and after magnitudes in the game log.")]
        [SettingPropertyGroup("General")]
        public bool Logging
        {
            get => _logging;
            set
            {
                if (_logging != value)
                {
                    _logging = value;
                    OnPropertyChanged();
                }
            }
        }
        public static Settings Current
        {
            get => Instance!;
        }
    }
}