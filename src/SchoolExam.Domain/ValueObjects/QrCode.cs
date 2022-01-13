namespace SchoolExam.Domain.ValueObjects;

public class QrCode
{
    public string Data { get; }

    public QrCode(string data)
    {
        Data = data;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not QrCode other)
            return false;
        return Equals(other);
    }

    protected bool Equals(QrCode other)
    {
        return Data.Equals(other.Data);
    }

    public override int GetHashCode()
    {
        return Data.GetHashCode();
    }

    public static implicit operator QrCode(string data) => new QrCode(data);
}