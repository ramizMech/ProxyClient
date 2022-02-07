using System;
using System.Collections.Generic;
using System.Text;

namespace MqttClientPublisher
{
    public class BusinessLogic
    {
        // change implemenation without affecting the rest of the code 

        readonly IRemoteReport reportClient = new RemoteMqttClient();
        public void RequestReport()
        {
            var result = reportClient.GetReport();
            Console.WriteLine("report received");
            ProcessReport(result);
        }

        //format or modify the message in any way you need
        public void ProcessReport(string report)
        {
            Console.WriteLine("Business Report: " + report);
        }
    }
}
