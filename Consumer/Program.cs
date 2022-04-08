using Consumer;

//var consumer = new ConsumerWorker<Feed>();
//await consumer.ConsumeAsync();

var kubeConsume = new ConsumerWorkerMQ<Feed>();
await  kubeConsume.ConsumeAsync();