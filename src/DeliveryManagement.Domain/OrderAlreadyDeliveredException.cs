namespace DeliveryManagement.Domain
{
    using System;

    public class OrderAlreadyDeliveredException : Exception
    {
        public string DeliveryId { get; }

        public OrderAlreadyDeliveredException(string deliveryId)
        {
            DeliveryId = deliveryId;
        }
    }
}