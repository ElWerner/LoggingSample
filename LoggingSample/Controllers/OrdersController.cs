using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_BLL.Services;
using LoggingSample_DAL.Context;
using LoggingSample_DAL.Entities;
using NLog;

namespace LoggingSample.Controllers
{
    [RoutePrefix("api")]
    public class OrdersController : ApiController
    {
        private readonly OrderService _orderService = new OrderService();

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        [Route("customers/{customerId}/orders", Name = "Orders")]
        public async Task<IHttpActionResult> Get(int customerId)
        {
            Logger.Info($"Start getting list of customers.");

            try
            {
                var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);

                if (orders == null)
                {
                    Logger.Info($"No orders for customerId {customerId} were found.");
                    return NotFound();
                }

                Logger.Info($"Retrieving a list of orders for customerId {customerId} to response.");

                return Ok(orders.Select(InitOrder));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Some error occured while getting a list of orders for customerId {customerId}.");
                throw;
            }
        }

        [Route("customers/{customerId}/orders/{orderId}", Name = "Order")]
        public async Task<IHttpActionResult> Get(int customerId, int orderId)
        {
            Logger.Info($"Start getting order with id {orderId} for customerId {customerId}.");

            try
            {
                var order = await _orderService.GetOrderByIdAsync(customerId, orderId);

                if (order == null)
                {
                    Logger.Info($"No order with id {orderId} for customerId {customerId} was found.");
                    return NotFound();
                }

                Logger.Info($"Retrieving order with id {orderId} for customerId {customerId} to response.");

                return Ok(InitOrder(order));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Some error occured while getting orderId {customerId} for customerId {customerId}.");
                throw;
            }
        }

        private object InitOrder(OrderModel model)
        {
            return new
            {
                _self = new UrlHelper(Request).Link("Order", new {customerId = model.CustomerId, orderId = model.Id}),
                customer = new UrlHelper(Request).Link("Customer", new {customerId = model.CustomerId}),
                data = model
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _orderService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}