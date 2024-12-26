using DeliverySushi.Model;
using DeliverySushi.Table;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DeliverySushi.ViewModel
{
    public class WorkerViewModel : INotifyPropertyChanged
    {
        private Worker worker;
        private int workerId;

        private ModelWorker modelWorker;

        public WorkerViewModel()
        {
            modelWorker = new ModelWorker();
        }

        public Worker Worker
        {
            get => worker;
            set
            {
                worker = value;
                OnPropertyChanged();
            }
        }

        public void SetId(int id)
        {
            if (id != workerId)
            {
                workerId = id;
                //LoadWorker(workerId); // Загружаем данные нового работника
            }
        }

        /// <summary>
        /// Аутентификация работника по логину и паролю.
        /// </summary>
        public async Task<Worker> AuthenticateAsync(string login, string password)
        {
            try
            {
                var worker = await modelWorker.AuthenticateAsync(login, password);
                if (worker != null)
                {
                    workerId = worker.id;
                }
                return worker;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка аутентификации: {ex.Message}");
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
                var worker = await modelWorker.GetWorkerByIdAsync(id);
                if (worker != null)
                {
                    Worker = worker;
                }
                return worker;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных работника: {ex.Message}");
                throw;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}