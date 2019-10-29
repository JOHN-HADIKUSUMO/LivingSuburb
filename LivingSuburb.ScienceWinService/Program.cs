using System.ServiceProcess;

namespace LivingSuburb.ScienceWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}
