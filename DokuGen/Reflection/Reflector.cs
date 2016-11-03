using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DokuGen.Reflection
{
    class Reflector
    {
        private List<Assembly> m_Assemblies;

        public Dictionary<string, Type> Types { get; set; }

        private string m_SearchDirectory = string.Empty;

        public Reflector(string p_AssemblyDirectory)
        {
            m_SearchDirectory = p_AssemblyDirectory;

            m_Assemblies = new List<Assembly>();
            Types = new Dictionary<string, Type>();
        }

        public bool LoadAssemblies()
        {
            var s_AssemblyPaths = Directory.GetFiles(m_SearchDirectory, "*.dll");

            var s_Assemblies = new List<Assembly>();
            foreach (var l_AssemblyFile in s_AssemblyPaths)
                s_Assemblies.Add(Assembly.LoadFile(l_AssemblyFile));

            foreach (var l_Assembly in s_Assemblies)
            {
                Type[] l_Types;

                try
                {
                    l_Types = l_Assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException p_Exception)
                {
                    l_Types = p_Exception.Types;
                }

                foreach (var l_Type in l_Types)
                {
                    if (l_Type == null)
                        continue;

                    var l_Path = l_Type.FullName.Replace('+', '.');

                    if (!Types.ContainsKey(l_Path))
                        Types.Add(l_Path, l_Type);
                }
            }

            return true;
        }
    }
}
