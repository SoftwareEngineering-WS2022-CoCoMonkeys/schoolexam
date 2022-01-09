namespace SchoolExam.Application.Pdf;

public class PdfOutlineInfo
{
    public string Title { get; }
    public int DestinationPage { get; }
    public float DestinationY { get; }

    public PdfOutlineInfo(string title, int destinationPage, float destinationY)
    {
        Title = title;
        DestinationPage = destinationPage;
        DestinationY = destinationY;
    }
}