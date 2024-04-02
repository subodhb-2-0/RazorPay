using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using RazorPayMvc.Models;
using Razorpay.Api;
namespace RazorPayMvc.Services
{
    public class PaymentServices:IPaymentServices
    {
        private readonly string API_KEY="rzp_test_RDZIduGxyfCN3g";
        private readonly string API_SECRET="rAXkz8y5ZFDqQ4z3YPSGsD9h";
        public string CompleteOrderProcess(IHttpContextAccessor _httpContextAccessor){
            try{
                string paymentId = _httpContextAccessor.HttpContext.Request.Form["rzp_paymentid"];
                string orderId = _httpContextAccessor.HttpContext.Request.Form["rzp_orderid"];

                RazorpayClient client = new RazorpayClient(API_KEY,API_SECRET);
                Payment payment = client.Payment.Fetch(paymentId);

                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", payment.Attributes["amount"]);
                Payment paymentCaptured = payment.Capture(options);

                string amt = paymentCaptured.Attributes["amount"];
                return paymentCaptured.Attributes["status"];
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<MerchantOrder> ProcessMerchantOrder(PaymentRequest paymentRequest)
        {
            try{
                Random randomObj = new Random();
                string transactionId = randomObj.Next(10000000, 100000000).ToString();
                RazorpayClient client = new RazorpayClient(API_KEY,API_SECRET);
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", paymentRequest.Amount * 100);
                options.Add("receipt", transactionId);
                options.Add("currency", "INR");
                options.Add("payment_capture", "0"); // 1 - automatic  , 0 - manual
                Order orderResponse = client.Order.Create(options);
                string OrderId=orderResponse["id"].ToString();

                MerchantOrder order = new(){
                    OrderId=orderResponse.Attributes["id"],
                    RazorpayKey=API_KEY,
                    Amount=paymentRequest.Amount*100,
                    Currency="INR",
                    Name=paymentRequest.Name,
                    Email = paymentRequest.Email,
                    PhoneNumber = paymentRequest.PhoneNumber,
                    Address = paymentRequest.Address,
                    Description = "Order by Merchant"
                };

                return Task.FromResult(order);
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public void RefundMoney(string paymentId){
            try{
                RazorpayClient client = new RazorpayClient(API_KEY, API_SECRET);

                // payment to be refunded, payment must be a captured payment
                Payment payment = client.Payment.Fetch(paymentId);

                //Full Refund
                Refund refund = payment.Refund();

                // //Partial Refund
                // Dictionary<string, object> data = new Dictionary<string, object>();
                // data.Add("amount", "500100");
                // Refund refund = payment.Refund(data);
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public void CreatePayment()
        {
            try
            {
                RazorpayClient client = new RazorpayClient(API_KEY, API_SECRET);
                Dictionary<string, object> orderRequest = new Dictionary<string, object>();
                orderRequest.Add("amount", 50000);
                orderRequest.Add("currency", "INR");
                //orderRequest.Add("receipt", "rcptid_11");
                Dictionary<string, object> payment = new Dictionary<string, object>();
                payment.Add("capture", "automatic");
                Dictionary<string, object> captureOptions = new Dictionary<string, object>();
                captureOptions.Add("automatic_expiry_period", 12);
                captureOptions.Add("manual_expiry_period", 7200);
                captureOptions.Add("refund_speed", "optimum");
                payment.Add("capture_options", captureOptions);
                orderRequest.Add("payment", payment);

                Order order = client.Order.Create(orderRequest);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}