using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Notepad3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CommonEditor editor;
        const string untitled_doc = "Untitled";
        public MainWindow()
        {
            InitializeComponent();
            //configure the view
            editor = new CommonEditor(rtfView,txtView);

            //handle being "opened with"
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                editor.Open(arguments[1]);
                SetTitlebar(editor.document.Name, editor.isSaved);
            }
            else
            {
                SetTitlebar(untitled_doc, editor.isSaved);
            }
            wordWrap.IsEnabled = editor.mode == CommonEditor.TextMode.TXT;
        }

        //================================UI click handlers==============================

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == toggleEdit)
            {
                editor.ToggleTextMode();

                //disable word wrap menu on 
                wordWrap.IsEnabled = editor.mode == CommonEditor.TextMode.TXT;
            }
            else if (sender == NewFile)
            {
                editor.NewFile();
                SetTitlebar("Untitled", editor.isSaved);
            }
            else if (sender == NewWindow)
            {
                ///launch another copy of the executable
                try
                {
                    Process p = new Process();
                    p.StartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Unable to launch process");
                }
            }
            else if (sender == SaveFile)
            {
                if (!editor.SaveFile())
                {
                    MessageBox.Show("There was a problem saving the document.", "Error saving file");
                }
                else
                {
                    SetTitlebar(editor.document.Name, editor.isSaved);
                }
                
            }
            else if (sender == SaveFileAs)
            {
                if (!editor.SaveFileAs())
                {
                    MessageBox.Show("There was a problem saving the document.", "Error saving file");
                }
                else
                {
                    SetTitlebar(editor.document.Name, editor.isSaved);
                }
            }
            else if (sender == OpenFile)
            {
                if (editor.OpenFile())
                {
                    SetTitlebar(editor.document.Name,editor.isSaved);
                }
            }
            else if(sender == wordWrap){
                txtView.TextWrapping = wordWrap.IsChecked? TextWrapping.Wrap : TextWrapping.NoWrap;
                txtView.HorizontalScrollBarVisibility = wordWrap.IsChecked ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Visible;
            }
            else if (sender == Exit)
            {
                Close();
            }
            else if (sender == Undo && !editor.Undo())
            {
                MessageBox.Show("There are no more commands to undo.", "Edit history"); 
            }
            else if (sender == Redo && !editor.Redo())
            {   
                MessageBox.Show("There are no more commands to redo.", "Edit history"); 
            }
            else if (sender == Cut) { editor.Cut(); }
            else if (sender == Copy) { editor.Copy(); }
            else if (sender == Paste) { editor.Paste(); }
            else if (sender == Update) { System.Diagnostics.Process.Start("https://github.com/Ravbug/Notepad3/releases"); }
            else if (sender == Repository) { System.Diagnostics.Process.Start("https://github.com/ravbug/notepad3"); }
        }

        /// <summary>
        /// Called when a key is pressed in either text view
        /// </summary>
        /// <param name="sender">Object that raised event</param>
        /// <param name="e">Event arguments</param>
        private void keyhandler(object sender, RoutedEventArgs e)
        {
            if (editor.isSaved)
            {
                string name = editor.document == null ? untitled_doc : editor.document.Name;
                SetTitlebar(name,false);
            }
            editor.SetSavedState(false);
        } 

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Sets the titlebar based on different criteria
        /// </summary>
        /// <param name="doc">Name of the document</param>
        /// <param name="isModified">True if the document is "dirty" (modified but not saved)</param>
        private void SetTitlebar(string doc, bool isModified)
        {
            this.Title = doc + (isModified ? "" : "*") + " - Notepad3";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!editor.Close())
            {
                e.Cancel = true;
            }
        }

        private void OnAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Version: 0.0.1a\nDeveloper: Ravbug\n\nCopyright © 2018-" + DateTime.Now.Year.ToString(), "About Notepad3",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        /* ==================== RTF Events ============================*/
        private void Font_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (editor != null)
            {
                editor.SetFont(e.AddedItems[0].ToString());
            }
        }

        private void FontSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (editor != null && e.AddedItems.Count > 0)
            {
                string value = (e.AddedItems[0] as ComboBoxItem).Content.ToString();
                editor.SetFontSize(double.Parse(value));
            }
        }

        private void FontSizeChanged(object sender, EventArgs e)
        {
            if (editor != null)
            {
                editor.SetFontSize(double.Parse(FontSizeCombo.Text));
            }
        }
    }
}
