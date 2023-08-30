using System;
using System.Collections.Generic;

namespace CarService
{
    internal class Program
    {
        static void Main()
        {
            const int numberServiceClient = 1;
            const int numberCheckStorage = 2;
            const int numberBuyDetails = 3;
            ConsoleKey exitButton = ConsoleKey.Enter;
            bool isWork = true;
            AutoService autoService = new AutoService();
            Console.WriteLine("Для начало работы автосервиса нажмите на любую клавишу");
            Console.ReadKey();
            Console.Clear();

            while (isWork)
            {
                Console.WriteLine($"Ваш баланс - {autoService.Money}\nДля начала работы с клиентом напишите {numberServiceClient}, для того посмотреть какие есть детали на складе {numberCheckStorage},для покупки деталей {numberBuyDetails}");
                int.TryParse(Console.ReadLine(), out int result);

                switch (result)
                {
                    case numberServiceClient:
                        autoService.WorkClients();
                        break;
                    case numberCheckStorage:
                        autoService.ShowDetailStorage();
                        break;
                    case numberBuyDetails:
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
        private Storage _storage = new Storage();
        private int _priceRemonte = 100;
        private int _punishment = 20;

        public AutoService()
        {
            Money = 1000;
        }

        public int Money { get; private set; }

        public void WorkClients()
        {
            AutoClient autoClient = new AutoClient();
            autoClient.ShowInfoClient();
            RemonteAuto(autoClient);
        }

        public void BuyDetailsStorage(AutoService autoService)
        {
            _storage.BuyDetail(autoService);
        }

        public void ShowDetailStorage()
        {
            _storage.ShowDetailsStorage();
        }

        private void RemonteAuto(AutoClient autoClient)
        {
            ShowDetailStorage();
            Console.WriteLine("Выберете деталь  для починки авто");
            int.TryParse(Console.ReadLine(), out int result);

            if (_storage.DetailsStorage.Count > 0)
            {
                Console.WriteLine($"Цена  деталь - {_storage.DetailsStorage[result - 1].CostDetail}, цена ремонта {_priceRemonte}");

                if (_storage.DetailsStorage[result - 1].ProblemClient == autoClient.NameProblemClient)
                {
                    PayMoney(_storage.DetailsStorage[result - 1].CostDetail);
                    _storage.DetailsStorage.RemoveAt(result - 1);
                    Console.WriteLine("Поздравляю довольный клиент");
                }
                else
                {
                    TakeMoney(_storage.DetailsStorage[result - 1].CostDetail);
                    Console.WriteLine($"Извините мы поставили нету детали в качестве извинения мы выплатим ущерб в виде {_storage.DetailsStorage[result - 1].CostDetail}$");
                    _storage.DetailsStorage.RemoveAt(result - 1);
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

        private void PayMoney(int costDetail)
        {
            int amountCost = costDetail + _priceRemonte;
            Money += amountCost;
            Console.WriteLine($"Вы заработали {amountCost} $ за успешную работу");
        }
    }

    class AutoClient
    {
        private List<string> _nameClients = new List<string>() { "Саня", "Вова", "Артем", "Вика", "Аня" };
        private List<string> _problemClients = new List<string>() { "Сломано Лобовое Окно", "Разбита одна Фара", "Отломались Поворотники", "Проколоты Шины", "Дверь пытались взломать сломали Замок", };
        private Random _random = new Random();

        public AutoClient()
        {
            SetProbllemClient();
        }

        public string NameClient { get; private set; }
        public string NameProblemClient { get; private set; }

        public void ShowInfoClient()
        {
            Console.WriteLine($"Имя клиента - {NameClient}, проблема в машине -{NameProblemClient}");
        }

        private void SetProbllemClient()
        {
            int firstProblemClient = 0;
            int numberProblemClient;
            numberProblemClient = _random.Next(firstProblemClient, _problemClients.Count + 1);
            NameProblemClient = _problemClients[numberProblemClient];
            numberProblemClient = _random.Next(firstProblemClient, _problemClients.Count + 1);
            NameClient = _nameClients[numberProblemClient];
        }
    }

    class Storage
    {
        private const int _numberDetailFirst = 1;
        private const int _numberDetailSecond = 2;
        private const int _numberDetailThird = 3;
        private const int _numberDetailFourth = 4;
        private const int _numberDetailFifth = 5;
        public List<Details> DetailsStorage = new List<Details>();
        private Dictionary<int, Details> _catalogDetails = new Dictionary<int, Details>();

        public Storage()
        {
            _catalogDetails.Add(_numberDetailFirst, new DetailFirst("Стекло", "Сломано Лобовое Окно", 100));
            _catalogDetails.Add(_numberDetailSecond, new DetailSecond("Фары", "Разбита одна Фара", 20));
            _catalogDetails.Add(_numberDetailThird, new DetailThird("Поворотники", "Отломались Поворотники", 40));
            _catalogDetails.Add(_numberDetailFourth, new DetailThird("Шины", "Проколоты Шины", 120));
            _catalogDetails.Add(_numberDetailFifth, new DetailThird("Замок", "Дверь пытались взломать сломали Замок", 50));
        }

        public void BuyDetail(AutoService service)
        {
            bool isWork = true;
            ConsoleKey exitButton = ConsoleKey.Enter;

            while (isWork)
            {
                ShowCatalogDetails();
                int.TryParse(Console.ReadLine(), out int result);

                if (_catalogDetails.ContainsKey(result))
                {
                    if (BuyAbilityDetail(service.Money, result))
                    {
                        service.TakeMoney(_catalogDetails[result].CostDetail);
                        DetailsStorage.Add(_catalogDetails[result]);
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

        public void ShowDetailsStorage()
        {
            for (int i = 0; i < DetailsStorage.Count; i++)
            {
                Console.WriteLine($"Номер {i + 1} лежащей на складе детали, тип детали {DetailsStorage[i].NameDetail}");
            }
        }

        private bool BuyAbilityDetail(int money, int idDetail)
        {
            bool isResult;

            if (_catalogDetails[idDetail].CostDetail <= money)
            {
                Console.WriteLine($"Поздравляю вы купили деталь за {_catalogDetails[idDetail].CostDetail}, у вас осталось {money} $");
                isResult = true;
            }
            else
            {
                Console.WriteLine($"Извините деталь стоит {_catalogDetails[idDetail].CostDetail}, а у вас в наличии {money} $"); ;
                isResult = false;
            }

            return isResult;
        }

        private void ShowCatalogDetails()
        {
            foreach (var numberDetail in _catalogDetails)
            {
                Console.WriteLine($"Номер детали в каталоге - {numberDetail.Key},тип детали в каталоге - {_catalogDetails[numberDetail.Key].NameDetail}");
            }
        }
    }

    abstract class Details
    {
        public Details(string nameDetail, string problemClient, int costDetail)
        {
            NameDetail = nameDetail;
            ProblemClient = problemClient;
            CostDetail = costDetail;
        }

        public int CostDetail { get; private set; }
        public string NameDetail { get; private set; }
        public string ProblemClient { get; private set; }

        protected abstract Details Clone();
    }

    class DetailFirst : Details
    {
        public DetailFirst(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        protected override Details Clone()
        {
            return new DetailFirst(NameDetail, ProblemClient, CostDetail);
        }
    }

    class DetailSecond : Details
    {
        public DetailSecond(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        protected override Details Clone()
        {
            return new DetailSecond(NameDetail, ProblemClient, CostDetail);
        }
    }

    class DetailThird : Details
    {
        public DetailThird(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        protected override Details Clone()
        {
            return new DetailThird(NameDetail, ProblemClient, CostDetail);
        }
    }

    class DetailFourth : Details
    {
        public DetailFourth(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        protected override Details Clone()
        {
            return new DetailFourth(NameDetail, ProblemClient, CostDetail);
        }
    }

    class DetailFifth : Details
    {
        public DetailFifth(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        protected override Details Clone()
        {
            return new DetailFifth(NameDetail, ProblemClient, CostDetail);
        }
    }
}