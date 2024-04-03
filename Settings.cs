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

        private int _baseMagnitudeParried = 100;
        private int _perfectParryBonus = 150;
        private bool _showLog = false;
        private int _shieldBreakWeaponDefendMalus = 90;
        private int _twoHandedWeaponParryBonus = 150;
        private int _littleWeaponParryMalus = 75;

        public override string Id => "PartialParryMeleeHit";
        public override string DisplayName => $"Parry doesn't always block everything";
        public override string FolderName => "Parry Setting";
        public override string FormatType => "json2";

        [SettingPropertyInteger("parry base magnitude", 0, 1000, RequireRestart = false, HintText = "how much base damage you can block before it starts crushing through parry")]
        [SettingPropertyGroup("General")]
        public int baseMagnitudeParried
        {
            get => _baseMagnitudeParried;
            set
            {
                if (_baseMagnitudeParried != value)
                {
                    _baseMagnitudeParried = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyInteger("perfect parry bonus in %", 0, 1000, RequireRestart = false, HintText = "multiply the \"parry base magnitude\" by this percentage if you get a perfect parry (blocking right before you get hit)")]
        [SettingPropertyGroup("General")]
        public int perfectParryBonus
        {
            get => _perfectParryBonus;
            set
            {
                if (_perfectParryBonus != value)
                {
                    _perfectParryBonus = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyBool("should show log", Order = 1, RequireRestart = false, HintText = "show the magnitude before and after parry in log")]
        [SettingPropertyGroup("General")]
        public bool showLog
        {
            get => _showLog;
            set
            {
                if (_showLog != value)
                {
                    _showLog = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyInteger("malus against shield break weapon %", 0, 100, RequireRestart = false, HintText = "multiply the \"parry base magnitude\" by this percentage if you parry a hit from a weapon with shield damage bonus on shield")]
        [SettingPropertyGroup("General")]
        public int shieldBreakWeaponDefendMalus
        {
            get => _shieldBreakWeaponDefendMalus;
            set
            {
                if (_shieldBreakWeaponDefendMalus != value)
                {
                    _shieldBreakWeaponDefendMalus = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyInteger("malus when parrying with a dagger in %", 0, 100, RequireRestart = false, HintText = "multiply the \"parry base magnitude\" by this percentage if you parry with a dagger")]
        [SettingPropertyGroup("General")]
        public int littleWeaponParryMalus
        {
            get => _littleWeaponParryMalus;
            set
            {
                if (_littleWeaponParryMalus != value)
                {
                    _littleWeaponParryMalus = value;
                    OnPropertyChanged();
                }
            }
        }

        [SettingPropertyInteger("two hand weapon bonus in %", 0, 1000, RequireRestart = false, HintText = " multiply the \"parry base magnitude\" by this percentage if you're using a two handed weapon")]
        [SettingPropertyGroup("General")]
        public int twoHandedWeaponParryBonus
        {
            get => _twoHandedWeaponParryBonus;
            set
            {
                if (_twoHandedWeaponParryBonus != value)
                {
                    _twoHandedWeaponParryBonus = value;
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