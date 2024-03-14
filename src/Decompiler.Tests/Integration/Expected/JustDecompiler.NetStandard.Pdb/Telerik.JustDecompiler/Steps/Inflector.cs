using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Telerik.JustDecompiler.Steps
{
	public static class Inflector
	{
		private readonly static List<Inflector.Rule> _plurals;

		private readonly static List<Inflector.Rule> _singulars;

		private readonly static List<string> _uncountables;

		private readonly static Dictionary<string, string> _reservedWords;

		static Inflector()
		{
			Inflector._plurals = new List<Inflector.Rule>();
			Inflector._singulars = new List<Inflector.Rule>();
			Inflector._uncountables = new List<string>();
			Inflector._reservedWords = new Dictionary<string, string>();
			Inflector.AddPlural("$", "s");
			Inflector.AddPlural("s$", "s");
			Inflector.AddPlural("(ax|test)is$", "$1es");
			Inflector.AddPlural("(octop|vir)us$", "$1i");
			Inflector.AddPlural("(alias|status)$", "$1es");
			Inflector.AddPlural("(bu)s$", "$1ses");
			Inflector.AddPlural("(buffal|tomat)o$", "$1oes");
			Inflector.AddPlural("([ti])um$", "$1a");
			Inflector.AddPlural("sis$", "ses");
			Inflector.AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
			Inflector.AddPlural("(hive)$", "$1s");
			Inflector.AddPlural("([^aeiouy]|qu)y$", "$1ies");
			Inflector.AddPlural("(x|ch|ss|sh)$", "$1es");
			Inflector.AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
			Inflector.AddPlural("([m|l])ouse$", "$1ice");
			Inflector.AddPlural("^(ox)$", "$1en");
			Inflector.AddPlural("(quiz)$", "$1zes");
			Inflector.AddSingular("s$", "");
			Inflector.AddSingular("ss$", "ss");
			Inflector.AddSingular("([ti])a$", "$1um");
			Inflector.AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
			Inflector.AddSingular("(^analy)ses$", "$1sis");
			Inflector.AddSingular("([^f])ves$", "$1fe");
			Inflector.AddSingular("(hive)s$", "$1");
			Inflector.AddSingular("(tive)s$", "$1");
			Inflector.AddSingular("([lr])ves$", "$1f");
			Inflector.AddSingular("([^aeiouy]|qu)ies$", "$1y");
			Inflector.AddSingular("(s)eries$", "$1eries");
			Inflector.AddSingular("(m)ovies$", "$1ovie");
			Inflector.AddSingular("(x|ch|ss|sh)es$", "$1");
			Inflector.AddSingular("([m|l])ice$", "$1ouse");
			Inflector.AddSingular("(bus)es$", "$1");
			Inflector.AddSingular("(o)es$", "$1");
			Inflector.AddSingular("(shoe)s$", "$1");
			Inflector.AddSingular("(cris|ax|test)es$", "$1is");
			Inflector.AddSingular("(octop|vir)i$", "$1us");
			Inflector.AddSingular("(alias|status)es$", "$1");
			Inflector.AddSingular("^(ox)en", "$1");
			Inflector.AddSingular("(vert|ind)ices$", "$1ex");
			Inflector.AddSingular("(matr)ices$", "$1ix");
			Inflector.AddSingular("(quiz)zes$", "$1");
			Inflector.AddIrregular("person", "people");
			Inflector.AddIrregular("man", "men");
			Inflector.AddIrregular("child", "children");
			Inflector.AddIrregular("sex", "sexes");
			Inflector.AddIrregular("move", "moves");
			Inflector.AddUncountable("equipment");
			Inflector.AddUncountable("information");
			Inflector.AddUncountable("rice");
			Inflector.AddUncountable("money");
			Inflector.AddUncountable("species");
			Inflector.AddUncountable("series");
			Inflector.AddUncountable("fish");
			Inflector.AddUncountable("sheep");
			Inflector.AddUncountable("news");
		}

		private static void AddIrregular(string singular, string plural)
		{
			String[] str = new String[] { "(", null, null, null, null };
			char chr = singular[0];
			str[1] = chr.ToString();
			str[2] = ")";
			str[3] = singular.Substring(1);
			str[4] = "$";
			Inflector.AddPlural(String.Concat(str), String.Concat("$1", plural.Substring(1)));
			String[] strArrays = new String[] { "(", null, null, null, null };
			chr = plural[0];
			strArrays[1] = chr.ToString();
			strArrays[2] = ")";
			strArrays[3] = plural.Substring(1);
			strArrays[4] = "$";
			Inflector.AddSingular(String.Concat(strArrays), String.Concat("$1", singular.Substring(1)));
		}

		private static void AddPlural(string rule, string replacement)
		{
			Inflector._plurals.Add(new Inflector.Rule(rule, replacement));
		}

		private static void AddSingular(string rule, string replacement)
		{
			Inflector._singulars.Add(new Inflector.Rule(rule, replacement));
		}

		private static void AddUncountable(string word)
		{
			Inflector._uncountables.Add(word.ToLower());
		}

		private static string ApplyRules(List<Inflector.Rule> rules, string word)
		{
			string str = word;
			if (!Inflector._uncountables.Contains(word.ToLower()))
			{
				if (Inflector._reservedWords.ContainsKey(word.ToLower()))
				{
					return Inflector._reservedWords[word.ToLower()].ToString();
				}
				for (int i = rules.Count - 1; i >= 0; i--)
				{
					string str1 = rules[i].Apply(word);
					str = str1;
					if (str1 != null)
					{
						break;
					}
				}
			}
			if (str != null)
			{
				return str;
			}
			return word;
		}

		public static string Camelize(string lowercaseAndUnderscoredWord)
		{
			if (String.IsNullOrEmpty(lowercaseAndUnderscoredWord))
			{
				return lowercaseAndUnderscoredWord;
			}
			StringBuilder stringBuilder = new StringBuilder(lowercaseAndUnderscoredWord);
			for (int i = 1; i < stringBuilder.Length - 1; i++)
			{
				if (stringBuilder[i] == '\u005F')
				{
					stringBuilder[i] = Char.ToUpper(stringBuilder[i + 1]);
					stringBuilder.Remove(i + 1, 1);
				}
			}
			stringBuilder[0] = Char.ToLower(stringBuilder[0]);
			return stringBuilder.ToString();
		}

		public static string Pluralize(string word)
		{
			return Inflector.ApplyRules(Inflector._plurals, word);
		}

		public static string Singularize(string word)
		{
			return Inflector.ApplyRules(Inflector._singulars, word);
		}

		private class Rule
		{
			private readonly Regex _regex;

			private readonly string _replacement;

			public Rule(string pattern, string replacement)
			{
				this._regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
				this._replacement = replacement;
			}

			public Rule(string pattern, string replacment, bool ncaseSensitive)
			{
				if (ncaseSensitive)
				{
					this._regex = new Regex(pattern, RegexOptions.Compiled);
					this._replacement = replacment;
					return;
				}
				this._regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
				this._replacement = replacment;
			}

			public string Apply(string word)
			{
				if (!this._regex.IsMatch(word))
				{
					return null;
				}
				return this._regex.Replace(word, this._replacement);
			}
		}
	}
}