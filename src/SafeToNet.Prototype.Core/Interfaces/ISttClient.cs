using System.Threading.Tasks;

namespace SafeToNet.Prototype.Core.Interfaces
{
    /// <summary>
    /// Interface for Speech-To-Text clients
    /// </summary>
    public interface ISttClient
    {
        /// <summary>
        /// Parses the speech.
        /// </summary>
        /// <param name="speech">The speech.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> ParseSpeech(byte[] speech);
    }
}
