﻿using Nwazet.Commerce.ApplicabilityCriteria;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.FlexibleShippingImplementations")]
    public class FlexibleShippingMethodPart
        : ContentPart<FlexibleShippingMethodRecord>, IShippingMethod {
        public string Name {
            get { return Retrieve(r => r.Name); }
            set { Store(r => r.Name, value); }
        }

        public string ShippingCompany {
            get { return Retrieve(r => r.ShippingCompany); }
            set { Store(r => r.ShippingCompany, value); }
        }
        public string IncludedShippingAreas {
            get { return Retrieve(r => r.IncludedShippingAreas); }
            set { Store(r => r.IncludedShippingAreas, value); }
        }
        public string ExcludedShippingAreas {
            get { return Retrieve(r => r.ExcludedShippingAreas); }
            set { Store(r => r.ExcludedShippingAreas, value); }
        }

        public decimal DefaultPrice {
            get { return Retrieve(r => r.DefaultPrice); }
            set { Store(r => r.DefaultPrice, value); }
        }

        public IList<ApplicabilityCriterionRecord> ApplicabilityCriteria {
            get { return Record.ApplicabilityCriteria; }
        }

        public IEnumerable<ShippingOption> ComputePrice(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            IEnumerable<IShippingMethod> shippingMethods,
            string country,
            string zipCode,
            IWorkContextAccessor workContextAccessor) {

            var workContext = workContextAccessor.GetContext();
            IFlexibleShippingManager flexibleShippingManager;
            if (workContext != null
                && workContext.TryResolve(out flexibleShippingManager)) {
                // we have a usable IFlexibleShippingManager here
                if (flexibleShippingManager.TestCriteria(
                    Id, new ApplicabilityContext(
                        productQuantities,
                        shippingMethods,
                        country,
                        zipCode
                    ))) {
                    // TODO: make price the result of something?
                    yield return GetOption();
                }
            }

            yield break;
        }

        private ShippingOption GetOption(decimal price) {
            return new ShippingOption {
                Description = Name,
                Price = price,
                IncludedShippingAreas =
                    IncludedShippingAreas == null
                        ? new string[] { }
                        : IncludedShippingAreas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                ExcludedShippingAreas =
                    ExcludedShippingAreas == null
                        ? new string[] { }
                        : ExcludedShippingAreas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            };
        }

        private ShippingOption GetOption() {
            return new ShippingOption {
                Description = Name,
                Price = DefaultPrice,
                IncludedShippingAreas =
                    IncludedShippingAreas == null
                        ? new string[] { }
                        : IncludedShippingAreas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                ExcludedShippingAreas =
                    ExcludedShippingAreas == null
                        ? new string[] { }
                        : ExcludedShippingAreas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            };
        }
    }
}