# Linux Desktop Shortcut Builder

## About

I wanted to simplify my own workflow for creating `.desktop` applications for applications I install in the AppImage format, so I created this application to make it simpler to get things set up!

### Built With

- [Avalonia UI 11](https://www.avaloniaui.net/)
- [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Getting Started

### Prerequisites

- Install the [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

### Installation

1. Clone the repo
   > git clone `https://github.com/gridlocdev/LinuxDesktopShortcutBuilder.git`

2. Install the project's NuGet packages
   > dotnet restore

3. (Optional, if you want the fast version) run `dotnet publish -c Release`, then use the compiled executable in the project directory: `./LinuxDesktopShortcutBuilder/bin/Release/net7.0/linux-x64/publish/LinuxDesktopShortcutBuilder`

## Usage

1. (Optional) Find or download an icon that you'd like to use for your application shortcut.
   > Note: Icons can usually be found on the app's website, or in a "brand" repository with image assets if the creator is cool enough
  
2. Run `dotnet run` in the to launch the application

## Roadmap

- [x] Build a basic functional version of the application
- [x] Remove deprecated fields (it looks like "Encoding" and "Terminal" have some things to fix using `desktop-file-validate`
- [ ] Add some additional fields (such as "Category")
- [ ] Add ComboBox controls to provide helpful suggestions for frequently used values
- [ ] Make it look prettier

## Contributing

I'll gladly review pull requests if there are any!

1. Fork the repository
2. Commit locally, then push your changes to the fork
3. Open a Pull Request

## Acknowledgements

Here are some helpful resources that I used when creating this project

- [ArchLinux Wiki - Desktop entries](https://wiki.archlinux.org/title/desktop_entries)
- [The xdg/desktop-file-utils CLI utilities](https://www.freedesktop.org/wiki/Software/desktop-file-utils/)
- [Avalonia UI 11 Documentation](https://docs.avaloniaui.net/docs/next/welcome)
- [Microsoft Learn - dotnet publish](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)
