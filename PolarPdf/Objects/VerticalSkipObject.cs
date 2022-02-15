using iText.Layout;

namespace PolarPdf
{
    public class VerticalSkipObject : IPdfObject
    {
        public VerticalSkipObject(float? YDelta=null)
        {
            if (YDelta.HasValue)
                this.YDelta = YDelta.Value;
        }
        public float YDelta { get; set; }

        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            easyDocument.CurrentYPosition -= YDelta;
        }
    }
}
