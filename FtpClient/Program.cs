using System;
using System.IO;
using System.Net;
using System.Text;

namespace FtpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStruct fileStruct = new FileStruct();
            Respoonse("ftp://10.7.180.107:21", fileStruct);
           Save("D:\\Save", "ftp://10.7.180.107:21",fileStruct);
          //  Console.Read();
        }

        static void Respoonse(string Addres,FileStruct FileStr)
        {
            
            string temp;
            // Console.ReadLine();
            // create FtpWebRequest
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Addres);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential("test_user", "1234567890");
            //request.EnableSsl = true; // если используется ssl

            // get answer as FtpWebResponse
            Console.WriteLine(request.GetResponse());
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            // get stream and save as file 
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                temp = line.Substring(49);
                if (line[0]=='d')
                {
                    FileStr.Direct.Add(new Direct
                    { Directory= temp,
                    NodeNext=new FileStruct()
                    });
                }
                else
                {
                    FileStr.Files.Add(temp);
                }
            }
            reader.Close();
            response.Close();
            string AddresTemp;
            for (int i = 0; i < FileStr.Direct.Count; i++)
            {
                AddresTemp = "";
                AddresTemp = Addres + "/" + FileStr.Direct[i].Directory;
                Respoonse(AddresTemp, FileStr.Direct[i].NodeNext);
            }


     
        }

        static void Save(string Addres,string AddresSave,FileStruct fileStr)
        {
            
            for (int i = 0; i < fileStr.Direct.Count; i++)
            {
                Directory.CreateDirectory(Addres + "\\"+fileStr.Direct[i].Directory);
                Save(Addres + "\\" + fileStr.Direct[i].Directory, AddresSave + "/"+ fileStr.Direct[i].Directory, fileStr.Direct[i].NodeNext);

             
            }
            for (int i = 0; i < fileStr.Files.Count; i++)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(AddresSave+"/"+fileStr.Files[i]);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("test_user", "1234567890");
                //request.EnableSsl = true; // если используется ssl

                // get answer as FtpWebResponse
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                // get stream and save as file 
                Stream responseStream = response.GetResponseStream();
                FileStream fs = new FileStream(Addres+"/"+fileStr.Files[i], FileMode.Create);
                
                byte[] buffer = new byte[64];
                int size = 0;

                while ((size = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, size);
                }
                fs.Close();
                response.Close();

                //Console.WriteLine("Saving complete");
                //Console.Read();
            }
        }
    }
}