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
			base();
			return;
		}

		public static implicit operator VideoField(Guid guid)
		{
			stackVariable0 = new VideoField();
			stackVariable0.set_Id(new Guid?(guid));
			return stackVariable0;
		}

		public static implicit operator VideoField(Piranha.Models.Media media)
		{
			stackVariable0 = new VideoField();
			stackVariable0.set_Id(new Guid?(media.get_Id()));
			return stackVariable0;
		}

		public static implicit operator String(VideoField video)
		{
			if (video.get_Media() == null)
			{
				return "";
			}
			return video.get_Media().get_PublicUrl();
		}
	}
}