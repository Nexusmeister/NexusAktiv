namespace Nex.AktivWinner.Data.Models;

public class League
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; }
    public virtual Season Season { get; set; }
}