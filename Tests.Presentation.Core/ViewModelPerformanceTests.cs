using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Presentation.Patterns;
using Presentation.Patterns.Helpers;

namespace Tests.Presentation
{
    // TODO:
    // 4. IConvertible - see about using instead of Convert.ChangeType where appropriate
    // 5. Maybe look at commands
    // 6. Should I implement async capable properties which could hook into busy flag etc. 
    // 7. Remove all backingstore code from PF, ready to try to use this code and see what goes wrong :)

    // need to try some performance tests, i.e. create 1mil POCO's and compare perf. against 1mil view model objects
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ViewModelPerformanceTests
    {
        private const int TOTAL = 1000000;

        private class PersonBasicViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string _firstName;
            private string _lastName;
            private int _age;

            public string FirstName
            {
                get { return _firstName; }
                set
                {
                    if (_firstName != value)
                    {
                        _firstName = value;
                        OnPropertyChanged();
                    }
                }
            }

            public string LastName
            {
                get { return _lastName; }
                set
                {
                    if (_lastName != value)
                    {
                        _lastName = value;
                        OnPropertyChanged();
                    }
                }
            }

            public int Age
            {
                get { return _age; }
                set
                {
                    if (_age != value)
                    {
                        _age = value;
                        OnPropertyChanged();
                    }
                }
            }

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class PersonNotifyPopertyChanged : NotifyPropertyChanged
        {
            private string _firstName;
            private string _lastName;
            private int _age;

            public string FirstName
            {
                get { return _firstName; }
                set { this.SetProperty(ref _firstName, value); }
            }

            public string LastName
            {
                get { return _lastName; }
                set { this.SetProperty(ref _lastName, value); }
            }

            public int Age
            {
                get { return _age; }
                set { this.SetProperty(ref _age, value); }
            }

        }

        public class PersonExtendedNotifyPropertyChanged : ExtendedNotifyPropertyChanged
        {
            private string _firstName;
            private string _lastName;
            private int _age;

            public string FirstName
            {
                get { return _firstName; }
                set { this.SetProperty(ref _firstName, value); }
            }

            public string LastName
            {
                get { return _lastName; }
                set { this.SetProperty(ref _lastName, value); }
            }

            public int Age
            {
                get { return _age; }
                set { this.SetProperty(ref _age, value); }
            }

        }

        public class PersonViewModel : ViewModel
        {
            public string FirstName
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }

            public string LastName
            {
                get { return GetProperty<string>(); }
                set { SetProperty(value); }
            }

            public int Age
            {
                get { return GetProperty<int>(); }
                set { SetProperty(value); }
            }
        }

        public class PersonViewModelWithBacking : ViewModelWithoutBacking
        {
            private string _firstName;
            private string _lastName;
            private int _age;

            public string FirstName
            {
                get { return GetProperty(ref _firstName); }
                set { SetProperty(ref _firstName, value); }
            }

            public string LastName
            {
                get { return GetProperty(ref _lastName); }
                set { SetProperty(ref _lastName, value); }
            }

            public int Age
            {
                get { return GetProperty(ref _age); }
                set { SetProperty(ref _age, value); }
            }
        }

        [Test(Description = "No asserts, this just gives us a comparison for TestViewModelCreation")]
        //[Repeat(TOTAL)]
        [MaxTime(100)]
        [Timeout(5000)]
        public void TestPocoCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonBasicViewModel();
            }

            sw.Stop();
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        //[Repeat(1)]
        [MaxTime(2000)]
        [Timeout(5000)]
        public void TestNotifyPopertyChangedCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonNotifyPopertyChanged();
            }

            sw.Stop();
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        //[Repeat(1)]
        [MaxTime(2000)]
        [Timeout(5000)]
        public void TestExtendedNotifyPropertyChangedCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonExtendedNotifyPropertyChanged();
            }

            sw.Stop();
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        //[Repeat(1)]
        [MaxTime(2000)]
        [Timeout(5000)]
        public void TestViewModelCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonViewModel();
            }

            sw.Stop();
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        //[Repeat(1)]
        [MaxTime(2000)]
        [Timeout(5000)]
        public void TestViewModelWithoutBackingCreation()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < TOTAL; i++)
            {
                var o = new PersonViewModelWithBacking();
            }

            sw.Stop();
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test(Description = "No asserts, this just gives us a comparison for TestViewModelCreation")]
        public void TestPocoChanges()
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
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        public void TestNotifyPopertyChangedChanges()
        {
            var list = new List<PersonNotifyPopertyChanged>(TOTAL);
            for (var i = 0; i < TOTAL; i++)
            {
                list.Add(new PersonNotifyPopertyChanged());
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
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        public void TestExtendedNotifyPropertyChangedChanges()
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
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        public void TestViewModelChanges()
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
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        public void TestViewModelWithoutBackingChanges()
        {
            var list = new List<PersonViewModelWithBacking>(TOTAL);
            for (var i = 0; i < TOTAL; i++)
            {
                list.Add(new PersonViewModelWithBacking());
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
            Console.Write(sw.ElapsedMilliseconds);
        }

        [Test]
        public void Test_ContainsKeyGet()
        {
            var d = new Dictionary<string, string>();
            for (var i = 0; i < 100000; i++)
            {
                d.Add(i.ToString(), (i * 100).ToString());
            }

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < 100000; i++)
            {
                var key = i.ToString();
                if (d.ContainsKey(key))
                {
                    var v = d[key];
                }
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public void Test_TryGetValue()
        {
            var d = new Dictionary<string, string>();
            for (var i = 0; i < 100000; i++)
            {
                d.Add(i.ToString(), (i * 100).ToString());
            }

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < 100000; i++)
            {
                d.TryGetValue(i.ToString(), out var _);
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
