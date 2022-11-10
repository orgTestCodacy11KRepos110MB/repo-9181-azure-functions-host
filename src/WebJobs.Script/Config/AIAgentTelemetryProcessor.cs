// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    internal class AIAgentTelemetryProcessor : ITelemetryProcessor
    {
        public AIAgentTelemetryProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        private ITelemetryProcessor Next { get; set; }

        public void Process(ITelemetry item)
        {
            // TODO: Find a better way to figure out Functions.<FunctionName>.User logs
            if (item.Context.Properties.ContainsKey("Category") && item.Context.Properties["Category"].EndsWith(".User"))
            {
                return;
            }
            this.Next.Process(item);
        }
    }
}
