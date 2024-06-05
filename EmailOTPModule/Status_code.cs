using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EmailOTPModule
{
    public static class Status_Code
    {
        /*public const int STATUS_EMAIL_OK = 200;
        public const int STATUS_EMAIL_FAIL = 500;
        public const int STATUS_EMAIL_INVALID = 400;
        public const int STATUS_OTP_OK = 200;
        public const int STATUS_OTP_FAIL = 500;
        public const int STATUS_OTP_AUTHENTICATION_FAIL = 401;
        public const int STATUS_OTP_TIMEOUT = 408;*/


        public const string STATUS_EMAIL_OK = "email containing OTP has been sent successfully.";
        public const string STATUS_EMAIL_FAIL = "email address does not exist or sending to the email has failed.";
        public const string STATUS_EMAIL_INVALID = "email address is invalid.";
        public const string STATUS_OTP_OK = "OTP is valid and checked";
        public const string STATUS_OTP_FAIL = "OTP is wrong after 10 tries";
        public const string STATUS_OTP_AUTHENTICATION_FAIL = "otp authentication failed";
        public const string STATUS_OTP_TIMEOUT = "timeout after 1 min";
    }
}
