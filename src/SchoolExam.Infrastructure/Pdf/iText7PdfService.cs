using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
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

    public byte[] RenderTexts(byte[] pdf, params PdfTextRenderInfo[] texts)
    {
        using var readStream = new MemoryStream(pdf);
        using var writeStream = new MemoryStream();
        var pdfReader = new PdfReader(readStream);
        var pdfWriter = new PdfWriter(writeStream);
        var pdfDocument = new PdfDocument(pdfReader, pdfWriter);
        var document = new Document(pdfDocument);

        foreach (var text in texts)
        {
            var div = new Div().SetPageNumber(text.Page).SetWidth(text.Width).SetHeight(text.Height)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE).SetFixedPosition(text.Left, text.Bottom, text.Width);

            var paragraph = new Paragraph(text.Text);
            div.Add(paragraph);

            document.Add(div);
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
        strategy.ImageParsed += (sender, e) =>
        {
            // get rotation of PDF page
            float pageRotation = (float) (pdfDocument.GetPage(page).GetRotation() * Math.PI / 180.0);
            // create rotation matrix for PDF page rotation from rotation angle
            var pageRotationMatrix = new RotationMatrix(pageRotation);
            // multiply rotation matrix of PDF page with rotation of image
            var rotationMatrix = pageRotationMatrix * e.RotationMatrix;

            images.Add(new PdfImageParseInfo(page, e.Data, rotationMatrix));
        };
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

    public IEnumerable<PdfUriLinkAnnotationInfo> GetUriLinkAnnotations(byte[] pdf)
    {
        using var memoryStream = new MemoryStream(pdf);
        var pdfReader = new PdfReader(memoryStream);
        var pdfDocument = new PdfDocument(pdfReader);

        var result = new List<PdfUriLinkAnnotationInfo>();
        var count = pdfDocument.GetNumberOfPages();
        for (int pageNumber = 1; pageNumber <= count; pageNumber++)
        {
            var page = pdfDocument.GetPage(pageNumber);
            var annotations = page.GetAnnotations();
            foreach (var annotation in annotations)
            {
                var rectangle = annotation.GetRectangle();
                var top = rectangle.GetAsNumber(3).FloatValue();
                var left = rectangle.GetAsNumber(0).FloatValue();
                var right = rectangle.GetAsNumber(2).FloatValue();
                var bottom = rectangle.GetAsNumber(1).FloatValue();
                var width = right - left;
                if (annotation is PdfLinkAnnotation linkAnnotation)
                {
                    var action = linkAnnotation.GetAction();
                    var actionType = action.Get(PdfName.S);
                    if (actionType.Equals(PdfName.URI))
                    {
                        var uri = action.GetAsString(PdfName.URI);
                        result.Add(new PdfUriLinkAnnotationInfo(uri.ToString(), pageNumber, left, top, bottom, width));
                    }
                }
            }
        }
        pdfDocument.Close();

        return result;
    }

    public byte[] RemoveUriLinkAnnotations(byte[] pdf, params PdfUriLinkAnnotationInfo[] annotationsToRemove)
    {
        using var readStream = new MemoryStream(pdf);
        using var writeStream = new MemoryStream();
        var pdfReader = new PdfReader(readStream);
        var pdfWriter = new PdfWriter(writeStream);
        var pdfDocument = new PdfDocument(pdfReader, pdfWriter);

        var annotationsToRemoveSet = annotationsToRemove.ToHashSet();
        
        var count = pdfDocument.GetNumberOfPages();
        for (int pageNumber = 1; pageNumber <= count; pageNumber++)
        {
            var page = pdfDocument.GetPage(pageNumber);
            var annotations = page.GetAnnotations();
            foreach (var annotation in annotations)
            {
                var rectangle = annotation.GetRectangle();
                var top = rectangle.GetAsNumber(3).FloatValue();
                var left = rectangle.GetAsNumber(0).FloatValue();
                var right = rectangle.GetAsNumber(2).FloatValue();
                var bottom = rectangle.GetAsNumber(1).FloatValue();
                var width = right - left;
                if (annotation is PdfLinkAnnotation linkAnnotation)
                {
                    var action = linkAnnotation.GetAction();
                    var actionType = action.Get(PdfName.S);
                    if (actionType.Equals(PdfName.URI))
                    {
                        var uri = action.GetAsString(PdfName.URI);
                        var parsedAnnotation =
                            new PdfUriLinkAnnotationInfo(uri.ToString(), pageNumber, left, top, bottom, width);
                        if (annotationsToRemoveSet.Contains(parsedAnnotation))
                        {
                            page.RemoveAnnotation(annotation);
                        }
                    }
                }
            }
        }
        
        pdfDocument.Close();

        var result = writeStream.ToArray();

        return result;
    }

    public byte[] SetTopLevelOutline(byte[] pdf, params PdfOutlineInfo[] outlineElements)
    {
        using var readStream = new MemoryStream(pdf);
        using var writeStream = new MemoryStream();
        var pdfReader = new PdfReader(readStream);
        var pdfWriter = new PdfWriter(writeStream);
        var pdfDocument = new PdfDocument(pdfReader, pdfWriter);
        
        var outline = pdfDocument.GetOutlines(false);
        // remove current outline elements
        var children = outline.GetAllChildren();
        foreach (var child in children)
        {
            child.RemoveOutline();
        }

        var orderedOutlineElements =
            outlineElements.OrderBy(x => x.DestinationPage).ThenByDescending(x => x.DestinationY);

        foreach (var outlineElement in orderedOutlineElements)
        {
            var newOutline = outline.AddOutline(outlineElement.Title);
            var page = pdfDocument.GetPage(outlineElement.DestinationPage);
            newOutline.AddDestination(PdfExplicitDestination.CreateFitH(page, outlineElement.DestinationY));
        }
        
        pdfDocument.Close();

        var result = writeStream.ToArray();

        return result;
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

    public byte[] Rotate(byte[] pdf, int degrees)
    {
        using var readStream = new MemoryStream(pdf);
        using var writeStream = new MemoryStream();
        var pdfReader = new PdfReader(readStream);
        var pdfWriter = new PdfWriter(writeStream);
        var pdfDocument = new PdfDocument(pdfReader, pdfWriter);
        var document = new Document(pdfDocument);

        for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
        {
            var pdfPage = pdfDocument.GetPage(page);
            var current = pdfPage.GetRotation();
            pdfPage.SetRotation((current + degrees) % 360);
            pdfPage.SetIgnorePageRotationForContent(true);
        }

        document.Close();
        var result = writeStream.ToArray();

        return result;
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

    public byte[] Protect(byte[] pdf, byte[] userPassword, byte[] ownerPassword)
    {
        using var readStream = new MemoryStream(pdf);
        using var writeStream = new MemoryStream();
        var pdfReader = new PdfReader(readStream);
        var writerProps = new WriterProperties().SetStandardEncryption(userPassword, ownerPassword,
            EncryptionConstants.ALLOW_MODIFY_CONTENTS,
            EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA);
        var pdfWriter = new PdfWriter(writeStream, writerProps);
        var pdfDocument = new PdfDocument(pdfReader, pdfWriter);
        pdfDocument.Close();

        var result = writeStream.ToArray();

        return result;
    }
}