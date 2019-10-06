using System.Collections.Generic;
using Domain;
using EasyNetQ;

namespace Messages
{
    [Queue(nameof(TextMessage))]
    public class TextMessage
    {
        public string Text { get; set; }
    }


    [Queue(nameof(BulkIndexPeople))]
    public class BulkIndexPeople
    {
        public IEnumerable<People> Collection { get; set; }
    }

    [Queue(nameof(BulkUpdatePeople))]
    public class BulkUpdatePeople
    {
        public IEnumerable<PartialPeople> Collection { get; set; }
    }

    [Queue(nameof(UpdatePeople))]
    public class UpdatePeople
    {
        public PartialPeople PartialDoc { get; set; }
    }

    [Queue(nameof(BulkIndexDone))]
    public class BulkIndexDone
    {
        public string Payload { get; set; }
    }
}