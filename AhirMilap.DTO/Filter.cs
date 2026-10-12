using System.ComponentModel.DataAnnotations;

namespace AhirMilap.DTO
{
    public class Filter
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int? PageIndex { get; set; } = 1;

        [Required]
        [Range(1, int.MaxValue)]
        public int? PageSize { get; set; } = 25;
        public string? Search { get; set; } = string.Empty;
        //public string? SortColumn { get; set; }
        //public string? SortOrder { get; set; }
        public int? MinAge { get; set; } = 0;
        public int? MaxAge { get; set; } = 100;
        public decimal? MinHeight { get; set; } = 0;
        public decimal? MaxHeight { get; set; } = 500;
        public string? MaritalStatus { get; set; } = string.Empty;
        public string? Education { get; set; } = string.Empty;
        public string? Profession { get; set; } = string.Empty;
        public decimal? MinIncome { get; set; } = 0;
        public decimal? MaxIncome { get; set; } = 99999999999;
        public string? Location { get; set; } = string.Empty;
        public int? FilterId { get; set; } = 0;

    }

    public class CommonFilter
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; } = 1;

        [Required]
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 25;
    }
}
