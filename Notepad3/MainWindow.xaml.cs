﻿using System;
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
        CommonEditor editor;
        const string untitled_doc = "Untitled";
        public MainWindow()
        {
            InitializeComponent();
            //configure the view
            editor = new CommonEditor(rtfView,txtView);
            SetTitlebar(untitled_doc, editor.isSaved);
        }   

        //================================UI click handlers==============================

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == toggleEdit)
            {
                editor.ToggleTextMode();
            }
            else if (sender == NewFile)
            {
                editor.NewFile();
                SetTitlebar("Untitled", editor.isSaved);
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
            else if (sender == Exit)
            {
                Close();
            }
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
    }
}
