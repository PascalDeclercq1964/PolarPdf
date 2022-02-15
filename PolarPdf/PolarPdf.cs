using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PolarPdf
{
    public class PolarPdf
    {
        public PolarPdf()
        {
            InterLineDistance = 18;
            TopYPosition = 800;
            BottomYPosition = 50;
            CurrentPageNumber = 1;
            Header1 = new Style { FontName = "Arial", FontSize = 24 };
            Header2 = new Style { FontName = "Arial", FontSize = 16 };
            DefaultStyle = new Style { FontName = "Arial", FontSize = 12 };
            SmallStyle=new Style { FontName = "Arial", FontSize = 8 };
        }

        List<IPdfObject> pdfElements = new List<IPdfObject>();

        public PolarPdf SetInterLineDistance(float InterLineDistance)
        {
            this.InterLineDistance = InterLineDistance;
            return this;
        }

        public PolarPdf SetTopYPostition(float Y)
        {
            this.TopYPosition = Y;
            return this;
        }

        public PolarPdf SetPageLayout(SectionObject PageLayout)
        {
            this.PageLayout = PageLayout;
            return this;
        }

        public float CurrentYPosition { get; set; }
        public float TopYPosition { get; set; }
        public float BottomYPosition { get; set; }
        public float InterLineDistance { get; set; }
        public SectionObject PageLayout { get; set; }
        public int CurrentPageNumber { get; set; }
        public Style Header1 { get; set; }
        public Style Header2 { get; set; }
        public Style DefaultStyle { get; set; }
        public Style SmallStyle { get; set; }

        public PolarPdf Add(IPdfObject pdfElement)
        {
            pdfElements.Add(pdfElement);
            return this;
        }



        public PolarPdf Add(string Text, float X, float? Y = null, Style Style = null, iText.Layout.Properties.TextAlignment TextAlignment=TextAlignment.LEFT)
        {
            pdfElements.Add(new TextObject { Text = Text, X = X, Y = Y, Style = Style, TextAlignment=TextAlignment });
            return this;
        }

        public PolarPdf Add(IEnumerable<object> data, string Tabs, float? Y=null, object[] HeaderTexts=null, object[] FooterTexts=null, Style DetailStyle=null, Style HeaderStyle=null, Style FooterStyle=null, int BordersHeader=0)
        {
            pdfElements.Add(new TableObject {Y = Y, DataSource = data, Tabs=Tabs, HeaderTexts=HeaderTexts, FooterTexts=FooterTexts, Style=DetailStyle, HeaderStyle=HeaderStyle, FooterStyle=FooterStyle, HeaderBorders=BordersHeader });
            return this;
        }

        public PolarPdf Add(byte[] ImageAsArray, float X, float Width, float? Y=null)
        {
            pdfElements.Add(new ImageObject { ImageAsArray=ImageAsArray, X=X, Y=Y, Width=Width });
            return this;
        }

        public void SaveAs(string FileName)
        {
            MemoryStream stream = GenerateStream();

            File.WriteAllBytes(FileName, stream.ToArray());

        }

        public byte[] SaveAsStream()
        {
            MemoryStream stream = GenerateStream();

            return stream.ToArray();
        }

        private MemoryStream GenerateStream()
        {
            MemoryStream stream = new MemoryStream();
            PdfWriter pdfWriter = new PdfWriter(stream);
            PdfDocument pdfDocument = new PdfDocument(pdfWriter);
            Document document = new Document(pdfDocument);

            if (PageLayout != null)
                PageLayout.Render(document, this, true);

            CurrentYPosition = TopYPosition;
            foreach (IPdfObject pdfElement in pdfElements)
            {
                pdfElement.Render(document, this, false);
            }

            document.Close();
            return stream;
        }

        internal Paragraph getStyledText(string Text, Style SelectedStyle)
        {
            Style style = SelectedStyle ?? DefaultStyle;
            Paragraph paragraph = new Paragraph(Text);
            style.ApplyStyle(paragraph);
            return paragraph;
        }

        internal void doNewPage(Document document)
        {
            NewPageObject newPage = new NewPageObject();
            newPage.Render(document, this, true);
            CurrentYPosition = TopYPosition;
        }
    }
}
