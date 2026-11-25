using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace SteamLuaManager.Services;

// Steam 进程启停：主窗口浮动菜单与云存档页共用，行为保持一致
public static class SteamProcess
{
    public sealed record SteamLaunchResult(bool Ok, string Title, string Message);

    public static bool IsSteamRunning()
    {
        try
        {
            var procs = Process.GetProcessesByName("steam");
            try
            {
                return procs.Length > 0;
            }
            finally
            {
                foreach (var p in procs)
                    try { p.Dispose(); } catch { }
            }
        }
        catch
        {
            return false;
        }
    }

    public static void KillSteamProcesses()
    {
        try
        {
            foreach (var proc in Process.GetProcessesByName("steam"))
            {
                using (proc)
                {
                    if (proc.Id != 0)
                        proc.Kill();
                }
            }
        }
        catch { }
    }

    public static SteamLaunchResult LaunchSteam(ISteamPathService steamPathService)
    {
        try
        {
            var path = steamPathService.GetCustomPath() ?? steamPathService.DetectSteamPath();
            if (string.IsNullOrEmpty(path))
                return new SteamLaunchResult(false, L.Get("Main_Tip"), L.Get("Svc_SpNoPath"));

            var exePath = Path.Combine(path, "steam.exe");
            if (!File.Exists(exePath))
                return new SteamLaunchResult(false, L.Get("Main_Tip"), L.Get("Svc_SpNoExe", exePath));

            StartSteamProcess(exePath)?.Dispose();
            return new SteamLaunchResult(true, string.Empty, string.Empty);
        }
        catch (Exception ex)
        {
            return new SteamLaunchResult(false, L.Get("Main_DialogError"), L.Get("Svc_SpStartFailed", ex.Message));
        }
    }

    public static SteamLaunchResult RestartSteam(ISteamPathService steamPathService)
    {
        KillSteamProcesses();
        return LaunchSteam(steamPathService);
    }

    public static async Task<SteamLaunchResult> RestartSteamClientAsync(ISteamPathService steamPathService)
    {
        try
        {
            var path = steamPathService.GetCustomPath() ?? steamPathService.DetectSteamPath();
            if (string.IsNullOrEmpty(path))
                return new SteamLaunchResult(false, L.Get("Main_Tip"), L.Get("Svc_SpNoPath"));

            var exePath = Path.Combine(path, "steam.exe");
            if (!File.Exists(exePath))
                return new SteamLaunchResult(false, L.Get("Main_Tip"), L.Get("Svc_SpNoExe", exePath));

            var running = Process.GetProcessesByName("steam");
            try
            {
                if (running.Any(process => !process.HasExited))
                {
                    using var shutdown = StartSteamProcess(exePath, "-shutdown");

                    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                    try
                    {
                        await Task.WhenAll(running.Select(process => process.WaitForExitAsync(timeout.Token)));
                        if (shutdown is not null)
                            await shutdown.WaitForExitAsync(timeout.Token);
                    }
                    catch (OperationCanceledException) when (timeout.IsCancellationRequested)
                    {
                        return new SteamLaunchResult(false, L.Get("Main_DialogError"), L.Get("Svc_SpShutdownTimeout"));
                    }
                }
            }
            finally
            {
                foreach (var process in running)
                    process.Dispose();
            }

            StartSteamProcess(exePath)?.Dispose();
            var startup = Stopwatch.StartNew();
            var stableSince = TimeSpan.Zero;
            int? previousPid = null;
            while (startup.Elapsed < TimeSpan.FromSeconds(20))
            {
                var pid = GetRunningSteamId();
                if (pid != previousPid)
                {
                    previousPid = pid;
                    stableSince = startup.Elapsed;
                }
                else if (pid.HasValue && startup.Elapsed - stableSince >= TimeSpan.FromSeconds(3))
                {
                    return new SteamLaunchResult(true, string.Empty, string.Empty);
                }

                await Task.Delay(250);
            }

            return new SteamLaunchResult(false, L.Get("Main_DialogError"), L.Get("Svc_SpRestartNotRunning"));
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or IOException)
        {
            return new SteamLaunchResult(false, L.Get("Main_DialogError"), L.Get("Svc_SpRestartFailed", ex.Message));
        }
    }

    private static Process? StartSteamProcess(string exePath, string arguments = "") =>
        Process.Start(new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = arguments,
            WorkingDirectory = Path.GetDirectoryName(exePath) ?? string.Empty,
            UseShellExecute = true
        });

    private static int? GetRunningSteamId()
    {
        var processes = Process.GetProcessesByName("steam");
        try
        {
            return processes.FirstOrDefault(process => !process.HasExited)?.Id;
        }
        finally
        {
            foreach (var process in processes)
                process.Dispose();
        }
    }
}
