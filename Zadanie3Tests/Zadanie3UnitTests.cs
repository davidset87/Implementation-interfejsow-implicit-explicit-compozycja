using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.IO;

using Zadanie3;



namespace Zadanie3Tests

{

    public class ConsoleRedirectionToStringWriter : IDisposable

    {

        private StringWriter stringWriter;

        private TextWriter originalOutput;



        public ConsoleRedirectionToStringWriter()

        {

            stringWriter = new StringWriter();

            originalOutput = Console.Out;

            Console.SetOut(stringWriter);

        }



        public string GetOutput() => stringWriter.ToString();



        public void Dispose()

        {

            Console.SetOut(originalOutput);

            stringWriter.Dispose();

        }

    }



    [TestClass]

    public class CopierTests

    {

        [TestMethod]

        public void Copier_PowerOnOff_Test()

        {

            var copier = new Copier(new Printer(), new Scanner());



            Assert.AreEqual(IDevice.State.off, copier.GetState());



            copier.PowerOn();

            Assert.AreEqual(IDevice.State.on, copier.GetState());



            copier.PowerOff();

            Assert.AreEqual(IDevice.State.off, copier.GetState());

        }



        [TestMethod]

        public void Copier_Print_DeviceOn()

        {

            var copier = new Copier(new Printer(), new Scanner());

            copier.PowerOn();



            using var consoleOutput = new ConsoleRedirectionToStringWriter();

            IDocument doc = new PDFDocument("file.pdf");



            copier.Print(in doc);



            string output = consoleOutput.GetOutput();

            Assert.IsTrue(output.Contains("Print"));

            Assert.IsTrue(output.Contains("file.pdf"));

        }



        [TestMethod]

        public void Copier_Scan_DeviceOn()

        {

            var copier = new Copier(new Printer(), new Scanner());

            copier.PowerOn();



            using var consoleOutput = new ConsoleRedirectionToStringWriter();

            copier.Scan(out IDocument doc, IDocument.FormatType.JPG);



            string output = consoleOutput.GetOutput();

            Assert.IsTrue(output.Contains("Scan"));

            Assert.IsNotNull(doc);

        }



        [TestMethod]

        public void Copier_ScanAndPrint_DeviceOn()

        {

            var copier = new Copier(new Printer(), new Scanner());

            copier.PowerOn();



            using var consoleOutput = new ConsoleRedirectionToStringWriter();

            copier.ScanAndPrint();



            string output = consoleOutput.GetOutput();

            Assert.IsTrue(output.Contains("Scan"));

            Assert.IsTrue(output.Contains("Print"));

        }



        [TestMethod]

        public void Copier_DeviceOff_NoOutput()

        {

            var copier = new Copier(new Printer(), new Scanner());

            copier.PowerOff();



            using var consoleOutput = new ConsoleRedirectionToStringWriter();

            copier.ScanAndPrint();



            string output = consoleOutput.GetOutput();

            Assert.IsFalse(output.Contains("Scan"));

            Assert.IsFalse(output.Contains("Print"));

        }



        [TestMethod]

        public void Copier_Counters_Test()

        {

            var copier = new Copier(new Printer(), new Scanner());



            copier.PowerOn();



            copier.Print(new PDFDocument("a.pdf"));

            copier.Print(new PDFDocument("b.pdf"));

            copier.Scan(out _, IDocument.FormatType.TXT);

            copier.Scan(out _, IDocument.FormatType.TXT);

            copier.ScanAndPrint();

            copier.PowerOff();

            copier.PowerOn();

            copier.ScanAndPrint();



            Assert.AreEqual(4, copier.PrintCounter);

            Assert.AreEqual(4, copier.ScanCounter);

            Assert.AreEqual(2, copier.Counter);

        }

    }
}