//-----------------------------------------------------------------------
// <copyright file="IFluentAssertionsJsonConfiguration.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json
{
    using System.Text.Json;

    /// <summary>
    /// Contains global options of the <c>PosInformatique.FluentAssertions.Json</c> library.
    /// </summary>
    public interface IFluentAssertionsJsonConfiguration
    {
        /// <summary>
        /// Gets the default <see cref="JsonSerializerOptions"/> to use for the JSON serialization assertions. By default,
        /// the .NET <see cref="JsonSerializerOptions"/> are used.
        /// </summary>
        JsonSerializerOptions JsonSerializerOptions { get; }
    }
}
