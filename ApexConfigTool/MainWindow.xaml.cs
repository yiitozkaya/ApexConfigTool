using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;

using ApexConfigTool.Models;
using ApexConfigTool.Services;
using ApexConfigTool.Views;

namespace ApexConfigTool
{
    public partial class MainWindow : Window
    {
        private readonly ApexDetector
            apexDetector =
                new();


        private readonly ProcessService
            processService =
                new();


        private readonly ProfileService
            profileService =
                new();


        private readonly ConfigDiffService
            diffService =
                new();


        private readonly AutoexecDiffService
            autoexecDiffService =
                new();


        private ApexInstallation?
            installation;


        private VideoConfigService?
            videoConfig;


        private BackupService?
            backupService;


        private AutoexecService?
            autoexecService;


        private readonly DashboardView
            dashboardView =
                new();


        private readonly DisplayView
            displayView =
                new();


        private readonly GraphicsView
            graphicsView =
                new();


        private readonly ShadowsView
            shadowsView =
                new();


        private readonly PerformanceView
            performanceView =
                new();


        private readonly AutoexecView
            autoexecView =
                new();


        private readonly ProfilesView
            profilesView =
                new();


        private readonly BackupsView
            backupsView =
                new();


        private readonly AdvancedView
            advancedView =
                new();


        public MainWindow()
        {
            InitializeComponent();


            performanceView.PresetRequested +=
                PerformanceView_PresetRequested;


            autoexecView.StatusChanged +=
                ChildView_StatusChanged;


            profilesView.StatusChanged +=
                ChildView_StatusChanged;


            profilesView.SaveProfileRequested +=
                ProfilesView_SaveProfileRequested;


            profilesView.LoadProfileRequested +=
                ProfilesView_LoadProfileRequested;


            backupsView.StatusChanged +=
                ChildView_StatusChanged;


            backupsView.BackupRestored +=
                BackupsView_BackupRestored;


            advancedView.StatusChanged +=
                ChildView_StatusChanged;


            advancedView.ConfigSaved +=
                AdvancedView_ConfigSaved;


            profilesView.Configure(
                profileService);


            Loaded +=
                MainWindow_Loaded;
        }


        private void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            DetectConfig();

            ShowDashboard();
        }


        private void DetectConfig()
        {
            try
            {
                installation =
                    apexDetector.Detect();


                autoexecView.LoadInstallation(
                    installation);


                autoexecService =
                    installation.GameFound
                        ? new AutoexecService(
                            installation.AutoexecPath)
                        : null;


                if (!installation.ConfigFound)
                {
                    videoConfig = null;

                    backupService = null;


                    backupsView.Configure(
                        null,
                        autoexecService);


                    advancedView.Configure(
                        null,
                        null);


                    StatusText.Text =
                        "Apex videoconfig could not be found.";


                    UpdateProcessStatus();

                    return;
                }


                videoConfig =
                    new VideoConfigService(
                        installation.VideoConfigPath);


                backupService =
                    new BackupService(
                        installation.VideoConfigPath);


                backupsView.Configure(
                    backupService,
                    autoexecService);


                advancedView.Configure(
                    videoConfig,
                    backupService);


                RefreshAll();


                StatusText.Text =
                    installation.GameFound
                        ? $"{installation.Launcher} installation detected."
                        : "Apex config loaded.";
            }
            catch (Exception ex)
            {
                StatusText.Text =
                    "Detection error: " +
                    ex.Message;
            }
        }


        private void RefreshAll()
        {
            if (videoConfig == null ||
                installation == null)
            {
                return;
            }


            var resolution =
                videoConfig.GetResolution();


            if (resolution == null)
            {
                StatusText.Text =
                    "Could not read Apex resolution.";

                return;
            }


            string config =
                videoConfig.Read();


            bool fullscreen =
                videoConfig.GetSetting(
                    config,
                    "setting.fullscreen")
                == "1";


            bool readOnly =
                videoConfig.IsReadOnly();


            bool apexRunning =
                processService.IsApexRunning();


            displayView.LoadSettings(
                resolution.Value.Width,
                resolution.Value.Height,
                fullscreen,
                readOnly);


            graphicsView.LoadSettings(
                videoConfig,
                config);


            shadowsView.LoadSettings(
                videoConfig,
                config);


            string fov =
                GetCurrentFov();


            string fps =
                autoexecService?
                    .GetCommandValue(
                        "fps_max")
                ?? "";


            dashboardView.UpdateData(
                installation.VideoConfigPath,
                resolution.Value.Width,
                resolution.Value.Height,
                readOnly,
                apexRunning,
                installation.Launcher,
                fov,
                fps);


            advancedView.ReloadEverything();


            UpdateProcessStatus();
        }


