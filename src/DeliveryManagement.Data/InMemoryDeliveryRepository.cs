namespace DeliveryManagement.Data
{
    using System.Threading.Tasks;
    using DeliveryManagement.Domain.Models;

    public class InMemoryDeliveryRepository : IDeliveryRepository
    {
        public Task<Delivery> GetByIdAsync(string name)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveAsync(Delivery delivery)
        {
            throw new System.NotImplementedException();
        }

        public Task<Delivery> GetByOrderNumberAsync(string orderNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
