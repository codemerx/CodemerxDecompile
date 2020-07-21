using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mix.Cms.Lib.Models.Cms;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Migrations
{
	[DbContext(typeof(MixCmsContext))]
	internal class MixCmsContextModelSnapshot : ModelSnapshot
	{
		public MixCmsContextModelSnapshot()
		{
			base();
			return;
		}

		protected override void BuildModel(ModelBuilder modelBuilder)
		{
			dummyVar0 = modelBuilder.HasAnnotation("ProductVersion", "3.1.3").HasAnnotation("Relational:MaxIdentifierLength", 128).HasAnnotation("SqlServer:ValueGenerationStrategy", (SqlServerValueGenerationStrategy)2);
			stackVariable12 = modelBuilder;
			stackVariable14 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable14 == null)
			{
				dummyVar1 = stackVariable14;
				stackVariable14 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_0);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_0 = stackVariable14;
			}
			dummyVar2 = stackVariable12.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeField", stackVariable14);
			stackVariable16 = modelBuilder;
			stackVariable18 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_1;
			if (stackVariable18 == null)
			{
				dummyVar3 = stackVariable18;
				stackVariable18 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_1);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_1 = stackVariable18;
			}
			dummyVar4 = stackVariable16.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSet", stackVariable18);
			stackVariable20 = modelBuilder;
			stackVariable22 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_2;
			if (stackVariable22 == null)
			{
				dummyVar5 = stackVariable22;
				stackVariable22 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_2);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_2 = stackVariable22;
			}
			dummyVar6 = stackVariable20.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetData", stackVariable22);
			stackVariable24 = modelBuilder;
			stackVariable26 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_3;
			if (stackVariable26 == null)
			{
				dummyVar7 = stackVariable26;
				stackVariable26 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_3);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_3 = stackVariable26;
			}
			dummyVar8 = stackVariable24.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetReference", stackVariable26);
			stackVariable28 = modelBuilder;
			stackVariable30 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_4;
			if (stackVariable30 == null)
			{
				dummyVar9 = stackVariable30;
				stackVariable30 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_4);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_4 = stackVariable30;
			}
			dummyVar10 = stackVariable28.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetValue", stackVariable30);
			stackVariable32 = modelBuilder;
			stackVariable34 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_5;
			if (stackVariable34 == null)
			{
				dummyVar11 = stackVariable34;
				stackVariable34 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_5);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_5 = stackVariable34;
			}
			dummyVar12 = stackVariable32.Entity("Mix.Cms.Lib.Models.Cms.MixCache", stackVariable34);
			stackVariable36 = modelBuilder;
			stackVariable38 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_6;
			if (stackVariable38 == null)
			{
				dummyVar13 = stackVariable38;
				stackVariable38 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_6);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_6 = stackVariable38;
			}
			dummyVar14 = stackVariable36.Entity("Mix.Cms.Lib.Models.Cms.MixCmsUser", stackVariable38);
			stackVariable40 = modelBuilder;
			stackVariable42 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_7;
			if (stackVariable42 == null)
			{
				dummyVar15 = stackVariable42;
				stackVariable42 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_7);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_7 = stackVariable42;
			}
			dummyVar16 = stackVariable40.Entity("Mix.Cms.Lib.Models.Cms.MixConfiguration", stackVariable42);
			stackVariable44 = modelBuilder;
			stackVariable46 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_8;
			if (stackVariable46 == null)
			{
				dummyVar17 = stackVariable46;
				stackVariable46 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_8);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_8 = stackVariable46;
			}
			dummyVar18 = stackVariable44.Entity("Mix.Cms.Lib.Models.Cms.MixCulture", stackVariable46);
			stackVariable48 = modelBuilder;
			stackVariable50 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_9;
			if (stackVariable50 == null)
			{
				dummyVar19 = stackVariable50;
				stackVariable50 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_9);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_9 = stackVariable50;
			}
			dummyVar20 = stackVariable48.Entity("Mix.Cms.Lib.Models.Cms.MixFile", stackVariable50);
			stackVariable52 = modelBuilder;
			stackVariable54 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_10;
			if (stackVariable54 == null)
			{
				dummyVar21 = stackVariable54;
				stackVariable54 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_10);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_10 = stackVariable54;
			}
			dummyVar22 = stackVariable52.Entity("Mix.Cms.Lib.Models.Cms.MixLanguage", stackVariable54);
			stackVariable56 = modelBuilder;
			stackVariable58 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_11;
			if (stackVariable58 == null)
			{
				dummyVar23 = stackVariable58;
				stackVariable58 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_11);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_11 = stackVariable58;
			}
			dummyVar24 = stackVariable56.Entity("Mix.Cms.Lib.Models.Cms.MixMedia", stackVariable58);
			stackVariable60 = modelBuilder;
			stackVariable62 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_12;
			if (stackVariable62 == null)
			{
				dummyVar25 = stackVariable62;
				stackVariable62 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_12);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_12 = stackVariable62;
			}
			dummyVar26 = stackVariable60.Entity("Mix.Cms.Lib.Models.Cms.MixModule", stackVariable62);
			stackVariable64 = modelBuilder;
			stackVariable66 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_13;
			if (stackVariable66 == null)
			{
				dummyVar27 = stackVariable66;
				stackVariable66 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_13);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_13 = stackVariable66;
			}
			dummyVar28 = stackVariable64.Entity("Mix.Cms.Lib.Models.Cms.MixModuleData", stackVariable66);
			stackVariable68 = modelBuilder;
			stackVariable70 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_14;
			if (stackVariable70 == null)
			{
				dummyVar29 = stackVariable70;
				stackVariable70 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_14);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_14 = stackVariable70;
			}
			dummyVar30 = stackVariable68.Entity("Mix.Cms.Lib.Models.Cms.MixModulePost", stackVariable70);
			stackVariable72 = modelBuilder;
			stackVariable74 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_15;
			if (stackVariable74 == null)
			{
				dummyVar31 = stackVariable74;
				stackVariable74 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_15);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_15 = stackVariable74;
			}
			dummyVar32 = stackVariable72.Entity("Mix.Cms.Lib.Models.Cms.MixPage", stackVariable74);
			stackVariable76 = modelBuilder;
			stackVariable78 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_16;
			if (stackVariable78 == null)
			{
				dummyVar33 = stackVariable78;
				stackVariable78 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_16);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_16 = stackVariable78;
			}
			dummyVar34 = stackVariable76.Entity("Mix.Cms.Lib.Models.Cms.MixPageModule", stackVariable78);
			stackVariable80 = modelBuilder;
			stackVariable82 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_17;
			if (stackVariable82 == null)
			{
				dummyVar35 = stackVariable82;
				stackVariable82 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_17);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_17 = stackVariable82;
			}
			dummyVar36 = stackVariable80.Entity("Mix.Cms.Lib.Models.Cms.MixPagePost", stackVariable82);
			stackVariable84 = modelBuilder;
			stackVariable86 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_18;
			if (stackVariable86 == null)
			{
				dummyVar37 = stackVariable86;
				stackVariable86 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_18);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_18 = stackVariable86;
			}
			dummyVar38 = stackVariable84.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPage", stackVariable86);
			stackVariable88 = modelBuilder;
			stackVariable90 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_19;
			if (stackVariable90 == null)
			{
				dummyVar39 = stackVariable90;
				stackVariable90 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_19);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_19 = stackVariable90;
			}
			dummyVar40 = stackVariable88.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation", stackVariable90);
			stackVariable92 = modelBuilder;
			stackVariable94 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_20;
			if (stackVariable94 == null)
			{
				dummyVar41 = stackVariable94;
				stackVariable94 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_20);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_20 = stackVariable94;
			}
			dummyVar42 = stackVariable92.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPageRole", stackVariable94);
			stackVariable96 = modelBuilder;
			stackVariable98 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_21;
			if (stackVariable98 == null)
			{
				dummyVar43 = stackVariable98;
				stackVariable98 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_21);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_21 = stackVariable98;
			}
			dummyVar44 = stackVariable96.Entity("Mix.Cms.Lib.Models.Cms.MixPost", stackVariable98);
			stackVariable100 = modelBuilder;
			stackVariable102 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_22;
			if (stackVariable102 == null)
			{
				dummyVar45 = stackVariable102;
				stackVariable102 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_22);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_22 = stackVariable102;
			}
			dummyVar46 = stackVariable100.Entity("Mix.Cms.Lib.Models.Cms.MixPostMedia", stackVariable102);
			stackVariable104 = modelBuilder;
			stackVariable106 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_23;
			if (stackVariable106 == null)
			{
				dummyVar47 = stackVariable106;
				stackVariable106 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_23);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_23 = stackVariable106;
			}
			dummyVar48 = stackVariable104.Entity("Mix.Cms.Lib.Models.Cms.MixPostModule", stackVariable106);
			stackVariable108 = modelBuilder;
			stackVariable110 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_24;
			if (stackVariable110 == null)
			{
				dummyVar49 = stackVariable110;
				stackVariable110 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_24);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_24 = stackVariable110;
			}
			dummyVar50 = stackVariable108.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedAttributeData", stackVariable110);
			stackVariable112 = modelBuilder;
			stackVariable114 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_25;
			if (stackVariable114 == null)
			{
				dummyVar51 = stackVariable114;
				stackVariable114 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_25);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_25 = stackVariable114;
			}
			dummyVar52 = stackVariable112.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet", stackVariable114);
			stackVariable116 = modelBuilder;
			stackVariable118 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_26;
			if (stackVariable118 == null)
			{
				dummyVar53 = stackVariable118;
				stackVariable118 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_26);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_26 = stackVariable118;
			}
			dummyVar54 = stackVariable116.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedData", stackVariable118);
			stackVariable120 = modelBuilder;
			stackVariable122 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_27;
			if (stackVariable122 == null)
			{
				dummyVar55 = stackVariable122;
				stackVariable122 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_27);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_27 = stackVariable122;
			}
			dummyVar56 = stackVariable120.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedPost", stackVariable122);
			stackVariable124 = modelBuilder;
			stackVariable126 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_28;
			if (stackVariable126 == null)
			{
				dummyVar57 = stackVariable126;
				stackVariable126 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_28);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_28 = stackVariable126;
			}
			dummyVar58 = stackVariable124.Entity("Mix.Cms.Lib.Models.Cms.MixTemplate", stackVariable126);
			stackVariable128 = modelBuilder;
			stackVariable130 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_29;
			if (stackVariable130 == null)
			{
				dummyVar59 = stackVariable130;
				stackVariable130 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_29);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_29 = stackVariable130;
			}
			dummyVar60 = stackVariable128.Entity("Mix.Cms.Lib.Models.Cms.MixTheme", stackVariable130);
			stackVariable132 = modelBuilder;
			stackVariable134 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_30;
			if (stackVariable134 == null)
			{
				dummyVar61 = stackVariable134;
				stackVariable134 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_30);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_30 = stackVariable134;
			}
			dummyVar62 = stackVariable132.Entity("Mix.Cms.Lib.Models.Cms.MixUrlAlias", stackVariable134);
			stackVariable136 = modelBuilder;
			stackVariable138 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_31;
			if (stackVariable138 == null)
			{
				dummyVar63 = stackVariable138;
				stackVariable138 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_31);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_31 = stackVariable138;
			}
			dummyVar64 = stackVariable136.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeField", stackVariable138);
			stackVariable140 = modelBuilder;
			stackVariable142 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_32;
			if (stackVariable142 == null)
			{
				dummyVar65 = stackVariable142;
				stackVariable142 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_32);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_32 = stackVariable142;
			}
			dummyVar66 = stackVariable140.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetData", stackVariable142);
			stackVariable144 = modelBuilder;
			stackVariable146 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_33;
			if (stackVariable146 == null)
			{
				dummyVar67 = stackVariable146;
				stackVariable146 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_33);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_33 = stackVariable146;
			}
			dummyVar68 = stackVariable144.Entity("Mix.Cms.Lib.Models.Cms.MixAttributeSetReference", stackVariable146);
			stackVariable148 = modelBuilder;
			stackVariable150 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_34;
			if (stackVariable150 == null)
			{
				dummyVar69 = stackVariable150;
				stackVariable150 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_34);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_34 = stackVariable150;
			}
			dummyVar70 = stackVariable148.Entity("Mix.Cms.Lib.Models.Cms.MixConfiguration", stackVariable150);
			stackVariable152 = modelBuilder;
			stackVariable154 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_35;
			if (stackVariable154 == null)
			{
				dummyVar71 = stackVariable154;
				stackVariable154 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_35);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_35 = stackVariable154;
			}
			dummyVar72 = stackVariable152.Entity("Mix.Cms.Lib.Models.Cms.MixFile", stackVariable154);
			stackVariable156 = modelBuilder;
			stackVariable158 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_36;
			if (stackVariable158 == null)
			{
				dummyVar73 = stackVariable158;
				stackVariable158 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_36);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_36 = stackVariable158;
			}
			dummyVar74 = stackVariable156.Entity("Mix.Cms.Lib.Models.Cms.MixLanguage", stackVariable158);
			stackVariable160 = modelBuilder;
			stackVariable162 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_37;
			if (stackVariable162 == null)
			{
				dummyVar75 = stackVariable162;
				stackVariable162 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_37);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_37 = stackVariable162;
			}
			dummyVar76 = stackVariable160.Entity("Mix.Cms.Lib.Models.Cms.MixModule", stackVariable162);
			stackVariable164 = modelBuilder;
			stackVariable166 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_38;
			if (stackVariable166 == null)
			{
				dummyVar77 = stackVariable166;
				stackVariable166 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_38);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_38 = stackVariable166;
			}
			dummyVar78 = stackVariable164.Entity("Mix.Cms.Lib.Models.Cms.MixModuleData", stackVariable166);
			stackVariable168 = modelBuilder;
			stackVariable170 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_39;
			if (stackVariable170 == null)
			{
				dummyVar79 = stackVariable170;
				stackVariable170 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_39);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_39 = stackVariable170;
			}
			dummyVar80 = stackVariable168.Entity("Mix.Cms.Lib.Models.Cms.MixModulePost", stackVariable170);
			stackVariable172 = modelBuilder;
			stackVariable174 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_40;
			if (stackVariable174 == null)
			{
				dummyVar81 = stackVariable174;
				stackVariable174 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_40);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_40 = stackVariable174;
			}
			dummyVar82 = stackVariable172.Entity("Mix.Cms.Lib.Models.Cms.MixPage", stackVariable174);
			stackVariable176 = modelBuilder;
			stackVariable178 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_41;
			if (stackVariable178 == null)
			{
				dummyVar83 = stackVariable178;
				stackVariable178 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_41);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_41 = stackVariable178;
			}
			dummyVar84 = stackVariable176.Entity("Mix.Cms.Lib.Models.Cms.MixPageModule", stackVariable178);
			stackVariable180 = modelBuilder;
			stackVariable182 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_42;
			if (stackVariable182 == null)
			{
				dummyVar85 = stackVariable182;
				stackVariable182 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_42);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_42 = stackVariable182;
			}
			dummyVar86 = stackVariable180.Entity("Mix.Cms.Lib.Models.Cms.MixPagePost", stackVariable182);
			stackVariable184 = modelBuilder;
			stackVariable186 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_43;
			if (stackVariable186 == null)
			{
				dummyVar87 = stackVariable186;
				stackVariable186 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_43);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_43 = stackVariable186;
			}
			dummyVar88 = stackVariable184.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPageNavigation", stackVariable186);
			stackVariable188 = modelBuilder;
			stackVariable190 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_44;
			if (stackVariable190 == null)
			{
				dummyVar89 = stackVariable190;
				stackVariable190 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_44);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_44 = stackVariable190;
			}
			dummyVar90 = stackVariable188.Entity("Mix.Cms.Lib.Models.Cms.MixPortalPageRole", stackVariable190);
			stackVariable192 = modelBuilder;
			stackVariable194 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_45;
			if (stackVariable194 == null)
			{
				dummyVar91 = stackVariable194;
				stackVariable194 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_45);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_45 = stackVariable194;
			}
			dummyVar92 = stackVariable192.Entity("Mix.Cms.Lib.Models.Cms.MixPost", stackVariable194);
			stackVariable196 = modelBuilder;
			stackVariable198 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_46;
			if (stackVariable198 == null)
			{
				dummyVar93 = stackVariable198;
				stackVariable198 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_46);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_46 = stackVariable198;
			}
			dummyVar94 = stackVariable196.Entity("Mix.Cms.Lib.Models.Cms.MixPostMedia", stackVariable198);
			stackVariable200 = modelBuilder;
			stackVariable202 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_47;
			if (stackVariable202 == null)
			{
				dummyVar95 = stackVariable202;
				stackVariable202 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_47);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_47 = stackVariable202;
			}
			dummyVar96 = stackVariable200.Entity("Mix.Cms.Lib.Models.Cms.MixPostModule", stackVariable202);
			stackVariable204 = modelBuilder;
			stackVariable206 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_48;
			if (stackVariable206 == null)
			{
				dummyVar97 = stackVariable206;
				stackVariable206 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_48);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_48 = stackVariable206;
			}
			dummyVar98 = stackVariable204.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet", stackVariable206);
			stackVariable208 = modelBuilder;
			stackVariable210 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_49;
			if (stackVariable210 == null)
			{
				dummyVar99 = stackVariable210;
				stackVariable210 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_49);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_49 = stackVariable210;
			}
			dummyVar100 = stackVariable208.Entity("Mix.Cms.Lib.Models.Cms.MixRelatedPost", stackVariable210);
			stackVariable212 = modelBuilder;
			stackVariable214 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_50;
			if (stackVariable214 == null)
			{
				dummyVar101 = stackVariable214;
				stackVariable214 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_50);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_50 = stackVariable214;
			}
			dummyVar102 = stackVariable212.Entity("Mix.Cms.Lib.Models.Cms.MixTemplate", stackVariable214);
			stackVariable216 = modelBuilder;
			stackVariable218 = MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_51;
			if (stackVariable218 == null)
			{
				dummyVar103 = stackVariable218;
				stackVariable218 = new Action<EntityTypeBuilder>(MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9, MixCmsContextModelSnapshot.u003cu003ec.u003cBuildModelu003eb__0_51);
				MixCmsContextModelSnapshot.u003cu003ec.u003cu003e9__0_51 = stackVariable218;
			}
			dummyVar104 = stackVariable216.Entity("Mix.Cms.Lib.Models.Cms.MixUrlAlias", stackVariable218);
			return;
		}
	}
}