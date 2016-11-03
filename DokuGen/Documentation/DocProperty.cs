using System.IO;
using System.Xml;

namespace DokuGen.Documentation
{
    class DocProperty : DokuFormattable
    {
        public string ParentFullName { get; set; }
        public DocProperty(XmlNode p_Property)
        {
            var s_Split = p_Property.Attributes["name"].Value.Split(':');
            if (s_Split.Length < 2)
                throw new InvalidDataException("Could not parse the name.");

            FullPath = s_Split[1];

            var s_NameIndex = FullPath.LastIndexOf('.');
            if (s_NameIndex == -1)
                throw new InvalidDataException("Could not get full name.");

            Name = FullPath.Substring(s_NameIndex + 1);
            ParentFullName = FullPath.Substring(0, s_NameIndex);

            Summary = p_Property.SelectSingleNode("summary")?.InnerText.Trim();
        }

        public override string Serialize(int p_HeaderLevel)
        {
            var s_Final = string.Empty;

            using (var s_Writer = new StringWriter())
            {
                var s_Header = new string('=', p_HeaderLevel);

                s_Writer.WriteLine($"{s_Header} {Name} {s_Header}");
                s_Writer.WriteLine();

                s_Writer.WriteLine($"''{FullPath}''");

                s_Writer.WriteLine();

                s_Writer.WriteLine($"<file>{Summary}</file>");

                s_Writer.WriteLine();

                s_Writer.Flush();

                s_Final = s_Writer.ToString();
            }

            return s_Final;
        }
    }
}
