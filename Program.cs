using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace theServerProject
{
    class Program
    {
        static readonly string superDir = @"C:\Users\a1332\Desktop\theServerProject\src";
        static readonly string superUrl = "http://" + GetLocalIPAddress() + ":5000/";
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...\nhome URL: " + superUrl);
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(superUrl); // add prefix "http://192.168.1.240:5000/" and subdir
            listener.Start(); // start server (Run application as Administrator!)
            listener.BeginGetContext(new AsyncCallback(OnContext), listener);
            Console.WriteLine("Server started.");
            Console.ReadLine();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        //Generate a public/private key pair.  
        static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        //Save the public key information to an RSAParameters structure.  
        static RSAParameters rsaKeyInfo = rsa.ExportParameters(false);
        private static void OnContext(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result); // get a context
            // Now, you'll find the request URL in context.Request.Url

            // start getting the next one immediately
            listener.BeginGetContext(OnContext, result.AsyncState);

            string url = context.Request.Url.ToString();
            byte[] _responseArray = null; // the bytes to response
            string subUrl = url.Substring(url.IndexOf(":5000") + 5);
            switch (subUrl)
            {
                case "/":
                    _responseArray = AttemptReadAllBytes(superDir + "\\index.html");
                    break;

                case "/chatbox/gib public key":
                    _responseArray = rsaKeyInfo.Modulus;
                    break;

                case "/chatbox/login request":
                    
                    break;

                default:
                    string path = superDir + subUrl;
                    path = path.Replace('/', '\\');
                    while (path.EndsWith("\\")) // ignore all \ at the end
                        path = path.Substring(0, path.Length - 1);

                    _responseArray = AttemptReadAllBytes(path);
                    if (_responseArray != null)
                        break;

                    path += ".html";
                    _responseArray = AttemptReadAllBytes(path);
                    if (_responseArray != null)
                        break;

                    context.Response.StatusCode = 404; // 404 not found
                    _responseArray = AttemptReadAllBytes(superDir + "\\404NotFound.html");
                    break;
            }
            try {
                context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length); // write bytes to the output stream
                context.Response.KeepAlive = false; // set the KeepAlive bool to false
            } catch (HttpListenerException) {
                Console.WriteLine("connection aborted");
            } finally {
                context.Response.Close(); // close the connection
            }
            Console.WriteLine("Request Responded: " + context.Request.Url + " " + "at: " + DateTime.Now);
        }

        private static byte[] AttemptReadAllBytes(string path)
        {
            try {
                return File.ReadAllBytes(path);
            } catch (Exception e) {
                // Console.WriteLine(e);
                return null;
            }
        }
    }
}