global using Core.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class DailyCounter : IDailyCounter
    {
        private readonly DataContext dataContext;

        public DailyCounter(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public int DineinCount()
        {
            var today = DateTime.Today;
            var count = dataContext.DailyDineinCounter.FirstOrDefault(v => v.lastDineinDate.Date == today);

            if (count == null)
            {
                var newCounter = new _DailyDineinCounter
                {
                    lastDineinDate = DateTime.Now,
                    Dinein = 1
                };
                dataContext.DailyDineinCounter.Add(newCounter);
                dataContext.SaveChanges();
                return 1;
            }
            else
            {
                count.Dinein += 1;
                dataContext.DailyDineinCounter.Update(count);
                dataContext.SaveChanges();
                return count.Dinein;


            }
        }

        int IDailyCounter.DeliveryCount( )
        {
           
                var today = DateTime.Today;
                var count = dataContext.DailyDeliveryCounter.FirstOrDefault(v => v.lastDeliverDate.Date == today);

                if (count == null)
                {
                    var newCounter = new _DailyDeliverCounter
                    {
                        lastDeliverDate = DateTime.Now,
                        Delivery = 1
                    };
                    dataContext.DailyDeliveryCounter.Add(newCounter);
                    dataContext.SaveChanges();
                    return 1;
                }
                else
                {
                    count.Delivery += 1;
                    dataContext.DailyDeliveryCounter.Update(count);
                    dataContext.SaveChanges();
                    return count.Delivery;


                }
            
          
        }


    }
}
