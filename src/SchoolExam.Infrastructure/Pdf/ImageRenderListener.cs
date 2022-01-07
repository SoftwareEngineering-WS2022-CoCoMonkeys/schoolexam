using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using SchoolExam.Application.Pdf;

namespace SchoolExam.Infrastructure.Pdf;

internal class ImageRenderListener : IEventListener
{
    internal event EventHandler<ImageParsedEventArgs> ImageParsed;
    internal event EventHandler<ImageParsingFailedEventArgs> ImageParsingFailed;

    public void EventOccurred(IEventData data, EventType type)
    {
        if (data is ImageRenderInfo imageData)
        {
            var imageObject = imageData.GetImage();
            var matrix = imageData.GetImageCtm();
            var rotationMatrix = new RotationMatrix(matrix.Get(Matrix.I11), matrix.Get(Matrix.I12),
                matrix.Get(Matrix.I21), matrix.Get(Matrix.I22));
            if (imageObject == null)
            {
                OnImageParsingFailed(new ImageParsingFailedEventArgs("Image could not be read"));
            }

            OnImageParsed(new ImageParsedEventArgs(imageObject!.GetImageBytes(), rotationMatrix));
        }
    }

    public ICollection<EventType> GetSupportedEvents()
    {
        return new List<EventType> {EventType.RENDER_IMAGE};
    }

    protected virtual void OnImageParsed(ImageParsedEventArgs e)
    {
        ImageParsed(this, e);
    }

    protected virtual void OnImageParsingFailed(ImageParsingFailedEventArgs e)
    {
        ImageParsingFailed(this, e);
    }
}