using ArtesianDataChecker.ClassObjects;
using ArtesianDataChecker.Configuration;
using ArtesianDataChecker.DataCheck;
using ArtesianDataChecker.Requests;
using ArtesianDataChecker.UserInterface;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ArtesianDataChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            int sel = 0;
            while (sel == 0)
            {
                Console.WriteLine("Select program: 1 - Compare Curves : 2 - Check Gaps : 3 - Compare Curves(versioned) : 4 - Check Gaps (versioned)");
                var inpt = Console.ReadLine();
                if (Regex.Match(inpt, @"\d").Success)
                {
                    sel = Convert.ToInt16(inpt);
                    if (sel == 1)
                        Compare();
                    else if (sel == 2)
                        Gaps();
                    else if (sel == 3)
                        CompareVersioned();
                    else if (sel == 4)
                        GapsVersioned();
                    else
                    {
                        Console.WriteLine("No program selected.");
                        sel = 0;
                    }
                }
                else
                    Console.WriteLine("Error: Invalid input.");
            }
            Console.WriteLine("\nPress key to exit program...");
            Console.ReadKey();
        }

        public static void Compare()
        {
            ArtesianDataChecker_Compare cmp = new ArtesianDataChecker_Compare();
            ArtesianDataChecker_Request req = new ArtesianDataChecker_Request();
            ArtesianDataChecker_Console csl = new ArtesianDataChecker_Console();
            ResultsContainer res = new ResultsContainer
            {
                Results = new List<CheckResult>()
            };
            //Paramaters param = csl.SetParametersDefault();
            List<string> providers = ArtesianDataChecker_Constants.AvailableProviders;
            Paramaters param = csl.SetParameters(providers, "compare");

            Stopwatch stopWatch = new Stopwatch();
            string reqTime = "";
            long totalTime = 0;
            Console.WriteLine("Starting check of " + param.TestIds.Count + " curves.");
            for (int i = 0; i < param.TestIds.Count; i++)
            {
                stopWatch.Start();
                var curveName = _getCurveName(param.TestIds[i], param.Provider1, req);
                var ProdId = _getIdFromMetadata(curveName, param.Provider2, req);
                var TestData = req.DataRequest(param.TestIds[i], param.Provider1, param.End, param.Start);
                var ProdData = req.DataRequest(ProdId, param.Provider2, param.End, param.Start);
                if (TestData != null && ProdData != null)
                {
                    var result = cmp.CompareData(TestData, ProdData);
                    res.Results.Add(result);
                    reqTime = stopWatch.ElapsedMilliseconds.ToString();
                    totalTime += stopWatch.ElapsedMilliseconds;
                    Console.WriteLine("Curve Checked : " + (i + 1) + "/" + param.TestIds.Count + " : " + result.ProdFileName + " | Time : " + reqTime);
                }
                else
                    Console.WriteLine("Error: Data request returned NULL.");
                stopWatch.Reset();
            }
            Console.WriteLine("Finished checking curves : Total time taken :" + (totalTime / 1000) + " sec");
            csl.PrintResultsToConsole(res.Results, param.Provider2);
        }

        public static void Gaps()
        {
            ArtesianDataChecker_GapCheck gap = new ArtesianDataChecker_GapCheck();
            ArtesianDataChecker_Request req = new ArtesianDataChecker_Request();
            ArtesianDataChecker_Console csl = new ArtesianDataChecker_Console();
            ResultsContainer res = new ResultsContainer()
            {
                GapResults = new List<GapResult>()
            };
            List<string> providers = ArtesianDataChecker_Constants.AvailableProviders;
            Paramaters param = csl.SetParameters(providers, "gaps");

            for (int i = 0; i < param.TestIds.Count; i++)
            {
                var data = req.DataRequest(param.TestIds[i], param.Provider1, param.Start, param.End);
                if (data != null)
                    res.GapResults.Add(gap.CheckIntervals(data));
                else
                    Console.WriteLine("Error: Data Request returned NULL.");
            }
            csl.PrintResultsToConsole(res.GapResults, param.Provider2);
        }

        public static void GapsVersioned()
        {
            ArtesianDataChecker_Console csl = new ArtesianDataChecker_Console();
            ArtesianDataChecker_Request req = new ArtesianDataChecker_Request();
            ArtesianDataChecker_GapCheck gap = new ArtesianDataChecker_GapCheck();
            ResultsContainer res = new ResultsContainer()
            {
                GapResults = new List<GapResult>()
            };
            List<string> providers = ArtesianDataChecker_Constants.AvailableProviders;
            Paramaters param = csl.SetParameters(providers, "gaps");
            //Paramaters param = csl.SetParametersDefault();
            foreach (var id in param.ProdIds)
            {
                var versions = req.DataRequestVersioned(id, param.Provider2, param.Start, param.End);
                foreach (var v in versions)
                {
                    res.GapResults.Add(gap.CheckIntervals(v));
                }
            }
            csl.PrintResultsToConsole(res.GapResults, param.Provider2);
        }

        public static void CompareVersioned()
        {
            // TODO:
            // - Implement method following pattern of Compare().
            // - This will require checking logic for versioned data to be written in ArtesianDataChecker_Compare class.
            Console.WriteLine("Not implemented");
        }

        public static string _getCurveName(int id, string provider, ArtesianDataChecker_Request req)
        {
            var today = LocalDate.FromDateTime(DateTime.Now);
            var d = req.DataRequest(id, provider, today.PlusDays(-3), today).GetEnumerator();
            d.MoveNext();
            return d.Current.CurveName;
        }

        public static int _getIdFromMetadata(string curveName, string provider, ArtesianDataChecker_Request req)
        {
            // Return the id of first row in curve.  
            return req.MetadataRequest(curveName, provider)[0].MarketDataId;
        }
    }
}
