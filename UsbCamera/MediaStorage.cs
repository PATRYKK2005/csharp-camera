using OpenCvSharp;

namespace UsbCameraMotionDetection;

class MediaStorage : IDisposable
{
    // Nagrywarka filmu (null = brak nagrywania)
    private VideoWriter? writer = null;

    // Sprawdzenie czy trwa nagrywanie
    public bool IsRecording
    {
        get { return writer != null; }
    }

    public MediaStorage()
    {
        // Utworzenie folderu na zdjecia i filmy
        Directory.CreateDirectory("zapisane_pliki");
    }

    // 1. ZAPIS ZDJECIA
    public string SavePhoto(Mat frame)
    {
        // Nazwa pliku z data i godzina
        string czas = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string sciezka = "zapisane_pliki/zdjecie_" + czas + ".jpg";

        // Zapis zdjecia na dysk
        bool udalo = Cv2.ImWrite(sciezka, frame);

        if (!udalo)
        {
            throw new Exception("Nie udalo sie zapisac zdjecia: " + sciezka);
        }

        return sciezka;
    }

    // 2. START NAGRYWANIA
    public string StartRecording(Size frameSize, double fps)
    {
        // Zakonczenie poprzedniego nagrania, jesli jeszcze trwa
        if (writer != null)
        {
            StopRecording();
        }

        // Nazwa pliku z data i godzina
        string czas = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string sciezka = "zapisane_pliki/film_" + czas + ".avi";

        // Otwarcie pliku AVI z kodekiem MJPG
        writer = new VideoWriter(sciezka, FourCC.MJPG, fps, frameSize);

        if (!writer.IsOpened())
        {
            writer.Dispose();
            writer = null;
            throw new Exception("Nie udalo sie otworzyc pliku wideo: " + sciezka);
        }

        return sciezka;
    }

    // 3. ZAPIS KLATKI DO FILMU
    public void WriteFrame(Mat frame)
    {
        if (writer != null)
        {
            writer.Write(frame);
        }
    }

    // 4. KONIEC NAGRYWANIA
    public void StopRecording()
    {
        if (writer != null)
        {
            // Zamkniecie pliku AVI
            writer.Release();
            writer.Dispose();
            writer = null;
        }
    }

    public void Dispose()
    {
        // Zakonczenie nagrywania przy zamykaniu programu
        StopRecording();
    }
}
