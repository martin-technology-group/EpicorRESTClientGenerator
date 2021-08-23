namespace EpicorRESTGenerator.Shared.Models
{
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    [System.Xml.Serialization.XmlRoot(Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
    public partial class Title
    {
        [System.Xml.Serialization.XmlAttribute()]
        public string Type { get; set; }

        [System.Xml.Serialization.XmlText()]
        public string Value { get; set; }
    }
}
