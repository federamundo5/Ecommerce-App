using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.OrderAggregate;

namespace API.Dtos
{
    public class OrderToReturnDto
    {
        public int id {get;set;}
         public string  BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        public OrderAddress ShipToAddress {get; set;}

        public string DeliveryMethod {get;set;}
        public decimal shippingPrice { get; set; }

        public IReadOnlyList<OrderItemDto> OrderItems {get;set;}

        public Decimal Subtotal {get;set;}
       public decimal Total { get; set; }
        public string Status {get;set;}

    }
}