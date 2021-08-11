using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DangerZone.Droid.Models
{
    public class StateDetail
    {
        public string State { get; set; }
        public int Cases { get; set; }
        public int TodayCases { get; set; }
        public int Deaths { get; set; }
        public int TodayDeaths { get; set; }
        public int Recovered { get; set; }
        public int Active { get; set; }
        public int CasesPerOneMillion { get; set; }
        public int DeathsPerOneMillion { get; set; }
        public int Tests { get; set; }
        public int TestsPerOneMillion { get; set; }
        public int Population { get; set; }
    }
}