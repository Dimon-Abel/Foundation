using System;
using System.Threading.Tasks.Dataflow;

namespace DataFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<double>();
            var actionBlock = new ActionBlock<double>(data => Console.WriteLine($"input:{data + 1}"));
            for (var i = 0; i < 3; i++)
            {
                var value = new Random().NextDouble();
                bufferBlock.Post(value);
                //actionBlock.Post(value);
            }
            //for (int i = 0; i < 3; i++)
            //Console.WriteLine($"bufferBlock:{bufferBlock.Receive()}");

            bufferBlock.LinkTo(actionBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            bufferBlock.Complete();
            Console.ReadKey();
            //actionBlock.Complete();
        }
    }
}
