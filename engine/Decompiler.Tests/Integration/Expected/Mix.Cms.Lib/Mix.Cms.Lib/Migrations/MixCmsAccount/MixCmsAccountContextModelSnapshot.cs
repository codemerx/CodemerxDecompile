using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Lib.Models.Account;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Migrations.MixCmsAccount
{
	[DbContext(typeof(MixCmsAccountContext))]
	internal class MixCmsAccountContextModelSnapshot : ModelSnapshot
	{
		public MixCmsAccountContextModelSnapshot()
		{
			base();
			return;
		}

		protected override void BuildModel(ModelBuilder modelBuilder)
		{
			dummyVar0 = modelBuilder.HasAnnotation("ProductVersion", "3.1.2").HasAnnotation("Relational:MaxIdentifierLength", 128).HasAnnotation("SqlServer:ValueGenerationStrategy", (SqlServerValueGenerationStrategy)2);
			stackVariable12 = modelBuilder;
			stackVariable14 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable14 == null)
			{
				dummyVar1 = stackVariable14;
				stackVariable14 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_0);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_0 = stackVariable14;
			}
			dummyVar2 = stackVariable12.Entity("Mix.Cms.Lib.Models.Account.AspNetRoleClaims", stackVariable14);
			stackVariable16 = modelBuilder;
			stackVariable18 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_1;
			if (stackVariable18 == null)
			{
				dummyVar3 = stackVariable18;
				stackVariable18 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_1);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_1 = stackVariable18;
			}
			dummyVar4 = stackVariable16.Entity("Mix.Cms.Lib.Models.Account.AspNetRoles", stackVariable18);
			stackVariable20 = modelBuilder;
			stackVariable22 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_2;
			if (stackVariable22 == null)
			{
				dummyVar5 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_2);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_2 = stackVariable22;
			}
			dummyVar6 = stackVariable20.Entity("Mix.Cms.Lib.Models.Account.AspNetUserClaims", stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable26 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_3;
			if (stackVariable26 == null)
			{
				dummyVar7 = stackVariable26;
				stackVariable26 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_3);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_3 = stackVariable26;
			}
			dummyVar8 = stackVariable24.Entity("Mix.Cms.Lib.Models.Account.AspNetUserLogins", stackVariable26);
			stackVariable28 = modelBuilder;
			stackVariable30 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_4;
			if (stackVariable30 == null)
			{
				dummyVar9 = stackVariable30;
				stackVariable30 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_4);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_4 = stackVariable30;
			}
			dummyVar10 = stackVariable28.Entity("Mix.Cms.Lib.Models.Account.AspNetUserRoles", stackVariable30);
			stackVariable32 = modelBuilder;
			stackVariable34 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_5;
			if (stackVariable34 == null)
			{
				dummyVar11 = stackVariable34;
				stackVariable34 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_5);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_5 = stackVariable34;
			}
			dummyVar12 = stackVariable32.Entity("Mix.Cms.Lib.Models.Account.AspNetUserTokens", stackVariable34);
			stackVariable36 = modelBuilder;
			stackVariable38 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_6;
			if (stackVariable38 == null)
			{
				dummyVar13 = stackVariable38;
				stackVariable38 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_6);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_6 = stackVariable38;
			}
			dummyVar14 = stackVariable36.Entity("Mix.Cms.Lib.Models.Account.AspNetUsers", stackVariable38);
			stackVariable40 = modelBuilder;
			stackVariable42 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_7;
			if (stackVariable42 == null)
			{
				dummyVar15 = stackVariable42;
				stackVariable42 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_7);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_7 = stackVariable42;
			}
			dummyVar16 = stackVariable40.Entity("Mix.Cms.Lib.Models.Account.Clients", stackVariable42);
			stackVariable44 = modelBuilder;
			stackVariable46 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_8;
			if (stackVariable46 == null)
			{
				dummyVar17 = stackVariable46;
				stackVariable46 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_8);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_8 = stackVariable46;
			}
			dummyVar18 = stackVariable44.Entity("Mix.Cms.Lib.Models.Account.RefreshTokens", stackVariable46);
			stackVariable48 = modelBuilder;
			stackVariable50 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_9;
			if (stackVariable50 == null)
			{
				dummyVar19 = stackVariable50;
				stackVariable50 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_9);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_9 = stackVariable50;
			}
			dummyVar20 = stackVariable48.Entity("Mix.Cms.Lib.Models.Account.AspNetRoleClaims", stackVariable50);
			stackVariable52 = modelBuilder;
			stackVariable54 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_10;
			if (stackVariable54 == null)
			{
				dummyVar21 = stackVariable54;
				stackVariable54 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_10);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_10 = stackVariable54;
			}
			dummyVar22 = stackVariable52.Entity("Mix.Cms.Lib.Models.Account.AspNetUserClaims", stackVariable54);
			stackVariable56 = modelBuilder;
			stackVariable58 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_11;
			if (stackVariable58 == null)
			{
				dummyVar23 = stackVariable58;
				stackVariable58 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_11);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_11 = stackVariable58;
			}
			dummyVar24 = stackVariable56.Entity("Mix.Cms.Lib.Models.Account.AspNetUserLogins", stackVariable58);
			stackVariable60 = modelBuilder;
			stackVariable62 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_12;
			if (stackVariable62 == null)
			{
				dummyVar25 = stackVariable62;
				stackVariable62 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_12);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_12 = stackVariable62;
			}
			dummyVar26 = stackVariable60.Entity("Mix.Cms.Lib.Models.Account.AspNetUserRoles", stackVariable62);
			stackVariable64 = modelBuilder;
			stackVariable66 = MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_13;
			if (stackVariable66 == null)
			{
				dummyVar27 = stackVariable66;
				stackVariable66 = new Action<EntityTypeBuilder>(MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_13);
				MixCmsAccountContextModelSnapshot.u003cu003ec.u003cu003e9__0_13 = stackVariable66;
			}
			dummyVar28 = stackVariable64.Entity("Mix.Cms.Lib.Models.Account.AspNetUserTokens", stackVariable66);
			return;
		}
	}
}