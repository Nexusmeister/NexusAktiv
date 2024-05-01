// See https://aka.ms/new-console-template for more information

using System.Text;
using AktivReader;
using AktivReader.Database.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using static System.Net.Mime.MediaTypeNames;


var files = Directory.GetFiles("F:\\AktivCrawling");

foreach (var file in files)
{
    var output = ExtractTextFromPdf(file);

    string[] lines = output.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    //string result = string.Join(Environment.NewLine, lines);
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

    //var players = new List<PlayerData>();
    //foreach (var player in resultList)
    //{
    //    if (player is not PlayerReaderResult p)
    //    {
    //        continue;
    //    }
    //    players.Add(ExtractPlayerData(p));
    //}

    var metadata = resultList.Where(x => x is MetadataReaderResult).Cast<MetadataReaderResult>().ToList();

    await SaveClubnames(metadata);

}




Console.Read();


static string ExtractTextFromPdf(string path)
{
    FileStream inputPDFStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputPDFStream);

    PdfPageBase page = loadedDocument.Pages[0];

    string extractedText = page.ExtractText();

    loadedDocument.Close(true);
    return extractedText;
}

static PlayerData ExtractPlayerData(PlayerReaderResult playerdata)
{
    // Sonderfall Auswechslung
    var linesToWork = playerdata.ReadLines?.ToList();
    linesToWork?.RemoveRange(0, 5);

    var resultLines = linesToWork.GetRange(0, 20);
    var listSets = new SortedList<int, SetData>();

    var chunkedSets = resultLines.Chunk(5);
    var setNo = 1;
    foreach (var setLines in chunkedSets)
    {
        listSets.Add(setNo, new SetData
        {
            SatzNr = setNo,
            Volle = Convert.ToInt32(setLines[0]),
            Abraeumen = Convert.ToInt32(setLines[1]),
            Fehl = Convert.ToInt32(setLines[2]),
            SatzPunkt = Convert.ToDecimal(setLines[4])
        });

        setNo++;
    }

    var playerMetadataLines = linesToWork[^6..];

    return new PlayerData
    {
        Saetze = listSets,
        PassNr = new PlayerPassData
        {
            PassNo = Convert.ToInt32(playerMetadataLines[2])
        },
        Name = playerMetadataLines[3],
        Mannschaftspunkt = Convert.ToDecimal(playerMetadataLines[^1])
    };
}

static async Task SaveClubnames(IReadOnlyCollection<MetadataReaderResult> metadata)
{
    SKCMatchesContext context = new SKCMatchesContext(new DbContextOptions<SKCMatchesContext>());

    // The last rows are the club names
    if (metadata.Any(x => x.ReadLines is null))
    {
        return;
    }

    var clubNames = metadata.Select(x => x.ReadLines!.TakeLast(1).FirstOrDefault()).ToList();

    var clubs = await context.Clubs.AsQueryable().ToListAsync();

    // Get all team names
    // Add a new list
    // Copy all team names
    // Remove team tags like I II m w g IIg etc.
    // Iterate multiple times over teams until no changes happened
    // Group by Names
    // Insert Clubs (but check if already exists)
    // Insert Teams with respective ClubId (obsolete)

    foreach (var clubName in clubNames.Where(clubName => !clubs.Exists(x => string.Equals(x.ClubName, clubName))))
    {
        // TODO Logic for extracting ONLY the club name (and not team specifications)
        context.Clubs.Add(new Club
        {
            ClubName = clubName
        });
    }

    await context.SaveChangesAsync();
}