using System;
using System.IO;

namespace Piranha
{
	public interface IImageProcessor
	{
		void Crop(Stream source, Stream dest, int width, int height);

		void CropScale(Stream source, Stream dest, int width, int height);

		void GetSize(Stream stream, out int width, out int height);

		void GetSize(byte[] bytes, out int width, out int height);

		void Scale(Stream source, Stream dest, int width);
	}
}