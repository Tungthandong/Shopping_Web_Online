using Shopping_Web.Models;
using Shopping_Web.Repositories;

namespace Shopping_Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetOrders()
        {
            return _orderRepository.GetOrders();
        }

        public string UpdateOrderStatus(int oid, string status)
        {
            return _orderRepository.UpdateOrderStatus(oid, status);
        }

        public void AddOrder(Order order)
        {
            _orderRepository.AddOrder(order);
        }

        public void AddOrderDetail(int oid, List<Cart> cart)
        {
            _orderRepository.AddOrderDetail(oid, cart);
        }

        public List<Order> GetOrdersByUser(string username)
        {
            return _orderRepository.GetOrdersByUser(username);
        }

        public void CancelOrder(int oid)
        {
            _orderRepository.CancelOrder(oid);
        }

        public List<OrderDetail> getDetailsByOrderId(int oid)
        {
            return _orderRepository.getDetailsByOrderId(oid);
        }
    }
}
