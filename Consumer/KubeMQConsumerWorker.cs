using System.ComponentModel;
using System.Text.Json;
using Confluent.Kafka;

namespace Consumer;

public class ConsumerWorker<T>
{
    private readonly string? _host;
    private readonly int _port;

    private readonly string _queueName;
    private readonly string KubeMQServerAddress;

    public ConsumerWorker()
    {
        KubeMQServerAddress = "localhost";
        _port = 50000;
        _queueName = "queue-dead-letter";
    }

    ConsumerConfig GetConsumerConfig()
    {
        return new ConsumerConfig()
        {
            BootstrapServers = $"{_host}:{_port}",
            GroupId = "Foo",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    public async Task ConsumeAsync()
    {
        KubeMQServerAddress = "localhost";

        var receiver = new KubeMQ.SDK.csharp.Queue.Queue("queue", "Csharp-sdk-cookbook-queues-stream-client",
                KubeMQServerAddress + ":" + port);
        var transaction = receiver.CreateTransaction();
        KubeMQ.SDK.csharp.Queue.Stream.TransactionMessagesResponse resRec;
        try
        {
            resRec = transaction.Receive(1, 5);
            if (resRec.IsError)
            {
                Console.WriteLine($"Message dequeue error, error:{resRec.Error}");
                transaction.Close();

            }
            else
            {
                Console.WriteLine(
                    $"message received, body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(resRec.Message.Body)}, rejecting");
                try
                {
                    var rejRes = transaction.RejectMessage(resRec.Message.Attributes.Sequence);
                    if (rejRes.IsError)
                    {
                        Console.WriteLine($"Error in reject Message, error:{rejRes.Error}");
                    }
                    else
                    {
                        Console.WriteLine($"Reject completed");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    transaction.Close();
                }
            }

        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Message Receive error, error:{ex.Message}");
        }

        try
        {
            var msg = queue.Pull("queue-dead-letter", 1, 1);
            if (msg.IsError)
            {
                Console.WriteLine($"message dequeue error, error:{msg.Error}");
            }

            {
                Console.WriteLine($"{msg.Messages.Count()} messages received from dead-letter queue");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }
}