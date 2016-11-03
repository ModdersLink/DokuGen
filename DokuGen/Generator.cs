using DokuGen.Documentation;
using DokuGen.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DokuGen
{
    class Generator
    {
        public string InputDirectory { get; set; } = string.Empty;
        public string OutputDirectory { get; set; } = string.Empty;

        private Reflector m_Reflector;
        private List<Doc> m_Docs;

        private const string c_Public = "public";
        private const string c_Private = "private";
        private const string c_Protected = "protected";
        private const string c_Interal = "internal";

        public bool Read()
        {
            if (string.IsNullOrWhiteSpace(InputDirectory))
                return false;

            if (!Directory.Exists(InputDirectory))
                return false;

            m_Reflector = new Reflector(InputDirectory);

            if (!m_Reflector.LoadAssemblies())
            {
                Console.WriteLine("Could not load assemblies.");
                return false;
            }

            // TODO: Refactor this to handle shit
            m_Docs = new List<Doc>();

            foreach (var l_XmlPath in Directory.GetFiles(InputDirectory, "*.xml", SearchOption.TopDirectoryOnly))
            {
                if (!ParseAssemblyDocumentation(l_XmlPath))
                    Console.WriteLine($"Could not parse xml documentation from path {l_XmlPath}.");
            }

            CreateDocumentation();

            return true;
        }

        private void CreateDocumentation()
        {
            var s_Types = m_Reflector.Types;

            foreach (var l_Pair in s_Types)
            {
                var l_FullName = l_Pair.Key;

                if (l_FullName.Contains("<") || l_FullName.Contains(">"))
                    continue;

                var l_Path = l_Pair.Key.Replace('.', '/').ToLower() + ".txt";
                var l_FinalPath = Path.Combine(OutputDirectory, l_Path);
                var l_Directory = Path.GetDirectoryName(l_FinalPath);

                if (!Directory.Exists(l_Directory))
                    Directory.CreateDirectory(l_Directory);

                var s_Data = PrintType(l_Pair.Value, 6);
                File.WriteAllText(l_FinalPath, s_Data);
            }
        }

        private string PrintProperty(Type p_Parent, PropertyInfo p_Property, int p_HeaderLevel)
        {
            var s_Doc = m_Docs.FirstOrDefault(p_Doc => p_Doc.AssemblyName == p_Property.DeclaringType.Assembly.GetName().Name);

            var s_DocProperty = s_Doc?.GetDocProperty($"{p_Property.ReflectedType.FullName}.{p_Property.Name}");

            using (var s_Writer = new StringWriter())
            {
                // Create the header level
                var s_TitleHeader = new string('=', p_HeaderLevel);

                // Write out the name header
                s_Writer.WriteLine($"{s_TitleHeader} {p_Property.Name} {s_TitleHeader}");
                s_Writer.WriteLine();

                s_Writer.WriteLine($"''{s_DocProperty?.FullPath}''");

                s_Writer.WriteLine($"<code>{s_DocProperty?.Summary}</code>");
                s_Writer.WriteLine();

                var s_Formatted = $"{p_Property.PropertyType.Name} {p_Property.Name}";

                var s_GetMethod = p_Property.GetMethod;
                var s_GetString = string.Empty;

                var s_SetMethod = p_Property.SetMethod;
                var s_SetString = string.Empty;

                if (s_GetMethod != null)
                {
                    var s_Modifier = string.Empty;

                    if (s_GetMethod.IsPublic)
                        s_Modifier = c_Public;
                    else if (s_GetMethod.IsPrivate)
                        s_Modifier = c_Private;

                    s_GetString = $"{s_Modifier} get;";
                }

                if (s_SetMethod != null)
                {
                    var s_Modifier = string.Empty;

                    if (s_SetMethod.IsPublic)
                        s_Modifier = c_Public;
                    else if (s_SetMethod.IsPrivate)
                        s_Modifier = c_Private;

                    s_SetString = $"{s_Modifier} set;";
                }

                if (s_GetMethod == null && s_SetMethod == null)
                    s_Writer.WriteLine($"{s_Formatted};");
                else
                    s_Writer.WriteLine($"{s_Formatted} {{ {(string.IsNullOrWhiteSpace(s_GetString) ? "" : s_GetString)} {(string.IsNullOrWhiteSpace(s_SetString) ? "" : s_SetString)} }}");

                s_Writer.WriteLine("----");

                return s_Writer.ToString();
            }
        }

        private string PrintEvent(EventInfo p_Event, int p_HeaderLevel)
        {
            var s_Doc = m_Docs.FirstOrDefault(p_Doc => p_Doc.AssemblyName == p_Event.DeclaringType.Assembly.GetName().Name);

            var s_DocEvent = s_Doc?.GetDocEvent($"{p_Event.ReflectedType.FullName}.{p_Event.Name}");

            using (var s_Writer = new StringWriter())
            {
                // Create the header level
                var s_TitleHeader = new string('=', p_HeaderLevel);

                // Write out the name header
                s_Writer.WriteLine($"{s_TitleHeader} {p_Event.Name} {s_TitleHeader}");
                s_Writer.WriteLine();

                s_Writer.WriteLine($"''{s_DocEvent?.FullPath}''");

                s_Writer.WriteLine($"<code>{s_DocEvent?.Summary}</code>");
                s_Writer.WriteLine();

                s_Writer.WriteLine(p_Event.ToString());

                s_Writer.WriteLine("----");

                return s_Writer.ToString();
            }
        }

        private string PrintField(FieldInfo p_Field, int p_HeaderLevel)
        {
            return string.Empty;
        }

        private string PrintMethod(MethodInfo p_Method, int p_HeaderLevel)
        {
            var s_Doc = m_Docs.FirstOrDefault(p_Doc => p_Doc.AssemblyName == p_Method.DeclaringType.Assembly.GetName().Name);

            var s_DocMethod = s_Doc?.GetDocMethod($"{p_Method.ReflectedType.FullName}.{p_Method.Name}");

            using (var s_Writer = new StringWriter())
            {
                // Create the header level
                var s_TitleHeader = new string('=', p_HeaderLevel);

                // Write out the name header
                s_Writer.WriteLine($"{s_TitleHeader} {p_Method.Name} {s_TitleHeader}");
                s_Writer.WriteLine();

                s_Writer.WriteLine($"''{s_DocMethod?.FullPath}''");

                s_Writer.WriteLine($"<code>{s_DocMethod?.Summary}</code>");
                s_Writer.WriteLine();

                var s_Modifier = string.Empty;

                if (p_Method.IsPublic)
                    s_Modifier = c_Public;
                else if (p_Method.IsPrivate)
                    s_Modifier = c_Private;

                s_Writer.WriteLine($"{s_Modifier} {p_Method.ToString()}");

                if (s_DocMethod != null)
                {
                    foreach (var l_Parameter in s_DocMethod.GetDocParameters())
                    {
                        s_Writer.WriteLine($"<code>{l_Parameter.Name} - {l_Parameter.Summary}</code>");
                        s_Writer.WriteLine();
                    }
                }

                s_Writer.WriteLine("----");

                return s_Writer.ToString();
            }
        }

        private string PrintType(Type p_Type, int p_HeaderLevel)
        {
            var s_Final = string.Empty;

            var s_Doc = m_Docs.FirstOrDefault(p_Doc => p_Doc.AssemblyName == p_Type.Assembly.GetName().Name);

            var s_DocType = s_Doc?.GetDocType(p_Type.FullName);

            using (var s_Writer = new StringWriter())
            {
                // Create the header level
                var s_TitleHeader = new string('=', p_HeaderLevel);

                // Write out the name header
                s_Writer.WriteLine($"{s_TitleHeader} {p_Type.Name} {s_TitleHeader}");
                s_Writer.WriteLine();

                s_Writer.WriteLine($"''{p_Type.FullName}''");

                // Write the summary
                s_Writer.WriteLine($"<file>{s_DocType?.Summary}</file>");
                s_Writer.WriteLine();

                // Write a horizontal line
                s_Writer.WriteLine("----");
                s_Writer.WriteLine();

                // Create and write the types header
                var s_SubHeader = new string('=', (p_HeaderLevel > 1 ? p_HeaderLevel - 1 : 1));

                // Write types
                s_Writer.WriteLine($"{s_SubHeader} Types {s_SubHeader}");
                foreach (var l_Type in p_Type.GetNestedTypes())
                    s_Writer.WriteLine(PrintType(l_Type, (p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));
                s_Writer.WriteLine();

                // Write properties
                s_Writer.WriteLine($"{s_SubHeader} Properties {s_SubHeader}");
                foreach (var l_Property in p_Type.GetProperties())
                    s_Writer.WriteLine(PrintProperty(p_Type, l_Property, (p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));
                s_Writer.WriteLine();

                // Write events
                s_Writer.WriteLine($"{s_SubHeader} Events {s_SubHeader}");
                foreach (var l_Event in p_Type.GetEvents())
                    s_Writer.WriteLine(PrintEvent(l_Event, (p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));
                s_Writer.WriteLine();

                // Write fields
                //s_Writer.WriteLine($"{s_SubHeader} Fields {s_SubHeader}");
                //foreach (var l_Field in p_Type.GetFields())
                //    s_Writer.WriteLine(PrintField(l_Field, (p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));
                //s_Writer.WriteLine();

                // Write methods
                s_Writer.WriteLine($"{s_SubHeader} Methods {s_SubHeader}");
                foreach (var l_Method in p_Type.GetMethods())
                {
                    if (l_Method.IsSpecialName)
                        continue;

                    s_Writer.WriteLine(PrintMethod(l_Method, (p_HeaderLevel > 2 ? p_HeaderLevel - 2 : 1)));
                }
                    
                s_Writer.WriteLine();
                s_Writer.WriteLine("----");
                s_Writer.WriteLine($"Build: {p_Type.Assembly.GetName().Name} - {p_Type.Assembly.GetName().Version}");

                s_Writer.Flush();
                s_Final = s_Writer.ToString();
            }

            return s_Final;
        }

        /// <summary>
        /// Reads the xml documentation into a proper format usable by DokuGen
        /// </summary>
        /// <param name="p_XmlPath">Path of the xml file</param>
        /// <returns>True on success, false otherwise</returns>
        private bool ParseAssemblyDocumentation(string p_XmlPath)
        {
            if (string.IsNullOrWhiteSpace(p_XmlPath))
                return false;

            if (!File.Exists(p_XmlPath))
                return false;

            try
            {
                var s_Document = new XmlDocument();

                s_Document.Load(p_XmlPath);

                var s_DokuDoc = new Doc(s_Document.SelectSingleNode("doc"));

                m_Docs.Add(s_DokuDoc);
            }
            catch (Exception p_Exception)
            {
                Console.WriteLine($"There was an exception parsing assembly documentation {p_Exception.Message}.");
                return false;
            }

            return true;
        }
    }
}
