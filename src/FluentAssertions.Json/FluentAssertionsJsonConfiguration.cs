//-----------------------------------------------------------------------
// <copyright file="FluentAssertionsJsonConfiguration.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json
{
    using System.Text.Json;

    /// <summary>
    /// Default implementation of the <see cref="IFluentAssertionsJsonConfiguration"/>.
    /// </summary>
    internal sealed class FluentAssertionsJsonConfiguration : IFluentAssertionsJsonConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentAssertionsJsonConfiguration"/> class.
        /// </summary>
        public FluentAssertionsJsonConfiguration()
        {
            this.JsonSerializerOptions = new JsonSerializerOptions();
        }

        /// <inheritdoc />
        public JsonSerializerOptions JsonSerializerOptions { get; }
    }
}
