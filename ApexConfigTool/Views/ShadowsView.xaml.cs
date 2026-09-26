using System.Windows.Controls;

using ApexConfigTool.Services;

namespace ApexConfigTool.Views
{
    public partial class ShadowsView : UserControl
    {
        private const string Keep =
            "__KEEP__";


        public ShadowsView()
        {
            InitializeComponent();

            ResetSelections();
        }


        private void ResetSelections()
        {
            ShadowEnableCombo.SelectedIndex = 0;
            DynamicShadowCombo.SelectedIndex = 0;
            CsmEnabledCombo.SelectedIndex = 0;
            NewShadowSettingsCombo.SelectedIndex = 0;

            CsmCoverageCombo.SelectedIndex = 0;
            CsmResolutionCombo.SelectedIndex = 0;
            ShadowDepthCombo.SelectedIndex = 0;
            ShadowUpresCombo.SelectedIndex = 0;
        }


        private string GetValue(
            ComboBox combo)
        {
            ComboBoxItem? item =
                combo.SelectedItem
                as ComboBoxItem;


            return item?.Tag?.ToString()
                   ?? Keep;
        }


        private void SelectValue(
            ComboBox combo,
            string value)
        {
            foreach (ComboBoxItem item
                     in combo.Items)
            {
                if (item.Tag?.ToString()
                    == value)
                {
                    combo.SelectedItem =
                        item;

                    return;
                }
            }


            combo.SelectedIndex = 0;
        }


        public void LoadSettings(
            VideoConfigService service,
            string config)
        {
            DisableAllShadowsCheck.IsChecked =
                false;


            SelectValue(
                ShadowEnableCombo,
                service.GetSetting(
                    config,
                    "setting.shadow_enable"));


            SelectValue(
                DynamicShadowCombo,
                service.GetSetting(
                    config,
                    "setting.shadow_maxdynamic"));


            SelectValue(
                CsmEnabledCombo,
                service.GetSetting(
                    config,
                    "setting.csm_enabled"));


            SelectValue(
                NewShadowSettingsCombo,
                service.GetSetting(
                    config,
                    "setting.new_shadow_settings"));


            SelectValue(
                CsmCoverageCombo,
                service.GetSetting(
                    config,
                    "setting.csm_coverage"));


            SelectValue(
                CsmResolutionCombo,
                service.GetSetting(
                    config,
                    "setting.csm_cascade_res"));


            SelectValue(
                ShadowDepthCombo,
                service.GetSetting(
                    config,
                    "setting.shadow_depth_dimen_min"));


            SelectValue(
                ShadowUpresCombo,
                service.GetSetting(
                    config,
                    "setting.shadow_depth_upres_factor_max"));
        }


        private string ApplySetting(
            VideoConfigService service,
            string config,
            ComboBox combo,
            string setting)
        {
            string value =
                GetValue(combo);


            if (value == Keep)
                return config;


            return service.SetSettingIfPresent(
                config,
                setting,
                value);
        }


        public string ApplyToConfig(
            VideoConfigService service,
            string config)
        {
            if (DisableAllShadowsCheck.IsChecked
                == true)
            {
                return service.DisableShadows(
                    config);
            }


            config =
                ApplySetting(
                    service,
                    config,
                    ShadowEnableCombo,
                    "setting.shadow_enable");


            config =
                ApplySetting(
                    service,
                    config,
                    DynamicShadowCombo,
                    "setting.shadow_maxdynamic");


            config =
                ApplySetting(
                    service,
                    config,
                    CsmEnabledCombo,
                    "setting.csm_enabled");


            config =
                ApplySetting(
                    service,
                    config,
                    NewShadowSettingsCombo,
                    "setting.new_shadow_settings");


            config =
                ApplySetting(
                    service,
                    config,
                    CsmCoverageCombo,
                    "setting.csm_coverage");


            config =
                ApplySetting(
                    service,
                    config,
                    CsmResolutionCombo,
                    "setting.csm_cascade_res");


            config =
                ApplySetting(
                    service,
                    config,
                    ShadowDepthCombo,
                    "setting.shadow_depth_dimen_min");


            config =
                ApplySetting(
                    service,
                    config,
                    ShadowUpresCombo,
                    "setting.shadow_depth_upres_factor_max");


            return config;
        }


        public void ApplyPreset(
            string preset)
        {
            DisableAllShadowsCheck.IsChecked =
                false;


            switch (preset)
            {
                case "Competitive":

                    SetLow();
                    break;


                case "Maximum FPS":

                    DisableAllShadowsCheck.IsChecked =
                        true;

                    SetLow();
                    break;


                case "Balanced":

                    SetOn();
                    break;


                case "Quality":

                    SetOn();

                    SelectValue(
                        CsmResolutionCombo,
                        "2048");

                    SelectValue(
                        ShadowDepthCombo,
                        "1024");

                    break;
            }
        }


        private void SetLow()
        {
            SelectValue(
                ShadowEnableCombo,
                "0");

            SelectValue(
                DynamicShadowCombo,
                "0");

            SelectValue(
                CsmEnabledCombo,
                "0");

            SelectValue(
                NewShadowSettingsCombo,
                "0");

            SelectValue(
                CsmCoverageCombo,
                "0");

            SelectValue(
                CsmResolutionCombo,
                "0");

            SelectValue(
                ShadowDepthCombo,
                "0");

            SelectValue(
                ShadowUpresCombo,
                "0");
        }


        private void SetOn()
        {
            SelectValue(
                ShadowEnableCombo,
                "1");

            SelectValue(
                DynamicShadowCombo,
                "1");

            SelectValue(
                CsmEnabledCombo,
                "1");

            SelectValue(
                NewShadowSettingsCombo,
                "1");
        }
    }
}