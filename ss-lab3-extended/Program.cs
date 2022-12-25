// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ss_lab2;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection(args[0]);

static string DictToString(Dictionary<string, object> dict) =>
    dict.Aggregate("", (s, kvp) => s + $"{kvp.Key} => {kvp.Value}\n");

static async Task<Dictionary<string, object>> ChangePassword(string token, IConfiguration config)
{
    var client = new HttpClient();
    var request = new HttpRequestMessage()
    {
        RequestUri = new Uri($"https://{config["domain"]}/api/v2/users/{config["user:user_id"]}"),
        Method = HttpMethod.Patch,
    };
    request.Headers.Add("authorization", $"Bearer {token}");

    var content = new Dictionary<string, object>()
    {
        {"password", config["user:password"]},
        {"connection", "Username-Password-Authentication"}
    };
        
    request.Content = new StringContent(JsonSerializer.Serialize(content));
    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    var response = await client.SendAsync(request);
    
    if (!response.IsSuccessStatusCode)
    {
        throw new Exception(await response.Content.ReadAsStringAsync());
    }
    
    return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();;
}

var token = await Token.GetClient(config);
var changedPassword = await ChangePassword(token["access_token"].ToString(), config);
Console.WriteLine(DictToString(changedPassword));