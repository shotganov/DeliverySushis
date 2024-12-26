using DeliverySushi.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeliverySushi.Model
{
    public class ModelCourier
    {
        private int courierId { get; set; }

        public ModelCourier(int id)
        {
            courierId = id;
        }

        public async Task<List<Order>> GetWaitOrders()
        {
            using (var context = new sushiContext())
            {
                var availableOrders = await context.Database
                 .SqlQuery<Order>("SELECT * FROM [Order] WHERE FK_worker_id IS NULL AND status = 'Принят'")
                 .ToListAsync();


                await loadOrder_items(availableOrders);

                return availableOrders;

            }
        }

        public async Task<List<Order>> GetInDeliveryOrders()
        {
            using (var context = new sushiContext())
            {
                var availableOrders = await context.Order
                    .Where(o => o.status == "В пути" && o.FK_worker_id == courierId)
                    .Include(o => o.Order_Item) // Явное включение связанных данных
                    .ToListAsync();

                await loadOrder_items(availableOrders);

                availableOrders.Reverse();

                return availableOrders;
            }
        }

        public async Task<List<Order>> GetDeliveredOrders()
        {
            using (var context = new sushiContext())
            {
                var DeliveredOrders = await context.Order.Where(o => o.FK_worker_id == courierId && o.status == "Доставлен").Include(o => o.Order_Item).ToListAsync();

                await loadOrder_items(DeliveredOrders);

                DeliveredOrders.Reverse();
               
                return DeliveredOrders;
            }
        }

        public async Task AcceptDeliveryOrder(Order order)
        {
            using (var context = new sushiContext())
            {
                var dbOrder = await context.Order.FirstOrDefaultAsync(c => c.id == order.id);

                if (dbOrder != null)
                {
                    // Обновляем FK_courier_id
                    dbOrder.FK_worker_id = courierId;
                    dbOrder.status = "В пути";

                    await context.SaveChangesAsync();

                }
            }
        }

        public async Task CancelDeliveryOrder(Order order)
        {
            using (var context = new sushiContext())
            {
                // Находим заказ в базе данных
                var dbOrder = await context.Order.FirstOrDefaultAsync(c => c.id == order.id);
                if (dbOrder != null)
                {
                    dbOrder.status = "Отменен";

                    await context.SaveChangesAsync();

                }
            }
        }

        public async Task ConfirmDeliveryOrder(Order order)
        {
            using (var context = new sushiContext())
            {
                // Находим заказ в базе данных
                var dbOrder = await context.Order.FirstOrDefaultAsync(c => c.id == order.id);
                if (dbOrder != null)
                {

                    dbOrder.status = "Доставлен";

                    // Сохраняем изменения
                    await context.SaveChangesAsync();

                }
            }
        }
        public async Task<List<Order>> GetCancelledOrders()
        {
            using (var context = new sushiContext())
            {
                var CanceledOrders = await context.Order.Where(o => o.FK_worker_id == courierId && o.status == "Отменен").Include(o => o.Order_Item).ToListAsync();

                await loadOrder_items(CanceledOrders);

                CanceledOrders.Reverse();

                return CanceledOrders;

            }
        }
        private async Task loadOrder_items(List<Order> orders)
        {
            using (var context = new sushiContext())
            {
                foreach (var order in orders)
                {
                    // Загружаем элементы заказа
                    var orderItems = await context.Order_Item
                        .Where(oi => oi.FK_order_id == order.id)
                        .ToListAsync();

                    foreach (var item in orderItems)
                    {

                        // Определяем тип продукта и связываем данные
                        if (item.product_type == "sushi")
                        {
                            var sushi = await context.Sushi.FindAsync(item.FK_product_id);
                            if (sushi != null)
                            {
                                item.name = "Роллы " + sushi.name;
                                item.ImageSource = ImageConverter.LoadImage(sushi.image);
                            }
                        }
                        else if (item.product_type == "set")
                        {
                            var set = await context.Set.FindAsync(item.FK_product_id);
                            if (set != null)
                            {
                                item.name = "Сет " + set.name;
                                item.ImageSource = ImageConverter.LoadImage(set.image);
                            }
                        }
                        else if (item.product_type == "addon")
                        {
                            var addon = await context.Addon.FindAsync(item.FK_product_id);
                            if (addon != null)
                            {
                                item.name = "Дополнения " + addon.name;
                                item.ImageSource = ImageConverter.LoadImage(addon.image);
                            }
                        }


                    }

                    order.Order_Item = orderItems;

                }
            }
        }
    }
}
