using System.Diagnostics;

namespace Sample.Services
{
    public class JobService : IJobService
    {
        public void DoWork()
        {
            Debug.WriteLine("Hello world!");
        }
    }
}
