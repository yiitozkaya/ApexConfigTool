using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ApexConfigTool.Models;

namespace ApexConfigTool.Services
{
    public class AutoexecDiffService
    {
        public List<ConfigChange> Compare(
            AutoexecService service,
            string oldContent,
            string newContent)
        {
            Dictionary<string, string> oldCommands =
                service.ParseCommands(
                    oldContent);


            Dictionary<string, string> newCommands =
                service.ParseCommands(
                    newContent);


            HashSet<string> commands =
                new HashSet<string>(
                    oldCommands.Keys,
                    StringComparer.OrdinalIgnoreCase);


            commands.UnionWith(
                newCommands.Keys);


            List<ConfigChange> changes =
                new List<ConfigChange>();


            foreach (string command in commands)
            {
                string oldValue =
                    oldCommands.TryGetValue(
                        command,
                        out string? oldResult)
                        ? oldResult
                        : "(default)";


                string newValue =
                    newCommands.TryGetValue(
                        command,
                        out string? newResult)
                        ? newResult
                        : "(default)";


                if (oldValue == newValue)
                    continue;


                changes.Add(
                    new ConfigChange
                    {
                        Source =
                            "Autoexec",

                        Setting =
                            GetFriendlyName(command),

                        OldValue =
                            FormatValue(
                                command,
                                oldValue),

                        NewValue =
                            FormatValue(
                                command,
                                newValue)
                    });
            }


            return changes
                .OrderBy(x => x.Setting)
                .ToList();
        }


        private string GetFriendlyName(
            string command)
        {
            return command switch
            {
                "cl_fovScale" =>
                    "FOV",

                "fov_disableAbilityScaling" =>
                    "Ability FOV scaling",

                "fps_max" =>
                    "FPS limit",

                "lobby_max_fps" =>
                    "Lobby FPS limit",

                "m_rawinput" =>
                    "Raw mouse input",

                "m_acceleration" =>
                    "Mouse acceleration",

                "m_customaccel" =>
                    "Custom acceleration",

                "noise_filter_scale" =>
                    "Film grain",

                "hud_setting_adsDof" =>
                    "ADS depth of field",

                "sprint_view_shake_style" =>
                    "Sprint view shake",

                "r_drawmodeldecals" =>
                    "Model decals",

                "r_forcecheapwater" =>
                    "Water rendering",

                _ => command
            };
        }


        private string FormatValue(
            string command,
            string value)
        {
            if (value == "(default)")
                return value;


            if (command == "cl_fovScale")
            {
                if (!double.TryParse(
                        value,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double scale))
                {
                    return value;
                }


                if (Math.Abs(scale - 1.00) < 0.02)
                    return "70";

                if (Math.Abs(scale - 1.27) < 0.02)
                    return "90";

                if (Math.Abs(scale - 1.485714) < 0.02)
                    return "104";

                if (Math.Abs(scale - 1.55) < 0.02)
                    return "110";

                if (Math.Abs(scale - 1.70) < 0.02)
                    return "120";
            }


            if (command ==
                "fov_disableAbilityScaling")
            {
                return value == "1"
                    ? "Disabled"
                    : "Default";
            }


            if (command == "m_rawinput")
            {
                return value == "1"
                    ? "On"
                    : "Off";
            }


            if (command == "m_acceleration")
            {
                return value == "0"
                    ? "Disabled"
                    : "Enabled";
            }


            return value;
        }
    }
}