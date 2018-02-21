using Microsoft.Extensions.Configuration.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace NetCoreConfigTest.Py
{
    class PyConfigurationProvider : JsonConfigurationProvider
    {
        private static readonly string Helper = PyConfigurationResources.Helper;

        public PyConfigurationProvider(PyConfigurationSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            using (var process = Process.Start(new ProcessStartInfo
                                               {
                                                   FileName = "python",
                                                   Arguments = "-c \"import sys; exec(sys.stdin.read())\"",
                                                   CreateNoWindow = true,
                                                   UseShellExecute = false,
                                                   RedirectStandardInput = true,
                                                   RedirectStandardOutput = true,
                                                   RedirectStandardError = true
                                               }))
            {
                var data = new StreamReader(stream).ReadToEnd();
                var dataExpr = Helper.Replace("\"$$$___$$$\"", data);
                process.StandardInput.Write(dataExpr);
                process.StandardInput.Close();
                process.WaitForExit();
                var error = process.StandardError.ReadToEnd();
                if (!String.IsNullOrWhiteSpace(error))
                    throw new FormatException(error);

                using (var jsonStream = new MemoryStream())
                {
                    var jsonWriter = new StreamWriter(jsonStream);
                    var dataJson = process.StandardOutput.ReadToEnd();
                    jsonWriter.Write(dataJson);
                    jsonWriter.Flush();
                    jsonStream.Position = 0;
                    base.Load(jsonStream);
                }
            }
        }
    }
}
