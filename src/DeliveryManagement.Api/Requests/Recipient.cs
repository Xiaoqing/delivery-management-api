namespace DeliveryManagement.Api.Requests
{
    using System.ComponentModel.DataAnnotations;

    public class Recipient
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}