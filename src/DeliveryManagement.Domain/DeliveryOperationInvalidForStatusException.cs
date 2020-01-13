namespace DeliveryManagement.Domain
{
    using System;
    using DeliveryManagement.Domain.Models;

    public class DeliveryOperationInvalidForStatusException : Exception
    {
        public string DeliveryId { get; }
        public State CurrentState { get; }

        public DeliveryOperationInvalidForStatusException(string deliveryId, State state)
        {
            DeliveryId = deliveryId;
            CurrentState = state;
        }
    }
}