using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Notepad3
{
    class CommonEditor
    {
        //enum for text editing mode
        public enum TextMode { RTF, TXT }
        public TextMode mode { get; private set; }

        //document and save status
        private bool isSaved = false;
        public FileInfo document { get; private set; }

        //the two editng views
        RichTextBox rtfView;
        TextBox txtView;

        //exterior classes interact with this control
        public Control CurrentEditor { get; private set; }

        /// <summary>
        /// Constructs a CommonEditor object
        /// </summary>
        /// <param name="rtf">RichTextBox control to associate with</param>
        /// <param name="txt">TextBox control to associate with</param>
        /// <param name="defaultState">The default editing state of this CommonEditor</param>
        public CommonEditor(RichTextBox rtf, TextBox txt, TextMode defaultState=TextMode.RTF)
        {
            rtfView = rtf;
            txtView = txt;
            mode = defaultState;

            //configure the views
            if (defaultState == TextMode.RTF)
            {
                rtfView.Visibility = System.Windows.Visibility.Visible;
                rtfView.IsEnabled = true;
                txtView.Visibility = System.Windows.Visibility.Hidden;
                txtView.IsEnabled = false;
                CurrentEditor = rtfView;
            }
            else
            {
                rtfView.Visibility = System.Windows.Visibility.Hidden;
                rtfView.IsEnabled = false;
                txtView.Visibility = System.Windows.Visibility.Visible;
                txtView.IsEnabled = true;
                CurrentEditor = txtView;
            }
        }

        /// <summary>
        /// Switches the current editing mode between TXT and RTF
        /// </summary>
        /// <param name="newMode">Enum representing the new mode to switch to</param>
        private void setTextMode(TextMode newMode)
        {
            if (newMode == mode) { return; }
            //prep the controls for switch
            Control old = CurrentEditor;
            Control current;
            if (newMode == TextMode.RTF)
            {
                //load the string into the RTF view
                rtfView.Document.Blocks.Clear();
                rtfView.Document.Blocks.Add(new Paragraph(new Run(txtView.Text)));
                current = rtfView;
                //clear the old text
                txtView.Clear();
            }
            else
            {
                //warn the user with a popup dialog
                MessageBoxResult res = MessageBox.Show("Converting from rich text to plain text will lose all formatting.\n\nAre you sure you want to convert your document to plain text?", "Potential data loss", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (res)
                {
                    case MessageBoxResult.No:
                        return;
                }
                //load the text
                string richText = new TextRange(rtfView.Document.ContentStart, rtfView.Document.ContentEnd).Text;
                txtView.Text = richText;
                current = txtView;

                //clear the old text
                rtfView.Document.Blocks.Clear();
            }
            //switch the views
            current.IsEnabled = true;
            current.Visibility = System.Windows.Visibility.Visible;

            old.IsEnabled = false;
            old.Visibility = System.Windows.Visibility.Hidden;

            CurrentEditor = current;
            mode = newMode;
        }
       
        /// <summary>
        /// Toggles between RTF editing and TXT editing
        /// </summary>
        public void ToggleTextMode()
        {
            TextMode mTemp = (mode == TextMode.RTF) ? TextMode.TXT : TextMode.RTF;
            setTextMode(mTemp);
        }

        /// <summary>
        /// Returns the editing control for a passed Enum
        /// </summary>
        /// <param name="m"></param>
        /// <returns>Control for enum</returns>
        private System.Windows.Controls.Control getControlForEnum(TextMode m)
        {
            switch (m){
                case TextMode.RTF:
                    return rtfView;
                default:
                    return txtView;
            }
        }

        /// <summary>
        /// Gets the text from the current editor
        /// </summary>
        public string GetText()
        {
            switch (mode)
            {
                case TextMode.RTF:
                    return new TextRange(rtfView.Document.ContentStart, rtfView.Document.ContentEnd).Text;
                default:
                    return txtView.Text;
            }
        }

        /// <summary>
        /// Set the text for the active control
        /// </summary>
        /// <param name="text">Text to set</param>
        public void SetText(string text)
        {
            switch (mode)
            {
                case TextMode.RTF:
                    rtfView.Document.Blocks.Clear();
                    rtfView.Document.Blocks.Add(new Paragraph(new Run(text)));
                    break;
                default:
                    txtView.Text = text;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void NewFile()
        {
            if (GetText().Length > 0 || !isSaved)
            {
                MessageBoxResult res = MessageBox.Show("Save changes before closing?", "Unsaved document", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                   bool success = SaveFile();
                   if (!success)
                    {
                        return;
                    }
                }
            }
            SetText("");
            isSaved = false;
            document = null;
        }

        /// <summary>
        /// Writes the file to disk
        /// </summary>
        /// <returns>True if saved successfully, false if failed</returns>
        public bool SaveFile()
        {
            //if no FileInfo set, prompt for one (re-prompt if invalid unless cancel
            if (document == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    document = new FileInfo(saveFileDialog.FileName);
                }
            }

            //attempt to write file, and return if it fails
            try
            {
                if (mode == TextMode.RTF)
                {
                    TextRange t = new TextRange(rtfView.Document.ContentStart,
                                    rtfView.Document.ContentEnd);
                    FileStream file = new FileStream(document.FullName, FileMode.Create);
                    t.Save(file, System.Windows.DataFormats.Rtf);
                    file.Close(); 
                }
                else
                {
                    File.WriteAllText(document.FullName, txtView.Text);
                }
            }
            catch(Exception)
            {

            }

            return false;
        }

        private FileInfo FileInfo(string fileName)
        {
            throw new NotImplementedException();
        }

        public void LoadFile(string path)
        {
            if (mode == TextMode.RTF)
            {
                var documentBytes = Encoding.UTF8.GetBytes(path);
                using (var reader = new MemoryStream(documentBytes))
                {
                    reader.Position = 0;
                    rtfView.SelectAll();
                    rtfView.Selection.Load(reader, DataFormats.Rtf);
                }
            }
        }

    }
}
