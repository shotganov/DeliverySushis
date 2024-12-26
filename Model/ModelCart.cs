﻿using DeliverySushi.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace DeliverySushi.Model
{
    public class ModelCart
    {
        private int userId { get; set; }
        private int sum;

        public ModelCart(int id)
        {
            userId = id;
            sum = 0;
        }

        public async Task<ObservableCollection<object>> LoadCartItem()
        {
            using (var context = new sushiContext())
            {
                var combinedItems = new ObservableCollection<object>();
                sum = 0; // Сбрасываем сумму

                // Загружаем суши из Cart_Sushi
                List<Cart_Sushi> sushiCartItems = await context.Cart_Sushi
                    .Where(cart => cart.FK_customer_id == userId)
                    .Include(cart => cart.Sushi)
                    .ToListAsync();

                foreach (var cartItem in sushiCartItems)
                {
                    if (cartItem.Sushi != null)
                    {
                        cartItem.Sushi.ImageSource = ImageConverter.LoadImage(cartItem.Sushi.image);
                        cartItem.Sushi.quantity = cartItem.quantity;
                        cartItem.Sushi.price = cartItem.Sushi.price * cartItem.quantity;
                        combinedItems.Add(cartItem.Sushi);
                        sum += cartItem.Sushi.price; // Увеличиваем сумму
                    }
                }

                // Загружаем сеты из Cart_Set
                List<Cart_Set> setCartItems = await context.Cart_Set
                    .Where(cart => cart.FK_customer_id == userId)
                    .Include(cart => cart.Set)
                    .ToListAsync();

                foreach (var cartItem in setCartItems)
                {
                    if (cartItem.Set != null)
                    {
                        cartItem.Set.ImageSource = ImageConverter.LoadImage(cartItem.Set.image);
                        cartItem.Set.quantity = cartItem.quantity;
                        cartItem.Set.price = cartItem.Set.price * cartItem.quantity;
                        combinedItems.Add(cartItem.Set);
                        sum += cartItem.Set.price; // Увеличиваем сумму
                    }
                }

                // Загружаем дополнения из Cart_Addon
                List<Cart_Addon> addonCartItems = await context.Cart_Addon
                    .Where(cart => cart.FK_customer_id == userId)
                    .Include(cart => cart.Addon)
                    .ToListAsync();

                foreach (var cartItem in addonCartItems)
                {
                    if (cartItem.Addon != null)
                    {
                        cartItem.Addon.ImageSource = ImageConverter.LoadImage(cartItem.Addon.image);
                        cartItem.Addon.quantity = cartItem.quantity;
                        cartItem.Addon.price = cartItem.Addon.price * cartItem.quantity;
                        combinedItems.Add(cartItem.Addon);
                        sum += cartItem.Addon.price; // Увеличиваем сумму
                    }
                }

                return combinedItems;
                //CartItems = combinedItems;
            }
        }

        public async Task ChangeQuantity(object item, bool increase)
        {
            using (var context = new sushiContext())
            {
                if (item is Sushi sushi)
                {
                    var cartItem = await context.Cart_Sushi.FirstOrDefaultAsync(c =>
                        c.FK_customer_id == userId && c.FK_sushi_id == sushi.id);

                    if (cartItem != null)
                    {
                        if (increase)
                        {
                            cartItem.quantity += 1;
                        }
                        else if (cartItem.quantity > 1)
                        {
                            cartItem.quantity -= 1;
                        }
                        else
                        {
                            context.Cart_Sushi.Remove(cartItem);
                        }
                    }
                }
                else if (item is Set set)
                {
                    var cartItem = await context.Cart_Set.FirstOrDefaultAsync(c =>
                        c.FK_customer_id == userId && c.FK_set_id == set.id);

                    if (cartItem != null)
                    {
                        if (increase)
                        {
                            cartItem.quantity += 1;
                        }
                        else if (cartItem.quantity > 1)
                        {
                            cartItem.quantity -= 1;
                        }
                        else
                        {
                            context.Cart_Set.Remove(cartItem);
                        }
                    }
                }
                else if (item is Addon addon)
                {
                    var cartItem = await context.Cart_Addon.FirstOrDefaultAsync(c =>
                        c.FK_customer_id == userId && c.FK_addon_id == addon.id);

                    if (cartItem != null)
                    {
                        if (increase)
                        {
                            cartItem.quantity += 1;
                        }
                        else if (cartItem.quantity > 1)
                        {
                            cartItem.quantity -= 1;
                        }
                        else
                        {
                            context.Cart_Addon.Remove(cartItem);
                        }
                    }
                }

                await context.SaveChangesAsync();

            }
        }
        public async Task DeleteCartItems()
        {
            using (var context = new sushiContext())
            {
                // Удаляем элементы из корзины
                var cartSushiItems = await context.Cart_Sushi
                    .Where(cart => cart.FK_customer_id == userId)
                    .ToListAsync();

                var cartSetItems = await context.Cart_Set
                    .Where(cart => cart.FK_customer_id == userId)
                    .ToListAsync();

                var cartAddonItems = await context.Cart_Addon
                    .Where(cart => cart.FK_customer_id == userId)
                    .ToListAsync();

                context.Cart_Sushi.RemoveRange(cartSushiItems);
                context.Cart_Set.RemoveRange(cartSetItems);
                context.Cart_Addon.RemoveRange(cartAddonItems);

                await context.SaveChangesAsync();
            }
        }

        public async void SetId(int id)
        {
            userId = id;
        }
        public async Task<int> GetSum()
        {
            return sum;
        }

    }
}
