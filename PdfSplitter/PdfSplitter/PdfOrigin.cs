using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFSplitter
{
   /// <summary>
   /// PdfOrigin class represents the original pdf document that gets split up
   /// </summary>
   public class PdfOrigin 
   {
      private const string PageNumberRegex = @"^\d+\s";
      private FileInfo _pdfFile;
      private PdfReader _reader;

      /// <summary>
      /// Initializes a new instance of the <see cref="PdfOrigin"/> class.
      /// This constructor is for testing. The stream for the reader can be set to 
      /// something besides a file.
      /// </summary>
      public PdfOrigin()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="PdfOrigin"/> class.
      /// This passes in the file to use. The reader will open a filestream 
      /// using this file.
      /// </summary>
      /// <param name="pdfFile">The PDF file.</param>
      public PdfOrigin(FileInfo pdfFile)
      {
         _pdfFile = pdfFile;
         FileName = _pdfFile.Name;
      }

      /// <summary>
      /// Gets or sets the name of the file.
      /// </summary>
      /// <value>The name of the file.</value>
      public string FileName { get; set; }

      /// <summary>
      /// Gets or sets the iText pdf reader.
      /// </summary>
      /// <value>The pdf reader.</value>
      public PdfReader Reader
      {
         get
         {
            if (_reader == null)
               _reader = new PdfReader(_pdfFile.OpenRead());
            return _reader;
         }

         set
         {
            _reader = value;
         }
      }

      /// <summary>
      /// Gets the number of pages in the pdf document.
      /// </summary>
      /// <value>The number of pages.</value>
      public int NumberOfPages
      {
         get
         {
            return Reader.NumberOfPages;
         }
      }

      /// <summary>
      /// Gets or sets the split documents.
      /// </summary>
      /// <value>The split documents.</value>
      public IList<IPdfSplit> SplitDocuments { get; set; }

      /// <summary>
      /// Setups the child Pdfs with the information necessary to do the splits.
      /// </summary>
      public void SetupChildPdfs()
      {
         ArrayList bookmarks = SimpleBookmark.GetBookmark(Reader);
         IList<IPdfSplit> childPdfs = new List<IPdfSplit>();
         string destinationFolder = ConfigurationManager.AppSettings["Destination Folder"];
         int sequence = 1;
         foreach (Hashtable bookmark in bookmarks)
         {
            IPdfSplit child = new PdfSplit();

            string page = bookmark["Page"].ToString();
            int startPage = GetPageNumberFromBookmark(page);
            child.StartPage = startPage;
            child.BookmarkName = bookmark["Title"].ToString();
            child.SequenceNumber = sequence++;
            child.FileName = GetChildFileName(FileName, child.BookmarkName, child.SequenceNumber);
            child.DestinationPath = destinationFolder;
            Document newDoc = new Document(Reader.GetPageSizeWithRotation(startPage));
            child.SetPdfDocument(newDoc);
            childPdfs.Add(child);
         }

         // find ending page for each document
         int lastPage = Reader.NumberOfPages;
         for (int i = childPdfs.Count - 1; i >= 0; i--)
         {
            IPdfSplit child = childPdfs[i];
            child.EndPage = lastPage;
            lastPage = child.StartPage - 1;
         }

         SplitDocuments = childPdfs;
      }

      /// <summary>
      /// Does the split.
      /// </summary>
      public void DoSplit()
      {
         foreach (IPdfSplit pdf in SplitDocuments)
         {
            pdf.OpenPdfDocument();
            pdf.SplitDocument(Reader);
            pdf.ClosePdfDocument();
         }
      }

      /// <summary>
      /// Closes the origin.
      /// </summary>
      public void CloseOrigin()
      {
         Reader.Close();
      }
      /// <summary>
      /// Gets the page number from bookmark.
      /// </summary>
      /// <param name="page">The page.</param>
      /// <returns></returns>
      internal int GetPageNumberFromBookmark(string page)
      {
         string num;
         Match m = Regex.Match(page, PageNumberRegex);
         if (m.Success)
            num = m.Value;
         else
            throw new Exception("PDF Bookmark does not contain the necessary page information. PDF document may be corrupt");
         int n = Convert.ToInt32(num);
         return n;
      }

      /// <summary>
      /// Gets the name of the child file.
      /// </summary>
      /// <param name="originFileName">Name of the origin file.</param>
      /// <param name="childName">Name of the child.</param>
      /// <param name="sequence">The sequence.</param>
      /// <returns></returns>
      internal string GetChildFileName(string originFileName, string childName, int sequence)
      {
         string fileName = originFileName.Substring(0, originFileName.IndexOf(".pdf"));

         // strip out punctuation
         StringBuilder sb = new StringBuilder();
         fileName = string.Format("{0}{1}_{2}", fileName, sequence, childName);
         foreach (char c in fileName)
         {
            if (char.IsPunctuation(c))
               sb.Append(' ');
            else
               sb.Append(c);
         }

         fileName = sb.ToString();

         // replaces spaces with '_'
         string regex = @"\s+";
         fileName = Regex.Replace(fileName, regex, "_");
         fileName = fileName + ".pdf";
         return fileName;
      }
   }
}