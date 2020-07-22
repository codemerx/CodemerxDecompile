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
			base();
			return;
		}

		public static implicit operator DocumentField(Guid guid)
		{
			stackVariable0 = new DocumentField();
			stackVariable0.set_Id(new Guid?(guid));
			return stackVariable0;
		}

		public static implicit operator DocumentField(Piranha.Models.Media media)
		{
			stackVariable0 = new DocumentField();
			stackVariable0.set_Id(new Guid?(media.get_Id()));
			return stackVariable0;
		}

		public static implicit operator String(DocumentField image)
		{
			if (image.get_Media() == null)
			{
				return "";
			}
			return image.get_Media().get_PublicUrl();
		}
	}
}