﻿using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationService : IVatConfigurationService {
        public int GetDefaultCategoryId() {
            throw new NotImplementedException();
        }
    }
}
