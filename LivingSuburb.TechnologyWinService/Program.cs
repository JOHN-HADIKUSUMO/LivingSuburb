using System.ServiceProcess;

namespace LivingSuburb.TechnologyWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}
