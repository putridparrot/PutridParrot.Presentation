using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace ProfilingApp
{
    class Program
    {
        private static int TOTAL = 10000000;

        public static void TestPocoCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonBasicViewModel();
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        public static void TestNotifyPropertyChangedCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonNotifyPropertyChanged();
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        public static void TestExtendedNotifyPropertyChangedCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonExtendedNotifyPropertyChanged();
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        public static void TestViewModelCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonViewModel();
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        public static void TestBasicViewModelChanges()
        {
            var list = new List<PersonBasicViewModel>(TOTAL);
            for (var i = 0; i < TOTAL; i++)
            {
                list.Add(new PersonBasicViewModel());
            }

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                list[i].FirstName = "Scooby";
                list[i].LastName = "Doo";
                list[i].Age = 25;
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public static void TestNotifyPropertyChangedChanges()
        {
            var list = new List<PersonNotifyPropertyChanged>(TOTAL);
            for (var i = 0; i < TOTAL; i++)
            {
                list.Add(new PersonNotifyPropertyChanged());
            }

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                list[i].FirstName = "Scooby";
                list[i].LastName = "Doo";
                list[i].Age = 25;
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public static void TestExtendedNotifyPropertyChangedChanges()
        {
            var list = new List<PersonExtendedNotifyPropertyChanged>(TOTAL);
            for (var i = 0; i < TOTAL; i++)
            {
                list.Add(new PersonExtendedNotifyPropertyChanged());
            }

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                list[i].FirstName = "Scooby";
                list[i].LastName = "Doo";
                list[i].Age = 25;
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public static void TestViewModelChanges()
        {
            var list = new List<PersonViewModel>(TOTAL);
            for (var i = 0; i < TOTAL; i++)
            {
                list.Add(new PersonViewModel());
            }

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                list[i].FirstName = "Scooby";
                list[i].LastName = "Doo";
                list[i].Age = 25;
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private static void Help()
        {
            Console.WriteLine("1. Basic Create, 2. NotifyProp Create, 3. Extended NotifyProp Create, 4. ViewModel Create");
            Console.WriteLine("5. Basic Changes, 6. NotifyProp Changes, 7. Extended NotifyProp Changes, 8. ViewModel Changes");
        }

        static void Main(string[] args)
        {
            string cmd;
            Help();
            while ((cmd = Console.ReadLine()) != null)
            { 
                switch (cmd)
                {
                    case "1":
                        TestPocoCreation();
                        break;
                    case "2":
                        TestNotifyPropertyChangedCreation();
                        break;
                    case "3":
                        TestExtendedNotifyPropertyChangedCreation();
                        break;
                    case "4":
                        TestViewModelCreation();
                        break;
                    case "5":
                        TestBasicViewModelChanges();
                        break;
                    case "6":
                        TestNotifyPropertyChangedChanges();
                        break;
                    case "7":
                        TestExtendedNotifyPropertyChangedChanges();
                        break;
                    case "8":
                        TestViewModelChanges();
                        break;
                }
                Help();
            }
        }
    }
}
