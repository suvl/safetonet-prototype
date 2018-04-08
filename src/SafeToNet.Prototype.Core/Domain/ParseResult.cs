using System.Collections.Generic;

namespace SafeToNet.Prototype.Core.Domain
{
    /// <summary>
    /// Result from NLP.
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>The entities.</value>
        public ICollection<ParsedEntity> Entities { get; set; } = new List<ParsedEntity>();

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }
    }
}