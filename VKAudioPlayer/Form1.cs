using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace VKAudioPlayer
{
    public partial class Form1 : Form
    {
        private List<Audio> audioList;
        WMPLib.IWMPPlaylist playList;
        WMPLib.IWMPMedia media;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            new Form2().Show();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Settings1.Default.auth)
            {
                Thread.Sleep(500);
            }
            string responseFromServer;
            WebRequest request = WebRequest.Create("https://api.vk.com/method/audio.get?owner_id=" + Settings1.Default.id + "&need_user=0&access_token=" + Settings1.Default.token);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            responseFromServer = HttpUtility.HtmlDecode(responseFromServer);

            JToken token = JToken.Parse(responseFromServer);
            audioList = token["response"].Children().Skip(1).Select(c => c.ToObject<Audio>()).ToList();
            
            this.Invoke((MethodInvoker)delegate
            {
                playList = axWindowsMediaPlayer1.playlistCollection.newPlaylist("vk");


                Text += " :: " + audioList.Count() + " songs load";
                for (int i = 0; i < audioList.Count(); i++)
                {

                    media = axWindowsMediaPlayer1.newMedia(audioList[i].url);
                    playList.appendItem(media);

                    listBox1.Items.Add(audioList[i].artist + " - " + audioList[i].title);
                }

                axWindowsMediaPlayer1.currentPlaylist = playList;
                axWindowsMediaPlayer1.Ctlcontrols.stop() ;
            });
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
            axWindowsMediaPlayer1.Ctlcontrols.currentItem = axWindowsMediaPlayer1.currentPlaylist.get_Item(listBox1.SelectedIndex);
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case 1:
                    textBox1.Text = "Stopped";
                    break;
                case 2:
                    textBox1.Text = "Pause";
                    break;
                case 3:
                    for (int i = 0; i < axWindowsMediaPlayer1.currentPlaylist.count; i++)
                    {
                        if (axWindowsMediaPlayer1.currentMedia.isIdentical[axWindowsMediaPlayer1.currentPlaylist.Item[i]])
                        {
                            textBox1.Text = audioList[i].artist + " - " + audioList[i].title;
                            listBox1.SelectedIndex = i;
                            break;
                        }
                    }
                   
                    
                    break;
                case 6:
                    textBox1.Text = "Buffering... so slow internet........";
                    break;
            default:
                   // textBox1.Text = "";
                    break;
            }
             
        }
    } 
    }

