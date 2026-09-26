using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ApexConfigTool.Models;
using ApexConfigTool.Services;

namespace ApexConfigTool.Views
{
    public partial class AdvancedView : UserControl
    {
        private VideoConfigService?
            videoConfig;


        private BackupService?
            backupService;


        private readonly ProcessService
            processService =
                new();


        private List<ConfigEntry>
            allSettings =
                new List<ConfigEntry>();


        public event Action<string>?
            StatusChanged;


        public event Action?
            ConfigSaved;


        public AdvancedView()
        {
            InitializeComponent();
        }


        public void Configure(
            VideoConfigService? videoService,
            BackupService? backups)
        {
            videoConfig =
                videoService;


            backupService =
                backups;


            ReloadEverything();
        }


        public void ReloadEverything()
        {
            if (videoConfig == null ||
                !videoConfig.Exists())
            {
                RawEditor.Text =
                    "";


                SettingsGrid.ItemsSource =
                    null;


                SettingCountText.Text =
                    "0 settings";


                return;
            }


            string content =
                videoConfig.Read();


            RawEditor.Text =
                content;


            allSettings =
                videoConfig.GetAllSettings(
                    content);


            ApplyFilter();
        }


        private void ApplyFilter()
        {
            string search =
                SearchBox.Text.Trim();


            List<ConfigEntry> result;


            if (string.IsNullOrWhiteSpace(
                search))
            {
                result =
                    allSettings;
            }
            else
            {
                result =
                    allSettings
                        .Where(
                            x =>
                                x.Setting.Contains(
                                    search,
                                    StringComparison
                                        .OrdinalIgnoreCase)
                                ||
                                x.Value.Contains(
                                    search,
                                    StringComparison
                                        .OrdinalIgnoreCase))
                        .ToList();
            }


            SettingsGrid.ItemsSource =
                result;


            SettingCountText.Text =
                $"{result.Count} settings";
        }


        private void SearchBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            ApplyFilter();
        }


        private void RefreshButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ReloadEverything();


            StatusChanged?.Invoke(
                "Advanced config inspector refreshed.");
        }


        private void ReloadRawButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ReloadEverything();


            StatusChanged?.Invoke(
                "videoconfig.txt reloaded from disk.");
        }


        private void SaveRawButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (videoConfig == null ||
                    backupService == null)
                {
                    StatusChanged?.Invoke(
                        "Video config is not loaded.");

                    return;
                }


                if (processService.IsApexRunning())
                {
                    StatusChanged?.Invoke(
                        "Close Apex before saving raw config.");

                    return;
                }


                MessageBoxResult result =
                    MessageBox.Show(
                        "Save the complete raw videoconfig.txt?\n\n" +
                        "A backup will be created first.",
                        "Save Raw Config",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);


                if (result !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                backupService.CreateBackup();


                videoConfig.RemoveReadOnly();


                videoConfig.Write(
                    RawEditor.Text);


                ReloadEverything();


                ConfigSaved?.Invoke();


                StatusChanged?.Invoke(
                    "Raw videoconfig.txt saved successfully.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Raw config save error: " +
                    ex.Message);
            }
        }
    }
}