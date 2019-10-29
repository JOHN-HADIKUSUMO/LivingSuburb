using System.ServiceProcess;

namespace LivingSuburb.HealthWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}
