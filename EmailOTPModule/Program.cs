using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace EmailOTPModule
{

    internal class Program
    {
        public static string getEmailPassword()
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(System.Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();

            // Retrieve the API key from configuration
            string emailPassword = configuration["ApiSettings:EMAIL_PASSWORD"];

            if (string.IsNullOrEmpty(emailPassword))
            {
                Console.WriteLine("API key is missing. Please set the API_KEY environment variable.");
                return emailPassword;
            }

            return emailPassword;
        }

        static void Main(string[] args)
        {
            string otpInput = "";

            Email_OTP_Module emailOTP = new Email_OTP_Module();
            Email_OTP_Module.start(getEmailPassword());

            Console.WriteLine("Enter Email: ");
            string email = Console.ReadLine();
           
            string emailStatus = emailOTP.generate_OTP_email(email);
            //Console.WriteLine(emailStatus);

            if (emailStatus == Status_Code.STATUS_EMAIL_FAIL)
            {
                Console.WriteLine(emailStatus);
                Console.ReadKey();
            }

            while (emailStatus == Status_Code.STATUS_EMAIL_INVALID)
            {
                Console.WriteLine(emailStatus);
                Console.WriteLine("Enter Email: ");
                email = Console.ReadLine();
                emailStatus = emailOTP.generate_OTP_email(email);
            }

            Console.WriteLine("Enter OTP: ");
            otpInput = Console.ReadLine();
            string statusCode = emailOTP.check_OTP(otpInput);
            Console.WriteLine(statusCode);

            while (statusCode == Status_Code.STATUS_OTP_AUTHENTICATION_FAIL)
            {
                Console.WriteLine("Enter OTP: ");
                otpInput = Console.ReadLine();
                statusCode = emailOTP.check_OTP(otpInput);
                Console.WriteLine(statusCode);

                if (statusCode == Status_Code.STATUS_OTP_OK)
                {
                    //Console.WriteLine(statusCode);
                    break;
                }

            };
            Email_OTP_Module.close();
            Console.ReadKey();
        }

        public static void RestartApplication()
        {
            // Get the path to the current executable
            string exePath = Assembly.GetExecutingAssembly().Location;

            // Start a new instance of the application
            Process.Start(new ProcessStartInfo(exePath)
            {
                UseShellExecute = true
            });

            // Close the current instance
            Environment.Exit(0);
        }

    }
}
