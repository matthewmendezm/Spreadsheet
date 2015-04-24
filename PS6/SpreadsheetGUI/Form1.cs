/*
 * Modified from CS 3500 spreadsheet to be multi user.
 * 
 * Author: Matthew Mendez, Jase Bleazard, Taylor Morris, Scott Wells.
 */

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SS
{
    /// <summary>
    /// This is the visual and control implementation of my spreadsheet application. It will allow a user to interact with a
    /// GUI that represents a spreadsheet
    /// </summary>
    public partial class SpreadsheetGUI : Form
    {
        private Controller controller;

        private AbstractSpreadsheet currentSheet;

        private enum columns { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z };

        private string selectedCell;
        private int selectedRow, selectedCol;
        private string currentFilePath;

        //Boolean closed = false;
        private Stack<BackItem> backActions = new Stack<BackItem>();

        /// <summary>
        /// constructor for the GUI, it initializes buttons and the current selected cell
        /// </summary>
        public SpreadsheetGUI()
        {
            InitializeComponent();
            controller = new Controller();
            controller.IncomingCellEvent += CellReceived;
            controller.IncomingConnectionEvent += ConnectionReceived;
            controller.IncomingErrorEvent += ErrorReceived;
            controller.IncomingDisconnectEvent += DisconnectReceived;

            // This an example of registering a method so that it is notified when an event happens. The SelectionChanged
            // event is declared with a delegate that specifies that all methods that register with it must take a
            // SpreadsheetPanel as its parameter and return nothing. So we register the displaySelection method below.

            // This could also be done graphically in the designer, as has been demonstrated in class.
            spreadsheetPanel1.SelectionChanged += displaySelection;

            spreadsheetPanel1.SetSelection(0, 0);
            selectedCell = "A1";

            currentSheet = new Spreadsheet(isVal, norm, "PS6");

            this.Width += 2;

            this.Text = "Mangosheets Online";

            closeConnectionToolStripMenuItem.Enabled = false;
            sendMessageToolStripMenuItem.Enabled = false;
        }

        private void DisconnectReceived(string obj)
        {
            spreadsheetPanel1.Clear();
            currentSheet = new Spreadsheet(isVal, norm, "PS6");

            this.Invoke(new Action(() =>
            {
                statusLabel.Text = "Disconnected";
                closeConnectionToolStripMenuItem.Enabled = false;
                sendMessageToolStripMenuItem.Enabled = false;
                textBoxCellContents.Enabled = false;
                this.Text = "Mangosheets Online";
                backToolStripMenuItem.Enabled = false;
            }));
        }

        private void ErrorReceived(string[] obj)
        {
            if (obj[0] == "0")
            {
                // Our client implementation does not send this error
            }
            else if (obj[0] == "1")
            {
                statusLabel.Invoke(new Action(() => { statusLabel.Text = "Current formula results in a circular dependency, no changes made"; }));

                if (currentSheet.GetCellContents(selectedCell) is FormulaFixed)
                {
                    textBoxCellContents.Text = "=" + textBoxCellContents.Text;
                }
            }
            else if (obj[0] == "2")
            {
                // Not possible to reach this state while using our client
            }
            else if (obj[0] == "3")
            {
                // Not possible to reach this state while using our client
            }
            else if (obj[0] == "4")
            {
                // our authentication method intentional causes this error, we ignore it.
            }
        }

        private void ConnectionReceived(string[] obj)
        {
            for (int i = 0; i < Int32.Parse(obj[0]); i++) ;
            this.Invoke(new Action(() =>
            {
                this.Text = obj[1] + " - " + obj[2] + " - Mangosheets Online";
                closeConnectionToolStripMenuItem.Enabled = true;
                sendMessageToolStripMenuItem.Enabled = true;
                textBoxCellContents.Enabled = true;
                backToolStripMenuItem.Enabled = true;
            }));
        }

        private void CellReceived(string[] obj)
        {
            this.Invoke(new Action(() =>
            {
                updateNewCell(obj[0], obj[1]);
            }));
        }

        /// <summary>
        /// this constructor is for opening a previously saved spreadsheet. it will generate the GUI with the spreadsheet
        /// provided
        /// </summary>
        /// <param name="filePath">the path to the spreadsheet to open</param>
        public SpreadsheetGUI(string filePath)
            : this()
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
                filePath = filePath.Substring(subInd + 1);
            }
            this.Text = "Mangosheets Online";
        }

        /// <summary>
        /// this is called every time the user selects a different cell, it will update the text boxes on the top with the
        /// values from the selected cell
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
            textBoxCellContents.SelectAll();
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
        /// this checks if the variable is valid within extra constraints. Checking if it is a cell that exists on the
        /// spreadsheetpanel
        /// </summary>
        /// <param name="var">the variable to check if it is valid</param>
        /// <returns>a boolean stating whether the var passed in was valid or not</returns>
        public static bool isVal(String var)
        {
            return Regex.IsMatch(var, "(^[A-Z][1-9][0-9]?$)");
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
                file = file.Substring(subInd + 1);
            }
            this.Text = file + " - Mangosheets Online";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.Z):
                    string message = "undo";
                    controller.SendMessage(message);
                    return true;

                case Keys.Tab:
                    textBoxCellContents_KeyUp(new object(), new KeyEventArgs(Keys.Tab));
                    break;

                default:
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// enters data into the currently selected cell. If any bad data is passed in, displays a pop-up box explaining what
        /// went wrong
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCellContents_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.Handled = true;

                backActions.Push(new BackItem(selectedCell, currentSheet.GetCellValue(selectedCell).ToString()));

                bool hasError = false;

                try
                {
                    this.Invoke(new Action(() =>
                    {
                        updateNewCell(selectedCell, textBoxCellContents.Text);
                        BackItem tempItem = backActions.Pop();
                        updateNewCell(tempItem.name, tempItem.value);
                    }));
                }
                catch (FormulaFormatException)
                {
                    statusLabel.Invoke(new Action(() => { statusLabel.Text = "Incorrectly formatted formula, no changes made"; }));

                    hasError = true;
                }
                catch (ArgumentException)
                {
                }
                finally
                {
                    if (hasError)
                    {
                        BackItem tempItem = backActions.Pop();
                    }

                    if (currentSheet.GetCellContents(selectedCell) is FormulaFixed)
                    {
                        this.Invoke(new Action(() =>
                        {
                            textBoxCellContents.Text = "=" + textBoxCellContents.Text;
                            textBoxCellValue.Text = "error";
                        }));
                    }
                }

                if (!hasError)
                {
                    string message = "cell " + selectedCell + " " + textBoxCellContents.Text;
                    controller.SendMessage(message);
                }
            }
        }

        private int getCol(string selectedCell)
        {
            // Capture the first character of the cell name and convert it to a 0 indexed integer equivalent
            string temp = selectedCell.Substring(0, 1);
            char column = char.Parse(temp);
            int col = char.ToUpper(column) - 65;//index == 0

            return col;
        }

        private int getRow(string selectedCell)
        {
            // Capture the rest of the cell name (could be a one or two digit number)
            string temp = selectedCell.Substring(1);
            int row = int.Parse(temp);

            return row;
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
            catch (FormatException)
            {
            }

            row -= 1;

            if (currentSheet.GetCellValue(cellName).ToString() == "SpreadsheetUtilities.FormulaError")
            {
                spreadsheetPanel1.SetValue(col, row, "##error##");
            }
            else
            {
                spreadsheetPanel1.SetValue(col, row, currentSheet.GetCellValue(cellName).ToString());
            }
        }

        /// <summary>
        /// a dialog box pops up if the user has closed a spreadsheet that has unsaved changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpreadsheetGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
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
            MessageBox.Show("This software was developed by Team Mangos for CS 3505 Spring Semester 2015 at the University of Utah. \n \n Scott Wells, Taylor Morris, Matthew Mendez, and Jase Bleazard", "Mangosheets Online", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // send an undo message to the server.
            string message = "undo";
            controller.SendMessage(message);
        }

        private void updateNewCell(string cellName, string newValue)
        {
            foreach (string cell in currentSheet.SetContentsOfCell(cellName, newValue))
            {
                updateSpreadValues(cell);
            }

            // this is in the right place
            if (currentSheet.GetCellValue(selectedCell) is FormulaError)
            {
                textBoxCellValue.Invoke(new Action(() => { textBoxCellValue.Text = "Dependant cells not set"; }));
            }

            statusLabel.Invoke(new Action(() => { statusLabel.Text = "Set cell " + selectedCell + " to " + textBoxCellContents.Text; }));
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

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void closeConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.Disconnect();
        }

        private void sendMessageToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SendDialog sendDialog = new SendDialog(controller);
            sendDialog.ShowDialog();
        }
    }
}