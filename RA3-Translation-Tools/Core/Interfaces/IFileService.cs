using SadPencil.Ra2CsfFile;
using System.Threading.Tasks;

namespace RA3_Translation_Tools.Core.Interfaces
{
    public interface IFileService
    {
        Task<string> ReadFileAsync(string filePath);
        Task<CsfFile> ReadCsfFileAsync(string filePath);
        Task WriteFileAsync(string filePath, string content);
        Task WriteCsfFileAsync(string filePath, CsfFile csf);
    }
}
