using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrTest
{
    public class ErrorCode
    {
        public string Code { get; }
        public string Description { get; }
        public int StatusCode { get; }

        public ErrorCode(string code, string description, int statusCode)
        {
            Code = code;
            Description = description;
            StatusCode = statusCode;
        }
    }
}
