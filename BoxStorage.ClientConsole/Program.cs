using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.Exceptions;
using BoxStorage.Models;
using BoxStorage.BL;

namespace BoxStorage.ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager storageManager = new Manager();
            storageManager.Menu();

            Console.ReadLine();
        }
    }
}
