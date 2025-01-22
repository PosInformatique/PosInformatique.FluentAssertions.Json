//-----------------------------------------------------------------------
// <copyright file="JsonAssertionsException.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json
{
    /// <summary>
    /// Occurs when an assertions related to JSON serialization / deserialization has been failed.
    /// </summary>
    public class JsonAssertionsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAssertionsException"/> class.
        /// </summary>
        public JsonAssertionsException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAssertionsException"/> class
        /// with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public JsonAssertionsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAssertionsException"/> class
        /// with the specified <paramref name="message"/> and the <paramref name="innerException"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner exception related to the <see cref="JsonAssertionsException"/> to create.</param>
        public JsonAssertionsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
