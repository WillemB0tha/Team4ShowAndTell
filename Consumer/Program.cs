using Consumer;

var consumer = new ConsumerWorker<Feed>();
await consumer.ConsumeAsync();