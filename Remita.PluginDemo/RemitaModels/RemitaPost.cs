using Remita.Plugin.Services;

namespace Remita.Plugin.RemitalModels
{
    public class RemitaPost
    {
        public string apiKey { get; set; }
        public string payerName { get; set; }
        public string payerEmail { get; set; }
        public string payerPhone { get; set; }
        public string orderId { get; set; }
        public string merchantId { get; set; }
        public string serviceTypeId { get; set; }
        public string responseurl { get; set; }
        public string amt { get; set; }
        public string paymenttype { get; set; }
        public string hash
        {
            get
            {
                return RemitaHash.HashRemitaRequest(orderId, amt, responseurl, merchantId, serviceTypeId, apiKey);
            }
        }

        public string postUrl { get; set; } = "https://www.remitademo.net/remita/ecomm/init.reg";



    }
}
