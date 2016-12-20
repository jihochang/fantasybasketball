using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace YahooSports.Api.Yql.Models
{
    //[XmlRoot("player")]
    public class Player2
    {
        [XmlElement("player_key")]
        public string PlayerKey { get; set; }
    }
}
