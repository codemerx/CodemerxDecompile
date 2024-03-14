using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Services;
using Mix.Identity.Data;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class MixCmsAccountContext : DbContext
	{
		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetRoleClaims> AspNetRoleClaims
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetRoles> AspNetRoles
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUserClaims> AspNetUserClaims
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUserLogins> AspNetUserLogins
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUserRoles> AspNetUserRoles
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUsers> AspNetUsers
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.AspNetUserTokens> AspNetUserTokens
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.Clients> Clients
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Account.RefreshTokens> RefreshTokens
		{
			get;
			set;
		}

		public MixCmsAccountContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public MixCmsAccountContext()
		{
		}

		public override void Dispose()
		{
			switch (Enum.Parse<MixEnums.DatabaseProvider>(MixService.GetConfig<string>("DatabaseProvider")))
			{
				case MixEnums.DatabaseProvider.MSSQL:
				{
					SqlConnection.ClearPool((SqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.get_Database()));
					break;
				}
				case MixEnums.DatabaseProvider.MySQL:
				{
					MySqlConnection.ClearPool((MySqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.get_Database()));
					break;
				}
				case MixEnums.DatabaseProvider.PostgreSQL:
				{
					NpgsqlConnection.ClearPool((NpgsqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.get_Database()));
					break;
				}
			}
			base.Dispose();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string connectionString = MixService.GetConnectionString("MixCmsConnection");
			if (!string.IsNullOrEmpty(connectionString))
			{
				switch (Enum.Parse<MixEnums.DatabaseProvider>(MixService.GetConfig<string>("DatabaseProvider")))
				{
					case MixEnums.DatabaseProvider.MSSQL:
					{
						SqlServerDbContextOptionsExtensions.UseSqlServer(optionsBuilder, connectionString, null);
						return;
					}
					case MixEnums.DatabaseProvider.MySQL:
					{
						MySqlDbContextOptionsExtensions.UseMySql(optionsBuilder, connectionString, null);
						return;
					}
					case MixEnums.DatabaseProvider.PostgreSQL:
					{
						NpgsqlDbContextOptionsExtensions.UseNpgsql(optionsBuilder, connectionString, null);
						break;
					}
					default:
					{
						return;
					}
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.AspNetRoleClaims>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetRoleClaims> entity) => {
				entity.HasIndex((Mix.Cms.Lib.Models.Account.AspNetRoleClaims e) => e.RoleId);
				entity.Property<int>((Mix.Cms.Lib.Models.Account.AspNetRoleClaims e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetRoleClaims e) => e.ClaimType).HasMaxLength(0x190);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetRoleClaims e) => e.ClaimValue).HasMaxLength(0x190);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetRoleClaims e) => e.RoleId).IsRequired(true).HasMaxLength(50);
				entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetRoles>((Mix.Cms.Lib.Models.Account.AspNetRoleClaims d) => d.Role).WithMany((Mix.Cms.Lib.Models.Account.AspNetRoles p) => p.AspNetRoleClaims).HasForeignKey((Mix.Cms.Lib.Models.Account.AspNetRoleClaims d) => d.RoleId);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.AspNetRoles>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetRoles> entity) => {
				RelationalIndexBuilderExtensions.HasFilter<Mix.Cms.Lib.Models.Account.AspNetRoles>(RelationalIndexBuilderExtensions.HasName<Mix.Cms.Lib.Models.Account.AspNetRoles>(entity.HasIndex((Mix.Cms.Lib.Models.Account.AspNetRoles e) => e.NormalizedName), "RoleNameIndex").IsUnique(true), "([NormalizedName] IS NOT NULL)");
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetRoles e) => e.Id).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetRoles e) => e.ConcurrencyStamp).HasMaxLength(0x190);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetRoles e) => e.Name).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetRoles e) => e.NormalizedName).HasMaxLength(250);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.AspNetUserClaims>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserClaims> entity) => {
				entity.HasIndex((Mix.Cms.Lib.Models.Account.AspNetUserClaims e) => e.ApplicationUserId);
				entity.HasIndex((Mix.Cms.Lib.Models.Account.AspNetUserClaims e) => e.UserId);
				entity.Property<int>((Mix.Cms.Lib.Models.Account.AspNetUserClaims e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUserClaims e) => e.ApplicationUserId).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUserClaims e) => e.ClaimType).HasMaxLength(0x190);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUserClaims e) => e.ClaimValue).HasMaxLength(0x190);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUserClaims e) => e.UserId).IsRequired(true).HasMaxLength(50);
				entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetUsers>((Mix.Cms.Lib.Models.Account.AspNetUserClaims d) => d.ApplicationUser).WithMany((Mix.Cms.Lib.Models.Account.AspNetUsers p) => p.AspNetUserClaimsApplicationUser).HasForeignKey((Mix.Cms.Lib.Models.Account.AspNetUserClaims d) => d.ApplicationUserId);
				entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetUsers>((Mix.Cms.Lib.Models.Account.AspNetUserClaims d) => d.User).WithMany((Mix.Cms.Lib.Models.Account.AspNetUsers p) => p.AspNetUserClaimsUser).HasForeignKey((Mix.Cms.Lib.Models.Account.AspNetUserClaims d) => d.UserId);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.AspNetUserLogins>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserLogins> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType46<string, string>).GetMethod(".ctor", new Type[] { typeof(u003cLoginProvideru003ej__TPar), typeof(u003cProviderKeyu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType46<string, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_LoginProvider").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_ProviderKey").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType46<string, string>).GetMethod("get_LoginProvider").MethodHandle, typeof(u003cu003ef__AnonymousType46<string, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType46<string, string>).GetMethod("get_ProviderKey").MethodHandle, typeof(u003cu003ef__AnonymousType46<string, string>).TypeHandle) };
				RelationalKeyBuilderExtensions.HasName(entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression })), "PK_AspNetUserLogins_1");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_ApplicationUserId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_LoginProvider").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_ProviderKey").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_ApplicationUserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_ProviderDisplayName").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x190);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Account.AspNetUserLogins, Mix.Cms.Lib.Models.Account.AspNetUsers> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetUsers>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, Mix.Cms.Lib.Models.Account.AspNetUsers>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_ApplicationUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Account.AspNetUsers, Mix.Cms.Lib.Models.Account.AspNetUserLogins> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUsers, IEnumerable<Mix.Cms.Lib.Models.Account.AspNetUserLogins>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers).GetMethod("get_AspNetUserLoginsApplicationUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "d");
				referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_ApplicationUserId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Account.AspNetUserLogins, Mix.Cms.Lib.Models.Account.AspNetUsers> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetUsers>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, Mix.Cms.Lib.Models.Account.AspNetUsers>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_User").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Account.AspNetUsers, Mix.Cms.Lib.Models.Account.AspNetUserLogins> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUsers, IEnumerable<Mix.Cms.Lib.Models.Account.AspNetUserLogins>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers).GetMethod("get_AspNetUserLoginsUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins), "d");
				referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserLogins, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserLogins).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.AspNetUserRoles>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserRoles> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType48<string, string>).GetMethod(".ctor", new Type[] { typeof(u003cUserIdu003ej__TPar), typeof(u003cRoleIdu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType48<string, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_UserId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_RoleId").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType48<string, string>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType48<string, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType48<string, string>).GetMethod("get_RoleId").MethodHandle, typeof(u003cu003ef__AnonymousType48<string, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_ApplicationUserId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_RoleId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_RoleId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_ApplicationUserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Account.AspNetUserRoles, Mix.Cms.Lib.Models.Account.AspNetUsers> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetUsers>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, Mix.Cms.Lib.Models.Account.AspNetUsers>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_ApplicationUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Account.AspNetUsers, Mix.Cms.Lib.Models.Account.AspNetUserRoles> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUsers, IEnumerable<Mix.Cms.Lib.Models.Account.AspNetUserRoles>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers).GetMethod("get_AspNetUserRolesApplicationUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "d");
				referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_ApplicationUserId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Account.AspNetUserRoles, Mix.Cms.Lib.Models.Account.AspNetRoles> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetRoles>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, Mix.Cms.Lib.Models.Account.AspNetRoles>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_Role").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetRoles), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Account.AspNetRoles, Mix.Cms.Lib.Models.Account.AspNetUserRoles> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetRoles, IEnumerable<Mix.Cms.Lib.Models.Account.AspNetUserRoles>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetRoles).GetMethod("get_AspNetUserRoles").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "d");
				referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_RoleId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Account.AspNetUserRoles, Mix.Cms.Lib.Models.Account.AspNetUsers> referenceNavigationBuilder2 = entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetUsers>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, Mix.Cms.Lib.Models.Account.AspNetUsers>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_User").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Account.AspNetUsers, Mix.Cms.Lib.Models.Account.AspNetUserRoles> referenceCollectionBuilder2 = referenceNavigationBuilder2.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUsers, IEnumerable<Mix.Cms.Lib.Models.Account.AspNetUserRoles>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers).GetMethod("get_AspNetUserRolesUser").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles), "d");
				referenceCollectionBuilder2.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserRoles, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserRoles).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.AspNetUserTokens>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUserTokens> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType50<string, string, string>).GetMethod(".ctor", new Type[] { typeof(u003cUserIdu003ej__TPar), typeof(u003cLoginProvideru003ej__TPar), typeof(u003cNameu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType50<string, string, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_UserId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_LoginProvider").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_Name").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType50<string, string, string>).GetMethod("get_UserId").MethodHandle, typeof(u003cu003ef__AnonymousType50<string, string, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType50<string, string, string>).GetMethod("get_LoginProvider").MethodHandle, typeof(u003cu003ef__AnonymousType50<string, string, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType50<string, string, string>).GetMethod("get_Name").MethodHandle, typeof(u003cu003ef__AnonymousType50<string, string, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserTokens, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserTokens, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserTokens, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_LoginProvider").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserTokens, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_Name").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserTokens, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_Value").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x190);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Account.AspNetUserTokens, Mix.Cms.Lib.Models.Account.AspNetUsers> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Account.AspNetUsers>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserTokens, Mix.Cms.Lib.Models.Account.AspNetUsers>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_User").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Account.AspNetUsers, Mix.Cms.Lib.Models.Account.AspNetUserTokens> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUsers, IEnumerable<Mix.Cms.Lib.Models.Account.AspNetUserTokens>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUsers).GetMethod("get_AspNetUserTokens").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens), "d");
				referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Account.AspNetUserTokens, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Account.AspNetUserTokens).GetMethod("get_UserId").MethodHandle)), new ParameterExpression[] { parameterExpression }));
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.AspNetUsers>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.AspNetUsers> entity) => {
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Lib.Models.Account.AspNetUsers>(entity.HasIndex((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.NormalizedEmail), "EmailIndex");
				RelationalIndexBuilderExtensions.HasFilter<Mix.Cms.Lib.Models.Account.AspNetUsers>(RelationalIndexBuilderExtensions.HasName<Mix.Cms.Lib.Models.Account.AspNetUsers>(entity.HasIndex((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.NormalizedUserName), "UserNameIndex").IsUnique(true), "([NormalizedUserName] IS NOT NULL)");
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.Id).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.Avatar).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.ConcurrencyStamp).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.Culture).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(RelationalPropertyBuilderExtensions.HasColumnName<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.Dob), "DOB"), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.Email).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.FirstName).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.Gender).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.JoinDate), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.LastName).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.LockoutEnd), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.ModifiedBy).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.NickName).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.NormalizedEmail).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.NormalizedUserName).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.PasswordHash).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.PhoneNumber).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.RegisterType).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.SecurityStamp).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.AspNetUsers e) => e.UserName).HasMaxLength(250);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.Clients>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.Clients> entity) => {
				entity.Property<string>((Mix.Cms.Lib.Models.Account.Clients e) => e.Id).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.Clients e) => e.AllowedOrigin).HasMaxLength(100);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.Clients e) => e.Name).IsRequired(true).HasMaxLength(100);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.Clients e) => e.Secret).IsRequired(true).HasMaxLength(50);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Account.RefreshTokens>((EntityTypeBuilder<Mix.Cms.Lib.Models.Account.RefreshTokens> entity) => {
				entity.Property<string>((Mix.Cms.Lib.Models.Account.RefreshTokens e) => e.Id).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.RefreshTokens e) => e.ClientId).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Account.RefreshTokens e) => e.Email).IsRequired(true).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Account.RefreshTokens e) => e.ExpiresUtc), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Account.RefreshTokens e) => e.IssuedUtc), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Account.RefreshTokens e) => e.Username).HasMaxLength(250);
			});
		}
	}
}