namespace Nex.AktivWinner.Data.Models;

public class Season
{
    public int Id { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public string Name { get; set; }

    public virtual ICollection<League> Leagues { get; set; } = new List<League>();
}