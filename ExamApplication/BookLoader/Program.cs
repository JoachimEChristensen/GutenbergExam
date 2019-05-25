using System;
using System.IO;

namespace BookLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\Desktop\kage\Downloads\archive\root\zipfiles\1jcfs10.txt";
            string readText = File.ReadAllText(path);
            //Console.WriteLine(readText);
        }
    }
}
