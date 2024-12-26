using DeliverySushi.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeliverySushi.Model
{
    public class ModelOrder
    {
        public int userId { get; set; }

        public ModelOrder(int id)
        {
           userId = id;
        }

        public async Task<Order> CreateNewOrder(int Sum, string name, string adress, string phone)
        {

            using (var context = new sushiContext())
            {
                var newOrder = new Order
                {
                    FK_customer_id = userId,
                    order_date = DateTime.Now,
                    status = "В обработке",
                    total_price = Sum,
                    //FK_order_id = numOrder + 1,
                    adress = adress,
                    name = name,
                    phone = long.Parse(phone)
                };
            
                context.Order.Add(newOrder);
                await context.SaveChangesAsync();

                return newOrder;
            }

        }
      

        public async Task<List<Order>> GetUserOrders()
        {
            using (var context = new sushiContext())
            {
                // Загружаем заказы пользователя
                var userOrders = await context.Order
                    .Where(o => o.FK_customer_id == userId)
                    .Include(o => o.Order_Item) // Загружаем связанные Order_Item
                    .ToListAsync();

                foreach (var order in userOrders)
                {
                    // Загружаем элементы заказа отдельно
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
                                item.name = addon.name;
                                item.ImageSource = ImageConverter.LoadImage(addon.image);
                            }
                        }
                    }

                    // Обновляем элементы заказа в заказе
                    order.Order_Item = orderItems;
                }
              
                userOrders.Reverse();

                return userOrders;
            }
        }
    }
}