        private string GetCurrentFov()
        {
            if (autoexecService == null)
                return "Default";


            string value =
                autoexecService.GetCommandValue(
                    "cl_fovScale");


            if (!double.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double scale))
            {
                return "Default";
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


            return value;
        }


        private void UpdateProcessStatus()
        {
            ApexProcessText.Text =
                processService.IsApexRunning()
                    ? "● Apex running"
                    : "● Apex off";
        }


        private string? BuildPendingVideoConfig()
        {
            if (videoConfig == null)
            {
                StatusText.Text =
                    "Apex config is not loaded.";

                return null;
            }


            DisplaySettings? settings =
                displayView.GetSettings();


            if (settings == null)
            {
                StatusText.Text =
                    "Invalid display configuration.";

                return null;
            }


            string config =
                videoConfig.Read();


            config =
                videoConfig.SetResolution(
                    config,
                    settings.Width,
                    settings.Height);


            config =
                videoConfig.SetFullscreen(
                    config,
                    settings.Fullscreen);


            config =
                shadowsView.ApplyToConfig(
                    videoConfig,
                    config);


            config =
                graphicsView.ApplyToConfig(
                    videoConfig,
                    config);


            return config;
        }


        private bool ShowPreviewAndApply(
            bool preferLaunch)
        {
            if (videoConfig == null ||
                backupService == null)
            {
                StatusText.Text =
                    "Apex config not found.";

                return false;
            }


            if (processService.IsApexRunning())
            {
                StatusText.Text =
                    "Close Apex before applying changes.";

                return false;
            }


            string? pendingVideo =
                BuildPendingVideoConfig();


            if (pendingVideo == null)
                return false;


            string currentVideo =
                videoConfig.Read();


            List<ConfigChange> videoChanges =
                diffService.Compare(
                    videoConfig,
                    currentVideo,
                    pendingVideo);


            string currentAutoexec =
                "";

            string pendingAutoexec =
                "";


            if (!autoexecView
                .TryBuildPendingContent(
                    out currentAutoexec,
                    out pendingAutoexec,
                    out string autoexecError))
            {
                StatusText.Text =
                    autoexecError;

                return false;
            }


            List<ConfigChange> autoexecChanges =
                new List<ConfigChange>();


            if (autoexecService != null)
            {
                autoexecChanges =
                    autoexecDiffService.Compare(
                        autoexecService,
                        currentAutoexec,
                        pendingAutoexec);
            }


            List<ConfigChange> allChanges =
                new List<ConfigChange>();


            allChanges.AddRange(
                videoChanges);


            allChanges.AddRange(
                autoexecChanges);


            if (allChanges.Count == 0)
            {
                if (preferLaunch)
                {
                    LaunchApex();

                    return true;
                }


                StatusText.Text =
                    "No changes are pending.";

                return false;
            }


            PreviewWindow preview =
                new PreviewWindow(
                    allChanges);


            preview.Owner =
                this;


            bool? result =
                preview.ShowDialog();


            if (result != true ||
                !preview.ApplyChanges)
            {
                StatusText.Text =
                    "Apply cancelled.";

                return false;
            }


            bool launchAfter =
                preferLaunch ||
                preview.LaunchAfterApply;


            WritePendingChanges(
                pendingVideo,
                videoChanges.Count > 0,
                pendingAutoexec,
                autoexecChanges.Count > 0);


            if (launchAfter)
            {
                LaunchApex();
            }


            return true;
        }


        private void WritePendingChanges(
            string pendingVideo,
            bool videoChanged,
            string pendingAutoexec,
            bool autoexecChanged)
        {
            if (videoConfig == null ||
                backupService == null)
            {
                return;
            }


            DisplaySettings? settings =
                displayView.GetSettings();


            if (settings == null)
                return;


            if (videoChanged)
            {
                backupService.CreateBackup();


                videoConfig.RemoveReadOnly();


                videoConfig.Write(
                    pendingVideo);


                if (settings.ReadOnly)
                {
                    videoConfig.MakeReadOnly();
                }
                else
                {
                    videoConfig.RemoveReadOnly();
                }
            }


            if (autoexecChanged &&
                autoexecService != null)
            {
                autoexecService.WriteAll(
                    pendingAutoexec);
            }


            if (installation != null)
            {
                autoexecView.LoadInstallation(
                    installation);
            }


            RefreshAll();


            backupsView.RefreshLists();


            int changedFiles =
                (videoChanged ? 1 : 0) +
                (autoexecChanged ? 1 : 0);


            StatusText.Text =
                changedFiles == 2
                    ? "Video config and autoexec applied."
                    : videoChanged
                        ? "Video config applied."
                        : "Autoexec applied.";
        }


