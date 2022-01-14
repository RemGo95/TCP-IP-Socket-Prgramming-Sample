using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Client
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public String OutputData;
        string ip = "192.168.73.1";
        string port = "12000";
        
        int _ConnectionErrorHandler=0;
        
        public override void Initialize()
        {
            base.Initialize();
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            string ipAndPort = ip + "\n" + port;
            ConnectionError.text = ipAndPort;

            foreach (IPAddress address in localIP)
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    //txtIP.Text = address.ToString();
                }
            }
        }
        
        public async Task SendData()
        {
            await Task.Run(() =>
            {
            

                client = new TcpClient();
                IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));

                try
                {
                    client.Connect(IpEnd);

                    if (client.Connected)
                    {
                        //txtLogs.AppendText("Connected to server" + "\n");
                        STW = new StreamWriter(client.GetStream());
                        STR = new StreamReader(client.GetStream());
                        STW.AutoFlush = true;

                        STW.WriteLine(OutputData);

                       // backgroundWorker1.RunWorkerAsync();
                       // backgroundWorker2.WorkerSupportsCancellation = true;
                        //txtLogs.Text += "Client Connected\n";
                        ConnectionError.text = ("Client Connected!!!").ToString();
                        //lblStatus.Text = "connected";

                    }
                    else
                    {
                        //txtLogs.Text += "Client did not connect\n";
                        ConnectionError.text = ("Problem with Connection!!!").ToString();
                    }
                }
                catch (Exception ex)
                {
                    //lblStatus.Text = "Error";
                    //txtLogs.Text += "Client could not connect\n>>>" + ex.Message.ToString() + "\n";
                    ConnectionError.text = ("Problem with Connection!!!").ToString();
                }


                Task.Delay(100).Wait();
            });
        }
        

        private void btnConnect_Click(object sender, EventArgs e)
        {

            client = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text));

            try
            {
                client.Connect(IpEnd);

                if (client.Connected)
                {
                    txtLogs.AppendText("Connected to server" + "\n");
                    STW = new StreamWriter(client.GetStream());
                    STR = new StreamReader(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                    txtLogs.Text += "Client Connected\n";
                    lblStatus.Text = "connected";

                }
                else
                {
                    txtLogs.Text += "Client did not connect\n";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error";
                txtLogs.Text += "Client could not connect\n>>>" + ex.Message.ToString() + "\n";
            }
        }

        
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(TextToSend);
                this.txtLogs.Invoke(new MethodInvoker(delegate ()
                {
                    txtLogs.AppendText("Me:" + TextToSend + "\n");
                    lblStatus.Text = "connected - sent";
                }));
            }
            backgroundWorker2.CancelAsync();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.txtLogs.Invoke(new MethodInvoker(delegate ()
                    {
                        txtLogs.AppendText("You:" + recieve + "\n");
                        lblStatus.Text = "connected";
                    }));
                    recieve = "";
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Error";
                    txtLogs.Text += "client could not connect\n>>>" + ex.Message.ToString() + "\n";
                }
            }
        }
    }
}
