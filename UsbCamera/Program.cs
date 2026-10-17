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

    // 5. USTAWIENIA OBRAZU
    double jasnosc = 0;
    double kontrast = 1.0;
    bool odbicie = false;

    // Obiekty do zapisu i wykrywania ruchu
    MediaStorage zapis = new MediaStorage();

    using (Mat klatka = new Mat())
    {
        Cv2.NamedWindow("Kamera", WindowFlags.Normal);

        Console.WriteLine("Kamera uruchomiona!");
        Console.WriteLine();
        Console.WriteLine("STEROWANIE (kliknij okno z obrazem):");
        Console.WriteLine(" s - zdjecie        r - start/stop nagrywania");
        Console.WriteLine(" j / c - jasniej / ciemniej");
        Console.WriteLine(" k / l - wiekszy / mniejszy kontrast");
        Console.WriteLine(" f - odbicie lustrzane");
        Console.WriteLine(" q - wyjscie");
        Console.WriteLine();

        try
        {
            // 6. PETLA WYSWIETLANIA OBRAZU
            while (true)
            {
                bool sukces = kamera.Read(klatka);

                if (!sukces || klatka.Empty())
                {
                    Console.WriteLine("Blad odczytu obrazu!");
                    break;
                }

                // Zmiana jasnosci, kontrastu i odbicie
                Mat obraz = FrameEffects.Apply(klatka, kontrast, jasnosc, odbicie);

                // Zapis klatki do filmu
                if (zapis.IsRecording)
                {
                    zapis.WriteFrame(obraz);
                }


                // Wyswietlenie aktualnej klatki
                Cv2.ImShow("Kamera", obraz);


                // Sprawdzenie nacisnietego klawisza
                int klawisz = Cv2.WaitKey(1);
                bool koniec = false;

                // 7. OBSLUGA KLAWISZY
                if (klawisz == 'q' || klawisz == 'Q' || klawisz == 27)
                {
                    koniec = true;
                }
                else if (klawisz == 's')
                {
                    // Zrobienie zdjecia
                    try
                    {
                        string plik = zapis.SavePhoto(obraz);
                        Console.WriteLine("Zapisano zdjecie: " + plik);
                    }
                    catch (Exception blad)
                    {
                        Console.WriteLine(blad.Message);
                    }
                }
                else if (klawisz == 'r')
                {
                    // Start lub stop nagrywania
                    if (!zapis.IsRecording)
                    {
                        try
                        {
                            string plik = zapis.StartRecording(obraz.Size(), 20);
                            Console.WriteLine("Start nagrywania: " + plik);
                        }
                        catch (Exception blad)
                        {
                            Console.WriteLine(blad.Message);
                        }
                    }
                    else
                    {
                        zapis.StopRecording();
                        Console.WriteLine("Koniec nagrywania.");
                    }
                }
                else if (klawisz == 'j')
                {
                    // Jasniej
                    jasnosc = Math.Min(jasnosc + 10, 100);
                    Console.WriteLine("Jasnosc: " + jasnosc);
                }
                else if (klawisz == 'c')
                {
                    // Ciemniej
                    jasnosc = Math.Max(jasnosc - 10, -100);
                    Console.WriteLine("Jasnosc: " + jasnosc);
                }
                else if (klawisz == 'k')
                {
                    // Wiekszy kontrast
                    kontrast = Math.Min(kontrast + 0.1, 3.0);
                    Console.WriteLine("Kontrast: " + kontrast.ToString("0.0"));
                }
                else if (klawisz == 'l')
                {
                    // Mniejszy kontrast
                    kontrast = Math.Max(kontrast - 0.1, 0.5);
                    Console.WriteLine("Kontrast: " + kontrast.ToString("0.0"));
                }
                else if (klawisz == 'f')
                {
                    // Odbicie lustrzane
                    odbicie = !odbicie;
                }

                // Zwolnienie pamieci obrazu
                obraz.Dispose();

                // Zakonczenie po nacisnieciu Q lub ESC
                if (koniec)
                {
                    break;
                }
            }
        }
        finally
        {
            // Zakonczenie nagrywania i zapis filmu
            zapis.Dispose();

            // Zamkniecie okien OpenCV
            Cv2.DestroyAllWindows();
        }
    }
}

Console.WriteLine("Program zakonczony.");
