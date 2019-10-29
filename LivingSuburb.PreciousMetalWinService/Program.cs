using System.ServiceProcess;

namespace LivingSuburb.PreciousMetalWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}