using Core.DTO;
using Core.ModelView;
using System.Collections.Generic;

namespace Core.IReprosatory
{
    public interface IOfferRepo
    {
        OfferMV Add(OfferD offer);
        OfferMV Update(int id, OfferD offer);
        void Delete(int id);
        List<OfferMV> GetAll(bool onlyActive);
        OfferMV? GetByCode(string code);
    }
}
