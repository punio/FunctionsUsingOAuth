
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;

namespace FunctionsUsingOAuth
{
	public static class Token
	{
		[FunctionName("Token")]
		public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
		{
			if (!req.Form.TryGetValue("user", out var user))
			{
				return new BadRequestResult();
			}

			var token = JwtToken.CreateToken(user);

			var result = new TokenResult
			{
				Token = token.token,
				ExpirationDate = token.expirationDate
			};

			return new OkObjectResult(result)
			{
				ContentTypes = new Microsoft.AspNetCore.Mvc.Formatters.MediaTypeCollection { "application/json" }
			};
		}

		public class TokenResult
		{
			public string Token { get; set; }
			public DateTime ExpirationDate { get; set; }
		}
	}
}
