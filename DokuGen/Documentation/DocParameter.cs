using System.Xml;

namespace DokuGen.Documentation
{
    /// <summary>
    /// FullPath is not used here
    /// </summary>
    class DocParameter : DokuFormattable
    {
        public DocParameter(XmlNode p_Parameter)
        {
            Name = p_Parameter.Attributes["name"].Value;
            Summary = p_Parameter.InnerText;
        }

        public override string Serialize(int p_HeaderLevel)
        {
            return "";

            //throw new NotImplementedException();
        }
    }
}
