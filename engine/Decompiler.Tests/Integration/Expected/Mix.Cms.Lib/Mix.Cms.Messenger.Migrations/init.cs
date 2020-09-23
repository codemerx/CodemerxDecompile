using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Mix.Cms.Messenger.Models.Data;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Migrations
{
	[DbContext(typeof(MixChatServiceContext))]
	[Migration("20181119151731_init")]
	public class init : Migration
	{
		public init()
		{
			base();
			return;
		}

		protected override void BuildTargetModel(ModelBuilder modelBuilder)
		{
			dummyVar0 = modelBuilder.HasAnnotation("ProductVersion", "2.1.4-rtm-31024");
			stackVariable4 = modelBuilder;
			stackVariable6 = init.u003cu003ec.u003cu003e9__2_0;
			if (stackVariable6 == null)
			{
				dummyVar1 = stackVariable6;
				stackVariable6 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_0);
				init.u003cu003ec.u003cu003e9__2_0 = stackVariable6;
			}
			dummyVar2 = stackVariable4.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom", stackVariable6);
			stackVariable8 = modelBuilder;
			stackVariable10 = init.u003cu003ec.u003cu003e9__2_1;
			if (stackVariable10 == null)
			{
				dummyVar3 = stackVariable10;
				stackVariable10 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_1);
				init.u003cu003ec.u003cu003e9__2_1 = stackVariable10;
			}
			dummyVar4 = stackVariable8.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerMessage", stackVariable10);
			stackVariable12 = modelBuilder;
			stackVariable14 = init.u003cu003ec.u003cu003e9__2_2;
			if (stackVariable14 == null)
			{
				dummyVar5 = stackVariable14;
				stackVariable14 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_2);
				init.u003cu003ec.u003cu003e9__2_2 = stackVariable14;
			}
			dummyVar6 = stackVariable12.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser", stackVariable14);
			stackVariable16 = modelBuilder;
			stackVariable18 = init.u003cu003ec.u003cu003e9__2_3;
			if (stackVariable18 == null)
			{
				dummyVar7 = stackVariable18;
				stackVariable18 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_3);
				init.u003cu003ec.u003cu003e9__2_3 = stackVariable18;
			}
			dummyVar8 = stackVariable16.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser", stackVariable18);
			stackVariable20 = modelBuilder;
			stackVariable22 = init.u003cu003ec.u003cu003e9__2_4;
			if (stackVariable22 == null)
			{
				dummyVar9 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_4);
				init.u003cu003ec.u003cu003e9__2_4 = stackVariable22;
			}
			dummyVar10 = stackVariable20.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerTeam", stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable26 = init.u003cu003ec.u003cu003e9__2_5;
			if (stackVariable26 == null)
			{
				dummyVar11 = stackVariable26;
				stackVariable26 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_5);
				init.u003cu003ec.u003cu003e9__2_5 = stackVariable26;
			}
			dummyVar12 = stackVariable24.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerUser", stackVariable26);
			stackVariable28 = modelBuilder;
			stackVariable30 = init.u003cu003ec.u003cu003e9__2_6;
			if (stackVariable30 == null)
			{
				dummyVar13 = stackVariable30;
				stackVariable30 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_6);
				init.u003cu003ec.u003cu003e9__2_6 = stackVariable30;
			}
			dummyVar14 = stackVariable28.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice", stackVariable30);
			stackVariable32 = modelBuilder;
			stackVariable34 = init.u003cu003ec.u003cu003e9__2_7;
			if (stackVariable34 == null)
			{
				dummyVar15 = stackVariable34;
				stackVariable34 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_7);
				init.u003cu003ec.u003cu003e9__2_7 = stackVariable34;
			}
			dummyVar16 = stackVariable32.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerMessage", stackVariable34);
			stackVariable36 = modelBuilder;
			stackVariable38 = init.u003cu003ec.u003cu003e9__2_8;
			if (stackVariable38 == null)
			{
				dummyVar17 = stackVariable38;
				stackVariable38 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_8);
				init.u003cu003ec.u003cu003e9__2_8 = stackVariable38;
			}
			dummyVar18 = stackVariable36.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser", stackVariable38);
			stackVariable40 = modelBuilder;
			stackVariable42 = init.u003cu003ec.u003cu003e9__2_9;
			if (stackVariable42 == null)
			{
				dummyVar19 = stackVariable42;
				stackVariable42 = new Action<EntityTypeBuilder>(init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_9);
				init.u003cu003ec.u003cu003e9__2_9 = stackVariable42;
			}
			dummyVar20 = stackVariable40.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser", stackVariable42);
			return;
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			dummyVar0 = migrationBuilder.DropTable("mix_messenger_message", null);
			dummyVar1 = migrationBuilder.DropTable("mix_messenger_nav_room_user", null);
			dummyVar2 = migrationBuilder.DropTable("mix_messenger_nav_team_user", null);
			dummyVar3 = migrationBuilder.DropTable("mix_messenger_user_device", null);
			dummyVar4 = migrationBuilder.DropTable("mix_messenger_hub_room", null);
			dummyVar5 = migrationBuilder.DropTable("mix_messenger_team", null);
			dummyVar6 = migrationBuilder.DropTable("mix_messenger_user", null);
			return;
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			stackVariable0 = migrationBuilder;
			stackVariable2 = init.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType51<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_0);
				init.u003cu003ec.u003cu003e9__0_0 = stackVariable2;
			}
			stackVariable4 = init.u003cu003ec.u003cu003e9__0_1;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType51<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_1);
				init.u003cu003ec.u003cu003e9__0_1 = stackVariable4;
			}
			dummyVar2 = stackVariable0.CreateTable("mix_messenger_hub_room", stackVariable2, null, stackVariable4, null);
			stackVariable7 = migrationBuilder;
			stackVariable9 = init.u003cu003ec.u003cu003e9__0_2;
			if (stackVariable9 == null)
			{
				dummyVar3 = stackVariable9;
				stackVariable9 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType52<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_2);
				init.u003cu003ec.u003cu003e9__0_2 = stackVariable9;
			}
			stackVariable11 = init.u003cu003ec.u003cu003e9__0_3;
			if (stackVariable11 == null)
			{
				dummyVar4 = stackVariable11;
				stackVariable11 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType52<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_3);
				init.u003cu003ec.u003cu003e9__0_3 = stackVariable11;
			}
			dummyVar5 = stackVariable7.CreateTable("mix_messenger_team", stackVariable9, null, stackVariable11, null);
			stackVariable14 = migrationBuilder;
			stackVariable16 = init.u003cu003ec.u003cu003e9__0_4;
			if (stackVariable16 == null)
			{
				dummyVar6 = stackVariable16;
				stackVariable16 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType53<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_4);
				init.u003cu003ec.u003cu003e9__0_4 = stackVariable16;
			}
			stackVariable18 = init.u003cu003ec.u003cu003e9__0_5;
			if (stackVariable18 == null)
			{
				dummyVar7 = stackVariable18;
				stackVariable18 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType53<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_5);
				init.u003cu003ec.u003cu003e9__0_5 = stackVariable18;
			}
			dummyVar8 = stackVariable14.CreateTable("mix_messenger_user", stackVariable16, null, stackVariable18, null);
			stackVariable21 = migrationBuilder;
			stackVariable23 = init.u003cu003ec.u003cu003e9__0_6;
			if (stackVariable23 == null)
			{
				dummyVar9 = stackVariable23;
				stackVariable23 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType54<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_6);
				init.u003cu003ec.u003cu003e9__0_6 = stackVariable23;
			}
			stackVariable25 = init.u003cu003ec.u003cu003e9__0_7;
			if (stackVariable25 == null)
			{
				dummyVar10 = stackVariable25;
				stackVariable25 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType54<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_7);
				init.u003cu003ec.u003cu003e9__0_7 = stackVariable25;
			}
			dummyVar11 = stackVariable21.CreateTable("mix_messenger_user_device", stackVariable23, null, stackVariable25, null);
			stackVariable28 = migrationBuilder;
			stackVariable30 = init.u003cu003ec.u003cu003e9__0_8;
			if (stackVariable30 == null)
			{
				dummyVar12 = stackVariable30;
				stackVariable30 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType56<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_8);
				init.u003cu003ec.u003cu003e9__0_8 = stackVariable30;
			}
			stackVariable32 = init.u003cu003ec.u003cu003e9__0_9;
			if (stackVariable32 == null)
			{
				dummyVar13 = stackVariable32;
				stackVariable32 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType56<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_9);
				init.u003cu003ec.u003cu003e9__0_9 = stackVariable32;
			}
			dummyVar14 = stackVariable28.CreateTable("mix_messenger_message", stackVariable30, null, stackVariable32, null);
			stackVariable35 = migrationBuilder;
			stackVariable37 = init.u003cu003ec.u003cu003e9__0_10;
			if (stackVariable37 == null)
			{
				dummyVar15 = stackVariable37;
				stackVariable37 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_10);
				init.u003cu003ec.u003cu003e9__0_10 = stackVariable37;
			}
			stackVariable39 = init.u003cu003ec.u003cu003e9__0_11;
			if (stackVariable39 == null)
			{
				dummyVar16 = stackVariable39;
				stackVariable39 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_11);
				init.u003cu003ec.u003cu003e9__0_11 = stackVariable39;
			}
			dummyVar17 = stackVariable35.CreateTable("mix_messenger_nav_room_user", stackVariable37, null, stackVariable39, null);
			stackVariable42 = migrationBuilder;
			stackVariable44 = init.u003cu003ec.u003cu003e9__0_12;
			if (stackVariable44 == null)
			{
				dummyVar18 = stackVariable44;
				stackVariable44 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_12);
				init.u003cu003ec.u003cu003e9__0_12 = stackVariable44;
			}
			stackVariable46 = init.u003cu003ec.u003cu003e9__0_13;
			if (stackVariable46 == null)
			{
				dummyVar19 = stackVariable46;
				stackVariable46 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_13);
				init.u003cu003ec.u003cu003e9__0_13 = stackVariable46;
			}
			dummyVar20 = stackVariable42.CreateTable("mix_messenger_nav_team_user", stackVariable44, null, stackVariable46, null);
			dummyVar21 = migrationBuilder.CreateIndex("IX_messenger_message_RoomId", "mix_messenger_message", "RoomId", null, false, null);
			dummyVar22 = migrationBuilder.CreateIndex("IX_messenger_message_TeamId", "mix_messenger_message", "TeamId", null, false, null);
			dummyVar23 = migrationBuilder.CreateIndex("IX_messenger_message_UserId", "mix_messenger_message", "UserId", null, false, null);
			dummyVar24 = migrationBuilder.CreateIndex("IX_messenger_nav_room_user_UserId", "mix_messenger_nav_room_user", "UserId", null, false, null);
			dummyVar25 = migrationBuilder.CreateIndex("IX_messenger_nav_team_user_UserId", "mix_messenger_nav_team_user", "UserId", null, false, null);
			return;
		}
	}
}