using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxStorage.BL;
using BoxStorage.Globals;
using BoxStorage.Exceptions;
using BoxStorage.Models;
using System.Threading;



namespace BoxStorage.ClientConsole
{
    class Manager
    {
        Storage _storage;
        Timer queueChecker;
        #region Ctor
        public Manager()
        {
            FromConfig.LoadConfigData();
            _storage = Storage.GetInstanse;
            TimerCallback trash = new TimerCallback(RunTrash);
            queueChecker = new Timer(trash, null, new TimeSpan(0), FromConfig.CheckingUnusedBox);
        }
        #endregion


        #region Trash-Timer
        private void RunTrash(object obj)
        {
            Respone<List<Box>> feedback = _storage.MoveToTrash();
            if (feedback.IsWorking)
            {
                Console.WriteLine();
                Console.WriteLine("---------------------------------");
                Console.WriteLine($"!!! Deleted {feedback.Data.Count} box(es) !!!");

                foreach (var item in feedback.Data)
                    Console.WriteLine(item);
                Console.WriteLine("---------------------------------");
            }
        }

#endregion

        #region Menu

        // Menu
        public void Menu()
        {
            Console.WriteLine();
            Console.WriteLine("------------------------------------ Menu ------------------------------------");
            Console.WriteLine("1: Add Box / 2: Get Box for Gift / 3: Explore Storage / 4: Explore Trash-Queue");
            Console.Write("Enter number: ");
            int select;
            if (!int.TryParse(Console.ReadLine(), out select))
                Menu();
            else
            {
                switch (select)
                {
                    case 1:
                        AddBox();
                        break;
                    case 2:
                        GetBox();
                        break;
                    case 3:
                        PrintStorage();
                        break;
                    case 4:
                        PrintQueue();
                        break;
                    default:
                        Menu();
                        break;
                }
            }

            Menu();
        }

        #endregion

        #region See All Data
        // Explore Storage
        private void PrintStorage()
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------- Storage ----------------------------------");
            if (!_storage.IsEmpty)
            {
                var list = _storage.StorageInOrder();
                foreach (var item in list)
                {
                    Console.WriteLine(item);
                }

            }
            else
                Console.WriteLine("Empty...");

            Console.ReadLine();
        }

        // Explore Trash-Queue
        private void PrintQueue()
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------- Trash-Queue --------------------------------");
            if (!_storage.IsEmpty)
            {
                var list = _storage.QueueToTrashInOrder();
                foreach (var item in list)
                {
                    Console.WriteLine(item);
                }
            }
            else
                Console.WriteLine("Empty...");

            Console.ReadLine();
        }

        #endregion

        #region Get Box
        private void GetBox()
        {
            int bottom = 0;
            int height = 0;
            string input = "";
            Respone<Box> searchResult = new Respone<Box>();

            Console.WriteLine();
            Console.WriteLine("----------------------------------- Get Box ----------------------------------");
            Console.WriteLine("Enter size of the gift (for back to menu enter \"M\")");

            // Get 1 Box
            while (input != "M" && input != "m")
            {
                // Get Input
                do
                {
                    Console.Write("Enter size of bottom-side: ");
                    input = Console.ReadLine();
                    if (input == "M" || input == "m")
                        break;
                } while (!int.TryParse(input, out bottom));

                if (input == "M" || input == "m")
                    break;

                do
                {
                    Console.Write("Enter height: ");
                    input = Console.ReadLine();
                    if (input == "M" || input == "m")
                        break;

                } while (!int.TryParse(input, out height));

                if (input == "M" || input == "m")
                    break;

                // Search
                try
                {
                    Gift gift = new Gift(bottom, height);
                    searchResult = _storage.TryPullBox(gift);
                    if (searchResult.Message != null)
                        Console.WriteLine(searchResult.Message);
                    if (searchResult.IsWorking)
                        Console.WriteLine(searchResult.Data);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine("---\nWant get another box? (For back to menu enter \"M\")\n---");
                input = Console.ReadLine();
            }

        }

        #endregion

        #region Add Box
        private void AddBox()
        {
            int bottom=0;
            int height=0;
            int quantity;
            string input = "";
            Box newBox;
            Respone<Box> feedback = new Respone<Box>();

            Console.WriteLine();
            Console.WriteLine("----------------------------------- Add Box ----------------------------------");
            Console.WriteLine("For back to menu enter \"M\"");

            // Add 1 Box
            while(input!="M" && input != "m")
            {
                // Get Input

                do
                {
                    Console.Write("Enter size of bottom-side: ");
                    input = Console.ReadLine();
                    if (input == "M" || input == "m")
                        break;
                } while (!int.TryParse(input, out bottom));

                if(input=="M" || input == "m")
                    break;

                do
                {
                    Console.Write("Enter height: ");
                    input = Console.ReadLine();
                    if (input == "M" || input == "m")
                        break;

                } while (!int.TryParse(input, out height));

                if (input == "M" || input == "m")
                    break;

                Console.Write("Enter quantity: ");
                input = Console.ReadLine();


                if(!int.TryParse(input, out quantity))
                {
                    if (input == "M" || input == "m")
                        break;
                    else
                    {
                        try
                        {
                            // creating box (with counter 1) & adding to storage
                            newBox = new Box(bottom, height);
                            feedback = _storage.Insert(newBox);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        // creating box (with some entered counter) & adding to storage
                        newBox = new Box(bottom, height, quantity);
                        feedback = _storage.Insert(newBox);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                // lett to user feedback about action
                if (!feedback.IsWorking)
                    Console.WriteLine(feedback.Message);
                else
                    Console.WriteLine("Added! =)");

                Console.WriteLine("---\nWant add another box? (For back to menu enter \"M\")\n---");
                input = Console.ReadLine();
            }

        }



        #endregion
    }
}
