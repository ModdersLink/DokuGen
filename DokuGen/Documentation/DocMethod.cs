using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DokuGen.Documentation
{
    class DocMethod : DokuFormattable
    {
        public string ParentFullName { get; set; }

        private List<DocParameter> m_Parameters;

        public DocMethod(XmlNode p_Method)
        {
            m_Parameters = new List<DocParameter>();

            var s_Split = p_Method.Attributes["name"].Value.Split(':');
            if (s_Split.Length < 2)
                throw new InvalidDataException("Could not parse the name.");

            FullPath = s_Split[1];

            var s_ParameterIndex = FullPath.IndexOf('(');
            var s_HasParameters = s_ParameterIndex != -1;

            if (s_HasParameters)
            {
                var s_NameSplit = FullPath.Substring(0, s_ParameterIndex);

                var s_NameIndex = s_NameSplit.LastIndexOf('.');
                if (s_NameIndex == -1)
                    throw new InvalidDataException("Could not get the name.");

                ParentFullName = s_NameSplit.Substring(0, s_NameIndex);
                var s_Name = s_NameSplit.Substring(s_NameIndex + 1);
                if (s_Name == "#ctor")
                {
                    var s_ParentIndex = ParentFullName.LastIndexOf('.');
                    s_Name = ParentFullName.Substring(s_ParentIndex + 1);
                }

                Name = s_Name;
                var s_Parameters = p_Method.SelectNodes("param");
                foreach (XmlNode l_Parameter in s_Parameters)
                    m_Parameters.Add(new DocParameter(l_Parameter));
            }
            else
            {
                var s_NameIndex = FullPath.LastIndexOf('.');
                if (s_NameIndex == -1)
                    throw new InvalidDataException("Could not get the name.");

                ParentFullName = FullPath.Substring(0, s_NameIndex);
                var s_Name = FullPath.Substring(s_NameIndex + 1);
                if (s_Name == "#ctor")
                {
                    var s_ParentIndex = ParentFullName.LastIndexOf('.');
                    s_Name = ParentFullName.Substring(s_ParentIndex + 1);
                }

                Name = s_Name;
            }

            Summary = p_Method.SelectSingleNode("summary")?.InnerText.Trim();

            
        }

        public override string Serialize(int p_HeaderLevel)
        {
            return "";

            //throw new NotImplementedException();
        }

        public DocParameter[] GetDocParameters()
        {
            return m_Parameters.ToArray();
        }
    }
}
