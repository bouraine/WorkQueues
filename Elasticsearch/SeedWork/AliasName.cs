namespace Elasticsearch
{
    public class AliasName
    {
        internal string Value { get; }

        public AliasName(string name)
        {
            Value = name;
        }

        public static implicit operator AliasName(string name)
        {
            // TODO: validate alias name
            return new AliasName(name);
        }
    }
}