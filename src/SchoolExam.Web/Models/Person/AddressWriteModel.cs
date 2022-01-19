namespace SchoolExam.Web.Models.Person;

public class AddressWriteModel
{
    public string StreetName { get; set; } = null!;
    public string StreetNumber { get; set; } = null!;
    public string PostCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
}