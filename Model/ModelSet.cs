using DeliverySushi.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliverySushi.Model
{
    public class ModelSet
    {
        private int userId;
        public ModelSet()
        {

        }

        public async void SetId(int id)
        {
            userId = id;
        }

        public async Task AddSetToCart(Set selectedSushi)
        {
            using (var context = new sushiContext())
            {
                var cartItem = await context.Cart_Set.FirstOrDefaultAsync(c =>
                    c.FK_set_id == selectedSushi.id && c.FK_customer_id == userId);

                if (cartItem != null)
                {
                    cartItem.quantity += 1;
                }
                else
                {
                    var newCartItem = new Cart_Set
                    {
                        FK_set_id = selectedSushi.id,
                        FK_customer_id = userId,
                        quantity = 1
                    };

                    context.Cart_Set.Add(newCartItem);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Set>> GetSets()
        {
            using (var context = new sushiContext())
            {

                var setList = await context.Set.ToListAsync();

                foreach (var set in setList)
                {
                    set.ImageSource = ImageConverter.LoadImage(set.image);
                }

                return setList;
            }
        }
    }
}
