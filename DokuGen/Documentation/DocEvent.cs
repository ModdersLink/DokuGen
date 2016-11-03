using System.IO;
using System.Xml;

namespace DokuGen.Documentation
{
    class DocEvent : DokuFormattable
    {
        public string ParentFullName { get; set; }
        public DocEvent(XmlNode p_Event)
        {
            var s_Split = p_Event.Attributes["name"].Value.Split(':');
            if (s_Split.Length < 2)
                throw new InvalidDataException("Could not parse the name.");

            FullPath = s_Split[1];

            var s_NameIndex = FullPath.LastIndexOf('.');
            if (s_NameIndex == -1)
                throw new InvalidDataException("Could not get full name.");

            Name = FullPath.Substring(s_NameIndex + 1);
            ParentFullName = FullPath.Substring(0, s_NameIndex);

            Summary = p_Event.SelectSingleNode("summary")?.InnerText.Trim();
        }

        public override string Serialize(int p_HeaderLevel)
        {
            return string.Empty;

            //throw new NotImplementedException();
        }
    }
}
