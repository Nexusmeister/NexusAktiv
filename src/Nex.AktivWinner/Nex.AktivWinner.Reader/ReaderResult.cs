﻿namespace Nex.AktivWinner.Reader;

public class ReaderResult
{
    public required string[]? ReadLines { get; init; }
    public string CompactString => ReadLines is null ? string.Empty : string.Join(Environment.NewLine, ReadLines);
}