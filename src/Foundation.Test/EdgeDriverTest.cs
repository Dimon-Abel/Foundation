using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit.Sdk;

namespace Foundation.Test
{
    [TestClass]
    public class EdgeDriverTest
    {
        [TestMethod]
        public void MQSendMessage()
        {
        }

        private int GetCarGroupCount(double target, Dictionary<string, double> cars)
        {
            Referee.Instance.ReStart();
            
            var buffer = new BufferBlock<KeyValuePair<string, double>>();
            Parallel.ForEach(cars, car => buffer.Post(car));

            var action = new ActionBlock<KeyValuePair<string, double>>(item => DriveCar(item.Key, item.Value, target));

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
