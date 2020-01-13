namespace DeliveryManagement.Api
{
    using System;
    using System.Net;
    using DeliveryManagement.Domain.Models;

    public class HttpResponseException : Exception
    {
        public HttpResponseException(HttpStatusCode statusCode) : this(statusCode, null)
        {
        }

        public HttpResponseException(HttpStatusCode statusCode, ErrorDetail errorDetail)
        {
            StatusCode = statusCode;
            ErrorDetail = errorDetail;
        }

        public HttpStatusCode StatusCode { get; set; }

        public ErrorDetail ErrorDetail { get; set; }
    }
}