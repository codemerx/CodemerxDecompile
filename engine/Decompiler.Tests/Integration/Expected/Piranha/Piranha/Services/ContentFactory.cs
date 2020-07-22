using Microsoft.Extensions.DependencyInjection;
using Piranha.Extend;
using Piranha.Models;
using Piranha.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public class ContentFactory : IContentFactory
	{
		private readonly IServiceProvider _services;

		public ContentFactory(IServiceProvider services)
		{
			base();
			this._services = services;
			return;
		}

		public Task<T> CreateAsync<T>(ContentTypeBase type)
		where T : ContentBase
		{
			if (Type.GetTypeFromHandle(// 
			// Current member / type: System.Threading.Tasks.Task`1<T> Piranha.Services.ContentFactory::CreateAsync(Piranha.Models.ContentTypeBase)
			// Exception in: System.Threading.Tasks.Task<T> CreateAsync(Piranha.Models.ContentTypeBase)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public async Task<object> CreateBlockAsync(string typeName)
		{
			V_0.u003cu003e4__this = this;
			V_0.typeName = typeName;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<object>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cCreateBlockAsyncu003ed__4>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<T> CreateDynamicModelAsync<T>(ContentTypeBase type)
		where T : ContentBase
		{
			V_0.u003cu003e4__this = this;
			V_0.type = type;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cCreateDynamicModelAsyncu003ed__5<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<object> CreateDynamicRegionAsync(ContentTypeBase type, string regionId, bool managerInit = false)
		{
			V_0 = new ContentFactory.u003cu003ec__DisplayClass3_0();
			V_0.regionId = regionId;
			V_1 = ServiceProviderServiceExtensions.CreateScope(this._services);
			try
			{
				V_2 = type.get_Regions().FirstOrDefault<RegionType>(new Func<RegionType, bool>(V_0.u003cCreateDynamicRegionAsyncu003eb__0));
				if (V_2 == null)
				{
					V_3 = null;
				}
				else
				{
					V_3 = this.CreateDynamicRegionAsync(V_1, V_2, true, managerInit);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_3;
		}

		private async Task<object> CreateDynamicRegionAsync(IServiceScope scope, RegionType regionType, bool initFields = true, bool managerInit = false)
		{
			V_0.u003cu003e4__this = this;
			V_0.scope = scope;
			V_0.regionType = regionType;
			V_0.initFields = initFields;
			V_0.managerInit = managerInit;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<object>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cCreateDynamicRegionAsyncu003ed__17>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private object CreateField(FieldType fieldType)
		{
			V_0 = App.get_Fields().GetByType(fieldType.get_Type());
			if (V_0 == null)
			{
				return null;
			}
			return Activator.CreateInstance(V_0.get_Type());
		}

		private async Task<T> CreateModelAsync<T>(ContentTypeBase type)
		where T : ContentBase
		{
			V_0.u003cu003e4__this = this;
			V_0.type = type;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cCreateModelAsyncu003ed__6<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<object> CreateRegionAsync(IServiceScope scope, object model, Type modelType, RegionType regionType, bool initFields = true)
		{
			V_0.u003cu003e4__this = this;
			V_0.scope = scope;
			V_0.modelType = modelType;
			V_0.regionType = regionType;
			V_0.initFields = initFields;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<object>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cCreateRegionAsyncu003ed__18>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<T> InitAsync<T>(T model, ContentTypeBase type)
		where T : ContentBase
		{
			return this.InitAsync<T>(model, type, false);
		}

		private async Task<T> InitAsync<T>(T model, ContentTypeBase type, bool managerInit)
		where T : ContentBase
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.type = type;
			V_0.managerInit = managerInit;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cInitAsyncu003ed__13<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task InitBlockAsync(IServiceScope scope, Block block, bool managerInit)
		{
			V_0.u003cu003e4__this = this;
			V_0.scope = scope;
			V_0.block = block;
			V_0.managerInit = managerInit;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cInitBlockAsyncu003ed__16>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<T> InitDynamicAsync<T>(T model, ContentTypeBase type)
		where T : IDynamicContent
		{
			return this.InitDynamicAsync<T>(model, type, false);
		}

		private async Task<T> InitDynamicAsync<T>(T model, ContentTypeBase type, bool managerInit)
		where T : IDynamicContent
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.type = type;
			V_0.managerInit = managerInit;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cInitDynamicAsyncu003ed__9<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<T> InitDynamicManagerAsync<T>(T model, ContentTypeBase type)
		where T : IDynamicContent
		{
			return this.InitDynamicAsync<T>(model, type, true);
		}

		private async Task InitDynamicRegionAsync(IServiceScope scope, object region, RegionType regionType, bool managerInit)
		{
			V_0.u003cu003e4__this = this;
			V_0.scope = scope;
			V_0.region = region;
			V_0.regionType = regionType;
			V_0.managerInit = managerInit;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cInitDynamicRegionAsyncu003ed__14>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<object> InitFieldAsync(object field, bool managerInit = false)
		{
			V_0.u003cu003e4__this = this;
			V_0.field = field;
			V_0.managerInit = managerInit;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<object>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cInitFieldAsyncu003ed__12>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<object> InitFieldAsync(IServiceScope scope, object field, bool managerInit)
		{
			V_0.scope = scope;
			V_0.field = field;
			V_0.managerInit = managerInit;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<object>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cInitFieldAsyncu003ed__20>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<T> InitManagerAsync<T>(T model, ContentTypeBase type)
		where T : ContentBase
		{
			return this.InitAsync<T>(model, type, true);
		}

		private async Task InitRegionAsync(IServiceScope scope, object region, RegionType regionType, bool managerInit)
		{
			V_0.u003cu003e4__this = this;
			V_0.scope = scope;
			V_0.region = region;
			V_0.regionType = regionType;
			V_0.managerInit = managerInit;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ContentFactory.u003cInitRegionAsyncu003ed__15>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}