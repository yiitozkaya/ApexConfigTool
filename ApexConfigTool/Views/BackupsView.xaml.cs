using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using ApexConfigTool.Models;
using ApexConfigTool.Services;

namespace ApexConfigTool.Views
{
    public partial class BackupsView : UserControl
    {
        private BackupService?
            videoBackupService;


        private AutoexecService?
            autoexecService;


        private readonly ProcessService
            processService =
                new();


        public event Action<string>?
            StatusChanged;


        public event Action?
            BackupRestored;


        public BackupsView()
        {
            InitializeComponent();
        }


        public void Configure(
            BackupService? videoService,
            AutoexecService? autoexec)
        {
            videoBackupService =
                videoService;


            autoexecService =
                autoexec;


            RefreshLists();
        }


        public void RefreshLists()
        {
            VideoBackupList.ItemsSource =
                videoBackupService?
                    .GetBackups();


            AutoexecBackupList.ItemsSource =
                autoexecService?
                    .GetBackups();
        }


        private bool CheckApexClosed()
        {
            if (!processService.IsApexRunning())
                return true;


            StatusChanged?.Invoke(
                "Close Apex Legends before restoring a backup.");


            return false;
        }


        private void RestoreVideoButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (!CheckApexClosed())
                    return;


                if (videoBackupService == null)
                {
                    StatusChanged?.Invoke(
                        "VideoConfig backup service unavailable.");

                    return;
                }


                BackupEntry? selected =
                    VideoBackupList.SelectedItem
                    as BackupEntry;


                if (selected == null)
                {
                    StatusChanged?.Invoke(
                        "Select a videoconfig backup first.");

                    return;
                }


                MessageBoxResult result =
                    MessageBox.Show(
                        "Restore this videoconfig backup?",
                        "Restore Backup",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);


                if (result !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                videoBackupService
                    .RestoreBackup(
                        selected.FullPath);


                RefreshLists();


                BackupRestored?.Invoke();


                StatusChanged?.Invoke(
                    "Selected videoconfig backup restored.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Restore error: " +
                    ex.Message);
            }
        }


        private void DeleteVideoButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (videoBackupService == null)
                    return;


                BackupEntry? selected =
                    VideoBackupList.SelectedItem
                    as BackupEntry;


                if (selected == null)
                {
                    StatusChanged?.Invoke(
                        "Select a videoconfig backup first.");

                    return;
                }


                MessageBoxResult result =
                    MessageBox.Show(
                        "Delete this backup permanently?",
                        "Delete Backup",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);


                if (result !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                videoBackupService.DeleteBackup(
                    selected.FullPath);


                RefreshLists();


                StatusChanged?.Invoke(
                    "Videoconfig backup deleted.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Delete error: " +
                    ex.Message);
            }
        }


        private void RestoreAutoexecButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (!CheckApexClosed())
                    return;


                if (autoexecService == null)
                {
                    StatusChanged?.Invoke(
                        "Autoexec backup service unavailable.");

                    return;
                }


                BackupEntry? selected =
                    AutoexecBackupList.SelectedItem
                    as BackupEntry;


                if (selected == null)
                {
                    StatusChanged?.Invoke(
                        "Select an autoexec backup first.");

                    return;
                }


                MessageBoxResult result =
                    MessageBox.Show(
                        "Restore this autoexec backup?",
                        "Restore Backup",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);


                if (result !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                autoexecService.RestoreBackup(
                    selected.FullPath);


                RefreshLists();


                BackupRestored?.Invoke();


                StatusChanged?.Invoke(
                    "Selected autoexec backup restored.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Restore error: " +
                    ex.Message);
            }
        }


        private void DeleteAutoexecButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (autoexecService == null)
                    return;


                BackupEntry? selected =
                    AutoexecBackupList.SelectedItem
                    as BackupEntry;


                if (selected == null)
                {
                    StatusChanged?.Invoke(
                        "Select an autoexec backup first.");

                    return;
                }


                MessageBoxResult result =
                    MessageBox.Show(
                        "Delete this backup permanently?",
                        "Delete Backup",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);


                if (result !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                autoexecService.DeleteBackup(
                    selected.FullPath);


                RefreshLists();


                StatusChanged?.Invoke(
                    "Autoexec backup deleted.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Delete error: " +
                    ex.Message);
            }
        }


        private void OpenVideoFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (videoBackupService == null)
                return;


            OpenFolder(
                videoBackupService.BackupDirectory);
        }


        private void OpenAutoexecFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (autoexecService == null)
                return;


            OpenFolder(
                autoexecService.BackupDirectory);
        }


        private void OpenFolder(
            string path)
        {
            try
            {
                Directory.CreateDirectory(
                    path);


                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            "explorer.exe",

                        Arguments =
                            "\"" + path + "\"",

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


        private void RefreshButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            RefreshLists();


            StatusChanged?.Invoke(
                "Backup list refreshed.");
        }
    }
}