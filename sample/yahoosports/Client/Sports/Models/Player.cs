using System.Xml.Serialization;

namespace YahooSports.Api.Sports.Models
{
    [XmlRoot(ElementName = "player")]
    public class Player
    {

        #region Nested Leage-Specific Classes

        [XmlRoot("transaction_data")]
        public class TransactionData
        {
            [XmlElement("type")]
            public string Type { get; set; }
            [XmlElement("source_type")]
            public string SourceType { get; set; }
            [XmlElement("destination_type")]
            public string DestinationType { get; set; }
            [XmlElement("destination_team_key")]
            public string DestinationTeamKey { get; set; }
        }

        #endregion  

        #region Nested Player Structs

        [XmlRoot("name")]
        public struct PlayerName
        {
            [XmlElement("full")]
            public string Full { get; set; }
            [XmlElement("first")]
            public string First { get; set; }
            [XmlElement("last")]
            public string Last { get; set; }
            [XmlElement("ascii_first")]
            public string AsciiFirst { get; set; }
            [XmlElement("ascii_last")]
            public string AsciiLast { get; set; }

            public override string ToString()
            {
                return Full;
            }
        }

        [XmlRoot("player_stats")]
        public class PlayerStats
        {
            #region Nested PlayerStats Structs

            //[XmlRoot("stat")]
            public struct PlayerStat
            {
                [XmlElement("stat_id")] public int Id;
                [XmlElement("value")] public string Value;
            }

            #endregion

            [XmlElement("coverage_type")]
            public string CoverageType { get; set; }
            [XmlElement("season")]
            public string Season { get; set; }
            [XmlElement("date")]
            public string Date { get; set; }
            [XmlElement("week")]
            public string Week { get; set; }
            [XmlArray("stats")] 
            [XmlArrayItem("stat")] 
            public PlayerStat[] Stats;

        }

        #endregion

        //Metadata
        [XmlElement("player_key")]
        public string Key { get; set; }
        [XmlElement("player_id")]
        public int Id { get; set; }
        [XmlElement("name")]
        public PlayerName Name { get; set; }
        [XmlElement("status")]
        public string Status { get; set; }
        [XmlElement("editorial_player_key")]
        public string EditorialPlayerKey { get; set; }
        [XmlElement("editorial_team_key")]
        public string EditorialTeamKey { get; set; }
        [XmlElement("editorial_team_full_name")]
        public string EditorialTeamFullName { get; set; }
        [XmlElement("editorial_team_abbr")]
        public string EditorialTeamAbbreviation { get; set; }
        [XmlArray("bye_weeks")]
        [XmlArrayItem("week")]
        public string[] Weeks { get; set; }
        [XmlElement("uniform_number")]
        public string UniformNumber { get; set; }
        [XmlElement("display_position")]
        public string DisplayPosition { get; set; }
        [XmlElement("image_url")]
        public string ImageUrl { get; set; }
        [XmlElement("is_undroppable")]
        public bool IsUndroppable { get; set; }
        [XmlArray("eligible_positions")]
        [XmlArrayItem("position")]
        public string[] Positions { get; set; }

        #region Sub-Resources

        [XmlElement("player_stats")]
        public PlayerStats PlayerStatistics { get; set; }

        #endregion


        #region League Specific: Player sub-resource

        [XmlElement("transaction_data")]
        public TransactionData TransactionInfo { get; set; }

        #endregion
    }

}
