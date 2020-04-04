using System.Xml.Linq;

namespace Supertext.Base.IO.FileHandling
{
    internal class XmlFileHelper : IXmlFileHelper
    {
        public XElement LoadAsXElement(string filePath)
        {
            return XElement.Load(filePath);
        }
    }
}
