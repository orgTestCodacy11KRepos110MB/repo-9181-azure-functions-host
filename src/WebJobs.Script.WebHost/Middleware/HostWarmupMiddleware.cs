﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Script.Extensions;
using Microsoft.Diagnostics.JitTrace;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Middleware
{
    public class HostWarmupMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IScriptWebHostEnvironment _webHostEnvironment;
        private readonly IEnvironment _environment;
        private readonly IScriptHostManager _hostManager;
        private readonly ILogger _logger;

        public HostWarmupMiddleware(RequestDelegate next, IScriptWebHostEnvironment webHostEnvironment, IEnvironment environment, IScriptHostManager hostManager, ILogger<HostWarmupMiddleware> logger)
        {
            _next = next;
            _webHostEnvironment = webHostEnvironment;
            _environment = environment;
            _hostManager = hostManager;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (IsWarmUpRequest(httpContext.Request, _webHostEnvironment.InStandbyMode, _environment))
            {
                PreJitPrepare(WarmUpConstants.JitTraceFileName);
                if (_environment.IsLinuxConsumption())
                {
                    PreJitPrepare(WarmUpConstants.LinuxJitTraceFileName);
                }

                await WarmUp(httpContext.Request);
            }

            await _next.Invoke(httpContext);
        }

        private void PreJitPrepare(string jitTraceFileName)
        {
            // This is to PreJIT all methods captured in coldstart.jittrace file to improve cold start time
            var path = Path.Combine(
                Path.GetDirectoryName(new Uri(typeof(HostWarmupMiddleware).Assembly.Location).LocalPath),
                WarmUpConstants.PreJitFolderName, jitTraceFileName);

            var file = new FileInfo(path);

            if (file.Exists)
            {
                JitTraceRuntime.Prepare(file, out int successfulPrepares, out int failedPrepares);

                // We will need to monitor failed vs success prepares and if the failures increase, it means code paths have diverged or there have been updates on dotnet core side.
                // When this happens, we will need to regenerate the coldstart.jittrace file.
                _logger.LogInformation(new EventId(100, "PreJit"),
                    $"PreJIT Successful prepares: {successfulPrepares}, Failed prepares: {failedPrepares} FileName = {jitTraceFileName}");
            }
        }

        public async Task WarmUp(HttpRequest request)
        {
            if (request.Query.TryGetValue("restart", out StringValues value) && string.Compare("1", value) == 0)
            {
                await _hostManager.RestartHostAsync(CancellationToken.None);

                // This call is here for sanity, but we should be fully initialized.
                await _hostManager.DelayUntilHostReady();
            }
        }

        public static bool IsWarmUpRequest(HttpRequest request, bool inStandbyMode, IEnvironment environment)
        {
            return inStandbyMode &&
                ((environment.IsAppService() && request.IsAppServiceInternalRequest(environment)) || environment.IsLinuxConsumption()) &&
                (request.Path.StartsWithSegments(new PathString($"/api/{WarmUpConstants.FunctionName}")) ||
                request.Path.StartsWithSegments(new PathString($"/api/{WarmUpConstants.AlternateRoute}")));
        }
    }
}
