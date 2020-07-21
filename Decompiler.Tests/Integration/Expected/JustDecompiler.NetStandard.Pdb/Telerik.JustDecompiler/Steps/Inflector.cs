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
			return;
		}

		private static void AddIrregular(string singular, string plural)
		{
			stackVariable1 = new String[5];
			stackVariable1[0] = "(";
			V_0 = singular.get_Chars(0);
			stackVariable1[1] = V_0.ToString();
			stackVariable1[2] = ")";
			stackVariable1[3] = singular.Substring(1);
			stackVariable1[4] = "$";
			Inflector.AddPlural(String.Concat(stackVariable1), String.Concat("$1", plural.Substring(1)));
			stackVariable25 = new String[5];
			stackVariable25[0] = "(";
			V_0 = plural.get_Chars(0);
			stackVariable25[1] = V_0.ToString();
			stackVariable25[2] = ")";
			stackVariable25[3] = plural.Substring(1);
			stackVariable25[4] = "$";
			Inflector.AddSingular(String.Concat(stackVariable25), String.Concat("$1", singular.Substring(1)));
			return;
		}

		private static void AddPlural(string rule, string replacement)
		{
			Inflector._plurals.Add(new Inflector.Rule(rule, replacement));
			return;
		}

		private static void AddSingular(string rule, string replacement)
		{
			Inflector._singulars.Add(new Inflector.Rule(rule, replacement));
			return;
		}

		private static void AddUncountable(string word)
		{
			Inflector._uncountables.Add(word.ToLower());
			return;
		}

		private static string ApplyRules(List<Inflector.Rule> rules, string word)
		{
			V_0 = word;
			if (!Inflector._uncountables.Contains(word.ToLower()))
			{
				if (Inflector._reservedWords.ContainsKey(word.ToLower()))
				{
					return Inflector._reservedWords.get_Item(word.ToLower()).ToString();
				}
				V_1 = rules.get_Count() - 1;
				while (V_1 >= 0)
				{
					stackVariable22 = rules.get_Item(V_1).Apply(word);
					V_0 = stackVariable22;
					if (stackVariable22 != null)
					{
						break;
					}
					V_1 = V_1 - 1;
				}
			}
			if (V_0 != null)
			{
				return V_0;
			}
			return word;
		}

		public static string Camelize(string lowercaseAndUnderscoredWord)
		{
			if (String.IsNullOrEmpty(lowercaseAndUnderscoredWord))
			{
				return lowercaseAndUnderscoredWord;
			}
			V_0 = new StringBuilder(lowercaseAndUnderscoredWord);
			V_1 = 1;
			while (V_1 < V_0.get_Length() - 1)
			{
				if (V_0.get_Chars(V_1) == '\u005F')
				{
					V_0.set_Chars(V_1, Char.ToUpper(V_0.get_Chars(V_1 + 1)));
					dummyVar0 = V_0.Remove(V_1 + 1, 1);
				}
				V_1 = V_1 + 1;
			}
			V_0.set_Chars(0, Char.ToLower(V_0.get_Chars(0)));
			return V_0.ToString();
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
				base();
				this._regex = new Regex(pattern, 9);
				this._replacement = replacement;
				return;
			}

			public Rule(string pattern, string replacment, bool ncaseSensitive)
			{
				base();
				if (ncaseSensitive)
				{
					this._regex = new Regex(pattern, 8);
					this._replacement = replacment;
					return;
				}
				this._regex = new Regex(pattern, 9);
				this._replacement = replacment;
				return;
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