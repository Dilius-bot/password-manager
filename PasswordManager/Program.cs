using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using PasswordManager.Models;
using PasswordManager.Services;

int loginCount = 3;
PasswordServices? service = null;
string masterPass = "";

while (loginCount != 0)
{
    Console.WriteLine("Введите МАСТЕР-ПАРОЛЬ для доступа к базе: ");
    masterPass = ReadPassword();
    
    Console.WriteLine("Повторите МАСТЕР-ПАРОЛЬ для доступа к базе: ");
    string? masterPassRepeat = ReadPassword();

    if (masterPass == masterPassRepeat)
    {
        byte[] secretKey = GenerateKeyFromMasterPassword(masterPass);
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string folderPath = Path.Combine(baseDir, "Password");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, "password.txt");

        service = new PasswordServices(filePath, secretKey);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Вы успешно вошли в базу данных. Доступ разрешён! ===");
        Console.ResetColor();
        Console.WriteLine("=== Проффесиональный менеджер паролей ===");
        break;

    } 
    else
    {
        loginCount -= 1;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"=== Пароли не совпадают. Осталось попыток: {loginCount}");
        Console.ResetColor();
    }
}

if (service == null)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Доступ заблокирован. Завершение работы!");
    return;
}

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n1. Добавить\n2. Удалить\n3. Найти\n4. Показать всё\n5. Выход\n6. Удалить базу данных");
    Console.ResetColor();

    string? choice = Console.ReadLine();
    if (choice == "1") AddService();
    else if (choice == "2") DeleteService();
    else if (choice == "3") SearchSevice();
    else if (choice == "4") ShowAll();
    else if (choice == "5")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Вы уверены, что хотите выйти из базы? Введите y - если да, любая клавиша - нет: ");
        Console.ResetColor();

        if (Console.ReadLine()?.ToLower() == "y")
        {
            break;
        }
        else Console.WriteLine("Выход отменён");
    }      
    else if (choice == "6") RemoveDataBase();
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Введено не корректное действие");
        Console.ResetColor();
    }
}

void AddService()
{
    var entry = new PasswordEntry();
    Console.WriteLine("Введите название сервиса: ");
    entry.Service = Console.ReadLine() ?? "";

    Console.WriteLine("Введите пароль от сервиса");
    entry.Password = ReadPassword();
    Console.WriteLine("Повторите пароль от сервиса");
    string password = ReadPassword();

    if (password != entry.Password)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Пароли не совпадают");
        Console.ResetColor();
        return; 
    }
   
    var result = service.Add(entry);

    if (result.Success)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(result.Message);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Ошибка: {result.Message}");
    }
    Console.ResetColor();
}

void DeleteService()
{
    Console.WriteLine("Введите название сервиса для его удаления");
    string serv = Console.ReadLine() ?? "";
    Console.WriteLine("Введите пароль от сервиса для его удаления");
    string pass = ReadPassword();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Введите МАСТЕР-ПАРОЛЬ для удаления пароля");
    Console.ResetColor();
    string? masterPassRepeat = ReadPassword();

    if (masterPassRepeat == masterPass)
    {
        var result = service.DeleteByPassword(serv, pass);

        if (result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(result.Message);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: {result.Message}");
        }
        Console.ResetColor();
    } 
    else Console.WriteLine("Неверный МАСТЕР-ПАРОЛЬ");
}

void SearchSevice()
{
    Console.WriteLine("Введите пароль от сервиса для его поиска");
    string pass = ReadPassword();

    var result = service.SearchByPassword(pass);

    if (result != null && result.Count > 0)
    { 
        Console.ForegroundColor = ConsoleColor.Green;
        foreach (var item in result)
        {
            Console.WriteLine($"Найдено! Сервис: {item.Service}, Пароль: {item.Password}");
        }
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Поиск по паролю не нашел совпадений");
        Console.ResetColor();
    }

}

void ShowAll()
{
    var data = service.GetAll();

    if (data.Count == 0) Console.WriteLine("База пуста");
    else data.ForEach(item =>  Console.WriteLine(item));
}

void RemoveDataBase()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Вы уверены, что хотите удалить базу данных? Введите y - если да, любая клавиша - нет: ");
    Console.ResetColor();

    if(Console.ReadLine()?.ToLower() == "y")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Введите МАСТЕР-ПАРОЛЬ для удаления пароля");
        Console.ResetColor();
        string? masterPassRepeat = ReadPassword();
        if (masterPassRepeat == masterPass)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Точно хотите удалить базу данных? Введите y - если да, любая клавиша - нет: ");

            if (Console.ReadLine()?.ToLower() == "y")
            {
                var result = service.DeleteDataBase();
                if (result.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(result.Message);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Message);
                }
            }
            else Console.WriteLine("Удаление отменено");
        }
        else Console.WriteLine("Неверный МАСТЕР-ПАРОЛЬ");
        Console.ResetColor();
    }
    else Console.WriteLine("Удаление отменено");
}

byte[] GenerateKeyFromMasterPassword(string masterPassword)
{
    byte[] salt = Encoding.UTF8.GetBytes("gH7@xP9!vR2#nT5$");

    return Rfc2898DeriveBytes.Pbkdf2(
        masterPassword,
        salt,
        iterations: 100000,
        hashAlgorithm: HashAlgorithmName.SHA256,
        outputLength: 32
     );
}

string ReadPassword()
{
    string password = "";

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (password.Length > 0)
            {
                password = password.Remove(password.Length - 1);
                Console.Write("\b \b");
            }
        }
        else
        {
            password += key.KeyChar;
            Console.Write("*");
        }
    }
    return password;
}
