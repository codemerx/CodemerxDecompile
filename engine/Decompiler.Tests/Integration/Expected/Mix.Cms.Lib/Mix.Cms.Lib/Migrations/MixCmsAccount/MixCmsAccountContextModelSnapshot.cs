using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
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
		}

		protected override void BuildModel(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("ProductVersion", "3.1.2").HasAnnotation("Relational:MaxIdentifierLength", 128).HasAnnotation("SqlServer:ValueGenerationStrategy", (SqlServerValueGenerationStrategy)2);
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetRoleClaims", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<int>(b.Property<int>("Id"), "int");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ClaimType"), "nvarchar(400)").HasMaxLength(0x190);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ClaimValue"), "nvarchar(400)").HasMaxLength(0x190);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("RoleId").IsRequired(true), "nvarchar(50)").HasMaxLength(50);
				b.HasKey(new string[] { "Id" });
				b.HasIndex(new string[] { "RoleId" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "AspNetRoleClaims");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetRoles", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Id"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ConcurrencyStamp"), "nvarchar(400)").HasMaxLength(0x190);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Name"), "nvarchar(250)").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("NormalizedName"), "nvarchar(250)").HasMaxLength(250);
				b.HasKey(new string[] { "Id" });
				RelationalIndexBuilderExtensions.HasFilter(RelationalIndexBuilderExtensions.HasName(b.HasIndex(new string[] { "NormalizedName" }).IsUnique(true), "RoleNameIndex"), "([NormalizedName] IS NOT NULL)");
				RelationalEntityTypeBuilderExtensions.ToTable(b, "AspNetRoles");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUserClaims", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<int>(b.Property<int>("Id"), "int");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ApplicationUserId"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ClaimType"), "nvarchar(400)").HasMaxLength(0x190);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ClaimValue"), "nvarchar(400)").HasMaxLength(0x190);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("UserId").IsRequired(true), "nvarchar(50)").HasMaxLength(50);
				b.HasKey(new string[] { "Id" });
				b.HasIndex(new string[] { "ApplicationUserId" });
				b.HasIndex(new string[] { "UserId" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "AspNetUserClaims");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUserLogins", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("LoginProvider"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ProviderKey"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ApplicationUserId"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ProviderDisplayName"), "nvarchar(400)").HasMaxLength(0x190);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("UserId").IsRequired(true), "nvarchar(50)").HasMaxLength(50);
				RelationalKeyBuilderExtensions.HasName(b.HasKey(new string[] { "LoginProvider", "ProviderKey" }), "PK_AspNetUserLogins_1");
				b.HasIndex(new string[] { "ApplicationUserId" });
				b.HasIndex(new string[] { "UserId" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "AspNetUserLogins");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUserRoles", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("UserId"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("RoleId"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ApplicationUserId"), "nvarchar(50)").HasMaxLength(50);
				b.HasKey(new string[] { "UserId", "RoleId" });
				b.HasIndex(new string[] { "ApplicationUserId" });
				b.HasIndex(new string[] { "RoleId" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "AspNetUserRoles");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUserTokens", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("UserId"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("LoginProvider"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Name"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Value"), "nvarchar(400)").HasMaxLength(0x190);
				b.HasKey(new string[] { "UserId", "LoginProvider", "Name" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "AspNetUserTokens");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUsers", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Id"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<int>(b.Property<int>("AccessFailedCount"), "int");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Avatar"), "nvarchar(250)").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ConcurrencyStamp"), "nvarchar(250)").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<int>(b.Property<int>("CountryId"), "int");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Culture"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnName<DateTime?>(b.Property<DateTime?>("Dob"), "DOB");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Email"), "nvarchar(250)").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<bool>(b.Property<bool>("EmailConfirmed"), "bit");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("FirstName"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Gender"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<bool>(b.Property<bool>("IsActived"), "bit");
				b.Property<DateTime>("JoinDate");
				b.Property<DateTime>("LastModified");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("LastName"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<bool>(b.Property<bool>("LockoutEnabled"), "bit");
				b.Property<DateTimeOffset?>("LockoutEnd");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ModifiedBy"), "nvarchar(250)").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("NickName"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("NormalizedEmail"), "nvarchar(250)").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("NormalizedUserName"), "nvarchar(250)").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("PasswordHash"), "nvarchar(250)").HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("PhoneNumber"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<bool>(b.Property<bool>("PhoneNumberConfirmed"), "bit");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("RegisterType"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("SecurityStamp"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<bool>(b.Property<bool>("TwoFactorEnabled"), "bit");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("UserName"), "nvarchar(250)").HasMaxLength(250);
				b.HasKey(new string[] { "Id" });
				RelationalIndexBuilderExtensions.HasName(b.HasIndex(new string[] { "NormalizedEmail" }), "EmailIndex");
				RelationalIndexBuilderExtensions.HasFilter(RelationalIndexBuilderExtensions.HasName(b.HasIndex(new string[] { "NormalizedUserName" }).IsUnique(true), "UserNameIndex"), "([NormalizedUserName] IS NOT NULL)");
				RelationalEntityTypeBuilderExtensions.ToTable(b, "AspNetUsers");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.Clients", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Id"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<bool>(b.Property<bool>("Active"), "bit");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("AllowedOrigin"), "nvarchar(100)").HasMaxLength(100);
				RelationalPropertyBuilderExtensions.HasColumnType<int>(b.Property<int>("ApplicationType"), "int");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Name").IsRequired(true), "nvarchar(100)").HasMaxLength(100);
				RelationalPropertyBuilderExtensions.HasColumnType<int>(b.Property<int>("RefreshTokenLifeTime"), "int");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Secret").IsRequired(true), "nvarchar(50)").HasMaxLength(50);
				b.HasKey(new string[] { "Id" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "Clients");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.RefreshTokens", (EntityTypeBuilder b) => {
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Id"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("ClientId"), "nvarchar(50)").HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Email").IsRequired(true), "nvarchar(250)").HasMaxLength(250);
				b.Property<DateTime>("ExpiresUtc");
				b.Property<DateTime>("IssuedUtc");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(b.Property<string>("Username"), "nvarchar(250)").HasMaxLength(250);
				b.HasKey(new string[] { "Id" });
				RelationalEntityTypeBuilderExtensions.ToTable(b, "RefreshTokens");
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetRoleClaims", (EntityTypeBuilder b) => b.HasOne("Mix.Cms.Lib.Models.Account.AspNetRoles", "Role").WithMany("AspNetRoleClaims").HasForeignKey(new string[] { "RoleId" }).OnDelete(3).IsRequired(true));
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUserClaims", (EntityTypeBuilder b) => {
				b.HasOne("Mix.Cms.Lib.Models.Account.AspNetUsers", "ApplicationUser").WithMany("AspNetUserClaimsApplicationUser").HasForeignKey(new string[] { "ApplicationUserId" });
				b.HasOne("Mix.Cms.Lib.Models.Account.AspNetUsers", "User").WithMany("AspNetUserClaimsUser").HasForeignKey(new string[] { "UserId" }).OnDelete(3).IsRequired(true);
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUserLogins", (EntityTypeBuilder b) => {
				b.HasOne("Mix.Cms.Lib.Models.Account.AspNetUsers", "ApplicationUser").WithMany("AspNetUserLoginsApplicationUser").HasForeignKey(new string[] { "ApplicationUserId" });
				b.HasOne("Mix.Cms.Lib.Models.Account.AspNetUsers", "User").WithMany("AspNetUserLoginsUser").HasForeignKey(new string[] { "UserId" }).OnDelete(3).IsRequired(true);
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUserRoles", (EntityTypeBuilder b) => {
				b.HasOne("Mix.Cms.Lib.Models.Account.AspNetUsers", "ApplicationUser").WithMany("AspNetUserRolesApplicationUser").HasForeignKey(new string[] { "ApplicationUserId" });
				b.HasOne("Mix.Cms.Lib.Models.Account.AspNetRoles", "Role").WithMany("AspNetUserRoles").HasForeignKey(new string[] { "RoleId" }).OnDelete(3).IsRequired(true);
				b.HasOne("Mix.Cms.Lib.Models.Account.AspNetUsers", "User").WithMany("AspNetUserRolesUser").HasForeignKey(new string[] { "UserId" }).OnDelete(3).IsRequired(true);
			});
			modelBuilder.Entity("Mix.Cms.Lib.Models.Account.AspNetUserTokens", (EntityTypeBuilder b) => b.HasOne("Mix.Cms.Lib.Models.Account.AspNetUsers", "User").WithMany("AspNetUserTokens").HasForeignKey(new string[] { "UserId" }).OnDelete(3).IsRequired(true));
		}
	}
}