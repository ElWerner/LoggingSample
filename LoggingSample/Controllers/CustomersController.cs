using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_BLL.Services;
using LoggingSample_DAL.Context;
using NLog;

namespace LoggingSample.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private readonly CustomerService _customerService = new CustomerService();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            Logger.Info($"Start getting list of customers.");

            try
            {
                var customers = await _customerService.GetAllCustomers();

                if (customers == null)
                {
                    Logger.Info($"No customers were found.");
                    return NotFound();
                }

                Logger.Info($"Retrieving a list of customers to response.");

                return Ok(customers.Select(InitCustomer));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Some error occured while getting a list of customers.");
                throw;
            }
        }

        [Route("{customerId}", Name = "Customer")]
        public async Task<IHttpActionResult> Get(int customerId)
        {
            Logger.Info($"Start getting customer with id {customerId}.");

            try
            {
                var customer = await _customerService.GetCustomerAsync(customerId);

                if (customer == null)
                {
                    Logger.Info($"No customer with id {customerId} was found.");
                    return NotFound();
                }

                Logger.Info($"Retrieving customer with id {customerId} to response.");

                return Ok(InitCustomer(customer));
            }
            catch (CustomerServiceException ex)
            {
                if (ex.Type == CustomerServiceException.ErrorType.WrongCustomerId)
                {
                    Logger.Warn($"Wrong customerId has been request: {customerId}", ex);
                    return BadRequest($"Wrong customerId has been request: {customerId}");
                }
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Some error occured while getting customerId {customerId}");
                throw;
            }
        }


        private object InitCustomer(CustomerModel model)
        {
            return new
            {
                _self = new UrlHelper(Request).Link("Customer", new {customerId = model.Id}),
                orders = new UrlHelper(Request).Link("Orders", new { customerId = model.Id }),
                data = model
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _customerService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}