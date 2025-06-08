using System;

namespace Zadanie5
{
    public interface IDevice
    {
        enum State { off, on, standby }
        State GetState();
        void SetState(State state);
        void PowerOn() => SetState(State.on);
        void PowerOff() => SetState(State.off);
        void StandbyOn() => SetState(State.standby);
        void StandbyOff() => SetState(State.on);
    }

    public interface IDocument
    {
        enum FormatType { TXT, PDF, JPG }
        string FileName { get; }
        FormatType GetFormatType();
    }

    public interface IPrinter : IDevice
    {
        int PrintCounter { get; }
        void Print(in IDocument document);
    }

    public interface IScanner : IDevice
    {
        int ScanCounter { get; }
        void Scan(out IDocument document, IDocument.FormatType formatType = IDocument.FormatType.JPG);
    }

    public class PDFDocument : IDocument
    {
        public string FileName { get; }
        public PDFDocument(string filename) => FileName = filename;
        public IDocument.FormatType GetFormatType() => IDocument.FormatType.PDF;
    }

    public class TextDocument : IDocument
    {
        public string FileName { get; }
        public TextDocument(string filename) => FileName = filename;
        public IDocument.FormatType GetFormatType() => IDocument.FormatType.TXT;
    }

    public class ImageDocument : IDocument
    {
        public string FileName { get; }
        public ImageDocument(string filename) => FileName = filename;
        public IDocument.FormatType GetFormatType() => IDocument.FormatType.JPG;
    }

    public class Printer : IPrinter
    {
        private IDevice.State _state = IDevice.State.off;
        private int _printCounter = 0;
        private int _printBatch = 0;

        public int PrintCounter => _printCounter;
        public IDevice.State GetState() => _state;
        public void SetState(IDevice.State state) => _state = state;

        public void Print(in IDocument document)
        {
            if (_state == IDevice.State.off) return;
            Console.WriteLine($"{DateTime.Now} Print: {document.FileName}");
            _printCounter++;
            _printBatch++;

            if (_printBatch >= 3)
            {
                SetState(IDevice.State.standby);
                _printBatch = 0;
            }
        }
    }

    public class Scanner : IScanner
    {
        private IDevice.State _state = IDevice.State.off;
        private int _scanCounter = 0;
        private int _scanBatch = 0;

        public int ScanCounter => _scanCounter;
        public IDevice.State GetState() => _state;
        public void SetState(IDevice.State state) => _state = state;

        public void Scan(out IDocument document, IDocument.FormatType formatType = IDocument.FormatType.JPG)
        {
            document = null;
            if (_state == IDevice.State.off) return;

            string name = $"Scan{_scanCounter + 1}";
            document = formatType switch
            {
                IDocument.FormatType.PDF => new PDFDocument(name + ".pdf"),
                IDocument.FormatType.TXT => new TextDocument(name + ".txt"),
                _ => new ImageDocument(name + ".jpg"),
            };

            Console.WriteLine($"{DateTime.Now} Scan: {document.FileName}");
            _scanCounter++;
            _scanBatch++;

            if (_scanBatch >= 2)
            {
                SetState(IDevice.State.standby);
                _scanBatch = 0;
            }
        }
    }

    public class Copier : IPrinter, IScanner
    {
        private readonly IPrinter _printer;
        private readonly IScanner _scanner;
        private IDevice.State _state = IDevice.State.off;

        public Copier(IPrinter printer, IScanner scanner)
        {
            _printer = printer;
            _scanner = scanner;
        }

        public IDevice.State GetState() => _state;
        public void SetState(IDevice.State state)
        {
            _state = state;
            _printer.SetState(state);
            _scanner.SetState(state);
        }

        public int PrintCounter => _printer.PrintCounter;
        public int ScanCounter => _scanner.ScanCounter;

        public void Print(in IDocument document) => _printer.Print(document);
        public void Scan(out IDocument document, IDocument.FormatType formatType = IDocument.FormatType.JPG)
            => _scanner.Scan(out document, formatType);

        public void ScanAndPrint()
        {
            Scan(out IDocument doc);
            Print(in doc);
        }
    }
    class Program
    {
        static void Main()
        {
            var printer = new Printer();
            var scanner = new Scanner();
            var copier = new Copier(printer, scanner);

            // Test
            copier.SetState(IDevice.State.on);
            copier.Print(new PDFDocument("doc1.pdf"));
            copier.Scan(out IDocument scannedDoc, IDocument.FormatType.PDF);
            copier.ScanAndPrint();

            Console.WriteLine($"Prints: {copier.PrintCounter}, Scans: {copier.ScanCounter}");
            copier.SetState(IDevice.State.standby);
            Console.WriteLine("État final: " + copier.GetState());
        }
    }
}