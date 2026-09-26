using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ApexConfigTool.Models;

namespace ApexConfigTool.Services
{
    public class BackupService
    {
        private readonly string configPath;

        private readonly string backupDirectory;


        public string BackupDirectory =>
            backupDirectory;


        public BackupService(
            string configPath)
        {
            this.configPath =
                configPath;


            string configDirectory =
                Path.GetDirectoryName(
                    configPath)
                ?? throw new Exception(
                    "Invalid config path.");


            backupDirectory =
                Path.Combine(
                    configDirectory,
                    "ApexConfigTool_Backups");
        }


        public string CreateBackup()
        {
            if (!File.Exists(
                configPath))
            {
                throw new FileNotFoundException(
                    "videoconfig.txt not found.");
            }


            Directory.CreateDirectory(
                backupDirectory);


            string filename =
                "videoconfig_" +
                DateTime.Now.ToString(
                    "yyyy-MM-dd_HH-mm-ss-fff") +
                ".txt";


            string destination =
                Path.Combine(
                    backupDirectory,
                    filename);


            File.Copy(
                configPath,
                destination,
                true);


            return destination;
        }


        public List<BackupEntry> GetBackups()
        {
            if (!Directory.Exists(
                backupDirectory))
            {
                return new List<BackupEntry>();
            }


            return Directory
                .GetFiles(
                    backupDirectory,
                    "videoconfig_*.txt")
                .Select(
                    path =>
                    {
                        FileInfo info =
                            new FileInfo(path);


                        return new BackupEntry
                        {
                            FullPath =
                                path,

                            FileName =
                                info.Name,

                            Type =
                                "VideoConfig",

                            CreatedAt =
                                info.CreationTime,

                            SizeBytes =
                                info.Length
                        };
                    })
                .OrderByDescending(
                    x => x.CreatedAt)
                .ToList();
        }


        public string? GetLatestBackup()
        {
            return GetBackups()
                .FirstOrDefault()
                ?.FullPath;
        }


        public bool RestoreLatest()
        {
            string? latest =
                GetLatestBackup();


            if (latest == null)
                return false;


            RestoreBackup(
                latest);


            return true;
        }


        public void RestoreBackup(
            string backupPath)
        {
            ValidateBackupPath(
                backupPath);


            if (!File.Exists(
                backupPath))
            {
                throw new FileNotFoundException(
                    "Backup file not found.");
            }


            RemoveReadOnly();


            File.Copy(
                backupPath,
                configPath,
                true);
        }


        public void DeleteBackup(
            string backupPath)
        {
            ValidateBackupPath(
                backupPath);


            if (File.Exists(
                backupPath))
            {
                File.Delete(
                    backupPath);
            }
        }


        private void RemoveReadOnly()
        {
            if (!File.Exists(
                configPath))
            {
                return;
            }


            FileAttributes attributes =
                File.GetAttributes(
                    configPath);


            if ((attributes &
                 FileAttributes.ReadOnly) != 0)
            {
                File.SetAttributes(
                    configPath,
                    attributes &
                    ~FileAttributes.ReadOnly);
            }
        }


        private void ValidateBackupPath(
            string backupPath)
        {
            string root =
                Path.GetFullPath(
                    backupDirectory)
                .TrimEnd(
                    Path.DirectorySeparatorChar)
                +
                Path.DirectorySeparatorChar;


            string target =
                Path.GetFullPath(
                    backupPath);


            if (!target.StartsWith(
                    root,
                    StringComparison
                        .OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid backup path.");
            }
        }
    }
}