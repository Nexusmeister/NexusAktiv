namespace AktivCrawler.Entities;

public sealed record PlayerData
{
    public required string Name { get; set; }
    public required PlayerPassData PassNr { get; set; }
    public required SortedList<int, SetData> Saetze { get; set; } = new();
    public int GesamtHolz => Saetze.Sum(x => x.Value.Gesamt);
    public int GesamtAbraeumen => Saetze.Sum(x => x.Value.Abraeumen);
    public int GesamtVolle => Saetze.Sum(x => x.Value.Volle);
    public int GesamtFehl => Saetze.Sum(x => x.Value.Fehl);
    public decimal GesamtSatzpunkte => Saetze.Sum(x => x.Value.SatzPunkt);
    public decimal Mannschaftspunkt { get; set; }
}