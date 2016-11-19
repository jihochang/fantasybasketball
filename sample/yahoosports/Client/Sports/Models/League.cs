using System.Xml.Serialization;

namespace YahooSports.Api.Sports.Models {
    [XmlRoot(ElementName = "league")]
    public class League
    {

        #region Nested League Classes

        [XmlRoot("settings")]
        public class Settings
        {

            #region Nested Settings Classes

            [XmlRoot("roster_position")]
            public class RosterPosition
            {
                [XmlElement("position")]
                public string Position { get; set; }
                [XmlElement("count")]
                public int Count { get; set; }
            }

            [XmlRoot("stat_categories")]
            public class StatCategory
            {
                #region Nested Stats Classes

                //[XmlRoot("stat")]
                public class LeagueStat
                {
                    [XmlElement("stat_id")]
                    public int Id { get; set; }
                    [XmlElement("enabled")]
                    public bool IsEnabled { get; set; }
                    [XmlElement("name")]
                    public string Name { get; set; }
                    [XmlElement("display_name")]
                    public string DisplayName { get; set; }
                    [XmlElement("stat_sort_orderid")]
                    public int SortOrder { get; set; }
                    [XmlElement("position_type")]
                    public string PositionType { get; set; }
                    [XmlElement("is_only_display_stat")]
                    public bool IsOnlyDisplayStat { get; set; }
                }

                #endregion

                [XmlArray("stats")]
                [XmlArrayItem("stat")]
                public LeagueStat[] Stats { get; set; }
            }

            #endregion

            [XmlElement("draft_type")]
            public string DraftType { get; set; }
            [XmlElement("scoring_type")]
            public string ScoringType { get; set; }
            [XmlElement("uses_playoff")]
            public bool UsesPlayoff { get; set; }
            [XmlElement("uses_faab")]
            public bool UsesFaab { get; set; }
            [XmlElement("trade_end_date")]
            public string TradeEndDate { get; set; }
            [XmlElement("trade_ratify_type")]
            public string TradeRatifyType { get; set; }
            [XmlElement("trade_reject_time")]
            public string TradeRejectTime { get; set; }
            [XmlArray("roster_positions")]
            [XmlArrayItem("roster_position")]
            public RosterPosition[] RosterPositions { get; set; }
            [XmlElement("stat_categories")]
            public StatCategory StatCategories { get; set; }

        }

        [XmlRoot("scoreboard")]
        public class Scoreboard
        {
            #region Nested Scoreboard Classes

            [XmlRoot("matchup")]
            public class Matchup
            {
                [XmlElement("week")]
                public int Week { get; set; }
                [XmlElement("status")]
                public string Status { get; set; }
                [XmlElement("is_tied")]
                public bool IsTied { get; set; }
                [XmlElement("winner_team_key")]
                public string WinnerTeamKey { get; set; }

                [XmlArray("teams")]
                [XmlArrayItem("team")]
                public Team[] Teams { get; set; }
            }

            #endregion

            [XmlElement("week")]
            public int Week { get; set; }
            [XmlArray("matchups")]
            [XmlArrayItem("matchup")]
            public Matchup[] Matchups { get; set; }
        }

        [XmlRoot("standings")]
        public class Standings
        {
            [XmlArray("teams")]
            [XmlArrayItem("team")]
            public Team[] Teams { get; set; }
        }

        [XmlRoot("transaction")]
        public class Transaction
        {
            [XmlElement("transaction_key")]
            public string Key { get; set; }
            [XmlElement("transaction_id")]
            public int Id { get; set; }
            [XmlElement("type")]
            public string Type { get; set; }
            [XmlElement("status")]
            public string Status { get; set; }
            [XmlElement("timestamp")]
            public long Timestamp { get; set; }

            [XmlArray("players")]
            [XmlArrayItem("player")]
            public Player[] Players { get; set; }
        }

        [XmlRoot("draft_result")]
        public class DraftResult
        {
            [XmlElement("pick")]
            public int Pick { get; set; }
            [XmlElement("round")]
            public int Round { get; set; }
            [XmlElement("team_key")]
            public string TeamKey { get; set; }
            [XmlElement("player_key")]
            public string PlayerKey { get; set; }
        }

        #endregion

        [XmlElement(ElementName = "league_key")]
        public string Key { get; set; }
        [XmlElement(ElementName = "league_id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
        [XmlElement(ElementName = "draft_status")]
        public string DraftStatus { get; set; }
        [XmlElement(ElementName = "num_teams")]
        public string NumTeams { get; set; }
        [XmlElement(ElementName = "edit_key")]
        public string EditKey { get; set; }

        [XmlElement(ElementName = "weekly_deadline")]
        public string WeeklyDeadline { get; set; }
        [XmlElement(ElementName = "league_update_timestamp")]
        public string LastUpdated { get; set; }
        [XmlElement(ElementName = "scoring_type")]
        public string ScoringType { get; set; }
        [XmlElement(ElementName = "current_week")]
        public string CurrentWeek { get; set; }
        [XmlElement(ElementName = "start_week")]
        public string StartWeek { get; set; }
        [XmlElement(ElementName = "end_week")]
        public string EndWeek { get; set; }
        [XmlElement(ElementName = "is_finished")]
        public bool IsFinished { get; set; }

        #region Sub-Resources

        [XmlElement("settings")]
        public Settings LeagueSettings { get; set; }

        [XmlElement("standings")]
        public Standings LeagueStandings { get; set; }

        [XmlArray("transactions")]
        [XmlArrayItem("transaction")]
        public Transaction[] Transactions { get; set; }

        [XmlArray("draft_results")]
        [XmlArrayItem("draft_result")]
        public DraftResult[] DraftResults { get; set; }

        [XmlElement("scoreboard")]
        public Scoreboard LeagueScoreboard { get; set; }

        #endregion

        
    }
}
