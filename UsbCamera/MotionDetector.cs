using OpenCvSharp;

namespace UsbCameraMotionDetection;

class MotionDetector : IDisposable
{
    // Model tla MOG2
    private BackgroundSubtractorMOG2 mog2;

    public MotionDetector()
    {
        // Utworzenie modelu tla (500 klatek historii, czulosc 16, bez cieni)
        mog2 = BackgroundSubtractorMOG2.Create(500, 16, false);
    }

    public void DrawMotion(Mat preview)
    {
        // 1. MASKA RUCHU
        // Bialy piksel = ruch, czarny = tlo
        Mat maska = new Mat();
        mog2.Apply(preview, maska);

        // 2. SZUKANIE KONTUROW
        Point[][] kontury;
        HierarchyIndex[] hierarchia;
        Cv2.FindContours(maska, out kontury, out hierarchia,
            RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        // 3. RYSOWANIE PROSTOKATOW
        foreach (Point[] kontur in kontury)
        {
            // Pominiecie malych plamek (szum)
            if (Cv2.ContourArea(kontur) > 500)
            {
                Rect prostokat = Cv2.BoundingRect(kontur);

                // Czerwony prostokat (kolory w kolejnosci BGR)
                Cv2.Rectangle(preview, prostokat, new Scalar(0, 0, 255), 2);
            }
        }

        // Zwolnienie pamieci maski
        maska.Dispose();
    }

    public void Reset()
    {
        // Usuniecie starego modelu tla i utworzenie nowego
        mog2.Dispose();
        mog2 = BackgroundSubtractorMOG2.Create(500, 16, false);
    }

    public void Dispose()
    {
        // Zwolnienie pamieci modelu tla
        mog2.Dispose();
    }
}
