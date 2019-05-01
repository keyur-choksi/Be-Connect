using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation; 

namespace Be_Connect_
{
    public partial class frm_Home : Form
    {
        public frm_Home()
        {
            InitializeComponent();
        }
        public static string Username = null;
        public static IPAddress Userip = null;
        public static string MyInput = "";
        
        private void frm_Home_Load(object sender, EventArgs e)
        {
            Username = null;
            lbl_Username.Text = Environment.UserName;
            lbl_PCname.Text = "PC Name: " + Environment.MachineName;
            lbl_IPAddress.Text = "IP Address: " + System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName.ToString();
            
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                lbl_Status.Text = "Network Status : Connected";
            }
            else lbl_Status.Text = "Network Status : Disconnected";
            string Domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            if (Domain == "")
            {
                lbl_Domain.Text = "Domain : None";
            }
            else { lbl_Domain.Text = "Domain : "+Domain; }
            
            LoadIPaddress();
            
            if (DGV_LoadUsers.Rows.Count == 0)
            {
                MessageBox.Show("You won't be able to access features\n please turn on \"Network Discovery\"","Be Connect+ - Network Discovery issue",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        
        void LoadIPaddress()
        {
            this.Cursor = Cursors.WaitCursor;
            Process netUtility = new Process();
            netUtility.StartInfo.FileName = "arp";
            netUtility.StartInfo.CreateNoWindow = true;
            netUtility.StartInfo.Arguments = "-a";
            netUtility.StartInfo.RedirectStandardOutput = true;
            netUtility.StartInfo.UseShellExecute = false;
            netUtility.StartInfo.RedirectStandardError = true;
            netUtility.Start();

            StreamReader streamReader = new StreamReader(netUtility.StandardOutput.BaseStream, netUtility.StandardOutput.CurrentEncoding);

            String line = "",pcname="", ipaddr;

            streamReader.ReadLine();
            streamReader.ReadLine(); //Skipping First 3 Lines
            streamReader.ReadLine();

            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.Substring(line.Length - 10, 1) == "d") //Allowing only Dynamic IPs
                {
                    try
                    {
                        ipaddr = line.Substring(2).Substring(0, line.Substring(2).IndexOf(" "));
                        pcname = System.Net.Dns.GetHostEntry(ipaddr).ToString();
                        DGV_LoadUsers.Rows.Add(pcname, ipaddr);
                        
                    }
                    catch
                    {
                        //If no such host found, skip it
                    }
                }
            }
            MessageBox.Show("Done Load final");
            streamReader.Close();
            netUtility.WaitForExit(1);
            DGV_LoadUsers.ClearSelection();
            this.Cursor = Cursors.Default;
            
        }

        public void frm_Home_Resize(object sender, EventArgs e)
        {
            label1.Location = new Point(panel1.Width / 2 - label1.Width / 2, label1.Location.Y);
            label3.Location = new Point(panel3.Width / 2 - label1.Width / 2, label3.Location.Y);
        }
        private void DGV_LoadUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                Username = DGV_LoadUsers.Rows[e.RowIndex].Cells[0].Value.ToString();
                Userip = IPAddress.Parse(DGV_LoadUsers.Rows[e.RowIndex].Cells[1].Value.ToString());
            }
        }
        
        private void DGV_LoadUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
            
            if (DGV_LoadUsers.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
               (new frm_InputForm()).ShowDialog();              
               ((DataGridViewTextBoxCell)DGV_LoadUsers.Rows[e.RowIndex].Cells[2]).Value = MyInput;
            }
        }
    }
}
