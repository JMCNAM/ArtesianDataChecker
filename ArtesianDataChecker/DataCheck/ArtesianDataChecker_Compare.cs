using Artesian.SDK.Dto;
using ArtesianDataChecker.ClassObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArtesianDataChecker.DataCheck
{
    class ArtesianDataChecker_Compare
    {
        public CheckResult CompareData(IEnumerable<TimeSerieRow.Actual> testData, IEnumerable<TimeSerieRow.Actual> prodData)
        {
            // Initial objects.
            CheckResult result = new CheckResult();

            // Initialize error counters
            int nullMatchTest = 0, nullMatchProd = 0, nullMatches = 0;
            int valMatches = 0, valMisMatch = 0;

            // Measure lengths of both data sets
            var en = testData.GetEnumerator();
            var lenTest = 0;
            while (en.MoveNext())
                lenTest++;

            en = prodData.GetEnumerator();
            var lenProd = 0;
            while (en.MoveNext())
                lenProd++;

            // Only proceed if data is equal.
            if (lenTest == lenProd)
            {
                // Extract values from both data sets.
                List<double?> testVals = new List<double?>();
                string testName = "";
                int testId = 0;
                foreach (var tval in testData)
                {
                    testVals.Add(tval.Value);
                    var temp = tval.CurveName;
                    testName = temp;
                    testId = tval.TSID;
                }
                List<double?> prodVals = new List<double?>();
                string prodName = "";
                int prodId = 0;
                foreach (var pval in prodData)
                {
                    prodVals.Add(pval.Value);
                    var temp = pval.CurveName;
                    prodName = temp;
                    prodId = pval.TSID;
                }
                // Only proceed if filenames match.
                if (testName == prodName)
                {
                    // Compare sets of values
                    for (int i = 0; i < testVals.Count; i++)
                    {
                        double? prodValue = prodVals[i];
                        double? testValue = testVals[i];

                        if (prodValue == null)
                        {
                            if (testValue == null)
                                nullMatches++;
                            else
                                nullMatchProd++;
                        }
                        else if (testValue == null)
                            nullMatchProd++;
                        else if (testValue == prodValue)
                            valMatches++;
                        else
                            valMisMatch++;
                    }
                }
                else
                {
                    result.ErrorNote = "Filename mismatch";
                }
                // Assign results to object.              
                result.TestFileId = testId;
                result.ProdFileId = prodId;
                result.ProdFileName = testName;
                result.TestFileName = prodName;

                result.NullMatched = nullMatches;
                result.NullProd = nullMatchProd;
                result.NullTest = nullMatchTest;
                result.Matched = valMatches;
                result.Mismatch = valMisMatch;

                if (result.ErrorNote == null)
                {
                    result.ErrorNote = "---";
                }            }
            return result;
        }

        public CheckResult ErrorResult(int testId, int prodId, int errId)
        {
            CheckResult error = new CheckResult();
            error.TestFileId = testId;
            error.ProdFileId = prodId;
            error.ErrorId = errId;
            error.ErrorNote = "Request fail";
            error.TestFileName = "---";
            error.ProdFileName = "---";
            return null;
        }

        public CheckResult CompareData(IEnumerable<TimeSerieRow.Versioned> testData, IEnumerable<TimeSerieRow.Versioned> prodData)
        {
            // TODO:
            throw new NotImplementedException();

            // Initial objects.
            CheckResult result = new CheckResult();

            // Initialize error counters
            int nullMatchTest = 0, nullMatchProd = 0, nullMatches = 0;
            int valMatches = 0, valMisMatch = 0;

            // Measure lengths of both data sets
            var en = testData.GetEnumerator();
            var lenTest = 0;
            while (en.MoveNext())
                lenTest++;

            en = prodData.GetEnumerator();
            var lenProd = 0;
            while (en.MoveNext())
                lenProd++;

            // Only proceed if data is equal.
            if (lenTest == lenProd)
            {
                // Extract values from both data sets.
                List<double?> testVals = new List<double?>();
                string testName = "";
                int testId = 0;
                foreach (var tval in testData)
                {
                    testVals.Add(tval.Value);
                    var temp = tval.CurveName;
                    testName = temp;
                    testId = tval.TSID;
                }
                List<double?> prodVals = new List<double?>();
                string prodName = "";
                int prodId = 0;
                foreach (var pval in prodData)
                {
                    prodVals.Add(pval.Value);
                    var temp = pval.CurveName;
                    prodName = temp;
                    prodId = pval.TSID;
                }
                // Only proceed if filenames match.
                if (testName == prodName)
                {
                    // Compare sets of values
                    for (int i = 0; i < testVals.Count; i++)
                    {
                        double? prodValue = prodVals[i];
                        double? testValue = testVals[i];

                        if (prodValue == null)
                        {
                            if (testValue == null)
                                nullMatches++;
                            else
                                nullMatchProd++;
                        }
                        else if (testValue == null)
                            nullMatchProd++;
                        else if (testValue == prodValue)
                            valMatches++;
                        else
                            valMisMatch++;
                    }
                }
                else
                {
                    result.ErrorNote = "Filename mismatch";
                }
                // Assign results to object.              
                result.TestFileId = testId;
                result.ProdFileId = prodId;
                result.ProdFileName = testName;
                result.TestFileName = prodName;

                result.NullMatched = nullMatches;
                result.NullProd = nullMatchProd;
                result.NullTest = nullMatchTest;
                result.Matched = valMatches;
                result.Mismatch = valMisMatch;

                if (result.ErrorNote == null)
                {
                    result.ErrorNote = "---";
                }
            }
            return result;
        }
    }
}
