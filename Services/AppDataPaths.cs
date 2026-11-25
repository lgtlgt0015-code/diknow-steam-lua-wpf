using System.IO;

namespace SteamLuaManager.Services;

internal static class AppDataPaths
{
    public static string RootDirectory { get; } = GetRootDirectory();

    public static string CacheDirectory => Path.Combine(RootDirectory, "cache");
    public static string LogDirectory => Path.Combine(RootDirectory, "logs");
    public static string SettingsFile => Path.Combine(RootDirectory, "settings.json");

    public static string GetSettingsFileForRead(string legacyDirectory) =>
        File.Exists(SettingsFile) ? SettingsFile : Path.Combine(legacyDirectory, "settings.json");

    private static string GetRootDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData,
            Environment.SpecialFolderOption.DoNotVerify);
        if (!Path.IsPathFullyQualified(localAppData))
            localAppData = Path.GetTempPath();

        return Path.Combine(localAppData, "diknow-steam-lua");
    }
}
