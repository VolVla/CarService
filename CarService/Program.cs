using System;
using System.Collections.Generic;

namespace CarService
{
    internal class Program
    {
        static void Main()
        {
            const int ServiceClient = 1;
            const int CheckStorage = 2;
            const int BuyDetails = 3;
            ConsoleKey exitButton = ConsoleKey.F;
            bool isWork = true;
            AutoService autoService = new AutoService();
            Console.WriteLine("Для начало работы автосервиса нажмите на любую клавишу");
            Console.ReadKey();
            Console.Clear();

            while (isWork)
            {
                Console.WriteLine($"Ваш баланс - {autoService.Money}\nДля начала работы с клиентом напишите {ServiceClient}, для того посмотреть какие есть детали на складе {CheckStorage},для покупки деталей {BuyDetails}");
                int.TryParse(Console.ReadLine(), out int result);

                switch (result)
                {
                    case ServiceClient:
                        autoService.WorkClients();
                        break;
                    case CheckStorage:
                        autoService.ShowDetailStorage();
                        break;
                    case BuyDetails:
                        autoService.BuyDetailsStorage();
                        break;
                    default:
                        Console.WriteLine("Выбрана не существующая команда");
                        break;
                }

                Console.WriteLine($"\nВы хотите выйти из программы?Нажмите {exitButton}.\nДля продолжение работы нажмите любую другую клавишу");

                if (Console.ReadKey().Key == exitButton)
                {
                    Console.WriteLine("Вы вышли из программы");
                    isWork = false;
                }

                Console.Clear();
            }
        }
    }

    class AutoService
    {
        private List<string> _namesClients;
        private List<string> _problemsClients;
        private Storage _storage;
        private int _priceRepair;
        private int _punishment;

        public AutoService()
        {
            _storage = new Storage(this);
            _namesClients = new List<string>() { "Саня", "Вова", "Артем", "Вика", "Аня" };
            _problemsClients = new List<string>() { "Сломано Лобовое Окно", "Разбита одна Фара", "Отломались Поворотники", "Проколоты Шины", "Дверь пытались взломать сломали Замок", };
            Money = 1000;
            _priceRepair = 100;
            _punishment = 20;
        }

        public int Money { get; private set; }

        public void WorkClients()
        {
            Client autoClient = new Client(_namesClients, _problemsClients);
            autoClient.ShowInfo();
            RemonteAuto(autoClient);
        }

        public void BuyDetailsStorage()
        {
            _storage.BuyDetail();
        }

        public void ShowDetailStorage()
        {
            _storage.ShowDetails();
        }

        private void RemonteAuto(Client autoClient)
        {
            ShowDetailStorage();

            Console.WriteLine("Выберете деталь  для починки авто");
            int.TryParse(Console.ReadLine(), out int numberDetail);

            if (_storage.DetailsList.Count > 0)
            {
                Console.WriteLine($"Цена  детали - {_storage.DetailsList[numberDetail - 1].Detail().Cost}, цена ремонта {_priceRepair}");

                if (_storage.DetailsList[numberDetail - 1].Detail().ProblemClient == autoClient.NameProblem)
                {
                    int amountCost = _priceRepair + _storage.DetailsList[numberDetail - 1].Detail().Cost;
                    Console.WriteLine($"Вы заработали {amountCost} $ за успешную работу");
                    Money += amountCost;
                    _storage.DetailsList.RemoveAt(numberDetail - 1);
                    Console.WriteLine("Поздравляю довольный клиент");
                }
                else
                {
                    TakeMoney(_storage.DetailsList[numberDetail - 1].Detail().Cost);
                    Console.WriteLine($"Извините мы поставили нету детали в качестве извинения мы выплатим ущерб в виде {_storage.DetailsList[numberDetail - 1].Detail().Cost}$");
                    _storage.DetailsList.RemoveAt(numberDetail - 1);
                }
            }
            else
            {
                Console.WriteLine($"Извините у нас нету нужной детали мы выплатим штраф {_punishment}");
                TakeMoney(_punishment);
            }
        }

        public void TakeMoney(int money)
        {
            Money -= money;
        }
    }

    class Client
    {
        private Random _random = new Random();

