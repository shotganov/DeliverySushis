using DeliverySushi.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace DeliverySushi.Model
{
    public class ModelManager
    {

        public ModelManager() { 

        }

        public async Task ConfirmOrder(Order order)
        {
   
            using (var context = new sushiContext())
            {
                var confirmOrder = await context.Order
                    .Where(o => o.status == "В обработке" && o.id == order.id)
                    .Include(o => o.Order_Item) // Явное включение связанных данных
                    .FirstAsync();

                confirmOrder.status = "Принят";

                await context.SaveChangesAsync();


            }
        }

        public async Task<List<Order>> GetConfrirmListOrders()
        {
            using (var context = new sushiContext())
            {
                var availableOrders = await context.Order
                    .Where(o => o.status == "В обработке")
                    .Include(o => o.Order_Item) // Явное включение связанных данных
                    .ToListAsync();

                await loadOrder_items(availableOrders);


                availableOrders.Reverse();

                return availableOrders;
            }
        }

        public async Task<List<Order>> GetHistoryListOrders()
        {
            using (var context = new sushiContext())
            {
                var historyOrders = await context.Order
                    .Where(o => o.status != "В обработке" && o.status != "Не принят")
                    .Include(o => o.Order_Item) // Явное включение связанных данных
                    .ToListAsync();

                await loadOrder_items(historyOrders);


                historyOrders.Reverse();

                return historyOrders;
            }
        }

        public double GetProductPrice(string productName)
        {
            var parts = productName.Split(new[] { ' ' }, 2); // Разделяем по первому пробелу
            if (parts.Length < 2)
                return 0;

            string productType = parts[0]; // Тип товара (Роллы, Сет, Дополнения)
            string name = parts[1]; // Название товара

            using (var context = new sushiContext())
            {
                switch (productType)
                {
                    case "Роллы":
                        var sushi = context.Sushi.FirstOrDefault(s => s.name == name);
                        return sushi?.price ?? 0;

                    case "Сет":
                        var set = context.Set.FirstOrDefault(s => s.name == name);
                        return set?.price ?? 0;

                    case "Дополнения":
                        var addon = context.Addon.FirstOrDefault(a => a.name == name);
                        return addon?.price ?? 0;

                    default:
                        return 0;
                }
            }
        }
        public async Task<Dictionary<string, int>> GetProductData(DateTime SelectedDate)
        {
            using (var context = new sushiContext())
            {
                // Загружаем заказы для выбранной даты
                var orders = context.Order
                .Where(o => o.order_date.HasValue && o.status == "Доставлен") // Фильтруем по наличию даты
                .Include(o => o.Order_Item) // Загружаем связанные Order_Item
                .ToList(); // Выполняем запрос к базе данных

                // Фильтруем заказы по выбранной дате в памяти
                var filteredOrders = orders
                    .Where(o => o.order_date.Value.Date == SelectedDate)
                    .ToList();


                // Создаем словарь для хранения данных для диаграммы
                var productData = new Dictionary<string, int>();

                foreach (var order in filteredOrders)
                {
                    foreach (var item in order.Order_Item)
                    {
                        // Определяем тип продукта и связываем данные
                        string productName = "Неизвестный продукт";

                        if (item.product_type == "sushi")
                        {
                            var sushi = context.Sushi.Find(item.FK_product_id);
                            if (sushi != null)
                            {
                                productName = "Роллы " + sushi.name;
                            }
                        }
                        else if (item.product_type == "set")
                        {
                            var set = context.Set.Find(item.FK_product_id);
                            if (set != null)
                            {
                                productName = "Сет " + set.name;
                            }
                        }
                        else if (item.product_type == "addon")
                        {
                            var addon = context.Addon.Find(item.FK_product_id);
                            if (addon != null)
                            {
                                productName = "Дополнения " + addon.name;
                            }
                        }

                        // Добавляем данные в словарь
                        if (productData.ContainsKey(productName))
                        {
                            productData[productName] += item.quantity ?? 0; // Суммируем количество
                        }
                        else
                        {
                            productData[productName] = item.quantity ?? 0;
                        }
                    }
                }
                return productData;
            }
        }

        public async Task<List<DateTime>> GetAvaliableDates()
        {
            using (var context = new sushiContext())
            {
                // Загружаем даты заказов из базы данных
                var dates = context.Order
                    .Where(o => o.order_date.HasValue)
                    .Select(o => o.order_date.Value)
                    .ToList();

                // Преобразуем даты в компоненты Date
                var availableDate = dates
                    .Select(d => d.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                return availableDate;
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
