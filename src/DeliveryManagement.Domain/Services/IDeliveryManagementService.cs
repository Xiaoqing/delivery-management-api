namespace DeliveryManagement.Domain.Services
{
    using System.Threading.Tasks;
    using DeliveryManagement.Domain.Models;

    public interface IDeliveryManagementService
    {
        Task CreateAsync(Delivery delivery);
        Task<Delivery> GetByIdAsync(string id);
        Task SaveAsync(Delivery delivery);
        Task CancelAsync(string id);
        Task ApproveAsync(string id);
        Task CompleteAsync(string id);
    }
}