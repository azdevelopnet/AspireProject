using System;
using System.ComponentModel;

namespace Aspire.ApiServices.Models
{
    public enum UserType
    {
        Provider,
        Patient,
        Staff
    }
    public enum ProviderType
    {
        [Description("Pediatrics")]
        Pediatrics,
        [Description("Family Medicine")]
        FamilyMedicine,
        [Description("Neurology")]
        Neurology,
        [Description("Cardiology")]
        Cardiology,
        [Description("Endocrinology")]
        Endocrinology,
        [Description("Psychology")]
        Psychology,
        [Description("Oncology")]
        Oncology,
        [Description("Internist")]
        Internist
    }

    public class ProviderDetail
    {
        public ProviderType ProviderType { get; set; }
        public bool IsTakingPatients { get; set; }
        public string[] Certifications { get; set; }
        public DateTime PracticeDate { get; set; }
    }

    public class User: MongoDbbase
    {
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public UserType UserType { get; set; }
        public ProviderDetail? ProviderDetail { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ProfileImageBase64 { get; set; }
        public InsuranceInfo? InsuranceInfo { get; set; }
    }

    public class InsuranceInfo
    {
        public string Provider { get; set; }
        public string MemberName { get; set; }
        public string PolicyNumber { get; set; }
        public string GroupNumber { get; set; }
        public string ProviderPhone { get; set; }
    }
    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }
    }
}
