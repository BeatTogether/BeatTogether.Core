namespace BeatTogether.Core.Messaging.Configuration
{
    public class MessagingConfiguration
    {
        public int RequestTimeout { get; set; } = 10000;
        public int MaximumRequestRetries { get; set; } = 5;
        public int RequestRetryDelay { get; set; } = 500;
        public RabbitMQConfiguration RabbitMQ { get; set; } = new RabbitMQConfiguration();
    }
}