        public Client(List<string> _names, List<string> _problems)
        {
            int firstProblemClient = 0;
            int numberProblemClient;
            numberProblemClient = _random.Next(firstProblemClient, _problems.Count + 1);
            NameProblem = _problems[numberProblemClient];
            numberProblemClient = _random.Next(firstProblemClient, _problems.Count + 1);
            Name = _names[numberProblemClient];
        }

        public string Name { get; private set; }
        public string NameProblem { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя клиента - {Name}, проблема в машине - {NameProblem}");
        }
    }

    class Storage
    {
        public List<Details> DetailsList;
        private const int _NumberDetailFirst = 1;
        private const int _NumberDetailSecond = 2;
        private const int _NumberDetailThird = 3;
        private const int _NumberDetailFourth = 4;
        private const int _NumberDetailFifth = 5;
        private List<Details> _catalogDetails;
        private AutoService _autoService;

        public Storage(AutoService autoService)
        {
            _autoService = autoService;
            DetailsList = new List<Details>();
            _catalogDetails = new List<Details>()
            {
               GetFactory(_NumberDetailFirst),
               GetFactory(_NumberDetailSecond),
               GetFactory(_NumberDetailThird),
               GetFactory(_NumberDetailFourth),
               GetFactory(_NumberDetailFifth)
            };
        }

        public void BuyDetail()
        {
            bool isWork = true;
            ConsoleKey exitButton = ConsoleKey.P;

            while (isWork)
            {
                ShowCatalogDetails();
                int.TryParse(Console.ReadLine(), out int numberDetail);

                if (numberDetail > 0 && _catalogDetails.Count > numberDetail)
                {
                    Details details = GetFactory(numberDetail);

                    if (BuyAbilityDetail(_autoService.Money, details))
                    {
                        IDetail detail = details.Detail();
                        _autoService.TakeMoney(detail.Cost);
                        DetailsList.Add(details);
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
                    Console.WriteLine("Введено неыерное значение");
                }
            }
        }

        public void ShowDetails()
        {
            for (int i = 0; i < DetailsList.Count; i++)
            {
                Console.WriteLine($"Номер {i + 1} лежащей на складе детали, тип детали {DetailsList[i].Detail().Name}");
            }
        }

        private Details GetFactory(int numberDetailType)
        {
            Details detail;

            switch (numberDetailType)
            {
                case _NumberDetailFirst:
                    detail = new FactoryGlass(1, 100, "Стекло", "Сломано Лобовое Окно");
                    break;
                case _NumberDetailSecond:
                    detail = new FactoryHeadlights(2, 20, "Фары", "Разбита одна Фара");
                    break;
                case _NumberDetailThird:
                    detail = new FactoryTurnSignals(3, 40, "Поворотники", "Отломались Поворотники");
                    break;
                case _NumberDetailFourth:
                    detail = new FactoryTires(4, 120, "Шины", "Проколоты Шины");
                    break;
                case _NumberDetailFifth:
                    detail = new FactoryDoorLock(5, 60, "Замок", "Дверь пытались взломать сломали Замок");
                    break;
                default:
                    detail = null;
                    break;
            }

            return detail;
        }

        private bool BuyAbilityDetail(int money, Details details)
        {
            bool isResult;

            if (details.Detail().Cost <= money)
            {
                Console.WriteLine($"Поздравляю вы купили деталь за {details.Detail().Cost}, у вас осталось {money} $");
                isResult = true;
            }
            else
            {
                Console.WriteLine($"Извините деталь стоит {details.Detail().Cost}, а у вас в наличии {money} $"); ;
                isResult = false;
            }

            return isResult;
        }

        private void ShowCatalogDetails()
        {
            foreach (var numberDetail in _catalogDetails)
            {
                Console.WriteLine($"Номер детали в каталоге - {numberDetail.Detail().Id},тип детали в каталоге - {numberDetail.Detail().Name}");
            }
        }
    }

    abstract class Details
    {
        public abstract IDetail Detail();
    }

    class FactoryGlass : Details
    {
        private readonly int _cost;
        private readonly string _name;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public FactoryGlass(int id, int cost, string name, string problemClient)
        {
            _cost = cost;
            _name = name;
            _problemClient = problemClient;
            _idDetail = id;
        }

        public override IDetail Detail()
        {
            Glass glass = new Glass(_idDetail, _name, _cost, _problemClient);
            return glass;
        }
    }

