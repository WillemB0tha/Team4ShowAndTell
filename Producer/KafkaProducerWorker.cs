using Confluent.Kafka;

namespace Producer;

public class KafkaProducerWorker<T>
{
    readonly string? _host;
    readonly int _port;
    readonly string? _topic;

    public ProducerWorker()
    {
        _host = "localhost";
        _port = 9092;
        _topic = "producer_logs";
    }

    ProducerConfig GetProducerConfig()
    {
        return new ProducerConfig
        {
            BootstrapServers = $"{_host}:{_port}"
        };
    }

    public async Task<DeliveryResult<Null, T>> ProduceAsync(T data)
    {
        using (var producer = new ProducerBuilder<Null, T>(GetProducerConfig())
                   .SetValueSerializer(new ValueSerializer<T>())
                   .Build())
        {
            Console.WriteLine("\nProducer loop started...\n\n");

            var message = $"Character #{character} sent at {DateTime.Now:yyyy-MM-dd_HH:mm:ss}";


            var deliveryReport = await producer.ProduceAsync(_topic, new Message<Null, T> { Value = z });

            Console.WriteLine($"Message sent (value: '{message}'). Delivery status: {deliveryReport.Status}");

            if (deliveryReport.Status != PersistenceStatus.Persisted)
            {
                // delivery might have failed after retries. This message requires manual processing.
                Console.WriteLine(
                    $"ERROR: Message not ack'd by all brokers (value: '{message}'). Delivery status: {deliveryReport.Status}");
            }

            return z;
        }
    }
}