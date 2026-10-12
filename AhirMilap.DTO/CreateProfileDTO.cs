using AhirMilap.Entity;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class CreateProfileDTO
    {
        public string ProfileFor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public int Age { get; set; }
        public string MaritalStatus { get; set; }
        public string State { get; set; }
        public string District { get; set; }

        [MaxLength(500)]
        public string? AboutMe { get; set; }
    }
    public class ReligionDTO
    {
        public string Religion { get; set; }
        public string Caste { get; set; }
        public string SubCaste { get; set; }
        public string LanguageKnow { get; set; }
        public IFormFile? AadharFile1 { get; set; }
        public IFormFile? AadharFile2 { get; set; }
    }
    public class EducationDTO
    {
        public string HighestEducation { get; set; }
        public string CollegeUniversity { get; set; }
        public string Occupation { get; set; }
        public string CompanyName { get; set; }
        public decimal AnnualIncome { get; set; }
    }
    public class LifestyleDTO
    {
        public decimal HeightCm { get; set; }
        public decimal WeightKg { get; set; }
        public string Complexion { get; set; }
        public string PhysicalDisability { get; set; }
        public string DisabilityDetails { get; set; }
    }
    public class FamilyDTO
    {
        public string FatherName { get; set; }
        public string FatherOccupation { get; set; }
        public string MotherName { get; set; }
        public string MotherOccupation { get; set; }
        public string FamilyType { get; set; }
        public List<Siblings> Siblings { get; set; } = new();
    }    
}
