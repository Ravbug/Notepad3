using System;
using System.Collections.Generic;
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
        //enum for text editing mode
        enum TextMode{RTF,TXT}
        static TextMode mode = TextMode.TXT;
        public MainWindow()
        {
            InitializeComponent();
            //by default loads the RTF view
            setTextMode(TextMode.RTF);
        }

        /// <summary>
        /// Returns the current active text editing view (RTF or TXT)
        /// </summary>
        /// <returns>System.Windows.Controls.Control current view</returns>
        private System.Windows.Controls.Control getCurrentEditor()
        {
            return getControlForEnum(mode);
        }
        
        /// <summary>
        /// Returns the editing control for a passed Enum
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private System.Windows.Controls.Control getControlForEnum(TextMode m)
        {
            if (m == TextMode.RTF)
            {
                return rtfView;
            }
            else
            {
                return txtView;
            }
        }

        /// <summary>
        /// Toggles between RTF editing and TXT editing
        /// </summary>
        private void toggleTextMode()
        {
            TextMode mTemp = (mode == TextMode.RTF) ? TextMode.TXT : TextMode.RTF;
            setTextMode(mTemp);
        }

        /// <summary>
        /// Switches the current editing mode between TXT and RTF
        /// </summary>
        /// <param name="newMode">Enum representing the new mode to switch to</param>
        private void setTextMode(TextMode newMode)
        {
            if (newMode == mode) { return; }
            //prep the controls for switch
            Control old = getCurrentEditor();
            Control current;
            if (newMode == TextMode.RTF)
            {
                //load the string into the RTF view
                rtfView.Document.Blocks.Clear();
                rtfView.Document.Blocks.Add(new Paragraph(new Run(txtView.Text)));
                current = rtfView;
            }
            else
            {
                //warn the user with a popup dialog

                //load the text
                string richText = new TextRange(rtfView.Document.ContentStart, rtfView.Document.ContentEnd).Text;
                txtView.Text = richText;
                current = txtView;
            }
            //switch the views
            current.IsEnabled = true;
            current.Visibility = System.Windows.Visibility.Visible;

            old.IsEnabled = false;
            old.Visibility = System.Windows.Visibility.Hidden;
            mode = newMode;
        }

        //================================UI click handlers==============================

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == toggleEdit)
            {
                toggleTextMode();
            }
        }
    }
}
