using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SecretSantaHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Participant> participants = new List<Participant>();
        private bool addClicked = false;
        private Participant selectedParticipant;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream = null;
            var filePicker = new OpenFileDialog();
            filePicker.InitialDirectory = "c:\\";
            filePicker.Filter = "csv files (*.csv)|*.csv";
            filePicker.FilterIndex = 2;
            filePicker.RestoreDirectory = true;

            if (filePicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    var dataRead = File.ReadLines(filePicker.FileName);
                    foreach (var dataLine in dataRead)
                    {
                        var dataParts = dataLine.Split(',');
                        if (!participants.Any(participant => participant.EmailAddress == dataParts[1]))
                        {
                            var participant = new Participant { Name = dataParts[0], EmailAddress = dataParts[1] };
                            participants.Add(participant);
                            lstParticipants.Items.Add(participant.DisplayValue());
                        }

                    }
                    if (lstParticipants.Items.NeedsRefresh)
                    {
                        lstParticipants.Items.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new SaveFileDialog();
            if (filePicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    File.WriteAllLines(filePicker.FileName, participants.Select(participant => participant.OutputValue()).ToArray());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnAdd.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnRemove.IsEnabled = true;
            btnGo.IsEnabled = true;
            btnExport.IsEnabled = true;
            btnImport.IsEnabled = true;
            lstParticipants.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtName.IsEnabled = false;

            if(addClicked)
            {
                 if (!participants.Any(participant => participant.EmailAddress == txtEmail.Text))
                 {
                     var participant = new Participant {Name = txtName.Text, EmailAddress = txtEmail.Text};
                     participants.Add(participant);
                     lstParticipants.Items.Add(participant.DisplayValue());
                 }else
                 {
                     MessageBox.Show("Someone with this address exists already!");
                 }
            }
            else
            {
                if (!participants.Any(participant => participant.EmailAddress == txtEmail.Text))
                {
                    selectedParticipant.EmailAddress = txtEmail.Text;
                    selectedParticipant.Name = txtName.Text;
                    lstParticipants.SelectedItem = selectedParticipant.DisplayValue();
                    selectedParticipant = null;
                }
                else
                {
                    MessageBox.Show("Someone with this address exists already!");
                }
            }
            txtEmail.Text = "";
            txtName.Text = "";
            if(lstParticipants.Items.NeedsRefresh)
            {
                lstParticipants.Items.Refresh();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnAdd.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnRemove.IsEnabled = true;
            btnGo.IsEnabled = true;
            btnExport.IsEnabled = true;
            btnImport.IsEnabled = true;
            lstParticipants.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            txtEmail.IsEnabled = false;
            txtName.IsEnabled = false;
            txtEmail.Text = "";
            txtName.Text = "";
            selectedParticipant = null;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            btnAdd.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnRemove.IsEnabled = false;
            btnGo.IsEnabled = false;
            btnExport.IsEnabled = false;
            btnImport.IsEnabled = false;
            lstParticipants.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            txtEmail.IsEnabled = true;
            txtName.IsEnabled = true;
            addClicked = true;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            btnAdd.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnRemove.IsEnabled = false;
            btnGo.IsEnabled = false;
            btnExport.IsEnabled = false;
            btnImport.IsEnabled = false;
            lstParticipants.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            txtEmail.IsEnabled = true;
            txtName.IsEnabled = true;
            addClicked = false;
            selectedParticipant = participants.FirstOrDefault(participant => participant.DisplayValue() == lstParticipants.SelectedItem.ToString());
            if (selectedParticipant != null)
            {
                txtEmail.Text = selectedParticipant.EmailAddress;
                txtName.Text = selectedParticipant.Name;
            }
        }

    }
}
