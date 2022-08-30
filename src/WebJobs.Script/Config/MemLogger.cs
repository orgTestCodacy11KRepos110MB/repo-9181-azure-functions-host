// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;

namespace Microsoft.Azure.WebJobs.Script.Configuration
{
    public static class MemLogger
    {
        private static readonly ConcurrentQueue<string> LogQueue = new ConcurrentQueue<string>();

        public static string[] GetMessages() => LogQueue.ToArray();

        public static void Log(string message)
        {
            LogError(null, message, "Info");
        }

        public static void LogError(Exception ex, string message, string level = "Error")
        {
            var msg = $"[{DateTime.Now:O}] [{level}] ";

            if (ex != null)
            {
                msg = $"{msg}{ex.ToString()}.";
            }

            if (message != null)
            {
                msg = $"{msg}{message}.";
            }

            LogQueue.Enqueue(msg);
        }
    }
}
