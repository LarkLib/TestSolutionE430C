using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSharpCompressConsoleApplication
{
    class SharpCompressOperation
    {
        string RarPath = ConfigurationManager.AppSettings["RarPath"];
        string ExtractPath = ConfigurationManager.AppSettings["ExtractPath"];
        string BackupPath = ConfigurationManager.AppSettings["BackupPath"];
        bool IsExtractToRarPath = bool.Parse(ConfigurationManager.AppSettings["IsExtractToRarPath"]);
        bool NeedDeleteRarFile = bool.Parse(ConfigurationManager.AppSettings["NeedDeleteRarFile"]);
        string Password = ConfigurationManager.AppSettings["Password"];
        int MaxFolderCount = int.Parse(ConfigurationManager.AppSettings["MaxFolderCount"]);
        int MaxThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxThreadCount"]);
        internal void Execute()
        {
            ExtractRarFiles();
        }
        private void ExtractRarFiles()
        {
            var directories = Directory.GetDirectories(RarPath, "*", SearchOption.AllDirectories).ToList();
            directories.Insert(0, RarPath);
            //foreach (var dir in directories)
            Parallel.ForEach(directories, new ParallelOptions() { MaxDegreeOfParallelism = MaxFolderCount }, dir =>
            {
                var fileList = new List<string>();
                fileList.AddRange(Directory.GetFiles(dir, "*.rar"));
                Parallel.ForEach(fileList, new ParallelOptions() { MaxDegreeOfParallelism = MaxThreadCount }, file =>
                      {
                          //using (var archive = RarArchive.Open(Path.Combine(RarPath, "StockDataCollector.rar"), new ReaderOptions() { LookForHeader = true, Password = null }))
                          using (var archive = RarArchive.Open(file, new ReaderOptions() { LookForHeader = true, Password = Password }))
                          {
                              foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                              {
                                  Console.WriteLine($"{entry.IsEncrypted},{entry.Key}");
                                  entry.WriteToDirectory(Path.Combine(IsExtractToRarPath ? dir : ExtractPath, Path.GetFileNameWithoutExtension(file)), new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                              }
                          }
                          if (NeedDeleteRarFile)
                          {
                              File.Delete(file);
                          }
                          else
                          {
                              if (!Directory.Exists(BackupPath))
                              {
                                  Directory.CreateDirectory(BackupPath);
                              }
                              File.Move(file, Path.Combine(BackupPath, Path.GetFileName(file)));
                          }
                      });
                Task.WaitAll();
            });
        }
    }
}
