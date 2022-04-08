using Confluent.Kafka;

namespace Producer;

public class KafkaProducerWorker<T>
{
    readonly string? _host;
    readonly int _port;
    readonly string? _topic;

    public KafkaProducerWorker()
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
            var deliveryReport = await producer.ProduceAsync(_topic, new Message<Null, T> { Value = data });
            if (deliveryReport.Status != PersistenceStatus.Persisted)
            {
                // delivery might have failed after retries. This message requires manual processing.
                Console.WriteLine(
                    $"ERROR: Message not ack'd by all brokers (value: '{data}'). Delivery status: {deliveryReport.Status}");
            }

            return deliveryReport;
        }
    }
}