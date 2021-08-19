namespace EpicorSwaggerRESTGenerator.Models
{
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2007/app")]
    public partial class ServiceWorkspace
    {
        [System.Xml.Serialization.XmlElement("collection")]
        public ServiceWorkspaceCollection[] Collection { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2005/Atom")]
        public Title Title { get; set; }
    }
}
