
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace UsbCamera;

public partial class MainForm : Form
{
    // DirectShow camera indexes to probe during discovery.
    private const int MaxCameraIndexes = 100;
    private const int PreviewIntervalMilliseconds = 50;
    // AVI uses a fixed frame rate; slow capture can shorten playback duration.
    private const double RecordingFps = 1000.0 / PreviewIntervalMilliseconds;

    private VideoCapture? camera;
    private Mat? currentFrame;
    private readonly MediaStorage storage = new MediaStorage();
    private readonly CancellationTokenSource scanCancellation = new CancellationTokenSource();
    private bool resourcesDisposed;
    private bool isScanning;

    private readonly System.Windows.Forms.Timer previewTimer =
        new System.Windows.Forms.Timer();

    // Image settings.
    private double brightness = 0;
    private double contrast = 1.0;
    private bool mirror = false;

    public MainForm()
    {
        InitializeComponent();

        // Brightness stays in the original -100 to 100 range.
        trackBarBrightness.Minimum = -10;
        trackBarBrightness.Maximum = 10;
        trackBarBrightness.Value = 0;
        trackBarBrightness.TickFrequency = 2;

        trackBarBrightness.ValueChanged += (sender, e) =>
        {
            brightness = trackBarBrightness.Value * 10;
            labelBrightness.Text = "Brightness: " + brightness;
        };

        // The centered slider maps to contrast 0.5 through 1.5.
        trackBarContrast.Minimum = -5;
        trackBarContrast.Maximum = 5;
        trackBarContrast.Value = 0;
        trackBarContrast.TickFrequency = 1;

        trackBarContrast.ValueChanged += (sender, e) =>
        {
            contrast = 1.0 + (trackBarContrast.Value * 0.1);
            labelContrast.Text = "Contrast: " + contrast.ToString("0.0", CultureInfo.InvariantCulture);
        };

        labelContrast.Text = "Contrast: 1.0";

        checkBoxMirror.Checked = false;

        checkBoxMirror.CheckedChanged += (sender, e) =>
        {
            mirror = checkBoxMirror.Checked;
        };

        Cv2.SetLogLevel(LogLevel.ERROR);

        // Target 20 frames per second; slow capture or disk I/O can lower this.
        previewTimer.Interval = PreviewIntervalMilliseconds;
        previewTimer.Tick += PreviewTimer_Tick;

        buttonRefresh.Click += async (sender, e) =>
            await RefreshCamerasAsync();
        buttonConnect.Click += ButtonConnect_Click;
        buttonPhoto.Click += ButtonPhoto_Click;
        buttonRecord.Click += ButtonRecord_Click;

        // Saving stays disabled until the first processed frame is ready.
        buttonConnect.Enabled = false;
        buttonPhoto.Enabled = false;
        buttonRecord.Enabled = false;

        // Discover cameras after the window opens.
        Shown += async (sender, e) =>
            await RefreshCamerasAsync();

    }

