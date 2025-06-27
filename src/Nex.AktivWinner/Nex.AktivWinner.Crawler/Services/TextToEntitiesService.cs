using Nex.AktivWinner.Crawler.Entities;

namespace Nex.AktivWinner.Crawler.Services;

public class TextToEntitiesService : ITextToEntitiesService
{
    public List<ReaderResult> GetEntitiesFromText(string linesRead)
    {
        var lines = linesRead.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries);
        var linesList = lines.ToList();

        var resultList = new List<ReaderResult>();

        var vafgOccurence = 0;
        var cutBegin = 0;

        // V A F G
        for (var i = 0; i <= lines.Length; i++)
        {
            if (i == lines.Length)
            {
                var cutEnd = i;
                var cutLines = lines[cutBegin..cutEnd];

                // Split last item into player and sum values
                var indexSumStart = cutLines.ToList().IndexOf("MP") + 2;

                resultList.Add(new PlayerReaderResult()
                {
                    ReadLines = cutLines[..indexSumStart]
                });

                resultList.Add(new MatchoverviewReaderResult()
                {
                    ReadLines = cutLines[indexSumStart..]
                });

                continue;
            }

            // TODO Zwischen drin kommt noch der Mannschaftsname der Gäste => Sonderfall abfangen
            // I 
            if (string.Equals(lines[i], "V")
                && lines[i + 1] == "A" &&
                lines[i + 2] == "F" &&
                lines[i + 3] == "G")
            {
                vafgOccurence++;
                var cutEnd = i;

                // Wir schneiden den Gastvereinsnamen vom letzten Heimspieler ab
                if (vafgOccurence == 7)
                {
                    cutEnd--;
                }

                var cutLines = lines[cutBegin..cutEnd];

                if (resultList.Count == 0)
                {
                    resultList.Add(new MetadataReaderResult()
                    {
                        ReadLines = cutLines
                    });
                }
                else
                {
                    resultList.Add(new PlayerReaderResult
                    {
                        ReadLines = cutLines
                    });
                }

                if (resultList.Count(x => x is PlayerReaderResult) == 6)
                {
                    resultList.Add(new MetadataReaderResult
                    {
                        ReadLines = lines[cutEnd..i]
                    });
                }

                cutBegin = i;
            }
        }

        return resultList;
    }
}