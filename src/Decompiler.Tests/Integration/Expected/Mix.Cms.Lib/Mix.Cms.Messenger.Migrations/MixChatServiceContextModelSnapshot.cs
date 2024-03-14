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
		}

		protected override void BuildModel(ModelBuilder modelBuilder)
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
	}
}