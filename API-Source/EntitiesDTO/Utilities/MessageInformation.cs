using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDTO.Utilities {
    public static class OTPMessage {
        public static string Sent = "OTP sent successfully via SMS and email !";
        public static string SentError = "Could not send the OTP !";
        public static string MobileExists = "Mobile number already exists !";
        public static string EmailExists = "Email already exists !";
        public static string DateError = "Invalid date !";
        public static string DateRangeError = "Invalid date range !";
        public static string CodeError = "Invalid code !";
        public static string SMSReSent = "OTP resend sent successfully !";
        public static string OTPVerifiedError = "OTP verification failed!";
        public static string OTPVerified = "Verified Successfully!";
        public static string SMSOTPVerifiedError = "SMS OTP verification failed!";
        public static string EmailOTPVerifiedError = "Email OTP verification failed!";
        public static string SMSOTPSent = "OTP sent successfully !";
        public static string MobileNotExists = "Mobile number not exists !";

    }
    public static class PlaceMessage
    {
        public static string District = "District list fetched Successful !!!";   
        public static string State = "State list fetched Successful !!!";   
        public static string Courier = "Courier charge fetched Successful !!!";   
        public static string CourierUpdate = "Courier charge updated Successful !!!";   
        public static string IdCard = "ID Card charge fetched Successful !!!";   
        public static string IdCardUpdate = "ID Card charge updated Successful !!!";   
        public static string ChargeUpdate = "Charge updated Successful !!!";   
    }
}
