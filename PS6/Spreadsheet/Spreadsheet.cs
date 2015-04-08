using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Collections;

namespace SS
{
    /// <summary>
    /// A spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 FormulaFixed class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and an and sign are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a FormulaFixed.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a FormulaFixed, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the FormulaFixed class.  The value of a FormulaFixed,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {

        private Dictionary<string, Cell> CellSet;
        private bool spreadChanged = false;

        /// <summary>
        /// the graph of all cell dependancies
        /// </summary>
        protected DependencyGraph CellDep;




        /// <summary>
        /// Your zero-argument constructor should create an empty spreadsheet 
        /// that imposes no extra validity conditions, normalizes every cell name to itself, 
        /// and has version "default"
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            CellSet = new Dictionary<string, Cell>();
            CellDep = new DependencyGraph();
        }

        /// <summary>
        /// You should add a four-argument constructor to the Spreadsheet class. It should allow the user to 
        /// provide a string representing a path to a file (first parameter), a validity delegate (second 
        /// parameter), a normalization delegate (third parameter), and a version (fourth parameter). It 
        /// should read a saved spreadsheet from a file (see the Save method) and use it to construct a new 
        /// spreadsheet. The new spreadsheet should use the provided validity delegate, normalization 
        /// delegate, and version.
        /// </summary>
        /// <param name="filePath">filepath to load the spreadsheet from</param>
        /// <param name="isValid">a delegate to check if a cellname is valid</param>
        /// <param name="normalize">normalizes the cellnames</param>
        /// <param name="version">sets this version of the spreadsheet</param>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            CellSet = new Dictionary<string, Cell>();
            CellDep = new DependencyGraph();
            GetSavedVersion(filePath);
        }

        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        /// <param name="isValid">a delegate to check if a cellname is valid</param>
        /// <param name="normalize">normalizes the cellnames</param>
        /// <param name="version">sets this version of the spreadsheet</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            //this.IsValid = isValid;
            //this.Normalize = normalize;
            //this.Version = version;
            CellSet = new Dictionary<string, Cell>();
            CellDep = new DependencyGraph();
        }

        

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed 
        {
            get { return spreadChanged; }
            protected set { spreadChanged = value; }
        }

        

        

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// <param name="filename">the filename to load from</param>
        /// <returns>a string of the version of spreadsheet in the file</returns>
        public override string GetSavedVersion(String filename)
        {
            //Queue fileContents = new Queue();

            string fileVersion = "";
            try
            {

                using (XmlReader reader = XmlReader.Create(filename))
                {

                    string cellName = "";
                    string cellCont = "";

                    while (reader.Read())
                    {


                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    fileVersion = reader["version"];
                                    break;

                                case "cell":
                                    //Console.WriteLine();
                                    cellName = "";
                                    cellCont = "";
                                    break;

                                case "name":
                                    //Console.Write("State name = ");
                                    reader.Read();
                                    cellName = reader.Value;
                                    //Console.WriteLine(reader.Value);
                                    break;

                                case "contents":
                                    //Console.Write("State capital = ");
                                    reader.Read();
                                    cellCont = reader.Value;
                                    SetContentsOfCell(cellName, cellCont);
                                    //Console.WriteLine(reader.Value);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("The file could not be read");
            }

            if (fileVersion.ToUpper() != Version.ToUpper())
            {
                if (Version == "default")
                {
                    Version = fileVersion;
                }
                else
                {

                    throw new SpreadsheetReadWriteException("The file version was not the same as the spreadsheet");
                }
            }

            Changed = false;
            return fileVersion;

        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a FormulaFixed f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// <param name="filename">the filename to save to</param>
        public override void Save(String filename)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (KeyValuePair<string, Cell> kvp in CellSet)
                    {
                        object contents = kvp.Value.CellContents;
                        string contentsString = "";
                        if (contents is double)
                        {
                            contentsString = contents.ToString();
                        }
                        else if (contents is FormulaFixed)
                        {
                            contentsString = "=" + contents.ToString();
                        }
                        else if (contents is string)
                        {
                            contentsString = (string)contents;
                        }

                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", kvp.Key);
                        writer.WriteElementString("contents", contentsString);
                        writer.WriteEndElement();

                    }


                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("The file could not be saved");
            }
            Changed = false;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        /// <param name="name">the cell to get the value from</param>
        /// <returns>returns the value of the cell</returns>
        public override object GetCellValue(String name)
        {
            if (Regex.IsMatch(name, "(^[a-zA-Z]+[0-9]+$)") && IsValid(Normalize(name)) == true)
            {
                name = Normalize(name);
                Cell cellAtKey;
                if (CellSet.TryGetValue(name, out cellAtKey))
                {
                    return cellAtKey.CellValue;
                }
                else
                {
                    string empty = "";
                    return empty;
                }

            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        /// <returns>an ienumerable object containing all the non empty cells</returns>
        public override IEnumerable<String> GetNamesOfAllNonemptyCells()
        {
            foreach (KeyValuePair<string, Cell> kvp in CellSet)
            {
                yield return kvp.Key;
            }
        }




        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaFixed.
        /// </summary>
        /// <param name="name">the name of the cell to pull the contents from</param>
        /// <returns>an object containing either a string, double, or formula that is the contents of the cell</returns>
        public override object GetCellContents(String name)
        {
            //this checks if the name is valid and not null
            if (Regex.IsMatch(name, "(^[a-zA-Z]+[0-9]+$)") && IsValid(Normalize(name)) == true)
            {
                name = Normalize(name);
                Cell cellAtKey;
                if (CellSet.TryGetValue(name, out cellAtKey))
                {
                    return cellAtKey.CellContents;
                }
                else
                {
                    string empty = "";
                    return empty;
                }

            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a FormulaFixed f using the FormulaFixed
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a FormulaFixed, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name">the cell name to assign a value</param>
        /// <param name="content">the content to put in the cell</param>
        /// <returns>an enumerable list containing all the values that have changed</returns>
        public override ISet<String> SetContentsOfCell(String name, String content)
        {
            double number;
            HashSet<String> depends;

            if (content == null)
            {
                throw new ArgumentNullException();
            }
            else if (!Regex.IsMatch(name, "(^[a-zA-Z]+[0-9]+$)") || IsValid(Normalize(name)) == false)
            {
                throw new InvalidNameException();
            }
            else if (Double.TryParse(content, out number))
            {
                depends = new HashSet<String>(SetCellContents(name, number));
            }
            else if (content.Length != 0 && content[0] == '=')
            {
                string tempContents = content.Substring(1);
                try
                {
                    FormulaFixed newFormula = new FormulaFixed(tempContents, Normalize, IsValid);
                }
                catch (Exception e)
                {
                    //throw new InvalidNameException();
                    throw new FormulaFormatException("Invalid formula");
                }
                FormulaFixed newFormula1 = new FormulaFixed(tempContents, Normalize, IsValid);
                depends = new HashSet<String>(SetCellContents(name, newFormula1));
                
            }
            else
            {
                depends = new HashSet<String>(SetCellContents(name, content));
            }

            //recalculate the dependants
            foreach (string cellName in depends)
            {
                
                Cell cellAtKey;
                if (CellSet.TryGetValue(cellName, out cellAtKey))
                {
                    if (cellAtKey.CellContents is string)
                    {
                        cellAtKey.CellValue = cellAtKey.CellContents;
                    }
                    else if (cellAtKey.CellContents is double)
                    {
                        cellAtKey.CellValue = cellAtKey.CellContents;
                    }
                    else if (cellAtKey.CellContents is FormulaFixed)
                    {
                        
                        //cellAtKey.CellValue = (cellAtKey.CellContents as FormulaFixed).Evaluate(getVar);
                        cellAtKey.evaluateFunc();
                    }
                }
            }

            Changed = true;
            return depends;
            
        }

        /// <summary>
        /// this is a helper function to get a variables value
        /// </summary>
        /// <param name="var">the cellname to get the value</param>
        /// <returns>the double for the variable</returns>
        protected double getVar(String var)
        {
            if (Regex.IsMatch(var, "(^[a-zA-Z]+[0-9]*$)") || IsValid(Normalize(var)) == true)
            {
                var = Normalize(var);
                Cell cellAtKey;
                if (CellSet.TryGetValue(var, out cellAtKey))
                {
                    if (cellAtKey.CellValue is double)
                    {
                        return (double)cellAtKey.CellValue;
                    }
                    else
                    {

                        throw new ArgumentException("var not here", "exp");
                    }
                }
                else
                {
                    throw new ArgumentException("var not here", "exp");
                }
            }
            else
            {
                throw new ArgumentException("invalid var", "exp");
            }

        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name">the name of the cell to set as </param>
        /// <param name="number">the double to put into the cell at the key</param>
        /// <returns>returns all, direct or inderect, dependants</returns>
        protected override ISet<String> SetCellContents(String name, double number)
        {
            //this checks if the name is valid and not null
            if (Regex.IsMatch(name, "(^[a-zA-Z]+[0-9]+$)") && IsValid(Normalize(name)) == true)
            {
                name = Normalize(name);
                Cell cellAtKey = new Cell(number, getVar);
                try
                {
                    CellSet.Add(name, cellAtKey);
                }
                catch (ArgumentException)
                {
                    cellReplace(name, cellAtKey);
                    
                }

                //i need to figure out how to get the indirect dependants, probably a recursive helper function
                HashSet<String> depends = new HashSet<String>(GetCellsToRecalculate(name));
                //depends.Add(name);
                return depends;
                //return (ISet<String>)CellDep.GetDependents(name);

            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name">the name of the cell to set as</param>
        /// <param name="text">the string to put into the cell at the key</param>
        /// <returns>returns all, direct or inderect, dependants</returns>
        protected override ISet<String> SetCellContents(String name, String text)
        {
            if (text == null)
            {
                throw new ArgumentNullException();
            }
            //this checks if the name is valid and not null
            else if (Regex.IsMatch(name, "(^[a-zA-Z]+[0-9]+$)") && IsValid(Normalize(name)) == true)
            {
                name = Normalize(name);
                Cell cellAtKey = new Cell(text, getVar);
                try
                {
                    CellSet.Add(name, cellAtKey);
                }
                catch (ArgumentException)
                {
                    cellReplace(name, cellAtKey);
                }
                //return
                HashSet<String> depends = new HashSet<String>(GetCellsToRecalculate(name));
                //depends.Add(name);
                return depends;
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a 
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <param name="name">the name of the cell to set as</param>
        /// <param name="formula">the formula to put into the cell at the key</param>
        /// <returns>returns all, direct or inderect, dependants</returns>
        protected override ISet<String> SetCellContents(String name, FormulaFixed formula)
        {
            if (formula == null)
            {
                throw new ArgumentNullException();
            }
            //this checks if the name is valid and not null
            else if (Regex.IsMatch(name, "(^[a-zA-Z]+[0-9]+$)") && IsValid(Normalize(name)) == true)
            {
                name = Normalize(name);
                Cell currentVal = null;
                bool newKey = false;

                Cell cellAtKey = new Cell(formula, getVar);
                try
                {
                    CellSet.Add(name, cellAtKey);
                    newKey = true;
                }
                catch (ArgumentException)
                {
                    CellSet.TryGetValue(name, out currentVal);
                    cellReplace(name, cellAtKey);
                }
                //setup dependancies
                HashSet<String> vars = new HashSet<String>(formula.GetVariables());
                foreach (String variable in vars)
                {
                    CellDep.AddDependency(variable, name);
                }

                //check for circular dependancy
                try
                {

                    GetCellsToRecalculate(new HashSet<String>(formula.GetVariables()));
                }
                catch (Exception e)
                {
                    //add back old dependancies if there were any
                    if (!newKey)
                    {
                        if (currentVal.CellContents is FormulaFixed)
                        {
                            FormulaFixed form = (currentVal.CellContents as FormulaFixed);
                            HashSet<String> vars2 = new HashSet<String>(form.GetVariables());
                            foreach (String variable in vars2)
                            {
                                CellDep.AddDependency(variable, name);
                            }
                        }
                        CellSet[name] = currentVal;
                    }
                    else
                    {
                        CellSet.Remove(name);
                    }

                    //remove old dependancies
                    foreach (String variable in vars)
                    {
                        CellDep.RemoveDependency(variable, name);
                    }
                    //restore cells value
                    throw new CircularException();
                }

                //return
                HashSet<String> depends = new HashSet<String>(GetCellsToRecalculate(name));
                //depends.Add(name);
                return depends;
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// This is a helper function for setCellContents, it handles when the key already exists
        /// </summary>
        /// <param name="name">key at which you are replacing a cell</param>
        /// <param name="newCell">the cell to put in it's place</param>
        private void cellReplace(string name, Cell newCell)
        {
            Cell currentVal;
            CellSet.TryGetValue(name, out currentVal);
            if (currentVal.CellContents is FormulaFixed)
            {
                FormulaFixed form = (currentVal.CellContents as FormulaFixed);
                HashSet<String> vars = new HashSet<String>(form.GetVariables());
                foreach (String variable in vars)
                {
                    CellDep.RemoveDependency(variable, name);
                }
            }
            CellSet[name] = newCell;
        }


        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        /// <param name="name">the key to find the dependants for</param>
        /// <returns>the dependants for the key are returned as an Ienemerable</returns>
        protected override IEnumerable<String> GetDirectDependents(String name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            //this checks if the name is valid and not null
            else if (Regex.IsMatch(name, "(^[a-zA-Z]+[0-9]+$)") && IsValid(Normalize(name)) == true)
            {
                name = Normalize(name);
                return CellDep.GetDependents(name);

            }
            else
            {
                throw new InvalidNameException();
            }
        }
    }
}
