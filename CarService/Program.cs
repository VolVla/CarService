using System;
using System.Collections.Generic;
using System.Linq;

namespace CarService
{
    internal class Program
    {
        static void Main()
        {
            WorkAutoService workAutoService = new WorkAutoService();
            workAutoService.WorkService();
        }
    }

    class WorkAutoService
    {
        private Queue<Client> _clients = new Queue<Client>();
        private Storage _storage;
        private int _priceRepair;
        private int _punishment;
        private int _money;

        public WorkAutoService()
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

            while (isWork == true)
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
            int.TryParse(Console.ReadLine(), out int numberDetail);

            if (amountDetails > 0)
            {
                if (numberDetail <= amountDetails)
                {
                    Console.WriteLine($"Цена  детали - {_storage.PriceDetail(numberDetail)}, цена ремонта {_priceRepair}");

                    if (autoClient.ProblemDetail == _storage.GetOneDetail(numberDetail))
                    {
                        int amountCost = _priceRepair + _storage.PriceDetail(numberDetail);
                        Console.WriteLine($"Вы заработали {amountCost} $ за успешную работу");
                        TakeMoney(amountCost);
                        Console.WriteLine("Поздравляю довольный клиент");
                    }
                    else
                    {
                        Console.WriteLine($"Извините мы поставили нету детали в качестве извинения мы выплатим ущерб в виде {_storage.PriceDetail(numberDetail)}$");
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
            _clients.Enqueue(new Client(nameClient,random.Next(_storage.AmountDetails());
        }
    }

    class Client
    {
        public Client(string _name, int numberProblemDetail)
        {
            Name = _name;

        }

        public Detail ProblemDetail { get; private set; }

        public string Name { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя клиента - {Name}, проблема в машине - {ProblemDetail.Name}");
        }
    }

    class Storage
    {
        private List<Detail> _details;

        public Storage()
        {
            _details = new List<Detail>()
            {
                new Glass( 100),
                new Headlights( 20),
                new TurnSignals(40),
                new Tires(120),
                new DoorLock(50)
            };
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

        public Detail GetOneDetail(int numberDetail)
        {
            return _details[numberDetail - 1];
        }

        public Detail CreateProblemDetail(int numberDetail)
        {
            Detail temporary = new Detail() 
            _details[numberDetail]
            return 
        }
    }

    abstract class Detail
    {
        public Detail(int cost)
        {
            Cost = cost;
        }

        public string Name { get; protected set; }
        public string NameProblem { get; protected set; }
        public int Cost { get; private set; }
    }

    class Glass : Detail
    {
        public Glass(int cost) : base(cost)
        {
            Name = "Стекло";
            NameProblem = "Сломано Лобовое Окно";
        }
    }

    class Headlights : Detail
    {
        public Headlights(int cost) : base(cost)
        {
            Name = "Фары";
            NameProblem = "Разбита одна Фара";
        }
    }

    class TurnSignals : Detail
    {
        public TurnSignals( int cost) : base(cost)
        {
            Name = "Поворотники";
            NameProblem = "Отломались Поворотники";
        }
    }

    class Tires : Detail
    {
        public Tires(int cost) : base(cost)
        {
            Name = "Шины";
            NameProblem = "Проколоты Шины";
        }
    }

    class DoorLock : Detail
    {
        public DoorLock(int cost) : base(cost)
        {
            Name = "Дверь";
            NameProblem = "Дверь пытались взломать сломали Замок";
        }
    }
}