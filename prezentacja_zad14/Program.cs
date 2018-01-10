using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace prezentacja_zad14
{
    class Program
    {
        public static async Task<XmlDocument> Zadanie3(string address)
        {
            WebClient webClient = new WebClient();
            string xmlContent = await webClient.DownloadStringTaskAsync(new Uri(address));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            return doc;
        }

        static void Main(string[] args)
        {
            byte[] buffer = new byte[128];
            Task b = Zadanie3("http://www.feedforall.com/sample.xml");
            Console.WriteLine(Zadanie3("http://www.feedforall.com/sample.xml").Result.InnerText);
            b.Wait();
            Console.WriteLine("end main");
        }
    }
}
