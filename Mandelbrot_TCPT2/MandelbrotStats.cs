using Newtonsoft.Json;

namespace Mandelbrot_TCPT2
{
    internal class MandelbrotStats
    {
        [JsonProperty("ETime")]
        public double ETime { get; private set; }
        [JsonProperty("MRes")]
        public string MRes { get; private set; }
        [JsonProperty("IsTaskBased")]
        public bool IsTaskBased { get; private set; }

        public MandelbrotStats(double eTime, string mRes, bool taskBased)
        {
            ETime = eTime;
            MRes = mRes;
            IsTaskBased = taskBased;
        }

        public override string ToString()
        {
            return $"Size: {MRes}, Time: {ETime.ToString("0.00")} ms, IsTaskBased: {IsTaskBased}";
        }
    }
}
