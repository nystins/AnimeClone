using AnimeClone.Data;
using AnimeClone.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class DataSeeder
{
    private readonly AnimeDbContext _context;

    public DataSeeder(AnimeDbContext context)
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
        foreach (var anime in animeList.Take(1000))
        {
            // Check if AnimeSeason exists, if not, create it
            var season = await _context.AnimeSeasons
                .FirstOrDefaultAsync(s =>
                    anime.AnimeSeason != null &&
                    (s.Season == anime.AnimeSeason.Season && s.Year == anime.AnimeSeason.Year));

            var season1 =  await _context.AnimeSeasons.FirstOrDefaultAsync();
            if (season == null && anime.AnimeSeason != null)
            {
                season = new AnimeSeason { Season = anime.AnimeSeason.Season, Year = anime.AnimeSeason.Year };
                _context.AnimeSeasons.Add(season); 
                await _context.SaveChangesAsync();
            }

            // Check if Duration exists, if not, create it
            var duration = await _context.Durations
                .FirstOrDefaultAsync(d =>
                    anime.Duration != null && (d.Value == anime.Duration.Value && d.Unit == anime.Duration.Unit));

            if (duration == null && anime.Duration != null)
            {
                duration = new Duration { Value = anime.Duration.Value, Unit = anime.Duration.Unit };
                _context.Durations.Add(duration);
            }

            // Update the anime references
            anime.DurationId = duration?.Id;
            anime.AnimeSeasonId = season?.Id;

            // Handle tags
            if (anime.Tags != null && anime.Tags.Count > 0)
            {
                foreach (var tagName in anime.Tags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        _context.Tags.Add(tag);
                    }

                    var animeTag = new AnimeTag { Anime = anime, Tag = tag };
                    _context.Add(animeTag);
                }
            }

            _context.Animes.Add(anime);
        }

        // Save all changes at once
        await _context.SaveChangesAsync();
    }
}