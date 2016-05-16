using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MongoDBToTridion.BAL.Model
{
    [DataContract]
    [Serializable]
    public class Article
    {
        [XmlElement("title")]
        [DataMember]
        public string title { get; set; }

        [XmlElement("description")]
        [DataMember]
        public string description { get; set; }

        [XmlElement("imageurl")]
        [DataMember]
        public string imageurl { get; set; }
    }
}
