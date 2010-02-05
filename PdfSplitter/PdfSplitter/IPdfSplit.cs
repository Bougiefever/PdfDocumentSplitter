using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFSplitter
{
   public interface IPdfSplit
   {
      /// <summary>
      /// Gets or sets the start page in the original pdf
      /// </summary>
      /// <value>The start page.</value>
      int StartPage { get; set; }

      /// <summary>
      /// Gets or sets the end page in the original pdf
      /// </summary>
      /// <value>The end page.</value>
      int EndPage { get; set; }

      /// <summary>
      /// Gets or sets the name of the bookmark.
      /// </summary>
      /// <value>The name of the bookmark.</value>
      string BookmarkName { get; set; }

      /// <summary>
      /// Gets or sets the file name for the new pdf document
      /// </summary>
      /// <value>The file.</value>
      string FileName { get; set; }

      /// <summary>
      /// Gets or sets the path to the folder where the split documents will be saved.
      /// </summary>
      /// <value>The destination path.</value>
      string DestinationPath { get; set; }

      /// <summary>
      /// Gets or sets the sequence number.
      /// </summary>
      /// <value>The sequence number.</value>
      int SequenceNumber { get; set; }

      /// <summary>
      /// Gets or sets the writer.
      /// </summary>
      /// <value>The writer.</value>
      PdfWriter Writer { get; set; }

      /// <summary>
      /// Sets the iTextSharp document instance
      /// </summary>
      /// <param name="document">The document.</param>
      void SetPdfDocument(Document document);

      /// <summary>
      /// Opens the iTextSharp document.
      /// </summary>
      void OpenPdfDocument();

      /// <summary>
      /// Closes the iTestSharp document.
      /// </summary>
      void ClosePdfDocument();

      /// <summary>
      /// Splits the document and saves the contents to a new pdf.
      /// </summary>
      /// <param name="reader">The reader.</param>
      void SplitDocument(PdfReader reader);
   }
}