using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Mix.Cms.Messenger.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Messenger.Migrations
{
	[DbContext(typeof(MixChatServiceContext))]
	[Migration("20181119151731_init")]
	public class init : Migration
	{
		public init()
		{
		}

		protected override void BuildTargetModel(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("ProductVersion", "2.1.4-rtm-31024");
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom", (EntityTypeBuilder b) => {
				b.Property<Guid>("Id");
				b.Property<string>("Avatar").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(b.Property<DateTime>("CreatedDate"), "datetime");
				b.Property<string>("Description");
				b.Property<string>("HostId").HasMaxLength(128);
				b.Property<bool>("IsOpen");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(b.Property<DateTime?>("LastModified"), "datetime");
				b.Property<string>("Name").IsRequired(true).HasMaxLength(50);
				b.Property<int?>("TeamId");
				b.Property<string>("Title").HasMaxLength(250);
				b.HasKey(new string[] { "Id" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "mix_messenger_hub_room");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerMessage", (EntityTypeBuilder b) => {
				b.Property<Guid>("Id");
				b.Property<string>("Content");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(b.Property<DateTime>("CreatedDate"), "datetime");
				b.Property<Guid?>("RoomId");
				b.Property<int?>("TeamId");
				b.Property<string>("UserId").HasMaxLength(50);
				b.HasKey(new string[] { "Id" });
				RelationalIndexBuilderExtensions.HasName(b.HasIndex(new string[] { "RoomId" }), "IX_messenger_message_RoomId");
				RelationalIndexBuilderExtensions.HasName(b.HasIndex(new string[] { "TeamId" }), "IX_messenger_message_TeamId");
				RelationalIndexBuilderExtensions.HasName(b.HasIndex(new string[] { "UserId" }), "IX_messenger_message_UserId");
				RelationalEntityTypeBuilderExtensions.ToTable(b, "mix_messenger_message");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser", (EntityTypeBuilder b) => {
				b.Property<Guid>("RoomId");
				b.Property<string>("UserId").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(b.Property<DateTime>("JoinedDate"), "datetime");
				b.HasKey(new string[] { "RoomId", "UserId" });
				RelationalIndexBuilderExtensions.HasName(b.HasIndex(new string[] { "UserId" }), "IX_messenger_nav_room_user_UserId");
				RelationalEntityTypeBuilderExtensions.ToTable(b, "mix_messenger_nav_room_user");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser", (EntityTypeBuilder b) => {
				b.Property<int>("TeamId");
				b.Property<string>("UserId").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(b.Property<DateTime>("JoinedDate"), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(b.Property<DateTime?>("LastModified"), "datetime");
				b.Property<int>("Status");
				b.HasKey(new string[] { "TeamId", "UserId" });
				RelationalIndexBuilderExtensions.HasName(b.HasIndex(new string[] { "UserId" }), "IX_messenger_nav_team_user_UserId");
				RelationalEntityTypeBuilderExtensions.ToTable(b, "mix_messenger_nav_team_user");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerTeam", (EntityTypeBuilder b) => {
				b.Property<int>("Id");
				b.Property<string>("Avatar").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(b.Property<DateTime>("CreatedDate"), "datetime");
				b.Property<string>("HostId").HasMaxLength(128);
				b.Property<bool?>("IsOpen").ValueGeneratedOnAdd();
				b.Property<string>("Name").IsRequired(true).HasMaxLength(250);
				b.Property<int>("Type");
				b.HasKey(new string[] { "Id" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "mix_messenger_team");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerUser", (EntityTypeBuilder b) => {
				b.Property<string>("Id").HasMaxLength(50);
				b.Property<string>("Avatar").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(b.Property<DateTime>("CreatedDate"), "datetime");
				b.Property<string>("FacebookId").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(b.Property<DateTime?>("LastModified"), "datetime");
				b.Property<string>("Name").IsRequired(true).HasMaxLength(250);
				b.Property<int>("Status").ValueGeneratedOnAdd();
				b.HasKey(new string[] { "Id" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "mix_messenger_user");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerUserDevice", (EntityTypeBuilder b) => {
				b.Property<string>("UserId").HasMaxLength(50);
				b.Property<string>("DeviceId").HasMaxLength(50);
				b.Property<string>("ConnectionId").IsRequired(true).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(b.Property<DateTime?>("EndDate"), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(b.Property<DateTime>("StartDate"), "datetime");
				b.Property<int>("Status");
				b.HasKey(new string[] { "UserId", "DeviceId" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "mix_messenger_user_device");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerMessage", (EntityTypeBuilder b) => {
				RelationalForeignKeyBuilderExtensions.HasConstraintName(b.HasOne("Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom", "Room").WithMany("MixMessengerMessage").HasForeignKey(new string[] { "RoomId" }), "FK_messenger_message_messenger_hub_room");
				RelationalForeignKeyBuilderExtensions.HasConstraintName(b.HasOne("Mix.Cms.Messenger.Models.Data.MixMessengerTeam", "Team").WithMany("MixMessengerMessage").HasForeignKey(new string[] { "TeamId" }), "FK_messenger_message_messenger_team");
				RelationalForeignKeyBuilderExtensions.HasConstraintName(b.HasOne("Mix.Cms.Messenger.Models.Data.MixMessengerUser", "User").WithMany("MixMessengerMessage").HasForeignKey(new string[] { "UserId" }), "FK_messenger_message_messenger_user");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavRoomUser", (EntityTypeBuilder b) => {
				RelationalForeignKeyBuilderExtensions.HasConstraintName(b.HasOne("Mix.Cms.Messenger.Models.Data.MixMessengerHubRoom", "Room").WithMany("MixMessengerNavRoomUser").HasForeignKey(new string[] { "RoomId" }), "FK_messenger_nav_room_user_messenger_hub_room");
				RelationalForeignKeyBuilderExtensions.HasConstraintName(b.HasOne("Mix.Cms.Messenger.Models.Data.MixMessengerUser", "User").WithMany("MixMessengerNavRoomUser").HasForeignKey(new string[] { "UserId" }), "FK_messenger_nav_room_user_messenger_user");
			});
			modelBuilder.Entity("Mix.Cms.Messenger.Models.Data.MixMessengerNavTeamUser", (EntityTypeBuilder b) => {
				RelationalForeignKeyBuilderExtensions.HasConstraintName(b.HasOne("Mix.Cms.Messenger.Models.Data.MixMessengerTeam", "Team").WithMany("MixMessengerNavTeamUser").HasForeignKey(new string[] { "TeamId" }), "FK_messenger_nav_team_user_messenger_team");
				RelationalForeignKeyBuilderExtensions.HasConstraintName(b.HasOne("Mix.Cms.Messenger.Models.Data.MixMessengerUser", "User").WithMany("MixMessengerNavTeamUser").HasForeignKey(new string[] { "UserId" }), "FK_messenger_nav_team_user_messenger_user");
			});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable("mix_messenger_message", null);
			migrationBuilder.DropTable("mix_messenger_nav_room_user", null);
			migrationBuilder.DropTable("mix_messenger_nav_team_user", null);
			migrationBuilder.DropTable("mix_messenger_user_device", null);
			migrationBuilder.DropTable("mix_messenger_hub_room", null);
			migrationBuilder.DropTable("mix_messenger_team", null);
			migrationBuilder.DropTable("mix_messenger_user", null);
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable("mix_messenger_hub_room", (ColumnsBuilder table) => {
				bool? nullable = null;
				bool? nullable1 = nullable;
				int? nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder = table.Column<Guid>(null, nullable1, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable3 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder1 = table.Column<string>(null, nullable3, new int?(250), false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable4 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder2 = table.Column<DateTime>("datetime", nullable4, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable5 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder3 = table.Column<string>(null, nullable5, nullable2, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable6 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder4 = table.Column<string>(null, nullable6, new int?(128), false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable7 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder5 = table.Column<bool>(null, nullable7, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable8 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder6 = table.Column<DateTime>("datetime", nullable8, nullable2, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable9 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder7 = table.Column<string>(null, nullable9, new int?(50), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable10 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder8 = table.Column<int>(null, nullable10, nullable2, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable11 = nullable;
				nullable = null;
				return new { Id = operationBuilder, Avatar = operationBuilder1, CreatedDate = operationBuilder2, Description = operationBuilder3, HostId = operationBuilder4, IsOpen = operationBuilder5, LastModified = operationBuilder6, Name = operationBuilder7, TeamId = operationBuilder8, Title = table.Column<string>(null, nullable11, new int?(250), false, null, true, null, null, null, nullable, null) };
			}, null, (table) => table.PrimaryKey("PK_mix_messenger_hub_room", (x) => x.Id), null);
			migrationBuilder.CreateTable("mix_messenger_team", (ColumnsBuilder table) => {
				bool? nullable = null;
				bool? nullable1 = nullable;
				int? nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder = table.Column<int>(null, nullable1, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable3 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder1 = table.Column<string>(null, nullable3, new int?(250), false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable4 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder2 = table.Column<DateTime>("datetime", nullable4, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable5 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder3 = table.Column<string>(null, nullable5, new int?(128), false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable6 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder4 = table.Column<bool>(null, nullable6, nullable2, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable7 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder5 = table.Column<string>(null, nullable7, new int?(250), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable8 = nullable;
				nullable2 = null;
				nullable = null;
				return new { Id = operationBuilder, Avatar = operationBuilder1, CreatedDate = operationBuilder2, HostId = operationBuilder3, IsOpen = operationBuilder4, Name = operationBuilder5, Type = table.Column<int>(null, nullable8, nullable2, false, null, false, null, null, null, nullable, null) };
			}, null, (table) => table.PrimaryKey("PK_mix_messenger_team", (x) => x.Id), null);
			migrationBuilder.CreateTable("mix_messenger_user", (ColumnsBuilder table) => {
				bool? nullable = null;
				bool? nullable1 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder = table.Column<string>(null, nullable1, new int?(50), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable2 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder1 = table.Column<string>(null, nullable2, new int?(50), false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable3 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder2 = table.Column<string>(null, nullable3, new int?(250), false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable4 = nullable;
				int? nullable5 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder3 = table.Column<DateTime>("datetime", nullable4, nullable5, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable6 = nullable;
				nullable5 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder4 = table.Column<DateTime>("datetime", nullable6, nullable5, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable7 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder5 = table.Column<string>(null, nullable7, new int?(250), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable8 = nullable;
				nullable5 = null;
				nullable = null;
				return new { Id = operationBuilder, FacebookId = operationBuilder1, Avatar = operationBuilder2, CreatedDate = operationBuilder3, LastModified = operationBuilder4, Name = operationBuilder5, Status = table.Column<int>(null, nullable8, nullable5, false, null, false, null, null, null, nullable, null).Annotation("Sqlite:Autoincrement", true) };
			}, null, (table) => table.PrimaryKey("PK_mix_messenger_user", (x) => x.Id), null);
			migrationBuilder.CreateTable("mix_messenger_user_device", (ColumnsBuilder table) => {
				bool? nullable = null;
				bool? nullable1 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder = table.Column<string>(null, nullable1, new int?(50), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable2 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder1 = table.Column<string>(null, nullable2, new int?(50), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable3 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder2 = table.Column<string>(null, nullable3, new int?(50), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable4 = nullable;
				int? nullable5 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder3 = table.Column<int>(null, nullable4, nullable5, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable6 = nullable;
				nullable5 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder4 = table.Column<DateTime>("datetime", nullable6, nullable5, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable7 = nullable;
				nullable5 = null;
				nullable = null;
				return new { UserId = operationBuilder, ConnectionId = operationBuilder1, DeviceId = operationBuilder2, Status = operationBuilder3, StartDate = operationBuilder4, EndDate = table.Column<DateTime>("datetime", nullable7, nullable5, false, null, true, null, null, null, nullable, null) };
			}, null, (table) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(u003cu003ef__AnonymousType54<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>), "x");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType55<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod(".ctor", new Type[] { typeof(u003cUserIdu003ej__TPar), typeof(u003cDeviceIdu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType55<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType54<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType54<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType54<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_DeviceId").MethodHandle, typeof(u003cu003ef__AnonymousType54<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType55<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType55<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType55<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_DeviceId").MethodHandle, typeof(u003cu003ef__AnonymousType55<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle) };
				table.PrimaryKey("PK_mix_messenger_user_device", Expression.Lambda(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
			}, null);
			migrationBuilder.CreateTable("mix_messenger_message", (ColumnsBuilder table) => {
				bool? nullable = null;
				bool? nullable1 = nullable;
				int? nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder = table.Column<Guid>(null, nullable1, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable3 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder1 = table.Column<string>(null, nullable3, nullable2, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable4 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder2 = table.Column<DateTime>("datetime", nullable4, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable5 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder3 = table.Column<Guid>(null, nullable5, nullable2, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable6 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder4 = table.Column<int>(null, nullable6, nullable2, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable7 = nullable;
				nullable = null;
				return new { Id = operationBuilder, Content = operationBuilder1, CreatedDate = operationBuilder2, RoomId = operationBuilder3, TeamId = operationBuilder4, UserId = table.Column<string>(null, nullable7, new int?(50), false, null, true, null, null, null, nullable, null) };
			}, null, (table) => {
				table.PrimaryKey("PK_mix_messenger_message", (x) => x.Id);
				table.ForeignKey("FK_messenger_message_messenger_hub_room", (x) => x.RoomId, "mix_messenger_hub_room", "Id", null, 0, 1);
				table.ForeignKey("FK_messenger_message_messenger_team", (x) => x.TeamId, "mix_messenger_team", "Id", null, 0, 1);
				table.ForeignKey("FK_messenger_message_messenger_user", (x) => x.UserId, "mix_messenger_user", "Id", null, 0, 1);
			}, null);
			migrationBuilder.CreateTable("mix_messenger_nav_room_user", (ColumnsBuilder table) => {
				bool? nullable = null;
				bool? nullable1 = nullable;
				int? nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder = table.Column<Guid>(null, nullable1, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable3 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder1 = table.Column<string>(null, nullable3, new int?(50), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable4 = nullable;
				nullable2 = null;
				nullable = null;
				return new { RoomId = operationBuilder, UserId = operationBuilder1, JoinedDate = table.Column<DateTime>("datetime", nullable4, nullable2, false, null, false, null, null, null, nullable, null) };
			}, null, (table) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>), "x");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType58<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod(".ctor", new Type[] { typeof(u003cRoomIdu003ej__TPar), typeof(u003cUserIdu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType58<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_RoomId").MethodHandle, typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType58<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_RoomId").MethodHandle, typeof(u003cu003ef__AnonymousType58<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType58<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType58<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle) };
				table.PrimaryKey("PK_mix_messenger_nav_room_user", Expression.Lambda(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>), "x");
				table.ForeignKey("FK_messenger_nav_room_user_messenger_hub_room", Expression.Lambda(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_RoomId").MethodHandle, typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)), new ParameterExpression[] { parameterExpression }), "mix_messenger_hub_room", "Id", null, 0, 1);
				parameterExpression = Expression.Parameter(typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>), "x");
				table.ForeignKey("FK_messenger_nav_room_user_messenger_user", Expression.Lambda(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType57<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)), new ParameterExpression[] { parameterExpression }), "mix_messenger_user", "Id", null, 0, 1);
			}, null);
			migrationBuilder.CreateTable("mix_messenger_nav_team_user", (ColumnsBuilder table) => {
				bool? nullable = null;
				bool? nullable1 = nullable;
				int? nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder = table.Column<int>(null, nullable1, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable3 = nullable;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder1 = table.Column<string>(null, nullable3, new int?(50), false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable4 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder2 = table.Column<DateTime>("datetime", nullable4, nullable2, false, null, false, null, null, null, nullable, null);
				nullable = null;
				bool? nullable5 = nullable;
				nullable2 = null;
				nullable = null;
				OperationBuilder<AddColumnOperation> operationBuilder3 = table.Column<DateTime>("datetime", nullable5, nullable2, false, null, true, null, null, null, nullable, null);
				nullable = null;
				bool? nullable6 = nullable;
				nullable2 = null;
				nullable = null;
				return new { TeamId = operationBuilder, UserId = operationBuilder1, JoinedDate = operationBuilder2, LastModified = operationBuilder3, Status = table.Column<int>(null, nullable6, nullable2, false, null, false, null, null, null, nullable, null) };
			}, null, (table) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>), "x");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType60<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod(".ctor", new Type[] { typeof(u003cTeamIdu003ej__TPar), typeof(u003cUserIdu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType60<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_TeamId").MethodHandle, typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType60<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_TeamId").MethodHandle, typeof(u003cu003ef__AnonymousType60<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType60<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType60<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle) };
				table.PrimaryKey("PK_mix_messenger_nav_team_user", Expression.Lambda(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>), "x");
				table.ForeignKey("FK_messenger_nav_team_user_messenger_team", Expression.Lambda(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_TeamId").MethodHandle, typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)), new ParameterExpression[] { parameterExpression }), "mix_messenger_team", "Id", null, 0, 1);
				parameterExpression = Expression.Parameter(typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>), "x");
				table.ForeignKey("FK_messenger_nav_team_user_messenger_user", Expression.Lambda(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType59<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>).TypeHandle)), new ParameterExpression[] { parameterExpression }), "mix_messenger_user", "Id", null, 0, 1);
			}, null);
			migrationBuilder.CreateIndex("IX_messenger_message_RoomId", "mix_messenger_message", "RoomId", null, false, null);
			migrationBuilder.CreateIndex("IX_messenger_message_TeamId", "mix_messenger_message", "TeamId", null, false, null);
			migrationBuilder.CreateIndex("IX_messenger_message_UserId", "mix_messenger_message", "UserId", null, false, null);
			migrationBuilder.CreateIndex("IX_messenger_nav_room_user_UserId", "mix_messenger_nav_room_user", "UserId", null, false, null);
			migrationBuilder.CreateIndex("IX_messenger_nav_team_user_UserId", "mix_messenger_nav_team_user", "UserId", null, false, null);
		}
	}
}