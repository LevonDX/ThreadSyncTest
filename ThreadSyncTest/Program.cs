using System.Runtime.CompilerServices;

namespace ThreadSyncTest
{
    internal class Program
    {
        static int[] array = new int[10];
        static int currentIndex = 0;

        private readonly static object o = new object();

        static void FillArray()
        {
            lock (o)
            {
                int sum = 0;
                for (int i = 1; i <= 100; i++)
                {
                    sum += i;
                    if (i % 10 == 0)
                    {
                        array[currentIndex] = sum;
                        sum = 0;

                        Monitor.PulseAll(o);
                        Monitor.Wait(o);
                    }
                }

                Monitor.PulseAll(o);
            }
        }

        static void Move()
        {
            lock (o)
            {
                do
                {
                    currentIndex++;

                    Monitor.PulseAll(o);
                    Monitor.Wait(o);

                } while (currentIndex < array.Length - 1);

                Monitor.PulseAll(o);
            }
        }

        static void Main(string[] args)
        {
            Thread t1 = new Thread(FillArray);
            t1.Name = "Fill Array";

            Thread t2 = new Thread(Move);
            t2.Name = "Move";

            t1.Start();
            t2.Start();

            t2.Join();
            t1.Join();

            array.ToList().ForEach(Console.WriteLine);
        }
    }
}