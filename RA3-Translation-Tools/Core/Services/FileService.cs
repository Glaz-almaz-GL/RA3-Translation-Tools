using RA3_Translation_Tools.Core.Interfaces;
using SadPencil.Ra2CsfFile;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RA3_Translation_Tools.Core.Services
{
    public class FileService() : IFileService
    {
        public async Task<string> ReadFileAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }

        public async Task<CsfFile> ReadCsfFileAsync(string filePath)
        {
            await using FileStream stream = File.OpenRead(filePath);
            return CsfFile.LoadFromCsfFile(stream);
        }

        public async Task WriteFileAsync(string filePath, string content)
        {
            string normalized = content.Replace("\r\n", "\n").Replace("\n", "\r\n");
            await File.WriteAllTextAsync(filePath, normalized, new UTF8Encoding(false, true));
        }

        public async Task WriteCsfFileAsync(string filePath, CsfFile csf)
        {
            await using FileStream fs = File.OpenWrite(filePath);
            csf.WriteCsfFile(fs);
        }
    }
}
