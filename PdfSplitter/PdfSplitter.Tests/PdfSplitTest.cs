using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using PDFSplitter;

namespace PdfSplitter.Tests
{
   [TestFixture]
   public class PdfSplitTest
   {
      [Test]
      public void SplitDocumentTest()
      {
         MemoryStream readerStream = TestHelper.ReadTestFileToMemory();
         PdfReader reader = new PdfReader(readerStream);
         PdfSplit newDoc = new PdfSplit();
         newDoc.StartPage = 3;
         newDoc.EndPage = 4;
         Document pdf = new Document(reader.GetPageSize(3));
         newDoc.SetPdfDocument(pdf);

         MemoryStream writerStream = new MemoryStream();
         
         PdfWriter writer = PdfWriter.GetInstance(pdf, writerStream);
         newDoc.Writer = writer;
         Assert.AreEqual(0, writerStream.Length);

         newDoc.OpenPdfDocument();

         newDoc.SplitDocument(reader);

         Assert.IsTrue(writerStream.Length > 0);
         newDoc.ClosePdfDocument();
      }
   }
}
