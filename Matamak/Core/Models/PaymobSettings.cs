using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public int IntegrationId { get; set; }
        public int IframeId { get; set; }
    }
}
