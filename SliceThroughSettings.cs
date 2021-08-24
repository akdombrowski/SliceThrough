using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace SliceThrough
{
    internal class SliceThroughSettings : AttributeGlobalSettings<SliceThroughSettings>
    {
        public SliceThroughSettings()
        {
            this.RemainingMomentum = 0;
        }

        public override string DisplayName => "SliceThrough";
        public override string FolderName => "SliceThrough";
        public override string FormatType => "json2";
        public override string Id => "SliceThrough";

        [SettingPropertyGroup("{=MCM_001_Settings_Header}General Mod Settings")]
        [SettingPropertyFloatingInteger("{=MCM_001_Settings_Name_001}Remaining Momentum", 0, 100000, "0", HintText = "{=MCM_001_Settings_Info_001}Sets the remaining momentum after a melee swing makes contact. Higher numbers will have higher damage. (0 = native)", RequireRestart = false)]
        public int RemainingMomentum
        {
            get; set;
        }
    }
}