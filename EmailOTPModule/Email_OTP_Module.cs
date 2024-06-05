using System;
using System.Net.Mail;
using System.Net;
using Timer = System.Timers.Timer;
using System.Timers;
using System.Runtime.CompilerServices;
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Serialization.Formatters;

namespace EmailOTPModule
{
    class Email_OTP_Module
    {
        private static SmtpClient MyServer;

        private static string emailFrom { get; set; }

        private static string otp { get; set; }

        private static int timeoutinseconds = 60;

        private static Timer timer;

        private static string domain { get; set; }

        private static int nooftries { get; set; }

        private static readonly object lockObject = new object();

        /* start can be called after an  of Email OTP is constructed. Can be used to initialise variables.
         */
        public static void start(string emailPassword)
        {

            //optional to implement
            domain = "dso.org.sg";
            emailFrom = "emailotpmodule@gmail.com";
            nooftries = 0;

            MyServer = new SmtpClient();
            MyServer.Host = "smtp.gmail.com";
            MyServer.Port = 587;
            MyServer.EnableSsl = true;
            MyServer.UseDefaultCredentials = true;
            MyServer.Credentials = new NetworkCredential(emailFrom, emailPassword);

        }

        /* close can be called after an instance of Email OTP is to be remove from the application.
         */
        public static void close()
        {

            //optional to implement
            lock (lockObject)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
                otp = null;
                nooftries = 0;
            }
        }

        /*
        @func generate_OTP_email sends a new 6 digit random OTP code to the given email address input by the users. Only emails from the ".dso.org.sg" domain should be allowed to receive an OTP code.
        You can assume a function send_email(email_address, email_body) is implemented. 
        Email body to the user should be in this format "You OTP Code is 123456. The code is valid for 1 minute"

        @param user_email is an email address entered by the user. 

        @returns the following status code (assume implemented as constants)
        STATUS_EMAIL_OK: email containing OTP has been sent successfully.
        STATUS_EMAIL_FAIL: email address does not exist or sending to the email has failed.
        STATUS_EMAIL_INVALID: email address is invalid.
        */

        public static void setTimer()
        {
            // Create a 1 min timer 
            timer = new Timer(timeoutinseconds * 1000);

            // Hook up the Elapsed event for the timer.
            timer.Elapsed += OnOtpExpired;
            timer.Enabled = false;
            timer.Start();
        }

        private static void OnOtpExpired(object source, ElapsedEventArgs e)
        {
            // do stuff
            lock (lockObject)
            {
                timer.Stop(); // Stop the timer
                Console.WriteLine("OTP has expired.");
                otp = null;
                Console.WriteLine(Status_Code.STATUS_OTP_TIMEOUT);
                close();
                Environment.Exit(0);
            }
        }


        public string generate_OTP_email(string user_email)
        {
            //implement me
            lock (lockObject)
            {
                if (!IsValid(user_email))
                {
                    return Status_Code.STATUS_EMAIL_INVALID;
                }

                string Email_domain = user_email.Split('@')[1];

                if (domain.Equals(Email_domain))
                {
                    Random rnd = new Random();
                    otp = rnd.Next(0, 1000000).ToString("D6");
                    try
                    {
                        send_email(user_email, otp);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return Status_Code.STATUS_EMAIL_FAIL;
                    }
                }
                else
                {
                    return Status_Code.STATUS_EMAIL_INVALID;
                }
                setTimer();
                return Status_Code.STATUS_EMAIL_OK;
            }
        }

        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public void send_email(string user_email, string otp)
        {
            MailAddress from = new MailAddress(emailFrom, "OTP Module");
            MailAddress receiver = new MailAddress(user_email.Trim(), "Name want to display");
            MailMessage Mymessage = new MailMessage(from, receiver);
            Mymessage.Subject = "OTP Generator";
            Mymessage.Body = "You OTP Code is " + otp + ". The code is valid for 1 minute";

            //sends the email
            MyServer.Send(Mymessage);
        }

        /*
        @func check_OTP reads the input stream for user input of the OTP. The OTP to match is the current OTP generated by a send
        allows user 10 tries to enter the valid OTP. check_OTP should return after 1min if the user does not give a valid OTP. 

        @param input is a generic IOstream. It implements input.rea2dOTP() which waits and returns the 6 digit entered by the user. this function call is blocking so you might need to wrap it in a timeout.

        @returns the following status code (assume implemented as constants)
        STATUS_OTP_OK: OTP is valid and checked
        STATUS_OTP_FAIL: OTP is wrong after 10 tries
        STATUS_OTP_TIMEOUT: timeout after 1 min
        */
        public string check_OTP(string input)
        {
            //implement me
            lock (lockObject)
            {
                if (otp == null)
                {
                    return Status_Code.STATUS_OTP_AUTHENTICATION_FAIL;
                }

                if (nooftries < 10)
                {
                    if (otp.Equals(input))
                    {
                        return Status_Code.STATUS_OTP_OK;
                    }

                    nooftries++;

                    if (nooftries >= 10)
                    {
                        return Status_Code.STATUS_OTP_FAIL;
                    }

                    return Status_Code.STATUS_OTP_AUTHENTICATION_FAIL;
                }
                else
                {
                    return Status_Code.STATUS_OTP_FAIL;
                }
            }
        }
    }
}
