using iText.Layout;
using iText.Layout.Properties;

namespace PolarPdf
{
    public class TextObject : IPdfObject
    {
        public TextObject()
        {
            TextAlignment = TextAlignment.LEFT;
        }

        public float X { get; set; }
        public float? Y { get; set; }

        public Style Style { get; set; }
        public string Text { get; set; }

        public iText.Layout.Properties.TextAlignment TextAlignment { get; set; }
        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            if (!Y.HasValue && easyDocument.CurrentYPosition<easyDocument.BottomYPosition && !SuppressNewPage)
            {
                easyDocument.doNewPage(document);
            }
            document.ShowTextAligned(easyDocument.getStyledText(Text, Style), X, Y ?? easyDocument.CurrentYPosition, TextAlignment);
            if (!Y.HasValue)
            {
                easyDocument.CurrentYPosition -= easyDocument.InterLineDistance;
            }
        }
    }
}
