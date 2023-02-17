using System;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace Elasticsearch.Indexers
{
    /// <summary>
    /// Provides a mecanisme to rotate indices (by using an alias and two indices) to ensure no downtime while performing heavy write operations.
    /// 
    /// The operations are done on the secondary index while the primary index continue to serve user's requests,
    /// when the work is done, we point the alias to the secondary index and delete the primary one.
    /// </summary>
    public class IndexRotator
    {
        private readonly IElasticClient _esClient;
        private readonly (string, string) _indexPrefix;

        public IndexRotator(IElasticClient esClient, (string, string)? indexPrefix)
        {
            _esClient = esClient;
            _indexPrefix = indexPrefix ?? ("v1", "v2");
        }

        /// <summary>
        /// Initializes a rotating index by creating an index named [alias]-[prefix] and an alias pointing to that index.
        /// Clean all indices named [alias-prefix].
        /// Do nothing if alias already exists
        /// </summary>
        /// <param name="aliasName">Alias name</param>
        /// <param name="mapper">TypeMappingDescriptor</param>
        public void InitAlias<T>(string aliasName,
            Func<TypeMappingDescriptor<T>, ITypeMapping> mapper) where T : class
        {
            if (AliasExists(aliasName)) return;
            ClearAlias(_indexPrefix);
            var indexName = GetIndexVersions(aliasName).Item1;
            CreateIndex(indexName, mapper);
            _esClient.Indices.PutAlias(indexName, aliasName);
        }

        /// <summary>
        /// Returns a secondary index named [aliasName]-[prefix] different from the primary index.
        /// </summary>
        /// <param name="aliasName">Alias name </param>
        /// <param name="mapper">Index mapping settings</param>
        /// <typeparam name="T">Index type</typeparam>
        /// <returns>Secondary index name</returns>
        public string GetSecondaryIndex<T>(string aliasName, Func<TypeMappingDescriptor<T>, ITypeMapping> mapper)
            where T : class
        {
            var newIndexName = GetSecondaryIndexName(aliasName);

            if (!IndexExists(newIndexName))
                CreateIndex(newIndexName, mapper);

            return newIndexName;
        }

        /// <summary>
        /// Points [aliasName] to the secondary index and removes the alias from the primary index
        /// </summary>
        /// <param name="aliasName">Alias name</param>
        /// <returns>Secondary index name</returns>
        public async Task<string> SwitchIndex(string aliasName)
        {
            var (v1, v2) = GetIndexVersions(aliasName);
            if (!IndexExists(v1) || !IndexExists(v2))
                throw new Exception(aliasName + " should have two indices to be able to switch it.");

            var currentIndex = GetPrimaryIndexName(aliasName);
            var newIndexName = currentIndex.Equals(v1) ? v2 : v1;
            await _esClient.Indices.DeleteAsync(currentIndex);
            await _esClient.Indices.PutAliasAsync(newIndexName, aliasName);
            return newIndexName;
        }

        /// <summary>
        /// Clears the primary and secondary indices
        /// </summary>
        /// <param name="indexPrefix">the names of the primary and secondary indices </param>
        public void ClearAlias((string, string) indexPrefix)
        {
            var (primary, secondary) = indexPrefix;
            DeleteIndex(primary);
            DeleteIndex(secondary);
        }

        /// <summary>
        /// Creates a new index
        /// </summary>
        /// <param name="indexName">Index name</param>
        /// <param name="mapper">IndexMapping settings</param>
        /// <typeparam name="T">Index Type</typeparam>
        public void CreateIndex<T>(string indexName,
            Func<TypeMappingDescriptor<T>, ITypeMapping> mapper) where T : class
        {
            var response = _esClient.Indices.Create(indexName, c => c.Map(mapper));
            if (!response.IsValid)
                throw response.OriginalException;
        }

        /// <summary>
        /// Gets the primary index name
        /// </summary>
        /// <param name="aliasName">Alias name</param>
        /// <returns>Primary index name</returns>
        public string GetPrimaryIndexName(string aliasName) =>
            _esClient.GetIndicesPointingToAlias(aliasName).FirstOrDefault();

        /// <summary>
        /// Get the secondary index name
        /// </summary>
        /// <param name="aliasName">Alias name</param>
        /// <returns>Secondary index name</returns>
        private string GetSecondaryIndexName(string aliasName)
        {
            var (v1, v2) = GetIndexVersions(aliasName);
            var currentIndex = GetPrimaryIndexName(aliasName);
            var newIndexName = currentIndex.Equals(v1) ? v2 : v1;
            return newIndexName;
        }

        private bool AliasExists(string aliasName) => _esClient.Indices.AliasExists(aliasName).Exists;

        private bool IndexExists(string indexName) => _esClient.Indices.Exists(indexName).Exists;

        public void DeleteIndex(string indexName) => _esClient.Indices.Delete(indexName);

        private (string, string) GetIndexVersions(string aliasName) =>
            ($"{aliasName}-{_indexPrefix.Item1}", $"{aliasName}-{_indexPrefix.Item2}");
    }
}
