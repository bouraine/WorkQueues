using System;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace Elasticsearch.Indexers
{
    /// <summary>
    /// Provide a mecanisme to rotate indices with no downtime by using an alias and two indices.
    /// The primary index is the current index which serves user's requests.
    /// The secondary index is a temporary index to use for heavy index operations.
    /// While the operations finished you switch the alias to point to the new created index.
    /// zda
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
        /// Initialize if not exists a rotating index by
        /// creating an index named alias-prefix and an alias pointing to that index.
        /// Clean all indices named alias-prefix.
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
        /// Create et get (if exists) a secondary index named aliasName-prefix different from the primary index.
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
        /// Point aliasName to the secondary index and remove the alias from primary index
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
        /// Clear primary and secondary indices
        /// </summary>
        /// <param name="indexPrefix"></param>
        public void ClearAlias((string, string) indexPrefix)
        {
            var (primary, secondary) = indexPrefix;
            DeleteIndex(primary);
            DeleteIndex(secondary);
        }

        /// <summary>
        /// Create a new index
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
        /// Get the primary index name
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