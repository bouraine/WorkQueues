using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Nest;

namespace Elasticsearch
{
    public class PeopleReader
    {
        private readonly IElasticClient _esClient;

        public PeopleReader(IElasticClient esClient)
        {
            _esClient = esClient;
        }

        public async Task<People> GetPeopeById(string id)
        {
            var result = await _esClient
                .SearchAsync<People>(s => s.Index("People")
                    .Query(q => q.Bool(b => b.Must(m => m.Term(t => t.Field(f => f.Id == id))))));

            return result.Documents.FirstOrDefault();
        }

        public async Task<IEnumerable<People>> GetAllPeope()
        {
            var result = await _esClient.SearchAsync<People>(s => s
                .Size(500)
                .Index("people-alias")
                .Query(q => q.MatchPhrase(d => d.Query("500"))));

            return result.Documents;
        }
    }
}