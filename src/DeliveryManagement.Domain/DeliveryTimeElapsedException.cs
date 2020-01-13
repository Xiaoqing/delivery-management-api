namespace DeliveryManagement.Domain
{
    using System;

    public class DeliveryTimeElapsedException : Exception
    {
        public string DeliveryId { get; }
        public DateTime StartTime { get; }

        public DeliveryTimeElapsedException(string deliveryId, DateTime startTime)
        {
            DeliveryId = deliveryId;
            StartTime = startTime;
        }
    }
}