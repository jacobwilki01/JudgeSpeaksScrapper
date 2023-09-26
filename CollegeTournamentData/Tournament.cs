using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeTournamentData
{
    public class Tournament
    {
        public List<Category> categories { get; set; } = new();

        public string? name { get; set; }
    }

    public class Category
    {
        public string? name { get; set; }

        public List<Event> events { get; set; } = new();
    }

    public class Event
    {
        public string? name { get; set; }

        public List<Round> rounds { get; set; } = new();
    }

    public class Round
    {
        public string? label { get; set; }

        public string? protocol_name { get; set; }

        public List<Section> sections { get; set; } = new();
    }

    public class Section
    {
        public string? round { get; set; }

        public List<Ballot> ballots { get; set; } = new();
    }

    public class Ballot
    {
        public string? judge_first { get; set; }

        public string? judge_last { get; set; }

        public List<Score> scores { get; set; } = new();
    }

    public class Score
    {
        public float? value { get; set; }

        public string? tag { get; set; }
    }
}
