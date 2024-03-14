using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Services;
using Mix.Identity.Entities;
using Mix.Identity.Models;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class MixDbContext : IdentityDbContext<ApplicationUser>
	{
		public DbSet<Client> Clients
		{
			get;
			set;
		}

		public DbSet<RefreshToken> RefreshTokens
		{
			get;
			set;
		}

		public MixDbContext(DbContextOptions<MixDbContext> options) : base(options)
		{
		}

		public MixDbContext()
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

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
	}
}