using System.Xml.Serialization;

namespace YahooSports.Api.Sports.Models
{
    [XmlRoot("team")]
    public class Team
    {
        #region Nested Team-Specific Classes

        [XmlRoot("roster")]
        public class Roster
        {

            #region Nested Roster-Specific Classes

            //[XmlRoot("player")]
            public class TeamPlayer : Player
            {

                #region Nested Player-Specific Structs

                [XmlRoot("starting_status")]
                public struct StartingStatus
                {
                    [XmlElement("coverage_type")]
                    public string CoverageType { get; set; }

                    [XmlElement("date")]
                    public string Date { get; set; }

                    [XmlElement("season")]
                    public string Season { get; set; }

                    [XmlElement("week")]
                    public string Week { get; set; }

                    [XmlElement("is_starting")]
                    public bool IsStarting { get; set; }
                }

                [XmlRoot("selected_position")]
                public struct SelectedPosition
                {
                    [XmlElement("coverage_type")]
                    public string CoverageType { get; set; }

                    [XmlElement("date")]
                    public string Date { get; set; }

                    [XmlElement("season")]
                    public string Season { get; set; }

                    [XmlElement("week")]
                    public string Week { get; set; }

                    [XmlElement("position")]
                    public string Position { get; set; }
                }

                #endregion

                [XmlElement("starting_status")]
                public StartingStatus PlayerStartingStatus { get; set; }

                [XmlElement("selected_position")]
                public SelectedPosition PlayerPosition { get; set; }
            }

            #endregion

            [XmlElement("date")]
            public string Date { get; set; }

            [XmlElement("week")]
            public string Week { get; set; }

            [XmlElement("season")]
            public string Season { get; set; }

            [XmlElement("coverage_type")]
            public string CoverageType { get; set; }

            [XmlArray("players")]
            [XmlArrayItem("player")]
            public TeamPlayer[] Players { get; set; }

        }

        #endregion

        #region Nested Team Structs

        [XmlRoot("team_points")]
        public struct TeamPoints
        {
            [XmlElement("coverage_type")]
            public string CoverageType { get; set; }

            [XmlElement("week")]
            public string Week { get; set; }
            [XmlElement("date")]
            public string Date { get; set; }
            [XmlElement("season")]
            public string Season { get; set; }
        }

        [XmlRoot("team_logo")]
        public struct Logo
        {
            [XmlElement("size")]
            public string Size { get; set; }

            [XmlElement("url")]
            public string LogoUrl { get; set; }
        }

        [XmlRoot("manager")]
        public struct Manager
        {
            [XmlElement("manager_id")]
            public int ManagerId { get; set; }

            [XmlElement("nickname")]
            public string Nickname { get; set; }

            [XmlElement("guid")]
            public string Guid { get; set; }
        }

        [XmlRoot("team_stats")]
        public class TeamStats
        {
            #region Nested TeamStats-specific Structs

            [XmlRoot("stat")]
            public struct Stat
            {
                [XmlElement("stat_id")]
                public int Id { get; set; }

                [XmlElement("value")]
                public string Value { get; set; }
            }

            #endregion

            [XmlElement("coverage_type")]
            public string CoverageType { get; set; }

            [XmlElement("date")]
            public string Date { get; set; }
            [XmlElement("week")]
            public string Week { get; set; }
            [XmlElement("season")]
            public string Season { get; set; }

            [XmlArray("stats")]
            [XmlArrayItem("stat")]
            public Stat[] Stats { get; set; }
        }

        #endregion

        [XmlElement("team_key")]
        public string Key { get; set; }

        [XmlElement("team_id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("url")]
        public string TeamUrl { get; set; }

        [XmlArray("team_logos")]
        [XmlArrayItem("team_logo")]
        public Logo[] Logos { get; set; }

        [XmlElement("waiver_priority")]
        public int WaiverPriority { get; set; }

        [XmlElement("number_of_moves")]
        public int NumberOfMoves { get; set; }

        [XmlArray("managers")]
        [XmlArrayItem("manager")]
        public Manager[] Managers { get; set; }

        [XmlElement("roster")]
        public Roster TeamRoster { get; set; }

        [XmlElement("team_stats")]
        public TeamStats Stats { get; set; }

        [XmlElement("team_points")]
        public TeamPoints Points { get; set; }
    }
}
