using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using ApexConfigTool.Models;
using ApexConfigTool.Services;

namespace ApexConfigTool.Views
{
    public partial class AutoexecView : UserControl
    {
        private const string Keep =
            "__KEEP__";


        private ApexInstallation?
            installation;


        private AutoexecService?
            autoexecService;


        private readonly ProcessService
            processService =
                new();


        private string loadedFov =
            Keep;

        private string loadedAbilityFov =
            Keep;

        private string loadedRawInput =
            Keep;

        private string loadedAcceleration =
            Keep;

        private string loadedFilmGrain =
            Keep;

        private string loadedAdsDof =
            Keep;

        private string loadedViewShake =
            Keep;

        private string loadedModelDecals =
            Keep;

        private string loadedCheapWater =
            Keep;

        private string loadedFps =
            "180";

        private string loadedLobbyFps =
            "60";


        public event Action<string>?
            StatusChanged;


        public AutoexecView()
        {
            InitializeComponent();

            ResetCombos();
        }


        private void ResetCombos()
        {
            FovCombo.SelectedIndex = 0;
            AbilityFovCombo.SelectedIndex = 0;

            RawInputCombo.SelectedIndex = 0;
            MouseAccelerationCombo.SelectedIndex = 0;

            FilmGrainCombo.SelectedIndex = 0;
            AdsDofCombo.SelectedIndex = 0;
            ViewShakeCombo.SelectedIndex = 0;
            ModelDecalsCombo.SelectedIndex = 0;
            CheapWaterCombo.SelectedIndex = 0;
        }


        public void LoadInstallation(
            ApexInstallation apexInstallation)
        {
            installation =
                apexInstallation;


            LauncherText.Text =
                apexInstallation.Launcher;


            GamePathText.Text =
                apexInstallation.GameFound
                    ? apexInstallation.GameDirectory
                    : "Not detected";


            AutoexecPathText.Text =
                apexInstallation.GameFound
                    ? apexInstallation.AutoexecPath
                    : "Unavailable";


            if (!apexInstallation.GameFound)
            {
                autoexecService =
                    null;


                AutoexecStateText.Text =
                    "Unavailable";


                LaunchOptionsBox.Text =
                    "";


                RawEditor.Text =
                    "";


                return;
            }


            autoexecService =
                new AutoexecService(
                    apexInstallation.AutoexecPath);


            LaunchOptionsBox.Text =
                apexInstallation.Launcher
                    == "Steam"
                        ? "+exec autoexec"
                        : apexInstallation.Launcher
                            == "EA App"
                                ? "-exec autoexec"
                                : "exec autoexec";


            ReloadFromDisk();
        }


        public void ReloadFromDisk()
        {
            if (autoexecService == null)
                return;


            bool exists =
                autoexecService.Exists();


            AutoexecStateText.Text =
                exists
                    ? "Found"
                    : "Not created";


            AutoexecStateText.Foreground =
                exists
                    ? System.Windows.Media.Brushes.LightGreen
                    : System.Windows.Media.Brushes.Khaki;


            RawEditor.Text =
                autoexecService.ReadOrEmpty();


            LoadFov();


            LoadComboCommand(
                AbilityFovCombo,
                "fov_disableAbilityScaling");


            LoadTextCommand(
                FpsCapBox,
                "fps_max",
                "180");


            LoadTextCommand(
                LobbyFpsCapBox,
                "lobby_max_fps",
                "60");


            LoadComboCommand(
                RawInputCombo,
                "m_rawinput");


            SelectComboValue(
                MouseAccelerationCombo,
                autoexecService.GetCommandValue(
                    "m_acceleration"));


            LoadComboCommand(
                FilmGrainCombo,
                "noise_filter_scale");


            LoadComboCommand(
                AdsDofCombo,
                "hud_setting_adsDof");


            LoadComboCommand(
                ViewShakeCombo,
                "sprint_view_shake_style");


            LoadComboCommand(
                ModelDecalsCombo,
                "r_drawmodeldecals");


            LoadComboCommand(
                CheapWaterCombo,
                "r_forcecheapwater");


            CaptureLoadedState();
        }


