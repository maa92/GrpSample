using System.ComponentModel.DataAnnotations;

namespace GRP.Models.Security
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "يجب ادخال اسم المستخدم")]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "يجب ادخال كلمة المرور")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        public bool SaveUName { get; set; }

        public string LoginErrMsg { get; set; }

        public string SystemType { get; set; }
    }
}
