namespace DeliveryManagement.Data
{
    using System.Threading.Tasks;
    using DeliveryManagement.Domain.Models;

    public interface IDeliveryRepository
    {
        Task<Delivery> GetByIdAsync(string id);
        Task SaveAsync(Delivery delivery);
        Task<Delivery> GetByOrderNumberAsync(string orderNumber);
    }
}
