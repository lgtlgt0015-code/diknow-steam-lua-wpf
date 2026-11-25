# diknow-steam-lua

<p>
  <a href="README.md">
    <img src="https://flagcdn.com/w40/cn.png" width="22" alt="China flag">
    简体中文
  </a>
  &nbsp;|&nbsp;
  <a href="README.en.md">
    <img src="https://flagcdn.com/w40/us.png" width="22" alt="United States flag">
    <b>English</b>
  </a>
</p>

A personal, customized build of a Steam Lua library manager, derived from [Fluent-Steam-Lua](https://github.com/huanyuejue/Fluent-Steam-Lua). Built with WPF + Fluent Design and currently supports OpenSteamTool only.

> This is an unofficial modified version, not affiliated with the original author, and it does not carry the original project's community or feedback channels.

## Changes in this fork

- Product name, window title, EXE icon and About page unified as `diknow-steam-lua`
- Added dark theme resources; the original follow-system / dark / light themes remain available
- Removed manifest monitoring, account file extraction, issue feedback and FAQ entries
- App updates are read only from this repository's Releases
- Settings, cache and logs live under `%LOCALAPPDATA%\diknow-steam-lua`
- Auto-update tolerates a renamed EXE (e.g. `xxx(3).exe`, `xxx - copy.exe`)

## Preview

<table>
  <tr>
    <td align="center">
      <img src="https://raw.githubusercontent.com/huanyuejue/Fluent-Steam-Lua/assets/screenshots/home.gif" width="400" />
    </td>
    <td align="center">
      <img src="https://raw.githubusercontent.com/huanyuejue/Fluent-Steam-Lua/assets/screenshots/ruku.gif" width="400" />
    </td>
  </tr>
  <tr>
    <td align="center">
      <img src="https://raw.githubusercontent.com/huanyuejue/Fluent-Steam-Lua/assets/screenshots/setting.gif" width="400" />
    </td>
    <td align="center">
      <img src="https://raw.githubusercontent.com/huanyuejue/Fluent-Steam-Lua/assets/screenshots/dlc.gif" width="400" />
    </td>
  </tr>
</table>

## Features

#### Library Management
- Auto-reads imported games and shows cover art and localized names (three switchable layouts)
- Search and import new games by AppID or game name
- Check DLC import status for the current game's manifest, with one-click DLC completion
- Quickly disable or enable a game's Lua manifest without deleting it
- Pin a game's manifest to the latest or currently installed version
- Extract and use Denuvo authorization data (appticket and eticket)
- One-click search and download for 480 and OnlineFix multiplayer patches

#### Trainers
- Search, download and manage game trainers (FLiNG)
- Bind a trainer to a game process so it launches automatically with the game
- After binding, optionally auto-activate specific trainer features when the trainer starts

#### Kernel
- A personally maintained fork of OpenSteamTool
- Quick install / update / uninstall of the OpenSteamTool kernel
- Customizable scan location for Lua manifest files
- Broadcast real play status to friends for Lua-imported games
- Hot-switch the upstream manifest source used by the kernel

#### Steam
- Quick launch and restart Steam
- Quick switch between Steam accounts with saved local credentials
- Manage achievement data for owned games with cloud backup (unlock / re-lock)

#### Cloud Save Redirection
- Built on [CloudRedirect](https://github.com/huanyuejue/CloudRedirect)
- Redirects Steam cloud saves of Lua-imported games to a storage location of your choice, fixing cloud save errors
- Six storage backends: local folder, Google Drive, OneDrive, Cloudflare R2, S3-compatible storage and WebDAV, switchable at any time in the UI
- Toggling and storage location changes take effect after a Steam restart; achievements and playtime still sync to cloud
- The target directory supports custom paths and one-click reset to default; switching locations migrates existing saves
- Lists and manages redirected games, with one-click open path and clear saves (restores the game to its original state)
- Deleting saves backs up and verifies the full data first; it only proceeds after verification, and backups can be copied back manually

#### Steam-Free Launch
- One-click SteamStub removal and Goldberg emulator deployment, letting games run without Steam while achievements still pop
- Supports restoring the crack; the emulator can be downloaded/updated (download source follows the mirror preference in settings, auto-switching between direct and mirror)
- Built-in AppID lookup: search candidates by game folder name and fill in
- Optional Steam Web API Key: fills in achievement and inventory info when provided, skipped otherwise, does not affect cracking
- Note: works only for SteamStub games; Denuvo-protected games are not supported

#### Other
- Automatic file change monitoring and cache refresh
- Modern Fluent Design interface, built with WPF for easy debugging
- Theme support: follow system / dark / light
- Backdrop support: Acrylic / Mica / none
- Multi-language UI (Simplified Chinese / English), switchable in settings with immediate effect and no restart
- Automatic update check (can be disabled)

## Download

Pick a file from the [Releases](https://github.com/lgtlgt0015-code/diknow-steam-lua-wpf/releases) page:

| File | Description |
| --- | --- |
| `diknow-steam-lua-<version>-win-x64.exe` | Self-contained single file with the .NET 8 runtime bundled |
| `diknow-steam-lua-<version>-single-file.zip` | Update package for single-file installs |
| `diknow-steam-lua-<version>-loose-file.zip` | Update package for loose-file installs |

## Requirements

- Windows 10 1809+ / Windows 11
- The self-contained build needs nothing extra; the loose build needs [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

## Data directory

Settings, cache and logs are stored in:

```
%LOCALAPPDATA%\diknow-steam-lua
```

## Building

Prerequisites: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0). The build automatically fetches the CloudRedirect native DLL (following the latest release of the maintained repo by default, requires network, with SHA256 verification). To reproduce an older build, pin the version: `dotnet build -p:CloudRedirectVersion=v2.6.5`.

Self-contained single file (the form used for releases):

```cmd
dotnet publish SteamLuaManager.csproj -c Release -r win-x64 --self-contained true -p:DebugType=none -p:AppExeName="diknow-steam-lua" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/standalone
```

Single file (framework-dependent):

```cmd
dotnet publish SteamLuaManager.csproj -c Release -r win-x64 --self-contained false -p:DebugType=none -p:AppExeName="diknow-steam-lua" -p:PublishSingleFile=true -o publish/single
```

Loose files:

```cmd
dotnet publish SteamLuaManager.csproj -c Release -r win-x64 --self-contained false -p:DebugType=none -p:AppExeName="diknow-steam-lua" -o publish/loose
```

## License and credits

- Based on [Fluent-Steam-Lua](https://github.com/huanyuejue/Fluent-Steam-Lua), under its zlib License, see [LICENSE](LICENSE)
- Cloud save redirection is built on [CloudRedirect](https://github.com/huanyuejue/CloudRedirect) (MIT License)
- Other third-party components and licenses: [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md)
