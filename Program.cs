using System;
using System.Globalization;
using Microsoft.Data.Sqlite;

class Program
{
    static string connectionString = @"Data Source=habit-Tracker.db";
    static void Main()
    {
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var TableCmd = connection.CreateCommand();
            TableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drink_water(
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER
            )";
            TableCmd.ExecuteNonQuery();
            connection.Close();
        }
        GetUserInput();
    }
    static void GetUserInput()
    {
        Console.Clear();
        bool closeApp = false;
        while(!closeApp)
        {
            System.Console.WriteLine("Main Menu");
            System.Console.WriteLine("What would you like to do?");
            System.Console.WriteLine("Type 0 to close the App");
            System.Console.WriteLine("Type 1 to View all record");
            System.Console.WriteLine("Type 2 to Insert record");
            System.Console.WriteLine("Type 3 to Delete record");
            System.Console.WriteLine("Type 4 to Update record");
            System.Console.WriteLine();
            System.Console.WriteLine("-------------------------");
            string comand = System.Console.ReadLine();
            switch (comand)
            {
                case "0":
                    System.Console.WriteLine("Goodbye");
                    closeApp = true;
                    break;
                case "1":
                    GetAllRecord();
                    break;
                case "2":
                    Insert();
                    break;
                case "3":
                    Delete();
                    break;
                case "4":
                    Update();
                    break;
                default:
                    System.Console.WriteLine("Please enter a valid input");
                    break;
            }
        }
    }

    private static void Update()
    {
        Console.Clear();
        GetAllRecord();
        var recordID = GetNumberInput("Which one you want to update");
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var CheckCmd = connection.CreateCommand();
            CheckCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drink_water WHERE Id = {recordID})";
            int rowCount = Convert.ToInt32(CheckCmd.ExecuteScalar());
            if(rowCount==0)
            {
                System.Console.WriteLine("No Matching Row");
                connection.Close();
                Update();
            }
            string date = GetDateInput();
            int _quantity = GetNumberInput("Please inset number of coups exactly");
            var TableCmd = connection.CreateCommand();
            TableCmd.CommandText = $"UPDATE drink_water SET date={date}, quantity={_quantity} WHERE Id = {recordID}";
            TableCmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    private static void Delete()
    {
        Console.Clear();
        GetAllRecord();
        var recordID = GetNumberInput("Which one you want to delete");
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var TableCmd = connection.CreateCommand();
            TableCmd.CommandText = $"DELETE from drink_water WHERE Id = {recordID}";
            int rowCount = TableCmd.ExecuteNonQuery();
            if(rowCount==0)
            {
                System.Console.WriteLine("No Matching Row");
                Delete();
            }
            connection.Close();
        }
        System.Console.WriteLine("That row was deleted");
        GetUserInput();
    }

    private static void GetAllRecord()
    {
        Console.Clear();
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var TableCmd = connection.CreateCommand();
            TableCmd.CommandText = $"SELECT * FROM drink_water";
            List<DrinkingWater> tableData = new();
            SqliteDataReader reader = TableCmd.ExecuteReader();
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    tableData.Add(new DrinkingWater
                    {
                        Id = reader.GetInt32(0),
                        Date = DateTime.ParseExact(reader.GetString(1), "dd-mm-yy", new CultureInfo("en-US")),
                        Quantity = reader.GetInt32(2)
                    });
                }
            }
            else
            {
                System.Console.WriteLine("No rows found");
            }
            connection.Close();
            System.Console.WriteLine("-------------------------");
            foreach(var dw in tableData)
            {
                System.Console.WriteLine($"{dw.Id} ----- {dw.Date} ---- Quantity -> {dw.Quantity}");
            }
            System.Console.WriteLine("-------------------------");
        }
    }

    private static void Insert()
    {
        string date = GetDateInput();
        int quantity = GetNumberInput("How many coups of water have you drank? (No decimal is allowed)");
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var TableCmd = connection.CreateCommand();
            TableCmd.CommandText = $"INSERT INTO drink_water(date, quantity) VALUES('{date}','{quantity}')";
            TableCmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    internal static int GetNumberInput(string message)
    {
        System.Console.WriteLine(message);
        int _quantity = Convert.ToInt32(System.Console.ReadLine());
        if(_quantity==0) GetDateInput();
        return _quantity;
    }

    internal static string GetDateInput()
    {
        System.Console.WriteLine("Pleas enter the date (format dd-mm-yy). Type 0 to return to the main menu");
        string _date = System.Console.ReadLine();
        if(_date == "0")
        {
            GetDateInput();
        }
        if(!DateTime.TryParseExact(_date, "dd-mm-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
        {
            System.Console.WriteLine("Enter Valid input");
            _date = System.Console.ReadLine();
        }
        return _date;
    }
}