using Eva_BLL.Interfaces;

namespace Eva_API.Middelwares
{
    public class TokenValidationMiddelware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddelware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenService blacklistService)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                var isRevoked = await blacklistService.IsTokenRevokedAsync(token);
                if (isRevoked)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token has been revoked.");
                    return;
                }
            }

            await _next(context);
        }

    }
}
