using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace YahooSports.Api.Yql.Models
{
    [XmlRoot]
    public class Query<T>
    {
        [XmlElement]
        public T[] Results { get; set; }
    }
}
