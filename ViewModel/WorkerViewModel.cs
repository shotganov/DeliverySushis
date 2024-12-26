using DeliverySushi.Table;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DeliverySushi.Model
{
    public class ModelWorker
    {
        /// <summary>
        /// Аутентификация работника по логину и паролю.
        /// </summary>
        public async Task<Worker> AuthenticateAsync(string login, string password)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var worker = await db.Worker.FirstOrDefaultAsync(u => u.login == login && u.password == password);

                    if (worker != null)
                    {
                        Console.WriteLine($"Работник найден: ID={worker.id}, Логин={worker.login}, Роль={worker.position}");
                    }
                    else
                    {
                        Console.WriteLine($"Работник с логином '{login}' не найден или пароль неверен.");
                    }

                    return worker;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Получение работника по ID.
        /// </summary>
        public async Task<Worker> GetWorkerByIdAsync(int id)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var fetchedWorker = await db.Worker.FirstOrDefaultAsync(u => u.id == id);
                    if (fetchedWorker != null)
                    {
                        Console.WriteLine($"Получены данные работника с ID={id}.");
                    }
                    else
                    {
                        Console.WriteLine($"Работник с ID={id} не найден.");
                    }

                    return fetchedWorker;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных работника: {ex.Message}");
                throw;
            }
        }
    }
}