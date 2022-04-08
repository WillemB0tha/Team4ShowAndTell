using System.ComponentModel;
using Confluent.Kafka;

namespace Consumer;

public class ConsumerWorker<T>
{
    private readonly string? _host;
    private readonly int _port;
    private readonly string _topic;

    public ConsumerWorker()
    {
        _host = "localhost";
        _port = 9092;
        _topic = "ProducerLog";
    }

    ConsumerConfig GetConsumerConfig()
    {
        return new ConsumerConfig()
        {
            BootstrapServers = $"{_host}:{_port}",
            GroupId = "CornerData",
            AutoOffsetReset = AutoOffsetReset.Earliest;
        };
    }

    public async Task Consume()
    {
        using (var consumer = new ConsumerBuilder<Ignore, T>(GetConsumerConfig())
                   .SetValueDeserializer(new ValueDeserializer<T>()).Build())
        {
            consumer.Subscribe(_topic);

            await Task.Run(() =>
            {
                while (true)
                {
                    var consumerResult = consumer.Consume(default(CancellationToken));
                    if (consumerResult.Message.Value is Feed)
                    {
                        //Save Here
                    }
                }
            });
            
            consumer.Close();
        }
    }
    
    
}