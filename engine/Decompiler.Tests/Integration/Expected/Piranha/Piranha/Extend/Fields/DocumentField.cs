using Piranha.Extend;
using Piranha.Models;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Document", Shorthand="Document", Component="document-field")]
	public class DocumentField : MediaFieldBase<DocumentField>
	{
		public DocumentField()
		{
		}

		public static implicit operator DocumentField(Guid guid)
		{
			return new DocumentField()
			{
				Id = new Guid?(guid)
			};
		}

		public static implicit operator DocumentField(Piranha.Models.Media media)
		{
			return new DocumentField()
			{
				Id = new Guid?(media.Id)
			};
		}

		public static implicit operator String(DocumentField image)
		{
			if (image.Media == null)
			{
				return "";
			}
			return image.Media.PublicUrl;
		}
	}
}