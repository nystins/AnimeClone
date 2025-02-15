using AnimeClone.Data;
using AnimeClone.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


public class DataSeeder
{
    private readonly AnimeCloneContext _context;

    public DataSeeder(AnimeCloneContext context)
    {
        _context = context;
    }

    public async Task Seed()
    {
        var dir = Directory.GetCurrentDirectory();
        string jsonString = await File.ReadAllTextAsync($"{dir}/resources/anime-offline-database.json");

        _context.Database.EnsureDeleted();
        _context.Database.Migrate(); // Apply migrations if not applied

        var jsonSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        var animeList = JsonConvert.DeserializeObject<IEnumerable<Anime>>(jsonString, jsonSettings);
        if ((animeList == null) || _context.Animes.Any()) return;

        // Use local lists instead of querying the database
        var animeSeasonsList = new List<AnimeSeason>();
        var durationsList = new List<Duration>();
        var tagsList = new List<Tag>();
        var animeInsertList = new List<Anime>();

        foreach (var anime in animeList.Take(100))
        {
            // Use local list to check if AnimeSeason exists
            //Console.WriteLine(anime.AnimeSeason.Season);
            AnimeSeason? season = null;
            if (anime.AnimeSeason != null)
            {
                season = animeSeasonsList.FirstOrDefault(s =>
                    s.Season == anime.AnimeSeason.Season && s.Year == anime.AnimeSeason.Year);

                if (season == null)
                {
                    season = new AnimeSeason
                    {
                        Season = anime.AnimeSeason.Season,
                        Year = anime.AnimeSeason.Year
                    };
                    animeSeasonsList.Add(season);
                    //_context.AnimeSeasons.Add(season);  
                }
            }

            // Use local list to check if Duration exists
            Duration? duration = null;
            if (anime.Duration != null)
            {
                duration = durationsList.FirstOrDefault(d =>
                    d.Value == anime.Duration.Value && d.Unit == anime.Duration.Unit);

                if (duration == null)
                {
                    duration = new Duration
                    {
                        Value = anime.Duration.Value,
                        Unit = anime.Duration.Unit
                    };
                    durationsList.Add(duration);
                }
            }

            // Assign IDs
            anime.DurationId = duration?.Id;
            anime.AnimeSeasonId = season?.Id;

            // Handle tags using a local list
            if (anime.Tags != null && anime.Tags.Count > 0)
            {
                foreach (var tagName in anime.Tags)
                {
                    var tag = tagsList.FirstOrDefault(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tag
                        {
                            Name = tagName
                        };
                        tagsList.Add(tag);
                        _context.Tags.Add(tag);
                    }
                }
            }

            anime.Duration = duration;
            anime.AnimeSeason = season;
            animeInsertList.Add(anime);
        }

        try
        {
            if (animeSeasonsList.Any())
            {
                _context.AnimeSeasons.AddRange(animeSeasonsList);
            }

            if (durationsList.Any())
            {
                _context.Durations.AddRange(durationsList);
            }

            if (tagsList.Any())
            {
                _context.Tags.AddRange(tagsList);
            }

            if (animeInsertList.Any())
            {
                _context.Animes.AddRange(animeInsertList);
            }

            await _context.SaveChangesAsync();


            var newAnimeTags = new List<AnimeTag>();
            foreach (var anime in animeInsertList)
            {
                if (anime.Tags != null)
                {
                    foreach (var tagName in anime.Tags)
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                        if (tag == null)
                        {
                            tag = new Tag
                            {
                                Name = tagName
                            };
                            _context.Tags.Add(tag);
                            await _context.SaveChangesAsync();
                        }
                        newAnimeTags.Add(new AnimeTag
                        {
                            AnimeId = anime.Id,
                            TagId = tag.Id
                        });
                    }
                }
            }
            if (newAnimeTags.Any())
            {
                _context.AnimeTag.AddRange(newAnimeTags);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}