// https://www.backloggd.com/api/user/79153/log/122748
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using GameListDB.Model;
using GameListDB.Model.Extensions;
using GameListDB.DTO;
using HtmlAgilityPack;

namespace GameListDB.BackloggdIntegration
{

  public class BackloggdExporter
  {
    private const string _DebugUsername = "";
    private readonly HttpClient _client;
    private CookieContainer cookieContainer;
    private readonly Backloggd backloggdConfig;
    string csrf = string.Empty;

    private readonly GameListsContext gamelistdb;

    public BackloggdExporter(GameListsContext gameListsContext, Config config)
    {
      gamelistdb = gameListsContext;
      backloggdConfig = config.BACKLOGGD;
      cookieContainer = new CookieContainer();

      String proxyURL = "http://localhost:8080";
      HttpClientHandler httpClientHandler = new HttpClientHandler
      {
        CookieContainer = cookieContainer,
        UseCookies = true,

        // Only for Debugging
        #if DEBUG
        Proxy = new WebProxy(proxyURL),
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        #endif
      };

      _client = new(httpClientHandler);
      _client.DefaultRequestHeaders.Accept.Clear();

      _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");
      _client.DefaultRequestHeaders.Add("Origin", "https://www.backloggd.com/");
      _client.DefaultRequestHeaders.Add("Referer", "https://www.backloggd.com/");
      _client.DefaultRequestHeaders.Add("Cookie",
                  //"cookies_consent=true;remember_user_token=1;" +
                  "ne_cookies_consent=false;" +
                  "advanced_open=false;" +
                  "daily_tip=%2C%20your%20collection%20awaits...;" +
                  "_august_app_session=" + backloggdConfig.COOKIE_TOKEN + ";" +
                  "_backloggd_session=" + backloggdConfig.BACKLOGGD_SESSION + ";" +
                  "remember_user_token=" + backloggdConfig.REMEMBER_USER_TOKEN + ";" +
                  "game-log-editor-mode='quick';"
                  );
    }

    private async Task CRFSetter(string htmlDocument)
    {
      
      HtmlDocument htmlSnippet = new HtmlDocument();
      htmlSnippet.LoadHtml(htmlDocument);
      csrf = htmlSnippet.DocumentNode.SelectSingleNode("/html/head/meta[15]").Attributes["content"].Value;

    }

    public async Task RunAsync()
    {
      var initRequest = await _client.GetAsync("https://www.backloggd.com/")
                        .ConfigureAwait(false).GetAwaiter().GetResult()
                        .Content.ReadAsStringAsync();
      if (! initRequest.Contains(_DebugUsername)) {Console.WriteLine("User Cookie Doesn't work") ; throw new Exception("User Cookie doesn't work"); }
      await CRFSetter(initRequest);

      foreach (var game in gamelistdb.GetBeatenGames(2024))
      {
        var nvc = new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>("game_id", gamelistdb.GetIgdbId(game).ToString()),
                    // STUBs
          new KeyValuePair<string, string>("playthroughs[0][id]", "-1"),
          new KeyValuePair<string, string>("playthroughs[0][title]", "Log"),
          new KeyValuePair<string, string>("playthroughs[0][rating]", "0"),
          new KeyValuePair<string, string>("playthroughs[0][review]", ""),
          new KeyValuePair<string, string>("playthroughs[0][review_spoilers]", "false"),
          new KeyValuePair<string, string>("playthroughs[0][platform]", ""),
          new KeyValuePair<string, string>("playthroughs[0][hours]", ""),
          new KeyValuePair<string, string>("playthroughs[0][minutes]", ""),
          new KeyValuePair<string, string>("playthroughs[0][is_master]", "false"),
          new KeyValuePair<string, string>("playthroughs[0][is_replay]", "false"),
          new KeyValuePair<string, string>("playthroughs[0][start_date]", ""),
          new KeyValuePair<string, string>("playthroughs[0][finish_date]", ""),
          // END STUB

          new KeyValuePair<string, string>("log[is_play]", (game.Status == "Beaten" || game.Status == "Finished").ToString().ToLower()),
          new KeyValuePair<string, string>("log[is_playing]", (game.Status == "InProgress").ToString().ToLower()),
          new KeyValuePair<string, string>("log[is_backlog]", (game.Status == "NotStarted").ToString().ToLower()),
          new KeyValuePair<string, string>("log[is_wishlist]", "false"),
          new KeyValuePair<string, string>("log[status]", game.Status == "Beaten" ? "completed" : ""),
          new KeyValuePair<string, string>("log[id]", ""),
          new KeyValuePair<string, string>("modal_type", "quick"),

        };
        var message = new HttpRequestMessage(HttpMethod.Post, 
        GetURL(gamelistdb.GetIgdbId(game).ToString())) { Content = new FormUrlEncodedContent(nvc) };
        message.Headers.Add("X-CSRF-Token", csrf);
      message.Headers.Add("X-Requested-With", "XMLHttpRequest");
      message.Headers.Add("Sec-Fetch-Dest", "empty");
      message.Headers.Add("Sec-Fetch-Mode", "cors");
      message.Headers.Add("Sec-Fetch-Site", "same-origin");

      var debug = await _client.SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult().Content.ReadAsStringAsync();
      }
    }

    private string GetURL(string gameId)
    {
      return "https://www.backloggd.com/api/user/" + backloggdConfig.BACKLOGGD_ID + "/log/" + gameId;
    }

  }
}