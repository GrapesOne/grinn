
using Entity;

namespace Interactor
{
    public class OrderInteractor
    {
        private OrderEntity order;
        public OrderEntity GetOrder() => order ?? new OrderEntity();
        public void SetOrder(OrderEntity orderEntity) => order = orderEntity;
    }
}
