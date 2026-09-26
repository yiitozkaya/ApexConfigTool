using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using ApexConfigTool.Models;

namespace ApexConfigTool.Services
{
    public class ApexDetector
    {
        public ApexInstallation Detect()
        {
            ApexInstallation installation =
                new ApexInstallation();


            DetectUserConfig(
                installation);


            var game =
                FindApexInstallation();


            if (game != null)
            {
                installation.GameFound =
                    true;


                installation.GameDirectory =
                    game.Value.Path;


                installation.Launcher =
                    game.Value.Launcher;


                string r5 =
                    Path.Combine(
                        game.Value.Path,
                        "r5apex.exe");


                string protectedGame =
                    Path.Combine(
                        game.Value.Path,
                        "start_protected_game.exe");


                if (File.Exists(r5))
                {
                    installation.ExecutablePath =
                        r5;
                }
                else if (File.Exists(protectedGame))
                {
                    installation.ExecutablePath =
                        protectedGame;
                }


                installation.CfgDirectory =
                    Path.Combine(
                        game.Value.Path,
                        "cfg");


                installation.AutoexecPath =
                    Path.Combine(
                        installation.CfgDirectory,
                        "autoexec.cfg");
            }


            return installation;
        }


        private void DetectUserConfig(
            ApexInstallation installation)
        {
            string userProfile =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.UserProfile);


            string configDirectory =
                Path.Combine(
                    userProfile,
                    "Saved Games",
                    "Respawn",
                    "Apex",
                    "local");


            string videoConfigPath =
                Path.Combine(
                    configDirectory,
                    "videoconfig.txt");


            installation.ConfigDirectory =
                configDirectory;


            installation.VideoConfigPath =
                videoConfigPath;


            installation.ConfigFound =
                File.Exists(videoConfigPath);
        }


        private (string Path, string Launcher)?
            FindApexInstallation()
        {
            string? steam =
                FindSteamApex();


            if (!string.IsNullOrWhiteSpace(
                steam))
            {
                return (
                    steam,
                    "Steam");
            }


            string? ea =
                FindEaApex();


            if (!string.IsNullOrWhiteSpace(
                ea))
            {
                return (
                    ea,
                    "EA App");
            }


            return null;
        }


        private string? FindSteamApex()
        {
            string? steamRoot =
                GetSteamRoot();


            if (string.IsNullOrWhiteSpace(
                steamRoot))
            {
                return null;
            }


            HashSet<string> libraries =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);


            libraries.Add(
                steamRoot);


            string libraryFile =
                Path.Combine(
                    steamRoot,
                    "steamapps",
                    "libraryfolders.vdf");


            if (File.Exists(libraryFile))
            {
                try
                {
                    string content =
                        File.ReadAllText(
                            libraryFile);


                    MatchCollection matches =
                        Regex.Matches(
                            content,
                            "\"path\"\\s+\"([^\"]+)\"",
                            RegexOptions.IgnoreCase);


                    foreach (Match match
                             in matches)
                    {
                        string path =
                            match.Groups[1]
                                .Value
                                .Replace(
                                    "\\\\",
                                    "\\");


                        if (!string.IsNullOrWhiteSpace(
                            path))
                        {
                            libraries.Add(
                                path);
                        }
                    }
                }
                catch
                {
                }
            }


            foreach (string library
                     in libraries)
            {
                string candidate =
                    Path.Combine(
                        library,
                        "steamapps",
                        "common",
                        "Apex Legends");


                if (IsApexDirectory(
                    candidate))
                {
                    return candidate;
                }
            }


