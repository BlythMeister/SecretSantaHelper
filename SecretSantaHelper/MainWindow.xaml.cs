using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
            lstParticipants.ItemsSource = from participant in participants select participant.DisplayValue();
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
                        }

                    }

                    lstParticipants.ItemsSource = null;
                    lstParticipants.ItemsSource = from participant in participants select participant.DisplayValue();
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
            filePicker.InitialDirectory = "c:\\";
            filePicker.Filter = "csv files (*.csv)|*.csv";
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

            if (addClicked)
            {
                if (!participants.Any(participant => participant.EmailAddress == txtEmail.Text))
                {
                    var participant = new Participant { Name = txtName.Text, EmailAddress = txtEmail.Text };
                    participants.Add(participant);
                }
                else
                {
                    MessageBox.Show("Someone with this address exists already!");
                }
            }
            else
            {
                if (!participants.Any(participant => participant != selectedParticipant && participant.EmailAddress == txtEmail.Text))
                {
                    selectedParticipant.EmailAddress = txtEmail.Text;
                    selectedParticipant.Name = txtName.Text;
                    selectedParticipant = null;
                }
                else
                {
                    MessageBox.Show("Someone with this address exists already!");
                }
            }
            txtEmail.Text = "";
            txtName.Text = "";

            lstParticipants.ItemsSource = null;
            lstParticipants.ItemsSource = from participant in participants select participant.DisplayValue();
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

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            selectedParticipant = participants.FirstOrDefault(participant => participant.DisplayValue() == lstParticipants.SelectedItem.ToString());
            if (selectedParticipant != null)
            {
                participants.Remove(selectedParticipant);
                lstParticipants.ItemsSource = null;
                lstParticipants.ItemsSource = from participant in participants select participant.DisplayValue();
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("You Haven't Entered The Password, Or It Is Incorrect");
                return;
            }

            try
            {
                var bytesToEncode = Encoding.UTF8.GetBytes(txtPassword.Password);
                var encodedText = Convert.ToBase64String(bytesToEncode);
                if (encodedText != "U2VjcmV0U2FudGE=")
                {
                    MessageBox.Show("You Haven't Entered The Password, Or It Is Incorrect");
                }

            }
            catch (Exception)
            {
                MessageBox.Show("You Haven't Entered The Password, Or It Is Incorrect");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFromAddress.Text))
            {
                MessageBox.Show("You Haven't Entered A From Address");
                return;
            }

            var emailHelper = new RegexUtilities();
            if (!emailHelper.IsValidEmail(txtFromAddress.Text))
            {
                MessageBox.Show("You Haven't Entered A Valid From Address");
                return;
            }

            foreach (var participant in participants)
            {
                if(!emailHelper.IsValidEmail(participant.EmailAddress))
                {
                    MessageBox.Show(participant.Name + " Does Not Have A Valid Email Address");
                    return;
                }
            }

            var participantsToPair = (from p in participants select p).ToList();
            var participantsToAssign = (from p in participants select p).ToList();
            var participantsToSend = new List<PairedParticipant>();
            var randomGenerator = new Random();

            while (participantsToPair.Count > 0)
            {
                if (participantsToAssign.Count == 1 && participantsToPair.Count == 1 && participantsToAssign.First().EmailAddress == participantsToPair.First().EmailAddress)
                {
                    participantsToPair = (from p in participants select p).ToList();
                    participantsToAssign = (from p in participants select p).ToList();
                    participantsToSend = new List<PairedParticipant>();
                }

                Participant pairParticipant = null;
                var assignIndex = randomGenerator.Next(0, participantsToAssign.Count);
                var assignParticipant = participantsToAssign[assignIndex];
                participantsToAssign.Remove(assignParticipant);
                var participantPairing = new PairedParticipant()
                                             {
                                                 EmailAddress = assignParticipant.EmailAddress,
                                                 Name = assignParticipant.Name
                                             };

                while (pairParticipant == null)
                {
                    var pairIndex = randomGenerator.Next(0, participantsToPair.Count);
                    pairParticipant = participantsToPair[pairIndex];
                    if (assignParticipant.EmailAddress == pairParticipant.EmailAddress)
                    {
                        pairParticipant = null;
                    }
                    else
                    {
                        participantsToPair.Remove(pairParticipant);
                        participantPairing.PairedWith = pairParticipant;
                        participantsToSend.Add(participantPairing);
                    }
                }

                var alreadyPaired =
                    participantsToSend.FirstOrDefault(p => p.PairedWith.EmailAddress == participantPairing.EmailAddress);
                if(alreadyPaired != null)
                {
                    if(alreadyPaired.EmailAddress == participantPairing.PairedWith.EmailAddress)
                    {
                        participantsToPair = (from p in participants select p).ToList();
                        participantsToAssign = (from p in participants select p).ToList();
                        participantsToSend = new List<PairedParticipant>();
                    }
                }

            }

            foreach (var pairedParticipant in participantsToSend)
            {
                var message = new MailMessage();
                message.From = new MailAddress(txtFromAddress.Text);
                message.To.Add(new MailAddress(pairedParticipant.EmailAddress));
                message.Subject = "Your Secret Santa has been selected";
                message.Body = string.Format("Dear {0}\n\nSanta has selected you to purchase a gift for {1} {2}\n\nRemember:\n- You have a budget of £5 to spend on any gift you wish...the more outrageous, the better ;)\n- Gifts must be labelled and left under the Christmas tree on or before the 7th December\n\nOn 7th December @ The North Laine Pub, Santa’s sack will be out and you will receive your gift\n\nHappy spending!\n\nFrom Santa’s Elf", pairedParticipant.Name, pairedParticipant.PairedWith.Name, pairedParticipant.PairedWith.EmailAddress);
                var client = new SmtpClient();
                client.Send(message);
            }

            MessageBox.Show("All Emails Have Been Sent");
        }
    }
}
