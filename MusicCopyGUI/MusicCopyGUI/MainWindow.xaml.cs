using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace MusicCopyGUI
{
    public partial class MainWindow : Window
    {
        private BackgroundWorker thread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void sourceDirectoryTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.sourceDirectoryTextBox.Text = fbd.SelectedPath;
            }
        }

        private void destinyDirectoryTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.destinyDirectoryTextBox.Text = fbd.SelectedPath;
            }
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            thread = new BackgroundWorker();
            thread.WorkerSupportsCancellation = true;
            thread.WorkerReportsProgress = true;
            thread.ProgressChanged += new ProgressChangedEventHandler(thread_ProgressChanged);
            thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(thread_RunWorkerCompleted);
            thread.DoWork += new DoWorkEventHandler(thread_DoWork);
            
            thread.RunWorkerAsync(new string[]{this.sourceDirectoryTextBox.Text, this.destinyDirectoryTextBox.Text});

            this.progressBar.Value = 0;
            this.copyButton.IsEnabled = false;
            this.stopButton.IsEnabled = true;
        }

        private void thread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void thread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.stopButton.IsEnabled = false;
            this.copyButton.IsEnabled = true;
        }
        
        private void thread_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] argument = (string[])e.Argument;

            if(Directory.Exists(argument[0]) && Directory.Exists(argument[1]))
            {
                string[] files = Directory.GetFiles(argument[0], "*", SearchOption.AllDirectories);
                int numberOfFiles = files.Length;
                int counter = 1;

                for (int x = 0; x < numberOfFiles; x++)
                {
                    if (thread.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    FileInfo file = new FileInfo(files[x]);
                    
                    if(file.Extension.Equals(".mp3"))
                        file.CopyTo(String.Format(@"{0}\{1}{2}", argument[1], counter++, file.Extension));

                    thread.ReportProgress(((x + 1) * 100) / numberOfFiles);
                }
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            this.thread.CancelAsync();
        }
    }
}
