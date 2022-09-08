// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Script.Workers.Profiles;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Workers.Profiles
{
    /// <summary>
    /// The default profile manager that manages profiles from language workers
    /// </summary>
    internal class WorkerProfileManager : IWorkerProfileManager
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IWorkerProfileConditionProvider> _conditionProviders;
        private readonly IProfileStateProvider _profileStateProvider;
        private Dictionary<string, List<WorkerDescriptionProfile>> _profiles;

        public WorkerProfileManager(ILogger logger, IEnumerable<IWorkerProfileConditionProvider> conditionProviders, IProfileStateProvider profileStateProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _conditionProviders = conditionProviders ?? throw new ArgumentNullException(nameof(conditionProviders));
            _profileStateProvider = profileStateProvider ?? throw new ArgumentNullException(nameof(profileStateProvider));
            _profiles = new Dictionary<string, List<WorkerDescriptionProfile>>();
        }

        /// <inheritdoc />
        public void SetWorkerDescriptionProfiles(List<WorkerDescriptionProfile> workerDescriptionProfiles, string language)
        {
            _profiles[language] = workerDescriptionProfiles;
        }

        // Evaluate profile conditions for a language
        private bool GetEvaluatedProfile(string language, out WorkerDescriptionProfile evaluatedProfile)
        {
            if (_profiles.TryGetValue(language, out List<WorkerDescriptionProfile> profiles))
            {
                foreach (var profile in profiles)
                {
                    if (profile.EvaluateConditions())
                    {
                        evaluatedProfile = profile;
                        return true;
                    }
                }
            }
            evaluatedProfile = null;
            return false;
        }

        /// <inheritdoc />
        public void LoadWorkerDescriptionFromProfiles(RpcWorkerDescription defaultWorkerDescription, out RpcWorkerDescription workerDescription)
        {
            if (GetEvaluatedProfile(defaultWorkerDescription.Language, out WorkerDescriptionProfile profile))
            {
                _logger.LogInformation("Worker initialized with profile from worker config. Name: '{name}' ID: '{id}'", profile.Name, profile.ProfileId);
                _profileStateProvider.SetActiveProfile(profile.ProfileId);
                workerDescription = profile.ApplyProfile(defaultWorkerDescription);
                return;
            }
            workerDescription = defaultWorkerDescription;
        }

        /// <inheritdoc />
        public bool TryCreateWorkerProfileCondition(WorkerProfileConditionDescriptor conditionDescriptor, out IWorkerProfileCondition condition)
        {
            foreach (var provider in _conditionProviders)
            {
                if (provider.TryCreateCondition(conditionDescriptor, out condition))
                {
                    return true;
                }
            }

            _logger.LogInformation("Unable to create profile condition for condition type '{conditionType}'", conditionDescriptor.Type);

            condition = null;
            return false;
        }

        /// <inheritdoc />
        public bool IsCorrectProfileLoaded(string workerRuntime)
        {
            var profileId = string.Empty;
            if (GetEvaluatedProfile(workerRuntime, out WorkerDescriptionProfile profile))
            {
                profileId = profile.ProfileId;
            }

            var activeProfile = _profileStateProvider.ActiveProfile;
            _logger.LogDebug($"Active profile: {activeProfile}. Evaluated profile: {profileId}");
            return activeProfile.Equals(profileId);
        }
    }
}
