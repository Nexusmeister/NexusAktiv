﻿namespace Nex.AktivWinner.Data.Models;

public class Team
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public required string Name { get; set; }
}