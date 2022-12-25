// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ss_lab2;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection(args[0]);

static string DictToString(Dictionary<string, object> dict) =>
    dict.Aggregate("", (s, kvp) => s + $"{kvp.Key} => {kvp.Value}\n");


var accessToken = await Token.GetUser(config);
Console.WriteLine(DictToString(accessToken));

Console.Write("\n \n");

var refreshedToken = await Token.Refresh(accessToken["refresh_token"].ToString(), config);
Console.WriteLine(DictToString(refreshedToken));