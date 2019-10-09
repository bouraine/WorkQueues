using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Nest;

namespace Elasticsearch.Indexers
{
    public class DocumentIndexer<T> where T : EntityBase
    {
        private readonly IElasticClient _esClient;
        private readonly IndexRotator _indexRotator;
        private readonly string _aliasName;

        public DocumentIndexer(IElasticClient esClient, IndexRotator indexRotator, string aliasName)
        {
            _esClient = esClient;
            _indexRotator = indexRotator;
            _aliasName = aliasName.Trim().ToLower();
        }

        public async Task BulkIndex(IEnumerable<T> documents,
            Func<TypeMappingDescriptor<T>, ITypeMapping> mapper = null, bool switchIndex = true)
        {
            _indexRotator.InitAlias<T>(_aliasName, m => mapper != null ? mapper(m) : m.AutoMap());
            var newIndexName = _indexRotator.GetSecondaryIndex(_aliasName, mapper);
            var bulk = documents.Aggregate(new BulkDescriptor(newIndexName),
                (acc, x) => acc.Index<T>(p => p.Index(newIndexName).Document(x)));
            await _esClient.BulkAsync(bulk);
            if (switchIndex)
                await _indexRotator.SwitchIndex(_aliasName);
        }

        public async Task SwitchIndex() => await _indexRotator.SwitchIndex(_aliasName);

        public async Task BulkUpdate<TPartial>(IEnumerable<TPartial> partialDocuments) where TPartial : EntityBase
        {
            var bulk = partialDocuments.Aggregate(new BulkDescriptor(_indexRotator.GetPrimaryIndexName(_aliasName)),
                (acc, x) => acc.Update<T, TPartial>(u => u.Id(x.Id).Doc(x)));

            await _esClient.BulkAsync(bulk);
        }

        public async Task UpdateDocument<TPartial>(TPartial partialDocument, string id) where TPartial : EntityBase
        {
            await _esClient.UpdateAsync<T, TPartial>(id, u => u.Doc(partialDocument));
        }
        
        public async Task IndexDocument(T document, string id)
        {
            await _esClient.IndexAsync(document, d => d.Id(document.Id));
        }
    }
}