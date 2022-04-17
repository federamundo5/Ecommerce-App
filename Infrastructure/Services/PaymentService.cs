using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.OrderAggregate;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = Core.Entities.Product;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        public IConfiguration _config { get; }
        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            //scrt key
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if(basket== null ) return null;
            var shippingPrice = 0m;

            if(basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                .GetByIdAsync((int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var items in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(items.Id);
                if(items.price != productItem.Price)
                    items.price = productItem.Price;
            }
                    var service = new PaymentIntentService();

                    PaymentIntent intent;
                    if(string.IsNullOrEmpty(basket.PaymentIntentId)){
                        var options = new PaymentIntentCreateOptions{
                            Amount = (long) basket.Items.Sum(i => i.Quantity * (i.price * 100) + (long)shippingPrice * 100),
                            Currency = "usd",
                            PaymentMethodTypes = new List<string> {"card"}
                        };
                        intent = await service.CreateAsync(options);
                        basket.PaymentIntentId = intent.Id;
                        basket.ClientSecret = intent.ClientSecret;
                    }
                    else
                    {
                        var options = new PaymentIntentUpdateOptions{
                                                        Amount = (long) basket.Items.Sum(i => i.Quantity * (i.price * 100) + (long)shippingPrice * 100),
                        };
                        await service.UpdateAsync(basket.PaymentIntentId, options);
                    }
           
            
                   await _basketRepository.UpdateBasketAsync(basket);
           
                   return basket;

        }

        public async Task<Core.OrderAggregate.Order> UpdateOrderPaymentSucceded(string paymentIntentId)
        {
           var spec = new OrderByPaymentIntentIdWithItemsSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Core.OrderAggregate.Order>().GetEntityWithSpec(spec);

            if(order == null) return null;
            
            order.Status = OrderStatus.PaymentReceived;
            _unitOfWork.Repository<Core.OrderAggregate.Order>().Update(order);
            await _unitOfWork.Complete();
            return order;        
            
            }

        public async Task<Core.OrderAggregate.Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
             var spec = new OrderByPaymentIntentIdWithItemsSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Core.OrderAggregate.Order>().GetEntityWithSpec(spec);
            order.Status = OrderStatus.PaymentFailed;
            await _unitOfWork.Complete();
            return order;

        }
    }
}