    class FactoryHeadlights : Details
    {
        private readonly int _cost;
        private readonly string _name;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public FactoryHeadlights(int id, int cost, string name, string problemClient)
        {
            _cost = cost;
            _name = name;
            _problemClient = problemClient;
            _idDetail = id;
        }

        public override IDetail Detail()
        {
            Headlights headlights = new Headlights(_idDetail, _name, _cost, _problemClient);
            return headlights;
        }
    }

    class FactoryTurnSignals : Details
    {
        private readonly int _cost;
        private readonly string _name;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public FactoryTurnSignals(int id, int cost, string name, string problemClient)
        {
            _cost = cost;
            _name = name;
            _problemClient = problemClient;
            _idDetail = id;
        }

        public override IDetail Detail()
        {
            TurnSignals turnSignals = new TurnSignals(_idDetail, _name, _cost, _problemClient);
            return turnSignals;
        }
    }

    class FactoryTires : Details
    {
        private readonly int _cost;
        private readonly string _name;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public FactoryTires(int id, int cost, string name, string problemClient)
        {
            _cost = cost;
            _name = name;
            _problemClient = problemClient;
            _idDetail = id;
        }

        public override IDetail Detail()
        {
            Tires Tires = new Tires(_idDetail, _name, _cost, _problemClient);
            return Tires;
        }
    }

    class FactoryDoorLock : Details
    {
        private readonly int _cost;
        private readonly string _name;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public FactoryDoorLock(int id, int cost, string name, string problemClient)
        {
            _cost = cost;
            _name = name;
            _problemClient = problemClient;
            _idDetail = id;
        }

        public override IDetail Detail()
        {
            DoorLock doorLock = new DoorLock(_idDetail, _name, _cost, _problemClient);
            return doorLock;
        }
    }

    interface IDetail
    {
        int Cost { get; }
        int Id { get; }
        string Name { get; }
        string ProblemClient { get; }
    }

    class Glass : IDetail
    {
        private readonly string _name;
        private readonly int _cost;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public Glass(int id, string name, int cost, string problemClient)
        {
            _name = name;
            _cost = cost;
            _problemClient = problemClient;
            _idDetail = id;
        }

        string IDetail.Name => _name;
        int IDetail.Cost => _cost;
        string IDetail.ProblemClient => _problemClient;
        int IDetail.Id => _idDetail;
    }

    class Headlights : IDetail
    {
        private readonly string _name;
        private readonly int _cost;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public Headlights(int id, string name, int cost, string problemClient)
        {
            _name = name;
            _cost = cost;
            _problemClient = problemClient;
            _idDetail = id;
        }

        string IDetail.Name => _name;
        int IDetail.Cost => _cost;
        string IDetail.ProblemClient => _problemClient;
        int IDetail.Id => _idDetail;
    }

    class TurnSignals : IDetail
    {
        private readonly string _name;
        private readonly int _cost;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public TurnSignals(int id, string name, int cost, string problemClient)
        {
            _name = name;
            _cost = cost;
            _problemClient = problemClient;
            _idDetail = id;
        }

        string IDetail.Name => _name;
        int IDetail.Cost => _cost;
        string IDetail.ProblemClient => _problemClient;
        int IDetail.Id => _idDetail;
    }

    class Tires : IDetail
    {
        private readonly string _name;
        private readonly int _cost;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public Tires(int id, string name, int cost, string problemClient)
        {
            _name = name;
            _cost = cost;
            _problemClient = problemClient;
            _idDetail = id;
        }

        string IDetail.Name => _name;
        int IDetail.Cost => _cost;
        string IDetail.ProblemClient => _problemClient;
        int IDetail.Id => _idDetail;
    }

    class DoorLock : IDetail
    {
        private readonly string _name;
        private readonly int _cost;
        private readonly string _problemClient;
        private readonly int _idDetail;

        public DoorLock(int id, string name, int cost, string problemClient)
        {
            _name = name;
            _cost = cost;
            _problemClient = problemClient;
            _idDetail = id;
        }

        string IDetail.Name => _name;
        int IDetail.Cost => _cost;
        string IDetail.ProblemClient => _problemClient;
        int IDetail.Id => _idDetail;
    }
}