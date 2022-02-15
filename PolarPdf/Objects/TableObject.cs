using iText.Layout;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PolarPdf
{
    public class TableObject : IPdfObject
    {
        public TableObject()
        {
            RepeatHeaderOnNewPage = true;
        }

        public TableObject(IEnumerable<object> DataSource, string Tabs)
        {
            this.DataSource = DataSource;
            this.Tabs = Tabs;
            RepeatHeaderOnNewPage = true;
        }
        public IEnumerable<object> DataSource { get; set; }

        public float? Y { get; set; }

        //public float[] Tabs { get; set; }

        public string Tabs { get; set; }

        public Style Style { get; set; }
        public object[] HeaderTexts { get; set; }
        public Style HeaderStyle { get; set; }
        public int HeaderBorders { get; set; } //1 = Bottom, 2 = Top, 4= Left, 8 = Right
        public object[] FooterTexts { get; set; }
        public Style FooterStyle { get; set; }
        public int FooterBorders { get; set; }
        public bool RepeatHeaderOnNewPage { get; set; }

        public float TopOfPage { get => 842; }
        public float WidthOfPage { get => 594; }


        public TableObject SetHeaders(object[] Texts, Style Style=null, int? Borders=null) { HeaderTexts = Texts; HeaderStyle = Style ?? HeaderStyle; HeaderBorders= Borders?? HeaderBorders ;return this;}
        public TableObject SetFooters(object[] Texts, Style Style = null, int? Borders=null) {FooterTexts = Texts; FooterStyle = Style ?? FooterStyle; FooterBorders = Borders ?? FooterBorders; return this;}
        public TableObject SetTabs(string Tabs){this.Tabs = Tabs;return this;}
        public TableObject SetStyle(Style Style) { this.Style = Style; return this; }
        public TableObject SetDataSource(IEnumerable<object> Data) { this.DataSource=Data; return this; }
        public TableObject Set(bool? RepeatHeaderOnNewPage=null) { this.RepeatHeaderOnNewPage = RepeatHeaderOnNewPage??this.RepeatHeaderOnNewPage; return this; }
        public TableObject SetPosition(float? Y=null) { this.Y = Y; return this; }

        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            float[] tabPositions = Tabs.Split(",").Select(x => (float)Convert.ToDouble(Regex.Match(x, "\\d{1,3}").Captures[0].Value)).ToArray();
            string[] tabAligments = Tabs.Split(",").Select(x =>
            {
                var result = Regex.Match(x, "L|C|R");
                return result.Success ? result.Captures[0].Value : "L";
            }
            ).ToArray();



            //float yDelta = 0;
            float y = Y ?? easyDocument.CurrentYPosition;

            y=doHeader(document, easyDocument, HeaderTexts, HeaderStyle, HeaderBorders, tabPositions, tabAligments, y);

            bool swapRowsAsColumns = tabPositions.Length>1 && DataSource.FirstOrDefault() is string;

            if (swapRowsAsColumns)
            {
                int rowCount = 0;
                foreach (var row in DataSource)
                {
                    TextAlignment textAlignment = ToTextAlignment(tabAligments[rowCount]);
                    document.ShowTextAligned(easyDocument.getStyledText(row.ToString(), Style), tabPositions[rowCount], y, textAlignment);
                    rowCount++;
                }
                y -= easyDocument.InterLineDistance;
            }
            else
            {
                foreach (var row in DataSource)
                {
                    if (tabPositions.Count() == 1)    //1 kolom table
                    {
                        TextAlignment textAlignment = ToTextAlignment(tabAligments[0]);
                        document.ShowTextAligned(easyDocument.getStyledText(row.ToString(), Style), tabPositions[0], y, textAlignment);
                    }
                    else
                    {
                        int i = 0;
                        foreach (var field in row.GetType().GetProperties())
                        {
                            TextAlignment textAlignment = ToTextAlignment(tabAligments[i]);
                            string value = field.GetValue(row).ToString();
                            document.ShowTextAligned(easyDocument.getStyledText(value, Style), tabPositions[i], y, textAlignment);
                            i++;
                        }
                    }
                    y -= easyDocument.InterLineDistance;

                    if ((y) < easyDocument.BottomYPosition && !SuppressNewPage)
                    {
                        easyDocument.doNewPage(document);
                        y = easyDocument.CurrentYPosition;

                        if (RepeatHeaderOnNewPage)
                            y = doHeader(document, easyDocument, HeaderTexts, HeaderStyle, HeaderBorders, tabPositions, tabAligments, y);
                    }
                }
            }

            y = doHeader(document, easyDocument, FooterTexts, FooterStyle, FooterBorders, tabPositions, tabAligments, y);
            easyDocument.CurrentYPosition = y; //  - easyDocument.InterLineDistance;

        }

        private float doHeader(Document document, PolarPdf easyDocument, object[] Texts, Style Style, int Borders, float[] tabPositions, string[] tabAligments, float Y)
        {
            if (Texts != null)
            {
                float padding = 4;  //padding for the borders
                float verticalOffset = 16;
                if (Borders != 0)
                {
                    RectangleObject rectangleObject = new RectangleObject { X = tabPositions[0] - padding, Y = Y + verticalOffset, XTo = tabPositions.Last() + padding, YTo = Y - easyDocument.InterLineDistance - padding * 2 + verticalOffset, Borders = Borders };
                    rectangleObject.Render(document, easyDocument, false);

                    if ((Borders & 2) == 2)
                        Y -= padding;
                }

                int i = 0;
                foreach (var text in Texts)
                {
                    TextAlignment textAlignment = ToTextAlignment(tabAligments[i]);
                    document.ShowTextAligned(easyDocument.getStyledText(text.ToString(), Style), tabPositions[i], Y , textAlignment);
                    i++;
                }
                Y -= easyDocument.InterLineDistance;
                if ((Borders & 1) == 1)
                    Y -= padding;
            }

            return Y;
        }

        TextAlignment ToTextAlignment(string TabAlignment)
        {
            TextAlignment[] alignments = new TextAlignment[] { iText.Layout.Properties.TextAlignment.LEFT, TextAlignment.CENTER, TextAlignment.RIGHT };
            return alignments["LCR".IndexOf(TabAlignment)];
        }
    }
}
