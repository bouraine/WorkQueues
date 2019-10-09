using System;
using Domain;
using Elasticsearch.Indexers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Elasticsearch.EsExtensions
{
    public static class ElasticsearchExtensions
    {
        public static void UseEsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var settings = new ConnectionSettings(new Uri(url))
                .BasicAuthentication("elastic", "changeme");
            
            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
            
            services.AddSingleton<PeopleReader>();
            services.AddSingleton(s => new DocumentIndexer<People>(
                s.GetService<IElasticClient>(),
                s.GetService<IndexRotator>(),
                "people"
            ));
            services.AddSingleton(s => new DocumentIndexer<Client>(
                s.GetService<IElasticClient>(),
                s.GetService<IndexRotator>(),
                "client"
            ));
            services.AddSingleton(s => new IndexRotator(
                s.GetService<IElasticClient>(),
                ("primary", "secondary"))
            );
            services.AddSingleton<ClientIndexer>();
        }
    }
}