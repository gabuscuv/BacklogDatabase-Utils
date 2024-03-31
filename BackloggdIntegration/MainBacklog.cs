// https://www.backloggd.com/api/user/79153/log/122748
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using GameListDB.Model;
using GameListDB.Model.Extensions;
using GameListDB.DTO;

namespace GameListDB.BackloggdIntegration
{

  public class BackloggdExporter
  {
    private readonly HttpClient _client;
    private readonly Backloggd backloggdConfig;
    private readonly GameListsContext gamelistdb;

    public BackloggdExporter(GameListsContext gameListsContext, Config config) 
    {
      gamelistdb = gameListsContext;
      backloggdConfig = config.BACKLOGGD;

      _client = new();
      _client.DefaultRequestHeaders.Accept.Clear();
      
      _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");
      _client.DefaultRequestHeaders.Add("Origin", "https://www.backloggd.com/");
      _client.DefaultRequestHeaders.Add("Referer", "https://www.backloggd.com/");
      _client.DefaultRequestHeaders.Add("Cookie", 
                  "cookies_consent=true;remember_user_token=1;"+
                  "_august_app_session='"+backloggdConfig.COOKIE_TOKEN+"';"+
                  "_backloggd_session='"+backloggdConfig.BACKLOGGD_SESSION+"';"+
                  "remember_user_token='"+backloggdConfig.REMEMBER_USER_TOKEN+"'"
                  );
        _client.DefaultRequestHeaders.Add("X-CSRF-Token", backloggdConfig.CSRF_TOKEN);

    }

    public async Task RunAsync()
    {
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

          new KeyValuePair<string, string>("log[is_play]", (game.Status == "Beaten").ToString().ToLower()),
          new KeyValuePair<string, string>("log[is_playing]", (game.Status == "InProgress").ToString().ToLower()),
          new KeyValuePair<string, string>("log[is_backlog]", (game.Status == "NotStarted").ToString().ToLower()),
          new KeyValuePair<string, string>("log[is_wishlist]", "false"),
          new KeyValuePair<string, string>("log[status]", game.Status == "Beaten" ? "completed" : ""),
          new KeyValuePair<string, string>("log[id]", ""),
          new KeyValuePair<string, string>("modal_type", "quick"),

        };
      
      var debug = await _client.PostAsync(
                GetURL(gamelistdb.GetIgdbId(game).ToString()),
                new FormUrlEncodedContent(nvc)).ConfigureAwait(false);
      }
    }

    private string GetURL(string gameId)
    {
      return "https://www.backloggd.com/api/user/" + backloggdConfig.BACKLOGGD_ID + "/log/" + gameId;
    }

  }
}