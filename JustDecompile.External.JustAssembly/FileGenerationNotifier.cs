using System;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	class FileGenerationNotifier : Telerik.JustDecompiler.External.IFileGenerationNotifier
	{
		private readonly IFileGenerationNotifier notifier;

		public void OnProjectFileGenerated(Telerik.JustDecompiler.External.Interfaces.IFileGeneratedInfo args)
		{
			FileGeneratedInfo fileGeneratedInfo = new FileGeneratedInfo(args.FullPath, args.HasErrors);
			this.notifier.OnProjectFileGenerated(fileGeneratedInfo);
		}

		public uint TotalFileCount
		{
			get
			{
				return this.notifier.TotalFileCount;
			}
			set
			{
				this.notifier.TotalFileCount = value;
			}
		}

		public FileGenerationNotifier(IFileGenerationNotifier notifier)
		{
			this.notifier = notifier;
		}

	}
}
