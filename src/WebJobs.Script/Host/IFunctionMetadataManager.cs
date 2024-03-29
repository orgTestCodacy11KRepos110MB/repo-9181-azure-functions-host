﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Workers;

namespace Microsoft.Azure.WebJobs.Script
{
    public interface IFunctionMetadataManager
    {
        ImmutableDictionary<string, ImmutableArray<string>> Errors { get; }

        ImmutableArray<FunctionMetadata> GetFunctionMetadata(bool forceRefresh = false, bool applyAllowlist = true, bool includeCustomProviders = true, IFunctionInvocationDispatcher dispatcher = null);

        bool TryGetFunctionMetadata(string functionName, out FunctionMetadata functionMetadata, bool forceRefresh = false);
    }
}