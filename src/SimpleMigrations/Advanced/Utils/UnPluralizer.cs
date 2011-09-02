using Simple.Data;

namespace SimpleMigrations.Advanced.Utils
{
    internal class UnPluralizer : IPluralizer
    {
        public bool IsPlural(string word)
        {
            return false;
        }

        public bool IsSingular(string word)
        {
            return false;
        }

        public string Pluralize(string word)
        {
            return word;
        }

        public string Singularize(string word)
        {
            return word;
        }
    }
}
