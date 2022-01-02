using SchoolExam.Application.Pdf;

namespace SchoolExam.Infrastructure.Pdf;

internal class ImageParsedEventArgs : EventArgs
{
    internal byte[] Data { get; set; }
    internal RotationMatrix RotationMatrix { get; set; }

    internal ImageParsedEventArgs(byte[] data, RotationMatrix rotationMatrix)
    {
        Data = data;
        RotationMatrix = rotationMatrix;
    }
}