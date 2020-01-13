namespace DeliveryManagement.Api.Validation
{
    using DeliveryManagement.Api.Requests;
    using FluentValidation;

    public class DeliveryValidator : AbstractValidator<Delivery>
    {
        public DeliveryValidator()
        {
            this.RuleFor(d => d.Recipient)
                .NotNull()
                .WithMessage("The recipient details for the delivery must be provided");

            this.RuleFor(d => d.Order)
                .NotNull()
                .WithMessage("The order details for the delivery must be provided");
        }
    }
}
