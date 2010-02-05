using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFSplitter
{
   /// <summary>
   /// Represents a new document that is split from the original.
   /// Only complete pages can be taken and saved to a new file
   /// using iTextSharp.
   /// </summary>
   public class PdfSplit : IPdfSplit
   {
      private PdfWriter _writer;
      private Document _document;

      /// <summary>
      /// Gets or sets the start page in the original pdf
      /// </summary>
      /// <value>The start page.</value>
      public int StartPage { get; set; }

      /// <summary>
      /// Gets or sets the end page in the original pdf
      /// </summary>
      /// <value>The end page.</value>
      public int EndPage { get; set; }

      /// <summary>
      /// Gets or sets the name of the bookmark.
      /// </summary>
      /// <value>The name of the bookmark.</value>
      public string BookmarkName { get; set; }

      /// <summary>
      /// Gets or sets the file name for the new pdf document
      /// </summary>
      /// <value>The file.</value>
      public string FileName { get; set; }

      /// <summary>
      /// Gets or sets the path to the folder where the split documents will be saved.
      /// </summary>
      /// <value>The destination path.</value>
      public string DestinationPath { get; set; }

      /// <summary>
      /// Gets or sets the sequence number.
      /// </summary>
      /// <value>The sequence number.</value>
      public int SequenceNumber { get; set; }

      /// <summary>
      /// Gets or sets the writer.
      /// </summary>
      /// <value>The writer.</value>
      public PdfWriter Writer
      {
         get
         {
            if (_writer == null)
            {
               FileInfo file = new FileInfo(Path.Combine(DestinationPath, FileName));
               Stream stream = file.Open(FileMode.CreateNew, FileAccess.Write);
               _writer = PdfWriter.GetInstance(_document, stream);
            }

            return _writer;
         }

         set
         {
            _writer = value;
         }
      }

      /// <summary>
      /// Sets the iTextSharp document instance
      /// </summary>
      /// <param name="document">The document.</param>
      public void SetPdfDocument(Document document)
      {
         _document = document;
      }

      /// <summary>
      /// Opens the iTextSharp document.
      /// </summary>
      public void OpenPdfDocument()
      {
         _document.Open();
      }


      /// <summary>
      /// Closes the document.
      /// </summary>
      public void ClosePdfDocument()
      {
         _document.Close();
      }

      /// <summary>
      /// Splits the document and saves the contents to a new pdf.
      /// </summary>
      /// <param name="reader">The reader.</param>
      public void SplitDocument(PdfReader reader)
      {
         PdfWriter writer = Writer;
         _document.Open();
         PdfContentByte content = writer.DirectContent;

         for (int i = StartPage; i <= EndPage; i++)
         {
            PdfImportedPage page = writer.GetImportedPage(reader, i);
            Rectangle pageSize = reader.GetPageSizeWithRotation(i);
            _document.SetPageSize(pageSize);
            _document.NewPage();
            int rotation = reader.GetPageRotation(i);
            if (rotation == 90 || rotation == 270)
               content.AddTemplate(page, 0, -1f, 1f, 0, 0, pageSize.Height);
            else
               content.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
         }
      }
   }
}