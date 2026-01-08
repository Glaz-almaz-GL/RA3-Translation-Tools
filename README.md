# RA3 Translation Tools

A cross-platform desktop application built with Avalonia UI, designed to assist in translating and converting files for Red Alert 3 (RA3) modding, but this program can be used for other C&C games whose release date is later than or equal to 2000, for example, Red Alert 2. It provides tools for packing/unpacking `.big` archives and converting between `.str`, `.csf`, and `.json` file formats commonly used for text localization.

## Features

*   **.big Archive Management:**
    *   Unpack `.big` files to a specified directory.
    *   Pack a directory of files back into a `.big` archive.
*   **Text File Conversion:**
    *   Convert `.str` files to `.csf` format.
    *   Convert `.csf` files to `.str` format.
    *   Convert `.csf` files to `.json` format for easier editing with translation tools.
    *   Convert `.json` files back to `.csf` format, allowing specification of the target language.
*   **User-Friendly Interface:** Built with [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia) for a modern and elegant look.
*   **Cross-Platform:** Runs on Windows, Linux, macOS, iOS, Android, and WebAssembly (with .NET support).

## Requirements

*   [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later runtime installed on your system.

## Installation

1.  Download the latest release from the [Releases](https://github.com/Glaz-almaz-GL/RA3-Translation-Tools/releases) page (if available).
2.  Extract the downloaded archive to a folder of your choice.
3.  Run the executable file (`RA3-Translation-Tools.exe` on Windows, or the appropriate executable for your platform).

Alternatively, if you have the source code and .NET SDK installed:

1.  Clone the repository: `git clone https://github.com/Glaz-almaz-GL/Avalonia_RA3_Mod_Translator.git`
2.  Navigate to the project directory: `cd Avalonia_RA3_Mod_Translator`
3.  Build the project: `dotnet build -c Release`
4.  Run the application: `dotnet run --project RA3-Translation-Tools`

## Usage

1.  Launch the application.
2.  Use the top section to pack or unpack `.big` archives by specifying the input file/directory and output location.
3.  Use the middle section to convert between `.str` and `.csf` files.
4.  Use the bottom section to convert between `.csf` and `.json` files. When converting `.json` to `.csf`, select the appropriate language from the dropdown menu.

<img width="700" height="728" alt="Screenshot_1" src="https://github.com/user-attachments/assets/3ae8ffca-dc37-44bf-a06f-9e58b7aca844" />

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

*   [Avalonia UI](https://avaloniaui.com/) - Cross-platform UI framework.
*   [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia) - Modern UI component library for Avalonia.
*   [SadPencil.Ra2CsfFile](https://github.com/SadPencil/Ra2CsfFile) - Library for handling CSF files.
*   [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM helpers and attributes.
*   [FluentIcons.Avalonia](https://github.com/microsoft/fluentui-system-icons) - Icon library used by Huskui.

## Discord Channel

Join to my [discord channel](https://discord.gg/tzW856ewKm), here you can find translations into various languages ​​for RA3 modifications, as well as modding and translation tools and help with the game.
