using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace Elasticsearch.Indexers
{
    public class ClientIndexer
    {
        private readonly DocumentIndexer<Client> _documentIndexer;

        public ClientIndexer(DocumentIndexer<Client> documentIndexer) => _documentIndexer = documentIndexer;

        public async Task IndexClients() =>
            await _documentIndexer.BulkIndex(ClientSeed.Data, ClientMapping.Mapping);

        public async Task IndexClient() =>
            await _documentIndexer.IndexDocument(ClientSeed.SingleClient);
    }
}