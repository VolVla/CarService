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
            ConsoleKey exitButton = ConsoleKey.Enter;
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
                        autoService.BuyDetailsStorage(autoService);
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
        private List<string> _namesClients = new List<string>() { "Саня", "Вова", "Артем", "Вика", "Аня" };
        private List<string> _problemsClients = new List<string>() { "Сломано Лобовое Окно", "Разбита одна Фара", "Отломались Поворотники", "Проколоты Шины", "Дверь пытались взломать сломали Замок", };
        private Storage _storage = new Storage();
        private int _priceRepair = 100;
        private int _punishment = 20;

        public AutoService()
        {
            Money = 1000;
        }

        public int Money { get; private set; }

        public void WorkClients()
        {
            Client autoClient = new Client(_namesClients, _problemsClients);
            autoClient.ShowInfo();
            RemonteAuto(autoClient);
        }

        public void BuyDetailsStorage(AutoService autoService)
        {
            _storage.BuyDetail(autoService);
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

            if (_storage.Details.Count > 0)
            {
                Console.WriteLine($"Цена  детали - {_storage.Details[numberDetail - 1].Cost}, цена ремонта {_priceRepair}");

                if (_storage.Details[numberDetail - 1].ProblemClient == autoClient.NameProblem)
                {
                    int amountCost = _priceRepair + _storage.Details[numberDetail - 1].Cost;
                    Console.WriteLine($"Вы заработали {amountCost} $ за успешную работу");
                    Money += amountCost;
                    _storage.Details.RemoveAt(numberDetail - 1);
                    Console.WriteLine("Поздравляю довольный клиент");
                }
                else
                {
                    TakeMoney(_storage.Details[numberDetail - 1].Cost);
                    Console.WriteLine($"Извините мы поставили нету детали в качестве извинения мы выплатим ущерб в виде {_storage.Details[numberDetail - 1].Cost}$");
                    _storage.Details.RemoveAt(numberDetail - 1);
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
            Console.WriteLine($"Имя клиента - {Name}, проблема в машине -{NameProblem}");
        }
    }

    class Storage
    {
        public List<Detail> Details = new List<Detail>();
        private int _numberDetailFirst;
        private int _numberDetailSecond;
        private int _numberDetailThird;
        private int _numberDetailFourth;
        private int _numberDetailFifth;
        private Dictionary<int, Detail> _catalogDetails = new Dictionary<int, Detail>();

        public Storage()
        {
            _numberDetailFirst = 1;
            _numberDetailSecond = 2;
            _numberDetailThird = 3;
            _numberDetailFourth = 4;
            _numberDetailFifth = 5;
            _catalogDetails.Add(_numberDetailFirst, new Detail("Стекло", "Сломано Лобовое Окно", 100));
            _catalogDetails.Add(_numberDetailSecond, new Detail("Фары", "Разбита одна Фара", 20));
            _catalogDetails.Add(_numberDetailThird, new Detail("Поворотники", "Отломались Поворотники", 40));
            _catalogDetails.Add(_numberDetailFourth, new Detail("Шины", "Проколоты Шины", 120));
            _catalogDetails.Add(_numberDetailFifth, new Detail("Замок", "Дверь пытались взломать сломали Замок", 50));
        }

        public void BuyDetail(AutoService service)
        {
            bool isWork = true;
            ConsoleKey exitButton = ConsoleKey.Enter;

            while (isWork)
            {
                ShowCatalogDetails();
                int.TryParse(Console.ReadLine(), out int numberDetail);

                if (_catalogDetails.ContainsKey(numberDetail))
                {
                    if (BuyAbilityDetail(service.Money, numberDetail))
                    {
                        service.TakeMoney(_catalogDetails[numberDetail].Cost);
                        Details.Add(_catalogDetails[numberDetail].Clone());
                    }
                }
                else
                {
                    Console.WriteLine("Вы выбрали несуществующий номер детали");
                }

                Console.WriteLine($"\nДля того чтобы закончить покупку деталей нажмите {exitButton}.\nДля продолжение работы нажмите любую другую клавишу");

                if (Console.ReadKey().Key == exitButton)
                {
                    Console.WriteLine("Вы вышли из программы");
                    isWork = false;
                }

                Console.Clear();
            }
        }

        public void ShowDetails()
        {
            for (int i = 0; i < Details.Count; i++)
            {
                Console.WriteLine($"Номер {i + 1} лежащей на складе детали, тип детали {Details[i].Name}");
            }
        }

        private bool BuyAbilityDetail(int money, int idDetail)
        {
            bool isResult;

            if (_catalogDetails[idDetail].Cost <= money)
            {
                Console.WriteLine($"Поздравляю вы купили деталь за {_catalogDetails[idDetail].Cost}, у вас осталось {money} $");
                isResult = true;
            }
            else
            {
                Console.WriteLine($"Извините деталь стоит {_catalogDetails[idDetail].Cost}, а у вас в наличии {money} $"); ;
                isResult = false;
            }

            return isResult;
        }

        private void ShowCatalogDetails()
        {
            foreach (var numberDetail in _catalogDetails)
            {
                Console.WriteLine($"Номер детали в каталоге - {numberDetail.Key},тип детали в каталоге - {_catalogDetails[numberDetail.Key].Name}");
            }
        }
    }

    class Detail
    {
        public Detail(string nameDetail, string problemClient, int costDetail)
        {
            Name = nameDetail;
            ProblemClient = problemClient;
            Cost = costDetail;
        }

        public int Cost { get; private set; }
        public string Name { get; private set; }
        public string ProblemClient { get; private set; }

        public Detail Clone()
        {
            return new Detail(Name, ProblemClient, Cost);
        }
    }
}