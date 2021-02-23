using Discord;
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

namespace CamBotButHesFullOfDumbShite.Modules
{
    
    public class Commands : ModuleBase
    {
        private DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        //keys
        private static string NasaKey = "4ZkKJ36jZhOQcOr5ZYPaOkMX7oLmQ3HW9X9jBk0L";
        private static string OWMKey = "63ac3e2e1c5fee8c30504c2fca40bb3c";
        private static string RapidAPIKey = "8f9bc3c51bmshefef4896fc3f94ap11e8bajsn3117a2fa356f";
        private static string ApodKey = "4ZkKJ36jZhOQcOr5ZYPaOkMX7oLmQ3HW9X9jBk0L";

        public Commands(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            var client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _client = client;
            _client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
            CamBotButHesFullOfDumbShite.API_Stuff.APIHelper.InitialiseClient();
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
        public async Task APOD([Remainder]string query)
        {
            var user = Context.User.Username;
            var embed = new EmbedBuilder();
            var sb = new StringBuilder();
            var rnd = new Random();

            var year = rnd.Next(1995, 2021);
            var month = rnd.Next(1, 13);
            var day = rnd.Next(1, 29);
            var date = new DateTime();
            var apodClient = new ApodClient(ApodKey);

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

            var position = await GetLongLatISS();

            sb.AppendLine("**At time: **" + DateTime.Now.ToString("dd/MM/yyy") + " BST");
            sb.AppendLine();
            sb.AppendLine($"**Longitude:** {position.longitude.ToString()}");
            sb.AppendLine($"**Latitude:** {position.latitude.ToString()}");

            embed.Title = "The International Space Station's current location is:";
            embed.Description = sb.ToString();

            await ReplyAsync(null, false, embed.Build());
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
            string url = $"https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?sol={sol}&page={page}&api_key={NasaKey}";

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
                    { "x-rapidapi-key", $"{RapidAPIKey}" },
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
                    { "x-rapidapi-key", $"{RapidAPIKey}" },
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
                    { "x-rapidapi-key", $"{RapidAPIKey}" },
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
            Console.WriteLine($"{user} -> $mathfact");
        }

        [Command("weather")]
        [Alias("forecast")]
        public async Task getWeatherInCountry([Remainder]string query = null)
        {
            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();
            var embed = new EmbedBuilder();
            var user = Context.User;

            if (!string.IsNullOrEmpty(query))
            {
                string url = $"http://api.openweathermap.org/data/2.5/weather?q={query}&units=metric&appid={OWMKey}";

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
        }
    }
}
