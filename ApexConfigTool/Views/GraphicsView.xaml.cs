using System.Windows.Controls;

using ApexConfigTool.Services;

namespace ApexConfigTool.Views
{
    public partial class GraphicsView : UserControl
    {
        private const string Keep =
            "__KEEP__";


        public GraphicsView()
        {
            InitializeComponent();

            ResetSelections();
        }


        private void ResetSelections()
        {
            TextureBudgetCombo.SelectedIndex = 0;
            TextureFilteringCombo.SelectedIndex = 0;
            TextureResolutionCombo.SelectedIndex = 0;

            ModelDetailCombo.SelectedIndex = 0;
            ParticleQualityCombo.SelectedIndex = 0;
            RagdollCountCombo.SelectedIndex = 0;
            DecalsCombo.SelectedIndex = 0;

            AntiAliasingCombo.SelectedIndex = 0;
            VSyncCombo.SelectedIndex = 0;
            VolumetricLightingCombo.SelectedIndex = 0;
            VolumetricFogCombo.SelectedIndex = 0;

            DynamicResolutionCombo.SelectedIndex = 0;
            DynamicStreamingCombo.SelectedIndex = 0;
            AmbientOcclusionCombo.SelectedIndex = 0;
            MapDetailCombo.SelectedIndex = 0;
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
            SelectValue(
                TextureBudgetCombo,
                service.GetSetting(
                    config,
                    "setting.stream_memory"));


            SelectValue(
                TextureFilteringCombo,
                service.GetSetting(
                    config,
                    "setting.mat_forceaniso"));


            SelectValue(
                TextureResolutionCombo,
                service.GetSetting(
                    config,
                    "setting.mat_picmip"));


            SelectValue(
                ModelDetailCombo,
                NormalizeFloat(
                    service.GetSetting(
                        config,
                        "setting.r_lod_switch_scale")));


            SelectValue(
                ParticleQualityCombo,
                service.GetSetting(
                    config,
                    "setting.particle_cpu_level"));


            SelectValue(
                RagdollCountCombo,
                service.GetSetting(
                    config,
                    "setting.cl_ragdoll_maxcount"));


            SelectValue(
                DecalsCombo,
                service.GetSetting(
                    config,
                    "setting.r_decals"));


            SelectValue(
                AntiAliasingCombo,
                service.GetSetting(
                    config,
                    "setting.mat_antialias_mode"));


            SelectValue(
                VSyncCombo,
                service.GetSetting(
                    config,
                    "setting.mat_vsync_mode"));


            SelectValue(
                VolumetricLightingCombo,
                service.GetSetting(
                    config,
                    "setting.volumetric_lighting"));


            SelectValue(
                VolumetricFogCombo,
                service.GetSetting(
                    config,
                    "setting.volumetric_fog"));


            SelectValue(
                DynamicResolutionCombo,
                service.GetSetting(
                    config,
                    "setting.dvs_enable"));


            SelectValue(
                DynamicStreamingCombo,
                service.GetSetting(
                    config,
                    "setting.dynamic_streaming_budget"));


            LoadAmbientOcclusion(
                service,
                config);


            SelectValue(
                MapDetailCombo,
                service.GetSetting(
                    config,
                    "setting.map_detail_level"));
        }


        private void LoadAmbientOcclusion(
            VideoConfigService service,
            string config)
        {
            if (service.HasSetting(
                config,
                "setting.ssao_quality"))
            {
                SelectValue(
                    AmbientOcclusionCombo,
                    service.GetSetting(
                        config,
                        "setting.ssao_quality"));

                return;
            }


            if (service.HasSetting(
                config,
                "setting.ssao_enabled"))
            {
                string enabled =
                    service.GetSetting(
                        config,
                        "setting.ssao_enabled");


                SelectValue(
                    AmbientOcclusionCombo,
                    enabled == "0"
                        ? "0"
                        : "1");

                return;
            }


            AmbientOcclusionCombo.SelectedIndex =
                0;
        }


        private string NormalizeFloat(
            string value)
        {
            if (value == "0.6")
                return "0.600000";

            if (value == "0.8")
                return "0.800000";

            if (value == "1.000000")
                return "1";

            if (value == "2.000000")
                return "2";


            return value;
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
            config = ApplySetting(
                service,
                config,
                TextureBudgetCombo,
                "setting.stream_memory");


            config = ApplySetting(
                service,
                config,
                TextureFilteringCombo,
                "setting.mat_forceaniso");


            config = ApplySetting(
                service,
                config,
                TextureResolutionCombo,
                "setting.mat_picmip");


            config = ApplySetting(
                service,
                config,
                ModelDetailCombo,
                "setting.r_lod_switch_scale");


            config = ApplySetting(
                service,
                config,
                ParticleQualityCombo,
                "setting.particle_cpu_level");


            config = ApplySetting(
                service,
                config,
                RagdollCountCombo,
                "setting.cl_ragdoll_maxcount");


            config = ApplySetting(
                service,
                config,
                DecalsCombo,
                "setting.r_decals");


            config = ApplySetting(
                service,
                config,
                AntiAliasingCombo,
                "setting.mat_antialias_mode");


            config = ApplySetting(
                service,
                config,
                VSyncCombo,
                "setting.mat_vsync_mode");


            config = ApplySetting(
                service,
                config,
                VolumetricLightingCombo,
                "setting.volumetric_lighting");


            config = ApplySetting(
                service,
                config,
                VolumetricFogCombo,
                "setting.volumetric_fog");


            config = ApplySetting(
                service,
                config,
                DynamicResolutionCombo,
                "setting.dvs_enable");


            config = ApplySetting(
                service,
                config,
                DynamicStreamingCombo,
                "setting.dynamic_streaming_budget");


            config = ApplySetting(
                service,
                config,
                MapDetailCombo,
                "setting.map_detail_level");


            config =
                ApplyAmbientOcclusion(
                    service,
                    config);


            return config;
        }


