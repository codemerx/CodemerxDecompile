using Piranha.Extend;
using Piranha.Models;
using System;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Video", Shorthand="Video", Component="video-field")]
	public class VideoField : MediaFieldBase<VideoField>
	{
		public VideoField()
		{
		}

		public static implicit operator VideoField(Guid guid)
		{
			return new VideoField()
			{
				Id = new Guid?(guid)
			};
		}

		public static implicit operator VideoField(Piranha.Models.Media media)
		{
			return new VideoField()
			{
				Id = new Guid?(media.Id)
			};
		}

		public static implicit operator String(VideoField video)
		{
			if (video.Media == null)
			{
				return "";
			}
			return video.Media.PublicUrl;
		}
	}
}