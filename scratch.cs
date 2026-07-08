using System;

class Program
{
    static void Main()
    {
        string hash = BCrypt.Net.BCrypt.HashPassword("Password123!");
        Console.WriteLine(hash);
    }
}
