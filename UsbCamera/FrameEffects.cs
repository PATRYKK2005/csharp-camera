using OpenCvSharp;

namespace UsbCamera;

static class FrameEffects
{
    public static Mat Apply(Mat source, double contrast, double brightness, bool flip)
    {
        Mat result = new Mat();
        try
        {
            // Each pixel becomes: pixel * contrast + brightness.
            source.ConvertTo(result, -1, contrast, brightness);

            if (flip)
            {
                // Mirror horizontally (left to right).
                Cv2.Flip(result, result, FlipMode.Y);
            }

            // The caller owns the returned frame.
            return result;
        }
        catch
        {
            result.Dispose();
            throw;
        }
    }
}
