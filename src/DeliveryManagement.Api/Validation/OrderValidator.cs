namespace DeliveryManagement.Api.Validation
{
    using DeliveryManagement.Api.Requests;
    using FluentValidation;

    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            this.RuleFor(o => o.OrderNumber)
                .NotEmpty()
                .WithMessage("The order number name must be provided");

            this.RuleFor(o => o.Sender)
                .NotEmpty()
                .WithMessage("The sender must be provided");
        }
    }
}