            return null;
        }


        private string? GetSteamRoot()
        {
            string? registryPath =
                ReadRegistryString(
                    Registry.CurrentUser,
                    @"Software\Valve\Steam",
                    "SteamPath");


            if (!string.IsNullOrWhiteSpace(
                registryPath) &&
                Directory.Exists(
                    registryPath))
            {
                return registryPath;
            }


            registryPath =
                ReadRegistryString(
                    Registry.LocalMachine,
                    @"SOFTWARE\WOW6432Node\Valve\Steam",
                    "InstallPath");


            if (!string.IsNullOrWhiteSpace(
                registryPath) &&
                Directory.Exists(
                    registryPath))
            {
                return registryPath;
            }


            string fallback =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder
                            .ProgramFilesX86),
                    "Steam");


            if (Directory.Exists(
                fallback))
            {
                return fallback;
            }


            return null;
        }


        private string? FindEaApex()
        {
            string? registryInstall =
                FindApexFromUninstallRegistry();


            if (!string.IsNullOrWhiteSpace(
                registryInstall) &&
                IsApexDirectory(
                    registryInstall))
            {
                return registryInstall;
            }


            List<string> candidates =
                new List<string>();


            string programFiles =
                Environment.GetFolderPath(
                    Environment.SpecialFolder
                        .ProgramFiles);


            string programFilesX86 =
                Environment.GetFolderPath(
                    Environment.SpecialFolder
                        .ProgramFilesX86);


            candidates.Add(
                Path.Combine(
                    programFiles,
                    "EA Games",
                    "Apex"));


            candidates.Add(
                Path.Combine(
                    programFiles,
                    "EA Games",
                    "Apex Legends"));


            candidates.Add(
                Path.Combine(
                    programFilesX86,
                    "Origin Games",
                    "Apex"));


            foreach (DriveInfo drive
                     in DriveInfo.GetDrives())
            {
                try
                {
                    if (!drive.IsReady)
                        continue;


                    if (drive.DriveType !=
                        DriveType.Fixed)
                    {
                        continue;
                    }


                    candidates.Add(
                        Path.Combine(
                            drive.RootDirectory
                                .FullName,
                            "EA Games",
                            "Apex"));


                    candidates.Add(
                        Path.Combine(
                            drive.RootDirectory
                                .FullName,
                            "EA Games",
                            "Apex Legends"));


                    candidates.Add(
                        Path.Combine(
                            drive.RootDirectory
                                .FullName,
                            "Origin Games",
                            "Apex"));
                }
                catch
                {
                }
            }


            foreach (string candidate
                     in candidates)
            {
                if (IsApexDirectory(
                    candidate))
                {
                    return candidate;
                }
            }


            return null;
        }


        private string?
            FindApexFromUninstallRegistry()
        {
            RegistryHive[] hives =
            {
                RegistryHive.LocalMachine,
                RegistryHive.CurrentUser
            };


            RegistryView[] views =
            {
                RegistryView.Registry64,
                RegistryView.Registry32
            };


            foreach (RegistryHive hive
                     in hives)
            {
                foreach (RegistryView view
                         in views)
                {
                    try
                    {
                        using RegistryKey baseKey =
                            RegistryKey.OpenBaseKey(
                                hive,
                                view);


                        using RegistryKey? uninstall =
                            baseKey.OpenSubKey(
                                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");


                        if (uninstall == null)
                            continue;


                        foreach (string subName
                                 in uninstall
                                     .GetSubKeyNames())
                        {
                            using RegistryKey? subKey =
                                uninstall.OpenSubKey(
                                    subName);


                            if (subKey == null)
                                continue;


                            string displayName =
                                subKey.GetValue(
                                    "DisplayName")
                                ?.ToString()
                                ?? "";


                            if (!displayName.Contains(
                                    "Apex Legends",
                                    StringComparison
                                        .OrdinalIgnoreCase))
                            {
                                continue;
                            }


                            string installLocation =
                                subKey.GetValue(
                                    "InstallLocation")
                                ?.ToString()
                                ?? "";


                            if (!string.IsNullOrWhiteSpace(
                                installLocation))
                            {
                                return installLocation
                                    .Trim('"');
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }


            return null;
        }


        private bool IsApexDirectory(
            string path)
        {
            if (!Directory.Exists(
                path))
            {
                return false;
            }


            return
                File.Exists(
                    Path.Combine(
                        path,
                        "r5apex.exe"))
                ||
                File.Exists(
                    Path.Combine(
                        path,
                        "start_protected_game.exe"));
        }


        private string? ReadRegistryString(
            RegistryKey root,
            string keyPath,
            string valueName)
        {
            try
            {
                using RegistryKey? key =
                    root.OpenSubKey(
                        keyPath);


                return key?
                    .GetValue(
                        valueName)
                    ?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}