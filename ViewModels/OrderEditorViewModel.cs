﻿using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace Nwazet.Commerce.ViewModels {
    public class OrderEditorViewModel {
        public OrderPart Order { get; set; }
        public IEnumerable<CheckoutItem> OrderItems { get; set; }
        public IDictionary<int, IContent> Products { get; set; }
        public string ShippingAddressText { get; set; }
        public string BillingAddressText { get; set; }
        public IEnumerable<string> OrderStates { get; set; }
        public IDictionary<string, LocalizedString> StatusLabels { get; set; } 
        public IEnumerable<string> EventCategories { get; set; }
        public IDictionary<string, LocalizedString> EventCategoryLabels { get; set; }
        public string LinkToTransaction { get; set; }
        public string UserName { get; set; }
        public bool UserNameNeeded { get; set; }
        public string CurrencyCode { get; set; }
        /// <summary>
        /// Use this string as a sort of "title" for the payment provider/system used
        /// </summary>
        public string PaymentProviderText { get; set; }

        public IEnumerable<dynamic> AdditionalMetadataShapes { get; set; }
        public IEnumerable<dynamic> AdditionalStatusShapes { get; set; }
        public IEnumerable<dynamic> AdditionalAddressesShapes { get; set; }
        public IEnumerable<dynamic> AdditionalProductShapes { get; set; }
        public IEnumerable<OrderEditorAdditionalProductInfoViewModel> AdditionalProductInformation { get; set; }
        public IEnumerable<dynamic> AdditionalOrderTrackingShapes { get; set; }
    }
}