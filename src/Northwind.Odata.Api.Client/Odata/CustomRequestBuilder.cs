using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Northwind.Odata.Api.Client.Models.ODataErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Odata.Api.Client.Odata
{
    public class CustomRequestBuilder : BaseRequestBuilder
    {
        public class CustomRequestBuilderGetQueryParameters
        {
            [QueryParameter("%24count")]
            public bool? Count { get; set; }

            [QueryParameter("%24expand")]
            public string[]? Expand { get; set; }

            [QueryParameter("%24filter")]
            public string? Filter { get; set; }

            [QueryParameter("%24orderby")]
            public string[]? Orderby { get; set; }

            [QueryParameter("%24search")]
            public string? Search { get; set; }

            [QueryParameter("%24select")]
            public string[]? Select { get; set; }

            [QueryParameter("%24top")]
            public int? Top { get; set; }
        }

        public CustomRequestBuilder(string path, IRequestAdapter requestAdapter)
            : base(requestAdapter, $"{{+baseurl}}/{WebUtility.UrlEncode(path)}{{?%24count,%24expand,%24filter,%24orderby,%24search,%24select,%24top}}", new Dictionary<string, object>())
        {
        }
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default, string contentType = "application/json")
        {
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", contentType);
            return requestInfo;
        }

        public async Task<T> GetAsync<T>(ParsableFactory<T> parsableFactory, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = null, string contentType = "application/json", CancellationToken cancellationToken = default)
            where T : IParsable
        {
            var requestInfo = ToGetRequestInformation(requestConfiguration, contentType);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
    {
        { "XXX", ODataError.CreateFromDiscriminatorValue }
    };
            return await RequestAdapter.SendAsync(requestInfo, parsableFactory, errorMapping, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<UntypedNode> GetAsync(Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = null, string contentType = "application/json", CancellationToken cancellationToken = default)
        {
            return await GetAsync(UntypedNode.CreateFromDiscriminatorValue, requestConfiguration, contentType, cancellationToken);
        }

        public RequestInformation ToPostRequestInformation(IParsable body, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default, string contentType = "application/json")
        {
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.POST, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", contentType);
            requestInfo.SetContentFromParsable(RequestAdapter, contentType, body);
            return requestInfo;
        }

        public async Task<T> PostAsync<T>(IParsable body, ParsableFactory<T> parsableFactory, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = null, string contentType = "application/json", CancellationToken cancellationToken = default)
            where T : IParsable
        {
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPostRequestInformation(body, requestConfiguration, contentType);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
    {
        { "XXX", ODataError.CreateFromDiscriminatorValue }
    };
            return await RequestAdapter.SendAsync(requestInfo, parsableFactory, errorMapping, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<UntypedNode> PostAsync(IParsable body, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default,
            CancellationToken cancellationToken = default)
        {
            return await PostAsync(body, UntypedNode.CreateFromDiscriminatorValue, requestConfiguration, cancellationToken: cancellationToken);
        }
    
    public RequestInformation ToPatchRequestInformation(IParsable body, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default, string contentType = "application/json")
        {
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.PATCH, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", contentType);
            requestInfo.SetContentFromParsable(RequestAdapter, contentType, body);
            return requestInfo;
        }

        public async Task<T> PatchAsync<T>(IParsable body, ParsableFactory<T> parsableFactory, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default, string contentType = "application/json",
           CancellationToken cancellationToken = default) where T : IParsable
        {
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPatchRequestInformation(body, requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
   {
      { "XXX", ODataError.CreateFromDiscriminatorValue },
   };
            return await RequestAdapter.SendAsync(requestInfo, parsableFactory, errorMapping, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UntypedNode> PatchAsync(IParsable body, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default,
           CancellationToken cancellationToken = default)
        {
            return await PatchAsync(body, UntypedNode.CreateFromDiscriminatorValue, requestConfiguration, cancellationToken: cancellationToken);
        }

        public RequestInformation ToPutRequestInformation(IParsable body, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default, string contentType = "application/json")
        {
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.PUT, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", contentType);
            requestInfo.SetContentFromParsable(RequestAdapter, contentType, body);
            return requestInfo;
        }

        public async Task<T> PutAsync<T>(IParsable body, ParsableFactory<T> parsableFactory, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default, string contentType = "application/json",
           CancellationToken cancellationToken = default) where T : IParsable
        {
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPutRequestInformation(body, requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
   {
      { "XXX", ODataError.CreateFromDiscriminatorValue },
   };
            return await RequestAdapter.SendAsync(requestInfo, parsableFactory, errorMapping, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UntypedNode> PutAsync(IParsable body, Action<RequestConfiguration<CustomRequestBuilderGetQueryParameters>> requestConfiguration = default,
           CancellationToken cancellationToken = default)
        {
            return await PutAsync(body, UntypedNode.CreateFromDiscriminatorValue, requestConfiguration, cancellationToken: cancellationToken);
        }
    }
}
