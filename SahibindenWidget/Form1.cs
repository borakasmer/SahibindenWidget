using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WatiN.Core;
using DAL;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace SahibindenWidget
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            System.Windows.Forms.Application.Exit();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Visible = false;
        }

        public bool isToolMenuShowen = true;
        public void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (isToolMenuShowen)
            {
                contextMenuStrip1.Show(System.Windows.Forms.Control.MousePosition);
                isToolMenuShowen = false;
            }
            else
            {
                contextMenuStrip1.Hide();
                isToolMenuShowen = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Settings.Instance.MakeNewIeInstanceVisible = false;
            Settings.AutoCloseDialogs = true;
            GetSahibindenCars();
            timer1.Start();
        }

        public void GetSahibindenCars()
        {
            Settings.AttachToBrowserTimeOut = 240; //240 seconds
            Settings.WaitUntilExistsTimeOut = 240;
            Settings.WaitForCompleteTimeOut = 240;

            string url = "http://www.sahibinden.com/otomobil";
            try
            {
                List<SahibindenCars> datas;
                using (var browser = new IE(url))
                {
                    datas = new List<SahibindenCars>();
                    browser.WaitForComplete();
                    System.Threading.Thread.Sleep(2000);
                    browser.Table(Find.ById("searchResultsTable")).WaitUntilExists();
                    foreach (TableRow trow in browser.Table(Find.ById("searchResultsTable")).TableRows.Skip(1).Take(6))
                    {                        
                        SahibindenCars data = new SahibindenCars();
                        data.ImageUrl = trow.TableCells[0].Images[0].Src;
                        data.Title = trow.TableCells[1].Text;
                        data.Year = int.Parse(trow.TableCells[2].Text ?? "0");
                        data.Km = int.Parse(trow.TableCells[3].Text == null ? "0" : trow.TableCells[3].Text.Replace(".", "").Trim());
                        data.Color = trow.TableCells[4].Text;
                        data.Price = trow.TableCells[5].Text;
                        IFormatProvider theCultureInfo = new System.Globalization.CultureInfo("tr-TR", true);
                        //data.Date = DateTime.Parse(trow.TableCells[6].Text);
                        data.Date =DateTime.ParseExact(trow.TableCells[6].Text.Trim().Replace("\r\n", string.Empty), "dd MMMM yyyy", theCultureInfo);
                        data.Place = trow.TableCells[7].Text;
                        datas.Add( data);                       
                    }                    
                }
                bool isChange = false;
                cacheDatas = null; //Tekrardan database silinince yenilenmesi için koyduk.
                if (cacheDatas == null) { FillCache(); }
                foreach (SahibindenCars item in datas)
                {
                    if (!isDataChange(item))
                    {
                        isChange = true;
                    }
                }
                if (isChange)
                {
                    FillCache();
                    using (Banner dbContext = new Banner())
                    {
                        dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE SahibindenCars");
                        dbContext.SahibindenCars.AddRange(((IEnumerable<SahibindenCars>) datas));
                        dbContext.SaveChanges();
                    }

                    //Triger SignalR                      
                    Triger();

                }
            }
            catch (Exception ex)
            {               
                int i = 0;
            }
        }

        public async void Triger()
        {
            try
            {
                //var hubConnection = new HubConnection("http://localhost:39325");
                var hubConnection = new HubConnection("http://sahibindenbanner2.azurewebsites.net/");            
                IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy("GetCars");
                await hubConnection.Start(new LongPollingTransport());
                stockTickerHubProxy.Invoke("RefreshCars");
            }
            catch (Exception ex)
            {
                int i = 0;
            }
        }

        public List<SahibindenCars> cacheDatas;
        public void FillCache()
        {
            if (cacheDatas == null)
            {
                cacheDatas = new List<SahibindenCars>();
            }
            else
            {
                cacheDatas.Clear();
            }
            using (Banner dbContext = new Banner())
            {
                cacheDatas = dbContext.SahibindenCars.ToList();
            }
        }

        public bool isDataChange(SahibindenCars data)
        {
            return cacheDatas.Any(cd => cd.ImageUrl == data.ImageUrl);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            GetSahibindenCars();
            timer1.Start();
        }
    }
}
