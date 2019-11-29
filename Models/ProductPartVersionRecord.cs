﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPartVersionRecord : ContentPartVersionRecord {
        public ProductPartVersionRecord() {
            ShippingCost = null;
        }
        public virtual string Sku { get; set; }
        public virtual decimal Price { get; set; }
        public virtual decimal DiscountPrice { get; set; }
        public virtual bool IsDigital { get; set; }
        public virtual decimal? ShippingCost { get; set; }
        public virtual double Weight { get; set; }
        public virtual string Size { get; set; }
        public virtual bool OverrideTieredPricing { get; set; }
        public virtual string PriceTiers { get; set; }
        public virtual bool AuthenticationRequired { get; set; }

        #region Properties that have been removed / moved to other classes
        public virtual bool ConsiderInventory { get; set; } //applies for digital products, telling whether to consider a limited inventory
        public virtual int Inventory { get; set; }
        public virtual string OutOfStockMessage { get; set; }
        public virtual bool AllowBackOrder { get; set; }
        public virtual int MinimumOrderQuantity { get; set; }
        #endregion
    }
}
