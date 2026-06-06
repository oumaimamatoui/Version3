namespace NeoEvaluation.API.Services
{
    public class FailedEmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public interface IFailedEmailQueue
    {
        void Add(FailedEmailMessage message);
        List<FailedEmailMessage> GetAll();
        void Clear();
        int Count { get; }
    }

    public class FailedEmailQueue : IFailedEmailQueue
    {
        private readonly List<FailedEmailMessage> _queue = new List<FailedEmailMessage>();

        public void Add(FailedEmailMessage message)
        {
            lock (_queue)
            {
                _queue.Add(message);
            }
        }

        public List<FailedEmailMessage> GetAll()
        {
            lock (_queue)
            {
                return _queue.ToList();
            }
        }

        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count;
                }
            }
        }
    }
}
