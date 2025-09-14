using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eva_DAL.DTOs.Language
{
    public class LanguageDTO
    {
        [Required(ErrorMessage = "Language is required")]
        [MaxLength(20,ErrorMessage ="Maxlength is 20")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Accept only letters")]
        public string? Lang { get; set; }

        [Required(ErrorMessage = "Language Short Code is required")]
        [MaxLength(3, ErrorMessage = "Language short code must be like en / ar")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Accept only letters")]
        public string? LangShortCode { get; set; }

        [Required(ErrorMessage = "Direction is required")]
        [MaxLength(3, ErrorMessage = "Dirction Must be like ltr/rtl")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Accept only letters")]
        public string? Direction { get; set; }
    }

    public class UpdateLanguageDTO
    {

        [MaxLength(20, ErrorMessage = "Maxlength is 20")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Accept only letters")]
        public string? Lang { get; set; }

        [MaxLength(3, ErrorMessage = "Language short code must be like en / ar")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Accept only letters")]
        public string LangShortCode { get; set; }

        [MaxLength(3, ErrorMessage = "Dirction Must be like ltr/rtl")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Accept only letters")]
        public string Direction { get; set; }
    }

    public class ShowLanguageDTO
    {
        public int Id { get; set; }
        public string Lang { get; set; }
        public string LangShortCode { get; set; }
        public string Direction { get; set; }
    }
}
