using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Extensions.Logging;
using SchoolExam.Application.Pdf;

namespace SchoolExam.Infrastructure.Pdf;

public class iText7PdfService : IPdfService
{
    private const string ModificationDateName = "ModificationDate";
    
    private readonly ILogger<iText7PdfService> _logger;
    
    public iText7PdfService(ILogger<iText7PdfService> logger)
    {
        _logger = logger;
    }
    
    public int GetNumberOfPages(byte[] pdf)
    {
        using var memoryStream = new MemoryStream(pdf);
        var pdfReader = new PdfReader(memoryStream);
        var pdfDocument = new PdfDocument(pdfReader);
        var result = pdfDocument.GetNumberOfPages();
        pdfDocument.Close();

        return result;
    }

    public byte[] RenderImages(byte[] pdf, params PdfImageRenderInfo[] images)
    {
        using var readStream = new MemoryStream(pdf);
        using var writeStream = new MemoryStream();
        var pdfReader = new PdfReader(readStream);
        var pdfWriter = new PdfWriter(writeStream);
        var pdfDocument = new PdfDocument(pdfReader, pdfWriter);
        var document = new Document(pdfDocument);

        foreach (var image in images)
        {
            var imageData = ImageDataFactory.CreatePng(image.Data);
            var pdfImage = new Image(imageData, image.Left, image.Bottom, image.Width).SetPageNumber(image.Page);
            document.Add(pdfImage);
        }

        document.Close();
        var result = writeStream.ToArray();

        return result;
    }

    public IEnumerable<PdfImageParseInfo> ParseImages(byte[] pdf)
    {
        using var memoryStream = new MemoryStream(pdf);
        var pdfReader = new PdfReader(memoryStream);
        var pdfDocument = new PdfDocument(pdfReader);

        ICollection<PdfImageParseInfo> images = new List<PdfImageParseInfo>();
        int page = 0;

        var strategy = new ImageRenderListener();
        strategy.ImageParsed += (sender, e) => { images.Add(new PdfImageParseInfo(page, e.Data)); };
        strategy.ImageParsingFailed += (sender, e) =>
        {
            _logger.LogWarning($"Image on page {page} could not be parsed. Reason: {e.Reason}.");
        };

        var parser = new PdfCanvasProcessor(strategy);
        for (page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
        {
            parser.ProcessPageContent(pdfDocument.GetPage(page));
        }

        pdfDocument.Close();

        return images;
    }

    public IList<byte[]> Split(byte[] pdf)
    {
        using var readStream = new MemoryStream(pdf);
        var pdfReader = new PdfReader(readStream);
        var pdfDocument = new PdfDocument(pdfReader);

        var splitter = new InMemorySplitter(pdfDocument);
        var pdfSplit = splitter.SplitByPageCount(1);
        
        var result = new List<byte[]>();
        for (int i = 0; i < pdfSplit.Count; i++)
        {
            pdfSplit[i].Close();
            result.Add(splitter.OutputStreams[i].ToArray());
        }

        pdfDocument.Close();

        return result;
    }

    public byte[] Merge(params byte[][] pdfs)
    {
        using var writeStream = new MemoryStream();
        var pdfWriter = new PdfWriter(writeStream);
        var pdfTargetDocument = new PdfDocument(pdfWriter);
        var merger = new PdfMerger(pdfTargetDocument);
        
        foreach (var pdf in pdfs)
        {
            using var readStream = new MemoryStream(pdf);
            var pdfReader = new PdfReader(readStream);
            var pdfSourceDocument = new PdfDocument(pdfReader);
            merger.Merge(pdfSourceDocument, 1, pdfSourceDocument.GetNumberOfPages());
            pdfSourceDocument.Close();
        }
        pdfTargetDocument.Close();

        return writeStream.ToArray();
    }

    public bool Compare(byte[] first, byte[] second)
    {
        var comparer = new CompareTool();
        using var streamFirst = new MemoryStream(first);
        using var streamSecond = new MemoryStream(second);
        var readerFirst = new PdfReader(streamFirst);
        var readerSecond = new PdfReader(streamSecond);
        var pdfDocumentFirst = new PdfDocument(readerFirst);
        var pdfDocumentSecond = new PdfDocument(readerSecond);
        
        var result = comparer.CompareByCatalog(pdfDocumentFirst, pdfDocumentSecond);
        
        pdfDocumentFirst.Close();
        pdfDocumentSecond.Close();
        return result.IsOk();
    }

    public DateTime GetModificationDate(byte[] pdf)
    {
        using var stream = new MemoryStream(pdf);
        var reader = new PdfReader(stream);
        var pdfDocument = new PdfDocument(reader);
        var pdfDocumentInfo = pdfDocument.GetDocumentInfo();
        var modificationDateString = pdfDocumentInfo.GetMoreInfo(ModificationDateName);
        var result = PdfDate.Decode(modificationDateString);

        pdfDocument.Close();
        return result;
    }
}