using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IServices
{
    public interface IDailyCounter
    {
        public int DeliveryCount( );
        public int DineinCount( );
    }
}
