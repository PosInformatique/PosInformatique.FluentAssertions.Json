//-----------------------------------------------------------------------
// <copyright file="FluentAssertionsJson.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json
{
    /// <summary>
    /// Allows to access to the configuration of <c>PosInformatique.FluentAssertions.Json</c> library.
    /// </summary>
    public static class FluentAssertionsJson
    {
        /// <summary>
        /// Gets the <see cref="IFluentAssertionsJsonConfiguration"/> instance which allows to configure the
        /// <c>PosInformatique.FluentAssertions.Json</c> library.
        /// </summary>
        public static IFluentAssertionsJsonConfiguration Configuration { get; internal set; } = new FluentAssertionsJsonConfiguration();
    }
}
