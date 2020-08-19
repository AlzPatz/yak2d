using System;

namespace Yak2D
{
    /// <summary>
    /// Represents errors that occur in Yak2D library.
    /// </summary>
    public class Yak2DException : Exception
    {
        /// <summary>
        /// Constructs a new Yak2DException
        /// </summary>
        public Yak2DException() { }

        /// <summary>
        /// Constructs a new Yak2DException with the given message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public Yak2DException(string message) : base(message) { }

        /// <summary>
        /// Constructs a new Yak2DException with the given message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public Yak2DException(string message, Exception innerException) : base(message, innerException) { }
    }
}