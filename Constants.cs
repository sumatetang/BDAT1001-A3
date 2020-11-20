using System;
using System.Collections.Generic;
using System.Text;


namespace FtpApp.Models
{
    public class Constants
    {
        public class FTP
        {
            public const string UserName = @"bdat100119f\bdat1001";
            public const string Password = "bdat1001";

            public const string BaseUrl = "ftp://waws-prod-dm1-127.ftp.azurewebsites.windows.net/bdat1001-10983";

            public const int OperationPauseTime = 10000;
        }

        public static Student Student { get; internal set; }

        public class Credentials
        {
            public const string UserName = "<yourusername>";
            public const string Password = "<yourpassword>";
        }

        public class Urls
        {
            public const string BaseUrl = "https://webapibasicsstudenttracker.azurewebsites.net";

            public static readonly string BaseUrlApi = $"{BaseUrl}/api";

            public const string GetStudentsUnsecure = "/students";
            public const string GetStudentsSecure = "/securestudents";

            public const string PostToken = "/Tokens";

        }

        public class HTTP
        {
            public const int Timeout = 60000; //60 seconds, a long time if needed
            public const string ContentType = "application/json";

            public class Security
            {
                public const string AuthHeader = "Authorization";
                public const string AuthMethod = "Bearer";
            }
        }
    }
}

