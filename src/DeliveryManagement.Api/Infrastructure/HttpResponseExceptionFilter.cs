namespace DeliveryManagement.Api.Infrastructure
{
    using System.Net;
    using DeliverManagement.Api;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class HttpResponseExceptionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // we are returning error details to the client so we only care about 400 level errors here;
            // 500 level errors will be handled by the global exception handler
            if (context.Exception is HttpResponseException ex
                && ex.StatusCode < HttpStatusCode.InternalServerError) 
            {
                context.Result = new ObjectResult(ex.ErrorDetail)
                {
                    StatusCode = (int)ex.StatusCode,
                };

                context.ExceptionHandled = true;
            }
        }
    }
}