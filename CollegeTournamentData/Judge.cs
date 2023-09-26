using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace CollegeTournamentData
{
    public class Judge
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public Guid Guid { get; set; } = Guid.NewGuid();

        public List<float> Speaks { get; set; } = new();

        public float GetAvg()
        {
            if (Speaks.Count == 0)
                return 0f;

            return (float)Math.Round(Speaks.Average(), 2);
        }

        public float GetStDev()
        {
            if (Speaks.Count == 0)
                return 0f;

            return (float)Math.Sqrt(Speaks.Average(v => Math.Pow(v - GetAvg(), 2)));
        }

        public float GetMin()
        {
            if (Speaks.Count == 0)
                return 0f;

            return Speaks.Min();
        }

        public float GetMax()
        {
            if (Speaks.Count == 0)
                return 0f;

            return Speaks.Max();
        }
    }
}
