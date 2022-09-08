// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;

namespace Microsoft.Azure.WebJobs.Script.Workers.Profiles
{
    /// <summary>
    /// Interface for a service tracking the currently active worker profile.
    /// </summary>
    internal interface IProfileStateProvider
    {
        /// <summary>
        /// Gets the current active worker profile
        /// </summary>
        string ActiveProfile { get; }

        /// <summary>
        /// Set an active worker profile
        /// </summary>
        void SetActiveProfile(string profile);
    }
}
