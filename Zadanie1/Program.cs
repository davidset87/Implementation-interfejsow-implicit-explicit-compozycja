using System;

namespace Zadanie1
{
    class Program
    {
        static void Main()
        {
            var xerox = new Copier();
            xerox.PowerOn();

            IDocument doc1 = new PDFDocument("aaa.pdf");
            xerox.Print(in doc1);

            IDocument doc2;
            xerox.Scan(out doc2);

            xerox.ScanAndPrint();

            Console.WriteLine($"Total operations: {xerox.Counter}");
            Console.WriteLine($"Print count: {xerox.PrintCounter}");
            Console.WriteLine($"Scan count: {xerox.ScanCounter}");

            xerox.PowerOff();
            xerox.Print(in doc1);
            xerox.Scan(out doc2);
        }
    }

    public class Copier : IPrinter, IScanner
    {
        private IDevice.State state = IDevice.State.off;
        public IDevice.State GetState() => state;

        public int Counter { get; private set; } = 0;
        public int PrintCounter { get; private set; } = 0;
        public int ScanCounter { get; private set; } = 0;

        public void PowerOn()
        {
            if (state == IDevice.State.off)
            {
                state = IDevice.State.on;
                Counter++;
                Console.WriteLine("Device powered ON");
            }
        }

        public void PowerOff()
        {
            if (state == IDevice.State.on)
            {
                state = IDevice.State.off;
                Console.WriteLine("Device powered OFF");
            }
        }

        public void Print(in IDocument document)
        {
            if (state != IDevice.State.on) return;

            PrintCounter++;
            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Print: {document.GetFileName()}");
        }

        public void Scan(out IDocument document, IDocument.FormatType formatType = IDocument.FormatType.JPG)
        {
            document = null;
            if (state != IDevice.State.on) return;

            ScanCounter++;
            string fileName = formatType switch
            {
                IDocument.FormatType.PDF => $"PDFScan{ScanCounter}.pdf",
                IDocument.FormatType.TXT => $"TextScan{ScanCounter}.txt",
                _ => $"ImageScan{ScanCounter}.jpg",
            };

            document = formatType switch
            {
                IDocument.FormatType.PDF => new PDFDocument(fileName),
                IDocument.FormatType.TXT => new TextDocument(fileName),
                _ => new ImageDocument(fileName),
            };

            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Scan: {fileName}");
        }

        public void ScanAndPrint()
        {
            if (state != IDevice.State.on) return;

            Scan(out IDocument document);
            Print(in document);
        }
    }
}