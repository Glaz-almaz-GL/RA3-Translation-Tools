namespace RA3_Translation_Tools.Core.Interfaces
{
    public interface ITempDirectoryManager
    {
        string GetUnpackPath(string bigFilePath, string baseTempDir);
        void EnsureCleanUnpackDirectory(string unpackDir);
        void CleanupAll(string baseTempDir);
    }
}
