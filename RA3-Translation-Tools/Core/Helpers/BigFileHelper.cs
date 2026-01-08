using RA3_Translation_Tools.Core.Constants;
using RA3_Translation_Tools.Managers;
using System;
using System.Diagnostics;
using System.IO;

namespace RA3_Translation_Tools.Core.Helpers
{
    public static class BigFileHelper
    {
        public static void PackBig(string inputDir, string outputBigFilePath)
        {
            try
            {
                ProcessStartInfo psi = new()
                {
                    FileName = PathConstants.MakeBigPath,
                    Arguments = $"-o:\"{outputBigFilePath}\" -q \"{inputDir}\"",
                    UseShellExecute = false,           // обязательно false для перенаправления
                    CreateNoWindow = true,             // ← скрывает консольное окно
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(inputDir) ?? Environment.CurrentDirectory
                };

                using Process? process = Process.Start(psi) ?? throw new InvalidOperationException("Не удалось запустить процесс упаковки.");
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string msg = $"Код ошибки: {process.ExitCode}\nВывод:\n{output}\nОшибки:\n{error}";
                    throw new InvalidOperationException(msg);
                }
            }
            catch (Exception ex)
            {
                GrowlsManager.ShowErrorMsg(ex, "Ошибка упаковки .big файла");
            }
        }

        public static void UnpackBig(string bigFilePath, string outputDir)
        {
            try
            {
                string fileName = Environment.Is64BitOperatingSystem
                    ? PathConstants.Big4F_64Path
                    : PathConstants.Big4F_32Path;

                // Убедимся, что выходная папка существует
                Directory.CreateDirectory(outputDir);

                ProcessStartInfo psi = new()
                {
                    FileName = fileName,
                    Arguments = $"x \"{bigFilePath}\" \"{outputDir}\"",
                    UseShellExecute = false,           // обязательно false
                    CreateNoWindow = true,             // ← скрывает окно консоли
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(bigFilePath) ?? Environment.CurrentDirectory
                };

                using Process? process = Process.Start(psi) ?? throw new InvalidOperationException("Не удалось запустить процесс распаковки.");
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string msg = $"Код ошибки: {process.ExitCode}\nВывод:\n{output}\nОшибки:\n{error}";
                    throw new InvalidOperationException(msg);
                }
            }
            catch (Exception ex)
            {
                GrowlsManager.ShowErrorMsg(ex, "Ошибка распаковки .big файла");
            }
        }
    }
}