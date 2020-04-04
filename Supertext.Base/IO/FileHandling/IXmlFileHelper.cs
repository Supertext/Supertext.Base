using System.Xml.Linq;

namespace Supertext.Base.IO.FileHandling
{
    public interface IXmlFileHelper
    {
        XElement LoadAsXElement(string filePath);
    }
}