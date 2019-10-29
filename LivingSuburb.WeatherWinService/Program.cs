using System.ServiceProcess;

namespace LivingSuburb.WeatherWinService
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new RunServices());
        }
    }
}