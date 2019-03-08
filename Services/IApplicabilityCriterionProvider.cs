﻿using Nwazet.Commerce.Descriptors.ApplicabilityCriterion;
using Orchard.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Services {
    public interface IApplicabilityCriterionProvider : IEventHandler {
        void Describe(DescribeCriterionContext describe);
    }
}