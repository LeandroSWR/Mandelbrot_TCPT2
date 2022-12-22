using Newtonsoft.Json;

namespace Mandelbrot_TCPT2
{
    internal class Mandelbrot
    {
        private const int width = 1024;
        private const int height = 1024;
        
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

        public void CalculateMandelParallel()
        {
            MandelbrotCalcParallel parallelCalc = new MandelbrotCalcParallel(width, height, 1000, 1.35f);
            long time = parallelCalc.RenderMandelbrotSet();

            Console.WriteLine("Parallel Mandelbrot calculations took {0} ms", time.ToString("0.00"));

            // Save Data
            MandelbrotStats stat =
                new MandelbrotStats(time, string.Format($"{width}x{height}"), true);
            MStats.Add(stat);
            benchStats.Add(stat);

            SaveNewData();
        }

        public void CalculateMandelLinear()
        {
            MandelbrotCalc linearCalc = new MandelbrotCalc(width, height, 1000, 1.35f);
            long time = linearCalc.RenderMandelbrotSet();

            Console.WriteLine("Linear Mandelbrot calculations took {0} ms", time.ToString("0.00"));

            // Save Data
            MandelbrotStats stat =
                new MandelbrotStats(time, string.Format($"{width}x{height}"), false);
            MStats.Add(stat);
            benchStats.Add(stat);

            SaveNewData();
        }

        public void RunBenchMark()
        {
            benchStats.Clear();

            for (int i = 0; i < 3; i++)
            {
                CalculateMandelLinear();
                CalculateMandelParallel();
            }

            StatsDisplay.Instance.DisplayStats(benchStats, true);

            benchStats.Clear();
        }
    }
}
