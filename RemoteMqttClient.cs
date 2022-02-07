using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace MqttClientPublisher
{
    public class RemoteMqttClient : IRemoteReport
    {
        static int id = 1;
        private IMqttClient client;
        private readonly Queue<MqttApplicationMessageReceivedEventArgs> messageQueue = new Queue<MqttApplicationMessageReceivedEventArgs>();
        public RemoteMqttClient()
        {
            InitClient();
        }

        public string GetReport()
        {
            //                        
            //Thread.Sleep(2000);

            //callback needed
            Publish(client, id);

            string report = null;

            //poll the queue until the message comes in
            while (report == null)
            {
                if (messageQueue.Count > 0)
                {
                    var msg = messageQueue.Dequeue();
                    report = Encoding.UTF8.GetString(msg.ApplicationMessage.Payload);
                }                
            }
             
            id++;

            return report;
        }

        private void InitClient()
        {
            var factory = new MqttFactory();
            client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("Publisher1")
                .WithTcpServer("localhost", 1884)
                .WithCredentials("admin", "admin")
                .WithCleanSession()
                .Build();

            client.UseConnectedHandler(e => {
                Console.WriteLine("Connected successfully to MQTT Brokers");
                //Subscribe to Topic

                client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("testResponse").Build()).Wait();
            });
            client.UseDisconnectedHandler(e => {
                Console.WriteLine("Disconnected from MQTT Brokers");
            });
            client.UseApplicationMessageReceivedHandler(e => ReplyHandler(e));
            
            client.ConnectAsync(options).Wait();
        }

        private void ReplyHandler(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("Received Message Reply");
            Console.WriteLine($"Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine($"QoS {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine();

            messageQueue.Enqueue(e);
            Console.WriteLine("added to queue");
        }


        public void Publish(IMqttClient client, int id)
        {
            var message = new MqttApplicationMessageBuilder()
                                .WithTopic("test")
                                .WithPayload($"Payload: {DateTime.Now}, messageId: {id}")
                                .WithExactlyOnceQoS()
                                .WithRetainFlag()
                                .Build();

            if (client.IsConnected)
            {
                client.PublishAsync(message);
            }
        }
    }
}
