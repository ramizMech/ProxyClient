
using System;
using System.Text;
using System.Threading;

namespace MqttClientPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Publisher Mqtt!");

            BusinessLogic logic = new BusinessLogic();


            Console.WriteLine("Press Escape to exit, press 'p' to get a report!");
            ConsoleKeyInfo keyInfo; 
            do
            {
                keyInfo = Console.ReadKey();
                Console.WriteLine("you pressed: " + keyInfo.Key);
                if (keyInfo.Key == ConsoleKey.P)
                {
                    Console.WriteLine("getting the report");
                    //call the business logic
                    logic.RequestReport();
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
            
            
        }

        
    }
}
