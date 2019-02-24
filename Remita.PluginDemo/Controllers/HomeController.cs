using Remita.NetDemo.ViewModels;
using Remita.Plugin.RemitalModels;
using Remita.Plugin.Services;
using System;
using System.Web.Mvc;

namespace Remita.PluginDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //Get  Home/PayRemita
        public ActionResult PayRemita()
        {
            return View();
        }

        //Post  Home/PayRemita
        [HttpPost]
        public ActionResult PayRemita(PayRemitaVm model)
        {
            /*If you are persisting the transaction which is proper to do according to remita specification
             * You must check to see if the user has already made a payment e. g
             */
            bool hasTransaction = false;
            if (hasTransaction)
            {
                //Please provide the orderId, ApiKey and Merchant Id of the transaction
                string orderId = string.Empty;
                string apiKey = string.Empty; //Please provide another key if different from the default
                string merchantId = string.Empty; //Please provide another key if different from the default
                var validate = RemitaTransaction.Validate(orderId, RemitaConfigParam.APIKEY, RemitaConfigParam.MERCHANTID, RemitaConfigParam.CHECKSTATUSURL); // this will hash and validate the orderId, ApiKey and MerchantId is optional
                var result = validate.Item2;
                if (validate.Item1) // checking if the validation was successful
                {
                    if (string.IsNullOrEmpty(result.Rrr)) // check if the transaction have already generate RRR
                    {
                        // If the RRR is empty or null
                        // Delete the transaction record from Database or any persistent 
                    }
                    else
                    {
                        //otherwise confirm the payment status and repost if neccessary
                        return RedirectToAction("ConfrimPayment", new { orderID = orderId });
                    }
                }
            }

            var url = Url.Action("ConfrimPayment", "Home", new { },
                       protocol: Request.Url.Scheme);
            var remitaPost = new RemitaPost() // This class is from the nuget package
            {
                amt = model.Amount.ToString(),
                apiKey = RemitaConfigParam.APIKEY, //This constant can be changed to production value
                merchantId = RemitaConfigParam.MERCHANTID,
                serviceTypeId = RemitaConfigParam.SERVICETYPEID, // This key defined the service type you are trying to pay for
                orderId = $"TEST{DateTime.Now.Ticks.ToString()}",
                payerName = model.PayerName,
                payerEmail = model.PayerEmail,
                paymenttype = model.RemitaPaymentType.ToString(),
                responseurl = url,
                payerPhone = model.PhoneNumber,
            };
            return RedirectToAction("SubmitRemita", remitaPost);
        }

        //Get SubmiteRemita
        public ActionResult SubmitRemita(RemitaPost model)
        {
            return View(model);
        }

        /// <summary>
        /// Confirm Remita Payment
        /// </summary>
        /// <param name="RRR"></param>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult ConfrimPayment(string RRR, string orderID)
        {
            RemitaResponse result = new RemitaResponse();
            //if you persit the transaction
             //Find the transaction with either the RRR or orderID

            var validate = RemitaTransaction.Validate(orderID, RemitaConfigParam.APIKEY, RemitaConfigParam.MERCHANTID, RemitaConfigParam.CHECKSTATUSURL); 
            // this will hash and validate the orderId, ApiKey and MerchantId are optional

            if (validate.Item1.Equals(true))
            {
                result = validate.Item2;

                if (result.Status.Equals("00") || result.Status.Equals("01"))
                {
                    // Update your transaction for successful payment
                    //TODO

                    // and redirect to payment status page
                    return View(result);
                }
                else
                {
                    // Update your transaction for unsuccessful payment 
                    // TODO

                    //and retry payment
                    return RedirectToAction("RetryPayment", new { rrr = result.Rrr });
                }
            }
            else
            {
                ViewBag.ErrorInfo = $"Check your internet connection and try again";
                ViewBag.ErrorMessage = "Remita is currently unreachable";
                return View("RemitaErrorPage");
            }
            return View();
        }

        public ActionResult RetryPayment(string rrr)
        {
            var validate = RemitaTransaction.ReQuery(rrr, RemitaConfigParam.APIKEY, RemitaConfigParam.MERCHANTID, RemitaConfigParam.CHECKSTATUSURL);
            if (validate.Item1)
            {
                var result = validate.Item2;
                if (result.Status.Equals("00") || result.Status.Equals("01"))
                {
                    return RedirectToAction("ConfrimPayment", "Home", new { RRR = result.Rrr, orderID = result.OrderId });
                }
                var hash = RemitaHash.HashRemitedRePost(rrr, RemitaConfigParam.MERCHANTID, RemitaConfigParam.APIKEY); // MerchantId and ApiKey will use default if not supplied
                var url = Url.Action("ConfrimPayment", "Home", new { },
                                       protocol: Request.Url.Scheme);
                var model = new RemitaRePost
                {
                    rrr = rrr,
                    merchantId = RemitaConfigParam.MERCHANTID,
                    hash = hash,
                    responseurl = url
                };
                return View(model);
            }
            else
            {

                ViewBag.ErrorInfo = $"Check your internet connection and try again";
                ViewBag.ErrorMessage = "Remita is currently unreachable";
                return View("RemitaErrorPage");
            }

        }
    }
}