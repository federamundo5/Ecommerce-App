using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.OrderAggregate;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IPaymentService _paymentService;

        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepo, IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
         
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethod, string basketId, OrderAddress ShippingAddress)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            var items = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
             var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);   
             var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
            var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
            items.Add(orderItem);
            }

            var deliveryMethodObject = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethod);
            
            var subtotal = items.Sum(item => item.Price * item.Quantity);


            //check to see if order exists
             var spec = new OrderByPaymentIntentIdWithItemsSpecification(basket.PaymentIntentId);
             var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
             if (existingOrder != null)
             {
                 _unitOfWork.Repository<Order>().Delete(existingOrder);
                 await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
             }

             //create order    
            var order = new Order(items,buyerEmail,ShippingAddress, deliveryMethodObject, subtotal, basket.PaymentIntentId);


            _unitOfWork.Repository<Order>().add(order);
            var result = await _unitOfWork.Complete();

            if(result <=0) return null;

            await _basketRepo.DeleteBaskeyAsync(basketId);

            return order;

        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public  async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id,buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUsersAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}