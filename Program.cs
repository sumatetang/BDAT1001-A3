using FtpApp.Models;
using FtpApp.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FtpApp
{

    class Program
    {
        static string cache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BDAT1001-10983");


        static void Main(string[] args)
        {
            // Create cache folder if not exists.
            if (!Directory.Exists(cache))
            {
                Directory.CreateDirectory(cache);
            }

            // 1. Output the directories to the Console
            List<string> directories = OutputDirectories();

            // 2. Extract the data from your directory
            ExtractMyData();

            // 4. Retrieve the Student information programmatically from each directory
            List<Student> students = GetAllStudents(directories);

            // 5.1
            if (students.Count > 0)
            {
                Console.WriteLine($"Found {students.Count} students");
                foreach (var student1 in students.OrderBy(x => x.LastName))
                {
                    Console.WriteLine(student1);
                }
            }

            // TODO: Output the information to the Console using the modified ToCSV() function

            // StartsWith "a"
            IEnumerable<Student> filtedStudents = students.Where(s => s.FirstName.StartsWith("a"));
            foreach (var student1 in filtedStudents)
            {
                Console.WriteLine(student1);
            }

            // Contains "a"
            // TODO: Recheck the purpose.
            filtedStudents = students.Where(s => s.FirstName.Contains("a"));
            foreach (var student1 in filtedStudents)
            {
                Console.WriteLine(student1);
            }

            // Find
            // TODO: Recheck the purpose.
            Student foundMe = students.Find(s => s.StudentId == "200471292");
            Console.WriteLine(foundMe);

            // SingleOrDefault
            foundMe = students.SingleOrDefault(s => s.StudentId == "200471292");
            if (foundMe != null)
            {
                Console.WriteLine(foundMe);
            }

            // Average
            double averageAge = students.Average(x => x.Age);
            Console.WriteLine($"The average age is => {averageAge.ToString("0")}");


            // Highest
            int highestMax = students.Max(x => x.Age);
            Console.WriteLine($"The highest Age in the list is {highestMax}");


            // Lowest
            int lowestMax = students.Min(x => x.Age);
            Console.WriteLine($"The lowest Age in the list is {lowestMax}");

            //5.2 Output the contents of the List object to a CSV file.
            var outputPath = Path.Combine(cache, "students.csv");
            using (System.IO.StreamWriter file=new System.IO.StreamWriter(outputPath))
            {
                file.WriteLine("StudentId,FirstName,LastName,MyRecord");
                foreach (Student student in students)
                {
                    file.WriteLine(student.ToCSV());
                }
            }


            // 5.3 Output the contents of the List object to a JSON file
            var studentsListJson = JsonConvert.SerializeObject(students);
            var outputPathJson = Path.Combine(cache, "students.json");
            System.IO.File.WriteAllText(outputPathJson, studentsListJson);
            Console.WriteLine($"Creat students.json");
          

            //5.4 Output the contents of the List object to an XML file
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(students.GetType());
            TextWriter writer = new StreamWriter($"{cache}\\students.xml");
            x.Serialize(writer, students);
            Console.WriteLine($"Creat students.xml");
        }

       

        private static List<Student> GetAllStudents(List<string> directories)
        {
            List<Student> students = new List<Student>();
            foreach (var directory in directories)
            {
                // 4.1 Add each student into a List object programmatically as a Student Model from 3.1.a
                var student = CreateStudentFromRemote(directory, out _);
                students.Add(student);

                // 4.2 Output the information to the Console using the modified ToString() function
                Console.WriteLine(student.ToString());

                // 4.3 Output the information to the Console using the modified ToCSV() function
                Console.WriteLine(student.ToCSV());
            }

            return students;
        }

        static void ExtractMyData()
        {
            // 2.2 Extract the data from your directory
            var myName = "200471292 Danhong Tang";
            var myDirectory = $"{Constants.FTP.BaseUrl}/{myName}";
            var files = FTP.GetDirectory(myDirectory);

            Console.WriteLine("Files in my directory:");
            foreach (var file in files)
            {
                Console.WriteLine($"\t{file}");
            }

            // 2.3 Extract the data from these files
            string[] lines;
            var student = CreateStudentFromRemote(myName, out lines);

            // If you have not already done so, output your myimage.jpg file as Base64 to a new column named ImageData in your info.csv file
            if (student.ImageData == null)
            {
                // No image yet.
                //Build a image file path from the original image path
                FileInfo fileinfo = new FileInfo($"{cache}\\{myName}\\myimage.jpg");

                //Provide an Image from a file on your Desktop
                Image image = Image.FromFile(fileinfo.FullName);

                //Convert Image to Base64 encoded text
                string base64image = Converter.ImageToBase64(image, ImageFormat.Jpeg);
                student.ImageData = base64image;

                // Save CSV file
                var csvHead = lines[0] + ", ImageData";
                var csvData = student.ToCSV();
                StreamWriter sw = new StreamWriter($"{cache}\\info.csv");
                sw.WriteLine(csvHead);
                sw.WriteLine(csvData);
                sw.Close();

                // Upload CSV file back to FTP server.
                var result = FTP.UploadFile($"{cache}\\info.csv", $"{myDirectory}/info.csv");
                Console.WriteLine($"Upload CSV - {result}");
            }
        }

        private static Student CreateStudentFromRemote(string name, out string[] lines)
        {
            var directory = $"{Constants.FTP.BaseUrl}/{name}";
            Console.WriteLine($"Retriving {directory}");

            // Download image to a file.
            if (!Directory.Exists($"{cache}\\{name}"))
            {
                Directory.CreateDirectory($"{cache}\\{name}");
            }
             string downloadImageResult = FTP.DownloadFile($"{directory}/myimage.jpg", $"{cache}\\{name}\\myimage.jpg"); 
              Console.WriteLine($"Download image - {downloadImageResult}");

            // Download CSV as byte array.
            var fileBytes = FTP.DownloadFileBytes($"{directory}/info.csv");

            // Convert byte array to string.
            string infoCsvData = Encoding.UTF8.GetString(fileBytes, 0, fileBytes.Length);

            // Generate student properties from CSV data.
            lines = infoCsvData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var student = new Student();
            student.FromCSV(lines[1]);
            return student;
        }

        private static List<string> OutputDirectories()
        {
            List<string> directories = FTP.GetDirectory(Constants.FTP.BaseUrl);

            // Enumerate every FTP directory.
            foreach (var directory in directories)
            {
                // Output the directories to the Console.
                Console.WriteLine(directory);
            }

            return directories;
        }


        //    foreach (var directory in directories)
        //    {
        //        try 
        //        { 
        //            Console.WriteLine(directory);
        //            //Path to the remote file you will download
        //            string remoteDownloadFilePath = "/" + directory + "/info.csv";
        //    //Path to a valid folder and the new file to be saved
        //    string localDownloadFileDestination = @"D:\备份\小温\BIG DATA\1001 information encoding\A3\info2.csv";
        //    Console.WriteLine(FTP.DownloadFile(Constants.FTP.BaseUrl + remoteDownloadFilePath, localDownloadFileDestination));
        //        }
        //        catch (Exception e)
        //{
        //    Console.WriteLine(e.Message);
        //}
        //    }

        //    class Program
        //{
        //    static void Main(string[] args)
        //    {
        //        Images.ReaderFile("D:\\备份\\小温\\BIG DATA\\1001 information encoding\\A3");

        //        string imagesOutputFolder = @"D:\备份\小温\BIG DATA\1001 information encoding\Vitual Studio\FtpApp\Content\Images";
        //        List<string> errors = new List<string>();
        //        List<Student> students = new List<Student>();
        //        List<string> directories = FTP.GetFileInDirectory(Constants.FTP.BaseUrl);

        //        foreach (var directory in directories)
        //        {
        //            Console.WriteLine(directory);

        //            Student student = new Student();
        //            student.FromDirectory(directory);

        //            students.Add(student);

        //            if (FTP.FileExists(Constants.FTP.BaseUrl + "/" + directory + "/info.csc"))
        //            {
        //                Console.WriteLine(" info.csv exists");

        //                var fileBytes = FTP.DownloadFileBytes(Constants.FTP.BaseUrl + "/" + directory + "/info.csv");
        //                string inforCsvData = Encoding.UTF8.GetString(fileBytes, 0, fileBytes.Length);

        //                string[] lines = inforCsvData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);



        //                //student.Image.Save(imagesOutputFolder + "\\" + directory + ".jpg");

        //            }
        //            else
        //            {
        //                errors.Add(directory + "_ info.csv does not exist");
        //                Console.WriteLine("info.csv does not exist");
        //            }

        //            //    if (FTP.FileExists(Contants.FTP.BaseUrl + "/" + directory + "/info.html"))
        //            //    {
        //            //        {
        //            //            Console.WriteLine("info.html exists");   
        //            //        }
        //            //    else
        //            //        {
        //            //            errors.Add(directory + "-info.html does not exist");
        //            //            Console.WriteLine("info.html does not exist");
        //            //        }

        //            //}

        //        }
        //        if (students.Count > 0)
        //        {
        //            Console.WriteLine($"Found {students.Count} students");
        //            foreach (var student in students.OrderBy(x => x.LastName))
        //            {
        //                Console.WriteLine(student);
        //            }
        //        }








        //    }
    }
}
