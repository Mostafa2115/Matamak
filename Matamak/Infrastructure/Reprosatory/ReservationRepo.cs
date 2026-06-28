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
    public class ReservationRepo : IReservationRepo
    {
        private readonly DataContext dataContext;

        public ReservationRepo(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public ReservationMV Add(ReservationD reservation)
        {
            var entity = new Reservation
            {
                CustomerName = reservation.CustomerName,
                ContactNumber = reservation.ContactNumber,
                TableNumber = reservation.TableNumber,
                NumberOfGuests = reservation.NumberOfGuests,
                ReservedAt = reservation.ReservedAt,
                Notes = reservation.Notes
            };

            dataContext.Reservations.Add(entity);
            dataContext.SaveChanges();

            return Map(entity);
        }

        public List<ReservationMV> GetAll()
        {
            return dataContext.Reservations
                .OrderBy(r => r.ReservedAt)
                .Select(Map)
                .ToList();
        }

        public ReservationMV UpdateStatus(int id, string status)
        {
            var entity = dataContext.Reservations.Find(id);
            if (entity == null)
            {
                throw new Exception("Reservation not found.");
            }

            entity.Status = status;
            dataContext.Reservations.Update(entity);
            dataContext.SaveChanges();
            return Map(entity);
        }

        private static ReservationMV Map(Reservation entity)
        {
            return new ReservationMV
            {
                Id = entity.Id,
                CustomerName = entity.CustomerName,
                ContactNumber = entity.ContactNumber,
                TableNumber = entity.TableNumber,
                NumberOfGuests = entity.NumberOfGuests,
                ReservedAt = entity.ReservedAt,
                Status = entity.Status,
                Notes = entity.Notes
            };
        }
    }
}
