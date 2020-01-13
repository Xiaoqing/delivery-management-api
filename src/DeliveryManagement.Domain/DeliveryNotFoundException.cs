namespace DeliveryManagement.Domain
{
    using System;

    public class DeliveryNotFoundException : Exception
    {
        public string DeliveryId { get; }

        public DeliveryNotFoundException(string deliveryId)
        {
            DeliveryId = deliveryId;
        }
    }
}