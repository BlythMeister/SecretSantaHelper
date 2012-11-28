using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SecretSantaHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool addClicked = false;
        private Participant selectedParticipant;
        private SantaSack santaSack;

        public MainWindow()
        {
            InitializeComponent();
            lblVersion.Content = string.Format("Version: {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            santaSack = SantaSackSerializer.Deserialize();
            lstParticipants.ItemsSource = from participant in santaSack.Participants select participant.DisplayValue();
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
                if (MessageBox.Show("Do You Want To Clear Previous Items Before Importing?", "Clear Previous?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    santaSack.Participants.Clear();
                }

                try
                {
                    var dataRead = File.ReadLines(filePicker.FileName);
                    foreach (var dataLine in dataRead)
                    {
                        var dataParts = dataLine.Split(',');
                        if (!santaSack.Participants.Any(participant => participant.EmailAddress == dataParts[1]))
                        {
                            var participant = new Participant { Name = dataParts[0], EmailAddress = dataParts[1] };
                            santaSack.Participants.Add(participant);
                        }

                    }

                    lstParticipants.ItemsSource = null;
                    lstParticipants.ItemsSource = from participant in santaSack.Participants select participant.DisplayValue();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            SantaSackSerializer.Serialize(santaSack);
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
                    File.WriteAllLines(filePicker.FileName, santaSack.Participants.Select(participant => participant.OutputValue()).ToArray());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file to disk. Original error: " + ex.Message);
                }
            }
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var emailHelper = new RegexUtilities();
            if (!emailHelper.IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("That is an invalid email address!");
                return;
            }

            if (addClicked)
            {
                if (!santaSack.Participants.Any(participant => participant.EmailAddress.ToLower() == txtEmail.Text.ToLower()))
                {
                    var participant = new Participant { Name = txtName.Text, EmailAddress = txtEmail.Text };
                    santaSack.Participants.Add(participant);
                }
                else
                {
                    MessageBox.Show("Someone with this address exists already!");
                    return;
                }
            }
            else
            {
                if (!santaSack.Participants.Any(participant => participant != selectedParticipant && participant.EmailAddress.ToLower() == txtEmail.Text.ToLower()))
                {
                    selectedParticipant.EmailAddress = txtEmail.Text;
                    selectedParticipant.Name = txtName.Text;
                    selectedParticipant = null;
                }
                else
                {
                    MessageBox.Show("Someone with this address exists already!");
                    return;
                }
            }

            btnSendDetails.IsEnabled = true;
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

            lstParticipants.ItemsSource = null;
            lstParticipants.ItemsSource = from participant in santaSack.Participants select participant.DisplayValue();
            SantaSackSerializer.Serialize(santaSack);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnSendDetails.IsEnabled = true;
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
            btnSendDetails.IsEnabled = false;
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
            btnSendDetails.IsEnabled = false;
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
            selectedParticipant = santaSack.Participants.FirstOrDefault(participant => participant.DisplayValue() == lstParticipants.SelectedItem.ToString());
            if (selectedParticipant != null)
            {
                txtEmail.Text = selectedParticipant.EmailAddress;
                txtName.Text = selectedParticipant.Name;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            selectedParticipant = santaSack.Participants.FirstOrDefault(participant => participant.DisplayValue() == lstParticipants.SelectedItem.ToString());
            if (selectedParticipant != null)
            {
                santaSack.Participants.Remove(selectedParticipant);
                lstParticipants.ItemsSource = null;
                lstParticipants.ItemsSource = from participant in santaSack.Participants select participant.DisplayValue();
            }
            SantaSackSerializer.Serialize(santaSack);
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            SantaSackSerializer.Serialize(santaSack);

            if (santaSack.Participants.Count < 3)
            {
                MessageBox.Show("You Haven't Got Enough People!");
                return;
            }

            if (string.IsNullOrWhiteSpace(santaSack.Template.FromAddress))
            {
                MessageBox.Show("You Haven't Entered A From Address!");
                return;
            }

            if (string.IsNullOrWhiteSpace(santaSack.Template.Content))
            {
                MessageBox.Show("You Haven't Got Any Email Content!");
                return;
            }

            if (string.IsNullOrWhiteSpace(santaSack.Template.Subject))
            {
                MessageBox.Show("You Haven't Got An Email Subject!");
                return;
            }

            if (string.IsNullOrWhiteSpace(santaSack.Template.Host) || string.IsNullOrWhiteSpace(santaSack.Template.Port))
            {
                MessageBox.Show("You Haven't Got A Host Or Port For Sending!");
                return;
            }

            var emailHelper = new RegexUtilities();
            if (!emailHelper.IsValidEmail(santaSack.Template.FromAddress))
            {
                MessageBox.Show("You Haven't Entered A Valid From Address!");
                return;
            }

            foreach (var participant in santaSack.Participants)
            {
                if (!emailHelper.IsValidEmail(participant.EmailAddress))
                {
                    MessageBox.Show(participant.Name + " Does Not Have A Valid Email Address!");
                    return;
                }
            }

            var rsltMessageBox = MessageBox.Show("Are You Sure You Want To Send Now?", "Are You Sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rsltMessageBox == System.Windows.Forms.DialogResult.No)
            {
                MessageBox.Show("Sending Cancelled!");
                return;
            }

            var participantsToPair = (from p in santaSack.Participants select p).ToList();
            var participantsToAssign = (from p in santaSack.Participants select p).ToList();
            var participantsToSend = new List<PairedParticipant>();
            var randomGenerator = new Random((int)DateTime.UtcNow.Ticks);

            while (participantsToPair.Count > 0)
            {
                if (participantsToAssign.Count == 1 && participantsToPair.Count == 1 && participantsToAssign.First().EmailAddress == participantsToPair.First().EmailAddress)
                {
                    participantsToPair = (from p in santaSack.Participants select p).ToList();
                    participantsToAssign = (from p in santaSack.Participants select p).ToList();
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
                if (alreadyPaired != null)
                {
                    if (alreadyPaired.EmailAddress == participantPairing.PairedWith.EmailAddress)
                    {
                        participantsToPair = (from p in santaSack.Participants select p).ToList();
                        participantsToAssign = (from p in santaSack.Participants select p).ToList();
                        participantsToSend = new List<PairedParticipant>();
                    }
                }

            }

            while (participantsToSend.Any(x => x.Sent == false))
            {
                if(participantsToSend.Any(x=>x.SendAttempts > 5))
                {
                    foreach (var tooManyAttemptParticipant in participantsToSend.Where(x=>x.SendAttempts>5))
                    {
                        MessageBox.Show(
                            string.Format("Couldn't Send Email To {0}, They Drew {1}",
                                          tooManyAttemptParticipant.EmailAddress,
                                          tooManyAttemptParticipant.PairedWith.EmailAddress), "Couldn't Send",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

                foreach (var pairedParticipant in participantsToSend.Where(x => x.Sent == false))
                {
                    var message = new MailMessage();
                    message.From = new MailAddress(santaSack.Template.FromAddress);
                    message.To.Add(!string.IsNullOrWhiteSpace(santaSack.Template.DiagnosticDeliveryAddress)
                                       ? new MailAddress(santaSack.Template.DiagnosticDeliveryAddress)
                                       : new MailAddress(pairedParticipant.EmailAddress));

                    message.Subject = santaSack.Template.Subject;
                    message.Body = santaSack.Template.Content
                        .Replace("{{GiftFromName}}", pairedParticipant.Name)
                        .Replace("{{GiftFromEmail}}", pairedParticipant.EmailAddress)
                        .Replace("{{GiftForName}}", pairedParticipant.PairedWith.Name)
                        .Replace("{{GiftForEmail}}", pairedParticipant.PairedWith.EmailAddress);
                    var client = new SmtpClient();
                    client.Host = santaSack.Template.Host;
                    int port;
                    if (!int.TryParse(santaSack.Template.Port, out port))
                    {
                        port = 25;
                    }
                    client.Port = port;
                    try
                    {
                        client.Send(message);
                        pairedParticipant.Sent = true;
                    }
                    catch (Exception)
                    {
                        pairedParticipant.Sent = false;
                        pairedParticipant.SendAttempts++;
                    }
                }
            }

            MessageBox.Show("All Emails Have Been Sent!");
        }

        private void btnSendDetails_Click(object sender, RoutedEventArgs e)
        {
            var sendDetails = new SendDetails(ref santaSack);
            sendDetails.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SantaSackSerializer.Serialize(santaSack);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