        private void CaptureLoadedState()
        {
            loadedFov =
                GetComboValue(
                    FovCombo);

            loadedAbilityFov =
                GetComboValue(
                    AbilityFovCombo);

            loadedRawInput =
                GetComboValue(
                    RawInputCombo);

            loadedAcceleration =
                GetComboValue(
                    MouseAccelerationCombo);

            loadedFilmGrain =
                GetComboValue(
                    FilmGrainCombo);

            loadedAdsDof =
                GetComboValue(
                    AdsDofCombo);

            loadedViewShake =
                GetComboValue(
                    ViewShakeCombo);

            loadedModelDecals =
                GetComboValue(
                    ModelDecalsCombo);

            loadedCheapWater =
                GetComboValue(
                    CheapWaterCombo);

            loadedFps =
                FpsCapBox.Text.Trim();

            loadedLobbyFps =
                LobbyFpsCapBox.Text.Trim();
        }


        private void LoadFov()
        {
            if (autoexecService == null)
                return;


            string value =
                autoexecService.GetCommandValue(
                    "cl_fovScale");


            if (string.IsNullOrWhiteSpace(
                    value))
            {
                FovCombo.SelectedIndex = 0;

                return;
            }


            foreach (ComboBoxItem item
                     in FovCombo.Items)
            {
                string tag =
                    item.Tag?.ToString()
                    ?? "";


                if (AreFovValuesClose(
                        tag,
                        value))
                {
                    FovCombo.SelectedItem =
                        item;

                    return;
                }
            }


            FovCombo.SelectedIndex = 0;
        }


        private bool AreFovValuesClose(
            string first,
            string second)
        {
            if (!double.TryParse(
                    first,
                    System.Globalization
                        .NumberStyles.Float,
                    System.Globalization
                        .CultureInfo.InvariantCulture,
                    out double a))
            {
                return false;
            }


            if (!double.TryParse(
                    second,
                    System.Globalization
                        .NumberStyles.Float,
                    System.Globalization
                        .CultureInfo.InvariantCulture,
                    out double b))
            {
                return false;
            }


            return Math.Abs(a - b) < 0.02;
        }


        private void LoadTextCommand(
            TextBox textBox,
            string command,
            string fallback)
        {
            if (autoexecService == null)
                return;


            string value =
                autoexecService.GetCommandValue(
                    command);


            textBox.Text =
                string.IsNullOrWhiteSpace(
                    value)
                    ? fallback
                    : value;
        }


        private void LoadComboCommand(
            ComboBox combo,
            string command)
        {
            if (autoexecService == null)
                return;


            SelectComboValue(
                combo,
                autoexecService.GetCommandValue(
                    command));
        }


