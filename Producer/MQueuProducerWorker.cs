using System.Text.Json;
using Confluent.Kafka;
using KubeMQ.SDK.csharp.Queue;

namespace Producer;

public class MQueuProducerWorker<T>
{
    private readonly string _queueName;
    private readonly string _clientID;
    private readonly string _kubeMQServerAddress;

    KubeMQ.SDK.csharp.Queue.Queue queue = null;

    public MQueuProducerWorker()
    {
        _queueName = "service_log";
        _clientID = "TestClient";
        _kubeMQServerAddress = "localhost:50000";

        queue = new KubeMQ.SDK.csharp.Queue.Queue(_queueName, _clientID, _kubeMQServerAddress);
    }

    public async Task<SendMessageResult> ProduceAsync(T data)
    {
        try
        {
            var dataStream = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray(JsonSerializer.Serialize(data));
            
            var res = queue.SendQueueMessage(new KubeMQ.SDK.csharp.Queue.Message
            {
                Body = dataStream,
                Metadata = "emptyMeta"
            });
            if (res.IsError)
            {
                Console.WriteLine($"message enqueue error, error:{res.Error}");
            }
            else
            {
                Console.WriteLine($"message sent at, {res.SentAt}");
            }

            return res;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}