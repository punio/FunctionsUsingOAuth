using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionsUsingOAuth
{
	public static class Hello
	{
		[FunctionName("Hello")]
		public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
		{
			var info = JwtToken.GetInfoFromHeader(req.Headers["Authorization"]);
			if (string.IsNullOrEmpty(info.name)) return new UnauthorizedResult();   // Tokenが不正
			// ここに有効期限チェックとか入れるのかな？
			return new OkObjectResult($"Hello {info.name}");
		}

	}
}
