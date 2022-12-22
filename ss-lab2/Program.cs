// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection(args[0]);

static string DictToString(Dictionary<string, object> dict) =>
    dict.Aggregate("", (s, kvp) => s + $"{kvp.Key} => {kvp.Value}\n");

static async Task<Dictionary<string, object>> GetToken(IConfiguration config)
{
    var client = new HttpClient();

    var response = await client.PostAsync($"https://{config["domain"]}/oauth/token",
        new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", config["client_id"]),
                new KeyValuePair<string, string>("client_secret", config["client_secret"]),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("audience", config["audience"])
            }
        )
    );

    if (!response.IsSuccessStatusCode)
    {
        throw new Exception(response.StatusCode.ToString());
    }

    return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
}

static async Task<Dictionary<string, object>> CreateUser(string token, IConfiguration config)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var response = await client.PostAsJsonAsync($"https://{config["domain"]}/api/v2/users",
        new Dictionary<string, object>()
        {
            { "email", "misha.muravynets@mail.com" },
            { "password", "Pass123!" },
            { "connection", "Username-Password-Authentication"}
        }
    );
    
    if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.Conflict)
    {
        throw new Exception(await response.Content.ReadAsStringAsync());
    }
    
    return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
}


var accessToken = await GetToken(config);
Console.WriteLine(DictToString(accessToken));

var user = await CreateUser(accessToken["access_token"].ToString(), config);
Console.WriteLine(DictToString(user));