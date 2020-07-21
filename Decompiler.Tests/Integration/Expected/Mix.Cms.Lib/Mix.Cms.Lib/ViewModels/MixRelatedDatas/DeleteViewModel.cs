using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Data.ViewModels;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixRelatedDatas
{
	public class DeleteViewModel : ViewModelBase<MixCmsContext, MixRelatedData, DeleteViewModel>
	{
		public int AttributeSetId
		{
			get;
			set;
		}

		public DateTime CreatedDateTime
		{
			get;
			set;
		}

		public string DataId
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public string ParentId
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

		public DeleteViewModel(MixRelatedData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override MixRelatedData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
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