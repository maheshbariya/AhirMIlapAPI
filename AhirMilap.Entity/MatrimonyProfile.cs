namespace AhirMilap.Entity
{
    public class MatrimonyProfile : BaseEntity
    {  
        public string UserId { get; set; }   // AspNetUsers Id

        public string ProfileFor { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime? DOB { get; set; }

        public int? Age { get; set; }

        public string MaritalStatus { get; set; }
        public string RequestStatus { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string? AboutMe { get; set; }


        public string Religion { get; set; }
        public string Caste { get; set; }
        public string SubCaste { get; set; }

        public string HighestEducation { get; set; }
        public string CollegeUniversity { get; set; }

        public string Occupation { get; set; }
        public string CompanyName { get; set; }

        public decimal? AnnualIncome { get; set; }

        public decimal? HeightCm { get; set; }
        public decimal? WeightKg { get; set; }

        public string Complexion { get; set; }

        public string PhysicalDisability { get; set; }

        public string DisabilityDetails { get; set; }

        public string FatherName { get; set; }
        public string FatherOccupation { get; set; }

        public string MotherName { get; set; }
        public string MotherOccupation { get; set; }

        public string FamilyType { get; set; }

        public string? AadhaarFilePath1 { get; set; }
        public string? AadhaarFilePath2 { get; set; }
        public string? ProfileFilePath { get; set; }

        public bool IsVerified { get; set; }
        public bool IsVerifyAadhaar { get; set; }

        public int ProfileCompletion { get; set; }
        public string? LanguageKnow { get; set; }
        public string? ProfileCode { get; set; }
        public string? Email { get; set; }
        public string? PageFlag { get; set; }
        public int ProfileStep { get; set; }
        public bool IsSentNotification { get; set; }

        public List<MatrimonyProfilePhotos> ProfilePhotos { get; set; }
        public List<Siblings> Siblings { get; set; } = new();
    }
}
