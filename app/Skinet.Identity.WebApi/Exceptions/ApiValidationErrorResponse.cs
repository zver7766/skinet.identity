using System.Collections.Generic;
using skinet.identity.Exceptions;

namespace skinet.identity.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public ApiValidationErrorResponse() : base(400)
        {
        }

        public IEnumerable<string> Errors { get; set; }
    }
}