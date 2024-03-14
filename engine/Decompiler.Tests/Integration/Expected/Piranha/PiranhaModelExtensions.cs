using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class PiranhaModelExtensions
{
	public static void Add(this IList<Taxonomy> list, params string[] titles)
	{
		string[] strArray = titles;
		for (int i = 0; i < (int)strArray.Length; i++)
		{
			string str = strArray[i];
			list.Add(new Taxonomy()
			{
				Title = str
			});
		}
	}
}