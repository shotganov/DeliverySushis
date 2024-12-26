using DeliverySushi.Table;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverySushi.Model
{
    public class ModelUser
    {
        /// <summary>
        /// Сохраняет изменения пользователя в базе данных.
        /// </summary>
        public async Task SaveUserAsync(User user, string tempPassword)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var existingUser = await db.User.FindAsync(user.id);
                    if (existingUser != null)
                    {
                        existingUser.login = user.login;
                        existingUser.email = user.email;
                        existingUser.password = tempPassword;
                        existingUser.phone = user.phone;
                        existingUser.adress = user.adress;

                        await db.SaveChangesAsync();
                        Console.WriteLine("Данные пользователя успешно обновлены.");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с ID={user.id} не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Загружает пользователя по ID.
        /// </summary>
        public async Task<User> LoadUserAsync(int id)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var fetchedUser = await db.User.FirstOrDefaultAsync(u => u.id == id);
                    if (fetchedUser != null)
                    {
                        Console.WriteLine($"Пользователь с ID={id} успешно загружен: {fetchedUser.login}");
                        return fetchedUser;
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с ID={id} не найден.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки пользователя: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Аутентификация пользователя по логину и паролю.
        /// </summary>
        public async Task<User> AuthenticateAsync(string login, string password)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var user = await db.User.FirstOrDefaultAsync(u => u.login == login && u.password == password);

                    if (user != null)
                    {
                        Console.WriteLine($"Пользователь найден: ID={user.id}, Логин={user.login}");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с логином '{login}' не найден или пароль неверен.");
                    }

                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        public async Task<int> RegisterAsync(string login, string password, string email, long phone)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    bool userExists = await db.User.AnyAsync(u => u.login == login || u.email == email);

                    if (userExists)
                    {
                        Console.WriteLine($"Пользователь с логином '{login}' или email '{email}' уже существует.");
                        return 0;
                    }

                    var newUser = new User
                    {
                        login = login,
                        password = password,
                        email = email,
                        phone = phone,
                        adress = ""
                    };

                    db.User.Add(newUser);
                    await db.SaveChangesAsync();

                    var createdUser = await db.User.FirstOrDefaultAsync(u => u.login == login && u.password == password);

                    if (createdUser != null)
                    {
                        Console.WriteLine($"Пользователь зарегистрирован: ID={createdUser.id}");
                        return createdUser.id;
                    }

                    Console.WriteLine("Ошибка при регистрации: пользователь не найден после создания.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка регистрации: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Получение пользователя по ID.
        /// </summary>
        public async Task<User> GetCustomerByIdAsync(int id)
        {
            try
            {
                using (var db = new sushiContext())
                {
                    var fetchedUser = await db.User.FirstOrDefaultAsync(u => u.id == id);
                    if (fetchedUser != null)
                    {
                        Console.WriteLine($"Получены данные пользователя с ID={id}.");
                    }
                    else
                    {
                        Console.WriteLine($"Пользователь с ID={id} не найден.");
                    }

                    return fetchedUser;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных пользователя: {ex.Message}");
                throw;
            }
        }
    }
}