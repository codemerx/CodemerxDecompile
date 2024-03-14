using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Areas.Api.Controllers.Schemas.Models.Fields;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Converters
{
	[Nullable(0)]
	[NullableContext(1)]
	internal sealed class FieldPropertiesDtoFactory : IFieldPropertiesVisitor<FieldPropertiesDto, None>
	{
		private readonly static FieldPropertiesDtoFactory Instance;

		static FieldPropertiesDtoFactory()
		{
			FieldPropertiesDtoFactory.Instance = new FieldPropertiesDtoFactory();
		}

		private FieldPropertiesDtoFactory()
		{
		}

		public static FieldPropertiesDto Create(FieldProperties properties)
		{
			return properties.Accept<FieldPropertiesDto, None>(FieldPropertiesDtoFactory.Instance, None.Value);
		}

		public FieldPropertiesDto Visit(ArrayFieldProperties properties, None args)
		{
			return SimpleMapper.Map<ArrayFieldProperties, ArrayFieldPropertiesDto>(properties, new ArrayFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(AssetsFieldProperties properties, None args)
		{
			return SimpleMapper.Map<AssetsFieldProperties, AssetsFieldPropertiesDto>(properties, new AssetsFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(BooleanFieldProperties properties, None args)
		{
			return SimpleMapper.Map<BooleanFieldProperties, BooleanFieldPropertiesDto>(properties, new BooleanFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(ComponentFieldProperties properties, None args)
		{
			return SimpleMapper.Map<ComponentFieldProperties, ComponentFieldPropertiesDto>(properties, new ComponentFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(ComponentsFieldProperties properties, None args)
		{
			return SimpleMapper.Map<ComponentsFieldProperties, ComponentsFieldPropertiesDto>(properties, new ComponentsFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(DateTimeFieldProperties properties, None args)
		{
			return SimpleMapper.Map<DateTimeFieldProperties, DateTimeFieldPropertiesDto>(properties, new DateTimeFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(GeolocationFieldProperties properties, None args)
		{
			return SimpleMapper.Map<GeolocationFieldProperties, GeolocationFieldPropertiesDto>(properties, new GeolocationFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(JsonFieldProperties properties, None args)
		{
			return SimpleMapper.Map<JsonFieldProperties, JsonFieldPropertiesDto>(properties, new JsonFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(NumberFieldProperties properties, None args)
		{
			return SimpleMapper.Map<NumberFieldProperties, NumberFieldPropertiesDto>(properties, new NumberFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(ReferencesFieldProperties properties, None args)
		{
			return SimpleMapper.Map<ReferencesFieldProperties, ReferencesFieldPropertiesDto>(properties, new ReferencesFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(StringFieldProperties properties, None args)
		{
			return SimpleMapper.Map<StringFieldProperties, StringFieldPropertiesDto>(properties, new StringFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(TagsFieldProperties properties, None args)
		{
			return SimpleMapper.Map<TagsFieldProperties, TagsFieldPropertiesDto>(properties, new TagsFieldPropertiesDto());
		}

		public FieldPropertiesDto Visit(UIFieldProperties properties, None args)
		{
			return SimpleMapper.Map<UIFieldProperties, UIFieldPropertiesDto>(properties, new UIFieldPropertiesDto());
		}
	}
}