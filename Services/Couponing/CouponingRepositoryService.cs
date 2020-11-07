﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models.Couponing;
using Orchard.Data;
using Nwazet.Commerce.Extensions;
using Orchard.Environment.Extensions;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels.Couponing;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services.Couponing {
    [OrchardFeature("Nwazet.Couponing")]
    public class CouponingRepositoryService : ICouponRepositoryService {

        private readonly IRepository<CouponRecord> _couponsRepository;

        public CouponingRepositoryService(
            IRepository<CouponRecord> couponsRepository) {

            _couponsRepository = couponsRepository;
        }

        public IQueryable<CouponRecord> GetCoupons() {
            return _couponsRepository.Table;
        }

        public Coupon Get(int id) {
            var coupon = _couponsRepository.Get(id).ToCoupon();
            return coupon;
        }

        public int CreateRecord(Coupon coupon) {
            var record = new CouponRecord();
            record.FromCoupon(coupon);
            _couponsRepository.Create(record);
            return record.Id;
        }

        public void UpdateRecord(Coupon coupon) {
            var record = _couponsRepository.Get(coupon.Id);
            record.FromCoupon(coupon);
        }

        public void DeleteRecord(int id) {
            var record = _couponsRepository.Get(id);
            _couponsRepository.Delete(record);
        }

    }
}
