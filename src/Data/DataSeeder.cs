using System.Text.Json;
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

    // public async Task SeedAsync(string jsonFilePath)
    // {
        // // Read the JSON file
        // string jsonString = await File.ReadAllTextAsync(jsonFilePath);
        // var animeList = JsonSerializer.Deserialize<List<Anime>>(jsonString);
        //
        // if (animeList != null)
        // {
        //     foreach (var anime in animeList)
        //     {
        //         // Check if AnimeSeason exists, if not, insert it
        //         AnimeSeason animeSeason = await _context.AnimeSeasons
        //             .FirstOrDefaultAsync(animeSeason => animeSeason.Season == anime.AnimeSeason.Season && animeSeason.Year == anime.AnimeSeason.Year);
        //
        //         if (animeSeason == null)
        //         {
        //             // If AnimeSeason doesn't exist, insert it
        //             animeSeason = new AnimeSeason
        //             {
        //                 Season = anime.AnimeSeason.Season,
        //                 Year = anime.AnimeSeason.Year
        //             };
        //             _context.AnimeSeasons.Add(animeSeason);
        //             await _context.SaveChangesAsync();  // Save AnimeSeason and get the generated Id
        //         }
        //
        //         // Check if Duration exists, if not, insert it
        //         Duration duration = await _context.Durations
        //             .FirstOrDefaultAsync(d => d.Value == anime.Duration.Value && d.Unit == anime.Duration.Unit);
        //
        //         if (duration == null)
        //         {
        //             // If Duration doesn't exist, insert it
        //             duration = new Duration
        //             {
        //                 Value = anime.Duration.Value,
        //                 Unit = anime.Duration.Unit
        //             };
        //             _context.Durations.Add(duration);
        //             await _context.SaveChangesAsync();  // Save Duration and get the generated Id
        //         }
        //         
        //         // Check if Duration exists, if not, insert it
        //         // 3. Check if Tags exist, or create them
        //         List<Tag> tags = new List<Tag>();
        //         foreach (var tagName in anime.Tags)
        //         {
        //             Tag tag = await _context.Tags
        //                 .FirstOrDefaultAsync(t => t.Name == tagName.Name);
        //
        //             if (tag == null)
        //             {
        //                 // If the tag doesn't exist, create it
        //                 tag = new Tag
        //                 {
        //                     Name = tagName.Name
        //                 };
        //
        //                 // Add the new tag to the context and save
        //                 _context.Tags.Add(tag);
        //                 await _context.SaveChangesAsync(); // This will generate the tag.Id
        //             }
        //
        //             // Add tag to the list (for linking later)
        //             tags.Add(tag);
        //         }
        //         
        //         if (duration == null)
        //         {
        //             // If Duration doesn't exist, insert it
        //             duration = new Duration
        //             {
        //                 Value = anime.Duration.Value,
        //                 Unit = anime.Duration.Unit
        //             };
        //             _context.Durations.Add(duration);
        //             await _context.SaveChangesAsync();  // Save Duration and get the generated Id
        //         }
        //
        //         // Now we can create the Anime, linking it to the existing or newly created AnimeSeason and Duration
        //         // var newAnime = new Anime
        //         // {
        //         //     Title = anime.Title,
        //         //     Type = anime.Type,
        //         //     Episodes = anime.Episodes,
        //         //     Status = anime.Status,
        //         //     AnimeSeasonId = animeSeason.Id,  // Link the Anime to the AnimeSeason
        //         //     DurationId = duration.Id,        // Link the Anime to the Duration
        //         //     Picture = anime.Picture,
        //         //     Thumbnail = anime.Thumbnail,
        //         //     Synonyms = anime.Synonyms,
        //         //     Tags = anime.Tags,  // Assuming tags are handled in a similar fashion
        //         //     Sources = anime.Sources,
        //         //     RelatedAnime = anime.RelatedAnime
        //         // };
        //         //
        //         
        //         anime.DurationId = duration.Id;
        //         anime.AnimeSeasonId = animeSeason.Id;
        //         _context.Animes.Add(anime);
        //         await _context.SaveChangesAsync();  // Save Anime with the foreign keys
        //     }
        // }
    // }
    
    public void Seed(string jsonData)
    {
        _context.Database.Migrate(); // Apply migrations if not applied

        var data = JsonConvert.DeserializeObject<Anime>(jsonData);
        if (data?.Data == null || _context.Animes.Any()) return;

        foreach (var animeItem in data.Data)
        {
            var season = _context.AnimeSeasons.FirstOrDefault(s => s.Season == animeItem.AnimeSeason.Season && s.Year == animeItem.AnimeSeason.Year) 
                         ?? new AnimeSeason { Season = animeItem.AnimeSeason.Season, Year = animeItem.AnimeSeason.Year };

            var duration = _context.Durations.FirstOrDefault(d => d.Value == animeItem.Duration.Value && d.Unit == animeItem.Duration.Unit) 
                           ?? new Duration { Value = animeItem.Duration.Value, Unit = animeItem.Duration.Unit };

            var anime = new Anime
            {
                Title = animeItem.Title,
                Description = string.Join(", ", animeItem.Synonyms),
                AnimeSeason = season,
                Duration = duration
            };

            _context.AnimeSeasons.Add(season);
            _context.Durations.Add(duration);
            _context.Animes.Add(anime);
            _context.SaveChanges(); // Save anime first to get its ID

            // Insert tags and establish many-to-many relationship
            foreach (var tagName in animeItem.Tags)
            {
                var tag = _context.Tags.FirstOrDefault(t => t.Name == tagName) 
                          ?? new Tag { Name = tagName };

                if (tag.Id == 0) // New tag, add to DB
                {
                    _context.Tags.Add(tag);
                    _context.SaveChanges();
                }

                var animeTag = new AnimeTag { AnimeId = anime.Id, TagId = tag.Id };
                _context.Add(animeTag);
            }

            _context.SaveChanges(); // Save AnimeTag relationships
        } 
}
