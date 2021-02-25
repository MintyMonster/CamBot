﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Http;
using Apod;
using Apod.Logic;
using System.Net.Http.Headers;
using CamBotButHesFullOfDumbShite.HubbleDefinitions;
using System.Threading;
using Newtonsoft.Json;
using WikipediaNet.Enums;
using WikipediaNet;
using WikipediaNet.Misc;
using WikipediaNet.Objects;
using CamBotButHesFullOfDumbShite.DadJokes;
using CamBotButHesFullOfDumbShite.ISSPositioning;
using CamBotButHesFullOfDumbShite.MarsRoverImages;
using CamBotButHesFullOfDumbShite.UrbanDictionary;
using CamBotButHesFullOfDumbShite.NumbersApi;
using CamBotButHesFullOfDumbShite.OpenWeatherMap;
using Microsoft.Extensions.Configuration;
using CamBotButHesFullOfDumbShite.CatsApi;
using CamBotButHesFullOfDumbShite.FoxApi;
using CamBotButHesFullOfDumbShite.DogApi;
using CamBotButHesFullOfDumbShite.CocktailApi;
using CamBotButHesFullOfDumbShite.CoinsApi;
using CamBotButHesFullOfDumbShite.MealsApi;

namespace CamBotButHesFullOfDumbShite.Modules
{
    
    public class Commands : ModuleBase
    {
        private DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;

        public Commands(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _config = services.GetRequiredService<IConfiguration>();
            var client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _client = client;
            _client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
            API_Stuff.APIHelper.InitialiseClient();
        }
        
        public async Task UserJoined(SocketGuildUser user)
        {
            var sb = new StringBuilder();
            if (user.IsBot || user.IsWebhook) return;
            Random rnd = new Random();
            int messageNum = rnd.Next(0, 11);

            switch (messageNum)
            {
                case 1:
                    sb.AppendLine($"I welcome you, {user.Mention}!");
                    break;
                case 2:
                    sb.AppendLine($"We are delighted to have you, {user.Mention}!");
                    break;
                case 3:
                    sb.AppendLine($"Greetings, {user.Mention}!");
                    break;
                case 4:
                    sb.AppendLine($"A big hello to {user.Mention} from everyone!");
                    break;
                case 5:
                    sb.AppendLine($"It's a pleasure, {user.Mention}!");
                    break;
                case 6:
                    sb.AppendLine($"Buenos dias, {user.Mention}");
                    break;
                case 7:
                    sb.AppendLine($"Why hello there, {user.Mention}");
                    break;
                case 8:
                    sb.AppendLine($"{user.Mention}, I hope you brought pizza!");
                    break;
                case 9:
                    sb.AppendLine($"We missed you, {user.Mention}");
                    break;
                case 10:
                    sb.AppendLine($"What's up, {user.Mention}?");
                    break;
            }

            await (user.Guild.DefaultChannel).SendMessageAsync(sb.ToString());
            return;
        }

        public async Task UserLeft(SocketGuildUser user)
        {
            if (user.IsBot || user.IsWebhook) return;
            await (user.Guild.DefaultChannel).SendMessageAsync($"I'll miss you, {user.Mention}!");
            return;
        }

