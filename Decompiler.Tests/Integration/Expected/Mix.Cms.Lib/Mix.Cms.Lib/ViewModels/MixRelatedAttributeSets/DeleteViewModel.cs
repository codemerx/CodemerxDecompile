using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Data.ViewModels;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixRelatedAttributeSets
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixRelatedAttributeSet, DeleteViewModel>
	{
		public DateTime CreatedDateTime
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}

		public int ParentId
		{
			get;
			set;
		}

		public MixEnums.MixAttributeSetDataType ParentType
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public DeleteViewModel()
		{
			base();
			return;
		}

		public DeleteViewModel(MixRelatedAttributeSet model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override MixRelatedAttributeSet ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = this.get_CreatedDateTime();
			V_0 = new DateTime();
			if (DateTime.op_Equality(stackVariable1, V_0))
			{
				this.set_CreatedDateTime(DateTime.get_UtcNow());
			}
			return this.ParseModel(_context, _transaction);
		}
	}
}