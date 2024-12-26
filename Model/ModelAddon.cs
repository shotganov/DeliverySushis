using DeliverySushi.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliverySushi.Model
{
    internal class ModelAddon
    {
        private int userId;
        public ModelAddon()
        {

        }

        public async void SetId(int id)
        {
            userId = id;
        }
        public async Task AddAddonToCart(Addon selectedSushi)
        {
            using (var context = new sushiContext())
            {
                var cartItem = await context.Cart_Addon.FirstOrDefaultAsync(c =>
                    c.FK_addon_id == selectedSushi.id && c.FK_customer_id == userId);

                if (cartItem != null)
                {
                    cartItem.quantity += 1;
                }
                else
                {
                    var newCartItem = new Cart_Addon
                    {
                        FK_addon_id = selectedSushi.id,
                        FK_customer_id = userId,
                        quantity = 1
                    };

                    context.Cart_Addon.Add(newCartItem);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Addon>> GetAddons()
        {
            using (var context = new sushiContext())
            {

                var addonList = await context.Addon.ToListAsync();

                foreach (var addon in addonList)
                {
                    addon.ImageSource = ImageConverter.LoadImage(addon.image);
                }

                return addonList;
            }
        }
    }
}
