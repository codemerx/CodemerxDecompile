using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Lib;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Models.Data
{
	public class MixChatServiceContext : DbContext
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

		public MixChatServiceContext()
		{
			base();
			return;
		}

		public MixChatServiceContext(DbContextOptions<sw_chatContext> options)
		{
			base(options);
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
			if (!optionsBuilder.get_IsConfigured())
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
			}
			return;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			stackVariable0 = modelBuilder;
			stackVariable1 = MixChatServiceContext.u003cu003ec.u003cu003e9__32_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>>(MixChatServiceContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__32_0);
				MixChatServiceContext.u003cu003ec.u003cu003e9__32_0 = stackVariable1;
			}
			dummyVar1 = stackVariable0.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>(stackVariable1);
			stackVariable3 = modelBuilder;
			stackVariable4 = MixChatServiceContext.u003cu003ec.u003cu003e9__32_1;
			if (stackVariable4 == null)
			{
				dummyVar2 = stackVariable4;
				stackVariable4 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>>(MixChatServiceContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__32_1);
				MixChatServiceContext.u003cu003ec.u003cu003e9__32_1 = stackVariable4;
			}
			dummyVar3 = stackVariable3.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(stackVariable4);
			stackVariable6 = modelBuilder;
			stackVariable7 = MixChatServiceContext.u003cu003ec.u003cu003e9__32_2;
			if (stackVariable7 == null)
			{
				dummyVar4 = stackVariable7;
				stackVariable7 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>>(MixChatServiceContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__32_2);
				MixChatServiceContext.u003cu003ec.u003cu003e9__32_2 = stackVariable7;
			}
			dummyVar5 = stackVariable6.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>(stackVariable7);
			stackVariable9 = modelBuilder;
			stackVariable10 = MixChatServiceContext.u003cu003ec.u003cu003e9__32_3;
			if (stackVariable10 == null)
			{
				dummyVar6 = stackVariable10;
				stackVariable10 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>>(MixChatServiceContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__32_3);
				MixChatServiceContext.u003cu003ec.u003cu003e9__32_3 = stackVariable10;
			}
			dummyVar7 = stackVariable9.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>(stackVariable10);
			stackVariable12 = modelBuilder;
			stackVariable13 = MixChatServiceContext.u003cu003ec.u003cu003e9__32_4;
			if (stackVariable13 == null)
			{
				dummyVar8 = stackVariable13;
				stackVariable13 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerTeam>>(MixChatServiceContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__32_4);
				MixChatServiceContext.u003cu003ec.u003cu003e9__32_4 = stackVariable13;
			}
			dummyVar9 = stackVariable12.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerTeam>(stackVariable13);
			stackVariable15 = modelBuilder;
			stackVariable16 = MixChatServiceContext.u003cu003ec.u003cu003e9__32_5;
			if (stackVariable16 == null)
			{
				dummyVar10 = stackVariable16;
				stackVariable16 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerUser>>(MixChatServiceContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__32_5);
				MixChatServiceContext.u003cu003ec.u003cu003e9__32_5 = stackVariable16;
			}
			dummyVar11 = stackVariable15.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerUser>(stackVariable16);
			stackVariable18 = modelBuilder;
			stackVariable19 = MixChatServiceContext.u003cu003ec.u003cu003e9__32_6;
			if (stackVariable19 == null)
			{
				dummyVar12 = stackVariable19;
				stackVariable19 = new Action<EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice>>(MixChatServiceContext.u003cu003ec.u003cu003e9.u003cOnModelCreatingu003eb__32_6);
				MixChatServiceContext.u003cu003ec.u003cu003e9__32_6 = stackVariable19;
			}
			dummyVar13 = stackVariable18.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice>(stackVariable19);
			return;
		}
	}
}