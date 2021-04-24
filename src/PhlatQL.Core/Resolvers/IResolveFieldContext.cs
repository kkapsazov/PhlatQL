using System.Collections.Generic;

namespace PhlatQL.Core.Resolvers
{
    public interface IResolveFieldContext
    {
        object Source { get; set; }

        Dictionary<string, object> Build();
    }
}
