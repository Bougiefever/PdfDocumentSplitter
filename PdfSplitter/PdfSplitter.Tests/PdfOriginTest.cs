using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using iTextSharp.text.pdf;
using NUnit.Framework;
using PdfSplitter.Tests;
using Rhino.Mocks;

namespace PDFSplitter.Tests
{
   using Rhino.Mocks.Constraints;

   [TestFixture]
   public class PdfOriginTest
   {
      [Test][Ignore]
      public void ReadFileTest()
      {
         MemoryStream memoryStream = TestHelper.ReadTestFileToMemory();
         Assert.IsNotNull(memoryStream);
      }

      [Test]
      public void GetPageNumberFromBookmarkTest()
      {
         PdfOrigin origin = new PdfOrigin();
         string pagetext = "11 XYZ null null null";
         int num = origin.GetPageNumberFromBookmark(pagetext);
         Assert.AreEqual(11, num);
      }

      /// <summary>
      /// Tests the GetPageNumberFromBookmark method. A bad format throws an exception.
      /// </summary>
      [Test]
      [ExpectedException(typeof(Exception))]
      public void TestGetPageNumberFromBookmarkBadFormatThrowsException()
      {
         PdfOrigin origin = new PdfOrigin();
         string pagetext = "my bad format";
         int num = origin.GetPageNumberFromBookmark(pagetext);
      }

      [Test]
      public void GetChildDocNameFromOriginFileNameTest()
      {
         PdfOrigin origin = new PdfOrigin();
         origin.FileName = "MyFile.pdf";
         string childName = origin.GetChildFileName(origin.FileName, "the bookmark name", 1);
         Assert.AreEqual("MyFile1_the_bookmark_name.pdf", childName);
      }

      [Test]
      public void TestGetChildDocumentNameStripsPunctuation()
      {
         char c = ',';
         Assert.IsTrue(char.IsPunctuation(c));
         PdfOrigin origin = new PdfOrigin();
         origin.FileName = "HomeTest.pdf";
         string bookmarkName = "Home Supp App - Pg 1,2";
         string fileName = origin.GetChildFileName(origin.FileName, bookmarkName, 3);
         Assert.AreEqual("HomeTest3_Home_Supp_App_Pg_1_2.pdf", fileName);
      }

      [Test]
      public void SetupChildPdfsTest()
      {
         MemoryStream memoryStream = TestHelper.ReadTestFileToMemory();
         PdfReader reader = new PdfReader(memoryStream);
         PdfOrigin origin = new PdfOrigin();
         origin.Reader = reader;
         origin.FileName = "Test.pdf";
         origin.SetupChildPdfs();
         Assert.AreEqual(7, origin.SplitDocuments.Count);
      }

      [Test]
      public void DoSplitTest()
      {
         MemoryStream memoryStream = TestHelper.ReadTestFileToMemory();
         PdfReader reader = new PdfReader(memoryStream);
         PdfOrigin origin = new PdfOrigin();
         origin.Reader = reader;

         MockRepository mocks = new MockRepository();

         IPdfSplit split1 = mocks.DynamicMock<IPdfSplit>();
         IPdfSplit split2 = mocks.DynamicMock<IPdfSplit>();
         IPdfSplit split3 = mocks.DynamicMock<IPdfSplit>();

         IList<IPdfSplit> splits = new List<IPdfSplit> { split1, split2, split3 };
         origin.SplitDocuments = splits;

         Expect.Call(split1.OpenPdfDocument);
         Expect.Call(() => split1.SplitDocument(null))
            .IgnoreArguments()
            .Constraints(Is.Same(reader));
         Expect.Call(split1.ClosePdfDocument);

         Expect.Call(split2.OpenPdfDocument);
         Expect.Call(() => split2.SplitDocument(null))
            .IgnoreArguments()
            .Constraints(Is.Same(reader));
         Expect.Call(split2.ClosePdfDocument);

         Expect.Call(split3.OpenPdfDocument);
         Expect.Call(() => split3.SplitDocument(null))
            .IgnoreArguments()
            .Constraints(Is.Same(reader));
         Expect.Call(split3.ClosePdfDocument);

         mocks.ReplayAll();

         origin.DoSplit();

         mocks.VerifyAll();
      }

      [Test]
      public void SplitDocumentIntegrationTest()
      {
         DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["Destination Folder"]);
         FileInfo[] files = di.GetFiles("*.pdf");
         foreach (FileInfo file in files)
            if (file.Exists) file.Delete();

         FileInfo fi = new FileInfo(ConfigurationManager.AppSettings["Original Pdf"]);
         PdfOrigin origin = new PdfOrigin(fi);
         origin.SetupChildPdfs();
         origin.DoSplit();

         files = di.GetFiles("*.pdf");
         Assert.AreEqual(7, files.Length);
      }
   }
}