        public static async Task<HubbleDefinitionModel> hubbleDefinitionCall(string query = null)
        {
            string url = "";
            if (!string.IsNullOrEmpty(query))
            {
                url = $"http://hubblesite.org/api/v3/glossary/{query}";
            }

            using (HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    HubbleDefinitionModel def = await response.Content.ReadAsAsync<HubbleDefinitionModel>();
                    return def;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public static async Task<DadJokeModel> getRandomDadJoke()
        {
            string url = "https://icanhazdadjoke.com/";

            using(HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    DadJokeModel joke = await response.Content.ReadAsAsync<DadJokeModel>();
                    return joke;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public static async Task<LongLatISS> GetLongLatISS()
        {
            string url = "http://api.open-notify.org/iss-now.json";

            using (HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    Iss_Position pos = await response.Content.ReadAsAsync<Iss_Position>();
                    return pos.iss_position;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        [Command("help")]
        [Alias("h")]
        public async Task help()
        {
            var sb = new StringBuilder();
            var user = Context.User.Username;

            sb.AppendLine("**I am a bot full of random things!**\n");
            sb.AppendLine("-**$test** -> See if the bot's online or not!\n");
            sb.AppendLine("-**$apod {optional: 'today'}** -> Get a random Astrology picture!\n");
            sb.AppendLine("-**$sdef {word}** -> Get the definition of a space word!\n");
            sb.AppendLine("-**$wiki {word}** -> Get Wikipedia search results!\n");
            sb.AppendLine("-**$dadjoke** -> Gives a random Dad joke!\n");
            sb.AppendLine("-**$iss** -> Get the current location of the International Space Station\n");
            sb.AppendLine("-**$mars** -> Get pictures straight from the Mars Rover!\n");
            sb.AppendLine("-**$ubdefine {word}** -> Get the Urban Dictionary definitions for words!\n");
            sb.AppendLine("-**$yearfact {optional: number}** -> Get a fact from a year!\n");
            sb.AppendLine("-**$mathfact {optional: number}** -> Get a random math fact!\n");
            sb.AppendLine("-**$weather {city name}** -> Get the weather for your city!\n");
            sb.AppendLine("-**$cat** -> Gives you a random cuddly kitten!\n");
            sb.AppendLine("-**$fox** -> Enjoy a fluffy fox!\n");
            sb.AppendLine("-**$dog** -> Doggies for everyone!\n");
            sb.AppendLine("-**$cocktail** -> Get a random cocktail recipe!\n");

            var embed = new EmbedBuilder()
            {
                Title = "Help",
                Description = sb.ToString(),
                Color = new Color(255, 255, 0)
            };

            await ReplyAsync(null, false, embed.Build());
            Console.Write($"{DateTime.Now.ToString("HH:mm:ss")} => {user} => $help"); // log to console
        }


        [Command("test")]
        public async Task testing()
        {
            var user = Context.User.Username;
            var embed = new EmbedBuilder()
            {
                Title = "We are online!",
                Color = new Color(0, 255, 0),
            };

            await ReplyAsync(null, false, embed.Build());
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} => {user} => $test");
        }

        [Command("APOD")]
        [Alias("apod")]
        public async Task APOD([Remainder] string query = null)
        {
            var user = Context.User.Username;
            var embed = new EmbedBuilder();
            var sb = new StringBuilder();
            var rnd = new Random();

            var year = rnd.Next(1995, 2021);
            var month = rnd.Next(1, 13);
            var day = rnd.Next(1, 29);
            var date = new DateTime();
            var apodClient = new ApodClient(_config["ApodKey"]);

            if (!string.IsNullOrEmpty(query))
            {
                if(query == "today")
                {
                    date = DateTime.Now;
                }
                else
                {
                    if (year == 2021)
                    {
                        var currentYear = DateTime.Now.Year;
                        var currentMonth = DateTime.Now.Month;
                        var currentDay = DateTime.Now.Day;

                        date = new DateTime(currentYear, rnd.Next(1, (int)currentMonth), rnd.Next(1, (int)currentDay));
                    }
                    else
                    {
                        date = new DateTime(year, month, day);
                    }
                }
                


                var response = await apodClient.FetchApodAsync(date);

                if (response.StatusCode != ApodStatusCode.OK)
                {
                    Console.WriteLine(response.Error.ErrorMessage);
                    Console.WriteLine(response.Error.ErrorCode);
                    apodClient.Dispose();
                    return;
                }
                else
                {
                    Console.WriteLine($"{user} - $apod -> {response.Content.ContentUrlHD}");
                }

                sb.AppendLine($"*{response.Content.Date}*");
                sb.AppendLine();
                sb.AppendLine(response.Content.Explanation);
                sb.AppendLine();
                sb.AppendLine($"({response.Content.ContentUrlHD})");

                embed.Title = response.Content.Title;
                embed.Description = sb.ToString();
                embed.ImageUrl = response.Content.ContentUrlHD;
                embed.Color = new Color(153, 50, 204);
            }
            else
            {
                if (year == 2021)
                {
                    var currentYear = DateTime.Now.Year;
                    var currentMonth = DateTime.Now.Month;
                    var currentDay = DateTime.Now.Day;

                    date = new DateTime(currentYear, rnd.Next(1, (int)currentMonth), rnd.Next(1, (int)currentDay));
                }
                else
                {
                    date = new DateTime(year, month, day);
                }

                var response = await apodClient.FetchApodAsync(date);

                if (response.StatusCode != ApodStatusCode.OK)
                {
                    Console.WriteLine(response.Error.ErrorMessage);
                    Console.WriteLine(response.Error.ErrorCode);
                    apodClient.Dispose();
                    return;
                }
                else
                {
                    Console.WriteLine($"{user} - $apod -> {response.Content.ContentUrlHD}");
                }

                sb.AppendLine($"*{response.Content.Date}*");
                sb.AppendLine();
                sb.AppendLine(response.Content.Explanation);
                sb.AppendLine();
                sb.AppendLine($"({response.Content.ContentUrlHD})");

                embed.Title = response.Content.Title;
                embed.Description = sb.ToString();
                embed.ImageUrl = response.Content.ContentUrlHD;
                embed.Color = new Color(153, 50, 204);
            }

            await ReplyAsync($"{Context.User.Mention}", false, embed.Build());
            Console.WriteLine($"{user} => $apod");
        }

        [Command("spacedefinition")]
        [Alias("sdef")]
        public async Task spaceDefinition([Remainder] string query)
        {
            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();
            var embed = new EmbedBuilder();
            var user = Context.User.Username;

            if(query == "help" || query == "Help")
            {
                sbTitle.AppendLine("Help menu for sdef");
                sb.AppendLine("Usage: $sdef {query}");
                sb.AppendLine("Example: $sdef Dark Matter");

            }
            else
            {
                if (!(string.IsNullOrEmpty(query)))
                {
                    var definition = await hubbleDefinitionCall(query);

                    
                    string def = $"{definition.definition}";
                    sb.AppendLine(definition.definition);

                    if (string.IsNullOrEmpty(def))
                    {
                        sbTitle.AppendLine("Uh oh...");
                        sb.Append("We don't contain a definition for this... yet.");
                    }
                    else
                    {
                        sbTitle.AppendLine($"Definition of {query}");
                    }
                }
            }

            embed.Title = sbTitle.ToString();
            embed.Description = sb.ToString();
            embed.Color = new Color(153, 50, 204);

            await ReplyAsync(null, false, embed.Build());
            Console.WriteLine($"{user} => $sdef");
        }

        [Command("Wiki")]
        [Alias("wsearch")]
        public async Task getWikiPages([Remainder]string query)
        {
            var sb = new StringBuilder();
            var user = Context.User.Username;
            var embed = new EmbedBuilder();

            Wikipedia wiki = new Wikipedia();
            wiki.Limit = 10;
            

            if (!(string.IsNullOrEmpty(query)))
            {
                QueryResult results = wiki.Search(query);
                foreach(Search s in results.Search)
                {
                    var completeUrl = s.Url.ToString();
                    var unSplitUrl = completeUrl.Replace(' ', '_');
                    var withoutUrl = completeUrl.Remove(0, 29);
                    sb.AppendLine($"**{withoutUrl}**");
                    sb.AppendLine(unSplitUrl.ToString());
                    sb.AppendLine();
                }
            }
            
            embed.Title = $"WikiSearch results for '{query}'";
            embed.Description = sb.ToString();
            embed.Color = new Color(0, 0, 255);

            await ReplyAsync(null, false, embed.Build()); 
            Console.WriteLine($"{user} => $wiki");
        }

        [Command("DadJoke")]
        [Alias("dadjoke")]
        public async Task getDadJoke()
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            var joke = await getRandomDadJoke();

            sb.AppendLine(joke.joke.ToString());

            embed.Title = "Dad Jokes!";
            embed.Description = sb.ToString();
            embed.Color = new Color(255, 255, 0);

            await ReplyAsync(null, false, embed.Build());
        }

        [Command("ISS")]
        public async Task getISSPosition()
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
            var user = Context.User;    

            var position = await GetLongLatISS();

            sb.AppendLine("**At time: **" + DateTime.Now.ToString("dd/MM/yyy") + " BST");
            sb.AppendLine();
            sb.AppendLine($"**Longitude:** {position.longitude.ToString()}");
            sb.AppendLine($"**Latitude:** {position.latitude.ToString()}");

            embed.Title = "The International Space Station's current location is:";
            embed.Description = sb.ToString();

            await ReplyAsync(null, false, embed.Build());
            Console.WriteLine($"{user.Mention} => $ISS");
        }

        [Command("Mars")]
        public async Task getMarsRoverImage()
        {
            var startDate = new DateTime(2021, 02, 22);
            var endDate = new DateTime();
            endDate = DateTime.Now;
            var totalDays = (endDate - startDate).Days;
            var addSols = 3038 + totalDays;
            var sb = new StringBuilder();
            var rnd = new Random();
            var sol = rnd.Next(1, addSols);
            var image = string.Empty;
            var page = rnd.Next(1, 3);
            var key = _config["NasaKey"];
            var user = Context.User;
            string url = $"https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?sol={sol}&page={page}&api_key={key}";

            using (HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    Root des = JsonConvert.DeserializeObject<Root>(await response.Content.ReadAsStringAsync());
                    int r = rnd.Next(des.photos.Count);
                    image = des.photos[r].img_src;
                    sb.AppendLine($"**Rover:** {des.photos[r].rover.name}");
                    sb.AppendLine($"**Date:** {des.photos[r].earth_date}");
                    sb.AppendLine($"**Camera:** {des.photos[r].camera.full_name}");
                    sb.AppendLine($"**Status:** {des.photos[r].rover.status}");
                    sb.AppendLine($"**Sol:** {sol}");
                    sb.AppendLine($"**Image ID:** {des.photos[r].id.ToString("###,###,###")}");
                    sb.AppendLine();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

            var embed = new EmbedBuilder()
            {
                Title = "Images straight from Mars!",
                Description = sb.ToString(),
                ImageUrl = image.ToString(),
                Color = new Color(255, 128, 0)
            };

            await ReplyAsync(null, false, embed.Build());
            Console.WriteLine($"{user.Mention} => $mars");
        }

        [Command("UBdefine")]
        [Alias("UBdef")]
        public async Task getDefinition([Remainder] string query)
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
            var client = new HttpClient();
            var user = Context.User;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://mashape-community-urban-dictionary.p.rapidapi.com/define?term={query}"),
                Headers =
                {
                    { "x-rapidapi-key", $"{_config["RapidApiKey"]}" },
                    { "x-rapidapi-host", "mashape-community-urban-dictionary.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                UrbanDictionaryRoot def = JsonConvert.DeserializeObject<UrbanDictionaryRoot>(await response.Content.ReadAsStringAsync());
                for(int i = 0; i < def.list.Count; i++)
                {
                    string definition = def.list[i].definition;
                    var removedLeftBrackets = definition.Replace('[', ' ');
                    var removedRightBrackets = removedLeftBrackets.Replace(']', ' ');
                    sb.AppendLine(removedRightBrackets);
                }
                
            }

            embed.Title = $"Urban Dictionary definition for: {query}";
            embed.Description = sb.ToString();
            embed.Color = new Color(0, 0, 128);

            await ReplyAsync(user.Mention, false, embed.Build());
            Console.WriteLine($"{user.Mention} => $ubdefine");
        }

        [Command("yearfact")]
        [Alias("yf")]
        public async Task getYearFact([Remainder] string query = null)
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
            var client = new HttpClient();
            var user = Context.User;
            var rnd = new Random();
            string url = string.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                url = $"https://numbersapi.p.rapidapi.com/{query}/year?fragment=true&json=true";
            }
            else
            {
                var number = rnd.Next(0, 2019);
                url = $"https://numbersapi.p.rapidapi.com/{number}/year?fragment=true&json=true";
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{url}"),
                Headers =
                {
                    { "x-rapidapi-key", $"{_config["RapidApiKey"]}" },
                    { "x-rapidapi-host", "numbersapi.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                NumbersApiRoot num = JsonConvert.DeserializeObject<NumbersApiRoot>(await response.Content.ReadAsStringAsync());
                sb.AppendLine($"**Year**: {num.number}");
                sb.AppendLine();
                sb.AppendLine($"**Found?**: {num.found}");
                sb.AppendLine();
                sb.AppendLine($"**Fact**: {num.text}");
            }

            embed.Title = $"Fact about the year {query}:";
            embed.Description = sb.ToString();
            embed.Color = new Color(255, 255, 0);

            await ReplyAsync(user.Mention, false, embed.Build());
            Console.WriteLine($"{user.Mention} => $yearfact");
        }

        [Command("mathfact")]
        [Alias("mfact")]
        public async Task getMathsFact([Remainder] string query = null)
        {
            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();
            var embed = new EmbedBuilder();
            var user = Context.User;
            var rnd = new Random();
            var url = string.Empty;

            var client = new HttpClient();

            if (!string.IsNullOrEmpty(query))
            {
                url = $"https://numbersapi.p.rapidapi.com/{query}/math?fragment=true&json=true";
            }
            else
            {
                var num = rnd.Next(0, 10000);
                url = $"https://numbersapi.p.rapidapi.com/{num}/math?fragment=true&json=true";
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "x-rapidapi-key", $"{_config["RapidApiKey"]}" },
                    { "x-rapidapi-host", "numbersapi.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                MathsApiRoot mathf = JsonConvert.DeserializeObject<MathsApiRoot>(await response.Content.ReadAsStringAsync());

                if(mathf.found == true)
                {
                    sbTitle.AppendLine($"Math facts:");
                    sb.AppendLine($"**Number:** {mathf.number}");
                    sb.AppendLine();
                    sb.AppendLine($"**Found?:** {mathf.found}");
                    sb.AppendLine();
                    sb.AppendLine($"**Fact:** {mathf.text}");
                }
                else
                {
                    if(mathf.text == "a number for which we're missing a fact (submit one to numbersapi at google mail!)")
                    {
                        sbTitle.AppendLine($"Uh oh...");
                        sb.AppendLine($"**Number:** {mathf.number}");
                        sb.AppendLine();
                        sb.AppendLine($"**Found?:** {mathf.found}");
                        sb.AppendLine();
                        sb.AppendLine($"**Fact:** A Number for which we're missing a fact :(");
                    }
                    else
                    {
                        sbTitle.AppendLine($"Uh oh...");
                        sb.AppendLine($"**Number:** {mathf.number}");
                        sb.AppendLine();
                        sb.AppendLine($"**Found?:** {mathf.found}");
                        sb.AppendLine();
                        sb.AppendLine($"**Fact:** {mathf.text}");
                    }
                    
                }
            }

            embed.Title = sbTitle.ToString();
            embed.Description = sb.ToString();
            embed.Color = new Color(255, 255, 0);

            await ReplyAsync(user.Mention, false, embed.Build());
            Console.WriteLine($"{user} => $mathfact");
        }

        [Command("weather")]
        public async Task getWeatherInCountry([Remainder]string query = null)
        {
            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();
            var embed = new EmbedBuilder();
            var user = Context.User;
            var key = _config["OWMKey"];

            if (!string.IsNullOrEmpty(query))
            {
                string url = $"http://api.openweathermap.org/data/2.5/weather?q={query}&units=metric&appid={key}";

                using (HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        OWMRoot owm = await response.Content.ReadAsAsync<OWMRoot>();
                        sbTitle.AppendLine($"The current weather in {owm.name}:");
                        sb.AppendLine($"**Description:** {owm.weather[0].description}\n");
                        sb.AppendLine($"**Temperature:** {owm.main.temp}°C\n");
                        sb.AppendLine($"**Feels like:** {owm.main.feels_like}°C\n");
                        sb.AppendLine($"**Max temp:** {owm.main.temp_max}°C\n");
                        sb.AppendLine($"**Min temp:** {owm.main.temp_min}°C\n");
                        sb.AppendLine($"**Humidity:** {owm.main.humidity}\n");
                        sb.AppendLine($"**Wind Speed:** {owm.wind.speed}mph\n");
                        sb.AppendLine($"**Wind direction:** {owm.wind.deg}°\n");
                    }
                    else
                    {
                        sbTitle.AppendLine("Uh oh...");
                        sb.AppendLine("Something went wrong...");
                        throw new Exception(response.ReasonPhrase); 
                    }
                }
            }
            else
            {
                sbTitle.AppendLine("Uh oh...");
                sb.AppendLine("Please specify a city.");
            }
            

            embed.Title = sbTitle.ToString();
            embed.Description = sb.ToString();
            embed.Color = new Color(135, 206, 235);

            await ReplyAsync(user.Mention, false, embed.Build());
            Console.WriteLine($"{user.Mention} => $weather");
        }

        [Command("cat")]
        [Alias("cats")]
        public async Task getCatPicturesAsync()
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
            var user = Context.User;
            var caturl = string.Empty;
            var rnd = new Random();
            var r = rnd.Next(0, 256);
            var g = rnd.Next(0, 256);
            var b = rnd.Next(0, 256);

            string url = "https://aws.random.cat/meow?ref=apilist.fun";

            using (HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    CatsRoot cats = JsonConvert.DeserializeObject<CatsRoot>(await response.Content.ReadAsStringAsync());
                    caturl = $"{cats.file}";
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            embed.ImageUrl = caturl;
            embed.Color = new Color(r, g, b);

            await ReplyAsync($"{user.Mention} wants to see cuteness!", false, embed.Build());
            Console.WriteLine($"{user.Mention} => $cat");
        }

        [Command("fox")]
        [Alias("foxes")]
        public async Task getFoxPicturesAsync()
        {
            var embed = new EmbedBuilder();
            var user = Context.User;
            var foxurl = string.Empty;
            var rnd = new Random();
            var r = rnd.Next(0, 256);
            var g = rnd.Next(0, 256);
            var b = rnd.Next(0, 256);

            string url = "https://randomfox.ca/floof/?ref=apilist.fun";

            using (HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    FoxRoot fox = JsonConvert.DeserializeObject<FoxRoot>(await response.Content.ReadAsStringAsync());
                    foxurl = $"{fox.image}";
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

            embed.ImageUrl = foxurl;
            embed.Color = new Color(r, g, b);

            await ReplyAsync($"A cuddly fox for {user.Mention}", false, embed.Build());
            Console.WriteLine($"{user.Mention} => $fox");
        }

        [Command("dog")]
        [Alias("dogs")]
        public async Task getDogPicturesAsync()
        {
            var embed = new EmbedBuilder();
            var user = Context.User;
            var dogurl = string.Empty;
            var rnd = new Random();
            var r = rnd.Next(0, 256);
            var g = rnd.Next(0, 256);
            var b = rnd.Next(0, 256);

            var url = "https://random.dog/woof.json";

            using (HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    DogRoot dog = JsonConvert.DeserializeObject<DogRoot>(await response.Content.ReadAsStringAsync());
                    dogurl = $"{dog.url}";
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

            embed.ImageUrl = dogurl;
            embed.Color = new Color(r, g, b);

            await ReplyAsync($"Fluffball delivery for: {user.Mention}", false, embed.Build());
            Console.WriteLine($"{user.Mention} => $dog");
        }

        [Command("cocktail")]
        [Alias("drinks")]
        public async Task getCocktailsAsync()
        {
            var embed = new EmbedBuilder();
            var sbTitle = new StringBuilder();
            var sb = new StringBuilder();
            var user = Context.User;
            var rnd = new Random();
            var imageurl = string.Empty;
            var r = rnd.Next(0, 256);
            var g = rnd.Next(0, 256);
            var b = rnd.Next(0, 256);

            var url = "https://www.thecocktaildb.com/api/json/v2/9973533/random.php";

            using (HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    CocktailRoot cocktail = JsonConvert.DeserializeObject<CocktailRoot>(await response.Content.ReadAsStringAsync());
                    sbTitle.AppendLine($"{cocktail.drinks[0].strDrink}");
                    sb.AppendLine($"[{user.Mention}]\n");
                    sb.AppendLine($"**Category:** {cocktail.drinks[0].strCategory}");
                    sb.AppendLine($"**Type:** {cocktail.drinks[0].strAlcoholic}");
                    sb.AppendLine($"**Glass type:** {cocktail.drinks[0].strGlass}\n");
                    sb.AppendLine($"**Instructions -**");
                    sb.AppendLine($"{cocktail.drinks[0].strInstructions}\n");
                    sb.AppendLine($"**Ingredients -**");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure1} {cocktail.drinks[0].strIngredient1}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure2} {cocktail.drinks[0].strIngredient2}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure3} {cocktail.drinks[0].strIngredient3}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure4} {cocktail.drinks[0].strIngredient4}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure5} {cocktail.drinks[0].strIngredient5}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure6} {cocktail.drinks[0].strIngredient6}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure7} {cocktail.drinks[0].strIngredient7}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure8} {cocktail.drinks[0].strIngredient8}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure9} {cocktail.drinks[0].strIngredient9}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure10} {cocktail.drinks[0].strIngredient10}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure11} {cocktail.drinks[0].strIngredient11}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure12} {cocktail.drinks[0].strIngredient12}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure13} {cocktail.drinks[0].strIngredient13}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure14} {cocktail.drinks[0].strIngredient14}");
                    sb.AppendLine($"{cocktail.drinks[0].strMeasure15} {cocktail.drinks[0].strIngredient15}");
                    imageurl = cocktail.drinks[0].strDrinkThumb;
                }
                else
                {
                    sbTitle.AppendLine("Uh oh...");
                    sb.AppendLine("Something went wrong...");
                    throw new Exception(response.ReasonPhrase);      
                }
            }

            embed.Title = sbTitle.ToString();
            embed.Description = sb.ToString();
            embed.ImageUrl = imageurl;
            embed.Color = new Color(r, g, b);

            await ReplyAsync(null, false, embed.Build());
            Console.WriteLine($"{user.Mention} => $cocktail");
        }

        [Command("prices")]
        [Alias("coins", "coin")]
        public async Task getFirstTenCoins()
        {
            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();
            var embed = new EmbedBuilder();
            var user = Context.User;
            var rnd = new Random();
            var r = rnd.Next(0, 256);
            var g = rnd.Next(0, 256);
            var b = rnd.Next(0, 256);

            var url = "https://api.coinlore.net/api/tickers/";

            using(HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    CoinsRoot coin = JsonConvert.DeserializeObject<CoinsRoot>(await response.Content.ReadAsStringAsync());
                    sb.AppendLine($"[{user.Mention}]\n");
                    for(var i = 0; i <= 10; i++)
                    {
                        sb.AppendLine($"**{coin.data[i].name} - **");
                        sb.AppendLine($"**Price:** ${coin.data[i].price_usd}");
                        sb.AppendLine($"**24h Change:** {coin.data[i].percent_change_24h}%\n");
                    }
                    sbTitle.AppendLine("Top 10 cryptocurrency prices!");
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

            embed.Title = sbTitle.ToString();
            embed.Description = sb.ToString();
            embed.Color = new Color(r, g, b);

            await ReplyAsync(null, false, embed.Build());
            Console.WriteLine($"{user.Mention} => $prices");
        }

        [Command("recipe")]
        [Alias("meals", "meal")]
        public async Task getRandomMealRecipe()
        {
            var embed = new EmbedBuilder();
            var sbTitle = new StringBuilder();
            var sb = new StringBuilder();
            var footerText = string.Empty;
            var user = Context.User;
            var rnd = new Random();
            var imageurl = string.Empty;
            var r = rnd.Next(0, 256);
            var g = rnd.Next(0, 256);
            var b = rnd.Next(0, 256);

            var url = "https://www.themealdb.com/api/json/v2/9973533/random.php";

            using(HttpResponseMessage response = await API_Stuff.APIHelper.APIClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    MealsRoot meal = JsonConvert.DeserializeObject<MealsRoot>(await response.Content.ReadAsStringAsync());           
                    sb.AppendLine($"[{user.Mention}]\n");
                    

                    Char[] chars = meal.meals[0].strInstructions.ToCharArray();
                    int amountOfChars = 0;
                    foreach(char c in chars)
                    {
                        amountOfChars++;
                    }

                    if(amountOfChars > 1700)
                    {
                        sbTitle.AppendLine("Error");
                        sb.AppendLine($"Uh oh...");
                        sb.AppendLine($"An error occurred... Please try again.");
                    }
                    else
                    {
                        sbTitle.AppendLine($"{meal.meals[0].strMeal}");
                        sb.AppendLine($"**Category:** {meal.meals[0].strCategory}");
                        sb.AppendLine($"**Area:** {meal.meals[0].strArea}");
                        sb.AppendLine($"**Tags:** {meal.meals[0].strTags}\n");
                        sb.AppendLine($"**Instructions -**");
                        sb.AppendLine($"{meal.meals[0].strInstructions}\n");
                        sb.AppendLine($"**Ingredients -**");
                        sb.AppendLine($"{meal.meals[0].strMeasure1} {meal.meals[0].strIngredient1}");
                        sb.AppendLine($"{meal.meals[0].strMeasure2} {meal.meals[0].strIngredient2}");
                        sb.AppendLine($"{meal.meals[0].strMeasure3} {meal.meals[0].strIngredient3}");
                        sb.AppendLine($"{meal.meals[0].strMeasure4} {meal.meals[0].strIngredient4}");
                        sb.AppendLine($"{meal.meals[0].strMeasure5} {meal.meals[0].strIngredient5}");
                        sb.AppendLine($"{meal.meals[0].strMeasure6} {meal.meals[0].strIngredient6}");
                        sb.AppendLine($"{meal.meals[0].strMeasure7} {meal.meals[0].strIngredient7}");
                        sb.AppendLine($"{meal.meals[0].strMeasure8} {meal.meals[0].strIngredient8}");
                        sb.AppendLine($"{meal.meals[0].strMeasure9} {meal.meals[0].strIngredient9}");
                        sb.AppendLine($"{meal.meals[0].strMeasure10} {meal.meals[0].strIngredient10}");
                        sb.AppendLine($"{meal.meals[0].strMeasure11} {meal.meals[0].strIngredient11}");
                        sb.AppendLine($"{meal.meals[0].strMeasure12} {meal.meals[0].strIngredient12}");
                        sb.AppendLine($"{meal.meals[0].strMeasure13} {meal.meals[0].strIngredient13}");
                        sb.AppendLine($"{meal.meals[0].strMeasure14} {meal.meals[0].strIngredient14}");
                        sb.AppendLine($"{meal.meals[0].strMeasure15} {meal.meals[0].strIngredient15}");
                        sb.AppendLine($"{meal.meals[0].strMeasure16} {meal.meals[0].strIngredient16}");
                        sb.AppendLine($"{meal.meals[0].strMeasure17} {meal.meals[0].strIngredient17}");
                        sb.AppendLine($"{meal.meals[0].strMeasure18} {meal.meals[0].strIngredient18}");
                        sb.AppendLine($"{meal.meals[0].strMeasure19} {meal.meals[0].strIngredient19}");
                        sb.AppendLine($"{meal.meals[0].strMeasure20} {meal.meals[0].strIngredient20}");
                        imageurl = meal.meals[0].strMealThumb;
                        footerText = meal.meals[0].strSource;
                    }  
                    
                }
                else
                {
                    sbTitle.AppendLine("Uh oh...");
                    sb.AppendLine("Something went wrong...");
                    throw new Exception(response.ReasonPhrase);
                }
            }

            embed.Title = sbTitle.ToString();
            embed.Description = sb.ToString();
            embed.ImageUrl = imageurl;
            embed.Color = new Color(r, g, b);
            embed.Url = $"{footerText}";

            await ReplyAsync(null, false, embed.Build());
            Console.WriteLine($"{user.Mention} => $recipe");
        }
    }
}
