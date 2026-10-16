using OpenCvSharp;

namespace UsbCameraMotionDetection;

static class FrameEffects
{
    public static Mat Apply(Mat source, double contrast, double brightness, bool flip)
    {
        // 1. NOWY OBRAZ NA WYNIK
        Mat wynik = new Mat();

        // 2. ZMIANA KONTRASTU I JASNOSCI
        // Kazdy piksel: piksel * kontrast + jasnosc
        source.ConvertTo(wynik, -1, contrast, brightness);

        // 3. ODBICIE LUSTRZANE
        if (flip)
        {
            // Odbicie w poziomie (lewo <-> prawo)
            Cv2.Flip(wynik, wynik, FlipMode.Y);
        }

        return wynik;
    }
}
