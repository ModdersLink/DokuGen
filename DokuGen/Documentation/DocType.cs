using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DokuGen.Documentation
{
    class DocType : DokuFormattable
    {
        private List<DocType> m_Types;
        private List<DocMethod> m_Methods;
        private List<DocProperty> m_Properties;
        private List<DocEvent> m_Events;
        private List<DocField> m_Fields;

        public string Namespace { get; set; }
        public DocType(XmlNode p_Type)
        {
            m_Types = new List<DocType>();
            m_Methods = new List<DocMethod>();
            m_Properties = new List<DocProperty>();
            m_Events = new List<DocEvent>();
            m_Fields = new List<DocField>();

            var s_Split = p_Type.Attributes["name"].Value.Split(':');
            if (s_Split.Length < 2)
                throw new InvalidDataException("Could not parse the name.");

            FullPath = s_Split[1];
            
            var s_NamespaceIndex = FullPath.LastIndexOf('.');
            if (s_NamespaceIndex == -1)
                throw new InvalidDataException("Could not get the full path.");

            Namespace = FullPath.Substring(0, s_NamespaceIndex);
            Name = FullPath.Substring(s_NamespaceIndex + 1);

            Summary = p_Type.SelectSingleNode("summary")?.InnerText.Trim();
        }

        public void AddType(DocType p_Type)
        {
            m_Types.Add(p_Type);
        }

        public void AddMethod(DocMethod p_Method)
        {
            m_Methods.Add(p_Method);
        }

        public void AddProperty(DocProperty p_Property)
        {
            m_Properties.Add(p_Property);
        }

        public void AddEvent(DocEvent p_Event)
        {
            m_Events.Add(p_Event);
        }

        public void AddField(DocField p_Field)
        {
            m_Fields.Add(p_Field);
        }

        public override string Serialize(int p_HeaderLevel)
        {
            var s_Final = string.Empty;

            using (var s_Writer = new StringWriter())
            {
                // Create the header level
                var s_TitleHeader = new string('=', p_HeaderLevel);

                // Write out the name header
                s_Writer.WriteLine($"{s_TitleHeader} {Name} {s_TitleHeader}");
                s_Writer.WriteLine();

                s_Writer.WriteLine($"''{FullPath}''");

                // Write the summary
                s_Writer.WriteLine($"<file>{Summary}</file>");
                s_Writer.WriteLine();

                // Write a horizontal line
                s_Writer.WriteLine("----");
                s_Writer.WriteLine();

                // Create and write the types header
                var s_SubHeader = new string('=', (p_HeaderLevel > 1 ? p_HeaderLevel - 1 : 1));
                
                // Write types
                s_Writer.WriteLine($"{s_SubHeader} Types {s_SubHeader}");
                foreach (var l_Type in m_Types)
                    s_Writer.WriteLine(l_Type.Serialize((p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));

                s_Writer.WriteLine();

                // Write properties
                s_Writer.WriteLine($"{s_SubHeader} Properties {s_SubHeader}");
                foreach (var l_Property in m_Properties)
                    s_Writer.WriteLine(l_Property.Serialize((p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));

                s_Writer.WriteLine();

                // Write events
                s_Writer.WriteLine($"{s_SubHeader} Events {s_SubHeader}");
                foreach (var l_Event in m_Events)
                    s_Writer.WriteLine(l_Event.Serialize((p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));

                s_Writer.WriteLine();

                // Write fields
                s_Writer.WriteLine($"{s_SubHeader} Fields {s_SubHeader}");
                foreach (var l_Field in m_Fields)
                    s_Writer.WriteLine(l_Field.Serialize((p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));

                s_Writer.WriteLine();

                // Write methods
                s_Writer.WriteLine($"{s_SubHeader} Methods {s_SubHeader}");
                foreach (var l_Method in m_Methods)
                    s_Writer.WriteLine(l_Method.Serialize((p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));

                // Leave some space
                s_Writer.WriteLine();

                s_Writer.Flush();
                s_Final = s_Writer.ToString();
            }

            return s_Final;
        }
    }
}
