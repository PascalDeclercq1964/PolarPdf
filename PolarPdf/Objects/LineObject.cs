using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;

namespace PolarPdf
{
    public class LineObject : IPdfObject
    {
        public LineObject()
        {
            LineWidth = 0.8F;
            Color = ColorConstants.BLACK;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float? XTo { get; set; }
        public float? YTo { get; set; }
        public float LineWidth { get; set; }
        public iText.Kernel.Colors.Color Color { get; set; }

        public LineObject SetPosition(float X, float Y, float? Xto, float? YTo)
        {
            this.X = X;
            this.Y = Y;
            this.XTo = Xto;
            this.YTo = YTo;
            return this;
        }

        public LineObject Set(float LineWitdh, iText.Kernel.Colors.Color Color)
        {
            this.LineWidth = LineWidth;
            this.Color = Color;
            return this;
        }

        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            var pdfDocument = document.GetPdfDocument();
            var pdfPage=pdfDocument.GetPage(easyDocument.CurrentPageNumber);
            var canvas = new PdfCanvas(pdfPage);
            canvas.SetLineWidth(LineWidth);
            canvas.SetStrokeColor(Color);
            canvas.MoveTo(X, Y);
            canvas.LineTo(XTo??X, YTo??Y);
            canvas.ClosePathStroke();
        }
    }
}
