using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Lib.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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
		}

		public sw_chatContext(DbContextOptions<sw_chatContext> options) : base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.get_IsConfigured())
			{
				string connectionString = MixService.GetConnectionString("MixCmsConnection");
				if (!string.IsNullOrEmpty(connectionString))
				{
					if (MixService.GetConfig<bool>("IsMysql"))
					{
						MySqlDbContextOptionsExtensions.UseMySql(optionsBuilder, connectionString, null);
						return;
					}
					SqlServerDbContextOptionsExtensions.UseSqlServer(optionsBuilder, connectionString, null);
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>((EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>(entity, "mix_messenger_hub_room");
				entity.Property<Guid>((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom e) => e.Avatar).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom e) => e.CreatedDate), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom e) => e.Description), "ntext");
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom e) => e.HostId).HasMaxLength(128);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom e) => e.Name).IsRequired(true).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom e) => e.Title).HasMaxLength(250);
			});
			modelBuilder.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>((EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerMessage> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(entity, "mix_messenger_message");
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(entity.HasIndex((Mix.Cms.Messenger.Models.Data.MixMessengerMessage e) => (object)e.RoomId), "IX_messenger_message_RoomId");
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(entity.HasIndex((Mix.Cms.Messenger.Models.Data.MixMessengerMessage e) => (object)e.TeamId), "IX_messenger_message_TeamId");
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(entity.HasIndex((Mix.Cms.Messenger.Models.Data.MixMessengerMessage e) => e.UserId), "IX_messenger_message_UserId");
				entity.Property<Guid>((Mix.Cms.Messenger.Models.Data.MixMessengerMessage e) => e.Id).ValueGeneratedNever();
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerMessage e) => e.Content), "ntext");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Messenger.Models.Data.MixMessengerMessage e) => e.CreatedDate), "datetime");
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerMessage e) => e.UserId).HasMaxLength(50);
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom, Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(entity.HasOne<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>((Mix.Cms.Messenger.Models.Data.MixMessengerMessage d) => d.Room).WithMany((Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom p) => p.MixMessengerMessage).HasForeignKey((Mix.Cms.Messenger.Models.Data.MixMessengerMessage d) => (object)d.RoomId), "FK_messenger_message_messenger_hub_room");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Messenger.Models.Data.MixMessengerTeam, Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(entity.HasOne<Mix.Cms.Messenger.Models.Data.MixMessengerTeam>((Mix.Cms.Messenger.Models.Data.MixMessengerMessage d) => d.Team).WithMany((Mix.Cms.Messenger.Models.Data.MixMessengerTeam p) => p.MixMessengerMessage).HasForeignKey((Mix.Cms.Messenger.Models.Data.MixMessengerMessage d) => (object)d.TeamId), "FK_messenger_message_messenger_team");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Messenger.Models.Data.MixMessengerUser, Mix.Cms.Messenger.Models.Data.MixMessengerMessage>(entity.HasOne<Mix.Cms.Messenger.Models.Data.MixMessengerUser>((Mix.Cms.Messenger.Models.Data.MixMessengerMessage d) => d.User).WithMany((Mix.Cms.Messenger.Models.Data.MixMessengerUser p) => p.MixMessengerMessage).HasForeignKey((Mix.Cms.Messenger.Models.Data.MixMessengerMessage d) => d.UserId), "FK_messenger_message_messenger_user");
			});
			modelBuilder.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>((EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType58<Guid, string>).GetMethod(".ctor", new Type[] { typeof(u003cRoomIdu003ej__TPar), typeof(u003cUserIdu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType58<Guid, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_RoomId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_UserId").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType58<Guid, string>).GetMethod("get_RoomId").MethodHandle, typeof(u003cu003ef__AnonymousType58<Guid, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType58<Guid, string>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType58<Guid, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>(entity, "mix_messenger_nav_room_user");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser), "e");
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>(entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })), "IX_messenger_nav_room_user_UserId");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_JoinedDate").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser), "d");
				ReferenceNavigationBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_Room").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom), "p");
				ReferenceCollectionBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom, Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom, IEnumerable<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom).GetMethod("get_MixMessengerNavRoomUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom, Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, object>>(Expression.Convert(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_RoomId").MethodHandle)), typeof(object)), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_messenger_nav_room_user_messenger_hub_room");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser), "d");
				ReferenceNavigationBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, Mix.Cms.Messenger.Models.Data.MixMessengerUser> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Messenger.Models.Data.MixMessengerUser>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, Mix.Cms.Messenger.Models.Data.MixMessengerUser>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_User").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUser), "p");
				ReferenceCollectionBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerUser, Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerUser, IEnumerable<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUser).GetMethod("get_MixMessengerNavRoomUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Messenger.Models.Data.MixMessengerUser, Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_messenger_nav_room_user_messenger_user");
			});
			modelBuilder.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>((EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType60<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cTeamIdu003ej__TPar), typeof(u003cUserIdu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType60<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_TeamId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_UserId").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType60<int, string>).GetMethod("get_TeamId").MethodHandle, typeof(u003cu003ef__AnonymousType60<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType60<int, string>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType60<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>(entity, "mix_messenger_nav_team_user");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "e");
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>(entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })), "IX_messenger_nav_team_user_UserId");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_JoinedDate").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "d");
				ReferenceNavigationBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, Mix.Cms.Messenger.Models.Data.MixMessengerTeam> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Messenger.Models.Data.MixMessengerTeam>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, Mix.Cms.Messenger.Models.Data.MixMessengerTeam>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_Team").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerTeam), "p");
				ReferenceCollectionBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerTeam, Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerTeam, IEnumerable<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerTeam).GetMethod("get_MixMessengerNavTeamUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Messenger.Models.Data.MixMessengerTeam, Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, object>>(Expression.Convert(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_TeamId").MethodHandle)), typeof(object)), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_messenger_nav_team_user_messenger_team");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "d");
				ReferenceNavigationBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, Mix.Cms.Messenger.Models.Data.MixMessengerUser> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Messenger.Models.Data.MixMessengerUser>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, Mix.Cms.Messenger.Models.Data.MixMessengerUser>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_User").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUser), "p");
				ReferenceCollectionBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerUser, Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerUser, IEnumerable<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUser).GetMethod("get_MixMessengerNavTeamUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Messenger.Models.Data.MixMessengerUser, Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_messenger_nav_team_user_messenger_user");
			});
			modelBuilder.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerTeam>((EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerTeam> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Messenger.Models.Data.MixMessengerTeam>(entity, "mix_messenger_team");
				entity.Property<int>((Mix.Cms.Messenger.Models.Data.MixMessengerTeam e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerTeam e) => e.Avatar).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Messenger.Models.Data.MixMessengerTeam e) => e.CreatedDate), "datetime");
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerTeam e) => e.HostId).HasMaxLength(128);
				RelationalPropertyBuilderExtensions.HasDefaultValueSql<bool?>(entity.Property<bool?>((Mix.Cms.Messenger.Models.Data.MixMessengerTeam e) => e.IsOpen), "((1))");
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerTeam e) => e.Name).IsRequired(true).HasMaxLength(250);
			});
			modelBuilder.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerUser>((EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerUser> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Messenger.Models.Data.MixMessengerUser>(entity, "mix_messenger_user");
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerUser e) => e.Id).HasMaxLength(50).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerUser e) => e.Avatar).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Messenger.Models.Data.MixMessengerUser e) => e.CreatedDate), "datetime");
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerUser e) => e.FacebookId).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Messenger.Models.Data.MixMessengerUser e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerUser e) => e.Name).IsRequired(true).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasDefaultValueSql<string>(entity.Property<string>((Mix.Cms.Messenger.Models.Data.MixMessengerUser e) => e.Status), "((1))");
			});
			modelBuilder.Entity<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice>((EntityTypeBuilder<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType55<string, string>).GetMethod(".ctor", new Type[] { typeof(u003cUserIdu003ej__TPar), typeof(u003cDeviceIdu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType55<string, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice).GetMethod("get_UserId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice).GetMethod("get_DeviceId").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType55<string, string>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType55<string, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType55<string, string>).GetMethod("get_DeviceId").MethodHandle, typeof(u003cu003ef__AnonymousType55<string, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice>(entity, "mix_messenger_user_device");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice).GetMethod("get_DeviceId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice).GetMethod("get_ConnectionId").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice).GetMethod("get_EndDate").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice).GetMethod("get_StartDate").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
			});
		}
	}
}