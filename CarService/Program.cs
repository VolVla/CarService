using System;
using System.Collections.Generic;
using System.Linq;

namespace CarService
{
    internal class Program
    {
        static void Main()
        {
            AutoService workAutoService = new AutoService();
            workAutoService.WorkService();
        }
    }

    class AutoService
    {
        private Queue<Client> _clients = new Queue<Client>();
        private Storage _storage;
        private int _priceRepair;
        private int _punishment;
        private int _money;

        public AutoService()
        {
            _storage = new Storage();
            _money = 1000;
            _priceRepair = 100;
            _punishment = 20;
        }

        public void WorkService()
        {
            const int CommandClient = 1;
            const int CommandStorage = 2;

            bool isWork = true;
            ConsoleKey exitButton = ConsoleKey.Enter;

            Console.WriteLine("Для начало работы автосервиса нажмите на любую клавишу");
            Console.ReadKey();
            Console.Clear();

            while (isWork)
            {
                CreateClient();
                ShowBalance();
                Console.WriteLine($"Для начала работы с клиентом напишите {CommandClient},\nДля того посмотреть какие есть детали на складе {CommandStorage}.");
                int.TryParse(Console.ReadLine(), out int result);

                switch (result)
                {
                    case CommandClient:
                        ServiceClient(_clients.Dequeue());
                        break;

                    case CommandStorage:
                        ShowDetailsStorage();
                        break;

                    default:
                        Console.WriteLine("Выбрана не существующая команда");
                        break;
                }

                Console.WriteLine($"Вы хотите выйти из программы?Нажмите {exitButton}.\nДля продолжение работы нажмите любую другую клавишу");

                if (Console.ReadKey().Key == exitButton)
                {
                    isWork = false;
                    Console.WriteLine("Вы вышли из программы");
                }
            }
        }

        private void ServiceClient(Client autoClient)
        {
            autoClient.ShowInfo();
            RepairCar(autoClient);
        }

        private void ShowBalance()
        {
            Console.WriteLine($"Ваш баланс на предприятии {_money} $");
        }

        private void ShowDetailsStorage()
        {
            _storage.ShowDetails();
        }

        private void GiveMoney(int money)
        {
            _money -= money;
        }

        private void TakeMoney(int amountCost)
        {
            _money += amountCost;
        }

        private void RepairCar(Client autoClient)
        {
            int amountDetails = _storage.AmountDetails();

            ShowDetailsStorage();
            Console.WriteLine("Выберете деталь  для починки авто");
            bool isCorrect = int.TryParse(Console.ReadLine(), out int numberDetail);

            if (amountDetails > 0 & isCorrect == true)
            {
                if (numberDetail <= amountDetails)
                {
                    int costDetail = _storage.PriceDetail(numberDetail);
                    Console.WriteLine($"Цена  детали - {costDetail}, цена ремонта {_priceRepair}");

                    if (autoClient.GiveDetail().Name == _storage.GetDetail(numberDetail).NameBroken)
                    {
                        autoClient.GetNewDetail(_storage.GetDetail(numberDetail));
                        int amountCost = _priceRepair + costDetail;
                        Console.WriteLine($"Вы заработали {amountCost} $ за успешную работу");
                        TakeMoney(amountCost);
                        Console.WriteLine("Поздравляю довольный клиент");
                    }
                    else
                    {
                        Console.WriteLine($"Извините мы поставили нету детали в качестве извинения мы выплатим ущерб в виде {costDetail}$");
                        GiveMoney(_storage.PriceDetail(numberDetail));
                    }

                    _storage.RemoveDetail(numberDetail);
                }
                else
                {
                    Console.WriteLine("Данной детали не существует");
                }
            }
            else
            {
                Console.WriteLine($"Извините у нас нету нужной детали мы выплатим штраф {_punishment}");
                GiveMoney(_punishment);
                Console.WriteLine("Для продолжения нажмите на любую клавишу");
                Console.ReadKey();
            }
        }

        private void CreateClient()
        {
            Random random = new Random();
            List<string> namesClients = new List<string>() { "Саня", "Вова", "Артем", "Вика", "Аня" };
            string nameClient = namesClients[random.Next(namesClients.Count)];
            int numberDetail = random.Next(_storage.AmountDetails());
            _clients.Enqueue(new Client(nameClient, _storage.GetNameBrokenDetail(numberDetail)));
        }
    }

    class Client
    {
        private Detail _Detail;
        private int _costBrokenDetail;

        public Client(string name, string nameBrokenDetail)
        {
            _costBrokenDetail = 0;
            Name = name;
            _Detail = new Detail(nameBrokenDetail, _costBrokenDetail, nameBrokenDetail);
        }

        public string Name { get; private set; }

        public Detail GiveDetail()
        {
            return _Detail;
        }

        public void GetNewDetail(Detail detail)
        {
            _Detail = detail;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя клиента - {Name}, проблема в машине - {_Detail.Name}");
        }
    }

    class Storage
    {
        private List<Detail> _details;
        private List<string> _nameBrokenDetails;

        public Storage()
        {
            _nameBrokenDetails = new List<string>()
            {
                "Сломано Лобовое Окно",
                "Разбита одна Фара",
                "Отломались Поворотники",
                "Проколоты Шины",
                "Дверь пытались взломать сломали Замок"
            };
            _details = new List<Detail>()
            {
                new Detail("Стекло",100,_nameBrokenDetails[0]),
                new Detail("Фары",20, _nameBrokenDetails[1]),
                new Detail("Поворотники",40, _nameBrokenDetails[2]),
                new Detail("Шины", 120, _nameBrokenDetails[3]),
                new Detail("Замок",50, _nameBrokenDetails[4])
            };
        }

        public string GetNameBrokenDetail(int numberDetail)
        {
            return _nameBrokenDetails[numberDetail];
        }

        public void ShowDetails()
        {
            if (_details.Count > 0)
            {
                for (int i = 0; i < _details.Count; i++)
                {
                    Console.WriteLine($"Номер {i + 1} лежащей на складе детали, тип детали {_details[i].Name}");
                }
            }
            else
            {
                Console.WriteLine("На складе нет деталей");
            }
        }

        public void RemoveDetail(int numberDetail)
        {
            _details.RemoveAt(numberDetail - 1);
        }

        public int AmountDetails()
        {
            return _details.Count();
        }

        public int PriceDetail(int numberDetail)
        {
            return _details[numberDetail - 1].Cost;
        }

        public Detail GetDetail(int numberDetail)
        {
            return _details[numberDetail - 1];
        }
    }

    class Detail
    {
        public Detail(string name, int cost, string nameBroken)
        {
            Name = name;
            Cost = cost;
            NameBroken = nameBroken;
        }

        public string NameBroken { get; private set; }
        public string Name { get; private set; }
        public int Cost { get; private set; }
    }
}