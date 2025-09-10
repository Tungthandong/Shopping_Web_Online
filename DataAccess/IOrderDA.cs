using Shopping_Web.Models;

namespace Shopping_Web.DataAccess
{
    public interface IOrderDA
    {
        List<Order> GetOrders();
        string UpdateOrderStatus(int oid, string status);
        void AddOrder(Order order);
        void AddOrderDetail(int oid, List<Cart> cart);
        List<Order> GetOrdersByUser(string username);
        void CancelOrder(int oid);
        List<OrderDetail> getDetailsByOrderId(int oid);
    }
}
