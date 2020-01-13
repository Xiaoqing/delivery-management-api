namespace DeliveryManagement.Api.Tests
{
    using System;
    using System.Threading.Tasks;
    using DeliveryManagement.Api.Controllers;
    using DeliveryManagement.Domain;
    using DeliveryManagement.Domain.Models;
    using DeliveryManagement.Domain.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Shouldly;
    using Delivery = DeliveryManagement.Api.Requests.Delivery;
    using Order = DeliveryManagement.Api.Requests.Order;
    using Recipient = DeliveryManagement.Api.Requests.Recipient;

    [TestFixture]
    public class DeliveryControllerTests
    {
        private Mock<ILogger<DeliveryController>> _logger;
        private Mock<IDeliveryManagementService> _service;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<DeliveryController>>();
            _service = new Mock<IDeliveryManagementService>();
        }

        [Test(Description = "Get Delivery")]
        public async Task Should_return_NotFound_when_delivery_id_does_not_exist()
        {
            var delivery = new Domain.Models.Delivery();

            this._service.Setup(s => s.GetByIdAsync(delivery.Id)).ReturnsAsync(() => null);

            var subject = this.CreateSubject();

            var result = await subject.Get(delivery.Id);
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("delivery_not_found");
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Test(Description = "Get Delivery")]
        public async Task Should_return_Ok_when_delivery_id_exist()
        {
            var delivery = new Domain.Models.Delivery {State = State.cancelled};

            _service.Setup(s => s.GetByIdAsync(delivery.Id)).ReturnsAsync(() => delivery);

            var subject = this.CreateSubject();

            var result = await subject.Get(delivery.Id);
            
            result.ShouldBeOfType<OkObjectResult>();
            var value = ActionResultHelper.GetObject<Domain.Models.Delivery>(result);
            value.Id.ShouldBe(delivery.Id);
            value.State.ShouldBe(delivery.State);
        }

        [Test(Description = "Create Delivery")]
        public async Task Should_return_CreatedAtRoute_when_delivery_order_does_not_exist()
        {
            var delivery = new Delivery {Order = new Order{OrderNumber = "OrderNumber", Sender = "Sender"}, Recipient = new Recipient()};

            _service.Setup(s => s.CreateAsync(It.IsAny<Domain.Models.Delivery>())).Returns(() => Task.CompletedTask);

            var subject = this.CreateSubject();

            var result = await subject.Create(delivery);

            result.ShouldBeOfType<CreatedAtRouteResult>();
        }

        [Test(Description = "Create Delivery")]
        public async Task Should_return_Conflicted_when_delivery_order_exist()
        {
            var delivery = new Delivery{Order = new Order(), Recipient = new Recipient()};

            _service
                .Setup(s => s.CreateAsync(It.IsAny<Domain.Models.Delivery>()))
                .Returns(Task.FromException(new OrderAlreadyDeliveredException("id")));

            var subject = this.CreateSubject();

            var result = await subject.Create(delivery);

            result.ShouldBeOfType<ConflictObjectResult>();
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("order_already_delivered");
        }

        [Test(Description = "Cancel Delivery")]
        public async Task Should_return_NotFound_when_delivery_to_cancel_does_not_exist()
        {
            string id = "Id";

            _service
                .Setup(s => s.CancelAsync(id))
                .Returns(Task.FromException(new DeliveryNotFoundException(id)));

            var subject = this.CreateSubject();

            var result = await subject.Cancel(id, new DeliveryCancellation());

            result.ShouldBeOfType<NotFoundObjectResult>();
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("delivery_not_found");
        }

        [Test(Description = "Cancel Delivery")]
        public async Task Should_return_Conflicted_when_delivery_to_cancel_not_in_right_status()
        {
            string id = "Id";

            _service
                .Setup(s => s.CancelAsync(id))
                .Returns(Task.FromException(new DeliveryOperationInvalidForStatusException(id, State.completed)));

            var subject = this.CreateSubject();

            var result = await subject.Cancel(id, new DeliveryCancellation());

            result.ShouldBeOfType<ConflictObjectResult>();
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("delivery_operation_invalid");
        }

        [Test(Description = "Cancel Delivery")]
        public async Task Should_return_OK_when_delivery_can_be_cancelled()
        {
            string id = "Id";

            _service
                .Setup(s => s.CancelAsync(id))
                .Returns(Task.CompletedTask);

            var subject = this.CreateSubject();

            var result = await subject.Cancel(id, new DeliveryCancellation());

            result.ShouldBeOfType<OkResult>();
        }

        [Test(Description = "Approve Delivery")]
        public async Task Should_return_NotFound_when_delivery_to_approve_does_not_exist()
        {
            string id = "Id";

            _service
                .Setup(s => s.ApproveAsync(id))
                .Returns(Task.FromException(new DeliveryNotFoundException(id)));

            var subject = this.CreateSubject();

            var result = await subject.Approve(id);

            result.ShouldBeOfType<NotFoundObjectResult>();
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("delivery_not_found");
        }

        [Test(Description = "Approve Delivery")]
        public async Task Should_return_Conflicted_when_delivery_to_approve_not_in_right_status()
        {
            string id = "Id";

            _service
                .Setup(s => s.ApproveAsync(id))
                .Returns(Task.FromException(new DeliveryOperationInvalidForStatusException(id, State.completed)));

            var subject = this.CreateSubject();

            var result = await subject.Approve(id);

            result.ShouldBeOfType<ConflictObjectResult>();
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("delivery_operation_invalid");
        }

        [Test(Description = "Approve Delivery")]
        public async Task Should_return_Conflicted_when_it_is_late_to_approve_the_delivery()
        {
            string id = "Id";

            _service
                .Setup(s => s.ApproveAsync(id))
                .Returns(Task.FromException(new DeliveryTimeElapsedException(id, DateTime.Now.AddHours(-1))));

            var subject = this.CreateSubject();

            var result = await subject.Approve(id);

            result.ShouldBeOfType<ConflictObjectResult>();
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("delivery_time_elapsed");
        }

        [Test(Description = "Approve Delivery")]
        public async Task Should_return_OK_when_delivery_can_be_approved()
        {
            string id = "Id";

            _service
                .Setup(s => s.ApproveAsync(id))
                .Returns(Task.CompletedTask);

            var subject = this.CreateSubject();

            var result = await subject.Approve(id);

            result.ShouldBeOfType<OkResult>();
        }

        [Test(Description = "Complete Delivery")]
        public async Task Should_return_NotFound_when_delivery_to_complete_does_not_exist()
        {
            string id = "Id";

            _service
                .Setup(s => s.CompleteAsync(id))
                .Returns(Task.FromException(new DeliveryNotFoundException(id)));

            var subject = this.CreateSubject();

            var result = await subject.Complete(id);

            result.ShouldBeOfType<NotFoundObjectResult>();
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("delivery_not_found");
        }

        [Test(Description = "Complete Delivery")]
        public async Task Should_return_Conflicted_when_delivery_to_complete_not_in_right_status()
        {
            string id = "Id";

            _service
                .Setup(s => s.CompleteAsync(id))
                .Returns(Task.FromException(new DeliveryOperationInvalidForStatusException(id, State.completed)));

            var subject = this.CreateSubject();

            var result = await subject.Complete(id);

            result.ShouldBeOfType<ConflictObjectResult>();
            var value = ActionResultHelper.GetObject<ErrorDetail>(result);
            value.ErrorCode.ShouldBe("delivery_operation_invalid");
        }

        [Test(Description = "Complete Delivery")]
        public async Task Should_return_OK_when_delivery_can_be_completed()
        {
            string id = "Id";

            _service
                .Setup(s => s.CancelAsync(id))
                .Returns(Task.CompletedTask);

            var subject = this.CreateSubject();

            var result = await subject.Complete(id);

            result.ShouldBeOfType<OkResult>();
        }

        private DeliveryController CreateSubject()
        {
            return new DeliveryController(_logger.Object, _service.Object);
        }
    }
}
