using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;

namespace AktivReader;

public class PdfMatchResultReaderService : IReaderService
{
    public Task<List<PlayerData>> ReadDataAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    private async Task<string> ExtractTextFromPdf(string path)
    {
        var inputPdfStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        var loadedDocument = new PdfLoadedDocument(inputPdfStream);

        // Dead pdf
        if (loadedDocument.Pages.Count == 0)
        {
            return string.Empty;
        }

        var page = loadedDocument.Pages[0];

        var extractedText = page.ExtractText();

        loadedDocument.Close(true);
        return extractedText;
    }
}