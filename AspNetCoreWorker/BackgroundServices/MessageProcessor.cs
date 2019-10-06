using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using EasyNetQ;
using EasyNetQ.FluentConfiguration;
using Elasticsearch.Indexers;
using Messages;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreWorker
{
    public class MessageProcessor : BackgroundService
    {
        private readonly IBus _bus;
        private readonly DocumentIndexer<People> _peopleIndexer;
        private readonly ClientIndexer _clientIndexer;

        public MessageProcessor(IBus bus, DocumentIndexer<People> peopleIndexer,
            ClientIndexer clientIndexer1)
        {
            _bus = bus;
            _peopleIndexer = peopleIndexer;
            _clientIndexer = clientIndexer1;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            void SubscriptionConfig(ISubscriptionConfiguration config) => config.WithPrefetchCount(30);

            _bus.SubscribeAsync<BulkIndexPeople>("aspWorker", OnBulkIndexReceived, SubscriptionConfig);
            _bus.SubscribeAsync<BulkUpdatePeople>("aspWorker", OnUpdateBulkPeopleReceived);
            _bus.SubscribeAsync<UpdatePeople>("aspWorker", OnUpdatePeopleReceived);
            _bus.SubscribeAsync<BulkIndexDone>("aspWorker", OnBulkIndexDoneReceived);
            _bus.SubscribeAsync<string>("aspWorker", OnIndexClientsReceived);
            return Task.CompletedTask;
        }

        private async Task OnIndexClientsReceived(string arg) => await _clientIndexer.IndexClients();


        private async Task OnBulkIndexReceived(BulkIndexPeople msg)
        {
            Console.WriteLine(
                $"message bulk index received: last item id : {(msg.Collection.Last().Id)} ");
            await _peopleIndexer.BulkIndex(msg.Collection, null, false);
        }

        private async Task OnUpdateBulkPeopleReceived(BulkUpdatePeople msg)
        {
            Console.WriteLine("message bulk update: " + msg.Collection.ToList().Count + " received");
            await _peopleIndexer.BulkUpdate(msg.Collection);
        }

        private async Task OnUpdatePeopleReceived(UpdatePeople msg)
        {
            Console.WriteLine("message update: " + msg.PartialDoc.Id + " received");
            if (!string.IsNullOrEmpty(msg?.PartialDoc?.Id))
                await _peopleIndexer.UpdateDocument(msg.PartialDoc, msg.PartialDoc.Id);
        }

        private async Task OnBulkIndexDoneReceived(BulkIndexDone obj)
        {
            Console.WriteLine("Switching index received");
            await _peopleIndexer.SwitchIndex();
        }
    }
}