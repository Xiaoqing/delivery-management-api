namespace DeliveryManagement.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using DeliveryManagement.Domain;
    using DeliveryManagement.Domain.Models;
    using DeliveryManagement.Domain.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Delivery = DeliveryManagement.Api.Requests.Delivery;
    using DeliveryModel = DeliveryManagement.Domain.Models.Delivery;

    [ApiController]
    [Route("v1/[controller]")]
    [Authorize]
    public class DeliveryController : ControllerBase
    {
        private readonly ILogger<DeliveryController> _logger;
        private readonly IDeliveryManagementService _service;

        public DeliveryController(ILogger<DeliveryController> logger, IDeliveryManagementService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Get a delivery by id.
        /// </summary>
        /// <param name="id">The id of the delivery</param>
        /// <returns>The delivery for the id</returns>
        /// <response code="200">The delivery for the id</response>
        /// <response code="404">If delivery for the id doesn't exist</response>
        [HttpGet("{id}", Name = "GetDelivery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var delivery = await _service.GetByIdAsync(id);
            if (delivery == null)
            {
                _logger.LogWarning("Delivery with id {Id} can not be found", id);
                var error = new ErrorDetail("delivery_not_found", $"Delivery with id {id} can not be found");
                return NotFound(error);
            }

            return Ok(delivery);
        }

        /// <summary>
        /// Get a delivery by id.
        /// </summary>
        /// <param name="delivery">The delivery to be created</param>
        /// <returns>The newly created delivery item</returns>
        /// <response code="201">The newly created delivery item</response>
        /// <response code="409">If the order number in the delivery request is already associated with another delivery</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] Delivery delivery)
        {
            try
            {
                var deliveryModel = GenerateDeliveryFromRequest(delivery);

                await _service.CreateAsync(deliveryModel);

                return CreatedAtRoute("GetDelivery", new {id = deliveryModel.Id}, deliveryModel);
            }
            catch (OrderAlreadyDeliveredException e)
            {
                _logger.LogWarning("Order number {OrderNumber} has already been delivered {Id}", delivery.Order.OrderNumber, e.DeliveryId);
                var error = new ErrorDetail("order_already_delivered", $"Order number {delivery.Order.OrderNumber} has already been delivered - {e.DeliveryId}");
                return Conflict(error);
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] string id, [FromBody] DeliveryCancellation cancellation)
        {
            try
            {
                await _service.CancelAsync(id);

                return Ok();
            }
            catch (DeliveryNotFoundException e)
            {
                _logger.LogWarning("Delivered {Id} can not be found", id);
                var error = new ErrorDetail("delivery_not_found", $"Delivered {id} can not be found");
                return NotFound(error);
            }
            catch (DeliveryOperationInvalidForStatusException e)
            {
                _logger.LogWarning("Delivered {Id} are not valid for cancellation", id);
                var error = new ErrorDetail("delivery_operation_invalid", $"Delivery {id} can not be cancelled because of its current state {e.CurrentState}");
                return Conflict(error);
            }
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve([FromRoute] string id)
        {
            try
            {
                await _service.ApproveAsync(id);

                return Ok();
            }
            catch (DeliveryNotFoundException e)
            {
                _logger.LogWarning("Delivered {Id} can not be found", id);
                var error = new ErrorDetail("delivery_not_found", $"Delivered {id} can not be found");
                return NotFound(error);
            }
            catch (DeliveryOperationInvalidForStatusException e)
            {
                _logger.LogWarning("Delivered {Id} are not valid for approval", id);
                var error = new ErrorDetail("delivery_operation_invalid", $"Delivery {id} can not be approved because of its current state {e.CurrentState}. Only delivery in status created can be approved");
                return Conflict(error);
            }
            catch (DeliveryTimeElapsedException e)
            {
                _logger.LogWarning("Delivered {Id} is too late for approval", id);
                var error = new ErrorDetail("delivery_time_elapsed", $"Delivery {id} can not be approved because of its start time {e.StartTime} is in the past");
                return Conflict(error);
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete([FromRoute] string id)
        {
            try
            {
                await _service.CompleteAsync(id);

                return Ok();
            }
            catch (DeliveryNotFoundException e)
            {
                _logger.LogWarning("Delivered {Id} can not be found", id);
                var error = new ErrorDetail("delivery_not_found", $"Delivered {id} can not be found");
                return NotFound(error);
            }
            catch (DeliveryOperationInvalidForStatusException e)
            {
                _logger.LogWarning("Delivered {Id} are not valid for approval", id);
                var error = new ErrorDetail("delivery_operation_invalid", $"Delivery {id} can not be approved because of its current state {e.CurrentState}. Only delivery in status created can be approved");
                return Conflict(error);
            }
        }

        private static DeliveryModel GenerateDeliveryFromRequest(Delivery delivery)
        {
            var deliveryModel = new DeliveryModel
            {
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now.AddMinutes(1),
                    EndTime = DateTime.Now.AddHours(2).AddMinutes(1)
                },
                Recipient = new Recipient
                {
                    Name = delivery.Recipient.Name,
                    Address = delivery.Recipient.Address,
                    Email = delivery.Recipient.Email,
                    PhoneNumber = delivery.Recipient.PhoneNumber
                },
                Order = new Order
                {
                    OrderNumber = delivery.Order.OrderNumber,
                    Sender = delivery.Order.Sender
                }
            };

            return deliveryModel;
        }
    }
}
