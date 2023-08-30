using System;
using System.Collections.Generic;
using System.Linq;

namespace CarService
{
    internal class Program
    {
        static void Main()
        {
            const int CommandClient = 1;
            const int CommandStorage = 2;
            const int CommandDetails = 3;
            const int CommandExit = 4;

            bool isWork = true;
            Catalog catalog = new Catalog();
            AutoService autoService = new AutoService(catalog);
            Console.WriteLine("Для начало работы автосервиса нажмите на любую клавишу");
            Console.ReadKey();
            Console.Clear();

            while (isWork)
            {
                autoService.ShowBalance();
                Console.WriteLine($"Для начала работы с клиентом напишите {CommandClient},\nДля того посмотреть какие есть детали на складе {CommandStorage}\nДля покупки деталей {CommandDetails}\nВы хотите выйти из программы?Нажмите {CommandExit}.");
                int.TryParse(Console.ReadLine(), out int result);

                switch (result)
                {
                    case CommandClient:
                        CreateClient();
                        break;
                    case CommandStorage:
                        autoService.ShowDetailsStorage();
                        break;
                    case CommandDetails:
                        autoService.BuyDetailsStorage();
                        break;
                    case CommandExit:
                        isWork = false;
                        break;
                    default:
                        Console.WriteLine("Выбрана не существующая команда");
                        break;
                }
            }

            Console.WriteLine("Вы вышли из программы");

            void CreateClient()
            {
                Random random = new Random();
                List<string> namesClients = new List<string>() { "Саня", "Вова", "Артем", "Вика", "Аня" };
                int minimumValue = 0;
                int numberNameClient = random.Next(minimumValue, namesClients.Count + 1);
                string nameClient = namesClients[numberNameClient];
                int numberDetailClient = random.Next(minimumValue, catalog.ReturnListDetail().Count + 1);
                autoService.ServiceClient(new Client(nameClient, catalog.ReturnListDetail()[numberDetailClient]));
            }
        }
    }

    class AutoService
    {
        private Storage _storage;
        private int _priceRepair;
        private int _punishment;
        private int _money;

        public AutoService(Catalog catalog)
        {
            _storage = new Storage(catalog);
            _money = 1000;
            _priceRepair = 100;
            _punishment = 20;
        }

        public void ServiceClient(Client autoClient)
        {
            autoClient.ShowInfo();
            RemonteAuto(autoClient);
        }

        public void BuyDetailsStorage()
        {
            _storage.BuyDetail(_money);
        }

        public void ShowBalance()
        {
            Console.WriteLine($"Ваш баланс на предприятии {_money} $");
        }

        public void ShowDetailsStorage()
        {
            _storage.ShowDetails();
        }

        private void TakeMoney(int money)
        {
            _money -= money;
        }

        private void RemonteAuto(Client autoClient)
        {
            ShowDetailsStorage();
            Console.WriteLine("Выберете деталь  для починки авто");
            int.TryParse(Console.ReadLine(), out int numberDetail);

            if (_storage.AmountDetails() > 0)
            {
                Console.WriteLine($"Цена  детали - {_storage.CostDetail(numberDetail)}, цена ремонта {_priceRepair}");

                if (autoClient.ReplaceDetail(_storage.GetDetail(numberDetail)))
                {
                    int amountCost = _priceRepair + _storage.CostDetail(numberDetail);
                    Console.WriteLine($"Вы заработали {amountCost} $ за успешную работу");
                    _money += amountCost;
                    Console.WriteLine("Поздравляю довольный клиент");
                }
                else
                {
                    Console.WriteLine($"Извините мы поставили нету детали в качестве извинения мы выплатим ущерб в виде {_storage.CostDetail(numberDetail)}$");
                    TakeMoney(_storage.CostDetail(numberDetail));
                }

                _storage.RemoveDetail(numberDetail);
            }
            else
            {
                Console.WriteLine($"Извините у нас нету нужной детали мы выплатим штраф {_punishment}");
                TakeMoney(_punishment);
                Console.WriteLine("Для продолжения нажмите на любую клавишу");
                Console.ReadKey();
            }
        }
    }

    class Client
    {
        private bool _isCorrectReplaceDetail;
        private Detail _problemDetail;

        public Client(string _name, Detail detail)
        {
            Name = _name;
            _problemDetail = detail;
        }

        public string Name { get; private set; }

        public bool ReplaceDetail(Detail detail)
        {
            if (detail.NameProblem == _problemDetail.NameProblem)
            {
                _isCorrectReplaceDetail = true;
                Console.WriteLine("Автомеханик заменил на правильную деталь");
            }
            else
            {
                _isCorrectReplaceDetail = false;
                Console.WriteLine("Автомеханик заменил на не правильную деталь");
            }

            _problemDetail = detail;

            return _isCorrectReplaceDetail;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя клиента - {Name}, проблема в машине - {_problemDetail.NameProblem}");
        }
    }

    class Catalog
    {
        private List<Detail> _catalogDetails = new List<Detail>();

