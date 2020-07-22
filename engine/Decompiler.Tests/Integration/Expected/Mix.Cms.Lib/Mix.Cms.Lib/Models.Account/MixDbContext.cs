using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mix.Cms.Lib;
using Mix.Identity.Entities;
using Mix.Identity.Models;
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

		public MixDbContext(DbContextOptions<MixDbContext> options)
		{
			base(options);
			return;
		}

		public MixDbContext()
		{
			base();
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
			return;
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			this.OnModelCreating(builder);
			return;
		}
	}
}