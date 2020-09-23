using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Messenger.Models.Data;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Migrations
{
	[DbContext(typeof(MixChatServiceContext))]
	internal class MixChatServiceContextModelSnapshot : ModelSnapshot
	{
		public MixChatServiceContextModelSnapshot()
		{
			base();
			return;
		}

		protected override void BuildModel(ModelBuilder modelBuilder)
		{
			dummyVar0 = modelBuilder.HasAnnotation("ProductVersion", "2.1.4-rtm-31024");
			stackVariable4 = modelBuilder;
			stackVariable6 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable6 == null)
			{
				dummyVar1 = stackVariable6;
				stackVariable6 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_0);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_0 = stackVariable6;
			}
			dummyVar2 = stackVariable4.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom", stackVariable6);
			stackVariable8 = modelBuilder;
			stackVariable10 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_1;
			if (stackVariable10 == null)
			{
				dummyVar3 = stackVariable10;
				stackVariable10 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_1);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_1 = stackVariable10;
			}
			dummyVar4 = stackVariable8.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerMessage", stackVariable10);
			stackVariable12 = modelBuilder;
			stackVariable14 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_2;
			if (stackVariable14 == null)
			{
				dummyVar5 = stackVariable14;
				stackVariable14 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_2);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_2 = stackVariable14;
			}
			dummyVar6 = stackVariable12.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser", stackVariable14);
			stackVariable16 = modelBuilder;
			stackVariable18 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_3;
			if (stackVariable18 == null)
			{
				dummyVar7 = stackVariable18;
				stackVariable18 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_3);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_3 = stackVariable18;
			}
			dummyVar8 = stackVariable16.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser", stackVariable18);
			stackVariable20 = modelBuilder;
			stackVariable22 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_4;
			if (stackVariable22 == null)
			{
				dummyVar9 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_4);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_4 = stackVariable22;
			}
			dummyVar10 = stackVariable20.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerTeam", stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable26 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_5;
			if (stackVariable26 == null)
			{
				dummyVar11 = stackVariable26;
				stackVariable26 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_5);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_5 = stackVariable26;
			}
			dummyVar12 = stackVariable24.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerUser", stackVariable26);
			stackVariable28 = modelBuilder;
			stackVariable30 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_6;
			if (stackVariable30 == null)
			{
				dummyVar13 = stackVariable30;
				stackVariable30 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_6);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_6 = stackVariable30;
			}
			dummyVar14 = stackVariable28.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice", stackVariable30);
			stackVariable32 = modelBuilder;
			stackVariable34 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_7;
			if (stackVariable34 == null)
			{
				dummyVar15 = stackVariable34;
				stackVariable34 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_7);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_7 = stackVariable34;
			}
			dummyVar16 = stackVariable32.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerMessage", stackVariable34);
			stackVariable36 = modelBuilder;
			stackVariable38 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_8;
			if (stackVariable38 == null)
			{
				dummyVar17 = stackVariable38;
				stackVariable38 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_8);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_8 = stackVariable38;
			}
			dummyVar18 = stackVariable36.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser", stackVariable38);
			stackVariable40 = modelBuilder;
			stackVariable42 = MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_9;
			if (stackVariable42 == null)
			{
				dummyVar19 = stackVariable42;
				stackVariable42 = new Action<EntityTypeBuilder>(MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9.u003cBuildModelu003eb__0_9);
				MixChatServiceContextModelSnapshot.u003cu003ec.u003cu003e9__0_9 = stackVariable42;
			}
			dummyVar20 = stackVariable40.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser", stackVariable42);
			return;
		}
	}
}