        private string ApplyAmbientOcclusion(
            VideoConfigService service,
            string config)
        {
            string value =
                GetValue(
                    AmbientOcclusionCombo);


            if (value == Keep)
                return config;


            config =
                service.SetSettingIfPresent(
                    config,
                    "setting.ssao_quality",
                    value);


            config =
                service.SetSettingIfPresent(
                    config,
                    "setting.ssao_enabled",
                    value == "0"
                        ? "0"
                        : "1");


            return config;
        }


        public void ApplyPreset(
            string preset)
        {
            switch (preset)
            {
                case "Competitive":

                    SelectValue(TextureBudgetCombo, "600000");
                    SelectValue(TextureFilteringCombo, "4");
                    SelectValue(TextureResolutionCombo, "1");

                    SelectValue(ModelDetailCombo, "0.800000");
                    SelectValue(ParticleQualityCombo, "0");
                    SelectValue(RagdollCountCombo, "0");
                    SelectValue(DecalsCombo, "0");

                    SelectValue(AntiAliasingCombo, "0");
                    SelectValue(VSyncCombo, "0");
                    SelectValue(VolumetricLightingCombo, "0");
                    SelectValue(VolumetricFogCombo, "0");

                    SelectValue(DynamicResolutionCombo, "0");
                    SelectValue(DynamicStreamingCombo, "0");
                    SelectValue(AmbientOcclusionCombo, "0");
                    SelectValue(MapDetailCombo, "0");

                    break;


                case "Maximum FPS":

                    SelectValue(TextureBudgetCombo, "300000");
                    SelectValue(TextureFilteringCombo, "0");
                    SelectValue(TextureResolutionCombo, "2");

                    SelectValue(ModelDetailCombo, "0.600000");
                    SelectValue(ParticleQualityCombo, "0");
                    SelectValue(RagdollCountCombo, "0");
                    SelectValue(DecalsCombo, "0");

                    SelectValue(AntiAliasingCombo, "0");
                    SelectValue(VSyncCombo, "0");
                    SelectValue(VolumetricLightingCombo, "0");
                    SelectValue(VolumetricFogCombo, "0");

                    SelectValue(DynamicResolutionCombo, "0");
                    SelectValue(DynamicStreamingCombo, "0");
                    SelectValue(AmbientOcclusionCombo, "0");
                    SelectValue(MapDetailCombo, "0");

                    break;


                case "Balanced":

                    SelectValue(TextureBudgetCombo, "1000000");
                    SelectValue(TextureFilteringCombo, "4");
                    SelectValue(TextureResolutionCombo, "1");

                    SelectValue(ModelDetailCombo, "1");
                    SelectValue(ParticleQualityCombo, "1");
                    SelectValue(RagdollCountCombo, "4");
                    SelectValue(DecalsCombo, "256");

                    SelectValue(AntiAliasingCombo, "12");
                    SelectValue(VSyncCombo, "0");
                    SelectValue(VolumetricLightingCombo, "0");
                    SelectValue(VolumetricFogCombo, "0");

                    SelectValue(DynamicResolutionCombo, "0");
                    SelectValue(DynamicStreamingCombo, "1");
                    SelectValue(AmbientOcclusionCombo, "1");
                    SelectValue(MapDetailCombo, "1");

                    break;


                case "Quality":

                    SelectValue(TextureBudgetCombo, "3000000");
                    SelectValue(TextureFilteringCombo, "16");
                    SelectValue(TextureResolutionCombo, "0");

                    SelectValue(ModelDetailCombo, "2");
                    SelectValue(ParticleQualityCombo, "2");
                    SelectValue(RagdollCountCombo, "8");
                    SelectValue(DecalsCombo, "256");

                    SelectValue(AntiAliasingCombo, "12");
                    SelectValue(VSyncCombo, "0");
                    SelectValue(VolumetricLightingCombo, "1");
                    SelectValue(VolumetricFogCombo, "1");

                    SelectValue(DynamicResolutionCombo, "0");
                    SelectValue(DynamicStreamingCombo, "1");
                    SelectValue(AmbientOcclusionCombo, "4");
                    SelectValue(MapDetailCombo, "2");

                    break;
            }
        }
    }
}