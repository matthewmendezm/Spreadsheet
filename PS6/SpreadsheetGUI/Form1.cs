﻿
using Spreadsheet;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS
{
    /// <summary>
    /// This is the visual and control implementation of my spreadsheet application. It will allow a user to interact with a GUI
    /// that represents a spreadsheet
    /// </summary>
    public partial class SpreadsheetGUI : Form
    {
        private Controller controller;

        AbstractSpreadsheet currentSheet;
        enum columns { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z};
        string selectedCell;
        int selectedRow, selectedCol;
        string currentFilePath;
        Boolean closed = false;
        Stack<BackItem> backActions = new Stack<BackItem>();

        /// <summary>
        /// constructor for the gui, it initializes buttons and the current selected cell
        /// </summary>
        public SpreadsheetGUI()
        {
            InitializeComponent();
            controller = new Controller();
            controller.IncomingMessageEvent += MessageReceived;


            // This an example of registering a method so that it is notified when
            // an event happens.  The SelectionChanged event is declared with a
            // delegate that specifies that all methods that register with it must
            // take a SpreadsheetPanel as its parameter and return nothing.  So we
            // register the displaySelection method below.

            // This could also be done graphically in the designer, as has been
            // demonstrated in class.
            spreadsheetPanel1.SelectionChanged += displaySelection;

            spreadsheetPanel1.SetSelection(0, 0);
            selectedCell = "A1";

            currentSheet = new Spreadsheet(isVal, norm, "PS6");

            this.Width += 2;

            this.Text = "New Spreadsheet - Mangosheets Online";

            //saveToolStripMenuItem.Enabled = false;
            //saveToolStripMenuItem1.Enabled = false;

        }

        private void MessageReceived(string obj)
        {
            //update gui
            //statusLabel.Text = obj;
            int col = 1;
            int row = 1;
                    
            //spreadsheetPanel1.SetValue(col, row, obj);
            spreadsheetPanel1.Invoke(new Action(() => { spreadsheetPanel1.SetValue(col, row, obj); }));
        }

        /// <summary>
        /// this constructor is for opening a previously saved spreadsheet. it will generate the gui with the spreadsheet provided
        /// </summary>
        /// <param name="filePath">the path to the spreadsheet to open</param>
        public SpreadsheetGUI(string filePath) : this()
        {
            currentFilePath = filePath;

            try
            {
                currentSheet.GetSavedVersion(filePath);
            }
            catch (SpreadsheetReadWriteException)
            {
                MessageBox.Show("This file could not be read correctly! It may be for an older version of Mangosheets Online, or may have become corrupt. Continue at own risk!", "Mangosheets Online", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

            foreach (string cellName in currentSheet.GetNamesOfAllNonemptyCells())
            {
                //string colLetter = cellName.Substring(1, 1);
                /*
                string letter = cellName.Substring(0, 1);
                string number = cellName.Substring(1);
                int col = (int)Enum.Parse(typeof(columns), letter);
                int row = 0;
                try
                {
                    row = Convert.ToInt32(number);
                }
                catch (FormatException e)
                {
                    
                }

                row -= 1; 
                spreadsheetPanel1.SetValue(col, row, currentSheet.GetCellValue(cellName).ToString());
                */
                updateSpreadValues(cellName);

            }

            textBoxCellValue.Text = currentSheet.GetCellValue(selectedCell).ToString();
            if (currentSheet.GetCellValue(selectedCell) is FormulaError)
            {
                textBoxCellValue.Text = "Dependant cells not set";

            }

            textBoxCellContents.Text = currentSheet.GetCellContents(selectedCell).ToString();

            int subInd = filePath.LastIndexOf(@"\");
            if (subInd >= 0)
            {
                filePath = filePath.Substring(subInd+1);
            }
            this.Text = filePath + " - Mangosheets Online";

        }

        /// <summary>
        /// this is called everytime the user selects a different cell, it will update the text boxes on the top with the values
        /// from the selected cell
        /// </summary>
        /// <param name="ss">the panel a cell has been selected from</param>
        private void displaySelection(SpreadsheetPanel ss)
        {


            int row, col;
            String value;
            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);

            selectedRow = row;
            selectedCol = col;

            row += 1;

            selectedCell = Enum.GetName(typeof(columns), col) + row;

            textBoxCellName.Text = selectedCell;

            textBoxCellValue.Text = currentSheet.GetCellValue(selectedCell).ToString();
            if (currentSheet.GetCellValue(selectedCell) is FormulaError)
            {
                textBoxCellValue.Text = "Dependant cells not set";

            }
            
            textBoxCellContents.Text = currentSheet.GetCellContents(selectedCell).ToString();
            if (currentSheet.GetCellContents(selectedCell) is FormulaFixed)
            {
                textBoxCellContents.Text = "=" + textBoxCellContents.Text;
            }
            
            

            statusLabel.Text = "Selected cell " + selectedCell;
            textBoxCellContents.Focus();
        }

        /// <summary>
        /// normalizes the variables to all upper case
        /// </summary>
        /// <param name="var">the variable to normalize</param>
        /// <returns>the variable after it has been normalized</returns>
        public static string norm(String var)
        {

            return var.ToUpper();

        }

        /// <summary>
        /// this checks if the variable is valid within extra constraints. Checking if it is a cell that exists
        /// on the spreadsheetpanel
        /// </summary>
        /// <param name="var">the variable to check if it is valid</param>
        /// <returns>a boolean stating whether the var passed in was valid or not</returns>
        public static bool isVal(String var)
        {
            if (Regex.IsMatch(var, "(^[A-Z][1-9][0-9]?$)"))
            {
                return true;

            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// creates a new window with a blank spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to ru n the form on the same thread as the other forms.
            SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetGUI());
            statusLabel.Text = "Opened new spreadsheet in new window";

        }

        /// <summary>
        /// closes the current spreadsheet window, and if there aren't anymore windows the main program will close itself
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            Close();
        }

        /// <summary>
        /// this opens a previously saved spreadsheet in a new window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            DialogResult dialog = openFileDialog1.ShowDialog(); 
            if (dialog == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                try
                {
                    
                }
                catch (IOException)
                {
                }

                SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetGUI(file));
                statusLabel.Text = "Opened spreadsheet at " + file + " in new window";
            }
        }

        /// <summary>
        /// saves the current spreadsheet either where it was saved previously, or asks for a new location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Saving";
            if (currentFilePath == null)
            {
                saveFileDialog1.ShowDialog();
            }
            else
            {
                currentSheet.Save(currentFilePath);
                statusLabel.Text = "Saved!";
                //saveToolStripMenuItem.Enabled = false;
                //saveToolStripMenuItem1.Enabled = false;
            }
        }

        /// <summary>
        /// saves the current spreadsheet either where it was saved previously, or asks for a new location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            // Get file name
            string file = saveFileDialog1.FileName;
            // Write to the file name selected
            currentSheet.Save(file);
            currentFilePath = file;
            statusLabel.Text = "Saved!";
            
            int subInd = file.LastIndexOf(@"\");

            if (subInd >= 0)
            {
                file = file.Substring(subInd+1);
            }
            this.Text = file + " - Mangosheets Online";

            //saveToolStripMenuItem.Enabled = false;
            //saveToolStripMenuItem1.Enabled = false;

        }

        /// <summary>
        /// enters data into the currently selected cell. If any bad data is passed in, displays a popup box explaining what went
        /// wrong
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCellContents_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                backActions.Push(new BackItem(selectedCell, currentSheet.GetCellValue(selectedCell).ToString()));

                backToolStripMenuItem.Enabled = true;
                
                //FOR SPREADSHEET SERVER PROJECT
                //FOR SPREADSHEET SERVER PROJECT
                //FOR SPREADSHEET SERVER PROJECT
                //FOR SPREADSHEET SERVER PROJECT
                controller.SendMessage(currentSheet.GetCellValue(selectedCell).ToString());
                
                try
                {

                    updateNewCell(selectedCell, textBoxCellContents.Text);
                }
                catch (FormulaFormatException ec)
                {
                    MessageBox.Show("You have put in bad data for a formula, your changes have not been made!", "Mangosheets Online", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                catch (CircularException ce)
                {
                    MessageBox.Show("Setting this as the formula would result in a circular dependancy, your changes have not been made!", "Mangosheets Online", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                catch (ArgumentException ecx)
                {

                }
                finally
                {
                    textBoxCellContents.Text = currentSheet.GetCellContents(selectedCell).ToString();
                    if (currentSheet.GetCellContents(selectedCell) is FormulaFixed)
                    {
                        textBoxCellContents.Text = "=" + textBoxCellContents.Text;
                    }
                }

                
            }
        }

        /// <summary>
        /// changes the status bar to give a tip to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCellContents_TextChanged(object sender, EventArgs e)
        {
            statusLabel.Text = "Press enter to set cell value";

        }

        /// <summary>
        /// saves at a new directory every time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Saving";
            saveFileDialog1.ShowDialog();
        }

        /// <summary>
        /// changes the status bar to give a tip to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCellContents_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Start cell contents with an equals(=) to make it a formula";
        }

        /// <summary>
        /// updates the values of the cell in the spreadsheetpanel
        /// </summary>
        /// <param name="cellName"></param>
        private void updateSpreadValues(string cellName)
        {
            string letter = cellName.Substring(0, 1);
            string number = cellName.Substring(1);
            int col = (int)Enum.Parse(typeof(columns), letter);
            int row = 0;
            try
            {
                row = Convert.ToInt32(number);
            }
            catch (FormatException e)
            {

            }

            row -= 1;
            spreadsheetPanel1.SetValue(col, row, currentSheet.GetCellValue(cellName).ToString());
        }

        /// <summary>
        /// a dialog box pops up if the user has closed a spreadsheet that has unsaved changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpreadsheetGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (currentSheet.Changed && closed != true)
            {
                if (MessageBox.Show("Are you sure you want to close? All unsaved changes will be lost! Oh no!", "Mangosheets Online", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;


                }
            }
        }

        /// <summary>
        /// saves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// displays a message box with info about the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This software was developed by Team Mangos for cs3505 at the University of Utah.", "Mangosheets Online", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// opens an html file with the help documentation for Mangosheets Online
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Process.Start("http://www.eng.utah.edu/~tmorris/spreadlyhelp.html");
        }

        /// <summary>
        /// undos changes to the spreadsheet. Uses a stack to keep track of changes to undo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackItem tempItem = backActions.Pop();

            updateNewCell(tempItem.name, tempItem.value);

            textBoxCellContents.Text = currentSheet.GetCellContents(selectedCell).ToString();
            

            if (backActions.Count < 1)
            {
                backToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// updates a new cell  and all the dependant cells
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="newValue"></param>
        private void updateNewCell(string cellName, string newValue)
        {
            foreach (string cell in currentSheet.SetContentsOfCell(cellName, newValue))
            {
                updateSpreadValues(cell);
            }


            //spreadsheetPanel1.SetValue(selectedCol, selectedRow, currentSheet.GetCellValue(selectedCell).ToString());


            textBoxCellValue.Text = currentSheet.GetCellValue(selectedCell).ToString();
            if (currentSheet.GetCellValue(selectedCell) is FormulaError)
            {
                textBoxCellValue.Text = "Dependant cells not set";

            }

            statusLabel.Text = "Set cell " + selectedCell + " to " + textBoxCellContents.Text;

            //saveToolStripMenuItem.Enabled = true;
            //saveToolStripMenuItem1.Enabled = true;
        }

        private void SpreadsheetGUI_Load(object sender, EventArgs e)
        {

        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectionDialog connectionDialog = new ConnectionDialog(controller);
            connectionDialog.ShowDialog();
        }

        private void statusLabel_Click(object sender, EventArgs e)
        {

        }

        private void spreadsheetPanel1_Load(object sender, EventArgs e)
        {

        }

        private void sendMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendDialog sendDialog = new SendDialog(controller);
            sendDialog.ShowDialog();
        }


    }
}
