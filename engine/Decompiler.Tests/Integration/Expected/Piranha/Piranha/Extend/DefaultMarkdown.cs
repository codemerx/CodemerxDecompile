using Markdig;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	public class DefaultMarkdown : IMarkdown
	{
		public MarkdownPipeline _pipeline
		{
			get;
			set;
		}

		public DefaultMarkdown()
		{
		}

		public string Transform(string md)
		{
			if (String.IsNullOrEmpty(md))
			{
				return md;
			}
			return Markdown.ToHtml(md, this._pipeline);
		}
	}
}