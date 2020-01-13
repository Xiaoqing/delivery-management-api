namespace DeliveryManagement.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using DeliveryManagement.Domain.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Logging;

    public class ModelValidationFilter : ActionFilterAttribute
    {
        private readonly ILogger<ModelValidationFilter> _logger;

        public ModelValidationFilter(ILogger<ModelValidationFilter> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = new List<ErrorDetail>();

                foreach (var modelState in actionContext.ModelState)
                {
                    var errorKey = $" invalid {modelState.Key}";
                    ModelErrorCollection errorCollection = modelState.Value.Errors;
                    if (errorCollection != null && errorCollection.Count > 0)
                    {
                        var errorDescription = string.Join(";", errorCollection.Select(e => e.ErrorMessage));
                        errors.Add(new ErrorDetail(errorKey, errorDescription, string.Empty));
                    }
                }

                _logger.LogInformation(string.Join("; ", errors));

                actionContext.Result = new BadRequestObjectResult(errors);
            }
        }
    }
}