namespace Nex.AktivWinner.Crawler.Entities;

public record SetData
{
    public int SatzNr { get; set; }
    public int Volle { get; set; }
    public int Abraeumen { get; set; }
    public int Fehl { get; set; }
    public int Gesamt => Volle + Abraeumen;
    public decimal SatzPunkt { get; set; } = 0.0m;
}