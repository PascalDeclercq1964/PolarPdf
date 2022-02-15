using iText.Kernel.Colors;
using iText.Layout;

namespace PolarPdf
{
    public class RectangleObject : IPdfObject
    {
        public RectangleObject()
        {
                LineWidth = 1;
                LineColor = ColorConstants.BLACK;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float XTo { get; set; }
        public float YTo { get; set; }
        public float LineWidth { get; set; }
        public iText.Kernel.Colors.Color LineColor { get; set; }
        public iText.Kernel.Colors.Color FillColor { get; set; }

        public int Borders { get; set; } //1 = bottom, 2 = Top , 4 =Left, 8 = Top
        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            if ((Borders&1)==1)
            {
                LineObject lineObject = new LineObject { X = X, Y = YTo, XTo = XTo, LineWidth = LineWidth, Color = LineColor };
                lineObject.Render(document, easyDocument, true);
            }
            if ((Borders & 2) == 2)
            {
                LineObject lineObject = new LineObject { X = X, Y = Y, XTo = XTo, LineWidth = LineWidth, Color = LineColor };
                lineObject.Render(document, easyDocument, true);
            }
            if ((Borders & 4) == 4)
            {
                LineObject lineObject = new LineObject { X = X, Y = Y, XTo = X, YTo=YTo, LineWidth = LineWidth, Color = LineColor };
                lineObject.Render(document, easyDocument, true);
            }
            if ((Borders & 8) == 8)
            {
                LineObject lineObject = new LineObject { X = XTo, Y = Y, XTo = XTo, YTo=YTo, LineWidth = LineWidth, Color = LineColor };
                lineObject.Render(document, easyDocument, true);
            }

            if (FillColor!=null)
            {
                //var pdfDocument = document.GetPdfDocument();
                //var pdfPage = pdfDocument.GetPage(easyDocument.CurrentPageNumber);
                //var canvas = new PdfCanvas(pdfPage);

                //iText.Kernel.Geom.Rectangle rect = new iText.Kernel.Geom.Rectangle(X, Y, XTo - X, YTo - Y);

                //Div div = new Div();
                //canvas.AddXObject(div); //.Rectangle(rect);
            }
        }
    }
}
