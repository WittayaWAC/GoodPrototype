using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {       
        public Form1()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;


            string fontsfolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts);
            System.Drawing.Text.PrivateFontCollection pfc = new System.Drawing.Text.PrivateFontCollection();
            try
            {
                pfc.AddFontFile(@"Kanit-Regular.ttf");
                Font = new Font(pfc.Families[0], 9, FontStyle.Regular);
                this.Font = Font;
            }
            catch
            {
                //
            }
          

        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Guid.NewGuid().ToString();
            label1.Text = textBox1.Text.ToUpper();

            changeCultureInfo("th");
            this.ActiveControl = label1;

            StartRunning();
           

        }

        private bool invokeInProgress = false;
        bool runnig = false;
        private object m_OperationMode = null;
        private Thread m_WorkerThread;
        private void StartRunning()
        {
            runnig = true;
            m_OperationMode = "TEST";
            m_WorkerThread = new Thread(new ThreadStart(ThreadRun));
            m_WorkerThread.SetApartmentState(ApartmentState.STA);
            m_WorkerThread.Start();
        }
        private void ThreadRun()
        {
            
            while (runnig)
            {
                Application.DoEvents();
                invokeInProgress = true;
                this.Invoke(new MethodInvoker(delegate {
                    this.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                }));
                invokeInProgress = false;
               
            }

        }


        private void changeCultureInfo(string lang)
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            this.Invoke(new Action(() =>
            {
                this.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            ));
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                textBox1.Text = Guid.NewGuid().ToString();
                label1.Text = textBox1.Text.ToUpper();
            }            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Button1_Click(object sender, EventArgs e)
        {           
            Application.Restart();
        }

        private async  void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (invokeInProgress)
            {
                e.Cancel = true;  // cancel the original event 
                runnig = false;// advise to stop taking new work

                // now wait until current invoke finishes
                await Task.Factory.StartNew(() =>
                {
                    while (invokeInProgress) ;
                });

                // now close the form
                this.Close();
            }

           
        }




        System.Threading.Timer tmrCheckFinishType = null;
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox origin = sender as TextBox;
            if (!origin.ContainsFocus)
                return;

            DisposeTimer();
            tmrCheckFinishType = new System.Threading.Timer(TimerElapsed, null, 1500, 1500);
        }
        private void TimerElapsed(Object obj)
        {
            CheckSyntaxAndReport();
            DisposeTimer();
        }

        private void DisposeTimer()
        {
            if (tmrCheckFinishType != null)
            {
                tmrCheckFinishType.Dispose();
                tmrCheckFinishType = null;
            }
        }

        private void CheckSyntaxAndReport()
        {
            this.Invoke(new Action(() =>
            {
                string s = textBox2.Text.ToUpper();
                s = s.Replace(" ", "");
                if (s != "")
                    textBox2.Text = ""; //MessageBox.Show(s);
            }
            ));
        }
    }
}
