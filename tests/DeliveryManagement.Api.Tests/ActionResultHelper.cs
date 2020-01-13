namespace DeliveryManagement.Api.Tests
{
    using System;
    using Microsoft.AspNetCore.Mvc;

    public class ActionResultHelper
    {
        public static T GetObject<T>(IActionResult result) where T : class
        {
            var objectResult = result as ObjectResult;
            if (objectResult == null)
            {
                throw new InvalidOperationException($"The result must be of type {nameof(ObjectResult)}");
            }

            var value = objectResult.Value as T;
            if (value == null)
            {
                throw new InvalidOperationException($"The value in the result must be of type {nameof(T)}");
            }

            return value;
        }
    }
}
