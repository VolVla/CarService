﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CarService
{
    internal class Program
    {
        static void Main()
        {
            WorkAutoService workAutoService = new WorkAutoService();
            workAutoService.Work();
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

        public void Work()
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
                        ServiceClient();
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
            ShowDetailsStorage();
            Console.WriteLine("Выберете деталь  для починки авто");
            int.TryParse(Console.ReadLine(), out int numberDetail);

            if (_storage.AmountDetails() > 0)
            {
                if (numberDetail <= _storage.AmountDetails())
                {
                    Console.WriteLine($"Цена  детали - {_storage.PriceDetail(numberDetail)}, цена ремонта {_priceRepair}");

                    if (autoClient.CorrectReplace(_storage.GetOneDetail(numberDetail)))
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
            ServiceClient(_clients.Dequeue());
        }
    }

    class Client
    {
        private Detail _problemDetail;

        public Client(string _name, Detail problemDetail)
        {
            Name = _name;
            _problemDetail = problemDetail;
        }

        public string Name { get; private set; }

        public bool CorrectReplace(Detail detail)
        {
            if (detail.NameProblem == _problemDetail.NameProblem)
            {
                Console.WriteLine("Автомеханик заменил на правильную деталь");
                return true;
            }
            else
            {
                Console.WriteLine("Автомеханик заменил на не правильную деталь");
                return false;
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя клиента - {Name}, проблема в машине - {_problemDetail.Name}");
        }
    }

    class Storage
    {
        private List<Detail> _details;

        public Storage()
        {
            _details = new List<Detail>()
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
    }

    class Glass : Detail
    {
        public Glass(string name, int cost) : base(cost, name)
        {
            NameProblem = "Сломано Лобовое Окно";
        }
    }

    class Headlights : Detail
    {
        public Headlights(string name, int cost) : base(cost, name)
        {
            NameProblem = "Разбита одна Фара";
        }
    }

    class TurnSignals : Detail
    {
        public TurnSignals(string name, int cost) : base(cost, name)
        {
            NameProblem = "Отломались Поворотники";
        }
    }

    class Tires : Detail
    {
        public Tires(string name, int cost) : base(cost, name)
        {
            NameProblem = "Проколоты Шины";
        }
    }

    class DoorLock : Detail
    {
        public DoorLock(string name, int cost) : base(cost, name)
        {
            NameProblem = "Дверь пытались взломать сломали Замок";
        }
    }
}