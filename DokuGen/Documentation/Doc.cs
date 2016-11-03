using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace DokuGen.Documentation
{
    class Doc
    {
        public string AssemblyName { get; private set; }

        private List<DocType> m_Types;
        private List<DocField> m_Fields;
        private List<DocMethod> m_Methods;
        private List<DocProperty> m_Properties;
        private List<DocEvent> m_Events;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_DocumentNode"></param>
        public Doc(XmlNode p_DocumentNode)
        {
            m_Types = new List<DocType>();
            m_Fields = new List<DocField>();
            m_Methods = new List<DocMethod>();
            m_Properties = new List<DocProperty>();
            m_Events = new List<DocEvent>();

            // Get the assembly name
            AssemblyName = ParseAssemblyName(p_DocumentNode);

            // Get all of the members that are in the documentation file
            var s_Members = p_DocumentNode.SelectSingleNode("members");
            
            // Parse all of the members
            if (!ParseMembers(s_Members))
            {
                Console.WriteLine("Could not find any assembly members.");
                return;
            }

            Console.WriteLine($"Parsed {s_Members.ChildNodes.Count} members.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_MembersNode"></param>
        /// <returns></returns>
        private bool ParseMembers(XmlNode p_MembersNode)
        {
            var s_Members = p_MembersNode.SelectNodes("member");

            foreach (XmlNode l_Member in s_Members)
            {
                var l_MemberName = l_Member.Attributes["name"]?.Value;

                if (!ParseMember(l_Member))
                {
                    Console.WriteLine($"Could not parse member {l_MemberName}");
                    continue;
                }

                Console.WriteLine($"Parsed member {l_MemberName}");
            }

            return true;
        }

        private bool ParseMember(XmlNode p_Member)
        {
            var s_Split = p_Member.Attributes["name"].Value.Split(':');
            if (s_Split.Length < 2)
                return false;

            var s_Type = s_Split[0];

            switch (s_Type)
            {
                case "T": // Type
                    {
                        var s_CreatedType = CreateType(p_Member);
                        if (s_CreatedType == null)
                            return false;

                        m_Types.Add(s_CreatedType);
                        break;
                    }
                case "M": // Method
                    {
                        var s_CreatedMethod = CreateMethod(p_Member);
                        if (s_CreatedMethod == null)
                            return false;

                        m_Methods.Add(s_CreatedMethod);
                        break;
                    }
                case "P": // Property
                    {
                        var s_CreatedProperty = CreateProperty(p_Member);
                        if (s_CreatedProperty == null)
                            return false;

                        m_Properties.Add(s_CreatedProperty);
                        break;
                    }
                case "E": // Event
                    {
                        var s_CreatedEvent = CreateEvent(p_Member);
                        if (s_CreatedEvent == null)
                            return false;

                        m_Events.Add(s_CreatedEvent);
                        break;
                    }
                case "F": // Field
                    {
                        var s_CreatedField = CreateField(p_Member);
                        if (s_CreatedField == null)
                            return false;


                        m_Fields.Add(s_CreatedField);
                        break;
                    }
            }

            return true;
        }

        private DocType CreateType(XmlNode p_Type)
        {
            DocType s_Type = null;

            try
            {
                s_Type = new DocType(p_Type);
            }
            catch (InvalidDataException p_Exception)
            {
                Console.WriteLine($"CreateType: {p_Exception}");
                return null;
            }

            return s_Type;
        }

        private DocMethod CreateMethod(XmlNode p_Method)
        {
            DocMethod s_Method = null;

            try
            {
                s_Method = new DocMethod(p_Method);
            }
            catch (InvalidDataException p_Exception)
            {
                Console.WriteLine($"CreateType: {p_Exception}");
                return null;
            }

            return s_Method;
        }

        private DocProperty CreateProperty(XmlNode p_Property)
        {
            DocProperty s_Property = null;

            try
            {
                s_Property = new DocProperty(p_Property);
            }
            catch (InvalidDataException p_Exception)
            {
                Console.WriteLine($"CreateType: {p_Exception}");
                return null;
            }

            return s_Property;
        }

        private DocEvent CreateEvent(XmlNode p_Event)
        {
            DocEvent s_Event = null;

            try
            {
                s_Event = new DocEvent(p_Event);
            }
            catch (InvalidDataException p_Exception)
            {
                Console.WriteLine($"CreateType: {p_Exception}");
                return null;
            }

            return s_Event;
        }

        private DocField CreateField(XmlNode p_Field)
        {
            DocField s_Field = null;

            try
            {
                s_Field = new DocField(p_Field);
            }
            catch (InvalidDataException p_Exception)
            {
                Console.WriteLine($"CreateType: {p_Exception}");
                return null;
            }

            return s_Field;
        }

        private string ParseAssemblyName(XmlNode p_DocumentNode)
        {
            var s_Assembly = p_DocumentNode.SelectSingleNode("assembly");

            if (s_Assembly == null)
                return string.Empty;

            return s_Assembly.SelectSingleNode("name")?.InnerText;
        }

        public string[] GetNamespaces()
        {
            return m_Types.Select(p_Type => p_Type.Namespace.Replace('.', Path.AltDirectorySeparatorChar)).Distinct().ToArray();
        }

        public DocType[] GetTypes()
        {
            return m_Types.ToArray();
        }

        public DocType GetDocType(string p_SearchType)
        {
            return m_Types.FirstOrDefault(p_Type => string.Compare(p_Type.FullPath, p_SearchType, true) == 0);
        }

        public DocMethod GetDocMethod(string p_SearchMethod)
        {
            return m_Methods.FirstOrDefault(p_Method => p_Method.FullPath.StartsWith(p_SearchMethod, true, CultureInfo.CurrentCulture));
        }

        public DocProperty GetDocProperty(string p_SearchProperty)
        {
            return m_Properties.FirstOrDefault(p_Property => p_Property.FullPath.StartsWith(p_SearchProperty, true, CultureInfo.CurrentCulture));
        }

        public DocEvent GetDocEvent(string p_SearchEvent)
        {
            return m_Events.FirstOrDefault(p_Event => p_Event.FullPath.StartsWith(p_SearchEvent, true, CultureInfo.CurrentCulture));
        }
    }
}
