using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.OrderAggregate
{
    public class Order: BaseEntity
    {
        public Order()
        {
        }

        public Order(IReadOnlyList<OrderItem> orderItems, 
        string buyerEmail, OrderAddress shipToAddress, DeliveryMethod deliveryMethod,  decimal subtotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShipToAddress = shipToAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            Subtotal = subtotal;
            PaymentIntentId = paymentIntentId;
        }

        public string  BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        [Required]
        public OrderAddress ShipToAddress {get; set;}

        public DeliveryMethod DeliveryMethod {get;set;}

        public IReadOnlyList<OrderItem> OrderItems {get;set;}

        public Decimal Subtotal {get;set;}

        public OrderStatus Status {get;set;} =  OrderStatus.Pending;

        public string PaymentIntentId {get;set;}

        public decimal GetTotal(){
            return Subtotal + DeliveryMethod.Price;
        }
        
    }
}