        public Catalog()
        {
            _catalogDetails.Add(new Glass("Стекло", 100, "Сломано Лобовое Окно"));
            _catalogDetails.Add(new Headlights("Фары", 20, "Разбита одна Фара"));
            _catalogDetails.Add(new TurnSignals("Поворотники", 40, "Отломались Поворотники"));
            _catalogDetails.Add(new Tires("Шины", 120, "Проколоты Шины"));
            _catalogDetails.Add(new DoorLock("Замок", 50, "Дверь пытались взломать сломали Замок"));
        }

        public List<Detail> ReturnListDetail()
        {
            List<Detail> details = new List<Detail>(_catalogDetails);
            return details;
        }

        public void ShowCatalogDetails()
        {
            for (int i = 0; i < _catalogDetails.Count; i++)
            {
                Console.WriteLine($"Номер детали в каталоге - {i + 1},тип детали в каталоге - {_catalogDetails[i].Name}");
            }
        }
    }

    class Storage
    {
        private Catalog _catalogDetails;
        private List<Detail> _detailsList;

        public Storage(Catalog catalog)
        {
            _catalogDetails = catalog;
            _detailsList = new List<Detail>();
        }

        public int BuyDetail(int money)
        {
            bool isWork = true;
            ConsoleKey exitButton = ConsoleKey.P;

            while (isWork)
            {
                Console.WriteLine($"Ваш баланс - {money}");
                _catalogDetails.ShowCatalogDetails();
                int.TryParse(Console.ReadLine(), out int numberDetail);

                if (numberDetail > 0 && _catalogDetails.ReturnListDetail().Count >= numberDetail)
                {
                    if (money >= _catalogDetails.ReturnListDetail()[numberDetail - 1].Cost)
                    {
                        money -= _catalogDetails.ReturnListDetail()[numberDetail - 1].Cost;
                        _detailsList.Add(_catalogDetails.ReturnListDetail()[numberDetail - 1].Clone());
                        Console.WriteLine($"Поздравляю вы купили деталь за {_detailsList[_detailsList.Count - 1].Cost}, у вас осталось {money} $");
                    }
                    else
                    {
                        Console.WriteLine($"Извините деталь стоит {_catalogDetails.ReturnListDetail()[numberDetail - 1].Cost}, а у вас в наличии {money} $"); ;
                    }

                    Console.WriteLine($"\nДля того чтобы закончить покупку деталей нажмите {exitButton}.\nДля продолжение работы нажмите любую другую клавишу");

                    if (Console.ReadKey().Key == exitButton)
                    {
                        Console.WriteLine("Вы вышли из программы");
                        isWork = false;
                    }

                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Введено неверное значение");
                }
            }

            return money;
        }

        public void ShowDetails()
        {
            if (_detailsList.Count > 0)
            {
                for (int i = 0; i < _detailsList.Count; i++)
                {
                    Console.WriteLine($"Номер {i + 1} лежащей на складе детали, тип детали {_detailsList[i].Name}");
                }
            }
            else
            {
                Console.WriteLine("На складе нет деталей");
            }
        }

        public void RemoveDetail(int numberDetail)
        {
            _detailsList.RemoveAt(numberDetail - 1);
        }

        public int AmountDetails()
        {
            return _detailsList.Count();
        }

        public int CostDetail(int numberDetail)
        {
            return _detailsList[numberDetail - 1].Cost;
        }

        public Detail GetDetail(int numberDetail)
        {
            Detail detail = _detailsList[numberDetail - 1];
            return detail;
        }
    }

    abstract class Detail
    {
        public Detail(int cost, string name, string nameProblem)
        {
            Cost = cost;
            Name = name;
            NameProblem = nameProblem;
        }

        public string Name { get; private set; }
        public string NameProblem { get; private set; }
        public int Cost { get; private set; }

        public abstract Detail Clone();
    }

    class Glass : Detail
    {
        public Glass(string name, int cost, string nameProblem) : base(cost, name, nameProblem) { }

        public override Detail Clone()
        {
            return new Glass(Name, Cost, NameProblem);
        }
    }

    class Headlights : Detail
    {
        public Headlights(string name, int cost, string nameProblem) : base(cost, name, nameProblem) { }

        public override Detail Clone()
        {
            return new Headlights(Name, Cost, NameProblem);
        }
    }

    class TurnSignals : Detail
    {
        public TurnSignals(string name, int cost, string nameProblem) : base(cost, name, nameProblem) { }

        public override Detail Clone()
        {
            return new TurnSignals(Name, Cost, NameProblem);
        }
    }

    class Tires : Detail
    {
        public Tires(string name, int cost, string nameProblem) : base(cost, name, nameProblem) { }

        public override Detail Clone()
        {
            return new Tires(Name, Cost, NameProblem);
        }
    }

    class DoorLock : Detail
    {
        public DoorLock(string name, int cost, string nameProblem) : base(cost, name, nameProblem) { }

        public override Detail Clone()
        {
            return new DoorLock(Name, Cost, NameProblem);
        }
    }
}