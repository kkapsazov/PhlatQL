using System.Collections.Generic;
using System.Linq;

namespace PhlatQL.Core.Validation
{
    public class ErrorResponse
    {
        public Dictionary<string, List<ErrorPair>> Errors { get; set; } = new();

        public ErrorResponse AddFieldError(string field, string message, List<string> args)
        {
            if (!this.Errors.ContainsKey(field))
            {
                this.Errors[field] = new List<ErrorPair>
                {
                    new()
                    {
                        Message = message,
                        Args = args
                    }
                };
            }
            else
            {
                this.Errors[field].Add(new ErrorPair
                {
                    Message = message,
                    Args = args
                });
            }

            return this;
        }

        public bool Any()
        {
            return this.Errors.Any();
        }
    }
}
