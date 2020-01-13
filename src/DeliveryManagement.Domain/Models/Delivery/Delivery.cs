namespace DeliveryManagement.Domain.Models
{
    using System;

    public class Delivery
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public State State { get; set; }
        public AccessWindow AccessWindow { get; set; }
        public Recipient Recipient { get; set; }
        public Order Order { get; set; }
    }
}