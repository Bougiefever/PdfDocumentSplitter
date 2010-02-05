using System;
using System.IO;

namespace PdfSplitter.Tests
{
   public static class TestHelper
   {
      public static MemoryStream ReadTestFileToMemory()
      {
         byte[] result = new byte[0];
         string file = System.Configuration.ConfigurationManager.AppSettings["Original Pdf"];
         FileInfo fi = new FileInfo(file);
         using (BinaryReader brdr = new BinaryReader(fi.OpenRead()))
         {
            int bufferSize = 32768; // 32k
            byte[] buffer;
            long pos = 0;

            while (true)
            {
               buffer = brdr.ReadBytes(bufferSize);
               if (pos > 0)
               {
                  // copy old data to bigger result
                  byte[] temp = new byte[result.LongLength];
                  Array.Copy(result, temp, result.LongLength);
                  result = new byte[temp.LongLength + buffer.Length];
                  Array.Copy(temp, result, temp.LongLength);

                  // add new data
                  for (int i = 0; i < buffer.Length; i++)
                  {
                     result[pos + i] = buffer[i];
                  }

                  pos += buffer.Length;
               }
               else
               {
                  result = new byte[buffer.Length];
                  Array.Copy(buffer, result, buffer.Length);
                  pos = buffer.Length;
               }
               if (buffer.Length < bufferSize)
                  break;
            }

            brdr.Close();
         }

         MemoryStream mem = new MemoryStream(result);
         return mem;
      }
   }
}