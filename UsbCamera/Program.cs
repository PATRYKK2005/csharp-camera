
using System;
using System.Collections.Generic;
using OpenCvSharp;

Cv2.SetLogLevel(LogLevel.ERROR); //żeby nei było błedów

// 1. WYSZUKIWANIE KAMER
Console.WriteLine("Szukam dostepnych kamer...");

List<int> znalezioneKamery = new List<int>();

Console.Write("Ile indeksow kamer sprawdzic? ");

int liczbaKamer;

if (!int.TryParse(Console.ReadLine(), out liczbaKamer) || liczbaKamer < 1)
{
    Console.WriteLine("Podaj poprawna liczbe!");
    return;
}

for (int i = 0; i < liczbaKamer; i++)
{
    using (VideoCapture test = new VideoCapture(i, VideoCaptureAPIs.DSHOW))
    {
        if (test.IsOpened())
        {
            znalezioneKamery.Add(i);
            Console.WriteLine("Znaleziono kamere: " + i);
        }
    }
}

// 2. SPRAWDZENIE LISTY KAMER
if (znalezioneKamery.Count == 0)
{
    Console.WriteLine("Nie znaleziono zadnej kamery!");
    return;
}


// 3. WYBOR KAMERY
Console.Write("Podaj numer kamery: ");

int numerKamery;

if (!int.TryParse(Console.ReadLine(), out numerKamery)
    || !znalezioneKamery.Contains(numerKamery))
{
    Console.WriteLine("Nieprawidlowy numer kamery!");
    return;
}

// 4. POLACZENIE Z KAMERA
using (VideoCapture kamera = new VideoCapture(numerKamery, VideoCaptureAPIs.DSHOW))
{
    if (!kamera.IsOpened())
    {
        Console.WriteLine("Nie mozna otworzyc kamery!");
        return;
    }

    using (Mat klatka = new Mat())
    {
        Cv2.NamedWindow("Kamera", WindowFlags.Normal);

        Console.WriteLine("Kamera uruchomiona!");
        Console.WriteLine("Nacisnij Q, aby zakonczyc.");

        try
        {
            // 5. PETLA WYSWIETLANIA OBRAZu
            while (true)
            {
                bool sukces = kamera.Read(klatka);

                if (!sukces || klatka.Empty())
                {
                    Console.WriteLine("Blad odczytu obrazu!");
                    break;
                }

                // Wyswietlenie aktualnej klatki
                Cv2.ImShow("Kamera", klatka);

                // Sprawdzenie nacisnietego klawisza
                int klawisz = Cv2.WaitKey(1);

                // Zakonczenie po nacisnieciu Q lub ESC
                if (klawisz == 'q' || klawisz == 'Q' || klawisz == 27)
                {
                    break;
                }
            }
        }
        finally
        {
            // Zamkniecie okien OpenCV
            Cv2.DestroyAllWindows();
        }
    }
}

Console.WriteLine("Program zakonczony.");
