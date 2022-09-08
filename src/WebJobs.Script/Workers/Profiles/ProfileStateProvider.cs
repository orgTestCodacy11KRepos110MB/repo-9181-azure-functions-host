// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reactive.Linq;
using Microsoft.Azure.WebJobs.Script.Eventing;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Workers.Profiles
{
    internal class ProfileStateProvider : IProfileStateProvider
    {
        private static readonly object mutex = new object();
        private string _activeProfile;

        public ProfileStateProvider()
        {
            _activeProfile = string.Empty;
        }

        public string ActiveProfile => _activeProfile;

        public void SetActiveProfile(string profile)
        {
            lock (mutex)
            {
                _activeProfile = profile;
            }
        }
    }
}
