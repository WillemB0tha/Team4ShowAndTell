using Confluent.Kafka;

namespace Producer;

public class MQueuProducerWorker<T>
{
    readonly string? _host;
    readonly int _port;
    readonly string? _topic;

    public MQueuProducerWorker()
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
            return await producer.ProduceAsync(_topic, new Message<Null, T> { Value = data });
        }
    }
}