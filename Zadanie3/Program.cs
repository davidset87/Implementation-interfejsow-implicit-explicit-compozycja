using System;

namespace Zadanie3
{
    public interface IDevice
    {
        enum State { on, off };

        void PowerOn();
        void PowerOff();
        State GetState();
    }

    public interface IPrinter : IDevice
    {
        void Print(in IDocument document);
    }

    public interface IScanner : IDevice
    {
        void Scan(out IDocument document, IDocument.FormatType formatType);
    }

    public interface IFax : IDevice
    {
        void SendFax(in IDocument document);
    }

    public interface IDocument
    {
        enum FormatType { TXT, PDF, JPG }

        string GetFileName();
    }

    public class PDFDocument : IDocument
    {
        private string filename;
        public PDFDocument(string filename) => this.filename = filename;
        public string GetFileName() => filename;
    }

    public class ImageDocument : IDocument
    {
        private string filename;
        public ImageDocument(string filename) => this.filename = filename;
        public string GetFileName() => filename;
    }

    public class TextDocument : IDocument
    {
        private string filename;
        public TextDocument(string filename) => this.filename = filename;
        public string GetFileName() => filename;
    }

    public class Printer : IPrinter
    {
        private IDevice.State state = IDevice.State.off;
        public int PrintCounter { get; private set; } = 0;

        public void PowerOn() => state = IDevice.State.on;
        public void PowerOff() => state = IDevice.State.off;
        public IDevice.State GetState() => state;

        public void Print(in IDocument document)
        {
            if (state == IDevice.State.off) return;
            PrintCounter++;
            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Print: {document.GetFileName()}");
        }
    }

    public class Scanner : IScanner
    {
        private IDevice.State state = IDevice.State.off;
        public int ScanCounter { get; private set; } = 0;

        public void PowerOn() => state = IDevice.State.on;
        public void PowerOff() => state = IDevice.State.off;
        public IDevice.State GetState() => state;

        public void Scan(out IDocument document, IDocument.FormatType formatType)
        {
            document = null;
            if (state == IDevice.State.off) return;

            ScanCounter++;
            string filename = formatType switch
            {
                IDocument.FormatType.PDF => $"PDFScan{ScanCounter}.pdf",
                IDocument.FormatType.TXT => $"TextScan{ScanCounter}.txt",
                _ => $"ImageScan{ScanCounter}.jpg",
            };

            document = formatType switch
            {
                IDocument.FormatType.PDF => new PDFDocument(filename),
                IDocument.FormatType.TXT => new TextDocument(filename),
                _ => new ImageDocument(filename),
            };

            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Scan: {document.GetFileName()}");
        }
    }

    public class Fax : IFax
    {
        private IDevice.State state = IDevice.State.off;
        public int FaxCounter { get; private set; } = 0;

        public void PowerOn() => state = IDevice.State.on;
        public void PowerOff() => state = IDevice.State.off;
        public IDevice.State GetState() => state;

        public void SendFax(in IDocument document)
        {
            if (state == IDevice.State.off) return;
            FaxCounter++;
            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Fax sent: {document.GetFileName()}");
        }
    }

    public class Copier : IDevice
    {
        private Printer printer = new Printer();
        private Scanner scanner = new Scanner();
        private IDevice.State state = IDevice.State.off;

        public Copier(Printer printer, Scanner scanner)
        {
            this.printer = printer;
            this.scanner = scanner;
        }
        public int Counter { get; private set; } = 0;

        public void PowerOn()
        {
            if (state == IDevice.State.off)
            {
                state = IDevice.State.on;
                printer.PowerOn();
                scanner.PowerOn();
                Counter++;
            }
        }

        public void PowerOff()
        {
            if (state == IDevice.State.on)
            {
                state = IDevice.State.off;
                printer.PowerOff();
                scanner.PowerOff();
            }
        }

        public IDevice.State GetState() => state;

        public void Print(in IDocument document) => printer.Print(document);

        public void Scan(out IDocument document, IDocument.FormatType formatType) =>
            scanner.Scan(out document, formatType);

        public void ScanAndPrint()
        {
            if (state == IDevice.State.off)
            {
                Console.WriteLine("Copier is off");
                return;
            }

            Scan(out IDocument doc, IDocument.FormatType.JPG);
            if (doc != null)
                Print(doc);
        }

        public int PrintCounter => printer.PrintCounter;
        public int ScanCounter => scanner.ScanCounter;
    }

    public class MultidimensionalDevice : IDevice, IPrinter, IScanner, IFax
    {
        private Printer printer = new Printer();
        private Scanner scanner = new Scanner();
        private Fax fax = new Fax();
        private IDevice.State state = IDevice.State.off;

        public int PowerOnCounter { get; private set; } = 0;

        public void PowerOn()
        {
            if (state == IDevice.State.off)
            {
                state = IDevice.State.on;
                printer.PowerOn();
                scanner.PowerOn();
                fax.PowerOn();
                PowerOnCounter++;
            }
        }

        public void PowerOff()
        {
            if (state == IDevice.State.on)
            {
                state = IDevice.State.off;
                printer.PowerOff();
                scanner.PowerOff();
                fax.PowerOff();
            }
        }

        public IDevice.State GetState() => state;

        public void Print(in IDocument document) => printer.Print(document);
        public void Scan(out IDocument document, IDocument.FormatType formatType) =>
            scanner.Scan(out document, formatType);
        public void SendFax(in IDocument document) => fax.SendFax(document);

        public int PrintCounter => printer.PrintCounter;
        public int ScanCounter => scanner.ScanCounter;
        public int FaxCounter => fax.FaxCounter;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var copier = new Copier(new Printer(), new Scanner());
            copier.PowerOn();

            IDocument doc1;
            copier.Scan(out doc1, IDocument.FormatType.TXT);
            copier.Print(in doc1);

            copier.ScanAndPrint();

            var multi = new MultidimensionalDevice();
            multi.PowerOn();

            IDocument faxDoc = new PDFDocument("fax1.pdf");
            multi.SendFax(faxDoc);
            multi.Scan(out faxDoc, IDocument.FormatType.PDF);
            multi.Print(in faxDoc);
        }
    }
}