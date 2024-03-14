using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Services;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixCmsContext : DbContext
	{
		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeField> MixAttributeField
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeSet> MixAttributeSet
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeSetData> MixAttributeSetData
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference> MixAttributeSetReference
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixAttributeSetValue> MixAttributeSetValue
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixCache> MixCache
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixCmsUser> MixCmsUser
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixConfiguration> MixConfiguration
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixCulture> MixCulture
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixFile> MixFile
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixLanguage> MixLanguage
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixMedia> MixMedia
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixModule> MixModule
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixModuleData> MixModuleData
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixModulePost> MixModulePost
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPage> MixPage
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPageModule> MixPageModule
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPagePost> MixPagePost
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPortalPage> MixPortalPage
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation> MixPortalPageNavigation
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPortalPageRole> MixPortalPageRole
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPost> MixPost
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPostMedia> MixPostMedia
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixPostModule> MixPostModule
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData> MixRelatedAttributeData
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet> MixRelatedAttributeSet
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixRelatedData> MixRelatedData
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixRelatedPost> MixRelatedPost
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixTemplate> MixTemplate
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixTheme> MixTheme
		{
			get;
			set;
		}

		public virtual DbSet<Mix.Cms.Lib.Models.Cms.MixUrlAlias> MixUrlAlias
		{
			get;
			set;
		}

		public MixCmsContext(DbContextOptions<MixCmsContext> options) : base(options)
		{
		}

		public MixCmsContext()
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
			optionsBuilder.EnableSensitiveDataLogging(true);
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
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeField>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeField> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixAttributeField>(entity, "mix_attribute_field");
				entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => (object)e.AttributeSetId);
				entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => (object)e.ReferenceId);
				entity.Property<int>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.AttributeSetName).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.CreatedDateTime), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.DefaultValue), "text");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.Name).IsRequired(true).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.Options), "text");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.Regex).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeField e) => e.Title).HasMaxLength(250);
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixAttributeSet, Mix.Cms.Lib.Models.Cms.MixAttributeField>(entity.HasOne<Mix.Cms.Lib.Models.Cms.MixAttributeSet>((Mix.Cms.Lib.Models.Cms.MixAttributeField d) => d.AttributeSet).WithMany((Mix.Cms.Lib.Models.Cms.MixAttributeSet p) => p.MixAttributeFieldAttributeSet).HasForeignKey((Mix.Cms.Lib.Models.Cms.MixAttributeField d) => (object)d.AttributeSetId).OnDelete(0), "FK_mix_attribute_field_mix_attribute_set");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixAttributeSet, Mix.Cms.Lib.Models.Cms.MixAttributeField>(entity.HasOne<Mix.Cms.Lib.Models.Cms.MixAttributeSet>((Mix.Cms.Lib.Models.Cms.MixAttributeField d) => d.Reference).WithMany((Mix.Cms.Lib.Models.Cms.MixAttributeSet p) => p.MixAttributeFieldReference).HasForeignKey((Mix.Cms.Lib.Models.Cms.MixAttributeField d) => (object)d.ReferenceId), "FK_mix_attribute_field_mix_attribute_set1");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeSet>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSet> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixAttributeSet>(entity, "mix_attribute_set");
				entity.Property<int>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.Description).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.EdmFrom).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.EdmSubject).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.EdmTemplate).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.FormTemplate).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.Name).IsRequired(true).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSet e) => e.Title).IsRequired(true).HasMaxLength(250);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeSetData>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSetData> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixAttributeSetData>(entity, "mix_attribute_set_data");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, object>>(Expression.Convert(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_AttributeSetId").MethodHandle)), typeof(object)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_Id").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_AttributeSetName").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, Mix.Cms.Lib.Models.Cms.MixAttributeSet> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixAttributeSet>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, Mix.Cms.Lib.Models.Cms.MixAttributeSet>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_AttributeSet").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSet), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSet, Mix.Cms.Lib.Models.Cms.MixAttributeSetData> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSet, IEnumerable<Mix.Cms.Lib.Models.Cms.MixAttributeSetData>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSet).GetMethod("get_MixAttributeSetData").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixAttributeSet, Mix.Cms.Lib.Models.Cms.MixAttributeSetData>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSetData, object>>(Expression.Convert(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSetData).GetMethod("get_AttributeSetId").MethodHandle)), typeof(object)), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_mix_attribute_set_data_mix_attribute_set");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference>(entity, "mix_attribute_set_reference");
				entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => (object)e.AttributeSetId);
				entity.Property<int>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => e.Description).HasMaxLength(0x1c2);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => e.Image).HasMaxLength(0x1c2);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixAttributeSet, Mix.Cms.Lib.Models.Cms.MixAttributeSetReference>(entity.HasOne<Mix.Cms.Lib.Models.Cms.MixAttributeSet>((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference d) => d.AttributeSet).WithMany((Mix.Cms.Lib.Models.Cms.MixAttributeSet p) => p.MixAttributeSetReference).HasForeignKey((Mix.Cms.Lib.Models.Cms.MixAttributeSetReference d) => (object)d.AttributeSetId).OnDelete(0), "FK_mix_attribute_set_reference_mix_attribute_set");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixAttributeSetValue>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSetValue> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixAttributeSetValue>(entity, "mix_attribute_set_value");
				entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.DataId);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.Id).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.AttributeFieldName).IsRequired(true).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.AttributeSetName).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.DataId).IsRequired(true).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.DateTimeValue), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.EncryptKey).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.EncryptValue).HasMaxLength(0xfa0);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.Regex).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.Specificulture).IsRequired(true).HasMaxLength(10);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixAttributeSetValue e) => e.StringValue).HasMaxLength(0xfa0);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixCache>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixCache> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixCache>(entity, "mix_cache");
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Lib.Models.Cms.MixCache>(entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixCache e) => (object)e.ExpiredDateTime), "Index_ExpiresAtTime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCache e) => e.Id).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCache e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixCache e) => e.CreatedDateTime), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixCache e) => e.ExpiredDateTime), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixCache e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCache e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCache e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCache e) => e.Value).IsRequired(true).HasMaxLength(0xfa0);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixCmsUser>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixCmsUser> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixCmsUser>(entity, "mix_cms_user");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.Id).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.Address).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.Avatar).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.Email).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.FirstName).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.LastName).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.MiddleName).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.PhoneNumber).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCmsUser e) => e.Username).HasMaxLength(250);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixConfiguration>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixConfiguration> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				RelationalKeyBuilderExtensions.HasName(entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression })), "PK_mix_configuration_1");
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixConfiguration>(entity, "mix_configuration");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Category").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Keyword").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Value").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixConfiguration, Mix.Cms.Lib.Models.Cms.MixCulture> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixCulture>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, Mix.Cms.Lib.Models.Cms.MixCulture>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_SpecificultureNavigation").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixConfiguration> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, IEnumerable<Mix.Cms.Lib.Models.Cms.MixConfiguration>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_MixConfiguration").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixConfiguration> referenceCollectionBuilder1 = referenceCollectionBuilder.HasPrincipalKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixConfiguration>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixConfiguration, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixConfiguration).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })), "FK_Mix_Configuration_Mix_Culture");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixCulture>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixCulture> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixCulture>(entity, "mix_culture");
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Lib.Models.Cms.MixCulture>(entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.Specificulture), "IX_Mix_Culture").IsUnique(true);
				entity.Property<int>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.Alias).HasMaxLength(150);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.Description).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.FullName).HasMaxLength(150);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.Icon).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.LastModified), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnName<string>(entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.Lcid), "LCID").HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.Specificulture).IsRequired(true).HasMaxLength(10);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixCulture e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixFile>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixFile> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixFile>(entity, "mix_file");
				entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixFile e) => (object)e.ThemeId);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.Extension).IsRequired(true).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.FileFolder).IsRequired(true).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.FileName).IsRequired(true).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.FolderType).IsRequired(true).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.StringContent).IsRequired(true).HasMaxLength(0xfa0);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixFile e) => e.ThemeName).IsRequired(true).HasMaxLength(250);
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixTheme, Mix.Cms.Lib.Models.Cms.MixFile>(entity.HasOne<Mix.Cms.Lib.Models.Cms.MixTheme>((Mix.Cms.Lib.Models.Cms.MixFile d) => d.Theme).WithMany((Mix.Cms.Lib.Models.Cms.MixTheme p) => p.MixFile).HasForeignKey((Mix.Cms.Lib.Models.Cms.MixFile d) => (object)d.ThemeId), "FK_mix_file_mix_template");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixLanguage>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixLanguage> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				RelationalKeyBuilderExtensions.HasName(entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression })), "PK_mix_language_1");
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixLanguage>(entity, "mix_language");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Category").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_DefaultValue").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Keyword").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Value").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixLanguage, Mix.Cms.Lib.Models.Cms.MixCulture> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixCulture>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, Mix.Cms.Lib.Models.Cms.MixCulture>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_SpecificultureNavigation").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixLanguage> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, IEnumerable<Mix.Cms.Lib.Models.Cms.MixLanguage>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_MixLanguage").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixLanguage> referenceCollectionBuilder1 = referenceCollectionBuilder.HasPrincipalKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixLanguage>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixLanguage, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixLanguage).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })), "FK_Mix_Language_Mix_Culture");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixMedia>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixMedia> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixMedia>(entity, "mix_media");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Extension").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_FileFolder").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_FileName").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_FileProperties").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_FileType").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Source").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Tags").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x190);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_TargetUrl").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_Title").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixModule>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixModule> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixModule>(entity, "mix_module");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_EdmTemplate").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Fields").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_FormTemplate").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Name").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Template").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Thumbnail").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Title").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixCulture> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixCulture>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixCulture>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_SpecificultureNavigation").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixModule> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, IEnumerable<Mix.Cms.Lib.Models.Cms.MixModule>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_MixModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixModule> referenceCollectionBuilder1 = referenceCollectionBuilder.HasPrincipalKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixModule>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })), "FK_Mix_Module_Mix_Culture");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixModuleData>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixModuleData> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixModuleData>(entity, "mix_module_data");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				ConstructorInfo constructorInfo = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle);
				Expression[] expressionArray1 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, object>>(Expression.New(constructorInfo, (IEnumerable<Expression>)expressionArray1, methodFromHandle1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				ConstructorInfo constructorInfo1 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int?, string>).GetMethod(".ctor", new Type[] { typeof(u003cPageIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType28<int?, string>).TypeHandle);
				Expression[] expressionArray2 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_PageId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int?, string>).GetMethod("get_PageId").MethodHandle, typeof(u003cu003ef__AnonymousType28<int?, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int?, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType28<int?, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, object>>(Expression.New(constructorInfo1, (IEnumerable<Expression>)expressionArray2, memberInfoArray1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				ConstructorInfo methodFromHandle2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int?, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int?, string>).TypeHandle);
				Expression[] expressionArray3 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray2 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int?, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int?, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int?, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int?, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, object>>(Expression.New(methodFromHandle2, (IEnumerable<Expression>)expressionArray3, memberInfoArray2), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				ConstructorInfo constructorInfo2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType64<int, int?, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cPageIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType64<int, int?, string>).TypeHandle);
				Expression[] expressionArray4 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_PageId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle3 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType64<int, int?, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType64<int, int?, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType64<int, int?, string>).GetMethod("get_PageId").MethodHandle, typeof(u003cu003ef__AnonymousType64<int, int?, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType64<int, int?, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType64<int, int?, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, object>>(Expression.New(constructorInfo2, (IEnumerable<Expression>)expressionArray4, methodFromHandle3), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Id").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Fields").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Value").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixModuleData, Mix.Cms.Lib.Models.Cms.MixModule> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixModule>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, Mix.Cms.Lib.Models.Cms.MixModule>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_MixModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixModuleData> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, IEnumerable<Mix.Cms.Lib.Models.Cms.MixModuleData>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_MixModuleData").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "d");
				ConstructorInfo constructorInfo3 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle);
				Expression[] expressionArray5 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray3 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixModuleData>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, object>>(Expression.New(constructorInfo3, (IEnumerable<Expression>)expressionArray5, memberInfoArray3), new ParameterExpression[] { parameterExpression })), "FK_Mix_Module_Data_Mix_Module");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixModuleData, Mix.Cms.Lib.Models.Cms.MixPage> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPage>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, Mix.Cms.Lib.Models.Cms.MixPage>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_MixPage").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPage, Mix.Cms.Lib.Models.Cms.MixModuleData> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, IEnumerable<Mix.Cms.Lib.Models.Cms.MixModuleData>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_MixModuleData").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "d");
				ConstructorInfo methodFromHandle4 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int?, string>).GetMethod(".ctor", new Type[] { typeof(u003cPageIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType28<int?, string>).TypeHandle);
				Expression[] expressionArray6 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_PageId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray4 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int?, string>).GetMethod("get_PageId").MethodHandle, typeof(u003cu003ef__AnonymousType28<int?, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int?, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType28<int?, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPage, Mix.Cms.Lib.Models.Cms.MixModuleData>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, object>>(Expression.New(methodFromHandle4, (IEnumerable<Expression>)expressionArray6, memberInfoArray4), new ParameterExpression[] { parameterExpression })), "FK_mix_module_data_mix_page");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixModuleData, Mix.Cms.Lib.Models.Cms.MixPost> referenceNavigationBuilder2 = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPost>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, Mix.Cms.Lib.Models.Cms.MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixModuleData> referenceCollectionBuilder2 = referenceNavigationBuilder2.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, IEnumerable<Mix.Cms.Lib.Models.Cms.MixModuleData>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_MixModuleData").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData), "d");
				ConstructorInfo constructorInfo4 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int?, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int?, string>).TypeHandle);
				Expression[] expressionArray7 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModuleData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle5 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int?, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int?, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int?, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int?, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixModuleData>(referenceCollectionBuilder2.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModuleData, object>>(Expression.New(constructorInfo4, (IEnumerable<Expression>)expressionArray7, methodFromHandle5), new ParameterExpression[] { parameterExpression })), "FK_mix_module_data_mix_post");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixModulePost>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixModulePost> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixModulePost>(entity, "mix_module_post");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				ConstructorInfo constructorInfo = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle);
				Expression[] expressionArray1 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, object>>(Expression.New(constructorInfo, (IEnumerable<Expression>)expressionArray1, methodFromHandle1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				ConstructorInfo constructorInfo1 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle);
				Expression[] expressionArray2 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, object>>(Expression.New(constructorInfo1, (IEnumerable<Expression>)expressionArray2, memberInfoArray1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixModulePost, Mix.Cms.Lib.Models.Cms.MixModule> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixModule>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, Mix.Cms.Lib.Models.Cms.MixModule>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_MixModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixModulePost> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, IEnumerable<Mix.Cms.Lib.Models.Cms.MixModulePost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_MixModulePost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "d");
				ConstructorInfo methodFromHandle2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle);
				Expression[] expressionArray3 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray2 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixModulePost>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, object>>(Expression.New(methodFromHandle2, (IEnumerable<Expression>)expressionArray3, memberInfoArray2), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_Mix_Module_Post_Mix_Module");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixModulePost, Mix.Cms.Lib.Models.Cms.MixPost> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPost>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, Mix.Cms.Lib.Models.Cms.MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixModulePost> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, IEnumerable<Mix.Cms.Lib.Models.Cms.MixModulePost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_MixModulePost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost), "d");
				ConstructorInfo constructorInfo2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle);
				Expression[] expressionArray4 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModulePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle3 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixModulePost>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModulePost, object>>(Expression.New(constructorInfo2, (IEnumerable<Expression>)expressionArray4, methodFromHandle3), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_Mix_Module_Post_Mix_Post");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPage>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPage> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPage>(entity, "mix_page");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Content").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_CssClass").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Excerpt").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_ExtraFields").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Icon").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Layout").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_SeoDescription").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_SeoKeywords").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_SeoName").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x1f4).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_SeoTitle").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_StaticUrl").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Tags").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Template").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Title").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Type").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPage, Mix.Cms.Lib.Models.Cms.MixCulture> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixCulture>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, Mix.Cms.Lib.Models.Cms.MixCulture>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_SpecificultureNavigation").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixPage> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPage>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_MixPage").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixPage> referenceCollectionBuilder1 = referenceCollectionBuilder.HasPrincipalKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixPage>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_Mix_Page_Mix_Culture");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPageModule>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPageModule> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPageModule>(entity, "mix_page_module");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				ConstructorInfo constructorInfo = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle);
				Expression[] expressionArray1 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, object>>(Expression.New(constructorInfo, (IEnumerable<Expression>)expressionArray1, methodFromHandle1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				ConstructorInfo constructorInfo1 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPageIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle);
				Expression[] expressionArray2 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_PageId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod("get_PageId").MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, object>>(Expression.New(constructorInfo1, (IEnumerable<Expression>)expressionArray2, memberInfoArray1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPageModule, Mix.Cms.Lib.Models.Cms.MixModule> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixModule>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, Mix.Cms.Lib.Models.Cms.MixModule>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_MixModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixPageModule> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPageModule>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_MixPageModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "d");
				ConstructorInfo methodFromHandle2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle);
				Expression[] expressionArray3 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray2 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixPageModule>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, object>>(Expression.New(methodFromHandle2, (IEnumerable<Expression>)expressionArray3, memberInfoArray2), new ParameterExpression[] { parameterExpression })), "FK_Mix_Menu_Module_Mix_Module1");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPageModule, Mix.Cms.Lib.Models.Cms.MixPage> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPage>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, Mix.Cms.Lib.Models.Cms.MixPage>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_MixPage").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPage, Mix.Cms.Lib.Models.Cms.MixPageModule> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPageModule>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_MixPageModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule), "d");
				ConstructorInfo constructorInfo2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPageIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle);
				Expression[] expressionArray4 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_PageId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPageModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle3 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod("get_PageId").MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPage, Mix.Cms.Lib.Models.Cms.MixPageModule>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPageModule, object>>(Expression.New(constructorInfo2, (IEnumerable<Expression>)expressionArray4, methodFromHandle3), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_Mix_Page_Module_Mix_Page");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPagePost>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPagePost> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPagePost>(entity, "mix_page_post");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				ConstructorInfo constructorInfo = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPageIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle);
				Expression[] expressionArray1 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_PageId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod("get_PageId").MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, object>>(Expression.New(constructorInfo, (IEnumerable<Expression>)expressionArray1, methodFromHandle1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				ConstructorInfo constructorInfo1 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle);
				Expression[] expressionArray2 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, object>>(Expression.New(constructorInfo1, (IEnumerable<Expression>)expressionArray2, memberInfoArray1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPagePost, Mix.Cms.Lib.Models.Cms.MixPage> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPage>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, Mix.Cms.Lib.Models.Cms.MixPage>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_MixPage").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPage), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPage, Mix.Cms.Lib.Models.Cms.MixPagePost> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPage, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPagePost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPage).GetMethod("get_MixPagePost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "d");
				ConstructorInfo methodFromHandle2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPageIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle);
				Expression[] expressionArray3 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_PageId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray2 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod("get_PageId").MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType28<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType28<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPage, Mix.Cms.Lib.Models.Cms.MixPagePost>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, object>>(Expression.New(methodFromHandle2, (IEnumerable<Expression>)expressionArray3, memberInfoArray2), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_Mix_Page_Post_Mix_Page");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPagePost, Mix.Cms.Lib.Models.Cms.MixPost> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPost>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, Mix.Cms.Lib.Models.Cms.MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixPagePost> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPagePost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_MixPagePost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost), "d");
				ConstructorInfo constructorInfo2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle);
				Expression[] expressionArray4 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPagePost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle3 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixPagePost>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPagePost, object>>(Expression.New(constructorInfo2, (IEnumerable<Expression>)expressionArray4, methodFromHandle3), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_Mix_Page_Post_Mix_Post");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPortalPage>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPortalPage> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPortalPage>(entity, "mix_portal_page");
				entity.Property<int>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.Description).HasMaxLength(0x1c2);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.Icon).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.TextDefault).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.TextKeyword).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPage e) => e.Url).HasMaxLength(250);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation>(entity, "mix_portal_page_navigation");
				entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => (object)e.ParentId);
				entity.Property<int>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => e.Description).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => e.Image).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation, Mix.Cms.Lib.Models.Cms.MixPortalPage>(entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPortalPage>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation d) => d.IdNavigation).WithOne((Mix.Cms.Lib.Models.Cms.MixPortalPage p) => p.MixPortalPageNavigationIdNavigation).HasForeignKey<Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation d) => (object)d.Id).OnDelete(0), "FK_mix_portal_page_navigation_mix_portal_page");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPortalPage, Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation>(entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPortalPage>((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation d) => d.Parent).WithMany((Mix.Cms.Lib.Models.Cms.MixPortalPage p) => p.MixPortalPageNavigationParent).HasForeignKey((Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation d) => (object)d.ParentId).OnDelete(0), "FK_mix_portal_page_navigation_mix_portal_page1");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPortalPageRole>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPortalPageRole> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType23<string, int>).GetMethod(".ctor", new Type[] { typeof(u003cRoleIdu003ej__TPar), typeof(u003cPageIdu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType23<string, int>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_RoleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_PageId").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType23<string, int>).GetMethod("get_RoleId").MethodHandle, typeof(u003cu003ef__AnonymousType23<string, int>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType23<string, int>).GetMethod("get_PageId").MethodHandle, typeof(u003cu003ef__AnonymousType23<string, int>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPortalPageRole>(entity, "mix_portal_page_role");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, object>>(Expression.Convert(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_PageId").MethodHandle)), typeof(object)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_RoleId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, Mix.Cms.Lib.Models.Cms.MixPortalPage> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPortalPage>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, Mix.Cms.Lib.Models.Cms.MixPortalPage>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_Page").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPage), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPortalPage, Mix.Cms.Lib.Models.Cms.MixPortalPageRole> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPage, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPortalPageRole>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPage).GetMethod("get_MixPortalPageRole").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPortalPage, Mix.Cms.Lib.Models.Cms.MixPortalPageRole>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPortalPageRole, object>>(Expression.Convert(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPortalPageRole).GetMethod("get_PageId").MethodHandle)), typeof(object)), new ParameterExpression[] { parameterExpression })), "FK_mix_portal_page_role_mix_portal_page");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPost>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPost> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPost>(entity, "mix_post");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Content").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Excerpt").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_ExtraFields").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_ExtraProperties").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Icon").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_PublishedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_SeoDescription").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_SeoKeywords").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_SeoName").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x1f4).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_SeoTitle").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Source").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Tags").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Template").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Thumbnail").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Title").MethodHandle)), new ParameterExpression[] { parameterExpression })), "text");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Type").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixCulture> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixCulture>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixCulture>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_SpecificultureNavigation").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixPost> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixPost> referenceCollectionBuilder1 = referenceCollectionBuilder.HasPrincipalKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixPost>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_Mix_Post_Mix_Culture");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPostMedia>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPostMedia> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPostMedia>(entity, "mix_post_media");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				ConstructorInfo constructorInfo = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType34<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cMediaIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType34<int, string>).TypeHandle);
				Expression[] expressionArray1 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_MediaId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType34<int, string>).GetMethod("get_MediaId").MethodHandle, typeof(u003cu003ef__AnonymousType34<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType34<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType34<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, object>>(Expression.New(constructorInfo, (IEnumerable<Expression>)expressionArray1, methodFromHandle1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				ConstructorInfo constructorInfo1 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle);
				Expression[] expressionArray2 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, object>>(Expression.New(constructorInfo1, (IEnumerable<Expression>)expressionArray2, memberInfoArray1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPostMedia, Mix.Cms.Lib.Models.Cms.MixMedia> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixMedia>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, Mix.Cms.Lib.Models.Cms.MixMedia>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_MixMedia").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixMedia), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixMedia, Mix.Cms.Lib.Models.Cms.MixPostMedia> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixMedia, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPostMedia>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixMedia).GetMethod("get_MixPostMedia").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "d");
				ConstructorInfo methodFromHandle2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType34<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cMediaIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType34<int, string>).TypeHandle);
				Expression[] expressionArray3 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_MediaId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray2 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType34<int, string>).GetMethod("get_MediaId").MethodHandle, typeof(u003cu003ef__AnonymousType34<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType34<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType34<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixMedia, Mix.Cms.Lib.Models.Cms.MixPostMedia>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, object>>(Expression.New(methodFromHandle2, (IEnumerable<Expression>)expressionArray3, memberInfoArray2), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_mix_post_media_mix_media");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPostMedia, Mix.Cms.Lib.Models.Cms.MixPost> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPost>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, Mix.Cms.Lib.Models.Cms.MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixPostMedia> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPostMedia>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_MixPostMedia").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia), "d");
				ConstructorInfo constructorInfo2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle);
				Expression[] expressionArray4 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostMedia).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle3 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixPostMedia>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostMedia, object>>(Expression.New(constructorInfo2, (IEnumerable<Expression>)expressionArray4, methodFromHandle3), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_mix_post_media_mix_post");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixPostModule>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixPostModule> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixPostModule>(entity, "mix_post_module");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				ConstructorInfo constructorInfo = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle);
				Expression[] expressionArray1 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, object>>(Expression.New(constructorInfo, (IEnumerable<Expression>)expressionArray1, methodFromHandle1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				ConstructorInfo constructorInfo1 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle);
				Expression[] expressionArray2 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, object>>(Expression.New(constructorInfo1, (IEnumerable<Expression>)expressionArray2, memberInfoArray1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPostModule, Mix.Cms.Lib.Models.Cms.MixModule> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixModule>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, Mix.Cms.Lib.Models.Cms.MixModule>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_MixModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixModule), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixPostModule> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixModule, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPostModule>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixModule).GetMethod("get_MixPostModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "d");
				ConstructorInfo methodFromHandle2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cModuleIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle);
				Expression[] expressionArray3 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_ModuleId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray2 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_ModuleId").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType27<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType27<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixModule, Mix.Cms.Lib.Models.Cms.MixPostModule>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, object>>(Expression.New(methodFromHandle2, (IEnumerable<Expression>)expressionArray3, memberInfoArray2), new ParameterExpression[] { parameterExpression })), "FK_Mix_Post_Module_Mix_Module1");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixPostModule, Mix.Cms.Lib.Models.Cms.MixPost> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPost>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, Mix.Cms.Lib.Models.Cms.MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixPostModule> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, IEnumerable<Mix.Cms.Lib.Models.Cms.MixPostModule>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_MixPostModule").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule), "d");
				ConstructorInfo constructorInfo2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cPostIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle);
				Expression[] expressionArray4 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_PostId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPostModule).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle3 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_PostId").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType30<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType30<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixPostModule>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPostModule, object>>(Expression.New(constructorInfo2, (IEnumerable<Expression>)expressionArray4, methodFromHandle3), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_Mix_Post_Module_Mix_Post");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<string, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<string, string>).TypeHandle) };
				RelationalKeyBuilderExtensions.HasName(entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression })), "PK_mix_related_attribute_data_1");
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData>(entity, "mix_related_attribute_data");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_Id").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_AttributeSetName").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_DataId").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x1c2);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_ParentId").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_ParentType").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet>(entity, "mix_related_attribute_set");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x1c2);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x1c2);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_ParentType").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, Mix.Cms.Lib.Models.Cms.MixAttributeSet> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixAttributeSet>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, Mix.Cms.Lib.Models.Cms.MixAttributeSet>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_IdNavigation").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSet), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixAttributeSet, Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixAttributeSet, IEnumerable<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixAttributeSet).GetMethod("get_MixRelatedAttributeSet").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixAttributeSet, Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet, object>>(Expression.Convert(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet).GetMethod("get_Id").MethodHandle)), typeof(object)), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_mix_related_attribute_set_mix_attribute_set");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixRelatedData>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedData> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixRelatedData>(entity, "mix_related_data");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_AttributeSetName").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_DataId").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x1c2);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_ParentId").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_ParentType").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedData, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedData).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixRelatedPost>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedPost> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixRelatedPost>(entity, "mix_related_post");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				ConstructorInfo constructorInfo = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType37<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cDestinationIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType37<int, string>).TypeHandle);
				Expression[] expressionArray1 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_DestinationId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType37<int, string>).GetMethod("get_DestinationId").MethodHandle, typeof(u003cu003ef__AnonymousType37<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType37<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType37<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, object>>(Expression.New(constructorInfo, (IEnumerable<Expression>)expressionArray1, methodFromHandle1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				ConstructorInfo constructorInfo1 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType38<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cSourceIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType38<int, string>).TypeHandle);
				Expression[] expressionArray2 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_SourceId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray1 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType38<int, string>).GetMethod("get_SourceId").MethodHandle, typeof(u003cu003ef__AnonymousType38<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType38<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType38<int, string>).TypeHandle) };
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, object>>(Expression.New(constructorInfo1, (IEnumerable<Expression>)expressionArray2, memberInfoArray1), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x1c2);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Image").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0x1c2);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedPost, Mix.Cms.Lib.Models.Cms.MixPost> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPost>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, Mix.Cms.Lib.Models.Cms.MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_MixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixRelatedPost> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, IEnumerable<Mix.Cms.Lib.Models.Cms.MixRelatedPost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_MixRelatedPostMixPost").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "d");
				ConstructorInfo methodFromHandle2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType37<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cDestinationIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType37<int, string>).TypeHandle);
				Expression[] expressionArray3 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_DestinationId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray2 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType37<int, string>).GetMethod("get_DestinationId").MethodHandle, typeof(u003cu003ef__AnonymousType37<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType37<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType37<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixRelatedPost>(referenceCollectionBuilder.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, object>>(Expression.New(methodFromHandle2, (IEnumerable<Expression>)expressionArray3, memberInfoArray2), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_mix_related_post_mix_post1");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixRelatedPost, Mix.Cms.Lib.Models.Cms.MixPost> referenceNavigationBuilder1 = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixPost>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, Mix.Cms.Lib.Models.Cms.MixPost>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_S").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixPost), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixRelatedPost> referenceCollectionBuilder1 = referenceNavigationBuilder1.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixPost, IEnumerable<Mix.Cms.Lib.Models.Cms.MixRelatedPost>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixPost).GetMethod("get_MixRelatedPostS").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost), "d");
				ConstructorInfo constructorInfo2 = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType38<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cSourceIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType38<int, string>).TypeHandle);
				Expression[] expressionArray4 = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_SourceId").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixRelatedPost).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] methodFromHandle3 = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType38<int, string>).GetMethod("get_SourceId").MethodHandle, typeof(u003cu003ef__AnonymousType38<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType38<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType38<int, string>).TypeHandle) };
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixPost, Mix.Cms.Lib.Models.Cms.MixRelatedPost>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixRelatedPost, object>>(Expression.New(constructorInfo2, (IEnumerable<Expression>)expressionArray4, methodFromHandle3), new ParameterExpression[] { parameterExpression })).OnDelete(0), "FK_mix_related_post_mix_post");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixTemplate>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixTemplate> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixTemplate>(entity, "mix_template");
				RelationalIndexBuilderExtensions.HasName<Mix.Cms.Lib.Models.Cms.MixTemplate>(entity.HasIndex((Mix.Cms.Lib.Models.Cms.MixTemplate e) => (object)e.ThemeId), "IX_mix_template_file_TemplateId");
				entity.Property<int>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.Id).ValueGeneratedNever();
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.Content).IsRequired(true), "text");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.Extension).IsRequired(true).HasMaxLength(50);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.FileFolder).IsRequired(true).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.FileName).IsRequired(true).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.FolderType).IsRequired(true).HasMaxLength(50);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.LastModified), "datetime");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.MobileContent), "text");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.Scripts), "text");
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.SpaContent), "text");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<string>(entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.Styles), "text");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTemplate e) => e.ThemeName).IsRequired(true).HasMaxLength(250);
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixTheme, Mix.Cms.Lib.Models.Cms.MixTemplate>(entity.HasOne<Mix.Cms.Lib.Models.Cms.MixTheme>((Mix.Cms.Lib.Models.Cms.MixTemplate d) => d.Theme).WithMany((Mix.Cms.Lib.Models.Cms.MixTheme p) => p.MixTemplate).HasForeignKey((Mix.Cms.Lib.Models.Cms.MixTemplate d) => (object)d.ThemeId).OnDelete(0), "FK_mix_template_mix_theme");
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixTheme>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixTheme> entity) => {
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixTheme>(entity, "mix_theme");
				entity.Property<int>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.Id).ValueGeneratedNever();
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.CreatedBy).HasMaxLength(50).IsUnicode(false);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.CreatedDateTime), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.Image).HasMaxLength(250);
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.LastModified), "datetime");
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.ModifiedBy).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.Name).IsRequired(true).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.PreviewUrl).HasMaxLength(0x1c2);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.Status).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.Thumbnail).HasMaxLength(250);
				entity.Property<string>((Mix.Cms.Lib.Models.Cms.MixTheme e) => e.Title).HasMaxLength(250);
			});
			modelBuilder.Entity<Mix.Cms.Lib.Models.Cms.MixUrlAlias>((EntityTypeBuilder<Mix.Cms.Lib.Models.Cms.MixUrlAlias> entity) => {
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				ConstructorInfo methodFromHandle = (ConstructorInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod(".ctor", new Type[] { typeof(u003cIdu003ej__TPar), typeof(u003cSpecificultureu003ej__TPar) }).MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle);
				Expression[] expressionArray = new Expression[] { Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_Id").MethodHandle)), Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)) };
				MemberInfo[] memberInfoArray = new MemberInfo[] { (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Id").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(u003cu003ef__AnonymousType6<int, string>).GetMethod("get_Specificulture").MethodHandle, typeof(u003cu003ef__AnonymousType6<int, string>).TypeHandle) };
				entity.HasKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, object>>(Expression.New(methodFromHandle, (IEnumerable<Expression>)expressionArray, memberInfoArray), new ParameterExpression[] { parameterExpression }));
				RelationalEntityTypeBuilderExtensions.ToTable<Mix.Cms.Lib.Models.Cms.MixUrlAlias>(entity, "mix_url_alias");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				entity.HasIndex(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(10);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_Alias").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_CreatedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime>(entity.Property<DateTime>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, DateTime>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_CreatedDateTime").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_Description").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(0xfa0);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				RelationalPropertyBuilderExtensions.HasColumnType<DateTime?>(entity.Property<DateTime?>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, DateTime?>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_LastModified").MethodHandle)), new ParameterExpression[] { parameterExpression })), "datetime");
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_ModifiedBy").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_SourceId").MethodHandle)), new ParameterExpression[] { parameterExpression })).HasMaxLength(250);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "e");
				entity.Property<string>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, string>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_Status").MethodHandle)), new ParameterExpression[] { parameterExpression })).IsRequired(true).HasMaxLength(50).IsUnicode(false);
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "d");
				ReferenceNavigationBuilder<Mix.Cms.Lib.Models.Cms.MixUrlAlias, Mix.Cms.Lib.Models.Cms.MixCulture> referenceNavigationBuilder = entity.HasOne<Mix.Cms.Lib.Models.Cms.MixCulture>(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, Mix.Cms.Lib.Models.Cms.MixCulture>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_SpecificultureNavigation").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixUrlAlias> referenceCollectionBuilder = referenceNavigationBuilder.WithMany(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, IEnumerable<Mix.Cms.Lib.Models.Cms.MixUrlAlias>>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_MixUrlAlias").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixCulture), "p");
				ReferenceCollectionBuilder<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixUrlAlias> referenceCollectionBuilder1 = referenceCollectionBuilder.HasPrincipalKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixCulture, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixCulture).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression }));
				parameterExpression = Expression.Parameter(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias), "d");
				RelationalForeignKeyBuilderExtensions.HasConstraintName<Mix.Cms.Lib.Models.Cms.MixCulture, Mix.Cms.Lib.Models.Cms.MixUrlAlias>(referenceCollectionBuilder1.HasForeignKey(Expression.Lambda<Func<Mix.Cms.Lib.Models.Cms.MixUrlAlias, object>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Mix.Cms.Lib.Models.Cms.MixUrlAlias).GetMethod("get_Specificulture").MethodHandle)), new ParameterExpression[] { parameterExpression })), "FK_Mix_Url_Alias_Mix_Culture");
			});
		}
	}
}