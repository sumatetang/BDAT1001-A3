using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FtpApp.Models.Utilities
{
    public class FTP
    {
        //Code goes here

        /// <summary>
        /// Converts the contents of the file to a byte array
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static byte[] GetStreamBytes(string filePath)
        {
            using (StreamReader sourceStream = new StreamReader(filePath))
            {
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                return fileContents;
            }
        }

        /// <summary>
        /// Convert a Stream to a byte array
        /// </summary>
        /// <param name="stream">A Stream Object</param>
        /// <returns>Array of bytes from the Stream</returns>
        public static byte[] ToByteArray(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] chunk = new byte[1024];
                int bytesRead;
                while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
                {
                    ms.Write(chunk, 0, bytesRead);
                }

                return ms.ToArray();
            }
        }

        public static List<string> GetDirectory(string url, string username = Constants.FTP.UserName, string password = Constants.FTP.Password)
        {
            List<string> output = new List<string>();

            //Build the WebRequest
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);

            request.Credentials = new NetworkCredential(username, password);

            request.Method = WebRequestMethods.Ftp.ListDirectory;
            //request.EnableSsl = false;
            request.KeepAlive = false;

            //Connect to the FTP server and prepare a Response
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                //Get a reference to the Response stream
                using (Stream responseStream = response.GetResponseStream())
                {
                    //Read the Response stream
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        //Retrieve the entire contents of the Response
                        string responseString = reader.ReadToEnd();

                        //Split the response by Carriage Return and Line Feed character to return a list of directories
                        output = responseString.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();

                        //Close the StreamReader
                        reader.Close();
                    }

                    //Close the Stream
                    responseStream.Close();
                }

                //Close the FtpWebResponse
                response.Close();
                Console.WriteLine($"Directory List Complete with status code: {response.StatusDescription}");
            }

            return (output);
        }

        internal static bool FileExists(object remoteFileUrl)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Downloads a file from an FTP site
        /// </summary>
        /// <param name="sourceFileUrl">Remote file Url</param>
        /// <param name="destinationFilePath">Destination local file path</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Result of file download</returns>
        public static string DownloadFile(string sourceFileUrl, string destinationFilePath, string username = Constants.FTP.UserName, string password = Constants.FTP.Password)
        {
            string output;

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sourceFileUrl);

            //Specify the method of transaction
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            //Indicate Binary so that any file type can be downloaded
            request.UseBinary = true;
            request.KeepAlive = false;

            try
            {
                //Create an instance of a Response object
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    //Request a Response from the server
                    using (Stream stream = response.GetResponseStream())
                    {
                        //Build a variable to hold the data using a size of 1Mb or 1024 bytes
                        byte[] buffer = new byte[1024]; //1 Mb chucks

                        //Establish a file stream to collect data from the response
                        using (FileStream fs = new FileStream(destinationFilePath, FileMode.Create))
                        {
                            //Read data from the stream at the rate of the size of the buffer
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);

                            //Loop until the stream data is complete
                            while (bytesRead > 0)
                            {
                                //Write the data to the file
                                fs.Write(buffer, 0, bytesRead);

                                //Read data from the stream at the rate of the size of the buffer
                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                            }

                            //Close the StreamReader
                            fs.Close();
                        }

                        //Close the Stream
                        stream.Close();
                    }

                    //Close the FtpWebResponse
                    response.Close();

                    //Output the results to the return string
                    output = $"Download Complete, status {response.StatusDescription}";
                }

            }
            catch (WebException e)
            {
                FtpWebResponse response = (FtpWebResponse)e.Response;
                //Something went wrong with the Web Request
                output = e.Message + $"\n Exited with status code {response.StatusCode}";
            }
            catch (Exception e)
            {
                //Something went wrong
                output = e.Message;
            }

            //Return the output of the Responce
            return (output);
        }

        /// <summary>
        /// Uploads a file to an FTP site
        /// </summary>
        /// <param name="sourceFilePath">Local file</param>
        /// <param name="destinationFileUrl">Destination Url</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string UploadFile(string sourceFilePath, string destinationFileUrl, string username = Constants.FTP.UserName, string password = Constants.FTP.Password)
        {
            string output;

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(destinationFileUrl);

            request.Method = WebRequestMethods.Ftp.UploadFile;

            //Close the connection after the request has completed
            request.KeepAlive = false;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            // Copy the contents of the file to the request stream.
            byte[] fileContents = GetStreamBytes(sourceFilePath);

            //Get the length or size of the file
            request.ContentLength = fileContents.Length;

            //Write the file to the stream on the server
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
            }

            //Send the request
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                output = $"Upload File Complete, status {response.StatusDescription}";

                response.Close();
            }

            return (output);
        }


        /// <summary>
        /// Tests to determine whether a file exists on an FTP site
        /// </summary>
        /// <param name="
        /// 
        /// Url"></param>
        /// <param name="username"></param>
        /// 
        /// 
        /// 
        /// 
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool FileExists(string remoteFileUrl, string username = Constants.FTP.UserName, string password = Constants.FTP.Password)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteFileUrl);

            //Specify the method of transaction
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.KeepAlive = false;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            try
            {
                //Create an instance of a Response object
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //Close the FtpWebResponse
                response.Close();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode ==
                    FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Retreives the contents of a file from an FTP site into an in-memory byte array
        /// </summary>
        /// <param name="sourceFileUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] DownloadFileBytes(string sourceFileUrl, string username = Constants.FTP.UserName, string password = Constants.FTP.Password)
        {
            byte[] output;

            try
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sourceFileUrl);

                //Specify the method of transaction
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(username, password);

                //Create an instance of a Response object
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    //Request a Response from the server
                    output = ToByteArray(response.GetResponseStream());

                    response.Close();

                    //Return the output of the Response
                    return output;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return new byte[0];
        }


        /// <summary>
        /// Deletes the directory at the given Url
        /// </summary>
        /// <param name="url">Url to directory to delete</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool DeleteFtpDirectory(string url, string username = Constants.FTP.UserName, string password = Constants.FTP.Password)
        {
            Console.WriteLine($"Be careful! Are you sure you want to delete the directory, Y for Yes, N for No\n{url}?");
            var deleteConfirm = Console.ReadLine().ToUpper();

            if (deleteConfirm == "N")
            {
                //Exit out, do not delete
                return false;
            }

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                request.KeepAlive = false;
                request.Credentials = new NetworkCredential(username, password);
                request.GetResponse().Close();
            }
            catch (WebException e)
            {
                //Something went wrong
                return false;
            }
            return true;
        }


        /// <summary>
        /// Rename a directory at the given Url
        /// </summary>
        /// <param name="url">Url to directory to rename</param>
        /// <param name="newDirectoryName">The new directory name (not the full path)</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true if successful</returns>
        public static bool RenameFtpDirectory(string url, string newDirectoryName, string username = Constants.FTP.UserName, string password = Constants.FTP.Password)
        {
            Console.WriteLine($"Be careful! Are you sure you want to rename the directory, Y for Yes, N for No\n{url}\nto\n{newDirectoryName}?");
            var deleteConfirm = Console.ReadLine().ToUpper();

            if (deleteConfirm == "N")
            {
                //Exit out, do not delete
                return false;
            }

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.Rename;
                request.KeepAlive = false;
                request.Credentials = new NetworkCredential(username, password);
                request.RenameTo = newDirectoryName;
                request.GetResponse().Close();
            }
            catch (WebException e)
            {
                //Something went wrong
                return false;
            }
            return true;
        }

        public static List<string> GetFileInDirectory(string url, string username = Constants.FTP.UserName, string password = Constants.FTP.Password)
        {
            List<string> output = new List<string>();

            //Build the WebRequest
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);

            request.Credentials = new NetworkCredential(username, password);

            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            //request.EnableSsl = false;
            request.KeepAlive = false;

            //Connect to the FTP server and prepare a Response
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                //Get a reference to the Response stream
                using (Stream responseStream = response.GetResponseStream())
                {
                    //Read the Response stream
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        //Retrieve the entire contents of the Response
                        string responseString = reader.ReadToEnd();

                        //Split the response by Carriage Return and Line Feed character to return a list of directories
                        output = responseString.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();

                        //Close the StreamReader
                        reader.Close();
                    }

                    //Close the Stream
                    responseStream.Close();
                }

                //Close the FtpWebResponse
                response.Close();
                Console.WriteLine($"Directory List Complete with status code: {response.StatusDescription}");
            }

            output = output
                .Select(s => s.Substring(39, s.Length - 39))
                    //.Where(s => s.Contains("Carolyn Knight"))
                    .Take(2)
                    .ToList();
            return (output);
        }
    }
}
