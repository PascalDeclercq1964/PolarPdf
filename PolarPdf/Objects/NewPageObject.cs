using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PolarPdf
{
    public class NewPageObject : IPdfObject
    {
        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            easyDocument.CurrentPageNumber++;
            easyDocument.CurrentYPosition = easyDocument.TopYPosition;
            if (easyDocument.PageLayout != null)
                easyDocument.PageLayout.Render(document, easyDocument, true);
        }
    }
}