    private async Task RefreshCamerasAsync()
    {
        if (isScanning || resourcesDisposed)
            return;

        DisconnectCamera();

        isScanning = true;
        buttonRefresh.Enabled = false;
        buttonRefresh.Text = "Scanning Cameras...";
        buttonConnect.Enabled = false;
        comboBoxCameras.Enabled = false;
        comboBoxCameras.Items.Clear();
        CancellationToken cancellationToken = scanCancellation.Token;

        try
        {
            List<int> cameras = await Task.Run(() =>
            {
                List<int> found = new List<int>();

                for (int i = 0; i < MaxCameraIndexes; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using (VideoCapture test = new VideoCapture())
                    {
                        if (test.Open(i, VideoCaptureAPIs.DSHOW))
                        {
                            found.Add(i);
                        }
                    }
                }

                return found;
            }, cancellationToken);

            if (resourcesDisposed || IsDisposed)
                return;

            foreach (int index in cameras)
            {
                comboBoxCameras.Items.Add(index);
            }

            if (cameras.Count > 0)
            {
                comboBoxCameras.SelectedIndex = 0;
                buttonConnect.Enabled = true;
            }
            else
            {
                MessageBox.Show(this, "No cameras found.", "USB Camera", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (OperationCanceledException)
        {
            // Closing the form cancels any remaining camera probes.
        }
        catch (Exception error)
        {
            if (!resourcesDisposed && !IsDisposed)
                ShowError("Failed to discover cameras.", error);
        }
        finally
        {
            isScanning = false;
            if (!resourcesDisposed && !IsDisposed)
            {
                buttonRefresh.Enabled = true;
                buttonRefresh.Text = "Refresh Cameras";
                comboBoxCameras.Enabled = true;
            }
        }
    }

    private void ButtonConnect_Click(object? sender, EventArgs e)
    {
        if (camera != null)
        {
            DisconnectCamera();
            return;
        }

        if (comboBoxCameras.SelectedItem is not int index)
            return;

        try
        {
            camera = new VideoCapture();

            if (!camera.Open(index, VideoCaptureAPIs.DSHOW))
            {
                DisconnectCamera();
                ShowError("Failed to open camera.");
                return;
            }

            previewTimer.Start();

            buttonConnect.Text = "Disconnect";
            buttonRefresh.Enabled = false;
            comboBoxCameras.Enabled = false;
        }
        catch (Exception error)
        {
            DisconnectCamera();
            ShowError("Failed to connect to the camera.", error);
        }
    }

    private void DisconnectCamera(bool showErrors = true)
    {
        previewTimer.Stop();

        Exception? recordingError = null;
        try
        {
            storage.StopRecording();
        }
        catch (Exception error)
        {
            recordingError = error;
        }

        camera?.Dispose();
        camera = null;

        currentFrame?.Dispose();
        currentFrame = null;

        var previousImage = pictureBoxPreview.Image;
        pictureBoxPreview.Image = null;
        previousImage?.Dispose();

        buttonConnect.Text = "Connect";
        buttonConnect.Enabled = !isScanning && comboBoxCameras.Items.Count > 0;
        buttonPhoto.Enabled = false;
        buttonRecord.Enabled = false;
        buttonRecord.Text = "Start Recording";
        buttonRefresh.Enabled = !isScanning;
        comboBoxCameras.Enabled = !isScanning;

        if (showErrors && recordingError != null)
            ShowError("Failed to finish recording.", recordingError);
    }

    private void DisposeCameraResources()
    {
        if (resourcesDisposed)
            return;

        resourcesDisposed = true;
        scanCancellation.Cancel();
        try
        {
            DisconnectCamera(showErrors: false);
        }
        finally
        {
            previewTimer.Dispose();
            storage.Dispose();
            scanCancellation.Dispose();
        }
    }

    private void PreviewTimer_Tick(object? sender, EventArgs e)
    {
        if (camera == null)
            return;

        try
        {
            using (Mat frame = new Mat())
            {
                if (!camera.Read(frame) || frame.Empty())
                {
                    DisconnectCamera();
                    ShowError("Failed to read camera frame.");
                    return;
                }

                using (Mat processed = FrameEffects.Apply(
                    frame, contrast, brightness, mirror))
                {
                    Mat nextFrame = processed.Clone();
                    currentFrame?.Dispose();
                    currentFrame = nextFrame;

                    // Record the same processed image that appears in the preview.
                    if (storage.IsRecording)
                    {
                        storage.WriteFrame(processed);
                    }

                    // Copy the bitmap so it stays valid after the stream is closed.
                    byte[] bytes = processed.ImEncode(".bmp");

                    using (MemoryStream stream = new MemoryStream(bytes))
                    using (Bitmap temporary = new Bitmap(stream))
                    {
                        Bitmap preview = new Bitmap(temporary);

                        var previousImage = pictureBoxPreview.Image;
                        pictureBoxPreview.Image = preview;
                        previousImage?.Dispose();
                    }

                    buttonPhoto.Enabled = true;
                    buttonRecord.Enabled = true;
                }
            }
        }
        catch (Exception error)
        {
            DisconnectCamera();
            ShowError("Failed to process or record a camera frame.", error);
        }
    }

    private void ButtonPhoto_Click(object? sender, EventArgs e)
    {
        if (currentFrame == null)
            return;

        try
        {
            string filename = storage.SavePhoto(currentFrame);
            MessageBox.Show(this, "Photo saved:\n" + Path.GetFullPath(filename), "USB Camera", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception error)
        {
            ShowError("Failed to save photo.", error);
        }
    }

    private void ButtonRecord_Click(object? sender, EventArgs e)
    {
        if (currentFrame == null)
            return;

        try
        {
            if (!storage.IsRecording)
            {
                storage.StartRecording(currentFrame.Size(), RecordingFps);
            }
            else
            {
                storage.StopRecording();
            }
        }
        catch (Exception error)
        {
            ShowError("Failed to start or stop recording.", error);
        }
        finally
        {
            buttonRecord.Text = storage.IsRecording ? "Stop Recording" : "Start Recording";
        }
    }

    private void ShowError(string message, Exception? error = null)
    {
        string details = error == null ? message : message + "\n" + error.Message;
        MessageBox.Show(this, details, "USB Camera", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

