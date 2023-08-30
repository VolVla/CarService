using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CarService
{
    internal class Program
    {
        static void Main()
        {
            const int ServiceClient = 1;
            const int CheckStorage = 2;
            const int BuyDetails = 3;
            const int ExitProgram = 4;

            bool isWork = true;
            AutoService autoService = new AutoService();
            Console.WriteLine("Для начало работы автосервиса нажмите на любую клавишу");
            Console.ReadKey();
            Console.Clear();

            while (isWork)
            {
                autoService.ShowBalance();
                Console.WriteLine($"Для начала работы с клиентом напишите {ServiceClient},\nДля того посмотреть какие есть детали на складе {CheckStorage}\nДля покупки деталей {BuyDetails}\nВы хотите выйти из программы?Нажмите {ExitProgram}.");
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
                    case ExitProgram:
                        isWork = false;
                        break;
                    default:
                        Console.WriteLine("Выбрана не существующая команда");
                        break;
                }

                Console.Clear();
            }

            Console.WriteLine("Вы вышли из программы");
        }
    }

    interface IDetail
    {
        int Cost { get; }
        int Id { get; }
        string Name { get; }
        string ProblemClient { get; }
    }

    class AutoService
    {
        private Storage _storage;
        private List<string> _namesClients;
        private List<string> _problemsClients;
        private List<Detail> _detailsList;
        private int _priceRepair;
        private int _punishment;
        private int Money;

        public AutoService()
        {
            _storage = new Storage();
            _detailsList = new List<Detail>();
            _namesClients = new List<string>() { "Саня", "Вова", "Артем", "Вика", "Аня" };
            _problemsClients = new List<string>() { "Сломано Лобовое Окно", "Разбита одна Фара", "Отломались Поворотники", "Проколоты Шины", "Дверь пытались взломать сломали Замок", };
            Money = 1000;
            _priceRepair = 100;
            _punishment = 20;
        }

        public void WorkClients()
        {
            Random random = new Random();
            int firstProblemClient = 0;
            int numberProblemClient = random.Next(firstProblemClient, _problemsClients.Count + 1);
            string nameProblem = _problemsClients[numberProblemClient];
            int numberNameClient = random.Next(firstProblemClient, _namesClients.Count + 1);
            string name = _namesClients[numberNameClient];
            Client autoClient = new Client(name, nameProblem);
            autoClient.ShowInfo();
            RemonteAuto(autoClient);
        }

        public void BuyDetailsStorage()
        {
            _storage.BuyDetail(ref Money);
            _detailsList = _storage.ReturnCopyList();
        }

        public void ShowDetailStorage()
        {
            for (int i = 0; i < _detailsList.Count; i++)
            {
                Console.WriteLine($"Номер {i + 1} лежащей на складе детали, тип детали {_detailsList[i].CreateDetail().Name}");
            }

            Console.WriteLine("Для продолжения нажмите на любую кнопку");
            Console.ReadKey();
        }

        public void ShowBalance()
        {
            Console.WriteLine($"Ваш баланс на предприятии {Money} $");
        }

        private void TakeMoney(int money)
        {
            Money -= money;
        }

        private void RemonteAuto(Client autoClient)
        {
            ShowDetailStorage();
            Console.WriteLine("Выберете деталь  для починки авто");
            int.TryParse(Console.ReadLine(), out int numberDetail);

            if (_detailsList.Count > 0)
            {
                Console.WriteLine($"Цена  детали - {_detailsList[numberDetail - 1].CreateDetail().Cost}, цена ремонта {_priceRepair}");

                if (autoClient.ReplaceDetail(_detailsList[numberDetail - 1].CreateDetail().ProblemClient))
                {
                    int amountCost = _priceRepair + _detailsList[numberDetail - 1].CreateDetail().Cost;
                    Console.WriteLine($"Вы заработали {amountCost} $ за успешную работу");
                    Money += amountCost;
                    _detailsList.RemoveAt(numberDetail - 1);
                    Console.WriteLine("Поздравляю довольный клиент");
                }
                else
                {
                    TakeMoney(_detailsList[numberDetail - 1].CreateDetail().Cost);
                    Console.WriteLine($"Извините мы поставили нету детали в качестве извинения мы выплатим ущерб в виде {_detailsList[numberDetail - 1].CreateDetail().Cost}$");
                    _detailsList.RemoveAt(numberDetail - 1);
                }
            }
            else
            {
                Console.WriteLine($"Извините у нас нету нужной детали мы выплатим штраф {_punishment}");
                TakeMoney(_punishment);
            }
        }
    }

    class Client
    {
        private bool _isCorrectReplaceDetail;

        public Client(string _name, string _problem)
        {
            Name = _name;
            NameProblem = _problem;
        }

        public string Name { get; private set; }
        public string NameProblem { get; private set; }

        public bool ReplaceDetail(string Detail)
        {
            if (Detail == NameProblem)
            {
                _isCorrectReplaceDetail = true;
                Console.WriteLine("Автомеханик заменил на правильную деталь");
            }
            else
            {
                _isCorrectReplaceDetail = false;
                Console.WriteLine("Автомеханик заменил на не правильную деталь");
            }

            return _isCorrectReplaceDetail;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя клиента - {Name}, проблема в машине - {NameProblem}");
        }
    }

    class Storage
    {
        private const int NumberDetailFirst = 1;
        private const int NumberDetailSecond = 2;
        private const int NumberDetailThird = 3;
        private const int NumberDetailFourth = 4;
        private const int NumberDetailFifth = 5;

        private List<Detail> _detailsList;
        private List<Detail> _catalogDetails;

        public Storage()
        {
            _detailsList = new List<Detail>();
            _catalogDetails = new List<Detail>()
            {
               GetDetail(NumberDetailFirst),
               GetDetail(NumberDetailSecond),
               GetDetail(NumberDetailThird),
               GetDetail(NumberDetailFourth),
               GetDetail(NumberDetailFifth)
            };
        }

        public List<Detail> ReturnCopyList()
        {
            List<Detail> details = new List<Detail>(_detailsList);
            return details;
        }

        public void BuyDetail(ref int Money)
        {
            bool isWork = true;
            ConsoleKey exitButton = ConsoleKey.P;

            while (isWork)
            {
                Console.WriteLine($"Ваш баланс - {Money}");
                ShowCatalogDetails();
                int.TryParse(Console.ReadLine(), out int numberDetail);

                if (numberDetail > 0 && _catalogDetails.Count >= numberDetail)
                {
                    Detail details = GetDetail(numberDetail);

                    if (BuyAbilityDetail(Money, details))
                    {
                        IDetail detail = details.CreateDetail();
                        Money -= detail.Cost;
                        _detailsList.Add(details);
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
        }

        private Detail GetDetail(int numberDetailType)
        {
            Detail detail;

            switch (numberDetailType)
            {
                case NumberDetailFirst:
                    detail = new FactoryGlass(1, 100, "Стекло", "Сломано Лобовое Окно");
                    break;
                case NumberDetailSecond:
                    detail = new FactoryHeadlights(2, 20, "Фары", "Разбита одна Фара");
                    break;
                case NumberDetailThird:
                    detail = new FactoryTurnSignals(3, 40, "Поворотники", "Отломались Поворотники");
                    break;
                case NumberDetailFourth:
                    detail = new FactoryTires(4, 120, "Шины", "Проколоты Шины");
                    break;
                case NumberDetailFifth:
                    detail = new FactoryDoorLock(5, 60, "Замок", "Дверь пытались взломать сломали Замок");
                    break;
                default:
                    detail = null;
                    break;
            }

            return detail;
        }

        private bool BuyAbilityDetail(int money, Detail details)
        {
            bool isResult;

            if (details.CreateDetail().Cost <= money)
            {
                Console.WriteLine($"Поздравляю вы купили деталь за {details.CreateDetail().Cost}, у вас осталось {money} $");
                isResult = true;
            }
            else
            {
                Console.WriteLine($"Извините деталь стоит {details.CreateDetail().Cost}, а у вас в наличии {money} $"); ;
                isResult = false;
            }

            return isResult;
        }

        private void ShowCatalogDetails()
        {
            foreach (var numberDetail in _catalogDetails)
            {
                Console.WriteLine($"Номер детали в каталоге - {numberDetail.CreateDetail().Id},тип детали в каталоге - {numberDetail.CreateDetail().Name}");
            }
        }
    }

    abstract class Detail
    {
        protected int Cost { get; set; }
        protected int Id { get; set; }
        protected string Name { get; set; }
        protected string ProblemClient { get; set; }

        public abstract IDetail CreateDetail();
    }

    class FactoryGlass : Detail
    {
        public FactoryGlass(int id, int cost, string name, string problemClient)
        {
            Cost = cost;
            Name = name;
            ProblemClient = problemClient;
            Id = id;
        }

        public override IDetail CreateDetail()
        {
            Glass glass = new Glass(Id, Name, Cost, ProblemClient);
            return glass;
        }
    }

    class FactoryHeadlights : Detail
    {
        public FactoryHeadlights(int id, int cost, string name, string problemClient)
        {
            Cost = cost;
            Name = name;
            ProblemClient = problemClient;
            Id = id;
        }

        public override IDetail CreateDetail()
        {
            Headlights headlights = new Headlights(Id, Name, Cost, ProblemClient);
            return headlights;
        }
    }

    class FactoryTurnSignals : Detail
    {
        public FactoryTurnSignals(int id, int cost, string name, string problemClient)
        {
            Cost = cost;
            Name = name;
            ProblemClient = problemClient;
            Id = id;
        }

        public override IDetail CreateDetail()
        {
            TurnSignals turnSignals = new TurnSignals(Id, Name, Cost, ProblemClient);
            return turnSignals;
        }
    }

    class FactoryTires : Detail
    {
        public FactoryTires(int id, int cost, string name, string problemClient)
        {
            Cost = cost;
            Name = name;
            ProblemClient = problemClient;
            Id = id;
        }

        public override IDetail CreateDetail()
        {
            Tires Tires = new Tires(Id, Name, Cost, ProblemClient);
            return Tires;
        }
    }

    class FactoryDoorLock : Detail
    {
        public FactoryDoorLock(int id, int cost, string name, string problemClient)
        {
            Cost = cost;
            Name = name;
            ProblemClient = problemClient;
            Id = id;
        }

        public override IDetail CreateDetail()
        {
            DoorLock doorLock = new DoorLock(Id, Name, Cost, ProblemClient);
            return doorLock;
        }
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