using System.Collections.Generic;

namespace PhlatQL.Core.Validation
{
    public class ErrorPair
    {
        public string Message { get; set; }
        public List<string> Args { get; set; }
    }
}