        private void LaunchApex()
        {
            try
            {
                if (installation == null)
                {
                    StatusText.Text =
                        "Apex installation is unavailable.";

                    return;
                }


                processService.LaunchApex(
                    installation);


                StatusText.Text =
                    "Launching Apex...";


                UpdateProcessStatus();
            }
            catch (Exception ex)
            {
                StatusText.Text =
                    "Launch error: " +
                    ex.Message;
            }
        }


        private void ShowDashboard()
        {
            MainContent.Content =
                dashboardView;
        }


        private void DashboardButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            RefreshAll();

            ShowDashboard();
        }


        private void DisplayButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainContent.Content =
                displayView;
        }


        private void GraphicsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainContent.Content =
                graphicsView;
        }


        private void ShadowsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainContent.Content =
                shadowsView;
        }


        private void PerformanceButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainContent.Content =
                performanceView;
        }


        private void AutoexecButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainContent.Content =
                autoexecView;
        }


        private void ProfilesButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            profilesView.RefreshProfiles();


            MainContent.Content =
                profilesView;
        }


        private void BackupsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            backupsView.RefreshLists();


            MainContent.Content =
                backupsView;
        }


        private void AdvancedButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            advancedView.ReloadEverything();


            MainContent.Content =
                advancedView;
        }


        private void ChildView_StatusChanged(
            string message)
        {
            StatusText.Text =
                message;
        }


        private void AdvancedView_ConfigSaved()
        {
            RefreshAll();

            backupsView.RefreshLists();
        }


        private void PerformanceView_PresetRequested(
            string preset)
        {
            graphicsView.ApplyPreset(
                preset);


            shadowsView.ApplyPreset(
                preset);


            StatusText.Text =
                $"{preset} preset loaded.";
        }


        private void ProfilesView_SaveProfileRequested(
            string profileName)
        {
            try
            {
                if (videoConfig == null)
                    return;


                string? autoexecContent =
                    null;


                if (autoexecService != null &&
                    autoexecService.Exists())
                {
                    autoexecContent =
                        autoexecService.ReadOrEmpty();
                }


                profileService.SaveProfile(
                    profileName,
                    videoConfig.Read(),
                    videoConfig.IsReadOnly(),
                    autoexecContent);


                profilesView.RefreshProfiles();


                StatusText.Text =
                    $"Profile \"{profileName}\" saved.";
            }
            catch (Exception ex)
            {
                StatusText.Text =
                    "Profile save error: " +
                    ex.Message;
            }
        }


        private void ProfilesView_LoadProfileRequested(
            ProfileData profile)
        {
            try
            {
                if (videoConfig == null ||
                    backupService == null)
                {
                    return;
                }


                if (processService.IsApexRunning())
                {
                    StatusText.Text =
                        "Close Apex before loading a profile.";

                    return;
                }


                backupService.CreateBackup();


                videoConfig.RemoveReadOnly();


                videoConfig.Write(
                    profile.VideoConfigContent);


                if (profile.VideoConfigReadOnly)
                {
                    videoConfig.MakeReadOnly();
                }


                if (profile.AutoexecContent != null &&
                    autoexecService != null)
                {
                    autoexecService.WriteAll(
                        profile.AutoexecContent);
                }


                if (installation != null)
                {
                    autoexecView.LoadInstallation(
                        installation);
                }


                RefreshAll();


                backupsView.RefreshLists();


                StatusText.Text =
                    $"Profile \"{profile.Name}\" loaded.";
            }
            catch (Exception ex)
            {
                StatusText.Text =
                    "Profile load error: " +
                    ex.Message;
            }
        }


        private void BackupsView_BackupRestored()
        {
            if (installation != null)
            {
                autoexecView.LoadInstallation(
                    installation);
            }


            RefreshAll();
        }


        private void BackupButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (backupService == null)
                    return;


                backupService.CreateBackup();


                if (autoexecService != null &&
                    autoexecService.Exists())
                {
                    autoexecService.CreateBackup();
                }


                backupsView.RefreshLists();


                StatusText.Text =
                    "Backup created.";
            }
            catch (Exception ex)
            {
                StatusText.Text =
                    "Backup error: " +
                    ex.Message;
            }
        }


        private void ApplyButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPreviewAndApply(
                false);
        }


        private void ApplyLaunchButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowPreviewAndApply(
                true);
        }


        private void RestoreButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (backupService == null)
                    return;


                if (processService.IsApexRunning())
                {
                    StatusText.Text =
                        "Close Apex before restoring.";

                    return;
                }


                bool restored =
                    backupService.RestoreLatest();


                if (!restored)
                {
                    StatusText.Text =
                        "No backup found.";

                    return;
                }


                RefreshAll();


                backupsView.RefreshLists();


                StatusText.Text =
                    "Latest video config restored.";
            }
            catch (Exception ex)
            {
                StatusText.Text =
                    "Restore error: " +
                    ex.Message;
            }
        }
    }
}