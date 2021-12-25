namespace SchoolExam.Domain.ValueObjects;

public class Address
{
    public string StreetName { get; }
    public string StreetNumber { get; }
    public string PostCode { get; }
    public string City { get; }
    public string Country { get; }

    public Address(string streetName, string streetNumber, string postCode, string city, string country)
    {
        StreetName = streetName;
        StreetNumber = streetNumber;
        PostCode = postCode;
        City = city;
        Country = country;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not Address other)
            return false;
        return Equals(other);
    }

    protected bool Equals(Address other)
    {
        return StreetName.Equals(other.StreetName) && StreetNumber.Equals(other.StreetNumber) &&
               PostCode.Equals(other.PostCode) && City.Equals(other.City) && Country.Equals(other.Country);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StreetName, StreetNumber, PostCode, City, Country);
    }
}