using Remita.Plugin.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Remita.NetDemo.ViewModels
{
    public class PayRemitaVm
    {
        [Required]
        public string PayerName { get; set; }
        [Required]
        public string PayerEmail { get; set; }
        [Required]

        public string PhoneNumber { get; set; }
        [Required]
        public double Amount { get; set; }
        public RemitaPaymentTypes RemitaPaymentType { get; set; } // This enum is from the nuget package installed
    }
}