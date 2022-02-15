
using iText.IO.Image;
using iText.Layout;
using iText.Layout.Element;
using System.Drawing;
using System.IO;

namespace PolarPdf
{
    public class ImageObject : IPdfObject
    {
        public ImageObject(System.Drawing.Image Image = null)
        {
            this.Image = Image;
        }

        public ImageObject(byte[] ImageArray)
        {
            this.ImageAsArray = ImageArray;
        }
        public System.Drawing.Image Image { get; set; }

        public byte[] ImageAsArray { get; set; } 
        public float X { get; set; }
        public float? Y { get; set; }
        public float Width { get; set; }

        public ImageObject SetPosition(float X, float? Y, float Width)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            return this;
        }

        public void Render(Document document, PolarPdf easyDocument, bool SuppressNewPage)
        {
            if (Image!=null)
            {
                using (var ms = new MemoryStream())
                {
                    Image.Save(ms, Image.RawFormat);
                    ImageAsArray= ms.ToArray();
                }
            }
            iText.Layout.Element.Image i = new iText.Layout.Element.Image(ImageDataFactory.Create(ImageAsArray), X, (Y ?? easyDocument.CurrentYPosition));
            i.SetWidth(Width);
            document.Add(i);
        }
    }
}
