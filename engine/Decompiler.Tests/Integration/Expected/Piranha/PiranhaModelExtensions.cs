using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class PiranhaModelExtensions
{
	public static void Add(this IList<Taxonomy> list, params string[] titles)
	{
		V_0 = titles;
		V_1 = 0;
		while (V_1 < (int)V_0.Length)
		{
			V_2 = V_0[V_1];
			stackVariable10 = new Taxonomy();
			stackVariable10.set_Title(V_2);
			list.Add(stackVariable10);
			V_1 = V_1 + 1;
		}
		return;
	}
}