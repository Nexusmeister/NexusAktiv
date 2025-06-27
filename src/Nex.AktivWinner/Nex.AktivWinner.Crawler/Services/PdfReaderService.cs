using Syncfusion.Pdf.Parsing;

namespace Nex.AktivWinner.Crawler.Services;

public class PdfReaderService : IFileReaderService
{
    public string ReadData(string filepath)
    {
        var pdftext = ExtractTextFromPdf(filepath);
        return pdftext;
    }

    private static string ExtractTextFromPdf(string path)
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