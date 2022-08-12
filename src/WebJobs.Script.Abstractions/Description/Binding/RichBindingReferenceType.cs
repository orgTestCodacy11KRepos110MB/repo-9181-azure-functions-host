// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Script.Abstractions.Description.Binding
{
    public class RichBindingReferenceType
    {
        /// <summary>
        /// Gets or Sets all the properties of ReferenceType
        /// </summary>
        public IDictionary<string, object> Properties { get; set; }
    }
}
