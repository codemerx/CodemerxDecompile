using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Mix.Cms.Lib.Models.Cms;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Migrations
{
	[DbContext(typeof(MixCmsContext))]
	[Migration("20200516033802_Init")]
	public class Init : Migration
	{
		public Init()
		{
			base();
			return;
		}

		protected override void BuildTargetModel(ModelBuilder modelBuilder)
		{
			dummyVar0 = modelBuilder.HasAnnotation("ProductVersion", "3.1.3").HasAnnotation("Relational:MaxIdentifierLength", 128).HasAnnotation("SqlServer:ValueGenerationStrategy", (SqlServerValueGenerationStrategy)2);
			stackVariable12 = modelBuilder;
			stackVariable14 = Init.u003cu003ec.u003cu003e9__2_0;
			if (stackVariable14 == null)
			{
				dummyVar1 = stackVariable14;
				stackVariable14 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_0);
				Init.u003cu003ec.u003cu003e9__2_0 = stackVariable14;
			}
			dummyVar2 = stackVariable12.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeField", stackVariable14);
			stackVariable16 = modelBuilder;
			stackVariable18 = Init.u003cu003ec.u003cu003e9__2_1;
			if (stackVariable18 == null)
			{
				dummyVar3 = stackVariable18;
				stackVariable18 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_1);
				Init.u003cu003ec.u003cu003e9__2_1 = stackVariable18;
			}
			dummyVar4 = stackVariable16.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSet", stackVariable18);
			stackVariable20 = modelBuilder;
			stackVariable22 = Init.u003cu003ec.u003cu003e9__2_2;
			if (stackVariable22 == null)
			{
				dummyVar5 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_2);
				Init.u003cu003ec.u003cu003e9__2_2 = stackVariable22;
			}
			dummyVar6 = stackVariable20.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetData", stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable26 = Init.u003cu003ec.u003cu003e9__2_3;
			if (stackVariable26 == null)
			{
				dummyVar7 = stackVariable26;
				stackVariable26 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_3);
				Init.u003cu003ec.u003cu003e9__2_3 = stackVariable26;
			}
			dummyVar8 = stackVariable24.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetReference", stackVariable26);
			stackVariable28 = modelBuilder;
			stackVariable30 = Init.u003cu003ec.u003cu003e9__2_4;
			if (stackVariable30 == null)
			{
				dummyVar9 = stackVariable30;
				stackVariable30 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_4);
				Init.u003cu003ec.u003cu003e9__2_4 = stackVariable30;
			}
			dummyVar10 = stackVariable28.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetValue", stackVariable30);
			stackVariable32 = modelBuilder;
			stackVariable34 = Init.u003cu003ec.u003cu003e9__2_5;
			if (stackVariable34 == null)
			{
				dummyVar11 = stackVariable34;
				stackVariable34 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_5);
				Init.u003cu003ec.u003cu003e9__2_5 = stackVariable34;
			}
			dummyVar12 = stackVariable32.Entity("Mix.Cms.Lib.Models.Cms.MixCache", stackVariable34);
			stackVariable36 = modelBuilder;
			stackVariable38 = Init.u003cu003ec.u003cu003e9__2_6;
			if (stackVariable38 == null)
			{
				dummyVar13 = stackVariable38;
				stackVariable38 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_6);
				Init.u003cu003ec.u003cu003e9__2_6 = stackVariable38;
			}
			dummyVar14 = stackVariable36.Entity("Mix.Cms.Lib.Models.Cms.MixCmsUser", stackVariable38);
			stackVariable40 = modelBuilder;
			stackVariable42 = Init.u003cu003ec.u003cu003e9__2_7;
			if (stackVariable42 == null)
			{
				dummyVar15 = stackVariable42;
				stackVariable42 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_7);
				Init.u003cu003ec.u003cu003e9__2_7 = stackVariable42;
			}
			dummyVar16 = stackVariable40.Entity("Mix.Cms.Lib.Models.Cms.MixConfiguration", stackVariable42);
			stackVariable44 = modelBuilder;
			stackVariable46 = Init.u003cu003ec.u003cu003e9__2_8;
			if (stackVariable46 == null)
			{
				dummyVar17 = stackVariable46;
				stackVariable46 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_8);
				Init.u003cu003ec.u003cu003e9__2_8 = stackVariable46;
			}
			dummyVar18 = stackVariable44.Entity("Mix.Cms.Lib.Models.Cms.MixCulture", stackVariable46);
			stackVariable48 = modelBuilder;
			stackVariable50 = Init.u003cu003ec.u003cu003e9__2_9;
			if (stackVariable50 == null)
			{
				dummyVar19 = stackVariable50;
				stackVariable50 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_9);
				Init.u003cu003ec.u003cu003e9__2_9 = stackVariable50;
			}
			dummyVar20 = stackVariable48.Entity("Mix.Cms.Lib.Models.Cms.MixFile", stackVariable50);
			stackVariable52 = modelBuilder;
			stackVariable54 = Init.u003cu003ec.u003cu003e9__2_10;
			if (stackVariable54 == null)
			{
				dummyVar21 = stackVariable54;
				stackVariable54 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_10);
				Init.u003cu003ec.u003cu003e9__2_10 = stackVariable54;
			}
			dummyVar22 = stackVariable52.Entity("Mix.Cms.Lib.Models.Cms.MixLanguage", stackVariable54);
			stackVariable56 = modelBuilder;
			stackVariable58 = Init.u003cu003ec.u003cu003e9__2_11;
			if (stackVariable58 == null)
			{
				dummyVar23 = stackVariable58;
				stackVariable58 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_11);
				Init.u003cu003ec.u003cu003e9__2_11 = stackVariable58;
			}
			dummyVar24 = stackVariable56.Entity("Mix.Cms.Lib.Models.Cms.MixMedia", stackVariable58);
			stackVariable60 = modelBuilder;
			stackVariable62 = Init.u003cu003ec.u003cu003e9__2_12;
			if (stackVariable62 == null)
			{
				dummyVar25 = stackVariable62;
				stackVariable62 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_12);
				Init.u003cu003ec.u003cu003e9__2_12 = stackVariable62;
			}
			dummyVar26 = stackVariable60.Entity("Mix.Cms.Lib.Models.Cms.MixModule", stackVariable62);
			stackVariable64 = modelBuilder;
			stackVariable66 = Init.u003cu003ec.u003cu003e9__2_13;
			if (stackVariable66 == null)
			{
				dummyVar27 = stackVariable66;
				stackVariable66 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_13);
				Init.u003cu003ec.u003cu003e9__2_13 = stackVariable66;
			}
			dummyVar28 = stackVariable64.Entity("Mix.Cms.Lib.Models.Cms.MixModuleData", stackVariable66);
			stackVariable68 = modelBuilder;
			stackVariable70 = Init.u003cu003ec.u003cu003e9__2_14;
			if (stackVariable70 == null)
			{
				dummyVar29 = stackVariable70;
				stackVariable70 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_14);
				Init.u003cu003ec.u003cu003e9__2_14 = stackVariable70;
			}
			dummyVar30 = stackVariable68.Entity("Mix.Cms.Lib.Models.Cms.MixModulePost", stackVariable70);
			stackVariable72 = modelBuilder;
			stackVariable74 = Init.u003cu003ec.u003cu003e9__2_15;
			if (stackVariable74 == null)
			{
				dummyVar31 = stackVariable74;
				stackVariable74 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_15);
				Init.u003cu003ec.u003cu003e9__2_15 = stackVariable74;
			}
			dummyVar32 = stackVariable72.Entity("Mix.Cms.Lib.Models.Cms.MixPage", stackVariable74);
			stackVariable76 = modelBuilder;
			stackVariable78 = Init.u003cu003ec.u003cu003e9__2_16;
			if (stackVariable78 == null)
			{
				dummyVar33 = stackVariable78;
				stackVariable78 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_16);
				Init.u003cu003ec.u003cu003e9__2_16 = stackVariable78;
			}
			dummyVar34 = stackVariable76.Entity("Mix.Cms.Lib.Models.Cms.MixPageModule", stackVariable78);
			stackVariable80 = modelBuilder;
			stackVariable82 = Init.u003cu003ec.u003cu003e9__2_17;
			if (stackVariable82 == null)
			{
				dummyVar35 = stackVariable82;
				stackVariable82 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_17);
				Init.u003cu003ec.u003cu003e9__2_17 = stackVariable82;
			}
			dummyVar36 = stackVariable80.Entity("Mix.Cms.Lib.Models.Cms.MixPagePost", stackVariable82);
			stackVariable84 = modelBuilder;
			stackVariable86 = Init.u003cu003ec.u003cu003e9__2_18;
			if (stackVariable86 == null)
			{
				dummyVar37 = stackVariable86;
				stackVariable86 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_18);
				Init.u003cu003ec.u003cu003e9__2_18 = stackVariable86;
			}
			dummyVar38 = stackVariable84.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPage", stackVariable86);
			stackVariable88 = modelBuilder;
			stackVariable90 = Init.u003cu003ec.u003cu003e9__2_19;
			if (stackVariable90 == null)
			{
				dummyVar39 = stackVariable90;
				stackVariable90 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_19);
				Init.u003cu003ec.u003cu003e9__2_19 = stackVariable90;
			}
			dummyVar40 = stackVariable88.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation", stackVariable90);
			stackVariable92 = modelBuilder;
			stackVariable94 = Init.u003cu003ec.u003cu003e9__2_20;
			if (stackVariable94 == null)
			{
				dummyVar41 = stackVariable94;
				stackVariable94 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_20);
				Init.u003cu003ec.u003cu003e9__2_20 = stackVariable94;
			}
			dummyVar42 = stackVariable92.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPageRole", stackVariable94);
			stackVariable96 = modelBuilder;
			stackVariable98 = Init.u003cu003ec.u003cu003e9__2_21;
			if (stackVariable98 == null)
			{
				dummyVar43 = stackVariable98;
				stackVariable98 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_21);
				Init.u003cu003ec.u003cu003e9__2_21 = stackVariable98;
			}
			dummyVar44 = stackVariable96.Entity("Mix.Cms.Lib.Models.Cms.MixPost", stackVariable98);
			stackVariable100 = modelBuilder;
			stackVariable102 = Init.u003cu003ec.u003cu003e9__2_22;
			if (stackVariable102 == null)
			{
				dummyVar45 = stackVariable102;
				stackVariable102 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_22);
				Init.u003cu003ec.u003cu003e9__2_22 = stackVariable102;
			}
			dummyVar46 = stackVariable100.Entity("Mix.Cms.Lib.Models.Cms.MixPostMedia", stackVariable102);
			stackVariable104 = modelBuilder;
			stackVariable106 = Init.u003cu003ec.u003cu003e9__2_23;
			if (stackVariable106 == null)
			{
				dummyVar47 = stackVariable106;
				stackVariable106 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_23);
				Init.u003cu003ec.u003cu003e9__2_23 = stackVariable106;
			}
			dummyVar48 = stackVariable104.Entity("Mix.Cms.Lib.Models.Cms.MixPostModule", stackVariable106);
			stackVariable108 = modelBuilder;
			stackVariable110 = Init.u003cu003ec.u003cu003e9__2_24;
			if (stackVariable110 == null)
			{
				dummyVar49 = stackVariable110;
				stackVariable110 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_24);
				Init.u003cu003ec.u003cu003e9__2_24 = stackVariable110;
			}
			dummyVar50 = stackVariable108.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData", stackVariable110);
			stackVariable112 = modelBuilder;
			stackVariable114 = Init.u003cu003ec.u003cu003e9__2_25;
			if (stackVariable114 == null)
			{
				dummyVar51 = stackVariable114;
				stackVariable114 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_25);
				Init.u003cu003ec.u003cu003e9__2_25 = stackVariable114;
			}
			dummyVar52 = stackVariable112.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet", stackVariable114);
			stackVariable116 = modelBuilder;
			stackVariable118 = Init.u003cu003ec.u003cu003e9__2_26;
			if (stackVariable118 == null)
			{
				dummyVar53 = stackVariable118;
				stackVariable118 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_26);
				Init.u003cu003ec.u003cu003e9__2_26 = stackVariable118;
			}
			dummyVar54 = stackVariable116.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedData", stackVariable118);
			stackVariable120 = modelBuilder;
			stackVariable122 = Init.u003cu003ec.u003cu003e9__2_27;
			if (stackVariable122 == null)
			{
				dummyVar55 = stackVariable122;
				stackVariable122 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_27);
				Init.u003cu003ec.u003cu003e9__2_27 = stackVariable122;
			}
			dummyVar56 = stackVariable120.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedPost", stackVariable122);
			stackVariable124 = modelBuilder;
			stackVariable126 = Init.u003cu003ec.u003cu003e9__2_28;
			if (stackVariable126 == null)
			{
				dummyVar57 = stackVariable126;
				stackVariable126 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_28);
				Init.u003cu003ec.u003cu003e9__2_28 = stackVariable126;
			}
			dummyVar58 = stackVariable124.Entity("Mix.Cms.Lib.Models.Cms.MixTemplate", stackVariable126);
			stackVariable128 = modelBuilder;
			stackVariable130 = Init.u003cu003ec.u003cu003e9__2_29;
			if (stackVariable130 == null)
			{
				dummyVar59 = stackVariable130;
				stackVariable130 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_29);
				Init.u003cu003ec.u003cu003e9__2_29 = stackVariable130;
			}
			dummyVar60 = stackVariable128.Entity("Mix.Cms.Lib.Models.Cms.MixTheme", stackVariable130);
			stackVariable132 = modelBuilder;
			stackVariable134 = Init.u003cu003ec.u003cu003e9__2_30;
			if (stackVariable134 == null)
			{
				dummyVar61 = stackVariable134;
				stackVariable134 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_30);
				Init.u003cu003ec.u003cu003e9__2_30 = stackVariable134;
			}
			dummyVar62 = stackVariable132.Entity("Mix.Cms.Lib.Models.Cms.MixUrlAlias", stackVariable134);
			stackVariable136 = modelBuilder;
			stackVariable138 = Init.u003cu003ec.u003cu003e9__2_31;
			if (stackVariable138 == null)
			{
				dummyVar63 = stackVariable138;
				stackVariable138 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_31);
				Init.u003cu003ec.u003cu003e9__2_31 = stackVariable138;
			}
			dummyVar64 = stackVariable136.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeField", stackVariable138);
			stackVariable140 = modelBuilder;
			stackVariable142 = Init.u003cu003ec.u003cu003e9__2_32;
			if (stackVariable142 == null)
			{
				dummyVar65 = stackVariable142;
				stackVariable142 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_32);
				Init.u003cu003ec.u003cu003e9__2_32 = stackVariable142;
			}
			dummyVar66 = stackVariable140.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetData", stackVariable142);
			stackVariable144 = modelBuilder;
			stackVariable146 = Init.u003cu003ec.u003cu003e9__2_33;
			if (stackVariable146 == null)
			{
				dummyVar67 = stackVariable146;
				stackVariable146 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_33);
				Init.u003cu003ec.u003cu003e9__2_33 = stackVariable146;
			}
			dummyVar68 = stackVariable144.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetReference", stackVariable146);
			stackVariable148 = modelBuilder;
			stackVariable150 = Init.u003cu003ec.u003cu003e9__2_34;
			if (stackVariable150 == null)
			{
				dummyVar69 = stackVariable150;
				stackVariable150 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_34);
				Init.u003cu003ec.u003cu003e9__2_34 = stackVariable150;
			}
			dummyVar70 = stackVariable148.Entity("Mix.Cms.Lib.Models.Cms.MixConfiguration", stackVariable150);
			stackVariable152 = modelBuilder;
			stackVariable154 = Init.u003cu003ec.u003cu003e9__2_35;
			if (stackVariable154 == null)
			{
				dummyVar71 = stackVariable154;
				stackVariable154 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_35);
				Init.u003cu003ec.u003cu003e9__2_35 = stackVariable154;
			}
			dummyVar72 = stackVariable152.Entity("Mix.Cms.Lib.Models.Cms.MixFile", stackVariable154);
			stackVariable156 = modelBuilder;
			stackVariable158 = Init.u003cu003ec.u003cu003e9__2_36;
			if (stackVariable158 == null)
			{
				dummyVar73 = stackVariable158;
				stackVariable158 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_36);
				Init.u003cu003ec.u003cu003e9__2_36 = stackVariable158;
			}
			dummyVar74 = stackVariable156.Entity("Mix.Cms.Lib.Models.Cms.MixLanguage", stackVariable158);
			stackVariable160 = modelBuilder;
			stackVariable162 = Init.u003cu003ec.u003cu003e9__2_37;
			if (stackVariable162 == null)
			{
				dummyVar75 = stackVariable162;
				stackVariable162 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_37);
				Init.u003cu003ec.u003cu003e9__2_37 = stackVariable162;
			}
			dummyVar76 = stackVariable160.Entity("Mix.Cms.Lib.Models.Cms.MixModule", stackVariable162);
			stackVariable164 = modelBuilder;
			stackVariable166 = Init.u003cu003ec.u003cu003e9__2_38;
			if (stackVariable166 == null)
			{
				dummyVar77 = stackVariable166;
				stackVariable166 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_38);
				Init.u003cu003ec.u003cu003e9__2_38 = stackVariable166;
			}
			dummyVar78 = stackVariable164.Entity("Mix.Cms.Lib.Models.Cms.MixModuleData", stackVariable166);
			stackVariable168 = modelBuilder;
			stackVariable170 = Init.u003cu003ec.u003cu003e9__2_39;
			if (stackVariable170 == null)
			{
				dummyVar79 = stackVariable170;
				stackVariable170 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_39);
				Init.u003cu003ec.u003cu003e9__2_39 = stackVariable170;
			}
			dummyVar80 = stackVariable168.Entity("Mix.Cms.Lib.Models.Cms.MixModulePost", stackVariable170);
			stackVariable172 = modelBuilder;
			stackVariable174 = Init.u003cu003ec.u003cu003e9__2_40;
			if (stackVariable174 == null)
			{
				dummyVar81 = stackVariable174;
				stackVariable174 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_40);
				Init.u003cu003ec.u003cu003e9__2_40 = stackVariable174;
			}
			dummyVar82 = stackVariable172.Entity("Mix.Cms.Lib.Models.Cms.MixPage", stackVariable174);
			stackVariable176 = modelBuilder;
			stackVariable178 = Init.u003cu003ec.u003cu003e9__2_41;
			if (stackVariable178 == null)
			{
				dummyVar83 = stackVariable178;
				stackVariable178 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_41);
				Init.u003cu003ec.u003cu003e9__2_41 = stackVariable178;
			}
			dummyVar84 = stackVariable176.Entity("Mix.Cms.Lib.Models.Cms.MixPageModule", stackVariable178);
			stackVariable180 = modelBuilder;
			stackVariable182 = Init.u003cu003ec.u003cu003e9__2_42;
			if (stackVariable182 == null)
			{
				dummyVar85 = stackVariable182;
				stackVariable182 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_42);
				Init.u003cu003ec.u003cu003e9__2_42 = stackVariable182;
			}
			dummyVar86 = stackVariable180.Entity("Mix.Cms.Lib.Models.Cms.MixPagePost", stackVariable182);
			stackVariable184 = modelBuilder;
			stackVariable186 = Init.u003cu003ec.u003cu003e9__2_43;
			if (stackVariable186 == null)
			{
				dummyVar87 = stackVariable186;
				stackVariable186 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_43);
				Init.u003cu003ec.u003cu003e9__2_43 = stackVariable186;
			}
			dummyVar88 = stackVariable184.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation", stackVariable186);
			stackVariable188 = modelBuilder;
			stackVariable190 = Init.u003cu003ec.u003cu003e9__2_44;
			if (stackVariable190 == null)
			{
				dummyVar89 = stackVariable190;
				stackVariable190 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_44);
				Init.u003cu003ec.u003cu003e9__2_44 = stackVariable190;
			}
			dummyVar90 = stackVariable188.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPageRole", stackVariable190);
			stackVariable192 = modelBuilder;
			stackVariable194 = Init.u003cu003ec.u003cu003e9__2_45;
			if (stackVariable194 == null)
			{
				dummyVar91 = stackVariable194;
				stackVariable194 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_45);
				Init.u003cu003ec.u003cu003e9__2_45 = stackVariable194;
			}
			dummyVar92 = stackVariable192.Entity("Mix.Cms.Lib.Models.Cms.MixPost", stackVariable194);
			stackVariable196 = modelBuilder;
			stackVariable198 = Init.u003cu003ec.u003cu003e9__2_46;
			if (stackVariable198 == null)
			{
				dummyVar93 = stackVariable198;
				stackVariable198 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_46);
				Init.u003cu003ec.u003cu003e9__2_46 = stackVariable198;
			}
			dummyVar94 = stackVariable196.Entity("Mix.Cms.Lib.Models.Cms.MixPostMedia", stackVariable198);
			stackVariable200 = modelBuilder;
			stackVariable202 = Init.u003cu003ec.u003cu003e9__2_47;
			if (stackVariable202 == null)
			{
				dummyVar95 = stackVariable202;
				stackVariable202 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_47);
				Init.u003cu003ec.u003cu003e9__2_47 = stackVariable202;
			}
			dummyVar96 = stackVariable200.Entity("Mix.Cms.Lib.Models.Cms.MixPostModule", stackVariable202);
			stackVariable204 = modelBuilder;
			stackVariable206 = Init.u003cu003ec.u003cu003e9__2_48;
			if (stackVariable206 == null)
			{
				dummyVar97 = stackVariable206;
				stackVariable206 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_48);
				Init.u003cu003ec.u003cu003e9__2_48 = stackVariable206;
			}
			dummyVar98 = stackVariable204.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet", stackVariable206);
			stackVariable208 = modelBuilder;
			stackVariable210 = Init.u003cu003ec.u003cu003e9__2_49;
			if (stackVariable210 == null)
			{
				dummyVar99 = stackVariable210;
				stackVariable210 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_49);
				Init.u003cu003ec.u003cu003e9__2_49 = stackVariable210;
			}
			dummyVar100 = stackVariable208.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedPost", stackVariable210);
			stackVariable212 = modelBuilder;
			stackVariable214 = Init.u003cu003ec.u003cu003e9__2_50;
			if (stackVariable214 == null)
			{
				dummyVar101 = stackVariable214;
				stackVariable214 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_50);
				Init.u003cu003ec.u003cu003e9__2_50 = stackVariable214;
			}
			dummyVar102 = stackVariable212.Entity("Mix.Cms.Lib.Models.Cms.MixTemplate", stackVariable214);
			stackVariable216 = modelBuilder;
			stackVariable218 = Init.u003cu003ec.u003cu003e9__2_51;
			if (stackVariable218 == null)
			{
				dummyVar103 = stackVariable218;
				stackVariable218 = new Action<EntityTypeBuilder>(Init.u003cu003ec.u003cu003e9.u003cBuildTargetModelu003eb__2_51);
				Init.u003cu003ec.u003cu003e9__2_51 = stackVariable218;
			}
			dummyVar104 = stackVariable216.Entity("Mix.Cms.Lib.Models.Cms.MixUrlAlias", stackVariable218);
			return;
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			dummyVar0 = migrationBuilder.DropTable("mix_attribute_field", null);
			dummyVar1 = migrationBuilder.DropTable("mix_attribute_set_data", null);
			dummyVar2 = migrationBuilder.DropTable("mix_attribute_set_reference", null);
			dummyVar3 = migrationBuilder.DropTable("mix_attribute_set_value", null);
			dummyVar4 = migrationBuilder.DropTable("mix_cache", null);
			dummyVar5 = migrationBuilder.DropTable("mix_cms_user", null);
			dummyVar6 = migrationBuilder.DropTable("mix_configuration", null);
			dummyVar7 = migrationBuilder.DropTable("mix_file", null);
			dummyVar8 = migrationBuilder.DropTable("mix_language", null);
			dummyVar9 = migrationBuilder.DropTable("mix_module_data", null);
			dummyVar10 = migrationBuilder.DropTable("mix_module_post", null);
			dummyVar11 = migrationBuilder.DropTable("mix_page_module", null);
			dummyVar12 = migrationBuilder.DropTable("mix_page_post", null);
			dummyVar13 = migrationBuilder.DropTable("mix_portal_page_navigation", null);
			dummyVar14 = migrationBuilder.DropTable("mix_portal_page_role", null);
			dummyVar15 = migrationBuilder.DropTable("mix_post_media", null);
			dummyVar16 = migrationBuilder.DropTable("mix_post_module", null);
			dummyVar17 = migrationBuilder.DropTable("mix_related_attribute_data", null);
			dummyVar18 = migrationBuilder.DropTable("mix_related_attribute_set", null);
			dummyVar19 = migrationBuilder.DropTable("mix_related_data", null);
			dummyVar20 = migrationBuilder.DropTable("mix_related_post", null);
			dummyVar21 = migrationBuilder.DropTable("mix_template", null);
			dummyVar22 = migrationBuilder.DropTable("mix_url_alias", null);
			dummyVar23 = migrationBuilder.DropTable("mix_page", null);
			dummyVar24 = migrationBuilder.DropTable("mix_portal_page", null);
			dummyVar25 = migrationBuilder.DropTable("mix_media", null);
			dummyVar26 = migrationBuilder.DropTable("mix_module", null);
			dummyVar27 = migrationBuilder.DropTable("mix_attribute_set", null);
			dummyVar28 = migrationBuilder.DropTable("mix_post", null);
			dummyVar29 = migrationBuilder.DropTable("mix_theme", null);
			dummyVar30 = migrationBuilder.DropTable("mix_culture", null);
			return;
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			stackVariable0 = migrationBuilder;
			stackVariable2 = Init.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType0<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_0);
				Init.u003cu003ec.u003cu003e9__0_0 = stackVariable2;
			}
			stackVariable4 = Init.u003cu003ec.u003cu003e9__0_1;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType0<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_1);
				Init.u003cu003ec.u003cu003e9__0_1 = stackVariable4;
			}
			dummyVar2 = stackVariable0.CreateTable("mix_attribute_set", stackVariable2, null, stackVariable4, null);
			stackVariable7 = migrationBuilder;
			stackVariable9 = Init.u003cu003ec.u003cu003e9__0_2;
			if (stackVariable9 == null)
			{
				dummyVar3 = stackVariable9;
				stackVariable9 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType1<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_2);
				Init.u003cu003ec.u003cu003e9__0_2 = stackVariable9;
			}
			stackVariable11 = Init.u003cu003ec.u003cu003e9__0_3;
			if (stackVariable11 == null)
			{
				dummyVar4 = stackVariable11;
				stackVariable11 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType1<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_3);
				Init.u003cu003ec.u003cu003e9__0_3 = stackVariable11;
			}
			dummyVar5 = stackVariable7.CreateTable("mix_attribute_set_value", stackVariable9, null, stackVariable11, null);
			stackVariable14 = migrationBuilder;
			stackVariable16 = Init.u003cu003ec.u003cu003e9__0_4;
			if (stackVariable16 == null)
			{
				dummyVar6 = stackVariable16;
				stackVariable16 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType2<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_4);
				Init.u003cu003ec.u003cu003e9__0_4 = stackVariable16;
			}
			stackVariable18 = Init.u003cu003ec.u003cu003e9__0_5;
			if (stackVariable18 == null)
			{
				dummyVar7 = stackVariable18;
				stackVariable18 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType2<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_5);
				Init.u003cu003ec.u003cu003e9__0_5 = stackVariable18;
			}
			dummyVar8 = stackVariable14.CreateTable("mix_cache", stackVariable16, null, stackVariable18, null);
			stackVariable21 = migrationBuilder;
			stackVariable23 = Init.u003cu003ec.u003cu003e9__0_6;
			if (stackVariable23 == null)
			{
				dummyVar9 = stackVariable23;
				stackVariable23 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType3<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_6);
				Init.u003cu003ec.u003cu003e9__0_6 = stackVariable23;
			}
			stackVariable25 = Init.u003cu003ec.u003cu003e9__0_7;
			if (stackVariable25 == null)
			{
				dummyVar10 = stackVariable25;
				stackVariable25 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType3<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_7);
				Init.u003cu003ec.u003cu003e9__0_7 = stackVariable25;
			}
			dummyVar11 = stackVariable21.CreateTable("mix_cms_user", stackVariable23, null, stackVariable25, null);
			stackVariable28 = migrationBuilder;
			stackVariable30 = Init.u003cu003ec.u003cu003e9__0_8;
			if (stackVariable30 == null)
			{
				dummyVar12 = stackVariable30;
				stackVariable30 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType4<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_8);
				Init.u003cu003ec.u003cu003e9__0_8 = stackVariable30;
			}
			stackVariable32 = Init.u003cu003ec.u003cu003e9__0_9;
			if (stackVariable32 == null)
			{
				dummyVar13 = stackVariable32;
				stackVariable32 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType4<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_9);
				Init.u003cu003ec.u003cu003e9__0_9 = stackVariable32;
			}
			dummyVar14 = stackVariable28.CreateTable("mix_culture", stackVariable30, null, stackVariable32, null);
			stackVariable35 = migrationBuilder;
			stackVariable37 = Init.u003cu003ec.u003cu003e9__0_10;
			if (stackVariable37 == null)
			{
				dummyVar15 = stackVariable37;
				stackVariable37 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType5<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_10);
				Init.u003cu003ec.u003cu003e9__0_10 = stackVariable37;
			}
			stackVariable39 = Init.u003cu003ec.u003cu003e9__0_11;
			if (stackVariable39 == null)
			{
				dummyVar16 = stackVariable39;
				stackVariable39 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType5<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_11);
				Init.u003cu003ec.u003cu003e9__0_11 = stackVariable39;
			}
			dummyVar17 = stackVariable35.CreateTable("mix_media", stackVariable37, null, stackVariable39, null);
			stackVariable42 = migrationBuilder;
			stackVariable44 = Init.u003cu003ec.u003cu003e9__0_12;
			if (stackVariable44 == null)
			{
				dummyVar18 = stackVariable44;
				stackVariable44 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType7<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_12);
				Init.u003cu003ec.u003cu003e9__0_12 = stackVariable44;
			}
			stackVariable46 = Init.u003cu003ec.u003cu003e9__0_13;
			if (stackVariable46 == null)
			{
				dummyVar19 = stackVariable46;
				stackVariable46 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType7<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_13);
				Init.u003cu003ec.u003cu003e9__0_13 = stackVariable46;
			}
			dummyVar20 = stackVariable42.CreateTable("mix_portal_page", stackVariable44, null, stackVariable46, null);
			stackVariable49 = migrationBuilder;
			stackVariable51 = Init.u003cu003ec.u003cu003e9__0_14;
			if (stackVariable51 == null)
			{
				dummyVar21 = stackVariable51;
				stackVariable51 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType8<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_14);
				Init.u003cu003ec.u003cu003e9__0_14 = stackVariable51;
			}
			stackVariable53 = Init.u003cu003ec.u003cu003e9__0_15;
			if (stackVariable53 == null)
			{
				dummyVar22 = stackVariable53;
				stackVariable53 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType8<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_15);
				Init.u003cu003ec.u003cu003e9__0_15 = stackVariable53;
			}
			dummyVar23 = stackVariable49.CreateTable("mix_related_attribute_data", stackVariable51, null, stackVariable53, null);
			stackVariable56 = migrationBuilder;
			stackVariable58 = Init.u003cu003ec.u003cu003e9__0_16;
			if (stackVariable58 == null)
			{
				dummyVar24 = stackVariable58;
				stackVariable58 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType9<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_16);
				Init.u003cu003ec.u003cu003e9__0_16 = stackVariable58;
			}
			stackVariable60 = Init.u003cu003ec.u003cu003e9__0_17;
			if (stackVariable60 == null)
			{
				dummyVar25 = stackVariable60;
				stackVariable60 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType9<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_17);
				Init.u003cu003ec.u003cu003e9__0_17 = stackVariable60;
			}
			dummyVar26 = stackVariable56.CreateTable("mix_related_data", stackVariable58, null, stackVariable60, null);
			stackVariable63 = migrationBuilder;
			stackVariable65 = Init.u003cu003ec.u003cu003e9__0_18;
			if (stackVariable65 == null)
			{
				dummyVar27 = stackVariable65;
				stackVariable65 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType10<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_18);
				Init.u003cu003ec.u003cu003e9__0_18 = stackVariable65;
			}
			stackVariable67 = Init.u003cu003ec.u003cu003e9__0_19;
			if (stackVariable67 == null)
			{
				dummyVar28 = stackVariable67;
				stackVariable67 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType10<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_19);
				Init.u003cu003ec.u003cu003e9__0_19 = stackVariable67;
			}
			dummyVar29 = stackVariable63.CreateTable("mix_theme", stackVariable65, null, stackVariable67, null);
			stackVariable70 = migrationBuilder;
			stackVariable72 = Init.u003cu003ec.u003cu003e9__0_20;
			if (stackVariable72 == null)
			{
				dummyVar30 = stackVariable72;
				stackVariable72 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType11<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_20);
				Init.u003cu003ec.u003cu003e9__0_20 = stackVariable72;
			}
			stackVariable74 = Init.u003cu003ec.u003cu003e9__0_21;
			if (stackVariable74 == null)
			{
				dummyVar31 = stackVariable74;
				stackVariable74 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType11<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_21);
				Init.u003cu003ec.u003cu003e9__0_21 = stackVariable74;
			}
			dummyVar32 = stackVariable70.CreateTable("mix_attribute_field", stackVariable72, null, stackVariable74, null);
			stackVariable77 = migrationBuilder;
			stackVariable79 = Init.u003cu003ec.u003cu003e9__0_22;
			if (stackVariable79 == null)
			{
				dummyVar33 = stackVariable79;
				stackVariable79 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType12<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_22);
				Init.u003cu003ec.u003cu003e9__0_22 = stackVariable79;
			}
			stackVariable81 = Init.u003cu003ec.u003cu003e9__0_23;
			if (stackVariable81 == null)
			{
				dummyVar34 = stackVariable81;
				stackVariable81 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType12<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_23);
				Init.u003cu003ec.u003cu003e9__0_23 = stackVariable81;
			}
			dummyVar35 = stackVariable77.CreateTable("mix_attribute_set_data", stackVariable79, null, stackVariable81, null);
			stackVariable84 = migrationBuilder;
			stackVariable86 = Init.u003cu003ec.u003cu003e9__0_24;
			if (stackVariable86 == null)
			{
				dummyVar36 = stackVariable86;
				stackVariable86 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType13<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_24);
				Init.u003cu003ec.u003cu003e9__0_24 = stackVariable86;
			}
			stackVariable88 = Init.u003cu003ec.u003cu003e9__0_25;
			if (stackVariable88 == null)
			{
				dummyVar37 = stackVariable88;
				stackVariable88 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType13<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_25);
				Init.u003cu003ec.u003cu003e9__0_25 = stackVariable88;
			}
			dummyVar38 = stackVariable84.CreateTable("mix_attribute_set_reference", stackVariable86, null, stackVariable88, null);
			stackVariable91 = migrationBuilder;
			stackVariable93 = Init.u003cu003ec.u003cu003e9__0_26;
			if (stackVariable93 == null)
			{
				dummyVar39 = stackVariable93;
				stackVariable93 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType14<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_26);
				Init.u003cu003ec.u003cu003e9__0_26 = stackVariable93;
			}
			stackVariable95 = Init.u003cu003ec.u003cu003e9__0_27;
			if (stackVariable95 == null)
			{
				dummyVar40 = stackVariable95;
				stackVariable95 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType14<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_27);
				Init.u003cu003ec.u003cu003e9__0_27 = stackVariable95;
			}
			dummyVar41 = stackVariable91.CreateTable("mix_related_attribute_set", stackVariable93, null, stackVariable95, null);
			stackVariable98 = migrationBuilder;
			stackVariable100 = Init.u003cu003ec.u003cu003e9__0_28;
			if (stackVariable100 == null)
			{
				dummyVar42 = stackVariable100;
				stackVariable100 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType15<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_28);
				Init.u003cu003ec.u003cu003e9__0_28 = stackVariable100;
			}
			stackVariable102 = Init.u003cu003ec.u003cu003e9__0_29;
			if (stackVariable102 == null)
			{
				dummyVar43 = stackVariable102;
				stackVariable102 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType15<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_29);
				Init.u003cu003ec.u003cu003e9__0_29 = stackVariable102;
			}
			dummyVar44 = stackVariable98.CreateTable("mix_configuration", stackVariable100, null, stackVariable102, null);
			stackVariable105 = migrationBuilder;
			stackVariable107 = Init.u003cu003ec.u003cu003e9__0_30;
			if (stackVariable107 == null)
			{
				dummyVar45 = stackVariable107;
				stackVariable107 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType16<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_30);
				Init.u003cu003ec.u003cu003e9__0_30 = stackVariable107;
			}
			stackVariable109 = Init.u003cu003ec.u003cu003e9__0_31;
			if (stackVariable109 == null)
			{
				dummyVar46 = stackVariable109;
				stackVariable109 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType16<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_31);
				Init.u003cu003ec.u003cu003e9__0_31 = stackVariable109;
			}
			dummyVar47 = stackVariable105.CreateTable("mix_language", stackVariable107, null, stackVariable109, null);
			stackVariable112 = migrationBuilder;
			stackVariable114 = Init.u003cu003ec.u003cu003e9__0_32;
			if (stackVariable114 == null)
			{
				dummyVar48 = stackVariable114;
				stackVariable114 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType17<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_32);
				Init.u003cu003ec.u003cu003e9__0_32 = stackVariable114;
			}
			stackVariable116 = Init.u003cu003ec.u003cu003e9__0_33;
			if (stackVariable116 == null)
			{
				dummyVar49 = stackVariable116;
				stackVariable116 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType17<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_33);
				Init.u003cu003ec.u003cu003e9__0_33 = stackVariable116;
			}
			dummyVar50 = stackVariable112.CreateTable("mix_module", stackVariable114, null, stackVariable116, null);
			stackVariable119 = migrationBuilder;
			stackVariable121 = Init.u003cu003ec.u003cu003e9__0_34;
			if (stackVariable121 == null)
			{
				dummyVar51 = stackVariable121;
				stackVariable121 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType18<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_34);
				Init.u003cu003ec.u003cu003e9__0_34 = stackVariable121;
			}
			stackVariable123 = Init.u003cu003ec.u003cu003e9__0_35;
			if (stackVariable123 == null)
			{
				dummyVar52 = stackVariable123;
				stackVariable123 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType18<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_35);
				Init.u003cu003ec.u003cu003e9__0_35 = stackVariable123;
			}
			dummyVar53 = stackVariable119.CreateTable("mix_page", stackVariable121, null, stackVariable123, null);
			stackVariable126 = migrationBuilder;
			stackVariable128 = Init.u003cu003ec.u003cu003e9__0_36;
			if (stackVariable128 == null)
			{
				dummyVar54 = stackVariable128;
				stackVariable128 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType19<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_36);
				Init.u003cu003ec.u003cu003e9__0_36 = stackVariable128;
			}
			stackVariable130 = Init.u003cu003ec.u003cu003e9__0_37;
			if (stackVariable130 == null)
			{
				dummyVar55 = stackVariable130;
				stackVariable130 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType19<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_37);
				Init.u003cu003ec.u003cu003e9__0_37 = stackVariable130;
			}
			dummyVar56 = stackVariable126.CreateTable("mix_post", stackVariable128, null, stackVariable130, null);
			stackVariable133 = migrationBuilder;
			stackVariable135 = Init.u003cu003ec.u003cu003e9__0_38;
			if (stackVariable135 == null)
			{
				dummyVar57 = stackVariable135;
				stackVariable135 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType20<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_38);
				Init.u003cu003ec.u003cu003e9__0_38 = stackVariable135;
			}
			stackVariable137 = Init.u003cu003ec.u003cu003e9__0_39;
			if (stackVariable137 == null)
			{
				dummyVar58 = stackVariable137;
				stackVariable137 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType20<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_39);
				Init.u003cu003ec.u003cu003e9__0_39 = stackVariable137;
			}
			dummyVar59 = stackVariable133.CreateTable("mix_url_alias", stackVariable135, null, stackVariable137, null);
			stackVariable140 = migrationBuilder;
			stackVariable142 = Init.u003cu003ec.u003cu003e9__0_40;
			if (stackVariable142 == null)
			{
				dummyVar60 = stackVariable142;
				stackVariable142 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType21<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_40);
				Init.u003cu003ec.u003cu003e9__0_40 = stackVariable142;
			}
			stackVariable144 = Init.u003cu003ec.u003cu003e9__0_41;
			if (stackVariable144 == null)
			{
				dummyVar61 = stackVariable144;
				stackVariable144 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType21<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_41);
				Init.u003cu003ec.u003cu003e9__0_41 = stackVariable144;
			}
			dummyVar62 = stackVariable140.CreateTable("mix_portal_page_navigation", stackVariable142, null, stackVariable144, null);
			stackVariable147 = migrationBuilder;
			stackVariable149 = Init.u003cu003ec.u003cu003e9__0_42;
			if (stackVariable149 == null)
			{
				dummyVar63 = stackVariable149;
				stackVariable149 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType22<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_42);
				Init.u003cu003ec.u003cu003e9__0_42 = stackVariable149;
			}
			stackVariable151 = Init.u003cu003ec.u003cu003e9__0_43;
			if (stackVariable151 == null)
			{
				dummyVar64 = stackVariable151;
				stackVariable151 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType22<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_43);
				Init.u003cu003ec.u003cu003e9__0_43 = stackVariable151;
			}
			dummyVar65 = stackVariable147.CreateTable("mix_portal_page_role", stackVariable149, null, stackVariable151, null);
			stackVariable154 = migrationBuilder;
			stackVariable156 = Init.u003cu003ec.u003cu003e9__0_44;
			if (stackVariable156 == null)
			{
				dummyVar66 = stackVariable156;
				stackVariable156 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType24<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_44);
				Init.u003cu003ec.u003cu003e9__0_44 = stackVariable156;
			}
			stackVariable158 = Init.u003cu003ec.u003cu003e9__0_45;
			if (stackVariable158 == null)
			{
				dummyVar67 = stackVariable158;
				stackVariable158 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType24<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_45);
				Init.u003cu003ec.u003cu003e9__0_45 = stackVariable158;
			}
			dummyVar68 = stackVariable154.CreateTable("mix_file", stackVariable156, null, stackVariable158, null);
			stackVariable161 = migrationBuilder;
			stackVariable163 = Init.u003cu003ec.u003cu003e9__0_46;
			if (stackVariable163 == null)
			{
				dummyVar69 = stackVariable163;
				stackVariable163 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType25<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_46);
				Init.u003cu003ec.u003cu003e9__0_46 = stackVariable163;
			}
			stackVariable165 = Init.u003cu003ec.u003cu003e9__0_47;
			if (stackVariable165 == null)
			{
				dummyVar70 = stackVariable165;
				stackVariable165 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType25<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_47);
				Init.u003cu003ec.u003cu003e9__0_47 = stackVariable165;
			}
			dummyVar71 = stackVariable161.CreateTable("mix_template", stackVariable163, null, stackVariable165, null);
			stackVariable168 = migrationBuilder;
			stackVariable170 = Init.u003cu003ec.u003cu003e9__0_48;
			if (stackVariable170 == null)
			{
				dummyVar72 = stackVariable170;
				stackVariable170 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType26<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_48);
				Init.u003cu003ec.u003cu003e9__0_48 = stackVariable170;
			}
			stackVariable172 = Init.u003cu003ec.u003cu003e9__0_49;
			if (stackVariable172 == null)
			{
				dummyVar73 = stackVariable172;
				stackVariable172 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType26<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_49);
				Init.u003cu003ec.u003cu003e9__0_49 = stackVariable172;
			}
			dummyVar74 = stackVariable168.CreateTable("mix_page_module", stackVariable170, null, stackVariable172, null);
			stackVariable175 = migrationBuilder;
			stackVariable177 = Init.u003cu003ec.u003cu003e9__0_50;
			if (stackVariable177 == null)
			{
				dummyVar75 = stackVariable177;
				stackVariable177 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType29<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_50);
				Init.u003cu003ec.u003cu003e9__0_50 = stackVariable177;
			}
			stackVariable179 = Init.u003cu003ec.u003cu003e9__0_51;
			if (stackVariable179 == null)
			{
				dummyVar76 = stackVariable179;
				stackVariable179 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType29<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_51);
				Init.u003cu003ec.u003cu003e9__0_51 = stackVariable179;
			}
			dummyVar77 = stackVariable175.CreateTable("mix_module_data", stackVariable177, null, stackVariable179, null);
			stackVariable182 = migrationBuilder;
			stackVariable184 = Init.u003cu003ec.u003cu003e9__0_52;
			if (stackVariable184 == null)
			{
				dummyVar78 = stackVariable184;
				stackVariable184 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType31<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_52);
				Init.u003cu003ec.u003cu003e9__0_52 = stackVariable184;
			}
			stackVariable186 = Init.u003cu003ec.u003cu003e9__0_53;
			if (stackVariable186 == null)
			{
				dummyVar79 = stackVariable186;
				stackVariable186 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType31<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_53);
				Init.u003cu003ec.u003cu003e9__0_53 = stackVariable186;
			}
			dummyVar80 = stackVariable182.CreateTable("mix_module_post", stackVariable184, null, stackVariable186, null);
			stackVariable189 = migrationBuilder;
			stackVariable191 = Init.u003cu003ec.u003cu003e9__0_54;
			if (stackVariable191 == null)
			{
				dummyVar81 = stackVariable191;
				stackVariable191 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType32<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_54);
				Init.u003cu003ec.u003cu003e9__0_54 = stackVariable191;
			}
			stackVariable193 = Init.u003cu003ec.u003cu003e9__0_55;
			if (stackVariable193 == null)
			{
				dummyVar82 = stackVariable193;
				stackVariable193 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType32<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_55);
				Init.u003cu003ec.u003cu003e9__0_55 = stackVariable193;
			}
			dummyVar83 = stackVariable189.CreateTable("mix_page_post", stackVariable191, null, stackVariable193, null);
			stackVariable196 = migrationBuilder;
			stackVariable198 = Init.u003cu003ec.u003cu003e9__0_56;
			if (stackVariable198 == null)
			{
				dummyVar84 = stackVariable198;
				stackVariable198 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType33<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_56);
				Init.u003cu003ec.u003cu003e9__0_56 = stackVariable198;
			}
			stackVariable200 = Init.u003cu003ec.u003cu003e9__0_57;
			if (stackVariable200 == null)
			{
				dummyVar85 = stackVariable200;
				stackVariable200 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType33<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_57);
				Init.u003cu003ec.u003cu003e9__0_57 = stackVariable200;
			}
			dummyVar86 = stackVariable196.CreateTable("mix_post_media", stackVariable198, null, stackVariable200, null);
			stackVariable203 = migrationBuilder;
			stackVariable205 = Init.u003cu003ec.u003cu003e9__0_58;
			if (stackVariable205 == null)
			{
				dummyVar87 = stackVariable205;
				stackVariable205 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType35<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_58);
				Init.u003cu003ec.u003cu003e9__0_58 = stackVariable205;
			}
			stackVariable207 = Init.u003cu003ec.u003cu003e9__0_59;
			if (stackVariable207 == null)
			{
				dummyVar88 = stackVariable207;
				stackVariable207 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType35<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_59);
				Init.u003cu003ec.u003cu003e9__0_59 = stackVariable207;
			}
			dummyVar89 = stackVariable203.CreateTable("mix_post_module", stackVariable205, null, stackVariable207, null);
			stackVariable210 = migrationBuilder;
			stackVariable212 = Init.u003cu003ec.u003cu003e9__0_60;
			if (stackVariable212 == null)
			{
				dummyVar90 = stackVariable212;
				stackVariable212 = new Func<ColumnsBuilder, u003cu003ef__AnonymousType36<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_60);
				Init.u003cu003ec.u003cu003e9__0_60 = stackVariable212;
			}
			stackVariable214 = Init.u003cu003ec.u003cu003e9__0_61;
			if (stackVariable214 == null)
			{
				dummyVar91 = stackVariable214;
				stackVariable214 = new Action<CreateTableBuilder<u003cu003ef__AnonymousType36<OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>, OperationBuilder<AddColumnOperation>>>>(Init.u003cu003ec.u003cu003e9.u003cUpu003eb__0_61);
				Init.u003cu003ec.u003cu003e9__0_61 = stackVariable214;
			}
			dummyVar92 = stackVariable210.CreateTable("mix_related_post", stackVariable212, null, stackVariable214, null);
			dummyVar93 = migrationBuilder.CreateIndex("IX_mix_attribute_field_AttributeSetId", "mix_attribute_field", "AttributeSetId", null, false, null);
			dummyVar94 = migrationBuilder.CreateIndex("IX_mix_attribute_field_ReferenceId", "mix_attribute_field", "ReferenceId", null, false, null);
			dummyVar95 = migrationBuilder.CreateIndex("IX_mix_attribute_set_data_AttributeSetId", "mix_attribute_set_data", "AttributeSetId", null, false, null);
			dummyVar96 = migrationBuilder.CreateIndex("IX_mix_attribute_set_reference_AttributeSetId", "mix_attribute_set_reference", "AttributeSetId", null, false, null);
			dummyVar97 = migrationBuilder.CreateIndex("IX_mix_attribute_set_value_DataId", "mix_attribute_set_value", "DataId", null, false, null);
			dummyVar98 = migrationBuilder.CreateIndex("Index_ExpiresAtTime", "mix_cache", "ExpiredDateTime", null, false, null);
			dummyVar99 = migrationBuilder.CreateIndex("IX_mix_configuration_Specificulture", "mix_configuration", "Specificulture", null, false, null);
			dummyVar100 = migrationBuilder.CreateIndex("IX_Mix_Culture", "mix_culture", "Specificulture", null, true, null);
			dummyVar101 = migrationBuilder.CreateIndex("IX_mix_file_ThemeId", "mix_file", "ThemeId", null, false, null);
			dummyVar102 = migrationBuilder.CreateIndex("IX_mix_language_Specificulture", "mix_language", "Specificulture", null, false, null);
			dummyVar103 = migrationBuilder.CreateIndex("IX_mix_module_Specificulture", "mix_module", "Specificulture", null, false, null);
			stackVariable309 = new string[2];
			stackVariable309[0] = "ModuleId";
			stackVariable309[1] = "Specificulture";
			dummyVar104 = migrationBuilder.CreateIndex("IX_mix_module_data_ModuleId_Specificulture", "mix_module_data", stackVariable309, null, false, null);
			stackVariable322 = new string[2];
			stackVariable322[0] = "PageId";
			stackVariable322[1] = "Specificulture";
			dummyVar105 = migrationBuilder.CreateIndex("IX_mix_module_data_PageId_Specificulture", "mix_module_data", stackVariable322, null, false, null);
			stackVariable335 = new string[2];
			stackVariable335[0] = "PostId";
			stackVariable335[1] = "Specificulture";
			dummyVar106 = migrationBuilder.CreateIndex("IX_mix_module_data_PostId_Specificulture", "mix_module_data", stackVariable335, null, false, null);
			stackVariable348 = new string[3];
			stackVariable348[0] = "ModuleId";
			stackVariable348[1] = "PageId";
			stackVariable348[2] = "Specificulture";
			dummyVar107 = migrationBuilder.CreateIndex("IX_mix_module_data_ModuleId_PageId_Specificulture", "mix_module_data", stackVariable348, null, false, null);
			stackVariable363 = new string[2];
			stackVariable363[0] = "ModuleId";
			stackVariable363[1] = "Specificulture";
			dummyVar108 = migrationBuilder.CreateIndex("IX_mix_module_post_ModuleId_Specificulture", "mix_module_post", stackVariable363, null, false, null);
			stackVariable376 = new string[2];
			stackVariable376[0] = "PostId";
			stackVariable376[1] = "Specificulture";
			dummyVar109 = migrationBuilder.CreateIndex("IX_mix_module_post_PostId_Specificulture", "mix_module_post", stackVariable376, null, false, null);
			dummyVar110 = migrationBuilder.CreateIndex("IX_mix_page_Specificulture", "mix_page", "Specificulture", null, false, null);
			stackVariable397 = new string[2];
			stackVariable397[0] = "ModuleId";
			stackVariable397[1] = "Specificulture";
			dummyVar111 = migrationBuilder.CreateIndex("IX_mix_page_module_ModuleId_Specificulture", "mix_page_module", stackVariable397, null, false, null);
			stackVariable410 = new string[2];
			stackVariable410[0] = "PageId";
			stackVariable410[1] = "Specificulture";
			dummyVar112 = migrationBuilder.CreateIndex("IX_mix_page_module_PageId_Specificulture", "mix_page_module", stackVariable410, null, false, null);
			stackVariable423 = new string[2];
			stackVariable423[0] = "PageId";
			stackVariable423[1] = "Specificulture";
			dummyVar113 = migrationBuilder.CreateIndex("IX_mix_page_post_PageId_Specificulture", "mix_page_post", stackVariable423, null, false, null);
			stackVariable436 = new string[2];
			stackVariable436[0] = "PostId";
			stackVariable436[1] = "Specificulture";
			dummyVar114 = migrationBuilder.CreateIndex("IX_mix_page_post_PostId_Specificulture", "mix_page_post", stackVariable436, null, false, null);
			dummyVar115 = migrationBuilder.CreateIndex("IX_mix_portal_page_navigation_ParentId", "mix_portal_page_navigation", "ParentId", null, false, null);
			dummyVar116 = migrationBuilder.CreateIndex("IX_mix_portal_page_role_PageId", "mix_portal_page_role", "PageId", null, false, null);
			dummyVar117 = migrationBuilder.CreateIndex("IX_mix_post_Specificulture", "mix_post", "Specificulture", null, false, null);
			stackVariable473 = new string[2];
			stackVariable473[0] = "MediaId";
			stackVariable473[1] = "Specificulture";
			dummyVar118 = migrationBuilder.CreateIndex("IX_mix_post_media_MediaId_Specificulture", "mix_post_media", stackVariable473, null, false, null);
			stackVariable486 = new string[2];
			stackVariable486[0] = "PostId";
			stackVariable486[1] = "Specificulture";
			dummyVar119 = migrationBuilder.CreateIndex("IX_mix_post_media_PostId_Specificulture", "mix_post_media", stackVariable486, null, false, null);
			stackVariable499 = new string[2];
			stackVariable499[0] = "ModuleId";
			stackVariable499[1] = "Specificulture";
			dummyVar120 = migrationBuilder.CreateIndex("IX_mix_post_module_ModuleId_Specificulture", "mix_post_module", stackVariable499, null, false, null);
			stackVariable512 = new string[2];
			stackVariable512[0] = "PostId";
			stackVariable512[1] = "Specificulture";
			dummyVar121 = migrationBuilder.CreateIndex("IX_mix_post_module_PostId_Specificulture", "mix_post_module", stackVariable512, null, false, null);
			stackVariable525 = new string[2];
			stackVariable525[0] = "DestinationId";
			stackVariable525[1] = "Specificulture";
			dummyVar122 = migrationBuilder.CreateIndex("IX_mix_related_post_DestinationId_Specificulture", "mix_related_post", stackVariable525, null, false, null);
			stackVariable538 = new string[2];
			stackVariable538[0] = "SourceId";
			stackVariable538[1] = "Specificulture";
			dummyVar123 = migrationBuilder.CreateIndex("IX_mix_related_post_SourceId_Specificulture", "mix_related_post", stackVariable538, null, false, null);
			dummyVar124 = migrationBuilder.CreateIndex("IX_mix_template_file_TemplateId", "mix_template", "ThemeId", null, false, null);
			dummyVar125 = migrationBuilder.CreateIndex("IX_mix_url_alias_Specificulture", "mix_url_alias", "Specificulture", null, false, null);
			return;
		}
	}
}