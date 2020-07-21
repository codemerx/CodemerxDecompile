using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Mix.Cms.Lib.Models.Account;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Migrations.MixCmsAccount
{
	[DbContext(typeof(MixCmsAccountContext))]
	[Migration("20200312035845_Init")]
	public class Init : Migration
	{
		public Init()
		{
			base();
			return;
		}

		protected override void BuildTargetModel(ModelBuilder modelBuilder)
		{
			dummyVar0 = modelBuilder.HasAnnotation("ProductVersion", "3.1.2").HasAnnotation("Relational:MaxIdentifierLength", 128).HasAnnotation("SqlServer:ValueGenerationStrategy", (SqlServerValueGenerationStrategy)2);
			stackVariable12 = modelBuilder;
			stackVariable14 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_0;
			if (stackVariable14 == null)
			{
				dummyVar1 = stackVariable14;
				stackVariable14 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_0);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_0 = stackVariable14;
			}
			dummyVar2 = stackVariable12.Entity("Mix.Cms.Lib.Models.Account.AspNetRoleClaims", stackVariable14);
			stackVariable16 = modelBuilder;
			stackVariable18 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_1;
			if (stackVariable18 == null)
			{
				dummyVar3 = stackVariable18;
				stackVariable18 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_1);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_1 = stackVariable18;
			}
			dummyVar4 = stackVariable16.Entity("Mix.Cms.Lib.Models.Account.AspNetRoles", stackVariable18);
			stackVariable20 = modelBuilder;
			stackVariable22 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_2;
			if (stackVariable22 == null)
			{
				dummyVar5 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_2);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_2 = stackVariable22;
			}
			dummyVar6 = stackVariable20.Entity("Mix.Cms.Lib.Models.Account.AspNetUserClaims", stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable26 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_3;
			if (stackVariable26 == null)
			{
				dummyVar7 = stackVariable26;
				stackVariable26 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_3);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_3 = stackVariable26;
			}
			dummyVar8 = stackVariable24.Entity("Mix.Cms.Lib.Models.Account.AspNetUserLogins", stackVariable26);
			stackVariable28 = modelBuilder;
			stackVariable30 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_4;
			if (stackVariable30 == null)
			{
				dummyVar9 = stackVariable30;
				stackVariable30 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_4);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_4 = stackVariable30;
			}
			dummyVar10 = stackVariable28.Entity("Mix.Cms.Lib.Models.Account.AspNetUserRoles", stackVariable30);
			stackVariable32 = modelBuilder;
			stackVariable34 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_5;
			if (stackVariable34 == null)
			{
				dummyVar11 = stackVariable34;
				stackVariable34 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_5);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_5 = stackVariable34;
			}
			dummyVar12 = stackVariable32.Entity("Mix.Cms.Lib.Models.Account.AspNetUserTokens", stackVariable34);
			stackVariable36 = modelBuilder;
			stackVariable38 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_6;
			if (stackVariable38 == null)
			{
				dummyVar13 = stackVariable38;
				stackVariable38 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_6);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_6 = stackVariable38;
			}
			dummyVar14 = stackVariable36.Entity("Mix.Cms.Lib.Models.Account.AspNetUsers", stackVariable38);
			stackVariable40 = modelBuilder;
			stackVariable42 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_7;
			if (stackVariable42 == null)
			{
				dummyVar15 = stackVariable42;
				stackVariable42 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_7);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_7 = stackVariable42;
			}
			dummyVar16 = stackVariable40.Entity("Mix.Cms.Lib.Models.Account.Clients", stackVariable42);
			stackVariable44 = modelBuilder;
			stackVariable46 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_8;
			if (stackVariable46 == null)
			{
				dummyVar17 = stackVariable46;
				stackVariable46 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_8);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_8 = stackVariable46;
			}
			dummyVar18 = stackVariable44.Entity("Mix.Cms.Lib.Models.Account.RefreshTokens", stackVariable46);
			stackVariable48 = modelBuilder;
			stackVariable50 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_9;
			if (stackVariable50 == null)
			{
				dummyVar19 = stackVariable50;
				stackVariable50 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_9);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_9 = stackVariable50;
			}
			dummyVar20 = stackVariable48.Entity("Mix.Cms.Lib.Models.Account.AspNetRoleClaims", stackVariable50);
			stackVariable52 = modelBuilder;
			stackVariable54 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_10;
			if (stackVariable54 == null)
			{
				dummyVar21 = stackVariable54;
				stackVariable54 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_10);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_10 = stackVariable54;
			}
			dummyVar22 = stackVariable52.Entity("Mix.Cms.Lib.Models.Account.AspNetUserClaims", stackVariable54);
			stackVariable56 = modelBuilder;
			stackVariable58 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_11;
			if (stackVariable58 == null)
			{
				dummyVar23 = stackVariable58;
				stackVariable58 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_11);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_11 = stackVariable58;
			}
			dummyVar24 = stackVariable56.Entity("Mix.Cms.Lib.Models.Account.AspNetUserLogins", stackVariable58);
			stackVariable60 = modelBuilder;
			stackVariable62 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_12;
			if (stackVariable62 == null)
			{
				dummyVar25 = stackVariable62;
				stackVariable62 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_12);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_12 = stackVariable62;
			}
			dummyVar26 = stackVariable60.Entity("Mix.Cms.Lib.Models.Account.AspNetUserRoles", stackVariable62);
			stackVariable64 = modelBuilder;
			stackVariable66 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_13;
			if (stackVariable66 == null)
			{
				dummyVar27 = stackVariable66;
				stackVariable66 = new Action<EntityTypeBuilder>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cBuildTargetModelu003eb__2_13);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__2_13 = stackVariable66;
			}
			dummyVar28 = stackVariable64.Entity("Mix.Cms.Lib.Models.Account.AspNetUserTokens", stackVariable66);
			return;
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			dummyVar0 = migrationBuilder.DropTable("AspNetRoleClaims", null);
			dummyVar1 = migrationBuilder.DropTable("AspNetUserClaims", null);
			dummyVar2 = migrationBuilder.DropTable("AspNetUserLogins", null);
			dummyVar3 = migrationBuilder.DropTable("AspNetUserRoles", null);
			dummyVar4 = migrationBuilder.DropTable("AspNetUserTokens", null);
			dummyVar5 = migrationBuilder.DropTable("Clients", null);
			dummyVar6 = migrationBuilder.DropTable("RefreshTokens", null);
			dummyVar7 = migrationBuilder.DropTable("AspNetRoles", null);
			dummyVar8 = migrationBuilder.DropTable("AspNetUsers", null);
			return;
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			stackVariable0 = migrationBuilder;
			stackVariable2 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType39<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_0);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_0 = stackVariable2;
			}
			stackVariable4 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_1;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType39<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_1);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_1 = stackVariable4;
			}
			dummyVar2 = stackVariable0.CreateTable("AspNetRoles", stackVariable2, null, stackVariable4, null);
			stackVariable7 = migrationBuilder;
			stackVariable9 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_2;
			if (stackVariable9 == null)
			{
				dummyVar3 = stackVariable9;
				stackVariable9 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType40<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_2);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_2 = stackVariable9;
			}
			stackVariable11 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_3;
			if (stackVariable11 == null)
			{
				dummyVar4 = stackVariable11;
				stackVariable11 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType40<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_3);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_3 = stackVariable11;
			}
			dummyVar5 = stackVariable7.CreateTable("AspNetUsers", stackVariable9, null, stackVariable11, null);
			stackVariable14 = migrationBuilder;
			stackVariable16 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_4;
			if (stackVariable16 == null)
			{
				dummyVar6 = stackVariable16;
				stackVariable16 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType41<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_4);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_4 = stackVariable16;
			}
			stackVariable18 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_5;
			if (stackVariable18 == null)
			{
				dummyVar7 = stackVariable18;
				stackVariable18 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType41<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_5);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_5 = stackVariable18;
			}
			dummyVar8 = stackVariable14.CreateTable("Clients", stackVariable16, null, stackVariable18, null);
			stackVariable21 = migrationBuilder;
			stackVariable23 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_6;
			if (stackVariable23 == null)
			{
				dummyVar9 = stackVariable23;
				stackVariable23 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType42<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_6);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_6 = stackVariable23;
			}
			stackVariable25 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_7;
			if (stackVariable25 == null)
			{
				dummyVar10 = stackVariable25;
				stackVariable25 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType42<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_7);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_7 = stackVariable25;
			}
			dummyVar11 = stackVariable21.CreateTable("RefreshTokens", stackVariable23, null, stackVariable25, null);
			stackVariable28 = migrationBuilder;
			stackVariable30 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_8;
			if (stackVariable30 == null)
			{
				dummyVar12 = stackVariable30;
				stackVariable30 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType43<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_8);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_8 = stackVariable30;
			}
			stackVariable32 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_9;
			if (stackVariable32 == null)
			{
				dummyVar13 = stackVariable32;
				stackVariable32 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType43<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_9);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_9 = stackVariable32;
			}
			dummyVar14 = stackVariable28.CreateTable("AspNetRoleClaims", stackVariable30, null, stackVariable32, null);
			stackVariable35 = migrationBuilder;
			stackVariable37 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_10;
			if (stackVariable37 == null)
			{
				dummyVar15 = stackVariable37;
				stackVariable37 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType44<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_10);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_10 = stackVariable37;
			}
			stackVariable39 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_11;
			if (stackVariable39 == null)
			{
				dummyVar16 = stackVariable39;
				stackVariable39 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType44<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_11);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_11 = stackVariable39;
			}
			dummyVar17 = stackVariable35.CreateTable("AspNetUserClaims", stackVariable37, null, stackVariable39, null);
			stackVariable42 = migrationBuilder;
			stackVariable44 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_12;
			if (stackVariable44 == null)
			{
				dummyVar18 = stackVariable44;
				stackVariable44 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType45<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_12);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_12 = stackVariable44;
			}
			stackVariable46 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_13;
			if (stackVariable46 == null)
			{
				dummyVar19 = stackVariable46;
				stackVariable46 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType45<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_13);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_13 = stackVariable46;
			}
			dummyVar20 = stackVariable42.CreateTable("AspNetUserLogins", stackVariable44, null, stackVariable46, null);
			stackVariable49 = migrationBuilder;
			stackVariable51 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_14;
			if (stackVariable51 == null)
			{
				dummyVar21 = stackVariable51;
				stackVariable51 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType47<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_14);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_14 = stackVariable51;
			}
			stackVariable53 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_15;
			if (stackVariable53 == null)
			{
				dummyVar22 = stackVariable53;
				stackVariable53 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType47<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_15);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_15 = stackVariable53;
			}
			dummyVar23 = stackVariable49.CreateTable("AspNetUserRoles", stackVariable51, null, stackVariable53, null);
			stackVariable56 = migrationBuilder;
			stackVariable58 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_16;
			if (stackVariable58 == null)
			{
				dummyVar24 = stackVariable58;
				stackVariable58 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType49<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_16);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_16 = stackVariable58;
			}
			stackVariable60 = Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_17;
			if (stackVariable60 == null)
			{
				dummyVar25 = stackVariable60;
				stackVariable60 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType49<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9, Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cUpu003eb__0_17);
				Mix.Cms.Lib.Migrations.MixCmsAccount.Init.u003cu003ec.u003cu003e9__0_17 = stackVariable60;
			}
			dummyVar26 = stackVariable56.CreateTable("AspNetUserTokens", stackVariable58, null, stackVariable60, null);
			dummyVar27 = migrationBuilder.CreateIndex("IX_AspNetRoleClaims_RoleId", "AspNetRoleClaims", "RoleId", null, false, null);
			dummyVar28 = migrationBuilder.CreateIndex("RoleNameIndex", "AspNetRoles", "NormalizedName", null, true, "([NormalizedName] IS NOT NULL)");
			dummyVar29 = migrationBuilder.CreateIndex("IX_AspNetUserClaims_ApplicationUserId", "AspNetUserClaims", "ApplicationUserId", null, false, null);
			dummyVar30 = migrationBuilder.CreateIndex("IX_AspNetUserClaims_UserId", "AspNetUserClaims", "UserId", null, false, null);
			dummyVar31 = migrationBuilder.CreateIndex("IX_AspNetUserLogins_ApplicationUserId", "AspNetUserLogins", "ApplicationUserId", null, false, null);
			dummyVar32 = migrationBuilder.CreateIndex("IX_AspNetUserLogins_UserId", "AspNetUserLogins", "UserId", null, false, null);
			dummyVar33 = migrationBuilder.CreateIndex("IX_AspNetUserRoles_ApplicationUserId", "AspNetUserRoles", "ApplicationUserId", null, false, null);
			dummyVar34 = migrationBuilder.CreateIndex("IX_AspNetUserRoles_RoleId", "AspNetUserRoles", "RoleId", null, false, null);
			dummyVar35 = migrationBuilder.CreateIndex("EmailIndex", "AspNetUsers", "NormalizedEmail", null, false, null);
			dummyVar36 = migrationBuilder.CreateIndex("UserNameIndex", "AspNetUsers", "NormalizedUserName", null, true, "([NormalizedUserName] IS NOT NULL)");
			return;
		}
	}
}