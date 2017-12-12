﻿using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System.Data;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.BaseTaxImplementations")]
    public class BaseTaxImplementationsMigrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable("StateOrCountryTaxPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("State")
                .Column<string>("Country")
                .Column<double>("Rate")
                .Column<int>("Priority")
            );

            ContentDefinitionManager.AlterTypeDefinition("StateOrCountryTax", cfg => cfg
              .WithPart("StateOrCountryTaxPart"));

            return 1;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition("ZipCodeTax", cfg => cfg
                .WithPart("ZipCodeTaxPart"));
            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.AlterTable("StateOrCountryTaxPartRecord", table =>
                table.AlterColumn("Rate", column =>
                    column.WithType(DbType.Decimal)));
            return 3;
        }
    }
}
