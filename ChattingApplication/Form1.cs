using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;


namespace ChattingApplication
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private string RecievedText;
        private string SendText;


        public Form1()
        {
            InitializeComponent();

            IPAddress[] ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in ipAddresses)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    txtToIP.Text = ip.ToString();
                    txtFromIP.Text = ip.ToString();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void WriteToChat(string Message)
        {
            lsChatView.Invoke(new MethodInvoker(delegate () { lsChatView.Items.Add(Message); }));
        }


        // to start listening on port 1000 
        private void btnStart_Click(object sender, EventArgs e)
        {

            WriteToChat("Started Listening");
            // asynchronous function to start listening and connect to the client
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 1000);
            tcpListener.Start();
            client = tcpListener.AcceptTcpClient();
            if (client != null)
            {
                MessageBox.Show(client.ToString());
                WriteToChat("connected to a client");

                

                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.CancelAsync();



                backgroundWorker2.WorkerSupportsCancellation = true;
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                byte[] b = new byte[256];
                int i = client.GetStream().Read(b,0,256);
                if (b != null)
                {
                    string s = Encoding.Default.GetString(b);
                    WriteToChat(s + "");
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            client.Connect(IPAddress.Parse(txtToIP.Text),1000);
            byte[] b = Encoding.ASCII.GetBytes("hello");
            client.GetStream().Write(b,0,b.Length);
            backgroundWorker3.RunWorkerAsync();

            WriteToChat("Connected to server");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                byte[] b = Encoding.ASCII.GetBytes(txtMessage.Text);
                client.GetStream().Write(b,0,b.Length);
                WriteToChat(txtMessage.Text);
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                byte[] b = new byte[256];
                int i = client.GetStream().Read(b, 0, 256);
                if (b != null)
                {
                    string s = Encoding.Default.GetString(b);
                    WriteToChat(s + "");
                }
            }
        }
    }
}
