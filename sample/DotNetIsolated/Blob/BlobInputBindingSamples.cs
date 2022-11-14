// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace SampleApp
{
    public class BlobInputBindingSamples
    {
        private readonly ILogger<BlobInputBindingSamples> _logger;

        public BlobInputBindingSamples(ILogger<BlobInputBindingSamples> logger)
        {
            _logger = logger;
        }

        [Function(nameof(BlobInputClientFunction))]
        public async Task<HttpResponseData> BlobInputClientFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            [BlobInput("test-input-sample/sample1.txt", Connection = "AzureWebJobsStorage")] BlobClient client)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            var downloadResult = await client.DownloadContentAsync();
            await response.Body.WriteAsync(downloadResult.Value.Content);
            return response;
        }

        [Function(nameof(BlobInputStreamFunction))]
        public async Task<HttpResponseData> BlobInputStreamFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            [BlobInput("test-input-sample/sample1.txt", Connection = "AzureWebJobsStorage")] Stream stream)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            using var blobStreamReader = new StreamReader(stream);
            await response.WriteStringAsync(blobStreamReader.ReadToEnd());
            return response;
        }

        [Function(nameof(BlobInputByteArrayFunction))]
        public async Task<HttpResponseData> BlobInputByteArrayFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            [BlobInput("test-input-sample/sample1.txt", Connection = "AzureWebJobsStorage")] Byte[] data)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(Encoding.Default.GetString(data));
            return response;
        }

        [Function(nameof(BlobInputStringFunction))]
        public async Task<HttpResponseData> BlobInputStringFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            [BlobInput("test-input-sample/sample1.txt", Connection = "AzureWebJobsStorage")] string data)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(data);
            return response;
        }

        [Function(nameof(BlobInputBookFunction))]
        public async Task<HttpResponseData> BlobInputBookFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            [BlobInput("test-input-sample/book.json", Connection = "AzureWebJobsStorage")] Book data)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(data.Name);
            return response;
        }
    }
}
