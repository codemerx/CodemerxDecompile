using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Lib;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixCmsContext : DbContext
	{
		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeField> MixAttributeField
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeSet> MixAttributeSet
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeSetData> MixAttributeSetData
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference> MixAttributeSetReference
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeSetValue> MixAttributeSetValue
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixCache> MixCache
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixCmsUser> MixCmsUser
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixConfiguration> MixConfiguration
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixCulture> MixCulture
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixFile> MixFile
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixLanguage> MixLanguage
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixMedia> MixMedia
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixModule> MixModule
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixModuleData> MixModuleData
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixModulePost> MixModulePost
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPage> MixPage
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPageModule> MixPageModule
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPagePost> MixPagePost
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPortalPage> MixPortalPage
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation> MixPortalPageNavigation
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPortalPageRole> MixPortalPageRole
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPost> MixPost
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPostMedia> MixPostMedia
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPostModule> MixPostModule
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData> MixRelatedAttributeData
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet> MixRelatedAttributeSet
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixRelatedData> MixRelatedData
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixRelatedPost> MixRelatedPost
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixTemplate> MixTemplate
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixTheme> MixTheme
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixUrlAlias> MixUrlAlias
		{
			get;
			set;
		}

		public MixCmsContext(DbContextOptions<MixCmsContext> options)
		{
			base(options);
			return;
		}

		public MixCmsContext()
		{
			base();
			return;
		}

		public override void Dispose()
		{
			switch (Enum.Parse<MixEnums.DatabaseProvider>(MixService.GetConfig<string>("DatabaseProvider")))
			{
				case 0:
				{
					SqlConnection.ClearPool((SqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.get_Database()));
					break;
				}
				case 1:
				{
					MySqlConnection.ClearPool((MySqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.get_Database()));
					break;
				}
				case 2:
				{
					NpgsqlConnection.ClearPool((NpgsqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.get_Database()));
					break;
				}
			}
			this.Dispose();
			return;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			dummyVar0 = optionsBuilder.EnableSensitiveDataLogging(true);
			V_0 = MixService.GetConnectionString("MixCmsConnection");
			if (!string.IsNullOrEmpty(V_0))
			{
				switch (Enum.Parse<MixEnums.DatabaseProvider>(MixService.GetConfig<string>("DatabaseProvider")))
				{
					case 0:
					{
						dummyVar1 = SqlServerDbContextOptionsExtensions.UseSqlServer(optionsBuilder, V_0, null);
						return;
					}
					case 1:
					{
						dummyVar2 = MySqlDbContextOptionsExtensions.UseMySql(optionsBuilder, V_0, null);
						return;
					}
					case 2:
					{
						dummyVar3 = NpgsqlDbContextOptionsExtensions.UseNpgsql(optionsBuilder, V_0, null);
						break;
					}
					default:
					{
						return;
					}
				}
			}
			return;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			stackVariable0 = modelBuilder;
			stackVariable1 = MixCmsContext.u003cu003ec.u003cu003e9__128_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeField>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_0);
				MixCmsContext.u003cu003ec.u003cu003e9__128_0 = stackVariable1;
			}
			dummyVar1 = stackVariable0.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeField>(stackVariable1);
			stackVariable3 = modelBuilder;
			stackVariable4 = MixCmsContext.u003cu003ec.u003cu003e9__128_1;
			if (stackVariable4 == null)
			{
				dummyVar2 = stackVariable4;
				stackVariable4 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSet>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_1);
				MixCmsContext.u003cu003ec.u003cu003e9__128_1 = stackVariable4;
			}
			dummyVar3 = stackVariable3.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeSet>(stackVariable4);
			stackVariable6 = modelBuilder;
			stackVariable7 = MixCmsContext.u003cu003ec.u003cu003e9__128_2;
			if (stackVariable7 == null)
			{
				dummyVar4 = stackVariable7;
				stackVariable7 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSetData>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_2);
				MixCmsContext.u003cu003ec.u003cu003e9__128_2 = stackVariable7;
			}
			dummyVar5 = stackVariable6.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeSetData>(stackVariable7);
			stackVariable9 = modelBuilder;
			stackVariable10 = MixCmsContext.u003cu003ec.u003cu003e9__128_3;
			if (stackVariable10 == null)
			{
				dummyVar6 = stackVariable10;
				stackVariable10 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_3);
				MixCmsContext.u003cu003ec.u003cu003e9__128_3 = stackVariable10;
			}
			dummyVar7 = stackVariable9.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference>(stackVariable10);
			stackVariable12 = modelBuilder;
			stackVariable13 = MixCmsContext.u003cu003ec.u003cu003e9__128_4;
			if (stackVariable13 == null)
			{
				dummyVar8 = stackVariable13;
				stackVariable13 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSetValue>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_4);
				MixCmsContext.u003cu003ec.u003cu003e9__128_4 = stackVariable13;
			}
			dummyVar9 = stackVariable12.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeSetValue>(stackVariable13);
			stackVariable15 = modelBuilder;
			stackVariable16 = MixCmsContext.u003cu003ec.u003cu003e9__128_5;
			if (stackVariable16 == null)
			{
				dummyVar10 = stackVariable16;
				stackVariable16 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixCache>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_5);
				MixCmsContext.u003cu003ec.u003cu003e9__128_5 = stackVariable16;
			}
			dummyVar11 = stackVariable15.Entity<Mix.Cms.Lib.Models.Cms.MixCache>(stackVariable16);
			stackVariable18 = modelBuilder;
			stackVariable19 = MixCmsContext.u003cu003ec.u003cu003e9__128_6;
			if (stackVariable19 == null)
			{
				dummyVar12 = stackVariable19;
				stackVariable19 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixCmsUser>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_6);
				MixCmsContext.u003cu003ec.u003cu003e9__128_6 = stackVariable19;
			}
			dummyVar13 = stackVariable18.Entity<Mix.Cms.Lib.Models.Cms.MixCmsUser>(stackVariable19);
			stackVariable21 = modelBuilder;
			stackVariable22 = MixCmsContext.u003cu003ec.u003cu003e9__128_7;
			if (stackVariable22 == null)
			{
				dummyVar14 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixConfiguration>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_7);
				MixCmsContext.u003cu003ec.u003cu003e9__128_7 = stackVariable22;
			}
			dummyVar15 = stackVariable21.Entity<Mix.Cms.Lib.Models.Cms.MixConfiguration>(stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable25 = MixCmsContext.u003cu003ec.u003cu003e9__128_8;
			if (stackVariable25 == null)
			{
				dummyVar16 = stackVariable25;
				stackVariable25 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixCulture>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_8);
				MixCmsContext.u003cu003ec.u003cu003e9__128_8 = stackVariable25;
			}
			dummyVar17 = stackVariable24.Entity<Mix.Cms.Lib.Models.Cms.MixCulture>(stackVariable25);
			stackVariable27 = modelBuilder;
			stackVariable28 = MixCmsContext.u003cu003ec.u003cu003e9__128_9;
			if (stackVariable28 == null)
			{
				dummyVar18 = stackVariable28;
				stackVariable28 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixFile>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_9);
				MixCmsContext.u003cu003ec.u003cu003e9__128_9 = stackVariable28;
			}
			dummyVar19 = stackVariable27.Entity<Mix.Cms.Lib.Models.Cms.MixFile>(stackVariable28);
			stackVariable30 = modelBuilder;
			stackVariable31 = MixCmsContext.u003cu003ec.u003cu003e9__128_10;
			if (stackVariable31 == null)
			{
				dummyVar20 = stackVariable31;
				stackVariable31 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixLanguage>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_10);
				MixCmsContext.u003cu003ec.u003cu003e9__128_10 = stackVariable31;
			}
			dummyVar21 = stackVariable30.Entity<Mix.Cms.Lib.Models.Cms.MixLanguage>(stackVariable31);
			stackVariable33 = modelBuilder;
			stackVariable34 = MixCmsContext.u003cu003ec.u003cu003e9__128_11;
			if (stackVariable34 == null)
			{
				dummyVar22 = stackVariable34;
				stackVariable34 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixMedia>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_11);
				MixCmsContext.u003cu003ec.u003cu003e9__128_11 = stackVariable34;
			}
			dummyVar23 = stackVariable33.Entity<Mix.Cms.Lib.Models.Cms.MixMedia>(stackVariable34);
			stackVariable36 = modelBuilder;
			stackVariable37 = MixCmsContext.u003cu003ec.u003cu003e9__128_12;
			if (stackVariable37 == null)
			{
				dummyVar24 = stackVariable37;
				stackVariable37 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixModule>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_12);
				MixCmsContext.u003cu003ec.u003cu003e9__128_12 = stackVariable37;
			}
			dummyVar25 = stackVariable36.Entity<Mix.Cms.Lib.Models.Cms.MixModule>(stackVariable37);
			stackVariable39 = modelBuilder;
			stackVariable40 = MixCmsContext.u003cu003ec.u003cu003e9__128_13;
			if (stackVariable40 == null)
			{
				dummyVar26 = stackVariable40;
				stackVariable40 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixModuleData>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_13);
				MixCmsContext.u003cu003ec.u003cu003e9__128_13 = stackVariable40;
			}
			dummyVar27 = stackVariable39.Entity<Mix.Cms.Lib.Models.Cms.MixModuleData>(stackVariable40);
			stackVariable42 = modelBuilder;
			stackVariable43 = MixCmsContext.u003cu003ec.u003cu003e9__128_14;
			if (stackVariable43 == null)
			{
				dummyVar28 = stackVariable43;
				stackVariable43 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixModulePost>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_14);
				MixCmsContext.u003cu003ec.u003cu003e9__128_14 = stackVariable43;
			}
			dummyVar29 = stackVariable42.Entity<Mix.Cms.Lib.Models.Cms.MixModulePost>(stackVariable43);
			stackVariable45 = modelBuilder;
			stackVariable46 = MixCmsContext.u003cu003ec.u003cu003e9__128_15;
			if (stackVariable46 == null)
			{
				dummyVar30 = stackVariable46;
				stackVariable46 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPage>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_15);
				MixCmsContext.u003cu003ec.u003cu003e9__128_15 = stackVariable46;
			}
			dummyVar31 = stackVariable45.Entity<Mix.Cms.Lib.Models.Cms.MixPage>(stackVariable46);
			stackVariable48 = modelBuilder;
			stackVariable49 = MixCmsContext.u003cu003ec.u003cu003e9__128_16;
			if (stackVariable49 == null)
			{
				dummyVar32 = stackVariable49;
				stackVariable49 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPageModule>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_16);
				MixCmsContext.u003cu003ec.u003cu003e9__128_16 = stackVariable49;
			}
			dummyVar33 = stackVariable48.Entity<Mix.Cms.Lib.Models.Cms.MixPageModule>(stackVariable49);
			stackVariable51 = modelBuilder;
			stackVariable52 = MixCmsContext.u003cu003ec.u003cu003e9__128_17;
			if (stackVariable52 == null)
			{
				dummyVar34 = stackVariable52;
				stackVariable52 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPagePost>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_17);
				MixCmsContext.u003cu003ec.u003cu003e9__128_17 = stackVariable52;
			}
			dummyVar35 = stackVariable51.Entity<Mix.Cms.Lib.Models.Cms.MixPagePost>(stackVariable52);
			stackVariable54 = modelBuilder;
			stackVariable55 = MixCmsContext.u003cu003ec.u003cu003e9__128_18;
			if (stackVariable55 == null)
			{
				dummyVar36 = stackVariable55;
				stackVariable55 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPortalPage>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_18);
				MixCmsContext.u003cu003ec.u003cu003e9__128_18 = stackVariable55;
			}
			dummyVar37 = stackVariable54.Entity<Mix.Cms.Lib.Models.Cms.MixPortalPage>(stackVariable55);
			stackVariable57 = modelBuilder;
			stackVariable58 = MixCmsContext.u003cu003ec.u003cu003e9__128_19;
			if (stackVariable58 == null)
			{
				dummyVar38 = stackVariable58;
				stackVariable58 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_19);
				MixCmsContext.u003cu003ec.u003cu003e9__128_19 = stackVariable58;
			}
			dummyVar39 = stackVariable57.Entity<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation>(stackVariable58);
			stackVariable60 = modelBuilder;
			stackVariable61 = MixCmsContext.u003cu003ec.u003cu003e9__128_20;
			if (stackVariable61 == null)
			{
				dummyVar40 = stackVariable61;
				stackVariable61 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPortalPageRole>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_20);
				MixCmsContext.u003cu003ec.u003cu003e9__128_20 = stackVariable61;
			}
			dummyVar41 = stackVariable60.Entity<Mix.Cms.Lib.Models.Cms.MixPortalPageRole>(stackVariable61);
			stackVariable63 = modelBuilder;
			stackVariable64 = MixCmsContext.u003cu003ec.u003cu003e9__128_21;
			if (stackVariable64 == null)
			{
				dummyVar42 = stackVariable64;
				stackVariable64 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPost>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_21);
				MixCmsContext.u003cu003ec.u003cu003e9__128_21 = stackVariable64;
			}
			dummyVar43 = stackVariable63.Entity<Mix.Cms.Lib.Models.Cms.MixPost>(stackVariable64);
			stackVariable66 = modelBuilder;
			stackVariable67 = MixCmsContext.u003cu003ec.u003cu003e9__128_22;
			if (stackVariable67 == null)
			{
				dummyVar44 = stackVariable67;
				stackVariable67 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPostMedia>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_22);
				MixCmsContext.u003cu003ec.u003cu003e9__128_22 = stackVariable67;
			}
			dummyVar45 = stackVariable66.Entity<Mix.Cms.Lib.Models.Cms.MixPostMedia>(stackVariable67);
			stackVariable69 = modelBuilder;
			stackVariable70 = MixCmsContext.u003cu003ec.u003cu003e9__128_23;
			if (stackVariable70 == null)
			{
				dummyVar46 = stackVariable70;
				stackVariable70 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPostModule>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_23);
				MixCmsContext.u003cu003ec.u003cu003e9__128_23 = stackVariable70;
			}
			dummyVar47 = stackVariable69.Entity<Mix.Cms.Lib.Models.Cms.MixPostModule>(stackVariable70);
			stackVariable72 = modelBuilder;
			stackVariable73 = MixCmsContext.u003cu003ec.u003cu003e9__128_24;
			if (stackVariable73 == null)
			{
				dummyVar48 = stackVariable73;
				stackVariable73 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_24);
				MixCmsContext.u003cu003ec.u003cu003e9__128_24 = stackVariable73;
			}
			dummyVar49 = stackVariable72.Entity<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData>(stackVariable73);
			stackVariable75 = modelBuilder;
			stackVariable76 = MixCmsContext.u003cu003ec.u003cu003e9__128_25;
			if (stackVariable76 == null)
			{
				dummyVar50 = stackVariable76;
				stackVariable76 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_25);
				MixCmsContext.u003cu003ec.u003cu003e9__128_25 = stackVariable76;
			}
			dummyVar51 = stackVariable75.Entity<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet>(stackVariable76);
			stackVariable78 = modelBuilder;
			stackVariable79 = MixCmsContext.u003cu003ec.u003cu003e9__128_26;
			if (stackVariable79 == null)
			{
				dummyVar52 = stackVariable79;
				stackVariable79 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedData>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_26);
				MixCmsContext.u003cu003ec.u003cu003e9__128_26 = stackVariable79;
			}
			dummyVar53 = stackVariable78.Entity<Mix.Cms.Lib.Models.Cms.MixRelatedData>(stackVariable79);
			stackVariable81 = modelBuilder;
			stackVariable82 = MixCmsContext.u003cu003ec.u003cu003e9__128_27;
			if (stackVariable82 == null)
			{
				dummyVar54 = stackVariable82;
				stackVariable82 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedPost>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_27);
				MixCmsContext.u003cu003ec.u003cu003e9__128_27 = stackVariable82;
			}
			dummyVar55 = stackVariable81.Entity<Mix.Cms.Lib.Models.Cms.MixRelatedPost>(stackVariable82);
			stackVariable84 = modelBuilder;
			stackVariable85 = MixCmsContext.u003cu003ec.u003cu003e9__128_28;
			if (stackVariable85 == null)
			{
				dummyVar56 = stackVariable85;
				stackVariable85 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixTemplate>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_28);
				MixCmsContext.u003cu003ec.u003cu003e9__128_28 = stackVariable85;
			}
			dummyVar57 = stackVariable84.Entity<Mix.Cms.Lib.Models.Cms.MixTemplate>(stackVariable85);
			stackVariable87 = modelBuilder;
			stackVariable88 = MixCmsContext.u003cu003ec.u003cu003e9__128_29;
			if (stackVariable88 == null)
			{
				dummyVar58 = stackVariable88;
				stackVariable88 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixTheme>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_29);
				MixCmsContext.u003cu003ec.u003cu003e9__128_29 = stackVariable88;
			}
			dummyVar59 = stackVariable87.Entity<Mix.Cms.Lib.Models.Cms.MixTheme>(stackVariable88);
			stackVariable90 = modelBuilder;
			stackVariable91 = MixCmsContext.u003cu003ec.u003cu003e9__128_30;
			if (stackVariable91 == null)
			{
				dummyVar60 = stackVariable91;
				stackVariable91 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixUrlAlias>>(MixCmsContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__128_30);
				MixCmsContext.u003cu003ec.u003cu003e9__128_30 = stackVariable91;
			}
			dummyVar61 = stackVariable90.Entity<Mix.Cms.Lib.Models.Cms.MixUrlAlias>(stackVariable91);
			return;
		}
	}
}