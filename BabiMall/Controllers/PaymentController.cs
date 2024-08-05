using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BabiMall.Models;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Core;
using PayPalHttp;
using PayPal.Api;

namespace BabiMall.Controllers
{
    public class PaymentController : Controller
    {
        private readonly string clientId = "YOUR_CLIENT_ID";
        private readonly string clientSecret = "YOUR_CLIENT_SECRET";

        public ActionResult CreatePayment()
        {
            // Thông tin về đơn hàng, số tiền, hình thức thanh toán và thông tin liên hệ của khách hàng
            decimal amount = 100; // Số tiền thanh toán (đơn vị tùy thuộc vào yêu cầu)
            string currency = "USD"; // Loại tiền tệ
            string orderId = "ABC123"; // Mã đơn hàng
            string returnUrl = "https://example.com/payment/complete"; // URL để chuyển hướng sau khi thanh toán hoàn tất
            string cancelUrl = "https://example.com/payment/cancel"; // URL để chuyển hướng nếu người dùng hủy thanh toán

            var apiContext = new APIContext(new OAuthTokenCredential(clientId, clientSecret).GetAccessToken());

            var payment = new Payment
            {
                intent = "sale",
                payer = new PayPal.Api.Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount { currency = currency, total = amount.ToString("N2") },
                    description = "Description of your payment",
                    invoice_number = orderId
                }
            },
                redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                }
            };

            try
            {
                var createdPayment = payment.Create(apiContext); // Gửi yêu cầu tạo PaymentIntent đến PayPal
                var redirectUrl = createdPayment.links.FirstOrDefault(x => x.rel.Equals("approval_url"))?.href;

                // Trả về URL redirect đến PayPal để người dùng thực hiện thanh toán
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                // Xử lý khi có lỗi từ PayPal
                return View("Error");
            }
        }
    }
}

