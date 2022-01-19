using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearch
{
    public abstract class ElasticHelper
    {
        private readonly ElasticClient ESClient;

        internal ElasticHelper(string indexName, Uri node)
        {
            if (node == null)
                throw AppTrace.Err(" Not set adress ElasticSearch ");

            if (string.IsNullOrEmpty(indexName))
                throw AppTrace.Err(" Not set indexname for search ElasticSearch ");

            ConnectionSettings settings = new ConnectionSettings(node)
                .DefaultIndex(indexName)
                .DisableDirectStreaming()
                .PrettyJson()
                .OnRequestCompleted(callDetails =>
                {
                    if (!callDetails.Success)
                        throw AppTrace.Err(callDetails.OriginalException.Message);

                    if (callDetails.RequestBodyInBytes != null)
                        AppTrace.Info($"{callDetails.HttpMethod} {callDetails.Uri} \n" +
                            $"{Encoding.UTF8.GetString(callDetails.RequestBodyInBytes)}");
                    else
                        AppTrace.Info($"{callDetails.HttpMethod} {callDetails.Uri}");

                    if (callDetails.ResponseBodyInBytes != null)
                        AppTrace.Info($"Status: {callDetails.HttpStatusCode}\n" +
                            $"{Encoding.UTF8.GetString(callDetails.ResponseBodyInBytes)}\n");
                    else
                        AppTrace.Info($"Status: {callDetails.HttpStatusCode}");
                });

            ESClient = new ElasticClient(settings);

            if (!ESClient.Indices.Exists(indexName).Exists)
            {
                CreateIndexResponse responseCreate = CreateIndex(indexName);
            }
        }

        internal async Task<IEnumerable<ResponseElastic<T>>> SearchAsync<T>
            (ElasticSearchArguments<T> ESA) where T : class
        {
            SearchDescriptor<T> searchDescriptor = ElasticDescriptor.GetSearchDescriptor(ESA);

            ISearchResponse<T> searchResponse = await ESClient.SearchAsync<T>(searchDescriptor);

            if (!searchResponse.IsValid)
                throw AppTrace.Err(searchResponse.DebugInformation);

            return ConvertResponse(searchResponse);
        }

        internal IEnumerable<ResponseElastic<T>> Search<T>
            (ElasticSearchArguments<T> ESA) where T : class
        {
            SearchDescriptor<T> searchDescriptor = ElasticDescriptor.GetSearchDescriptor(ESA);

            ISearchResponse<T> searchResponse = ESClient.Search<T>(searchDescriptor);

            if (!searchResponse.IsValid)
                throw AppTrace.Err(searchResponse.DebugInformation);

            return ConvertResponse(searchResponse);
        }

        #region Helpers

        #region sync

        private CreateIndexResponse CreateIndex(string indexName) =>
            ESClient.Indices.Create(indexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(ad => ad
                            .Custom("windows_path_hierarchy_analyzer", ca => ca
                                .Tokenizer("windows_path_hierarchy_tokenizer")
                            )
                        )
                        .Tokenizers(t => t
                            .PathHierarchy("windows_path_hierarchy_tokenizer", ph => ph
                                .Delimiter('\\')
                            )
                        )
                    )
                )
            );

        private DeleteIndexResponse DeleteIndex(string indexName) =>
            ESClient.Indices.Delete(indexName);

        internal IndexResponse InsertDoc<T>(T doc) where T : class =>
           ESClient.Index<T>(doc, i => i.Pipeline("attachment").Refresh(Refresh.WaitFor));

        internal DeleteResponse DeleteDoc<T>(int id) where T : class =>
            ESClient.Delete<T>(id);

        #endregion

        #region async

        private async Task<CreateIndexResponse> CreateIndexAsync(string indexName) =>
            await ESClient.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(ad => ad
                            .Custom("windows_path_hierarchy_analyzer", ca => ca
                                .Tokenizer("windows_path_hierarchy_tokenizer")
                            )
                        )
                        .Tokenizers(t => t
                            .PathHierarchy("windows_path_hierarchy_tokenizer", ph => ph
                                .Delimiter('\\')
                            )
                        )
                    )
                )
            );

        private async Task<DeleteIndexResponse> DeleteIndexAsync(string indexName) =>
            await ESClient.Indices.DeleteAsync(indexName);

        internal async Task<IndexResponse> InsertDocAsync<T>(T doc) where T : class =>
            await ESClient.IndexAsync<T>(doc, i => i.Pipeline("attachment").Refresh(Refresh.WaitFor));

        internal async Task<DeleteResponse> DeleteDocAsync<T>(int id) where T : class =>
            await ESClient.DeleteAsync<T>(id);

        #endregion


        private ICollection<ResponseElastic<T>> ConvertResponse<T>(ISearchResponse<T> searchResponse) where T : class
        {
            ICollection<ResponseElastic<T>> Response = new List<ResponseElastic<T>>();

            foreach (var hit in searchResponse.Hits)
            {
                Response.Add(new ResponseElastic<T>()
                {
                    Id = hit.Id,
                    Hits = hit.Highlight.Values.SelectMany(id => id),
                    Suggest = searchResponse.Suggest.Values.SelectMany(SelMany => SelMany.SelectMany(SelManySelector => SelManySelector.Options.Select(s1 => s1.Text))),
                    Documents = hit.Source
                });
            }

            return Response;
        }

        internal class ESException : Exception
        {
            public ESException(string message) : base(message) { }
        }

        #endregion
    }
}