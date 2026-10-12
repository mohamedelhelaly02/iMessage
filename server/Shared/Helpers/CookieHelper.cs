namespace server.Shared.Helpers;

public static class CookieHelper
{
    private const string CookieName = "refresh_token";
    public static void SetRefreshTokenCookie(
        HttpResponse response,
        string refreshToken,
        DateTime expires,
        bool isDev)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            Expires = expires,
            Path = "/api/auth"
        };

        response.Cookies.Append(CookieName, refreshToken, cookieOptions);
    }

    public static void DeleteRefreshTokenCookie(HttpResponse response, bool isDev)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDev,
            SameSite = SameSiteMode.Lax,
            Path = "/api/auth"
        };

        response.Cookies.Delete(CookieName, cookieOptions);
    }
}
