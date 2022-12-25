using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace ss_lab2;

public class Token
{
    public static async Task<Dictionary<string, object>> GetClient(IConfiguration config)
    {
        var client = new HttpClient();
    
        var response = await client.PostAsync($"https://{config["domain"]}/oauth/token",
            new FormUrlEncodedContent( new []
                {
                    new KeyValuePair<string, string>("client_id", config["client_id"]),
                    new KeyValuePair<string, string>("client_secret", config["client_secret"]),
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("audience", config["audience"]),
                    new KeyValuePair<string, string>("scope", "update:users")
                }
            )
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
    }
    
    public static async Task<Dictionary<string, object>> GetUser(IConfiguration config)
    {
        var client = new HttpClient();
    
        var response = await client.PostAsync($"https://{config["domain"]}/oauth/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"audience", config["audience"]},
                    {"grant_type", "http://auth0.com/oauth/grant-type/password-realm"},
                    {"client_id", config["client_id"]},
                    {"client_secret", config["client_secret"]},
                    {"username", config["user:name"]},
                    {"password", config["user:password"]},
                    {"realm", "Username-Password-Authentication"},
                    {"scope", "offline_access"}
                }
            )
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
    }
    
    public static async Task<Dictionary<string, object>> Refresh(string token, IConfiguration config)
    {
        var client = new HttpClient();

        var response = await client.PostAsync($"https://{config["domain"]}/oauth/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"},
                    {"client_id", config["client_id"]},
                    {"client_secret", config["client_secret"]},
                    {"refresh_token", token},
                    {"scope", "offline_access"}
                }
            )
        );
    
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(await response.Content.ReadAsStringAsync());
        }
    
        return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
    }
}