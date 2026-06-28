using Core.DTO;
using Core.ModelView;
using System.Collections.Generic;

namespace Core.IReprosatory
{
    public interface IReviewRepo
    {
        ReviewMV Add(string username, ReviewD review);
        List<ReviewMV> GetByItem(int itemId);
    }
}
