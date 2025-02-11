//-----------------------------------------------------------------------
// <copyright file="JsonAssertionFailedExceptionTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json.Tests
{
    using global::FluentAssertions;

    public class JsonAssertionFailedExceptionTest
    {
        [Fact]
        public void Constructor()
        {
            var exception = new JsonAssertionFailedException();

            exception.Message.Should().Be("Exception of type 'PosInformatique.FluentAssertions.Json.JsonAssertionFailedException' was thrown.");
            exception.InnerException.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithMessage()
        {
            var exception = new JsonAssertionFailedException("The message");

            exception.Message.Should().Be("The message");
            exception.InnerException.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException()
        {
            var innerException = new FormatException("The inner exception");
            var exception = new JsonAssertionFailedException("The message", innerException);

            exception.Message.Should().Be("The message");
            exception.InnerException.Should().BeSameAs(innerException);
        }
    }
}