        private void SelectComboValue(
            ComboBox combo,
            string value)
        {
            if (string.IsNullOrWhiteSpace(
                    value))
            {
                combo.SelectedIndex = 0;

                return;
            }


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


        private string GetComboValue(
            ComboBox combo)
        {
            ComboBoxItem? item =
                combo.SelectedItem
                as ComboBoxItem;


            return item?.Tag?.ToString()
                   ?? Keep;
        }


        private void ApplyComboChange(
            Dictionary<string, string> commands,
            string command,
            ComboBox combo,
            string loadedValue)
        {
            string current =
                GetComboValue(
                    combo);


            if (current == loadedValue)
                return;


            if (current == Keep)
            {
                commands.Remove(
                    command);

                return;
            }


            commands[command] =
                current;
        }


        public bool TryBuildPendingContent(
            out string currentContent,
            out string pendingContent,
            out string error)
        {
            currentContent =
                "";

            pendingContent =
                "";

            error =
                "";


            if (autoexecService == null)
                return true;


            currentContent =
                autoexecService.ReadOrEmpty();


            Dictionary<string, string> commands =
                autoexecService.GetManagedCommands();


            ApplyComboChange(
                commands,
                "cl_fovScale",
                FovCombo,
                loadedFov);


            ApplyComboChange(
                commands,
                "fov_disableAbilityScaling",
                AbilityFovCombo,
                loadedAbilityFov);


            ApplyComboChange(
                commands,
                "m_rawinput",
                RawInputCombo,
                loadedRawInput);


            string currentAcceleration =
                GetComboValue(
                    MouseAccelerationCombo);


            if (currentAcceleration !=
                loadedAcceleration)
            {
                if (currentAcceleration == Keep)
                {
                    commands.Remove(
                        "m_acceleration");

                    commands.Remove(
                        "m_customaccel");
                }
                else
                {
                    commands["m_acceleration"] =
                        currentAcceleration;

                    commands["m_customaccel"] =
                        currentAcceleration;
                }
            }


            ApplyComboChange(
                commands,
                "noise_filter_scale",
                FilmGrainCombo,
                loadedFilmGrain);


            ApplyComboChange(
                commands,
                "hud_setting_adsDof",
                AdsDofCombo,
                loadedAdsDof);


            ApplyComboChange(
                commands,
                "sprint_view_shake_style",
                ViewShakeCombo,
                loadedViewShake);


            ApplyComboChange(
                commands,
                "r_drawmodeldecals",
                ModelDecalsCombo,
                loadedModelDecals);


            ApplyComboChange(
                commands,
                "r_forcecheapwater",
                CheapWaterCombo,
                loadedCheapWater);


            string fps =
                FpsCapBox.Text.Trim();


            if (fps != loadedFps)
            {
                if (string.IsNullOrWhiteSpace(
                        fps))
                {
                    commands.Remove(
                        "fps_max");
                }
                else
                {
                    if (!int.TryParse(
                            fps,
                            out int fpsValue)
                        ||
                        fpsValue < 30
                        ||
                        fpsValue > 1000)
                    {
                        error =
                            "FPS limit must be between 30 and 1000.";

                        return false;
                    }


                    commands["fps_max"] =
                        fpsValue.ToString();
                }
            }


            string lobbyFps =
                LobbyFpsCapBox.Text.Trim();


            if (lobbyFps !=
                loadedLobbyFps)
            {
                if (string.IsNullOrWhiteSpace(
                        lobbyFps))
                {
                    commands.Remove(
                        "lobby_max_fps");
                }
                else
                {
                    if (!int.TryParse(
                            lobbyFps,
                            out int lobbyValue)
                        ||
                        lobbyValue < 20
                        ||
                        lobbyValue > 300)
                    {
                        error =
                            "Lobby FPS limit must be between 20 and 300.";

                        return false;
                    }


                    commands["lobby_max_fps"] =
                        lobbyValue.ToString();
                }
            }


            pendingContent =
                autoexecService.BuildManagedContent(
                    currentContent,
                    commands);


            return true;
        }


        private bool CanWriteRaw()
        {
            if (autoexecService == null ||
                installation == null ||
                !installation.GameFound)
            {
                StatusChanged?.Invoke(
                    "Apex installation was not detected.");

                return false;
            }


            if (processService.IsApexRunning())
            {
                StatusChanged?.Invoke(
                    "Close Apex before editing autoexec.");

                return false;
            }


            return true;
        }


        private void SaveRawButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (!CanWriteRaw())
                    return;


                autoexecService!
                    .WriteAll(
                        RawEditor.Text);


                ReloadFromDisk();


                StatusChanged?.Invoke(
                    "Autoexec saved.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Autoexec save error: " +
                    ex.Message);
            }
        }


        private void ReloadButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ReloadFromDisk();


            StatusChanged?.Invoke(
                "Autoexec reloaded.");
        }


        private void BackupButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (autoexecService == null)
                {
                    StatusChanged?.Invoke(
                        "Autoexec is unavailable.");

                    return;
                }


                string? backup =
                    autoexecService.CreateBackup();


                if (backup == null)
                {
                    StatusChanged?.Invoke(
                        "No autoexec file exists yet.");

                    return;
                }


                StatusChanged?.Invoke(
                    "Autoexec backup created.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Autoexec backup error: " +
                    ex.Message);
            }
        }


        private void CopyLaunchOptionsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                    LaunchOptionsBox.Text))
            {
                return;
            }


            Clipboard.SetText(
                LaunchOptionsBox.Text);


            StatusChanged?.Invoke(
                "Launch options copied.");
        }


        private void OpenCfgFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (installation == null ||
                    !installation.GameFound)
                {
                    StatusChanged?.Invoke(
                        "Apex installation was not detected.");

                    return;
                }


                Directory.CreateDirectory(
                    installation.CfgDirectory);


                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            "explorer.exe",

                        Arguments =
                            "\"" +
                            installation.CfgDirectory +
                            "\"",

                        UseShellExecute =
                            true
                    });
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Could not open folder: " +
                    ex.Message);
            }
        }
    }
}