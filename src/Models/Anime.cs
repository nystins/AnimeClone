using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeClone.Models;

public class Anime
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    public ICollection<string> Sources { get; set; }
    public ICollection<string> Synonyms { get; set; }
    public ICollection<string> RelatedAnime { get; set; }
    public ICollection<AnimeTag> AnimeTags { get; set; }
    public List<string>? Tags { get; set; }


}