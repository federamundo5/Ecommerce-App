using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Order  = Core.OrderAggregate.Order;

namespace API.Controllers
{
    public class PaymentController: BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private const string WhSecret = "whsec_0a70fdc8f908a002707b6f0eb2d0bfb5800fabdaf408a081c31fa86660ffa451";
        public ILogger<PaymentController> _logger;
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger){
            _logger = logger;
            _paymentService = paymentService;           
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId){
           var basket =   await _paymentService.CreateOrUpdatePaymentIntent(basketId);

           if(basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));
           return basket;
        } 

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebHook(){
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
            WhSecret);

            PaymentIntent intent;
            Order order;

            switch(stripeEvent.Type){
                case  "payment_intent.succeded":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succceded: ", intent.Id);
                     order = await _paymentService.UpdateOrderPaymentSucceded(intent.Id);
                     _logger.LogInformation("Order updated to payment Received", order.Id);
                    break;
                
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                   _logger.LogInformation("Payment Failed: ", intent.Id);
                       order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                     _logger.LogInformation("Payment Failed", order.Id);
                   break;

            }

            return new EmptyResult();

        }

        
    }
}