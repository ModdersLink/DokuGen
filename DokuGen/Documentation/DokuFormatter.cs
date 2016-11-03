namespace DokuGen.Documentation
{
    abstract class DokuFormattable : IDokuFormattable
    {
        public string Type { get; set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }

        public abstract string Serialize(int p_HeaderLevel);

        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
