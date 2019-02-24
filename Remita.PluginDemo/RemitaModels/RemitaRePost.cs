using System;
using System.Collections.Generic;
using System.Text;

namespace Remita.Plugin.RemitalModels
{
    public class RemitaRePost
    {
        public string merchantId { get; set; }
        public string hash { get; set; }
        public string rrr { get; set; }
        public string responseurl { get; set; }
        public string postUrl { get; set; } = "https://www.remitademo.net/remita/ecomm/finalize.reg";
    }
}
