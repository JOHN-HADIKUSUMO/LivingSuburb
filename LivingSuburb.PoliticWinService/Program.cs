using System.ServiceProcess;

namespace LivingSuburb.PoliticWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}