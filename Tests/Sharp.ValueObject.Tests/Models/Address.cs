namespace Sharp.ValueObject.Tests.Models
{
    public class Address : ComplexValueObject<Address>
    {
        public Address(string country, string? city, int postCode)
        {
            Country = country;
            City = city;
            PostCode = postCode;
        }

        public string Country { get; }

        public string? City { get; }

        public int PostCode { get; }
    }
}
