using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace YahooSports.Api.Sports.Models
{
    [XmlRoot(ElementName = "fantasy_content", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class FantasyContent
    {
        [XmlElement("game")]
        public Game Game { get; set; }

        [XmlElement("player")]
        public Player Player { get; set; }

        [XmlElement("league")]
        public League League { get; set; }

        [XmlElement("team")]
        public Team Team { get; set; }
    }
}
