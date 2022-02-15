using iText.Layout;
using iText.Layout.Properties;
using System.Collections.Generic;

namespace PolarPdf
{
    public class SectionObject : IPdfObject
    {
        public SectionObject()
        {
            Elements = new List<IPdfObject>();
        }
        public string SectionName { get; set; }

        List<IPdfObject> Elements { get; set; }

        public SectionObject Add(IPdfObject pdfElement)
        {
            //todo: enforce the the pdfElement.Y has a value
            Elements.Add(pdfElement);
            return this;
        }
        public SectionObject Add(string Text, float X, float Y , Style Style = null, iText.Layout.Properties.TextAlignment TextAlignment = TextAlignment.LEFT)
        {
            Elements.Add(new TextObject { Text = Text, X = X, Y = Y, Style = Style, TextAlignment=TextAlignment });
            return this;
        }

        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            foreach(IPdfObject pdfElement in Elements)
            {
                pdfElement.Render(document, easyDocument, SuppressNewPage);
            }
        }
    }
}
