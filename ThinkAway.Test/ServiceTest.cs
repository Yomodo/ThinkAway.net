using System;
using ThinkAway.Plus.Services.Service;
using ThinkAway.Plus.Services.Tasks;

namespace ThinkAway.Test
{
    class ServiceTest : Service
    {
        public ServiceTest() : base("ThinkAway.Test")
        {
            ServiceTasks.Add(new MyTask(30,false));
        }

        public class MyTask : CyclicServiceTask
        {
            public MyTask(int intervalSeconds, bool synchronous) : base(intervalSeconds, synchronous)
            {
            }

            protected override void RunTask()
            {
                Console.Beep(10,10);
            }
        }
    }
}
