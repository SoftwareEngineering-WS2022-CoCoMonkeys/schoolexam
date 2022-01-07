using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using SchoolExam.Extensions;

namespace SchoolExam.Infrastructure.Pdf;

public class InMemorySplitter : PdfSplitter
{
    private readonly IList<MemoryStream> _outputStreams;

    public IReadOnlyList<MemoryStream> OutputStreams => _outputStreams.AsReadOnly();

    public InMemorySplitter(PdfDocument pdfDocument) : base(pdfDocument)
    {
        _outputStreams = new List<MemoryStream>();
    }

    protected override PdfWriter GetNextPdfWriter(PageRange documentPageRange)
    {
        var memoryStream = new MemoryStream();
        _outputStreams.Add(memoryStream);
        var pdfWriter = new PdfWriter(memoryStream);

        return pdfWriter;
    }
}