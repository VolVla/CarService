using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarService
{
    internal class Program
    {
        static void Main(string[] args)
        {        
            const int numberServiceClient = 1;
            const int numberCheckStorage = 2;
            const int numberBuyDetails = 3;
            ConsoleKey exitButton = ConsoleKey.Enter;
            bool isWork = true;

            while (isWork)
            {
                AutoService autoService = new AutoService();
                Console.WriteLine("Для начало работы автосервиса нажмите на любую клавишу");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine($"Для начала работы с клиентами напишите {1}");
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
                        autoService.BuyDetailsStorage();
                        break;
                }
                autoService.WorkClients();
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
        public AutoService() { }

        public int Money { get;private set; }

        public void WorkClients() 
        {
            AutoClient autoClient = new AutoClient();
            autoClient.ShowInfoClient();
        }

        public void BuyDetailsStorage()
        {
            _storage.BuyDetail(Money);
        }

        public void ShowDetailStorage()
        {
            _storage.ShowDetailsStorage();
        }

    }

    class AutoClient
    {
        public string Name { get;private set; }

        public AutoClient() 
        { 

        }

        public void ShowInfoClient()
        {
            Console.WriteLine($"");
        }
    }

    class Storage : AutoService
    {
        protected List<Details> _detailsStorage = new List<Details>();
        private const int numberDetailFirst = 1;
        private const int numberDetailSecond = 2;
        private const int numberDetailThird = 3;
        private const int numberDetailFourth = 4;
        private const int numberDetailFifth = 5;
        private Dictionary<int, Details> _catalogDetails = new Dictionary<int, Details>();

        public Storage() 
        {
            _catalogDetails.Add(numberDetailFirst, new DetailFirst("Стекло", "Сломано Лобовое Окно", 100));
            _catalogDetails.Add(numberDetailSecond, new DetailSecond("Фары", "Разбита одна Фара",20));
            _catalogDetails.Add(numberDetailThird, new DetailThird("Поворотники", "Отломались Поворотники",40));
            _catalogDetails.Add(numberDetailFourth, new DetailThird("Шины", "Проколоты Шины", 120));
            _catalogDetails.Add(numberDetailFifth, new DetailThird("Замок", "Дверь пытались взломать сломали Замок",50));
        }

        public void BuyDetail(int money) 
        {
            bool isWork = true;
            ConsoleKey exitButton = ConsoleKey.Enter;

            while (isWork)
            {
                ShowCatalogDetails();
                int.TryParse(Console.ReadLine(), out int result);
                
                if (_catalogDetails.ContainsKey(result))
                {
                    if (BuyAbilityDetail(money, result))
                    {
                        _detailsStorage.Add(_catalogDetails[result]);
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
            foreach(var numberDetail in _catalogDetails)
            {
                Console.WriteLine($"Номер детали в каталоге - {numberDetail.Key},тип детали в каталоге - {_catalogDetails[numberDetail.Key].NameDetail}");
            }
        }

        public void ShowDetailsStorage()
        {
            for( int i = 0; i < _detailsStorage.Count;i++)
            {
                Console.WriteLine($"Номер {i} лежащей на складе детали, тип детали {_detailsStorage[i].NameDetail}");
            }
        }
    }

    abstract class Details 
    {
        public Details(string nameDetail, string problemClient,int costDetail)
        {
            NameDetail = nameDetail;
            ProblemClient = problemClient;
            CostDetail = costDetail;
        }

        public string NameDetail { get; private set; }
        public string ProblemClient { get; private set; } 
        public int CostDetail { get; private set; }

        public abstract Details Clone();
    }

    class DetailFirst : Details
    {
        public DetailFirst(string nameDetail, string problemClient, int costDetail) : base (nameDetail, problemClient,costDetail) { }

        public override Details Clone()
        {
            return new DetailFirst(NameDetail, ProblemClient,CostDetail);
        }
    }

    class DetailSecond : Details
    {
        public DetailSecond(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        public override Details Clone()
        {
            return new DetailSecond(NameDetail, ProblemClient,CostDetail);
        }
    }

    class DetailThird : Details
    { 
        public DetailThird(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        public override Details Clone()
        {
            return new DetailThird(NameDetail, ProblemClient, CostDetail);
        }

    }

    class DetailFourth : Details 
    {
        public DetailFourth(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        public override Details Clone()
        {
            return new DetailFourth(NameDetail, ProblemClient, CostDetail);
        }
    }

    class DetailFifth : Details 
    {
        public DetailFifth(string nameDetail, string problemClient, int costDetail) : base(nameDetail, problemClient, costDetail) { }

        public override Details Clone()
        {
            return new DetailFifth(NameDetail, ProblemClient, CostDetail);
        }
    }
}
