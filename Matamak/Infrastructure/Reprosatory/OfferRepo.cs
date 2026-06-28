using Core.DTO;
using Core.IReprosatory;
using Core.ModelView;
using Core.Models;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Reprosatory
{
    public class OfferRepo : IOfferRepo
    {
        private readonly DataContext dataContext;

        public OfferRepo(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public OfferMV Add(OfferD offer)
        {
            var entity = new Offer
            {
                Name = offer.Name,
                Code = offer.Code.Trim(),
                DiscountPercentage = offer.DiscountPercentage,
                FlatDiscountAmount = offer.FlatDiscountAmount,
                IsActive = offer.IsActive,
                StartsAt = offer.StartsAt,
                EndsAt = offer.EndsAt
            };

            dataContext.Offers.Add(entity);
            dataContext.SaveChanges();
            return Map(entity);
        }

        public void Delete(int id)
        {
            var entity = dataContext.Offers.Find(id);
            if (entity == null)
            {
                throw new Exception("Offer not found.");
            }

            dataContext.Offers.Remove(entity);
            dataContext.SaveChanges();
        }

        public List<OfferMV> GetAll(bool onlyActive)
        {
            var query = dataContext.Offers.AsQueryable();
            if (onlyActive)
            {
                var now = DateTime.UtcNow;
                query = query.Where(o => o.IsActive
                    && (!o.StartsAt.HasValue || o.StartsAt <= now)
                    && (!o.EndsAt.HasValue || o.EndsAt >= now));
            }

            return query.OrderByDescending(o => o.Id).Select(Map).ToList();
        }

        public OfferMV? GetByCode(string code)
        {
            var entity = dataContext.Offers.FirstOrDefault(o => o.Code == code);
            return entity == null ? null : Map(entity);
        }

        public OfferMV Update(int id, OfferD offer)
        {
            var entity = dataContext.Offers.Find(id);
            if (entity == null)
            {
                throw new Exception("Offer not found.");
            }

            entity.Name = offer.Name;
            entity.Code = offer.Code.Trim();
            entity.DiscountPercentage = offer.DiscountPercentage;
            entity.FlatDiscountAmount = offer.FlatDiscountAmount;
            entity.IsActive = offer.IsActive;
            entity.StartsAt = offer.StartsAt;
            entity.EndsAt = offer.EndsAt;

            dataContext.Offers.Update(entity);
            dataContext.SaveChanges();
            return Map(entity);
        }

        private static OfferMV Map(Offer entity)
        {
            return new OfferMV
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                DiscountPercentage = entity.DiscountPercentage,
                FlatDiscountAmount = entity.FlatDiscountAmount,
                IsActive = entity.IsActive,
                StartsAt = entity.StartsAt,
                EndsAt = entity.EndsAt
            };
        }
    }
}
