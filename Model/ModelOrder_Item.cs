using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using DeliverySushi.Table;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace DeliverySushi.Model
{
    public class ModelOrder_Item
    {
        private int userId;
        public ModelOrder_Item(int id) {

            userId = id;

        }

        public async Task CreateOrder_Items(ObservableCollection<object> CartItems, Order newOrder)
        {
            using (var context = new sushiContext())
            {
                
                foreach (var item in CartItems)
                {
                    Trace.WriteLine(newOrder.FK_order_id.ToString());
                    if (item is Sushi sushi)
                    {
                        Trace.WriteLine($"Sushi ID: {sushi.id}, Quantity: {sushi.quantity}, Price: {sushi.price}");
                        var orderItem = new Order_Item
                        {
                            FK_order_id = newOrder.id,
                            FK_product_id = sushi.id,
                            product_type = "sushi",
                            quantity = sushi.quantity,
                            price = sushi.price
                        };
                        context.Order_Item.Add(orderItem);
                    }
                    else if (item is Set set)
                    {
                        Trace.WriteLine($"Set ID: {set.id}, Quantity: {set.quantity}, Price: {set.price}");
                        var orderItem = new Order_Item
                        {
                            FK_order_id = newOrder.id,
                            FK_product_id = set.id,
                            product_type = "set",
                            quantity = set.quantity,
                            price = set.price
                        };
                        context.Order_Item.Add(orderItem);
                    }
                    else if (item is Addon addon)
                    {
                        Trace.WriteLine($"Addon ID: {addon.id}, Quantity: {addon.quantity}, Price: {addon.price}");
                        var orderItem = new Order_Item
                        {
                            FK_order_id = newOrder.id,
                            FK_product_id = addon.id,
                            product_type = "addon",
                            quantity = addon.quantity,
                            price = addon.price
                        };
                        context.Order_Item.Add(orderItem);
                    }
                    else
                    {
                        Trace.WriteLine("Неизвестный тип элемента в корзине.");
                    }

                }
                await context.SaveChangesAsync();
            }
        } 

    }
}
