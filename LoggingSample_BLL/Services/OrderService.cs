using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_DAL.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingSample_BLL.Services
{
    public class OrderService : IDisposable
    {
        private readonly AppDbContext _context = new AppDbContext();

        public async Task<IEnumerable<OrderModel>> GetOrdersByCustomerIdAsync(int customerId)
        {
            return (await _context.Orders.Where(item => item.CustomerId == customerId).ToListAsync()).Select(item => item?.Map());
        }

        public async Task<OrderModel> GetOrderByIdAsync(int customerId, int orderId)
        {
            return await _context.Orders.SingleOrDefaultAsync(item => item.Id == orderId && item.CustomerId == customerId).ContinueWith(task =>
             {
                 var order = task.Result;

                 return order?.Map();
             });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
