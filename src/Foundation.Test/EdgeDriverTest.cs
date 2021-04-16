using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit.Sdk;

namespace Foundation.Test
{
    [TestClass]
    public class EdgeDriverTest
    {
        public class ClassA
        {
            public string Name { get; set; }
        }

        public class ClassB
        {
            public string Name { get; set; }
        }

        [TestMethod]
        public void MQSendMessage()
        {
            List<ClassA> lst1 = new List<ClassA>();
            List<ClassB> lst2 = new List<ClassB>();
            for (int i = 0; i < 5; i++)
            {
                lst1.Add(new ClassA() { Name = new Random().NextDouble().ToString() });
                lst2.Add(new ClassB() { Name = new Random().NextDouble().ToString() });
                //if (i == 3)
                //{
                //    lst1.Add(new ClassA() { Name = "Class" });
                //    lst2.Add(new ClassB() { Name = "Class" });
                //}
            }

            var state = Comparer(lst1, lst2, (a, b) => a.Name.Equals(b.Name));
            Console.ReadKey();
        }

        public bool Comparer<T, V>(ICollection<T> lst1, ICollection<V> lst2, Func<T, V, bool> func)
        {
            foreach (var item in lst1)
                foreach (var item2 in lst2)
                    if (func.Invoke(item, item2))
                        return true;
            return false;
        }

        private int GetCarGroupCount(double target, Dictionary<string, double> cars)
        {
            Referee.Instance.ReStart();

            var buffer = new BufferBlock<KeyValuePair<string, double>>();
            Parallel.ForEach(cars, car => buffer.Post(car));

            var action = new ActionBlock<KeyValuePair<string, double>>(item => DriveCar(item.Key, item.Value, target));
            return 1;
        }

        private void DriveCar(string carName, double speed, double target)
        {

        }

        class Referee
        {
            private Referee() { }
            private static Referee _referee;
            private List<CarGroup> _carGroups;
            public static Referee Instance
            {
                get
                {
                    if (_referee == null)
                    {
                        _referee = new Referee();
                        _referee._carGroups = new List<CarGroup>();
                    }
                    return _referee;
                }
            }
            public void ReStart() => _carGroups.Clear();
        }

        class CarGroup
        {
            private List<string> _cars { get; set; }
            public void Join(string carName) => _cars.Add(carName);
        }
    }
}
