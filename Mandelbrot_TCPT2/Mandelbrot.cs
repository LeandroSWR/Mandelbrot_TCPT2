using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Mandelbrot_TCPT2
{
    internal class Mandelbrot
    {
        private const int width = 4096;
        private const int height = 4096;
        
        public List<MandelbrotStats> MStats { get; set; }
        private List<MandelbrotStats> benchStats;

        public Mandelbrot()
        {
            MStats = new List<MandelbrotStats>();
            benchStats = new List<MandelbrotStats>();

            CheckForPreviousStats();
        }

        private void CheckForPreviousStats()
        {
            // Check to update stats
            if (File.Exists("mStats.json"))
            {
                UpdateMandelbrotStats();
            }
        }
        
        private void UpdateMandelbrotStats()
        {
            using (StreamReader f = new StreamReader("mStats.json"))
            {
                string jsonString = f.ReadToEnd();
                MStats = JsonConvert.DeserializeObject<List<MandelbrotStats>>(jsonString);
            }
        }

        private void SaveNewData()
        {
            string jsonString = JsonConvert.SerializeObject(MStats);
            File.WriteAllText("mStats.json", jsonString);
        }

        public void CalculateMandelParallel(bool isBench = false)
        {
            long time = 0;
            Bitmap bmp = new Bitmap(width, height);
            MandelbrotCalcParallel parallelCalc = new MandelbrotCalcParallel(width, height, 1000, 1.35f);
            parallelCalc.RenderMandelbrotSet(ref time, ref bmp);

            HandleResult(time, bmp, "Parallel", isBench);
        }

        public void CalculateMandelLinear(bool isBench = false)
        {
            long time = 0;
            Bitmap bmp = new Bitmap(width, height);
            MandelbrotCalc linearCalc = new MandelbrotCalc(width, height, 1000, 1.35f);
            linearCalc.RenderMandelbrotSet(ref time, ref bmp);

            HandleResult(time, bmp, "Linear", isBench);
        }

        private void HandleResult(long time, Bitmap bmp, string type, bool isBench)
        {
            Console.WriteLine($"{type} Mandelbrot calculations took {time:0.00} ms");

            // Add the data to the stats list
            MandelbrotStats stat =
                new MandelbrotStats(time, string.Format($"{width}x{height}"), type == "Parallel");
            MStats.Add(stat);
            benchStats.Add(stat);

            if (!isBench)
            {
                DisplayMandelbrot(bmp, type);
            }

            // Save the new Data
            SaveNewData();
        }

        private void DisplayMandelbrot(Bitmap bmp, string name)
        {
            if (bmp != null)
            {
                // Save the image to a file and open the file using the default image viewer
                bmp.Save($"{name}.png", System.Drawing.Imaging.ImageFormat.Png);
                Process.Start(new ProcessStartInfo(@$"{name}.png") { UseShellExecute = true });
            }
        }

        public void RunBenchMark()
        {
            benchStats.Clear();

            for (int i = 0; i < 3; i++)
            {
                // `!(i==2)` Makes sure we only render the last image from the benchmark
                CalculateMandelLinear(!(i == 2));
                CalculateMandelParallel(!(i == 2));
            }

            StatsDisplay.Instance.DisplayStats(benchStats, true);

            // Get the handle of the console window
            var handle = GetConsoleWindow();

            // Bring the console window to the front
            SetForegroundWindow(handle);

            benchStats.Clear();
        }

        // Import nessessary dll to brind the console into focuse when the benchmark is over
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();
    }
}
