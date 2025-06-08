using System;

namespace Zadanie1
{
    public class MultifunctionalDevice : IPrinter, IScanner, IFax
    {
        private IDevice.State state = IDevice.State.off;
        public IDevice.State GetState() => state;

        public int Counter { get; private set; } = 0;
        public int PrintCounter { get; private set; } = 0;
        public int ScanCounter { get; private set; } = 0;
        public int FaxSendCounter { get; private set; } = 0;
        public int FaxReceiveCounter { get; private set; } = 0;

        public void PowerOn()
        {
            if (state == IDevice.State.off)
            {
                state = IDevice.State.on;
                Counter++;
            }
        }

        public void PowerOff()
        {
            if (state == IDevice.State.on)
            {
                state = IDevice.State.off;
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

            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Scan: {document.GetFileName()}");
        }

        public void Send(in IDocument document)
        {
            if (state != IDevice.State.on) return;

            FaxSendCounter++;
            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Fax sent: {document.GetFileName()}");
        }

        public void Receive(out IDocument document)
        {
            document = null;
            if (state != IDevice.State.on) return;

            FaxReceiveCounter++;
            string fileName = $"FaxReceived{FaxReceiveCounter}.pdf";
            document = new PDFDocument(fileName);

            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} Fax received: {document.GetFileName()}");
        }
    }
}