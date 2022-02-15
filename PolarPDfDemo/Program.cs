using System;

namespace PolarPdfDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Creating the a demo invoice in c:\temp\polarpdf");
            InvoiceDemo invoiceDemo = new InvoiceDemo();
            invoiceDemo.Do();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
