using System.ServiceProcess;

namespace LivingSuburb.GeneralWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}