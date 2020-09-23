using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class mix_structure_accountContext : DbContext
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

		public mix_structure_accountContext()
		{
			base();
			return;
		}

		public mix_structure_accountContext(DbContextOptions<mix_structure_accountContext> options)
		{
			base(options);
			return;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.get_IsConfigured())
			{
				dummyVar0 = SqlServerDbContextOptionsExtensions.UseSqlServer(optionsBuilder, "Server=115.79.139.90,4444;Database=mix_structure_account;UID=tinku;Pwd=1234qwe@;MultipleActiveResultSets=true;", null);
			}
			return;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			stackVariable0 = modelBuilder;
			stackVariable1 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetRoleClaims>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_0);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_0 = stackVariable1;
			}
			dummyVar1 = stackVariable0.Entity<Mix.Cms.Lib.Models.Account.AspNetRoleClaims>(stackVariable1);
			stackVariable3 = modelBuilder;
			stackVariable4 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_1;
			if (stackVariable4 == null)
			{
				dummyVar2 = stackVariable4;
				stackVariable4 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetRoles>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_1);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_1 = stackVariable4;
			}
			dummyVar3 = stackVariable3.Entity<Mix.Cms.Lib.Models.Account.AspNetRoles>(stackVariable4);
			stackVariable6 = modelBuilder;
			stackVariable7 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_2;
			if (stackVariable7 == null)
			{
				dummyVar4 = stackVariable7;
				stackVariable7 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserClaims>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_2);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_2 = stackVariable7;
			}
			dummyVar5 = stackVariable6.Entity<Mix.Cms.Lib.Models.Account.AspNetUserClaims>(stackVariable7);
			stackVariable9 = modelBuilder;
			stackVariable10 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_3;
			if (stackVariable10 == null)
			{
				dummyVar6 = stackVariable10;
				stackVariable10 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserLogins>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_3);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_3 = stackVariable10;
			}
			dummyVar7 = stackVariable9.Entity<Mix.Cms.Lib.Models.Account.AspNetUserLogins>(stackVariable10);
			stackVariable12 = modelBuilder;
			stackVariable13 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_4;
			if (stackVariable13 == null)
			{
				dummyVar8 = stackVariable13;
				stackVariable13 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserRoles>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_4);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_4 = stackVariable13;
			}
			dummyVar9 = stackVariable12.Entity<Mix.Cms.Lib.Models.Account.AspNetUserRoles>(stackVariable13);
			stackVariable15 = modelBuilder;
			stackVariable16 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_5;
			if (stackVariable16 == null)
			{
				dummyVar10 = stackVariable16;
				stackVariable16 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserTokens>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_5);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_5 = stackVariable16;
			}
			dummyVar11 = stackVariable15.Entity<Mix.Cms.Lib.Models.Account.AspNetUserTokens>(stackVariable16);
			stackVariable18 = modelBuilder;
			stackVariable19 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_6;
			if (stackVariable19 == null)
			{
				dummyVar12 = stackVariable19;
				stackVariable19 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUsers>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_6);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_6 = stackVariable19;
			}
			dummyVar13 = stackVariable18.Entity<Mix.Cms.Lib.Models.Account.AspNetUsers>(stackVariable19);
			stackVariable21 = modelBuilder;
			stackVariable22 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_7;
			if (stackVariable22 == null)
			{
				dummyVar14 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.Clients>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_7);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_7 = stackVariable22;
			}
			dummyVar15 = stackVariable21.Entity<Mix.Cms.Lib.Models.Account.Clients>(stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable25 = mix_structure_accountContext.u003cu003ec.u003cu003e9__39_8;
			if (stackVariable25 == null)
			{
				dummyVar16 = stackVariable25;
				stackVariable25 = new Action<EntityTypeBuilder<Mix.Cms.Lib.Models.Account.RefreshTokens>>(mix_structure_accountContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__39_8);
				mix_structure_accountContext.u003cu003ec.u003cu003e9__39_8 = stackVariable25;
			}
			dummyVar17 = stackVariable24.Entity<Mix.Cms.Lib.Models.Account.RefreshTokens>(stackVariable25);
			return;
		}
	}
}