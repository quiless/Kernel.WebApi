using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Kernel.WebApi.Common
{
    public class ApiResponseResult<T> : IHttpActionResult
    {
        T _value;
        HttpStatusCode _statusCode;
        public ApiResponseResult(T value, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _value = value;
            _statusCode = statusCode;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(_statusCode)
            {
                Content = new ObjectContent<T>(_value, new JsonMediaTypeFormatter())
            };
            return Task.FromResult(response);
        }
    }
}