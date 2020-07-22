using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class sw_chatContext : DbContext
	{
		public virtual DbSet<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom> MixMessengerHubRoom
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Messenger.Models.Data.MixMessengerMessage> MixMessengerMessage
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser> MixMessengerNavRoomUser
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser> MixMessengerNavTeamUser
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Messenger.Models.Data.MixMessengerTeam> MixMessengerTeam
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Messenger.Models.Data.MixMessengerUser> MixMessengerUser
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice> MixMessengerUserDevice
		{
			get;
			set;
		}

		public sw_chatContext()
		{
			base();
			return;
		}

		public sw_chatContext(DbContextOptions<sw_chatContext> options)
		{
			base(options);
			return;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.get_IsConfigured())
			{
				V_0 = MixService.GetConnectionString("MixCmsConnection");
				if (!string.IsNullOrEmpty(V_0))
				{
					if (MixService.GetConfig<bool>("IsMysql"))
					{
						dummyVar0 = MySqlDbContextOptionsExtensions.UseMySql(optionsBuilder, V_0, null);
						return;
					}
					dummyVar1 = SqlServerDbContextOptionsExtensions.UseSqlServer(optionsBuilder, V_0, null);
				}
			}
			return;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			stackVariable0 = modelBuilder;
			stackVariable1 = sw_chatContext.u003cu003ec.u003cu003e9__31_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>>(sw_chatContext.u003cu003ec.u003cu003e9, sw_chatContext.u003cu003ec.u003cOnModelCreatingu003eb__31_0);
				sw_chatContext.u003cu003ec.u003cu003e9__31_0 = stackVariable1;
			}
			dummyVar1 = stackVariable0.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>(stackVariable1);
			stackVariable3 = modelBuilder;
			stackVariable4 = sw_chatContext.u003cu003ec.u003cu003e9__31_1;
			if (stackVariable4 == null)
			{
				dummyVar2 = stackVariable4;
				stackVariable4 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>>(sw_chatContext.u003cu003ec.u003cu003e9, sw_chatContext.u003cu003ec.u003cOnModelCreatingu003eb__31_1);
				sw_chatContext.u003cu003ec.u003cu003e9__31_1 = stackVariable4;
			}
			dummyVar3 = stackVariable3.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(stackVariable4);
			stackVariable6 = modelBuilder;
			stackVariable7 = sw_chatContext.u003cu003ec.u003cu003e9__31_2;
			if (stackVariable7 == null)
			{
				dummyVar4 = stackVariable7;
				stackVariable7 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>>(sw_chatContext.u003cu003ec.u003cu003e9, sw_chatContext.u003cu003ec.u003cOnModelCreatingu003eb__31_2);
				sw_chatContext.u003cu003ec.u003cu003e9__31_2 = stackVariable7;
			}
			dummyVar5 = stackVariable6.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>(stackVariable7);
			stackVariable9 = modelBuilder;
			stackVariable10 = sw_chatContext.u003cu003ec.u003cu003e9__31_3;
			if (stackVariable10 == null)
			{
				dummyVar6 = stackVariable10;
				stackVariable10 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>>(sw_chatContext.u003cu003ec.u003cu003e9, sw_chatContext.u003cu003ec.u003cOnModelCreatingu003eb__31_3);
				sw_chatContext.u003cu003ec.u003cu003e9__31_3 = stackVariable10;
			}
			dummyVar7 = stackVariable9.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>(stackVariable10);
			stackVariable12 = modelBuilder;
			stackVariable13 = sw_chatContext.u003cu003ec.u003cu003e9__31_4;
			if (stackVariable13 == null)
			{
				dummyVar8 = stackVariable13;
				stackVariable13 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerTeam>>(sw_chatContext.u003cu003ec.u003cu003e9, sw_chatContext.u003cu003ec.u003cOnModelCreatingu003eb__31_4);
				sw_chatContext.u003cu003ec.u003cu003e9__31_4 = stackVariable13;
			}
			dummyVar9 = stackVariable12.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerTeam>(stackVariable13);
			stackVariable15 = modelBuilder;
			stackVariable16 = sw_chatContext.u003cu003ec.u003cu003e9__31_5;
			if (stackVariable16 == null)
			{
				dummyVar10 = stackVariable16;
				stackVariable16 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerUser>>(sw_chatContext.u003cu003ec.u003cu003e9, sw_chatContext.u003cu003ec.u003cOnModelCreatingu003eb__31_5);
				sw_chatContext.u003cu003ec.u003cu003e9__31_5 = stackVariable16;
			}
			dummyVar11 = stackVariable15.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerUser>(stackVariable16);
			stackVariable18 = modelBuilder;
			stackVariable19 = sw_chatContext.u003cu003ec.u003cu003e9__31_6;
			if (stackVariable19 == null)
			{
				dummyVar12 = stackVariable19;
				stackVariable19 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice>>(sw_chatContext.u003cu003ec.u003cu003e9, sw_chatContext.u003cu003ec.u003cOnModelCreatingu003eb__31_6);
				sw_chatContext.u003cu003ec.u003cu003e9__31_6 = stackVariable19;
			}
			dummyVar13 = stackVariable18.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice>(stackVariable19);
			return;
		}
	}
}