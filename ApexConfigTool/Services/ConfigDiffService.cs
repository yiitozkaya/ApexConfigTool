using System.Collections.Generic;
using System.Linq;

using ApexConfigTool.Models;

namespace ApexConfigTool.Services
{
    public class ConfigDiffService
    {
        public List<ConfigChange> Compare(
            VideoConfigService service,
            string oldConfig,
            string newConfig)
        {
            Dictionary<string, string> oldSettings =
                service
                    .GetAllSettings(oldConfig)
                    .GroupBy(x => x.Setting)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Last().Value);


            Dictionary<string, string> newSettings =
                service
                    .GetAllSettings(newConfig)
                    .GroupBy(x => x.Setting)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Last().Value);


            HashSet<string> keys =
                new HashSet<string>(
                    oldSettings.Keys);


            keys.UnionWith(
                newSettings.Keys);


            List<ConfigChange> changes =
                new List<ConfigChange>();


            foreach (string key in keys)
            {
                string oldValue =
                    oldSettings.TryGetValue(
                        key,
                        out string? oldResult)
                        ? oldResult
                        : "(missing)";


                string newValue =
                    newSettings.TryGetValue(
                        key,
                        out string? newResult)
                        ? newResult
                        : "(missing)";


                if (oldValue == newValue)
                    continue;


                changes.Add(
                    new ConfigChange
                    {
                        Source = "Video",

                        Setting =
                            GetFriendlyName(key),

                        OldValue =
                            oldValue,

                        NewValue =
                            newValue
                    });
            }


            return changes
                .OrderBy(x => x.Setting)
                .ToList();
        }


        private string GetFriendlyName(
            string key)
        {
            return key switch
            {
                "setting.defaultres" =>
                    "Resolution width",

                "setting.defaultresheight" =>
                    "Resolution height",

                "setting.fullscreen" =>
                    "Fullscreen",

                "setting.mat_vsync_mode" =>
                    "VSync",

                "setting.mat_antialias_mode" =>
                    "Anti-aliasing",

                "setting.mat_forceaniso" =>
                    "Texture filtering",

                "setting.mat_picmip" =>
                    "Texture resolution",

                "setting.stream_memory" =>
                    "Streaming budget",

                "setting.r_lod_switch_scale" =>
                    "Model detail",

                "setting.particle_cpu_level" =>
                    "Particle quality",

                "setting.cl_ragdoll_maxcount" =>
                    "Ragdolls",

                "setting.r_decals" =>
                    "World decals",

                "setting.volumetric_lighting" =>
                    "Volumetric lighting",

                "setting.volumetric_fog" =>
                    "Volumetric fog",

                "setting.dvs_enable" =>
                    "Dynamic resolution",

                "setting.dynamic_streaming_budget" =>
                    "Dynamic streaming",

                "setting.ssao_quality" =>
                    "Ambient occlusion",

                "setting.ssao_enabled" =>
                    "Ambient occlusion",

                "setting.map_detail_level" =>
                    "Map detail",

                "setting.shadow_enable" =>
                    "Shadows",

                "setting.shadow_maxdynamic" =>
                    "Dynamic shadows",

                "setting.csm_enabled" =>
                    "Cascade shadows",

                "setting.csm_coverage" =>
                    "CSM coverage",

                "setting.csm_cascade_res" =>
                    "CSM resolution",

                "setting.new_shadow_settings" =>
                    "Shadow system",

                _ => key
            };
        }
    }
}