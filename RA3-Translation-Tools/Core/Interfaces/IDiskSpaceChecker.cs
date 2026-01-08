namespace RA3_Translation_Tools.Core.Interfaces
{
    public interface IDiskSpaceChecker
    {
        bool HasEnoughSpace(string filePath, string targetDir, double multiplier = 1.1);
    }
}
