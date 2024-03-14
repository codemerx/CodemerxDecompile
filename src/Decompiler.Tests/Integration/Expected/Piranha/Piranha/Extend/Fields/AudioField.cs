using Piranha.Extend;
using Piranha.Models;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Audio", Shorthand="Audio", Component="audio-field")]
	public class AudioField : MediaFieldBase<AudioField>
	{
		public AudioField()
		{
		}

		public static implicit operator AudioField(Guid guid)
		{
			return new AudioField()
			{
				Id = new Guid?(guid)
			};
		}

		public static implicit operator AudioField(Piranha.Models.Media media)
		{
			return new AudioField()
			{
				Id = new Guid?(media.Id)
			};
		}

		public static implicit operator String(AudioField audio)
		{
			if (audio.Media == null)
			{
				return "";
			}
			return audio.Media.PublicUrl;
		}
	}
}