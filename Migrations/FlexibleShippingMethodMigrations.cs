﻿using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System.Data;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.FlexibleShippingImplementations")]
    public class FlexibleShippingMethodMigrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable("FlexibleShippingMethodRecord",
                table => table
                    .ContentPartRecord()
                    .Column("Name", DbType.String)
                    .Column("ShippingCompany", DbType.String)
                    .Column("DefaultPrice", DbType.Decimal)
                    .Column("IncludedShippingAreas", DbType.String)
                    .Column("ExcludedShippingAreas", DbType.String)
                );

            SchemaBuilder.CreateTable("ApplicabilityCriterionRecord",
                table => table
                    .Column<int>("Id", c => c.PrimaryKey().Identity())
                    .Column<string>("Type", c => c.WithLength(64))
                    .Column<string>("Description", c => c.WithLength(255))
                    .Column<string>("State", c => c.Unlimited())
                    .Column<int>("Position")
                    .Column<int>("FlexibleShippingMethodRecord_id")
                );


            ContentDefinitionManager.AlterTypeDefinition("FlexibleShippingMethod", cfg => cfg
              .WithPart("FlexibleShippingMethodPart")
              .WithPart("TitlePart"));

            return 1;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("ApplicabilityCriterionRecord", table => table
                .AddColumn<string>("Category", c => c.WithLength(64)));

            return 2;
        }
    }
}