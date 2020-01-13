namespace DeliveryManagement.Api.Requests
{
    using System.ComponentModel.DataAnnotations;

    public class Delivery
    {
        [Required]
        public Recipient Recipient { get; set; }
        [Required]
        public Order Order { get; set; }
    }
}