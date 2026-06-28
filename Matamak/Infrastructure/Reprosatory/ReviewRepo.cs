using Core.DTO;
using Core.IReprosatory;
using Core.ModelView;
using Core.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Reprosatory
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly DataContext dataContext;

        public ReviewRepo(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public ReviewMV Add(string username, ReviewD review)
        {
            var item = dataContext.Items.FirstOrDefault(i => i.Id == review.ItemId);
            if (item == null)
            {
                throw new Exception("Item not found.");
            }

            var entity = new Review
            {
                ItemId = review.ItemId,
                CustomerUsername = username,
                Rating = review.Rating,
                Comment = review.Comment
            };

            dataContext.Reviews.Add(entity);
            dataContext.SaveChanges();

            entity = dataContext.Reviews.Include(r => r.Item).First(r => r.Id == entity.Id);
            return Map(entity);
        }

        public List<ReviewMV> GetByItem(int itemId)
        {
            return dataContext.Reviews.Include(r => r.Item)
                .Where(r => r.ItemId == itemId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(Map)
                .ToList();
        }

        private static ReviewMV Map(Review entity)
        {
            return new ReviewMV
            {
                Id = entity.Id,
                ItemId = entity.ItemId,
                ItemName = entity.Item?.Name ?? string.Empty,
                CustomerUsername = entity.CustomerUsername,
                Rating = entity.Rating,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
