using System;
using System.IO;

namespace RA3_Translation_Tools.Core.Constants
{
    public static class PathConstants
    {
        public static readonly string AppFolder = Path.GetDirectoryName(Environment.ProcessPath)!;
        public static readonly string RoamingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string MapsFolder = Path.Combine(RoamingFolder, "Red Alert 3", "Maps");
        public static readonly string CasheFolder = Path.Combine(AppFolder, "Cache");

        public static readonly string UtilsDir = Path.Combine(AppFolder, "Utils");
        public static readonly string FontsDir = Path.Combine(AppFolder, "Fonts");

        public static readonly string Big4FDir = Path.Combine(UtilsDir, "big4f");

        public static readonly string Big4F_64Path = Path.Combine(Big4FDir, "big4f_64x.exe");
        public static readonly string Big4F_32Path = Path.Combine(Big4FDir, "big4f_32x.exe");
        public static readonly string MakeBigPath = Path.Combine(UtilsDir, "MakeBig.exe");
    }
}