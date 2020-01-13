namespace DeliveryManagement.Api.Validation
{
    using DeliveryManagement.Api.Requests;
    using FluentValidation;

    public class RecipientValidator : AbstractValidator<Recipient>
    {
        public RecipientValidator()
        {
            this.RuleFor(r => r.Name)
                .NotEmpty()
                .WithMessage("The recipient name must be provided");

            this.RuleFor(r => r.Address)
                .NotEmpty()
                .WithMessage("The recipient address must be provided");

            this.RuleFor(r => r.Email)
                .NotEmpty()
                .WithMessage("The recipient address must be provided")
                .EmailAddress()
                .WithMessage("Email address must be valid");

            this.RuleFor(r => r.PhoneNumber)
                .NotEmpty()
                .WithMessage("The recipient address must be provided");
        }
    }
}
