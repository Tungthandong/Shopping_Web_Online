using Shopping_Web.DataAccess;
using Shopping_Web.Models;

namespace Shopping_Web.Services
{
    public class OrderService : IOrderService
    {
        IOrderDA _orderDA;
        public OrderService(IOrderDA orderDA)
        {
            _orderDA = orderDA;
        }
        public List<Order> GetOrders()
        {
            return _orderDA.GetOrders();
        }

        public string UpdateOrderStatus(int oid, string status)
        {
            return _orderDA.UpdateOrderStatus(oid, status);
        }

        public void AddOrder(Order order)
        {
            _orderDA.AddOrder(order);
        }

        public void AddOrderDetail(int oid, List<Cart> cart)
        {
            _orderDA.AddOrderDetail(oid, cart);
        }

        public List<Order> GetOrdersByUser(string username)
        {
            return _orderDA.GetOrdersByUser(username);
        }

        public void CancelOrder(int oid)
        {
            _orderDA.CancelOrder(oid);
        }

        public List<OrderDetail> getDetailsByOrderId(int oid)
        {
            return _orderDA.getDetailsByOrderId(oid);
        }
    }
}
