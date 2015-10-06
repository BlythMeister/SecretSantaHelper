using System.Windows;

namespace SecretSantaHelper
{
    /// <summary>
    /// Interaction logic for SendDetails.xaml
    /// </summary>
    public partial class SendDetails : Window
    {
        private readonly SantaSack santaSack;

        public SendDetails(ref SantaSack santaSack)
        {
            this.santaSack = santaSack;
            InitializeComponent();
            txtSMTP.Text = santaSack.Template.Host;
            txtPort.Text = santaSack.Template.Port;
            txtFrom.Text = santaSack.Template.FromAddress;
            txtSubject.Text = santaSack.Template.Subject;
            txtContent.Text = santaSack.Template.Content;
            txtDiagnostic.Text = santaSack.Template.DiagnosticDeliveryAddress;
            enableSSL.IsChecked = santaSack.Template.EnableSsl;
            txtBlindCarbonCopy.Text = santaSack.Template.BlindCarbonCopy;
            txtFrom_Pass.Password = santaSack.Template.FromPassword;
            btnDone.IsEnabled = true;
        }

        public SendDetails()
        {
            InitializeComponent();
            btnDone.IsEnabled = false;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            var emailHelper = new RegexUtilities();
            if (!emailHelper.IsValidEmail(txtFrom.Text))
            {
                MessageBox.Show("That is an invalid from email address!");
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtDiagnostic.Text) && !emailHelper.IsValidEmail(txtDiagnostic.Text))
            {
                MessageBox.Show("That is an invalid diagnostic email address!");
                return;
            }
            Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            santaSack.Template.Host = txtSMTP.Text;
            santaSack.Template.Port = txtPort.Text;
            santaSack.Template.FromAddress = txtFrom.Text;
            santaSack.Template.Subject = txtSubject.Text;
            santaSack.Template.Content = txtContent.Text;
            santaSack.Template.DiagnosticDeliveryAddress = txtDiagnostic.Text;
            santaSack.Template.FromPassword = txtFrom_Pass.Password;
            santaSack.Template.EnableSsl = enableSSL.IsChecked.GetValueOrDefault();
            santaSack.Template.BlindCarbonCopy = txtBlindCarbonCopy.Text;
        }
    }
}
