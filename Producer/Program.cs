using System;
using System.Linq;
using Domain;
using EasyNetQ;
using Messages;

namespace Producer
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            const string connectionString = "host=localhost;username=user;password=password";
            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                var input = "";
                Console.WriteLine("Enter a message. 'Quit' to quit.");
                while ((input = Console.ReadLine()) != "Quit")
                {
                    switch (input)
                    {
                        case "loop":
                            BulkIndex(bus);
                            break;
                        case "loop-one":
                            IndexPeople(bus);
                            break;
                        case "loop-update":
                            BulkUpdate(bus);
                            break;
                        case "done":
                            BulkIndexDone(bus);
                            break;
                        case "client":
                            bus.Publish("hello");
                            break;
                        default:
                            UpdateOne(bus, input);
                            break;
                    }
                }
            }
        }

        private static void IndexOne(IBus bus, string input)
        {
            bus.Publish(new BulkIndexPeople
            {
                Collection = new[]
                {
                    new People
                    {
                        Id = input,
                        Name = "peole" + input,
                    }
                }
            });
        }

        private static void IndexPeople(IBus bus)
        {
            for (var i = 0; i < 200000; i++)
            {
                bus.Publish(new BulkIndexPeople
                {
                    Collection = new[]
                    {
                        new People
                        {
                            Id = i.ToString(),
                            Name = "peole" + i,
                        }
                    }
                });
            }
        }

        private static void BulkIndexDone(IBus bus)
        {
            bus.Publish(new BulkIndexDone {Payload = "done"});
            Console.WriteLine("Switching index send");
        }

        private static void BulkIndex(IBus bus)
        {
            var chunks = Enumerable.Range(0, 200000).Chunk(200).ToArray();
            Console.WriteLine("bulk index");
            foreach (var chunkItem in chunks)
            {
                var bulk = chunkItem.Select(i => new People
                {
                    Id = i.ToString(), Name = "peole" + i,
                });
                var bulkIndexPeople = new BulkIndexPeople
                {
                    Collection = bulk.ToList()
                };

                bus.Publish(bulkIndexPeople);
            }
        }

        private static void BulkUpdate(IBus bus)
        {
            var chunks = Enumerable.Range(0, 10001).Chunk(100).ToArray();
            Console.WriteLine("bulk index");
            foreach (var chunkItem in chunks)
            {
                var bulk = chunkItem.Select(i => new PartialPeople
                {
                    Id = i.ToString(),
                    Adress = new Adress
                    {
                        City = "City" + i,
                        Street = "Street" + i,
                        Country = "Country" + i,
                        PostalCode = "PostalCode" + i
                    },
                });
                var listIndexPeople = new BulkUpdatePeople
                {
                    Collection = bulk.ToList()
                };

                bus.Publish(listIndexPeople);
            }
        }

        private static void UpdateOne(IBus bus, string id)
        {
            var partial = new UpdatePeople
            {
                PartialDoc = new PartialPeople
                {
                    Id = id, Adress = new Adress
                    {
                        City = "City" + id,
                        Street = "Street" + id,
                        Country = "Country" + id,
                        PostalCode = "PostalCode" + id
                    }
                }
            };
            bus.Publish(partial);
        }
    }
}