using Microsoft.EntityFrameworkCore;
using Shopping_Web.Data;
using Shopping_Web.Models;

namespace Shopping_Web.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly YugiohCardShopContext _context;

        public OrderRepository(YugiohCardShopContext context)
        {
            _context = context;
        }

        public List<Order> GetOrders()
        {
            return _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public string UpdateOrderStatus(int oid, string status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == oid);
            if (order == null) return "Order not exist";

            order.OrderStatus = status;
            _context.SaveChanges();
            return "Update Successfully";
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void AddOrderDetail(int oid, List<Cart> cart)
        {
            foreach (var c in cart)
            {
                var detail = new OrderDetail
                {
                    OrderId   = oid,
                    ProductId = c.Product.ProductId,
                    Quantity  = c.Quantity,
                    Price     = c.Quantity * (int)c.Product.UnitPrice,
                    VariantId = c.VariantId
                };
                _context.OrderDetails.Add(detail);
            }
            _context.SaveChanges();
        }

        public List<Order> GetOrdersByUser(string username)
        {
            return _context.Orders
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Variant)
                .OrderByDescending(o => o.OrderDate)
                .Where(o => o.Username == username)
                .ToList();
        }

        public void CancelOrder(int oid)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == oid);
            if (order == null) return;

            order.OrderStatus = "Cancelled";
            _context.SaveChanges();
        }

        public List<OrderDetail> getDetailsByOrderId(int oid)
        {
            return _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.Order.OrderId == oid)
                .ToList();
        }
    }
}
