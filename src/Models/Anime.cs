namespace AnimeClone.Models;

public class Anime
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public int Episodes { get; set; }
    public string Status { get; set; }
    public string Picture { get; set; }
    public string Thumbnail { get; set; }
    public int? DurationId { get; set; } 
    public Duration Duration { get; set; }
    public int? AnimeSeasonId { get; set; }  
    public AnimeSeason AnimeSeason { get; set; }
    public List<string> Sources { get; set; }
    public List<string> Synonyms { get; set; }
    public List<string> RelatedAnime { get; set; }
    public List<AnimeTag> AnimeTags { get; set; } = new();
    
    
}