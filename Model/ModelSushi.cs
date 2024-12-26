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
    public class ModelSushi
    {
        private int userId;
        public ModelSushi()
        {
           
        }

        public async void SetId(int id)
        {
            userId = id;
        }

        public async Task AddSushiToCart(Sushi selectedSushi)
        {
            using (var context = new sushiContext())
            {
                var cartItem = await context.Cart_Sushi.FirstOrDefaultAsync(c =>
                    c.FK_sushi_id == selectedSushi.id && c.FK_customer_id == userId);

                if (cartItem != null)
                {
                    cartItem.quantity += 1;
                }
                else
                {
                    var newCartItem = new Cart_Sushi
                    {
                        FK_sushi_id = selectedSushi.id,
                        FK_customer_id = userId,
                        quantity = 1
                    };

                    context.Cart_Sushi.Add(newCartItem);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Sushi>> GetSushis()
        {
            using (var context = new sushiContext())
            {

                var sushiList = await context.Sushi.ToListAsync();

                foreach (var sushi in sushiList)
                {
                    sushi.ImageSource = ImageConverter.LoadImage(sushi.image);
                }

                return sushiList;
            }
        }
    }
}
