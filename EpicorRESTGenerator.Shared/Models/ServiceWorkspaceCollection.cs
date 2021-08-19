using System.Xml.Serialization;

namespace EpicorRESTGenerator.Shared.Models
{
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2007/app")]
    public partial class ServiceWorkspaceCollection
    {
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }

        [XmlText()]
        public string[] Text { get; set; }

        [XmlElement("title", Namespace = "http://www.w3.org/2005/Atom")]
        public Title Title { get; set; }
    }
}
