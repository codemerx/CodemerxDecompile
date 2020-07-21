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
			base();
			return;
		}

		public static implicit operator AudioField(Guid guid)
		{
			stackVariable0 = new AudioField();
			stackVariable0.set_Id(new Guid?(guid));
			return stackVariable0;
		}

		public static implicit operator AudioField(Piranha.Models.Media media)
		{
			stackVariable0 = new AudioField();
			stackVariable0.set_Id(new Guid?(media.get_Id()));
			return stackVariable0;
		}

		public static implicit operator String(AudioField audio)
		{
			if (audio.get_Media() == null)
			{
				return "";
			}
			return audio.get_Media().get_PublicUrl();
		}
	}
}