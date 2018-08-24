using ArtesianDataChecker.ClassObjects;
using NodaTime;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ArtesianDataChecker.UserInterface
{
    class ArtesianDataChecker_Console
    {
        public Paramaters SetParameters(List<string> providers, string program)
        {
            if (program == "compare")
                return _setParamatersForCompare(providers);
            else if (program == "gaps")
                return _setParamatersForGaps(providers);
            else
                return null;
        }

        public Paramaters SetParametersDefault()
        { // Use for debugging to auto-set parameters.
            Paramaters param = new Paramaters();
            var TestIds = new List<int>() { 100001246 };
            var ProdIds = new List<int>() { 100021005 };
            var pvdr1 = "Test";
            var pvdr2 = "ProdDemo";

            param.TestIds = TestIds;
            param.ProdIds = ProdIds;
            param.Provider1 = pvdr1;
            param.Provider2 = pvdr2;
            param.Start = LocalDate.FromDateTime(DateTime.Now.AddDays(-3));
            param.End = LocalDate.FromDateTime(DateTime.Now.AddDays(-2));

            return param;
        }

        public void PrintResultsToConsole(List<CheckResult> results, string provider)
        {
            if (!results[0].Versioned)
            {
                string[] headers = { "Development File", "ID", "Test File", "ID", "Match", "Null Match", "Null Dev", "Null Test", "Mismatch", "Error ID", "Error Note" };
                Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|");
                Console.WriteLine(String.Format("| {0, -50} | {1, -10} | {2, -50} | {3, -10} | {4, -10} | {5,  10} | {6,  10} | {7,  10} | {8,  10} | {9,  10} | {10,  -20} |",
                    headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10]));
                Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|");

                foreach (var result in results)
                {
                    if (result.ProdFileName != null && result.TestFileName != null)
                    {
                        if (result.ProdFileName.Length > 50)
                            result.ProdFileName = result.ProdFileName.Substring(0, 45) + "...";
                        if (result.TestFileName.Length > 50)
                            result.TestFileName = result.TestFileName.Substring(0, 45) + "...";

                        Console.WriteLine(String.Format("| {0, -50} | {1,  10} | {2, -50} | {3, 10} | {4, -10} | {5,  10} | {6,  10} | {7,  10} | {8,  10} | {9,  10} | {10,  -20} |",
                            result.ProdFileName,
                            result.ProdFileId,
                            result.TestFileName,
                            result.TestFileId,
                            result.Matched,
                            result.NullMatched,
                            result.NullProd,
                            result.NullTest,
                            result.Mismatch,
                            result.ErrorId,
                            result.ErrorNote));
                    }
                }
                Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|");
                
                _writeToCSVFile(results, provider);
            }
            else
            {
                Console.WriteLine("Versioned data checker not implemented");
            }
        }

        public void PrintResultsToConsole(List<GapResult> results, string provider)
        {
            if (results[0].Versioned)
            {
                string[] headers = { "File", "Version", "ID", "Interval", "Number of Records", "Gap Indicies" };
                Console.WriteLine("|--------------------------------------------------------------------------------------------------------------------------------------------------------------|");
                Console.WriteLine(String.Format("| {0, -50} | {1, -20} | {2, -10} | {3, -20} | {4, -20} | {5, -10}          |",
                    headers[0], headers[1], headers[2], headers[3], headers[4], headers[5]));
                Console.WriteLine("|--------------------------------------------------------------------------------------------------------------------------------------------------------------|");

                foreach (var result in results)
                {
                    string indicies = "";
                    foreach (var index in result.GapIndices)
                    {
                        indicies += index + " ";
                    }
                    if (result.FileName != null)
                    {
                        if (result.FileName.Length > 50)
                            result.FileName = result.FileName.Substring(0, 45) + "...";

                        Console.WriteLine(String.Format("| {0, -50} | {1, -20} | {2, -10} | {3, -20} | {4, -20} | {5, -10}            |",
                            result.FileName,
                            result.Version,
                            result.FileId,
                            result.Interval,
                            result.NumRecords,
                            indicies));
                    }
                }
                Console.WriteLine("|--------------------------------------------------------------------------------------------------------------------------------------------------------------|");
                _writeToCSVFile(results, provider);
            }
            else
            {
                string[] headers = { "File", "ID", "Interval", "Number of Records", "Gap Indicies" };
                Console.WriteLine("|-----------------------------------------------------------------------------------------------------------------------------------------------|");
                Console.WriteLine(String.Format("| {0, -50} | {1, -10} | {2, -20} | {3, -20} | {4, -10}                  |",
                    headers[0], headers[1], headers[2], headers[3], headers[4]));
                Console.WriteLine("|-----------------------------------------------------------------------------------------------------------------------------------------------|");

                foreach (var result in results)
                {
                    string indicies = "";
                    foreach (var index in result.GapIndices)
                    {
                        indicies += index + " ";
                    }
                    if (result.FileName != null)
                    {
                        if (result.FileName.Length > 50)
                            result.FileName = result.FileName.Substring(0, 45) + "...";

                        Console.WriteLine(String.Format("| {0, -50} | {1,  10} | {2, -20} | {3, -20} | {4, 10}                    |",
                            result.FileName,
                            result.FileId,
                            result.Interval,
                            result.NumRecords,
                            indicies));
                    }
                }
                Console.WriteLine("|-----------------------------------------------------------------------------------------------------------------------------------------------|");
            }
        }

        private Paramaters _setParamatersForCompare(List<string> providers)
        {
            Console.WriteLine("| Artesian Data Checker |\n- Compare contents of a provider database with the test database.\n");
            Paramaters param = new Paramaters();
            param.Provider1 = "Test";
            Console.WriteLine("Available providers : ");
            string inpt;
            // Print available providers
            int idx = 1;
            foreach (var provider in providers)
            {
                if (provider != "Test")
                {
                    Console.WriteLine(idx + "  " + provider + " ");
                    idx++;
                }
            }
            // Recieve provider from user.
            while (param.Provider2 == null)
            {
                Console.WriteLine("\nEnter provider name: ");
                inpt = Console.ReadLine();
                if (providers.Contains(inpt))
                    param.Provider2 = inpt;
                else if (Regex.Match(inpt, @"\d").Success)
                {
                    if (Convert.ToInt32(inpt) <= providers.Count && Convert.ToInt32(inpt) != 0)
                    {
                        param.Provider2 = providers[Convert.ToInt32(inpt)];
                        Console.WriteLine("Selected provider : " + param.Provider2);
                    }
                    else
                        Console.WriteLine("ERROR : Provider given is not available.\n");
                }
                else
                    Console.WriteLine("ERROR : Input is not valid.\n");
            }
            // Recieve Id's for first provider.
            while (param.TestIds == null)
            {
                Console.WriteLine("\nEnter list of Id's from test (comma separated)");
                inpt = _readLine();
                if (Regex.Match(inpt, @"((?!=^|,)([0-9]))").Success)
                    param.TestIds = _readIdsFromConsole(inpt);
                else
                    Console.WriteLine("ERROR : Id's must a comma-separated string of numerals.\n");
            }
            // Recieve date range.
            while (param.Start.Year == 0001)
            {
                Console.WriteLine("\nEnter number of days back from today to request: (From three days ago, enter '3')");
                var d = Console.ReadLine();
                if (Regex.Match(d, @"\d").Success)
                {
                    param.Start = LocalDate.FromDateTime(DateTime.Now);
                    param.End = LocalDate.FromDateTime(DateTime.Now.AddDays(-Convert.ToInt32(d)));
                }
                else
                    Console.WriteLine("ERROR : Range must a single numerical value.\n");
            }
            return param;
        }

        private Paramaters _setParamatersForGaps(List<string> providers)
        {
            Console.WriteLine("| Artesian Gap Checker |\n- Identifes gaps in data table.\n");           
            Console.WriteLine("Available providers : ");
            Paramaters param = new Paramaters();
            param.Provider1 = "Test";
            string inpt;
            // Print available providers
            int idx = 1;
            foreach (var provider in providers)
            {
                Console.WriteLine(idx + "  " + provider + " ");
                idx++;
            }
            // Recieve provider
            while (param.Provider2 == null)
            {
                Console.WriteLine("\nEnter provider name: ");
                inpt = Console.ReadLine();
                if (providers.Contains(inpt))
                    param.Provider2 = inpt;
                else if (Regex.Match(inpt, @"\d").Success)
                {
                    if (Convert.ToInt32(inpt) <= providers.Count && Convert.ToInt32(inpt) != 0)
                    {
                        param.Provider2 = providers[Convert.ToInt32(inpt)-1];
                        Console.WriteLine("Selected provider : " + param.Provider2);
                    }
                    else
                        Console.WriteLine("ERROR : Provider given is not available.\n");
                }
                else
                    Console.WriteLine("ERROR : Input is not valid.\n");
            }
            // Recieve Id's for first provider.
            while (param.TestIds == null)
            {
                Console.WriteLine("\nEnter list of Id's from test (comma separated)");
                inpt = Console.ReadLine();
                if (Regex.Match(inpt, @"((?!=^|,)([0-9]))").Success)
                    param.TestIds = _readIdsFromConsole(inpt);
                else
                    Console.WriteLine("ERROR : Id's must a comma-separated string of numerals.\n");
            }
            // Recieve date range.
            while (param.Start.Year == 0001)
            {
                Console.WriteLine("\nEnter number of days back from today to request: (From three days ago, enter '3')");
                var d = Console.ReadLine();
                if (Regex.Match(d, @"\d").Success)
                {
                    param.Start = LocalDate.FromDateTime(DateTime.Now);
                    param.End = LocalDate.FromDateTime(DateTime.Now.AddDays(-Convert.ToInt32(d)));
                }
                else
                    Console.WriteLine("ERROR : Range must a single numerical value.\n");
            }
            return param;
        }

        static List<int> _readIdsFromConsole(string stringIds)
        {
            string[] ids = stringIds.Split(',');
            List<int> idList = new List<int>();

            foreach (string id in ids)
            {
                idList.Add(Convert.ToInt32(id));
            }
            return idList;
        }


        public void _writeToCSVFile(List<CheckResult> results, string provider)
        {
            // File is written to solution folder.
            string[] headers = { "Development File", "DevID", "Test File", "TestID", "Match", "Null Match", "Null Dev", "Null Test", "Mismatch", "Error ID", "Error Note" };
            string path = @"../../../../CheckResults_" + provider + "_" + DateTime.Now.ToLongDateString() + ".csv";
            using (StreamWriter file = new StreamWriter(path))
            {
                foreach (var header in headers)
                {
                    file.Write(header + ";");
                }
                file.Write("\n");
                foreach (var result in results)
                {
                    file.Write(result.ProdFileName + ";");
                    file.Write(result.ProdFileId + ";");
                    file.Write(result.TestFileName + ";");
                    file.Write(result.TestFileId + ";");
                    file.Write(result.Matched + ";");
                    file.Write(result.NullMatched + ";");
                    file.Write(result.NullProd + ";");
                    file.Write(result.NullTest + ";");
                    file.Write(result.Mismatch + ";");
                    file.Write(result.ErrorId + ";");
                    file.Write(result.ErrorNote + ";");
                    file.Write("\n");
                }
                
            }
        }

        public void _writeToCSVFile(List<GapResult> results, string provider)
        {
            string[] headers = { "File", "ID", "Interval", "Number of Records", "Gap Indicies" };
            string path = @"../../../../CheckResults_" + provider + "_" + DateTime.Now.ToLongDateString() + ".csv";
            using (StreamWriter file = new StreamWriter(path))
            {
                foreach (var header in headers)
                {
                    file.Write(header + ";");
                }
                file.Write("\n");
                foreach (var result in results)
                {
                    string indicies = "";
                    foreach (var index in result.GapIndices)
                    {
                        indicies += index + " ";
                    }
                    file.Write(result.FileName + ";");
                    file.Write(result.FileId + ";");
                    file.Write(result.Interval + ";");
                    file.Write(result.NumRecords + ";");
                    file.Write(indicies + ";");
                }
                file.Write("\n");
            }
        }

        private static string _readLine()
        { // For reading large numbers of id's into console.
            var READLINE_BUFFER_SIZE = 2048;
            Stream inputStream = Console.OpenStandardInput(READLINE_BUFFER_SIZE);
            byte[] bytes = new byte[READLINE_BUFFER_SIZE];
            int outputLength = inputStream.Read(bytes, 0, READLINE_BUFFER_SIZE);
            //Console.WriteLine(outputLength);
            char[] chars = Encoding.UTF7.GetChars(bytes, 0, outputLength);
            return new string(chars);
        }

        //public void _writeToTextFile(List<CheckResult> results)
        //{
        //    FileStream ostrm;
        //    StreamWriter writer;
        //    TextWriter oldOut = Console.Out;
        //    try
        //    {
        //        ostrm = new FileStream("./Redirect.txt", FileMode.OpenOrCreate, FileAccess.Write);
        //        writer = new StreamWriter(ostrm);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Cannot open Redirect.txt for writing");
        //        Console.WriteLine(e.Message);
        //        return;
        //    }
        //    Console.SetOut(writer);


        //    string[] headers = { "Development File", "ID", "Test File", "ID", "Match", "Null Match", "Null Dev", "Null Test", "Mismatch", "Error ID", "Error Note" };
        //    Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|");
        //    Console.WriteLine(String.Format("| {0, -50} | {1, -10} | {2, -50} | {3, -10} | {4, -10} | {5,  10} | {6,  10} | {7,  10} | {8,  10} | {9,  10} | {10,  -20} |",
        //        headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10]));
        //    Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|");

        //    foreach (var result in results)
        //    {
        //        if (result.ProdFileName != null && result.TestFileName != null)
        //        {
        //            if (result.ProdFileName.Length > 50)
        //                result.ProdFileName = result.ProdFileName.Substring(0, 45) + "...";
        //            if (result.TestFileName.Length > 50)
        //                result.TestFileName = result.TestFileName.Substring(0, 45) + "...";

        //            Console.WriteLine(String.Format("| {0, -50} | {1,  10} | {2, -50} | {3, 10} | {4, -10} | {5,  10} | {6,  10} | {7,  10} | {8,  10} | {9,  10} | {10,  -20} |",
        //                result.ProdFileName,
        //                result.ProdFileId,
        //                result.TestFileName,
        //                result.TestFileId,
        //                result.Matched,
        //                result.NullMatched,
        //                result.NullProd,
        //                result.NullTest,
        //                result.Mismatch,
        //                result.ErrorId,
        //                result.ErrorNote));
        //        }
        //    }
        //    Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|");
        //    Console.SetOut(oldOut);
        //    writer.Close();
        //    ostrm.Close();
        //    Console.WriteLine("Done");
        //}

    }
}
