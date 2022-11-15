using System.Collections.Generic;

namespace GRP.DataAccess.Xml
{
    public class XmlNodeData
    {
        public string NodeName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public XmlNodeData()
        {
            this.Attributes = new Dictionary<string, string>();
        }
        public XmlNodeData(string NodeName)
        {
            this.NodeName = NodeName;
            this.Attributes = new Dictionary<string, string>();
        }
    }
}