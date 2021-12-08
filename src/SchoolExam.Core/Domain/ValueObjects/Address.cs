namespace SchoolExam.Core.Domain.ValueObjects
{
    public class Address
    {
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public Address(string streetName, string streetNumber, string postCode, string city, string country)
        {
            StreetName = streetName;
            StreetNumber = streetNumber;
            PostCode = postCode;
            City = city;
            Country = country;
        }
    }
}