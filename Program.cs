using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace theServerProject
{
    class Program
    {
        static readonly string superDir = @"C:\Users\a1332\Desktop\theServerProject\src";
        static readonly string superUrl = "http://192.168.1.240:5000/";
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            List<String> dirs = DirSearch(superDir);
            foreach (string d in dirs)
            {
                Console.WriteLine((d.Substring(superDir.Length + 1)
                    .Replace("\\", "/")
                    .Replace(".html", "") + "/")
                    .Replace("index/", ""));
                createPage(
                    (d.Substring(superDir.Length + 1)
                    .Replace("\\", "/")
                    .Replace(".html", "") + "/")
                    .Replace("index/", ""), d);
            }
            Console.WriteLine("Server started.");
            Console.ReadLine();
        }

        private static List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }

            return files;
        }

        static void createPage(string subdir, string responseFilePath)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(superUrl + subdir); // add prefix "http://192.168.1.240:5000/" and subdir
            listener.Start(); // start server (Run application as Administrator!)
            object[] state = new object[2];
            state[0] = listener;
            state[1] = responseFilePath;
            listener.BeginGetContext(new AsyncCallback(OnContext), state);
        }
        private static void OnContext(IAsyncResult result)
        {
            object[] state = (object[])result.AsyncState;
            HttpListener listener = (HttpListener)state[0];
            HttpListenerContext context = listener.EndGetContext(result); // get a context
            // Now, you'll find the request URL in context.Request.Url

            // start getting the next one immediately
            listener.BeginGetContext(OnContext, state);

            string responseFilePath = (string)state[1];
            byte[] _responseArray = File.ReadAllBytes(responseFilePath); // get the bytes to response
            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
            context.Response.KeepAlive = false; // set the KeepAlive bool to false
            Console.WriteLine("Request Responded: " + context.Request.Url);
            context.Response.Close(); // close the connection
        }
    }
}