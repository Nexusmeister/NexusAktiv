using Syncfusion.Pdf.Parsing;
using System.IO;

namespace Nex.AktivWinner.Crawler.Services;

public class PdfReaderService : IFileReaderService
{
    public string ReadData(string filepath)
    {
        var inputPdfStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var pdftext = ExtractTextFromPdf(inputPdfStream);
        return pdftext;
    }

    public string ReadData(Stream stream)
    {
        var pdftext = ExtractTextFromPdf(stream);
        return pdftext;
    }

    private static string ExtractTextFromPdf(Stream stream)
    {
        using var loadedDocument = new PdfLoadedDocument(stream);

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