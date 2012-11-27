using System.Windows;

namespace SecretSantaHelper
{
    /// <summary>
    /// Interaction logic for SendDetails.xaml
    /// </summary>
    public partial class SendDetails : Window
    {
        private readonly SantaSack templateDetails;

        public SendDetails(ref SantaSack templateDetails)
        {
            this.templateDetails = templateDetails;
            InitializeComponent();
            txtSMTP.Text = templateDetails.Template.Host;
            txtPort.Text = templateDetails.Template.Port;
            txtFrom.Text = templateDetails.Template.FromAddress;
            txtSubject.Text = templateDetails.Template.Subject;
            txtContent.Text = templateDetails.Template.Content;
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
            Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            templateDetails.Template.Host = txtSMTP.Text;
            templateDetails.Template.Port = txtPort.Text;
            templateDetails.Template.FromAddress = txtFrom.Text;
            templateDetails.Template.Subject = txtSubject.Text;
            templateDetails.Template.Content = txtContent.Text;
        }
    }
}
