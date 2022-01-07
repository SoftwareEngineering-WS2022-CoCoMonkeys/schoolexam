namespace SchoolExam.Infrastructure.Pdf;

internal class ImageParsingFailedEventArgs : EventArgs
{
    internal string Reason { get; }

    public ImageParsingFailedEventArgs(string reason)
    {
        Reason = reason;
    }
}