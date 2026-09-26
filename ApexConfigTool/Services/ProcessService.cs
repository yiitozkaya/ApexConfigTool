using System;
using System.Diagnostics;
using System.IO;

using ApexConfigTool.Models;

namespace ApexConfigTool.Services
{
    public class ProcessService
    {
        public bool IsApexRunning()
        {
            return
                Process.GetProcessesByName(
                    "r5apex").Length > 0;
        }


        public void LaunchApex(
            ApexInstallation installation)
        {
            if (IsApexRunning())
            {
                return;
            }


            if (installation.Launcher ==
                "Steam")
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            "steam://rungameid/1172470",

                        UseShellExecute =
                            true
                    });


                return;
            }


            if (!installation.GameFound)
            {
                throw new Exception(
                    "Apex installation was not detected.");
            }


            string protectedExecutable =
                Path.Combine(
                    installation.GameDirectory,
                    "start_protected_game.exe");


            string r5Executable =
                Path.Combine(
                    installation.GameDirectory,
                    "r5apex.exe");


            string executable;


            if (File.Exists(
                protectedExecutable))
            {
                executable =
                    protectedExecutable;
            }
            else if (File.Exists(
                r5Executable))
            {
                executable =
                    r5Executable;
            }
            else
            {
                throw new FileNotFoundException(
                    "Could not find Apex executable.");
            }


            Process.Start(
                new ProcessStartInfo
                {
                    FileName =
                        executable,

                    WorkingDirectory =
                        installation.GameDirectory,

                    UseShellExecute =
                        true
                });
        }
    }
}