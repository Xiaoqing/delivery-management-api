namespace DeliveryManagement.Domain.Data
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using DeliveryManagement.Domain.Models;

    public class InMemoryDeliveryRepository : IDeliveryRepository
    {
        private readonly ConcurrentDictionary<string, Delivery> _deliveries = new ConcurrentDictionary<string, Delivery>();

        public Task<Delivery> GetByIdAsync(string id)
        {
            if (_deliveries.TryGetValue(Normalize(id), out var delivery))
            {
                return Task.FromResult(delivery);
            }

            return Task.FromResult<Delivery>(null);
        }

        public Task SaveAsync(Delivery delivery)
        {
            _deliveries.AddOrUpdate(Normalize(delivery.Id), delivery, (s, delivery1) => delivery);

            return Task.CompletedTask;
        }

        public Task<Delivery> GetByOrderNumberAsync(string orderNumber)
        {
            var delivery = _deliveries.Values
                .FirstOrDefault(d => d.Order.OrderNumber.Equals(orderNumber, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(delivery);
        }

        private static string Normalize(string name)
        {
            return name.ToUpperInvariant();
        }
    }
}
