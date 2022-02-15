using iText.Layout;

namespace PolarPdf
{
    public class PageNumberObject : IPdfObject
    {
        public PageNumberObject(string Template="Page {0}")
        {
            this.Template = Template;
            X = 500;
            Y = 50;
        }
        public string Template { get; set;}
        public float X { get; set; }
        public float Y { get; set; }
        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            document.ShowTextAligned(string.Format(Template, easyDocument.CurrentPageNumber), X, Y, iText.Layout.Properties.TextAlignment.LEFT);
        }
    }
}
