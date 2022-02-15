using PolarPdf;
using Electra.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Demo
    {
        public void DoDemo()
        {
            string data = Encoding.UTF8.GetString(Resource1.Nobel, 0, Resource1.Nobel.Length);
            var root =Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(data);

            var dataSet1 = root.prizes
                .Select(x => new { x.year, x.category, Count= x.laureates?.Count ?? 0 });

            var dataSet2 = root.prizes
                .GroupBy(x => x.category)
                .Select(x => new { category = x.Key, Count = x.Sum(y=>y.laureates?.Count()??0) })
                .OrderByDescending(x=>x.Count);

            PolarPdf.PolarPdf easyDocument = new PolarPdf.PolarPdf();

            /* 1
            //easyDocument.Add(dataSet1, "50, 150, 300R" );
            */

            /* 2
            //easyDocument.Add(dataSet1, "50, 150, 450R", HeaderTexts: new string[]{"Year", "Category", "Number of laureats" });
            */

            /* 3
            easyDocument.Add(dataSet2, "50, 350R", HeaderTexts: new string[] { "Category", "Number of laureats" }, HeaderStyle: easyDocument.DefaultStyle.SetBold());
            easyDocument.Add(dataSet1, "50, 150, 450R", HeaderTexts: new string[] { "Year", "Category", "Number of laureats" }, HeaderStyle: easyDocument.DefaultStyle.SetBold());
            */

            /* 4

            easyDocument.Add("List of no", 50, Style: easyDocument.Header1.SetBold());

            easyDocument.Add(dataSet2, "50, 350R", HeaderTexts: new string[] { "Category", "Number of laureats" }, HeaderStyle: easyDocument.DefaultStyle.SetBold());
            easyDocument.Add(dataSet1, "50, 150, 450R", HeaderTexts: new string[] { "Year", "Category", "Number of laureats" }, HeaderStyle: easyDocument.DefaultStyle.SetBold());
            */

            SectionObject section = new SectionObject()
                .Add("List of no", 50, 780, Style: easyDocument.Header1.SetBold())
                .Add(new PageNumberObject("Pagina {0}"));
            RectangleObject rectangleObject = new RectangleObject { X = 45, Y = 810, XTo = 550, YTo = 780, Borders = 0, FillColor = iText.Kernel.Colors.ColorConstants.ORANGE };
            section.Add(rectangleObject);

            easyDocument.PageLayout = section;
            easyDocument.TopYPosition = 760;

            TableObject tableObject2 = new TableObject(dataSet2, "50, 350R")
                .SetHeaders(new object[] { "Category", "Number of laureats" }, easyDocument.DefaultStyle.SetBold(),3)
                .SetFooters(new object[] { "Totals", dataSet2.Sum(x => x.Count) }, easyDocument.DefaultStyle.SetBold(), 3);
            easyDocument.Add(tableObject2);


            TableObject tableObject1 = new TableObject(dataSet1, "50, 150, 450R")
                .SetHeaders(new string[] { "Year", "Category", "Number of laureats" }, easyDocument.DefaultStyle.SetBold(), 3);
            easyDocument.Add(tableObject1);

            easyDocument.SaveAs("c:\\temp\\Demo.pdf");
        }

        public void DemoCnx()
        {
            //EasyDocument2<WerfTechHoofd> easyDocument2 = new EasyPdf.EasyDocument2<WerfTechHoofd>();
            CXNACC_ELECTRACONTRACTORSNVContext.ConnectionString = "server=localhost\\sqlexpress;database=CXNACC_ELECTRACONTRACTORSNV;Trusted_connection=true";
            CXNACC_ELECTRACONTRACTORSNVContext context = new CXNACC_ELECTRACONTRACTORSNVContext();



            WerfTechHoofd werfTechHoofd = context.WerfTechHoofds
                .Include("Werf")
                .Include("WerfTechArtikels.Artikel")
                .Include("WerfTechUren.Artikel")
                .Include("WerfTechUren.WerkNemer")
                .FirstOrDefault(x => x.Id == 4);


            PolarPdf.PolarPdf easyDocument = new PolarPdf.PolarPdf();

            SectionObject section = new SectionObject();
            section.Add(Text: "Hoofding", X: 50, Y: 800, Style: easyDocument.Header1.SetBold());
            section.Add(new TableObject
            {
                Y = 770,
                Tabs = "50",
                DataSource = new string[] { "Electra contractors", "Emiel Clausstraat 45", "8793   Waregem" }
            });
            section.Add(new TableObject { Y = 770, Tabs = "400", DataSource = new string[] { werfTechHoofd.Werf.Naam, werfTechHoofd.Werf.Straat, werfTechHoofd.Werf.Postnr + "  " + werfTechHoofd.Werf.Gem } });
            section.Add(new PageNumberObject("Pagina {0}"));
            section.Add(X: 50, Y: 680, Style: easyDocument.Header2, Text: $"Werkbon {werfTechHoofd.Id} van {werfTechHoofd.Datum:dd/MM/yyyy}");
            easyDocument.PageLayout = section;

            easyDocument.TopYPosition = 650;

            easyDocument.Add(new TableObject
            {
                DataSource = werfTechHoofd.WerfTechArtikels.Select(x => new { x.Artikel.Code, x.Omschrijving, x.Aantal }),
                Tabs = "50, 150, 500R",
                HeaderTexts = new string[] { "Code", "Omschrijving", "Aantal" },
                HeaderStyle = easyDocument.DefaultStyle.SetBold(),
                HeaderBorders = 3
            });
            easyDocument.Add(new TableObject
            {
                DataSource = werfTechHoofd.WerfTechUren.Select(x => new { x.Artikel.Omsn, x.WerkNemer.Naam, Aantal = x.Aantal.ToString() }),
                Tabs = "50, 250, 450",
                HeaderTexts = new string[] { "Type", "Werknemer", "Uren" }
            });

            easyDocument.Add(new LineObject { X = 50, Y = 140, XTo = 400 });
            easyDocument.Add("Handtekening klant", 50, 120, easyDocument.Header2.SetItalic());

            if (werfTechHoofd.Handtekening != null)
            {
                easyDocument.Add(werfTechHoofd.Handtekening, 50, 200, 70);
            }

            easyDocument.SaveAs("c:\\temp\\test.pdf");

        }
    }

    public class Laureate
    {
        public string id { get; set; }
        public string firstname { get; set; }
        public string surname { get; set; }
        public string motivation { get; set; }
        public string share { get; set; }
    }

    public class Prize
    {
        public string year { get; set; }
        public string category { get; set; }
        public List<Laureate> laureates { get; set; }
        public string overallMotivation { get; set; }
    }

    public class Root
    {
        public List<Prize> prizes { get; set; }
    }

}
