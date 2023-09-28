using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace L1_A
{
    class Program
    {
        const string read1 = @"C:\Users\foxtr\OneDrive\Desktop\All Uni softwares\concurrent code\Final Project\Rajesh_Lab1\Text1.txt";
        const string read2 = @"C:\Users\foxtr\OneDrive\Desktop\All Uni softwares\concurrent code\Final Project\Rajesh_Lab1\Text2.txt";
        const string read3 = @"C:\Users\foxtr\OneDrive\Desktop\All Uni softwares\concurrent code\Final Project\Rajesh_Lab1\Text3.txt";
        const string read4 = @"C:\Users\foxtr\OneDrive\Desktop\All Uni softwares\concurrent code\Final Project\Rajesh_Lab1\Text4.txt";
        const string read5 = @"C:\Users\foxtr\OneDrive\Desktop\All Uni softwares\concurrent code\Final Project\Rajesh_Lab1\Text5.txt";
        const string read6 = @"C:\Users\foxtr\OneDrive\Desktop\All Uni softwares\concurrent code\Final Project\Rajesh_Lab1\Text6.txt";
        const string read7 = @"C:\Users\foxtr\OneDrive\Desktop\All Uni softwares\concurrent code\Final Project\Rajesh_Lab1\Text7.txt";
        const string read8 = @"C:\Users\foxtr\OneDrive\Desktop\All Uni softwares\concurrent code\Final Project\Rajesh_Lab1\Text8.txt";

        static void Main(string[] args)
        {
            string website_1="https://moodle.ktu.edu/course/view.php?id=6837";
            string website_2="https://www.fighterpilotpodcast.com/glossary/";
            string website_3="https://learn.microsoft.com/en-us/visualstudio";
            string website_4="https://www.tunefind.com/";
            string website_5="https://www.britannica.com/technology/concurrent-programming";
            string website_6="https://wiki.esn.org/pages/viewpage.action?pageId=7045162";
            string website_7="https://combatace.com/files/file/9401-super-hornet-package-for-sf2-v41/";
            string website_8="https://wiki.esn.org/display/WEL/Welcome+to+ESN+Wiki+-+Guide+for+beginners";

            // Create a list of file paths
            List<string> filePaths = new List<string>(){ read1, read2, read3 , read4, read5 , read6, read7, read8 };

            // Create a list of websites
            List<string> websites = new List<string>() { website_1, website_2, website_3, website_4, website_5, website_6, website_7, website_8 };
            
            // Download the text from each website and save it to the corresponding text file
            for (int i = 0; i < websites.Count; i++)
            {
                string text = DownloadText(websites[i]);
                SaveTextToFile(text, filePaths[i]);
            }
            
            var timer = new Stopwatch();
            timer.Start();

            // Read the text from each file and count the number of vowels
            int numThreads = 5;
            ParallelEnumerable.Range(0, filePaths.Count).WithDegreeOfParallelism(numThreads).ForAll(i => {
                string text = ReadTextFromFile(filePaths[i]); int vowelCount = text.Count(c => "aeiouAEIOU".Contains(c));
                Console.WriteLine($"File {i + 1}: {vowelCount} vowels");
            });
            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(foo);

        }
        
        // Function to download the text from a website
        static string DownloadText(string website)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(website);
            }
        }
        
        // Function to save text to a file
        static void SaveTextToFile(string text, string filePath)
        {
            File.WriteAllText(filePath, text);
        }
        
        // Function to read text from a file
        static string ReadTextFromFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
        
        // Function to count the number of vowels in a string
        static int CountVowels(string text)
        {
            return text.Count(c => "aeiouAEIOU".Contains(c));
        }
    }
}
