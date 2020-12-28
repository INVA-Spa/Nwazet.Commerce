﻿using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class ShoppingCartInfosetPartStorage : IShoppingCartStorage {
        //we store the cart in a content item for authenticated users, and in the session
        //for anonymous users
        private readonly IWorkContextAccessor _wca;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _extensionProviders;
        private readonly IContentManager _contentManager;
        private readonly IPersistentShoppingCartServices _persistentShoppingCartServices;

        public ShoppingCartInfosetPartStorage(
            IWorkContextAccessor wca,
            IEnumerable<IProductAttributeExtensionProvider> extensionProviders,
            IContentManager contentManager,
            IPersistentShoppingCartServices persistentShoppingCartServices) {

            _wca = wca;
            _extensionProviders = extensionProviders;
            _contentManager = contentManager;
            _persistentShoppingCartServices = persistentShoppingCartServices;
        }

        public string Country {
            get {
                return _persistentShoppingCartServices.Country;
            }

            set {
                _persistentShoppingCartServices.Country = value;
            }
        }

        public ShippingOption ShippingOption {
            get {
                return _persistentShoppingCartServices.ShippingOption;
            }

            set {
                _persistentShoppingCartServices.ShippingOption = value;
            }
        }

        public string ZipCode {
            get {
                return _persistentShoppingCartServices.ZipCode;
            }

            set {
                _persistentShoppingCartServices.ZipCode = value;
            }
        }

        public List<CartPriceAlteration> PriceAlterations {
            get {
                return _persistentShoppingCartServices.PriceAlterations;
            }

            set {
                _persistentShoppingCartServices.PriceAlterations = value;
            }
        }

        public List<ShoppingCartItem> Retrieve() {
            return _persistentShoppingCartServices.RetrieveCartItems();
        }

    }
}
