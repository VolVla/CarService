using System;
using System.Collections.Generic;
using System.Linq;

namespace CarService
{
    internal class Program
    {
        static void Main()
        {
            City city = new City();
            city.Start();
        }
    }
}


class City
{
    private const int CommandClient = 1;
    private const int CommandStorage = 2;
    private const int CommandExit = 3;
    private bool _isWork = true;
    private AutoService _autoService = new AutoService();
    private Queue<Client> _clients = new Queue<Client>();

    public void Start()
    {
        Console.WriteLine("Для начало работы автосервиса нажмите на любую клавишу");
        Console.ReadKey();
        Console.Clear();

        while (_isWork)
        {
            CreateClient();
            _autoService.ShowBalance();
            Console.WriteLine($"Для начала работы с клиентом напишите {CommandClient},\nДля того посмотреть какие есть детали на складе {CommandStorage}\nВы хотите выйти из программы?Нажмите {CommandExit}.");

            switch (Console.ReadLine())
            {
                case "1":
                    ServiceClient();
                    break;
                case "2":
                    _autoService.ShowDetailsStorage();
                    break;
                case "3":
                    _isWork = false;
                    break;
                default:
                    Console.WriteLine("Выбрана не существующая команда");
                    break;
            }
        }

        Console.WriteLine("Вы вышли из программы");
    }

    private void CreateClient()
    {
        List<Detail> brokenDetails = new List<Detail>()
        {
            new Glass("Сломано Лобовое Окно", 0),
            new Headlights("Разбита одна Фара", 0),
            new TurnSignals("Отломались Поворотники", 0),
            new Tires("Проколоты Шины", 0),
            new DoorLock("Дверь пытались взломать сломали Замок", 0)
        };
        Random random = new Random();
        List<string> namesClients = new List<string>() { "Саня", "Вова", "Артем", "Вика", "Аня" };
        string nameClient = namesClients[random.Next(namesClients.Count)];
        _clients.Enqueue(new Client(nameClient, brokenDetails[random.Next(brokenDetails.Count)]));
    }

    private void ServiceClient()
    {
        _autoService.ServiceClient(_clients.Dequeue());
    }
}

class AutoService
{
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

    public void ServiceClient(Client autoClient)
    {
        autoClient.ShowInfo();
        RepairCar(autoClient);
    }

    public void ShowBalance()
    {
        Console.WriteLine($"Ваш баланс на предприятии {_money} $");
    }

    public void ShowDetailsStorage()
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
        ShowDetailsStorage();
        Console.WriteLine("Выберете деталь  для починки авто");
        int.TryParse(Console.ReadLine(), out int numberDetail);

        if (_storage.AmountDetails() > 0)
        {
            if (numberDetail <= _storage.AmountDetails())
            {
                Console.WriteLine($"Цена  детали - {_storage.CostDetail(numberDetail)}, цена ремонта {_priceRepair}");

                if (autoClient.ReplaceDetail(_storage.GetDetail(numberDetail)))
                {
                    int amountCost = _priceRepair + _storage.CostDetail(numberDetail);
                    Console.WriteLine($"Вы заработали {amountCost} $ за успешную работу");
                    TakeMoney(amountCost);
                    Console.WriteLine("Поздравляю довольный клиент");
                }
                else
                {
                    Console.WriteLine($"Извините мы поставили нету детали в качестве извинения мы выплатим ущерб в виде {_storage.CostDetail(numberDetail)}$");
                    GiveMoney(_storage.CostDetail(numberDetail));
                }

                _storage.RemoveDetail(numberDetail);
            }
            else
            {
                Console.WriteLine("Данной детали не существует");
            }

            return;
        }
        else
        {
            Console.WriteLine($"Извините у нас нету нужной детали мы выплатим штраф {_punishment}");
            GiveMoney(_punishment);
            Console.WriteLine("Для продолжения нажмите на любую клавишу");
            Console.ReadKey();
        }
    }
}

class Client
{
    private bool _isCorrectReplaceDetail;
    private Detail _problemDetail;

    public Client(string _name, Detail problemDetail)
    {
        Name = _name;
        _problemDetail = problemDetail;
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
        Console.WriteLine($"Имя клиента - {Name}, проблема в машине - {_problemDetail.Name}");
    }
}

class Storage
{
    private List<Detail> _detailsList;

    public Storage()
    {
        _detailsList = new List<Detail>()
        {
            new Glass("Стекло", 100),
            new Headlights("Фары", 20),
            new TurnSignals("Поворотники", 40),
            new Tires("Шины", 120),
            new DoorLock("Замок", 50)

        };
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
        return _detailsList[numberDetail - 1];
    }
}

abstract class Detail
{
    public Detail(int cost, string name)
    {
        Cost = cost;
        Name = name;
    }

    public string Name { get; private set; }
    public string NameProblem { get; protected set; }
    public int Cost { get; private set; }

    public abstract Detail Clone();
}

class Glass : Detail
{
    public Glass(string name, int cost) : base(cost, name)
    {
        NameProblem = "Сломано Лобовое Окно";
    }

    public override Detail Clone()
    {
        return new Glass(Name, Cost);
    }
}

class Headlights : Detail
{
    public Headlights(string name, int cost) : base(cost, name)
    {
        NameProblem = "Разбита одна Фара";
    }

    public override Detail Clone()
    {
        return new Headlights(Name, Cost);
    }
}

class TurnSignals : Detail
{
    public TurnSignals(string name, int cost) : base(cost, name)
    {
        NameProblem = "Отломались Поворотники";
    }

    public override Detail Clone()
    {
        return new TurnSignals(Name, Cost);
    }
}

class Tires : Detail
{
    public Tires(string name, int cost) : base(cost, name)
    {
        NameProblem = "Проколоты Шины";
    }

    public override Detail Clone()
    {
        return new Tires(Name, Cost);
    }
}

class DoorLock : Detail
{
    public DoorLock(string name, int cost) : base(cost, name)
    {
        NameProblem = "Дверь пытались взломать сломали Замок";
    }

    public override Detail Clone()
    {
        return new DoorLock(Name, Cost);
    }
}