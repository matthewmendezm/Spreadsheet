using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// this class is used to represent a single cell in a spreadsheet.
    /// </summary>
    class Cell
    {

        private object CellContentsInternal;
        public object CellValue;
        private Func<string, double> lookup;
        //private object CellValueInternal;

        /// <summary>
        /// the constructor for the cell, it sets up the member variables
        /// </summary>
        /// <param name="contents">this is what the cell contains, not to be confused with the value</param>
        public Cell(object contents, Func<string, double> lookupFunc)
        {
            CellContentsInternal = contents;
            lookup = lookupFunc;
            if (contents is FormulaFixed)
            {
                
                
                //CellValue = (contents as Formula).Evaluate(lookup);
                //evaluateFunc();
            }
            else
            {
                CellValue = contents;
            }
        }

        //this is a getter for the contents of the cell
        public object CellContents   // the Name property
        {
            get
            {
                return CellContentsInternal;
            }
        }

        public void evaluateFunc()
        {
            try
            {
                CellValue = (CellContentsInternal as FormulaFixed).Evaluate(lookup);
            }
            catch (System.ArgumentException ae)
            {

            }
            
        }

    }
}
