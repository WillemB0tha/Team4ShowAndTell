using System.ComponentModel;
using System.Text.Json;
using Confluent.Kafka;

namespace Consumer;

public class ConsumerWorkerMQ<T>
{
    private readonly string _queueName;
    private readonly string _clientID;
    private readonly string _kubeMQServerAddress;

    private readonly KubeMQ.SDK.csharp.Queue.Queue _queue = null;

    public ConsumerWorkerMQ()
    {
        _queueName = "service_log";
        _clientID = "TestClient";
        _kubeMQServerAddress = "localhost:50000";

        _queue = new KubeMQ.SDK.csharp.Queue.Queue(_queueName, _clientID, _kubeMQServerAddress);
    }

    public async Task ConsumeAsync()
    {
        try
        {
            var msg = _queue.ReceiveQueueMessages();
            if (msg.IsError)
            {
                Console.WriteLine($"message dequeue error, error:{msg.Error}");
                return;
            }

            Console.WriteLine($"Received {msg.MessagesReceived} Messages:");
            foreach (var item in msg.Messages)
            {
                Console.WriteLine(
                    $"MessageID: {item.MessageID}, Body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(item.Body)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}