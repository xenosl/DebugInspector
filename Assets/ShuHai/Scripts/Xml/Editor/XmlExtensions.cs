using System.Xml;
using System.Xml.Linq;

namespace ShuHai.Xml
{
    public static class XmlExtensions
    {
        public static XElement ToXElement(this XmlElement self)
        {
            return XElement.Load(self.CreateNavigator().ReadSubtree());
        }
    }
}