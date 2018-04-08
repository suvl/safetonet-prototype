using System.Collections.Generic;

namespace SafeToNet.Prototype.Core.Domain
{
    public class ParsedEntity
    {
        public string Name { get; set; }
        public ICollection<Entity> Values { get; set; }
    }
}