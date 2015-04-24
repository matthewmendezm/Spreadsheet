using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnitTest
{
    [TestClass]
    public class Tester
    {
        [TestMethod]
        public void CellsTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "3");
            sheet.SetContentsOfCell("B1", "=A1 * A1");
            sheet.SetContentsOfCell("C1", "winning ");
            sheet.SetContentsOfCell("D1", "=B1 - C1");
            HashSet<String> cellkeys = new HashSet<String>(sheet.GetNamesOfAllNonemptyCells());
            Assert.IsTrue(cellkeys.Count == 4 && cellkeys.Contains("A1") && cellkeys.Contains("B1") && cellkeys.Contains("C1") && cellkeys.Contains("D1"));

            Assert.IsTrue(sheet.GetCellContents("C1") is string);
            Assert.IsTrue((sheet.GetCellContents("C1") as string) == "winning ");

            Assert.IsTrue(sheet.GetCellContents("A1") is Double);
        }

        /*
        // For example, suppose that A1 contains 3 B1 contains the formula A1 * A1 C1 contains the formula B1 + A1 D1
        // contains the formula B1 - C1 The direct dependents of A1 are B1 and C1
        [TestMethod]
        public void DependantsTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", 3);
            sheet.SetContentsOfCell("B1", new FormulaFixed("A1 * A1"));
            sheet.SetContentsOfCell("C1", new FormulaFixed("B1 + A1"));
            sheet.SetContentsOfCell("D1", new FormulaFixed("B1 - C1"));
            HashSet<String> cellkeys = new HashSet<String>(sheet.GetDirectDependents("A1"));

            Assert.IsTrue(cellkeys.Count == 2 && cellkeys.Contains("B1") && cellkeys.Contains("C1"));
        }
         * */

        [TestMethod]
        public void GetContentsTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "3");
            sheet.SetContentsOfCell("B1", "=5 * A1");
            sheet.SetContentsOfCell("A1", "=D1 + 7");
            sheet.SetContentsOfCell("D1", "=23 - 10");
            object contents = sheet.GetCellContents("A1");

            Assert.IsTrue(contents is FormulaFixed);
            Assert.IsTrue((contents as FormulaFixed).ToString() == "D1+7");
        }

        [TestMethod]
        public void EmptyCellTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "3");
            sheet.SetContentsOfCell("B1", "=5 * A1");
            sheet.SetContentsOfCell("A1", "=D1 + 7");
            sheet.SetContentsOfCell("D1", "=23 - 10");
            object contents = sheet.GetCellContents("a1");

            Assert.IsTrue(contents is string);
            Assert.IsTrue((contents as string) == "");
        }

        // For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the set {A1, B1, C1} is returned.
        [TestMethod]
        public void SetCellTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("B1", "=A1*2");
            sheet.SetContentsOfCell("C1", "=B1+A1");

            HashSet<String> cellReturn = new HashSet<String>(sheet.SetContentsOfCell("A1", "3"));
            Assert.IsTrue(cellReturn.Count == 3 && cellReturn.Contains("A1") && cellReturn.Contains("B1") && cellReturn.Contains("C1"));
        }

        // Spreadsheets are never allowed to contain a combination of Formulas that establish a circular dependency. A
        // circular dependency exists when a cell depends on itself. For example, suppose that A1 contains B1*2, B1 contains
        // C1*2, and C1 contains A1*2. A1 depends on B1, which depends on C1, which depends on A1. That's a circular
        // dependency.
        /*
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularDependancyTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1 * 2");
            sheet.SetContentsOfCell("B1", "=C1 * 2");
            sheet.SetContentsOfCell("C1", "=A1 * 2");
        }
         */

        [TestMethod]
        public void CircularDependancyRestoreTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1 * 2");
            sheet.SetContentsOfCell("B1", "=C1 * 2");
            try
            {
                sheet.SetContentsOfCell("C1", "=A1 * 2");
            }
            catch
            {
                HashSet<String> cellkeys = new HashSet<String>(sheet.GetNamesOfAllNonemptyCells());
                Assert.IsTrue(cellkeys.Count == 2 && cellkeys.Contains("A1") && cellkeys.Contains("B1"));
            }
        }

        [TestMethod]
        public void CircularDependancyRestoreOldFuncTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1 * 2");
            sheet.SetContentsOfCell("B1", "=C1 * 2");
            sheet.SetContentsOfCell("C1", "=D1 * 2");
            try
            {
                sheet.SetContentsOfCell("C1", "=A1 * 2");
            }
            catch
            {
                HashSet<String> cellkeys = new HashSet<String>(sheet.GetNamesOfAllNonemptyCells());
                Assert.IsTrue(cellkeys.Count == 3 && cellkeys.Contains("A1") && cellkeys.Contains("B1") && cellkeys.Contains("C1"));
                object contents = sheet.GetCellContents("C1");

                Assert.IsTrue(contents is FormulaFixed);
                Assert.IsTrue((contents as FormulaFixed).ToString() == "D1*2");
            }
        }

        [TestMethod]
        public void SetCellReturnTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("B1", "=A1*2");
            sheet.SetContentsOfCell("C1", "=B1+A1");
            sheet.SetContentsOfCell("D1", "=C1+13");
            sheet.SetContentsOfCell("E1", "=H1+13");
            HashSet<String> cellReturn = new HashSet<String>(sheet.SetContentsOfCell("A1", "3"));
            Assert.IsTrue(cellReturn.Count == 4 && cellReturn.Contains("A1") && cellReturn.Contains("B1") && cellReturn.Contains("C1") && cellReturn.Contains("D1"));
        }

        [TestMethod]
        public void SetNonNullFunctionCellStringTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("B1", "=A1*2");
            sheet.SetContentsOfCell("C1", "=B1+A1");
            sheet.SetContentsOfCell("D1", "=C1+13");
            sheet.SetContentsOfCell("E1", "=H1+13");
            HashSet<String> cellReturn = new HashSet<String>(sheet.SetContentsOfCell("B1", "hello"));
            Assert.IsTrue(cellReturn.Count == 3 && cellReturn.Contains("B1") && cellReturn.Contains("C1") && cellReturn.Contains("D1"));
        }

        [TestMethod]
        public void SetNonNullFunctionCellDoubleTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("B1", "=A1*2");
            sheet.SetContentsOfCell("C1", "=B1+A1");
            sheet.SetContentsOfCell("D1", "=C1+13");
            sheet.SetContentsOfCell("E1", "=H1+13");
            HashSet<String> cellReturn = new HashSet<String>(sheet.SetContentsOfCell("B1", "5"));
            Assert.IsTrue(cellReturn.Count == 3 && cellReturn.Contains("B1") && cellReturn.Contains("C1") && cellReturn.Contains("D1"));

            Assert.IsTrue(sheet.GetCellContents("B1") is Double);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullStringTest()
        {
            string nullstring = null;
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", nullstring);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFormulaTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName1Test()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("$1", "5");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName2Test()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("4A55", "hello");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName3Test()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("^A55", "H1+13");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName4Test()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("^A55");
        }

        [TestMethod]
        public void GetValueTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("A1", "2");
            sheet.SetContentsOfCell("B1", "=5+A1");
            sheet.SetContentsOfCell("C1", "=B1+3");
            Assert.IsTrue((double)sheet.GetCellValue("C1") == 10);

            sheet = new Spreadsheet();

            sheet.SetContentsOfCell("C1", "=B1+3");
            sheet.SetContentsOfCell("A1", "3");
            Assert.IsTrue(sheet.GetCellValue("C1") is FormulaError);
            sheet.SetContentsOfCell("B1", "=5+A1");
            Assert.IsTrue((double)sheet.GetCellValue("C1") == 11);
            Assert.IsTrue((string)sheet.GetCellValue("H1") == "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidVarTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(isVal, norm, "default");

            sheet.SetContentsOfCell("A0", "2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidVarEquationTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(isVal, norm, "default");

            sheet.SetContentsOfCell("A1", "=2 - A0");
        }

        [TestMethod]
        public void SaveTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(isVal, norm, "default");

            sheet.SetContentsOfCell("A1", "2");
            sheet.SetContentsOfCell("B1", "=5+A1");
            sheet.SetContentsOfCell("C1", "=B1+3");
            sheet.SetContentsOfCell("D1", "hello");
            sheet.Save("myspreadsheet1.xml");

            sheet = new Spreadsheet("myspreadsheet1.xml", isVal, norm, "default");
            Assert.IsTrue((double)sheet.GetCellValue("C1") == 10);
            HashSet<String> cellkeys = new HashSet<String>(sheet.GetNamesOfAllNonemptyCells());
            Assert.IsTrue(cellkeys.Count == 4 && cellkeys.Contains("A1") && cellkeys.Contains("B1") && cellkeys.Contains("C1") && cellkeys.Contains("D1"));
        }

        public static string norm(String var)
        {
            return var.ToUpper();
        }

        public static bool isVal(String var)
        {
            if (Regex.IsMatch(var, "(^[a-zA-Z]+[1-9]+[0-9]*$)"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}