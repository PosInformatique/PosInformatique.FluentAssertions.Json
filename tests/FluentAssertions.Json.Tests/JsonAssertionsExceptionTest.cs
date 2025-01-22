//-----------------------------------------------------------------------
// <copyright file="JsonAssertionsExceptionTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.FluentAssertions.Json.Tests
{
    using global::FluentAssertions;

    public class JsonAssertionsExceptionTest
    {
        [Fact]
        public void Constructor()
        {
            var exception = new JsonAssertionsException();

            exception.Message.Should().Be("Exception of type 'PosInformatique.FluentAssertions.Json.JsonAssertionsException' was thrown.");
            exception.InnerException.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithMessage()
        {
            var exception = new JsonAssertionsException("The message");

            exception.Message.Should().Be("The message");
            exception.InnerException.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException()
        {
            var innerException = new FormatException("The inner exception");
            var exception = new JsonAssertionsException("The message", innerException);

            exception.Message.Should().Be("The message");
            exception.InnerException.Should().BeSameAs(innerException);
        }
    }
}