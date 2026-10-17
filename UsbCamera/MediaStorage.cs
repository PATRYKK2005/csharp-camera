using OpenCvSharp;

namespace UsbCamera;

class MediaStorage : IDisposable
{
    // Keep existing output names so previously saved media stays in one place.
    private const string OutputDirectory = "zapisane_pliki";
    private VideoWriter? writer;
    private OpenCvSharp.Size recordingSize;

    public bool IsRecording
    {
        get { return writer != null; }
    }

    public string SavePhoto(Mat frame)
    {
        if (frame.Empty())
            throw new ArgumentException("The photo frame is empty.", nameof(frame));

        // Create the directory here so file errors reach the UI's error handler.
        Directory.CreateDirectory(OutputDirectory);
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string path = Path.Combine(OutputDirectory, "zdjecie_" + timestamp + ".jpg");

        bool saved = Cv2.ImWrite(path, frame);

        if (!saved)
        {
            throw new IOException("Failed to save photo: " + path);
        }

        return path;
    }

    public string StartRecording(OpenCvSharp.Size frameSize, double fps)
    {
        if (frameSize.Width <= 0 || frameSize.Height <= 0 ||
            frameSize.Width % 2 != 0 || frameSize.Height % 2 != 0)
            throw new ArgumentException("MJPG recording requires positive, even frame dimensions.", nameof(frameSize));

        if (!double.IsFinite(fps) || fps <= 0)
            throw new ArgumentOutOfRangeException(nameof(fps), "Recording FPS must be positive and finite.");

        StopRecording();
        Directory.CreateDirectory(OutputDirectory);
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string path = Path.Combine(OutputDirectory, "film_" + timestamp + ".avi");

        VideoWriter newWriter = new VideoWriter();
        try
        {
            if (!newWriter.Open(path, FourCC.MJPG, fps, frameSize))
                throw new IOException("Failed to open video file: " + path);

            recordingSize = frameSize;
            writer = newWriter;
        }
        catch
        {
            newWriter.Dispose();
            throw;
        }

        return path;
    }

    public void WriteFrame(Mat frame)
    {
        if (writer != null)
        {
            if (frame.Empty() || frame.Size() != recordingSize || frame.Type() != MatType.CV_8UC3)
                throw new InvalidOperationException("The video frame must match the recording dimensions and use 8-bit BGR color.");

            writer.Write(frame);
        }
    }

    public void StopRecording()
    {
        // Clear the state first; Dispose also finalizes and closes the AVI file.
        VideoWriter? previousWriter = writer;
        writer = null;
        previousWriter?.Dispose();
    }

    public void Dispose()
    {
        StopRecording();
    }
}
