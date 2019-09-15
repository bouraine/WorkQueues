using EasyNetQ;

namespace Messages
{
    [Queue("WQ.TextMessage", ExchangeName = "WQ.Client")]
    public class TextMessage
    {
        public string Text { get; set; }
    }
}