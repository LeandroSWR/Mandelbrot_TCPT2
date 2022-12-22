using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mandelbrot_TCPT2
{
    class StatsDisplay
    {
        private class AverageStats
        {
            public int Num { get; set; }
            public double Avg { get; set; }
            public string Size { get; set; }
            public bool IsTaskBased { get; }

            public AverageStats(bool isTask)
            {
                Num = 0;
                Avg = 0;
                Size = "";
                IsTaskBased = isTask;
            }

            public override string ToString()
            {
                string space = "";
                for (int i = 0; i < ((13 - (Avg.ToString("0.00") + " ms").Length) / 2); i++)
                    space += " ";

                return String.Format($"| {Size} |" +
                    $"{space}{Avg.ToString("0.00")} ms{space}");
            }
        }

        public static StatsDisplay Instance;

        AverageStats avgStatsTask;
        AverageStats avgStatsLinear;

        private string[] benchSprite =
        {
            "\t\t" + @"  ____                  _                          _    ",
            "\t\t" + @" |  _ \                | |                        | |   ",
            "\t\t" + @" | |_) | ___ _ __   ___| |__  _ __ ___   __ _ _ __| | __",
            "\t\t" + @" |  _ < / _ \ '_ \ / __| '_ \| '_ ` _ \ / _` | '__| |/ /",
            "\t\t" + @" | |_) |  __/ | | | (__| | | | | | | | | (_| | |  |   < ",
            "\t\t" + @" |____/ \___|_| |_|\___|_| |_|_| |_| |_|\__,_|_|  |_|\_\"
        };

        private string[] statsSprite =
        {
            " \t " + @"   _____                          _      _____ _        _       ",
            " \t " + @"  / ____|                        | |    / ____| |      | |      ",
            " \t " + @" | |    _   _ _ __ _ __ ___ _ __ | |_  | (___ | |_ __ _| |_ ___ ",
            " \t " + @" | |   | | | | '__| '__/ _ \ '_ \| __|  \___ \| __/ _` | __/ __|",
            " \t " + @" | |___| |_| | |  | | |  __/ | | | |_   ____) | || (_| | |_\__ \",
            " \t " + @"  \_____\__,_|_|  |_|  \___|_| |_|\__| |_____/ \__\__,_|\__|___/"
        };

        private string[] averageTables =
        {
            "|-------------------------|",
            "|                         |",
            "|-------------------------|",
            "|    Size   | AverageTime |",
            "|-------------------------|",
            "|           |             |",
            "|-------------------------|"
        };

        public StatsDisplay() 
        {
            Instance = this;
        }

        public void DisplayStats(List<MandelbrotStats> mStats, bool isBench = false)
        {
            Console.Clear();
            
            if (mStats == null)
            {
                Console.WriteLine("There are no stats to be displayed...\n");
                Console.WriteLine("Press Any Key to go back...");
                Console.ReadKey();
                return;
            }
            else
            {
                for (int i = 0; i < benchSprite.Length; i++)
                {
                    Console.WriteLine(isBench ? benchSprite[i] : statsSprite[i]);
                }

                SortStats(mStats);
                PrintLinearTable();
                PrintTaskTable();
                Console.SetCursorPosition(0, 15);
                if (isBench)
                    Console.WriteLine("All sets calculated during the benchmark:");
                else
                    Console.WriteLine("All sets calculated so far:");
                Console.WriteLine();

                for (int i = 0; i < mStats.Count; i++)
                {
                    Console.WriteLine(" - " + mStats[i].ToString());
                }
            }
        }

        private void PrintLinearTable(bool isBench = false)
        {
            // Print Empty Table
            for (int i = 0; i < averageTables.Length; i++)
            {
                Console.SetCursorPosition(2, 7 + i);
                Console.Write(averageTables[i]);
            }
            string title = "Mandelbrot Set Linear";
            string ms = avgStatsLinear.Avg.ToString("0.00") + " ms";

            Console.SetCursorPosition(3 + ((25 - title.Length) / 2), 8);
            Console.Write(title);
            Console.SetCursorPosition(2, 12);
            Console.Write(avgStatsLinear.ToString());
        }

        private void PrintTaskTable(bool isBench = false)
        {
            // Print Empty Table
            for (int i = 0; i < averageTables.Length; i++)
            {
                Console.SetCursorPosition(56, 7 + i);
                Console.Write(averageTables[i]);
            }
            string title = "Mandelbrot Set Parallel";
            string ms = avgStatsTask.Avg.ToString("0.00") + " ms";

            Console.SetCursorPosition(57 + ((26 - title.Length) / 2), 8);
            Console.Write(title);
            Console.SetCursorPosition(56, 12);
            Console.Write(avgStatsTask.ToString());
        }

        private void SortStats(List<MandelbrotStats> mStats)
        {
            avgStatsTask = new AverageStats(true);
            avgStatsLinear = new AverageStats(false);

            foreach (MandelbrotStats stat in mStats)
            {
                if (stat.IsTaskBased)
                {
                    avgStatsTask.Num++;
                    avgStatsTask.Size = stat.MRes;
                    avgStatsTask.Avg += stat.ETime;
                }
                else
                {
                    avgStatsLinear.Num++;
                    avgStatsLinear.Size = stat.MRes;
                    avgStatsLinear.Avg += stat.ETime;
                }
            }

            avgStatsTask.Avg /= avgStatsTask.Num;
            avgStatsLinear.Avg /= avgStatsLinear.Num;
        }
    }
}
