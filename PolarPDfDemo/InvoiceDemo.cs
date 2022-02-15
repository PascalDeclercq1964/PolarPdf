using PolarPdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarPdfDemo
{
    public class InvoiceDemo
    {

        public void Do()
        {
            /* prepare the data and output path*/
            Invoice invoiceData = Newtonsoft.Json.JsonConvert.DeserializeObject<Invoice>(jsonData);
            string path = "c:\\temp\\polarpdf";
            System.IO.Directory.CreateDirectory(path);


            /* The real work starts here ... */
            PolarPdf.PolarPdf invoice = new PolarPdf.PolarPdf();

            /* 
            Create a section that will contain all the stuff that will be repeated on each page. 
            A section is simply a collection of pdf objects that can be grouped/repeated 
            This section object is assigned to the polarpdf's PageLayout a bit further
            */
            SectionObject section = new SectionObject()
                .Add(new ImageObject(Resource1.Super_fancy_logo).SetPosition(50,730,150))
                .Add(new TableObject(new object[] {invoiceData.Customer.Name, invoiceData.Customer.Address, invoiceData.Customer.Zip+' '+invoiceData.Customer.City }, "400").SetPosition(700))
                .Add(new TableObject(new object[] { "Polar Pdf","Arctic avenue 750", "75512 Antartica" }, "50").SetPosition(710))
                .Add(new LineObject() { LineWidth=0.5F}.SetPosition(50, 60, 594-50, null))
                .Add(new TableObject(new object[] { "www.polarpdf.com", "info@polarpdf.com", "VAT BE 0123.425.633" }, "50, 250C, 500R").SetPosition(40))
                .Add(new RectangleObject { X = 30, XTo = 594-30, Y = 842-30, YTo = 30, Borders = 15, LineWidth=0.5F });


            /* Create a tableoject that holds the invoice detail lines 
               In the constructor we pass the table data and the tabs as required info.
               The rest, in this case the headers, is optional
               The Tabs parameter is a comma delimited string with tabpositions for each column where a tab can be suffixed with L, R or C for left or right aligned or centered.
               Left aligned is the default and the L can be ommitted.
             */
            TableObject details = new TableObject(invoiceData.InvoiceDetails.Select(x=>new { x.ProductCode, x.Description, x.Quantity, Price=x.Price.ToString("0.00"), x.VatRate, SubTotal=x.SubTotal.ToString("0.00") }), "50,120,390R,450R,495R,544R")
                .SetHeaders(new string[] { "Code", "Product", "Quantity", "Price", "Vat rate", "Subtotal" }, Borders: 3);

            /* Create another tableobject that holds the invoices header info. As it isn't added the the Page Layout section, this info is not repeated on each page.
               In the constructor we pass the table data and the tabs as required info.
               The SetPosition(560) forces this object on a fixed vertical position. 
            */
            TableObject headerinfo = new TableObject(new string[] { invoiceData.InvoiceDate.ToShortDateString(), invoiceData.Customer.CustomerId.ToString(), invoiceData.DueDate.ToShortDateString(), invoiceData.Customer.VatNumber  }, "50,150, 300, 400")
                .SetPosition(560)
                .SetHeaders(new object[] { "Document date", "Customer number", "Due date", "Vat number" }, Style: invoice.DefaultStyle.SetBold());

            /* Create yet another tableobject with the Vat summary
               This is an example of a table object with headers and footers
            */
            TableObject vatSummary = new TableObject(invoiceData.InvoiceDetails.GroupBy(x => x.VatRate).Select(x => new { x.Key, VatBase = x.Sum(y => y.SubTotal), VatAmount=x.Sum(x=>x.VatAmount) }), "50, 150R, 250R")
                .SetHeaders(new string[] {"Rate", "Base", "Amount" }, Style: invoice.DefaultStyle.SetBold(), 3)
                .SetFooters(new string[] {"", invoiceData.InvoiceDetails.Sum(x=>x.SubTotal).ToString("0.00"), invoiceData.InvoiceDetails.Sum(x => x.VatAmount).ToString("0.00") });

            /* Now we are ready to put everything together.
               It is important to know for elements that are added (using the Add()), the order is important.
            */
            invoice
                .SetPageLayout(section)
                .SetTopYPostition(410)
                .Add($"Invoice #{invoiceData.InvoiceNumber}", 50, 600, Style: invoice.Header2.SetBold())
                .Add(headerinfo)
                .Add(details)
                .Add(new VerticalSkipObject(10))
                .Add(vatSummary);


            /* Create the pdf and save it to disk. To get a stream instead use the Save() method.*/
            invoice.SaveAs($"{path}\\InvoiceDemo.pdf");

        }


        #region helper stuff for the demo

        class Invoice
        {
            public DateTime InvoiceDate { get; set; }
            public Customer Customer { get; set; }
            public string InvoiceNumber { get; set; }
            public DateTime DueDate { get; set; }
            public List<InvoiceDetail> InvoiceDetails { get; set; }
        }

        class InvoiceDetail
        {
            public string ProductCode { get; set; }
            public string Description { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal VatRate { get; set; }

            public decimal SubTotal { get { return Quantity * Price; } }
            public decimal VatAmount { get { return Math.Round(SubTotal * VatRate/100, 2); } }
            public decimal Total { get { return SubTotal + VatAmount; } }
        }
        class Customer
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Zip { get; set; }
            public string City { get; set; }
            public string VatNumber { get; set; }
            public int CustomerId { get; set; }

        }


        string jsonData =
@"{
    'InvoiceDate' : '2022/02/03',
    'DueDate' : '2022/04/30',
    'InvoiceNumber' : '2022/0156',
    'Customer' : {'Name': 'Pascal Declercq', 'Address' : 'Black row 8', 'Zip' : '8501', 'City' : 'Heule', 'CustomerId' : '124279', 'VatNumber' : 'BE 0679.190.049'},
    'InvoiceDetails' : [
            { 'ProductCode' : '1002565', 'Description' : 'SpringVeer', 'Quantity' : 25, 'Price' : 1.55, 'VatRate' : 21},
            { 'ProductCode' : '5522144', 'Description' : 'Snowboard Ice mountain', 'Quantity' : 2, 'Price' : 355, 'VatRate' : 21},
            { 'ProductCode' : '9875454', 'Description' : 'Bike helmet chickenhead', 'Quantity' : 5, 'Price' : 37.5, 'VatRate' : 21},
            { 'ProductCode' : '3213440', 'Description' : 'Backpack monkey island', 'Quantity' : 3, 'Price' : 12.77, 'VatRate' : 21},
            { 'ProductCode' : '0054653', 'Description' : 'Notebook classix', 'Quantity' : 12, 'Price' : 2.4, 'VatRate' : 21},
            { 'ProductCode' : '4654444', 'Description' : 'Coca cola 33cl', 'Quantity' : 5, 'Price' : 2.2, 'VatRate' : 6},
            { 'ProductCode' : '6589878', 'Description' : 'Sandwich classic', 'Quantity' : 12, 'Price' : 3.2, 'VatRate' : 6},
            { 'ProductCode' : '5646544', 'Description' : 'Apple', 'Quantity' : 12, 'Price' : 0.25, 'VatRate' : 6}]
}";
    }
    #endregion
}
