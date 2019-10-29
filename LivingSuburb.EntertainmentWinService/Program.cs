using System.ServiceProcess;

namespace LivingSuburb.EntertainmentWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}