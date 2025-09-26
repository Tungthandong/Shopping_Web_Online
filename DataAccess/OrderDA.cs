using Microsoft.EntityFrameworkCore;
using Shopping_Web.Models;

namespace Shopping_Web.DataAccess
{
    public class OrderDA : IOrderDA
    {
        YugiohCardShopContext context = new YugiohCardShopContext();
        public List<Order> GetOrders()
        {
            return context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).OrderByDescending(o => o.OrderDate).ToList();
        }

        public string UpdateOrderStatus(int oid, string status)
        {
            var order = context.Orders.FirstOrDefault(o => o.OrderId == oid);
            if (order==null)
            {
                return "Order not exist";
            }
            order.OrderStatus = status;
            context.SaveChanges();
            return "Update Successfully";
        }

        public void AddOrder(Order order)
        {
            context.Add(order);
            context.SaveChanges();
        }

        public void AddOrderDetail(int oid, List<Cart> cart)
        {
            foreach (var c in cart)
            {
                var detail = new OrderDetail()
                {
                    OrderId = oid,
                    ProductId = c.Product.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Quantity*(int)c.Product.UnitPrice,
                    VariantId = c.VariantId
                };
                context.OrderDetails.Add(detail);
            }
            context.SaveChanges();
        }

        public List<Order> GetOrdersByUser(string username)
        {
            return context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).Include(o => o.OrderDetails).ThenInclude(od => od.Variant).OrderByDescending(o => o.OrderDate).Where(o => o.Username==username).ToList();
        }

        public void CancelOrder(int oid)
        {
            var order = context.Orders.FirstOrDefault(o => o.OrderId == oid);
            order.OrderStatus = "Cancelled";
            context.SaveChanges();
        }

        public List<OrderDetail> getDetailsByOrderId(int oid)
        {
            return context.OrderDetails.Include(od => od.Order).Where(od => od.Order.OrderId == oid).ToList();
        }
    }
}
