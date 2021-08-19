namespace EpicorSwaggerRESTGenerator.Models
{
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2007/app")]
    public partial class ServiceWorkspaceCollection
    {
        [System.Xml.Serialization.XmlAttribute()]
        public string Href { get; set; }

        [System.Xml.Serialization.XmlText()]
        public string[] Text { get; set; }

        [System.Xml.Serialization.XmlElement(Namespace = "http://www.w3.org/2005/Atom")]
        public Title Title { get; set; }
    }
}
