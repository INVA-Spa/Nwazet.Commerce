﻿using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Territories")]
    public class TerritoryHierarchyPartRecord : ContentPartRecord {

        public virtual IList<TerritoryPartRecord> Territories { get; set; }

        public virtual string TerritoryType { get; set; }
    }
}
