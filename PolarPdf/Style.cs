using iText.Kernel.Font;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarPdf
{
    public class Style
    {
        public float FontSize { get; set; }

        public string FontName { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }

        public void ApplyStyle(Paragraph paragraph)
        {
            paragraph.SetFontSize(FontSize);
            if (IsBold)
                paragraph.SetBold();
            if (IsItalic)
                paragraph.SetItalic();
        }

        public Style SetBold()
        {
            Style style = (Style)this.MemberwiseClone();
            style.IsBold = true;
            return style;
        }
        public Style SetItalic()
        {
            Style style = (Style)this.MemberwiseClone();
            style.IsItalic = true;
            return style;
        }
    }
}
