using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Lib;
using Mix.Identity.Data;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class MixCmsAccountContext : DbContext
	{
		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetRoleClaims> AspNetRoleClaims
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetRoles> AspNetRoles
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUserClaims> AspNetUserClaims
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUserLogins> AspNetUserLogins
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUserRoles> AspNetUserRoles
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUsers> AspNetUsers
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUserTokens> AspNetUserTokens
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.Clients> Clients
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.RefreshTokens> RefreshTokens
		{
			get;
			set;
		}

		public MixCmsAccountContext(DbContextOptions<ApplicationDbContext> options)
		{
			base(options);
			return;
		}

		public MixCmsAccountContext()
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
			V_0 = MixService.GetConnectionString("MixCmsConnection");
			if (!string.IsNullOrEmpty(V_0))
			{
				switch (Enum.Parse<MixEnums.DatabaseProvider>(MixService.GetConfig<string>("DatabaseProvider")))
				{
					case 0:
					{
						dummyVar0 = SqlServerDbContextOptionsExtensions.UseSqlServer(optionsBuilder, V_0, null);
						return;
					}
					case 1:
					{
						dummyVar1 = MySqlDbContextOptionsExtensions.UseMySql(optionsBuilder, V_0, null);
						return;
					}
					case 2:
					{
						dummyVar2 = NpgsqlDbContextOptionsExtensions.UseNpgsql(optionsBuilder, V_0, null);
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
			stackVariable1 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetRoleClaims>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_0);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_0 = stackVariable1;
			}
			dummyVar1 = stackVariable0.Entity<Mix.Cms.Lib.Models.Account.AspNetRoleClaims>(stackVariable1);
			stackVariable3 = modelBuilder;
			stackVariable4 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_1;
			if (stackVariable4 == null)
			{
				dummyVar2 = stackVariable4;
				stackVariable4 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetRoles>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_1);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_1 = stackVariable4;
			}
			dummyVar3 = stackVariable3.Entity<Mix.Cms.Lib.Models.Account.AspNetRoles>(stackVariable4);
			stackVariable6 = modelBuilder;
			stackVariable7 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_2;
			if (stackVariable7 == null)
			{
				dummyVar4 = stackVariable7;
				stackVariable7 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserClaims>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_2);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_2 = stackVariable7;
			}
			dummyVar5 = stackVariable6.Entity<Mix.Cms.Lib.Models.Account.AspNetUserClaims>(stackVariable7);
			stackVariable9 = modelBuilder;
			stackVariable10 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_3;
			if (stackVariable10 == null)
			{
				dummyVar6 = stackVariable10;
				stackVariable10 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserLogins>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_3);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_3 = stackVariable10;
			}
			dummyVar7 = stackVariable9.Entity<Mix.Cms.Lib.Models.Account.AspNetUserLogins>(stackVariable10);
			stackVariable12 = modelBuilder;
			stackVariable13 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_4;
			if (stackVariable13 == null)
			{
				dummyVar8 = stackVariable13;
				stackVariable13 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserRoles>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_4);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_4 = stackVariable13;
			}
			dummyVar9 = stackVariable12.Entity<Mix.Cms.Lib.Models.Account.AspNetUserRoles>(stackVariable13);
			stackVariable15 = modelBuilder;
			stackVariable16 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_5;
			if (stackVariable16 == null)
			{
				dummyVar10 = stackVariable16;
				stackVariable16 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserTokens>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_5);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_5 = stackVariable16;
			}
			dummyVar11 = stackVariable15.Entity<Mix.Cms.Lib.Models.Account.AspNetUserTokens>(stackVariable16);
			stackVariable18 = modelBuilder;
			stackVariable19 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_6;
			if (stackVariable19 == null)
			{
				dummyVar12 = stackVariable19;
				stackVariable19 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUsers>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_6);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_6 = stackVariable19;
			}
			dummyVar13 = stackVariable18.Entity<Mix.Cms.Lib.Models.Account.AspNetUsers>(stackVariable19);
			stackVariable21 = modelBuilder;
			stackVariable22 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_7;
			if (stackVariable22 == null)
			{
				dummyVar14 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.Clients>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_7);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_7 = stackVariable22;
			}
			dummyVar15 = stackVariable21.Entity<Mix.Cms.Lib.Models.Account.Clients>(stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable25 = MixCmsAccountContext.u003cu003ec.u003cu003e9__40_8;
			if (stackVariable25 == null)
			{
				dummyVar16 = stackVariable25;
				stackVariable25 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.RefreshTokens>>(MixCmsAccountContext.u003cu003ec.u003cu003e9, MixCmsAccountContext.u003cu003ec.u003cOnModelCreatingu003eb__40_8);
				MixCmsAccountContext.u003cu003ec.u003cu003e9__40_8 = stackVariable25;
			}
			dummyVar17 = stackVariable24.Entity<Mix.Cms.Lib.Models.Account.RefreshTokens>(stackVariable25);
			return;
		}
	}
}