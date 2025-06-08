using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zadanie1;
using System.IO;
using System;

namespace Zadanie2Tests
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

        public string GetOutput()
        {
            return stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(originalOutput);
            stringWriter.Dispose();
        }
    }

    [TestClass]
    public class UnitTestMultifunctionalDevice
    {
        [TestMethod]
        public void Device_ScanSendReceivePrint()
        {
            var device = new MultifunctionalDevice();
            device.PowerOn();

            using (var console = new ConsoleRedirectionToStringWriter())
            {
                device.Scan(out var doc1);
                device.Print(in doc1);
                device.Send(in doc1);
                device.Receive(out var received);

                string output = console.GetOutput();

                Assert.IsTrue(output.Contains("Scan"));
                Assert.IsTrue(output.Contains("Print"));
                Assert.IsTrue(output.Contains("Fax sent"));
                Assert.IsTrue(output.Contains("Fax received"));
                Assert.IsNotNull(doc1);
                Assert.IsNotNull(received);
            }
        }

        [TestMethod]
        public void Counters_AreCorrect()
        {
            var device = new MultifunctionalDevice();
            device.PowerOn();

            device.Scan(out var doc1);
            device.Print(in doc1);
            device.Send(in doc1);
            device.Receive(out var doc2);
            device.Send(in doc1);

            Assert.AreEqual(1, device.Counter, "Power-on counter should be 1");
            Assert.AreEqual(1, device.ScanCounter, "Scan count should be 1");
            Assert.AreEqual(1, device.PrintCounter, "Print count should be 1");
            Assert.AreEqual(2, device.FaxSendCounter, "Fax send count should be 2");
            Assert.AreEqual(1, device.FaxReceiveCounter, "Fax receive count should be 1");
        }

        [TestMethod]
        public void Device_Off_NoActions()
        {
            var device = new MultifunctionalDevice();
            device.PowerOff();

            using (var console = new ConsoleRedirectionToStringWriter())
            {
                device.Scan(out var doc1);
                device.Print(in doc1);
                device.Send(in doc1);
                device.Receive(out var doc2);

                string output = console.GetOutput();
                Assert.AreEqual("", output.Trim(), "No output should occur when device is off.");
            }

            Assert.AreEqual(0, device.PrintCounter);
            Assert.AreEqual(0, device.ScanCounter);
            Assert.AreEqual(0, device.FaxSendCounter);
            Assert.AreEqual(0, device.FaxReceiveCounter);
        }
    }
}