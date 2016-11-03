using System.IO;
using System.Xml;

namespace DokuGen.Documentation
{
    class DocField : DokuFormattable
    {
        public string ParentFullName { get; set; }

        public DocField(XmlNode p_Field)
        {
            var s_Split = p_Field.Attributes["name"].Value.Split(':');
            if (s_Split.Length < 2)
                throw new InvalidDataException("Could not parse the name.");

            FullPath = s_Split[1];

            var s_NameIndex = FullPath.LastIndexOf('.');
            if (s_NameIndex == -1)
                throw new InvalidDataException("Could not parse the name.");

            ParentFullName = FullPath.Substring(0, s_NameIndex);

            Name = FullPath.Substring(s_NameIndex + 1);

            Summary = p_Field.SelectSingleNode("summary")?.InnerText.Trim();
        }

        public override string Serialize(int p_HeaderLevel)
        {
            return string.Empty;

            //throw new NotImplementedException();
        }
    }
}
