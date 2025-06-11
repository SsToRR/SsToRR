using System;
using System.Collections.Generic;
using System.Linq;

class Drink
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }

    public Drink(int id, string name, int price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}

class CashRegister
{
    private Dictionary<int, int> denominations;

    public CashRegister(Dictionary<int, int> denominations)
    {
        this.denominations = denominations;
    }

    public void AddCash(int denom, int count)
    {
        if (!denominations.ContainsKey(denom)) denominations[denom] = 0;
        denominations[denom] += count;
    }

    public Dictionary<int, int> GiveChange(int change)
    {
        var denominationsCopy = new Dictionary<int, int>(denominations);
        var sortedDenoms = denominationsCopy.Where(d => d.Value > 0).Select(d => d.Key).OrderBy(x => x).ToList();
        var changeGiven = new Dictionary<int, int>();
        int tempChange = change;

        while (tempChange > 0)
        {
            bool anyUsed = false;
            foreach (var denom in sortedDenoms)
            {
                if (tempChange >= denom && denominationsCopy[denom] > 0)
                {
                    tempChange -= denom;
                    if (!changeGiven.ContainsKey(denom)) changeGiven[denom] = 0;
                    changeGiven[denom]++;
                    denominationsCopy[denom]--;
                    anyUsed = true;
                }
            }
            if (!anyUsed) break;
        }

        if (tempChange > 0) return new Dictionary<int, int>();

        foreach (var pair in changeGiven)
        {
            denominations[pair.Key] -= pair.Value;
        }

        return changeGiven;
    }

    public int GetTotal()
    {
        return denominations.Sum(pair => pair.Key * pair.Value);
    }
}

class CoffeeMachine
{
    private int choice, person_money, price_of_coffee, password, number_of_denominations;
    private Dictionary<int, int> drinks = new();
    private Dictionary<int, int> denominations = new();
    private List<Drink> drinkList = new();

    public CoffeeMachine()
    {
        drinks[1] = 100;
        drinks[2] = 150;
        drinks[3] = 200;
        drinks[4] = 0;

        denominations[10] = 0;
        denominations[50] = 0;
        denominations[100] = 0;
        denominations[200] = 0;
        denominations[500] = 0;

        drinkList.Add(new Drink(1, "Espresso", 100));
        drinkList.Add(new Drink(2, "Cappucino", 150));
        drinkList.Add(new Drink(3, "Latte", 200));
    }

    public void Run()
    {
        var cash = new CashRegister(denominations);

        while (true)
        {
            ShowMenu();
            if (!int.TryParse(Console.ReadLine(), out choice)) continue;

            if (choice == 4)
            {
                Console.WriteLine("Введите пароль:");
                if (!int.TryParse(Console.ReadLine(), out password)) continue;

                if (password == 1234)
                {
                    Console.WriteLine("Вы успешно вошли! Выберите опцию:\n1. Пополнение купюр");
                    if (!int.TryParse(Console.ReadLine(), out choice)) continue;

                    if (choice == 1)
                    {
                        foreach (var denom in denominations.Keys.ToList())
                        {
                            Console.WriteLine($"Сколько купюр номиналом {denom} KZT добавить? (Сейчас: {denominations[denom]})");
                            if (!int.TryParse(Console.ReadLine(), out number_of_denominations)) continue;
                            cash.AddCash(denom, number_of_denominations);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Неправильный пароль");
                }
                continue;
            }

            if (!drinks.ContainsKey(choice))
            {
                Console.WriteLine("Неверный выбор");
                continue;
            }

            price_of_coffee = drinks[choice];
            Console.WriteLine($"Прекрасный выбор! Произведите оплату в {price_of_coffee} тенге:");
            if (!int.TryParse(Console.ReadLine(), out person_money)) continue;

            if (person_money < price_of_coffee)
            {
                Console.WriteLine("Недостаточно средств.");
                continue;
            }

            int change = person_money - price_of_coffee;
            if (change == 0)
            {
                Console.WriteLine("Спасибо! Ваш напиток готов.");
                continue;
            }

            int currentTotal = denominations.Sum(pair => pair.Key * pair.Value);
            if (currentTotal < change)
            {
                Console.WriteLine("Недостаточно купюр для выдачи сдачи.");
                continue;
            }

            var result = cash.GiveChange(change);
            if (result.Count == 0)
            {
                Console.WriteLine("Невозможно выдать сдачу доступными купюрами.");
            }
            else
            {
                Console.WriteLine("Сдача:");
                foreach (var pair in result)
                {
                    Console.WriteLine($"{pair.Key} x {pair.Value}");
                }
                Console.WriteLine("Ваш напиток готов! Приятного дня!");
            }
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine("\nВыберите напиток:");
        foreach (var drink in drinkList)
        {
            Console.WriteLine($"{drink.Id}. {drink.Name} - {drink.Price} KZT");
        }
        Console.WriteLine("4. Админ панель");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var machine = new CoffeeMachine();
        machine.Run();
    }
}
