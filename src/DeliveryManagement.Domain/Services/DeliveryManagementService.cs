namespace DeliveryManagement.Domain.Services
{
    using System;
    using System.Threading.Tasks;
    using DeliveryManagement.Domain.Data;
    using DeliveryManagement.Domain.Models;

    public class DeliveryManagementService : IDeliveryManagementService
    {
        private readonly IDeliveryRepository _repository;

        public DeliveryManagementService(IDeliveryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Delivery> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Delivery delivery)
        {
            var existingDelivery = await _repository.GetByOrderNumberAsync(delivery.Order.OrderNumber);
            if (existingDelivery != null && existingDelivery.State != State.cancelled)
            {
                throw new OrderAlreadyDeliveredException(existingDelivery.Id);
            }

            await _repository.SaveAsync(delivery);
        }

        public async Task SaveAsync(Delivery delivery)
        {
             await _repository.SaveAsync(delivery);
        }

        public async Task CancelAsync(string id)
        {
            var delivery = await _repository.GetByIdAsync(id);
            if (delivery == null)
            {
                throw new DeliveryNotFoundException(id);
            }

            if (delivery.State == State.cancelled)
            {
                return;
            }

            if (delivery.State != State.created && delivery.State != State.approved)
            {
                throw new DeliveryOperationInvalidForStatusException(id, delivery.State);
            }

            delivery.State = State.cancelled;
            await _repository.SaveAsync(delivery);
        }

        public async Task ApproveAsync(string id)
        {
            var delivery = await _repository.GetByIdAsync(id);
            if (delivery == null)
            {
                throw new DeliveryNotFoundException(id);
            }

            if (delivery.State == State.approved)
            {
                return;
            }

            if (delivery.State != State.created)
            {
                throw new DeliveryOperationInvalidForStatusException(id, delivery.State);
            }

            if (delivery.AccessWindow.StartTime.ToUniversalTime() < DateTime.UtcNow)
            {
                throw new DeliveryTimeElapsedException(id, delivery.AccessWindow.StartTime);
            }

            delivery.State = State.approved;
            await _repository.SaveAsync(delivery);
        }

        public async Task CompleteAsync(string id)
        {
            var delivery = await _repository.GetByIdAsync(id);
            if (delivery == null)
            {
                throw new DeliveryNotFoundException(id);
            }

            if (delivery.State == State.completed)
            {
                return;
            }

            if (delivery.State != State.approved)
            {
                throw new DeliveryOperationInvalidForStatusException(id, delivery.State);
            }

            delivery.State = State.completed;
            await _repository.SaveAsync(delivery);
        }
    }
}
