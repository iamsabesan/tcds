using Realtor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDTO.OTP
{
    public class RegistrationOTPRequest
    {
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} number should be {1} digits")]
        public string Phone { get; set; } = string.Empty;
        [Required]
        [StringLength(7, ErrorMessage = "{0} code should not exceed {1} digits")]
        public string PhoneCode { get; set; } = string.Empty;
    }
    public class RegistrationOTPResult
    {
        public Guid OTPTransID { get; set; }
        public int ValidityTime { get; set; }
    }
    public class SMSResendRequest
    {
        [Required]
        [StringLength(7, ErrorMessage = "{0} code should not exceed {1} digits")]
        public string PhoneCode { get; set; } = string.Empty;
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} number should be {1} digits")]
        public string Phone { get; set; } = string.Empty;
    }
    public class OTPResult
    {
        public Guid OTPTransID { get; set; }
        public int ValidityTime { get; set; }
    }
    public class OTPVerify
    {
        [IsGuidEmpty]
        public Guid SMSOTPTransID { get; set; }
        [Required]
        public string SMSOTPCode { get; set; } = string.Empty;

    }
    public class OTPVerifyResult
    {
        public bool IsOTPVerified { get; set; }
        public bool IsMobileVerified { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Message { get; set; } = string.Empty;
    }
    public class OTPRequest
    {
        public string Phone { get; set; } = string.Empty;
        public string OTPType { get; set; } = string.Empty;
        public List<string> MessageParameter { get; set; } = new List<string>();
    }
    public class OTPResponse
    {
        public Guid OTPTransID { get; set; }
        public Guid MappingID { get; set; }
        public int ValidityTime { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string OTPCode { get; set; } = string.Empty;
    }
    public class VerifyOtp
    {
        public Guid OTPTransID { get; set; }
        public string OTPCode { get; set; } = string.Empty;
        public string OTPType { get; set; } = string.Empty;
    }
    public class IsMobileCheck
    {
        public string Mobile { get; set; } = string.Empty;
    }
}
