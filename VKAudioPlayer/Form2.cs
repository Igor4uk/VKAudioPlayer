using System;
using System.Windows.Forms;

namespace VKAudioPlayer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://oauth.vk.com/authorize?client_id=5122975&display=popup&redirect_uri=https://oauth.vk.com/blank.html&scope=audio&response_type=token&v=5.37");
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            toolStripStatusLabel1.Text = "Loading...";
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Success!";
            try
            {
                string url = webBrowser1.Url.ToString();
                string temp = url.Split('#')[1];
                if (temp[0] == 'a')
                {
                    Settings1.Default.token = temp.Split('&')[0].Split('=')[1];
                    Settings1.Default.id = temp.Split('=')[3];
                    Settings1.Default.auth = true;
                  //  MessageBox.Show(Settings1.Default.token + "" + Settings1.Default.id);
                    this.Close();
                }
            }
            catch (Exception)
            {

               // throw;
            }
           
        }
    }
}
