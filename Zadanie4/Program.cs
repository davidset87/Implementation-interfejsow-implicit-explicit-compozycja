using System;

namespace Zadanie4
{
    public interface IDocument
    {
        enum FormatType { TXT, PDF, JPG }

        string FileName { get; }
        FormatType GetFormatType();
    }

    public class PDFDocument : IDocument
    {
        public string FileName { get; private set; }
        public PDFDocument(string filename) => FileName = filename;
        public IDocument.FormatType GetFormatType() => IDocument.FormatType.PDF;
    }

    public class TextDocument : IDocument
    {
        public string FileName { get; private set; }
        public TextDocument(string filename) => FileName = filename;
        public IDocument.FormatType GetFormatType() => IDocument.FormatType.TXT;
    }

    public class ImageDocument : IDocument
    {
        public string FileName { get; private set; }
        public ImageDocument(string filename) => FileName = filename;
        public IDocument.FormatType GetFormatType() => IDocument.FormatType.JPG;
    }

    public interface IDevice
    {
        enum State { off, on, standby }
        State GetState();
        void SetState(State state);

        void PowerOn()
        {
            SetState(State.on);
        }

        void PowerOff()
        {
            SetState(State.off);
        }

        void StandbyOn()
        {
            SetState(State.standby);
        }

        void StandbyOff()
        {
            SetState(State.on);
        }
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

    public class Copier : IPrinter, IScanner
    {
        private int _printCounter = 0;
        private int _scanCounter = 0;
        private int _powerOnCounter = 0;

        private IDevice.State printerState = IDevice.State.off;
        private IDevice.State scannerState = IDevice.State.off;

        private int printJobBatch = 0;
        private int scanJobBatch = 0;

        public int PrintCounter => _printCounter;
        public int ScanCounter => _scanCounter;
        public int Counter => _powerOnCounter;

        public IDevice.State GetState()
        {
            if (printerState == IDevice.State.off && scannerState == IDevice.State.off)
                return IDevice.State.off;
            if (printerState == IDevice.State.standby && scannerState == IDevice.State.standby)
                return IDevice.State.standby;
            return IDevice.State.on;
        }

        public void SetState(IDevice.State state)
        {
            printerState = state;
            scannerState = state;

            if (state == IDevice.State.on)
                _powerOnCounter++;
        }

        public void Print(in IDocument document)
        {
            if (printerState == IDevice.State.off) return;

            if (scannerState == IDevice.State.on)
                scannerState = IDevice.State.standby;

            Console.WriteLine($"{DateTime.Now} Print: {document.FileName}");
            _printCounter++;
            printJobBatch++;

            if (printJobBatch == 3)
            {
                printerState = IDevice.State.standby;
                printJobBatch = 0;
            }
        }

        public void Scan(out IDocument document, IDocument.FormatType formatType = IDocument.FormatType.JPG)
        {
            document = null;
            if (scannerState == IDevice.State.off) return;

            if (printerState == IDevice.State.on)
                printerState = IDevice.State.standby;

            string name = $"Scan{_scanCounter + 1}";
            document = formatType switch
            {
                IDocument.FormatType.PDF => new PDFDocument(name + ".pdf"),
                IDocument.FormatType.TXT => new TextDocument(name + ".txt"),
                _ => new ImageDocument(name + ".jpg"),
            };

            Console.WriteLine($"{DateTime.Now} Scan: {document.FileName}");
            _scanCounter++;
            scanJobBatch++;

            if (scanJobBatch == 2)
            {
                scannerState = IDevice.State.standby;
                scanJobBatch = 0;
            }
        }

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
            var copier = new Copier();
            IDevice device = copier;

            device.PowerOn();
            copier.Print(new PDFDocument("test1.pdf"));
            copier.Print(new TextDocument("test2.txt"));
            copier.Print(new ImageDocument("test3.jpg"));
            copier.Print(new PDFDocument("test4.pdf"));

            copier.Scan(out IDocument doc1);
            copier.Scan(out IDocument doc2);
            copier.Scan(out IDocument doc3);

            device.StandbyOn();
            device.PowerOff();
            Console.WriteLine("State after standby: " + copier.GetState());

            device.PowerOff();
            Console.WriteLine("Final state: " + copier.GetState());
        }
    }
}