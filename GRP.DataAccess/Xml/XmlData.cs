using System.Collections.Generic;
using System.Xml;

namespace GRP.DataAccess.Xml
{
    public static class XmlData
    {
        public static string CreateXmlDocument(string RootNodeName, List<XmlNodeData> ChildNodes)
        {
            string retVal = string.Empty;

            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement(RootNodeName);
            xml.AppendChild(root);

            XmlElement child = null;

            foreach (XmlNodeData n in ChildNodes)
            {
                child = xml.CreateElement(n.NodeName);
                foreach (string key in n.Attributes.Keys)
                {
                    if (!string.IsNullOrEmpty(n.Attributes[key]))           // small enhancement to exclude empty attributes.
                        child.SetAttribute(key, n.Attributes[key]);
                }

                root.AppendChild(child);
            }

            return xml.OuterXml;
        }
    }

    //var document = XDocument.Parse(original);
    //document.Descendants()
    //    .Where(e => e.IsEmpty || String.IsNullOrWhiteSpace(e.Value))
    //    .Remove();
}
