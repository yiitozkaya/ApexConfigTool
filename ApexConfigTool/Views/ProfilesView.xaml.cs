using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using ApexConfigTool.Models;
using ApexConfigTool.Services;

namespace ApexConfigTool.Views
{
    public partial class ProfilesView : UserControl
    {
        private ProfileService?
            profileService;


        public event Action<string>?
            SaveProfileRequested;


        public event Action<ProfileData>?
            LoadProfileRequested;


        public event Action<string>?
            StatusChanged;


        public ProfilesView()
        {
            InitializeComponent();
        }


        public void Configure(
            ProfileService service)
        {
            profileService =
                service;


            RefreshProfiles();
        }


        public void RefreshProfiles()
        {
            if (profileService == null)
                return;


            ProfileList.ItemsSource =
                profileService.GetProfiles();
        }


        private void SaveProfileButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string name =
                ProfileNameBox.Text.Trim();


            if (string.IsNullOrWhiteSpace(
                name))
            {
                StatusChanged?.Invoke(
                    "Enter a profile name first.");

                return;
            }


            SaveProfileRequested?.Invoke(
                name);
        }


        private void LoadProfileButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ProfileData? selected =
                ProfileList.SelectedItem
                as ProfileData;


            if (selected == null)
            {
                StatusChanged?.Invoke(
                    "Select a profile first.");

                return;
            }


            MessageBoxResult result =
                MessageBox.Show(
                    $"Load profile \"{selected.Name}\"?\n\n" +
                    "Your current config will be backed up first.",
                    "Load Profile",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (result !=
                MessageBoxResult.Yes)
            {
                return;
            }


            LoadProfileRequested?.Invoke(
                selected);
        }


        private void DeleteProfileButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (profileService == null)
                    return;


                ProfileData? selected =
                    ProfileList.SelectedItem
                    as ProfileData;


                if (selected == null)
                {
                    StatusChanged?.Invoke(
                        "Select a profile first.");

                    return;
                }


                MessageBoxResult result =
                    MessageBox.Show(
                        $"Delete profile \"{selected.Name}\"?",
                        "Delete Profile",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);


                if (result !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                profileService.DeleteProfile(
                    selected);


                RefreshProfiles();


                StatusChanged?.Invoke(
                    "Profile deleted.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Profile delete error: " +
                    ex.Message);
            }
        }


        private void RefreshButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            RefreshProfiles();


            StatusChanged?.Invoke(
                "Profiles refreshed.");
        }


        private void OpenProfilesFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                if (profileService == null)
                    return;


                Directory.CreateDirectory(
                    profileService
                        .ProfilesDirectory);


                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            "explorer.exe",

                        Arguments =
                            "\"" +
                            profileService.ProfilesDirectory +
                            "\"",

                        UseShellExecute =
                            true
                    });
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(
                    "Could not open profiles folder: " +
                    ex.Message);
            }
        }
    }
}