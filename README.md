# USB Camera

A lightweight Windows desktop application for USB camera capture and real-time image processing, built with **C#**, **.NET 8**, **Windows Forms**, and **OpenCvSharp**.

The application provides a graphical interface for previewing camera feeds, capturing photos, recording videos, and adjusting image properties without using the command line.

## Features

- **Camera discovery** — Detect available cameras using the DirectShow backend.
- **Live preview** — Display the selected camera feed directly in the application.
- **Photo capture** — Capture and save images in JPEG format.
- **Video recording** — Record videos in AVI format using the MJPG codec.
- **Brightness adjustment** — Adjust brightness from -100 to +100.
- **Contrast adjustment** — Adjust contrast from 0.5 to 1.5, with 1.0 as the default.
- **Horizontal mirroring** — Flip the camera image horizontally.
- **Camera controls** — Connect, disconnect, and refresh available devices.
- **Error handling** — Handle camera connection failures, invalid frames, and media-saving errors.

Image adjustments are applied to both the live preview and saved media.

## Tech Stack

| Technology    | Purpose                             |
| ------------- | ----------------------------------- |
| C#            | Application logic                   |
| .NET 8        | Application framework               |
| Windows Forms | Desktop graphical user interface    |
| OpenCvSharp4  | Camera capture and image processing |
| DirectShow    | Camera access on Windows            |
| Git & GitHub  | Version control and collaboration   |

## Requirements

- Windows 10 or Windows 11 (x64)
- A compatible webcam or USB camera
- .NET 8 Desktop Runtime for framework-dependent builds

For development:

- Visual Studio 2022 with the .NET desktop development workload, or a compatible .NET SDK
- .NET 8 SDK

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/PATRYKK2005/csharp-camera.git
cd csharp-camera
```

### 2. Build the application

Open `UsbCamera.sln` in Visual Studio and build the solution.

Alternatively, use the .NET CLI:

```bash
dotnet restore UsbCamera.sln
dotnet build UsbCamera.sln
```

NuGet automatically restores the required OpenCvSharp dependencies.

### 3. Run the application

Press `F5` or `Ctrl + F5` in Visual Studio.

Alternatively:

```bash
dotnet run --project UsbCamera/UsbCamera.csproj
```

The application opens a Windows Forms window and starts scanning for available cameras.

## Usage

1. Wait for the application to discover available cameras.
2. Select a camera from the dropdown list.
3. Click **Connect** to start the live preview.
4. Use **Take Photo** to capture a JPEG image.
5. Use **Start Recording** and **Stop Recording** to record a video.
6. Adjust brightness and contrast using the sliders.
7. Enable **Mirror image** to flip the preview horizontally.
8. Click **Disconnect** to release the camera.

Use **Refresh Cameras** to scan for devices again.

### Output Files

Captured media is saved to a `zapisane_pliki` directory, relative to the application's current working directory.

Supported output formats:

| Media  | Format                          |
| ------ | ------------------------------- |
| Photos | JPEG (`.jpg`)                   |
| Videos | AVI (`.avi`) with MJPG encoding |

Files are automatically named using timestamps to reduce naming conflicts.

## Project Structure

```text
csharp-camera/
├── UsbCamera.sln
├── UsbCamera/
│   ├── Program.cs
│   ├── MainForm.cs
│   ├── MainForm.Designer.cs
│   ├── MainForm.resx
│   ├── FrameEffects.cs
│   ├── MediaStorage.cs
│   └── UsbCamera.csproj
├── .gitignore
└── README.md
```

### Main Components

**Program.cs**

Application entry point. Initializes Windows Forms and opens the main window.

**MainForm.cs**

Manages the user interface, camera discovery, camera connection, live preview, and user interactions.

**FrameEffects.cs**

Processes image frames by applying brightness, contrast, and horizontal mirroring.

**MediaStorage.cs**

Handles JPEG photo capture and AVI/MJPG video recording, including file creation and resource cleanup.

## Technical Notes

- Camera discovery scans DirectShow device indexes from 0 to 99. Scanning may take some time depending on the system.
- The application targets approximately 20 preview updates per second.
- Video recording uses a fixed 20 FPS setting. Actual capture performance may vary depending on camera hardware and system performance.
- Camera capture and video writing run on the UI thread during preview updates, so slow devices may affect interface responsiveness.
- Camera and image resources are released when disconnecting or closing the application.

## Build Verification

The application has passed Debug and Release builds, along with automated checks covering image processing, media encoding, and resource handling.

Hardware behavior depends on the connected camera and Windows drivers. Physical camera operation should be verified on the target computer.
