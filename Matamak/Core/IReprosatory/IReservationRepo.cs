using Core.DTO;
using Core.ModelView;
using System.Collections.Generic;

namespace Core.IReprosatory
{
    public interface IReservationRepo
    {
        ReservationMV Add(ReservationD reservation);
        ReservationMV UpdateStatus(int id, string status);
        List<ReservationMV> GetAll();
    }
}
