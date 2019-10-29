using System.ServiceProcess;

namespace LivingSuburb.SportWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}
