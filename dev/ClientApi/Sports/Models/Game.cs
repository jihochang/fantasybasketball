
using System.Xml.Serialization;

namespace YahooSports.Api.Sports.Models
{
    [XmlRoot(ElementName = "game")]
    public class Game
    {
        [XmlElement(ElementName = "game_key")]
        public string Key { get; set; }
        [XmlElement(ElementName = "game_id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "code")]
        public string Code { get; set; }
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
        [XmlElement(ElementName = "season")]
        public string Season { get; set; }

        #region Sub-Resources

        //Leagues
        [XmlArray("leagues")]
        [XmlArrayItem("league")]
        public League[] Leagues { get; set; }

        //Players
        [XmlArray("players")]
        [XmlArrayItem("player")]
        public Player[] Players { get; set;}

        #endregion

        

    }
}
