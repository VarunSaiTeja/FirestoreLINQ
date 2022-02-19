using System.Text.RegularExpressions;

namespace FirestoreLINQ
{
    /// <summary>
    /// Container for registered Vocabularies.  At present, only a single vocabulary is supported: Default.
    /// </summary>
    public static class Vocabularies
    {
        private static readonly Lazy<Vocabulary> Instance;

        static Vocabularies()
        {
            Instance = new Lazy<Vocabulary>(BuildDefault, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// The default vocabulary used for singular/plural irregularities.
        /// Rules can be added to this vocabulary and will be picked up by called to Singularize() and Pluralize().
        /// At this time, multiple vocabularies and removing existing rules are not supported.
        /// </summary>
        public static Vocabulary Default => Instance.Value;

        private static Vocabulary BuildDefault()
        {
            var _default = new Vocabulary();

            _default.AddPlural("$", "s");
            _default.AddPlural("s$", "s");
            _default.AddPlural("(ax|test)is$", "$1es");
            _default.AddPlural("(octop|vir|alumn|fung|cact|foc|hippopotam|radi|stimul|syllab|nucle)us$", "$1i");
            _default.AddPlural("(alias|bias|iris|status|campus|apparatus|virus|walrus|trellis)$", "$1es");
            _default.AddPlural("(buffal|tomat|volcan|ech|embarg|her|mosquit|potat|torped|vet)o$", "$1oes");
            _default.AddPlural("([dti])um$", "$1a");
            _default.AddPlural("sis$", "ses");
            _default.AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
            _default.AddPlural("(hive)$", "$1s");
            _default.AddPlural("([^aeiouy]|qu)y$", "$1ies");
            _default.AddPlural("(x|ch|ss|sh)$", "$1es");
            _default.AddPlural("(matr|vert|ind|d)(ix|ex)$", "$1ices");
            _default.AddPlural("(^[m|l])ouse$", "$1ice");
            _default.AddPlural("^(ox)$", "$1en");
            _default.AddPlural("(quiz)$", "$1zes");
            _default.AddPlural("(buz|blit|walt)z$", "$1zes");
            _default.AddPlural("(hoo|lea|loa|thie)f$", "$1ves");
            _default.AddPlural("(alumn|alg|larv|vertebr)a$", "$1ae");
            _default.AddPlural("(criteri|phenomen)on$", "$1a");

            _default.AddSingular("s$", "");
            _default.AddSingular("(n)ews$", "$1ews");
            _default.AddSingular("([dti])a$", "$1um");
            _default.AddSingular("(analy|ba|diagno|parenthe|progno|synop|the|ellip|empha|neuro|oa|paraly)ses$", "$1sis");
            _default.AddSingular("([^f])ves$", "$1fe");
            _default.AddSingular("(hive)s$", "$1");
            _default.AddSingular("(tive)s$", "$1");
            _default.AddSingular("([lr]|hoo|lea|loa|thie)ves$", "$1f");
            _default.AddSingular("(^zomb)?([^aeiouy]|qu)ies$", "$2y");
            _default.AddSingular("(s)eries$", "$1eries");
            _default.AddSingular("(m)ovies$", "$1ovie");
            _default.AddSingular("(x|ch|ss|sh)es$", "$1");
            _default.AddSingular("(^[m|l])ice$", "$1ouse");
            _default.AddSingular("(?<!^[a-z])(o)es$", "$1");
            _default.AddSingular("(shoe)s$", "$1");
            _default.AddSingular("(cris|ax|test)es$", "$1is");
            _default.AddSingular("(octop|vir|alumn|fung|cact|foc|hippopotam|radi|stimul|syllab|nucle)i$", "$1us");
            _default.AddSingular("(alias|bias|iris|status|campus|apparatus|virus|walrus|trellis)es$", "$1");
            _default.AddSingular("^(ox)en", "$1");
            _default.AddSingular("(matr|d)ices$", "$1ix");
            _default.AddSingular("(vert|ind)ices$", "$1ex");
            _default.AddSingular("(quiz)zes$", "$1");
            _default.AddSingular("(buz|blit|walt)zes$", "$1z");
            _default.AddSingular("(alumn|alg|larv|vertebr)ae$", "$1a");
            _default.AddSingular("(criteri|phenomen)a$", "$1on");
            _default.AddSingular("([b|r|c]ook|room|smooth)ies$", "$1ie");

            _default.AddIrregular("person", "people");
            _default.AddIrregular("man", "men");
            _default.AddIrregular("human", "humans");
            _default.AddIrregular("child", "children");
            _default.AddIrregular("sex", "sexes");
            _default.AddIrregular("glove", "gloves");
            _default.AddIrregular("move", "moves");
            _default.AddIrregular("goose", "geese");
            _default.AddIrregular("wave", "waves");
            _default.AddIrregular("foot", "feet");
            _default.AddIrregular("tooth", "teeth");
            _default.AddIrregular("curriculum", "curricula");
            _default.AddIrregular("database", "databases");
            _default.AddIrregular("zombie", "zombies");
            _default.AddIrregular("personnel", "personnel");
            _default.AddIrregular("cache", "caches");

            _default.AddIrregular("ex", "exes", matchEnding: false);
            _default.AddIrregular("is", "are", matchEnding: false);
            _default.AddIrregular("that", "those", matchEnding: false);
            _default.AddIrregular("this", "these", matchEnding: false);
            _default.AddIrregular("bus", "buses", matchEnding: false);
            _default.AddIrregular("die", "dice", matchEnding: false);
            _default.AddIrregular("tie", "ties", matchEnding: false);

            _default.AddUncountable("staff");
            _default.AddUncountable("training");
            _default.AddUncountable("equipment");
            _default.AddUncountable("information");
            _default.AddUncountable("corn");
            _default.AddUncountable("milk");
            _default.AddUncountable("rice");
            _default.AddUncountable("money");
            _default.AddUncountable("species");
            _default.AddUncountable("series");
            _default.AddUncountable("fish");
            _default.AddUncountable("sheep");
            _default.AddUncountable("deer");
            _default.AddUncountable("aircraft");
            _default.AddUncountable("oz");
            _default.AddUncountable("tsp");
            _default.AddUncountable("tbsp");
            _default.AddUncountable("ml");
            _default.AddUncountable("l");
            _default.AddUncountable("water");
            _default.AddUncountable("waters");
            _default.AddUncountable("semen");
            _default.AddUncountable("sperm");
            _default.AddUncountable("bison");
            _default.AddUncountable("grass");
            _default.AddUncountable("hair");
            _default.AddUncountable("mud");
            _default.AddUncountable("elk");
            _default.AddUncountable("luggage");
            _default.AddUncountable("moose");
            _default.AddUncountable("offspring");
            _default.AddUncountable("salmon");
            _default.AddUncountable("shrimp");
            _default.AddUncountable("someone");
            _default.AddUncountable("swine");
            _default.AddUncountable("trout");
            _default.AddUncountable("tuna");
            _default.AddUncountable("corps");
            _default.AddUncountable("scissors");
            _default.AddUncountable("means");
            _default.AddUncountable("mail");
            _default.AddUncountable("metadata");

            return _default;
        }
    }

    /// <summary>
    /// A container for exceptions to simple pluralization/singularization rules.
    /// Vocabularies.Default contains an extensive list of rules for US English.
    /// At this time, multiple vocabularies and removing existing rules are not supported.
    /// </summary>
    public class Vocabulary
    {
        internal Vocabulary()
        {
        }

        private readonly List<Rule> _plurals = new List<Rule>();
        private readonly List<Rule> _singulars = new List<Rule>();
        private readonly List<string> _uncountables = new List<string>();

        /// <summary>
        /// Adds a word to the vocabulary which cannot easily be pluralized/singularized by RegEx, e.g. "person" and "people".
        /// </summary>
        /// <param name="singular">The singular form of the irregular word, e.g. "person".</param>
        /// <param name="plural">The plural form of the irregular word, e.g. "people".</param>
        /// <param name="matchEnding">True to match these words on their own as well as at the end of longer words. False, otherwise.</param>
        public void AddIrregular(string singular, string plural, bool matchEnding = true)
        {
            if (matchEnding)
            {
                AddPlural("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
                AddSingular("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
            }
            else
            {
                AddPlural($"^{singular}$", plural);
                AddSingular($"^{plural}$", singular);
            }
        }

        /// <summary>
        /// Adds an uncountable word to the vocabulary, e.g. "fish".  Will be ignored when plurality is changed.
        /// </summary>
        /// <param name="word">Word to be added to the list of uncountables.</param>
        public void AddUncountable(string word)
        {
            _uncountables.Add(word.ToLower());
        }

        /// <summary>
        /// Adds a rule to the vocabulary that does not follow trivial rules for pluralization, e.g. "bus" -> "buses"
        /// </summary>
        /// <param name="rule">RegEx to be matched, case insensitive, e.g. "(bus)es$"</param>
        /// <param name="replacement">RegEx replacement  e.g. "$1"</param>
        public void AddPlural(string rule, string replacement)
        {
            _plurals.Add(new Rule(rule, replacement));
        }

        /// <summary>
        /// Adds a rule to the vocabulary that does not follow trivial rules for singularization, e.g. "vertices/indices -> "vertex/index"
        /// </summary>
        /// <param name="rule">RegEx to be matched, case insensitive, e.g. ""(vert|ind)ices$""</param>
        /// <param name="replacement">RegEx replacement  e.g. "$1ex"</param>
        public void AddSingular(string rule, string replacement)
        {
            _singulars.Add(new Rule(rule, replacement));
        }

        /// <summary>
        /// Pluralizes the provided input considering irregular words
        /// </summary>
        /// <param name="word">Word to be pluralized</param>
        /// <param name="inputIsKnownToBeSingular">Normally you call Pluralize on singular words; but if you're unsure call it with false</param>
        /// <returns></returns>
        public string Pluralize(string word, bool inputIsKnownToBeSingular = true)
        {
            var result = ApplyRules(_plurals, word, false);

            if (inputIsKnownToBeSingular)
            {
                return result ?? word;
            }

            var asSingular = ApplyRules(_singulars, word, false);
            var asSingularAsPlural = ApplyRules(_plurals, asSingular, false);
            if (asSingular != null && asSingular != word && asSingular + "s" != word && asSingularAsPlural == word && result != word)
            {
                return word;
            }

            return result;
        }

        /// <summary>
        /// Singularizes the provided input considering irregular words
        /// </summary>
        /// <param name="word">Word to be singularized</param>
        /// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false</param>
        /// <param name="skipSimpleWords">Skip singularizing single words that have an 's' on the end</param>
        /// <returns></returns>
        public string Singularize(string word, bool inputIsKnownToBePlural = true, bool skipSimpleWords = false)
        {
            var result = ApplyRules(_singulars, word, skipSimpleWords);

            if (inputIsKnownToBePlural)
            {
                return result ?? word;
            }

            // the Plurality is unknown so we should check all possibilities
            var asPlural = ApplyRules(_plurals, word, false);
            var asPluralAsSingular = ApplyRules(_singulars, asPlural, false);
            if (asPlural != word && word + "s" != asPlural && asPluralAsSingular == word && result != word)
            {
                return word;
            }

            return result ?? word;
        }

        private string ApplyRules(IList<Rule> rules, string word, bool skipFirstRule)
        {
            if (word == null)
            {
                return null;
            }

            if (word.Length < 1)
            {
                return word;
            }

            if (IsUncountable(word))
            {
                return word;
            }

            var result = word;
            var end = skipFirstRule ? 1 : 0;
            for (var i = rules.Count - 1; i >= end; i--)
            {
                if ((result = rules[i].Apply(word)) != null)
                {
                    break;
                }
            }
            return result != null ? MatchUpperCase(word, result) : result;
        }

        private bool IsUncountable(string word)
        {
            return _uncountables.Contains(word.ToLower());
        }

        private string MatchUpperCase(string word, string replacement)
        {
            return char.IsUpper(word[0]) && char.IsLower(replacement[0]) ? char.ToUpper(replacement[0]) + replacement.Substring(1) : replacement;
        }

        private class Rule
        {
            private readonly Regex _regex;
            private readonly string _replacement;

            public Rule(string pattern, string replacement)
            {
                _regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptionsUtil.Compiled);
                _replacement = replacement;
            }

            public string Apply(string word)
            {
                if (!_regex.IsMatch(word))
                {
                    return null;
                }

                return _regex.Replace(word, _replacement);
            }
        }
    }

    internal static class RegexOptionsUtil
    {
        static RegexOptionsUtil()
        {
            Compiled = Enum.TryParse("Compiled", out RegexOptions compiled) ? compiled : RegexOptions.None;
        }

        public static RegexOptions Compiled { get; }
    }
}
