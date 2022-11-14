// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SampleApp
{
    public static class ExpressionFunction
    {
        [Function(nameof(ExpressionFunction))]
        public static void Run(
            [QueueTrigger("test-input-sample", Connection = "AzureWebJobsStorage")] Book book,
            [BlobInput("test-input-sample/{id}.txt", Connection = "AzureWebJobsStorage")] string myBlob,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(ExpressionFunction));
            logger.LogInformation(myBlob);
        }
    }
}
