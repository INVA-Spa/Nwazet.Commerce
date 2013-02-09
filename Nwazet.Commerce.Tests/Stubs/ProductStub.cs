﻿using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Tests.Stubs {
    public class ProductStub : ProductPart {
        public ProductStub(int id = -1, IEnumerable<int> attributeIds = null) {
            Record = new ProductPartRecord();
            ShippingCost = -1;
            ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = "Product"
            };
            ContentItem.Record.Id = id;
            ContentItem.Weld(this);
            if (attributeIds != null) {
                var attrPartRecord = new ProductAttributesPartRecord();
                var attrPart = new ProductAttributesPart {
                    Record = attrPartRecord
                };
                attrPart.AttributeIds = attributeIds;
                ContentItem.Weld(attrPart);
            }
        }

        public ProductStub(int id, string path, IEnumerable<int> attributeIds = null)
            : this(id, attributeIds) {
            Path = path;
        }

        public string Path { get; private set; }
    }
}