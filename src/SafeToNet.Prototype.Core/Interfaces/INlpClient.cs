using System.Threading.Tasks;

namespace SafeToNet.Prototype.Core.Interfaces
{
    /// <summary>
    /// Interface for the NLP clients
    /// </summary>
    public interface INlpClient
    {
        /// <summary>
        /// Parses the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;Domain.ParseResult&gt;.</returns>
        Task<Domain.ParseResult> Parse(string message);
    }
}