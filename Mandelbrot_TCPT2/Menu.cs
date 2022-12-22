using System;
using System.Text.Json;
using System.IO;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Mandelbrot_TCPT2
{
    public class Menu
    {
        private Mandelbrot mandelbrot;

        private string[] titleSprite =
        {
            @"  __  __                 _      _ _               _",
            @" |  \/  |               | |    | | |             | |",
            @" | \  / | __ _ _ __   __| | ___| | |__  _ __ ___ | |_",
            @" | |\/| |/ _` | '_ \ / _` |/ _ \ | '_ \| '__/ _ \| __|",
            @" | |  | | (_| | | | | (_| |  __/ | |_) | | | (_) | |_",
            @" |_|  |_|\__,_|_| |_|\__,_|\___|_|_.__/|_|  \___/ \__|"
        };

        private string[] menuBox =
        {
            "\t    |-----------------------------|",
            "\t    |\t\t\t\t  |",
            "\t    |      1 - Run Benchmark      |",
            "\t    |\t\t\t\t  |",
            "\t    |      2 - Parallel Only      |",
            "\t    |\t\t\t\t  |",
            "\t    |      3 - Linear Only        |",
            "\t    |\t\t\t\t  |",
            "\t    |      4 - GPU Mandelbrot     |",
            "\t    |\t\t\t\t  |",
            "\t    |      5 - Check Stats        |",
            "\t    |\t\t\t\t  |",
            "\t    |      6 - Exit               |",
            "\t    |-----------------------------|",
        };

        public Menu() 
        {
            mandelbrot = new Mandelbrot();
            // Initizlises the singleton in the StatsDisplay class
            new StatsDisplay();
        }

        public void DrawMenu()
        {
            // Make sure console is empty before drawing the menu
            Console.Clear();

            for (int i = 0; i < titleSprite.Length; i++)
            {
                Console.WriteLine(titleSprite[i]);
            }
            Console.WriteLine("\n");
            for (int i = 0; i < menuBox.Length; i++)
            {
                Console.WriteLine(menuBox[i]);
            }

            int choice;
            char c;
            // Lock until the user chooses a valid option
            do
            {
                c = Console.ReadKey().KeyChar;
            } while (
                !int.TryParse(c.ToString(), out choice) &&
                choice < 1 && 
                choice > 5);

            CheckChoice(choice);
        }

        private void CheckChoice(int selection)
        {
            Console.Clear();

            switch (selection)
            {
                default:
                case 1:
                    mandelbrot.RunBenchMark();
                    break;
                case 2:
                    mandelbrot.CalculateMandelParallel();
                    break;
                case 3:
                    mandelbrot.CalculateMandelLinear();
                    break;
                case 4:
                    Window window = new Window();
                    window.Run();
                    break;
                case 5:
                    StatsDisplay.Instance.DisplayStats(mandelbrot.MStats);
                    break;
                case 6:
                    Environment.Exit(0);
                    break;
            }

            Console.WriteLine("\nPress Any Key to go back to the Menu...");
            Console.ReadKey();
            DrawMenu();
        }
    }
}