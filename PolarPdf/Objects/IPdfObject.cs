using iText.Layout;

namespace PolarPdf
{
    public interface IPdfObject
    {
        void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage);
    }
}
