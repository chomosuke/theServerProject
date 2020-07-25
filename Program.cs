using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;

namespace theServerProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            createPage("", "<html><head><title>Richard server -- port 5000</title></head>" +
                    "<body>Welcome to the <strong>Richard server</strong> -- <em>port 5000!</em>" +
                    "<p>remember i told you i'm building my own server?</p>" +
                    "<a href=\"http://118.92.5.160:5000/richardtube/\">Visit this unfinished RichardTube with nothing in it</a>" +
                    "<img src=\"image.jpg\"/>" +
                    "</body></html>");
            createPage("richardtube/", "<html><head><title>RTube</title></head>" +
                    "<body><strong>RichardTube</strong>" +
                    "<p>some videos will be here</p></body></html>");
            Console.WriteLine("Server started.");
            Console.ReadLine();
        }
        static void createPage(string subdir, string response)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://192.168.1.240:5000/" + subdir); // add prefix "http://192.168.1.240:5000/" and subdir
            listener.Start(); // start server (Run application as Administrator!)
            object[] state = new object[2];
            state[0] = listener;
            state[1] = response;
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
            if (context.Request.Url.Equals(new Uri("http://118.92.5.160:5000/image.jpg")))
            {
                byte[] _responseArray;
                Image img = Image.FromFile(@"C:/Users/a1332/Pictures/Screenshots/Screenshot (4).png");
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    _responseArray = ms.ToArray();
                }
                context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                context.Response.KeepAlive = false; // set the KeepAlive bool to false
            }
            else
            {
                string response = (string)state[1];
                byte[] _responseArray = Encoding.UTF8.GetBytes(response); // get the bytes to response
                context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                context.Response.KeepAlive = false; // set the KeepAlive bool to false
            }
            Console.WriteLine("Request Responded: " + context.Request.Url);
            context.Response.Close(); // close the connection

        }


    }
}