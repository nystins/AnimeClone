using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeClone.Models;

public class AnimeTag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? AnimeId { get; set; }
    public Anime Anime { get; set; }
    public int? TagId { get; set; }
    public Tag Tag { get; set; }
}