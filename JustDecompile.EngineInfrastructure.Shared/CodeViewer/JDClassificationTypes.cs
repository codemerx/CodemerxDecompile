#if !JUSTASSEMBLY && !ENGINEONLYBUILD
using Telerik.Windows.Documents.Code.Tagging;
#endif

namespace JustDecompile.EngineInfrastructure
{
	public class JDClassificationTypes
	{
#if !JUSTASSEMBLY && !ENGINEONLYBUILD
		public readonly static ClassificationType MailLink = new ClassificationType("MailLinkClassificationType");

		public readonly static ClassificationType UpdateJdLink = new ClassificationType("UpdateJdLink");

		public readonly static ClassificationType UpdateJdText = new ClassificationType("UpdateJdText");

		public readonly static ClassificationType Text = new ClassificationType("TextClassificationType");

		public readonly static ClassificationType NotResolvedReference = new ClassificationType("NotResolvedReferenceClassificationType");

		public readonly static ClassificationType XmlAttribute = new ClassificationType("XmlAttributeClassificationType");

		public readonly static ClassificationType FakeIdentifier = new ClassificationType("FakeIdentifierType");

		public readonly static ClassificationType DocumentationTag = new ClassificationType("DocumentationTagType");
#endif
	}
}
