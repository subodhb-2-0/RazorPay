using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RazorPayMvc.Models;
using RazorPayMvc.Services;

namespace RazorPayMvc.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentServices _paymentService;

        public PaymentsController(ILogger<PaymentsController> logger, IPaymentServices paymentService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProcessRequestOrder(PaymentRequest _paymentRequest)
        {
            MerchantOrder _merchantOrder = await _paymentService.ProcessMerchantOrder(_paymentRequest);
            return View("payments", _merchantOrder);
        }
        
        [HttpPost]
        public IActionResult CompleteOrderProcess()
        {
            string PaymentMessage =_paymentService.CompleteOrderProcess(_httpContextAccessor);
            if (PaymentMessage == "captured")
            {
                return Content("Success");
            }
            else
            {
                return Content("Failed");
            }
        }

        [HttpPost]
        public IActionResult RefundFunds(string paymentId){
            _paymentService.RefundMoney(paymentId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreatePayment()
        {
            _paymentService.CreatePayment();
            return Content("Ok");
        }
    }
}