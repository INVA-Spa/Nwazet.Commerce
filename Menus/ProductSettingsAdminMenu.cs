﻿using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace Orchard.Core.Settings {
    public class ProductSettingsAdminMenu : INavigationProvider {
         public string MenuName {
            get { return "admin"; }
        }

         public ProductSettingsAdminMenu() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("nwazet-commerce")
                .Add(item => item
                    .Caption(T("Commerce"))
                    .Position("2")
                    .LinkToFirstChild(false)

                    .Add(subItem => subItem
                        .Caption(T("Pricing"))
                        .Position("2.9")
                        .Action("Index", "ProductSettingsAdmin", new { area = "Nwazet.Commerce" })
                    )
                );
        }
    }
}
