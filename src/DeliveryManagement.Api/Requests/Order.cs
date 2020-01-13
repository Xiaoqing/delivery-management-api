namespace DeliveryManagement.Api.Requests
{
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        [Required]
        public string OrderNumber { get; set; }
        [Required]
        public string Sender { get; set; }
    }
}