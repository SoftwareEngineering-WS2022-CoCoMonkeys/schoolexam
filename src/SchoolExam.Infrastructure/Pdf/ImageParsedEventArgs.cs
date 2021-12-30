namespace SchoolExam.Infrastructure.Pdf;

internal class ImageParsedEventArgs : EventArgs
{
    internal byte[] Data { get; set; }

    internal ImageParsedEventArgs(byte[] data)
    {
        Data = data;
    }
}