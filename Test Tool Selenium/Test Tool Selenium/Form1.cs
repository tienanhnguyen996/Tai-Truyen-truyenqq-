using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Test_Tool_Selenium
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowNewFolderButton = true;
            DialogResult dialogResult = folder.ShowDialog();
            if(dialogResult == DialogResult.OK)
            {
                path.Text = folder.SelectedPath;
            }
        }

        ChromeDriver chrome;

        private void button2_Click(object sender, EventArgs e)
        {
            new Thread(() => {
                link.Enabled = false;
                button2.Enabled = false;
                soLuong.Enabled = false;
                if (link.Text.Equals(""))
                {
                    MessageBox.Show("Vui lòng nhập link");
                }
                else
                {
                    if (soLuong.Text.Equals(""))
                    {
                        MessageBox.Show("Vui lòng nhập số lượng chap cần tải");
                    }
                    else
                    {
                        chrome = new ChromeDriver();
                        chrome.Url = link.Text;
                        chrome.Navigate();
                        for (int i = 0; i < Int16.Parse(soLuong.Text); i++)
                        {
                            WaitToLoad(chrome, By.ClassName("lazy"));
                            chrome.ExecuteScript("console.log('Load Page Done');");
                            createDirectory(chrome);
                            getImage(chrome, i);
                            string size = "\\" + xuli(chrome.FindElementByClassName("detail-title").Text);
                            path.Text = path.Text.Substring(0, path.Text.Length - size.Length);
                            try
                            {
                                chrome.Url = chrome.FindElementByClassName("link-next-chap").GetAttribute("href");
                                chrome.Navigate();
                            }
                            catch(Exception)
                            {
                                MessageBox.Show("Không Tìm Thấy Link Chap Mới. Kêt Thúc Việc Tải");
                                break;
                            }
                        }
                        chrome.Close();
                        chrome.Quit();
                    }
                }
                link.Enabled = true;
                button2.Enabled = true;
                soLuong.Enabled = true;
                MessageBox.Show("Done");
            })
            { IsBackground = true }.Start();    
        }

        private string xuli(string title)
        {
            title = title.Replace(':', ' ');
            title = title.Replace('<', ' ');
            title = title.Replace('>', ' ');
            title = title.Replace('?', ' ');
            title = title.Replace('*', ' ');
            title = title.Replace('/', ' ');
            title = title.Replace('\\', ' ');
            title = title.Replace('|', ' ');
            return title;
        }

        private void createDirectory(ChromeDriver chrome)
        {
            string dirPath = path.Text = path.Text+ "\\" + xuli(chrome.FindElementByClassName("detail-title").Text);
            bool exist = Directory.Exists(dirPath);
            // Nếu không tồn tại, tạo thư mục này.
            if (!exist)
            {
                // Tạo thư mục.
                Directory.CreateDirectory(dirPath);
            }
            else
            {
            }
        }

        private void getImage(ChromeDriver chrome,int a)
        {
            var img = chrome.FindElementsByClassName("lazy");
            for(int i=0;i<img.Count;i++)
            {
                DowloadImage(img[i].GetAttribute("src"), i);   
            }
        }

        public void DowloadImage(string url,int i)
        {
            new Thread(() => {
                try
                {
                    SaveImage(url, path.Text + "/" + i.ToString() + ".png", ImageFormat.Png);
                }
                catch (ExternalException)
                {
                    // Something is wrong with Format -- Maybe required Format is not 
                    // applicable here
                }
                catch (ArgumentNullException)
                {
                    // Something wrong with Stream
                }
            }){ IsBackground = true }.Start();
        }

        public void SaveImage(string imageUrl,string filename, ImageFormat format)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                bitmap.Save(filename, format);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }

        private void soLuong_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(soLuong.Text, "[^0-9]"))
            {
                MessageBox.Show("Vui lòng chỉ nhập số.");
                soLuong.Text = soLuong.Text.Remove(soLuong.Text.Length - 1);
            }
        }

        public bool WaitToLoad(ChromeDriver chrome,By by)
        {
            int i = 0;
            while (i < 600)
            {
                i++;
                Thread.Sleep(100); // sleep 100 ms
                try
                {
                    chrome.FindElement(by);
                    break;
                }
                catch { }
            }
            if (i == 600) return false; // page load failed in 1 min
            else return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        /*
        private void button1_Click(object sender, EventArgs e)
        {
            ChromeDriver chromeDriver = new ChromeDriver();
            chromeDriver.Url = "https://www.instagram.com/?hl=vi";
            chromeDriver.Navigate();
            Thread thread = new Thread(() =>
            {
                chromeDriver.Manage().Window.Maximize();
                Thread.Sleep(1000);
                chromeDriver.ExecuteScript("document.getElementsByClassName('sqdOP L3NKy   y3zKF')[0].click();");
                loginfb(chromeDriver);
                Thread.Sleep(1000);
                chromeDriver.Url = "https://golike.site/login";
                Login(chromeDriver);
                Thread.Sleep(500);
                chromeDriver.Url = "https://golike.site/jobs/instagram";
                Thread.Sleep(1000);
                chromeDriver.ExecuteScript("document.getElementsByClassName('col text-right')[0].click();");
                Thread.Sleep(1000);
                //chromeDriver.ExecuteScript("document.getElementsByClassName('btn bg-button - 1 px - 0 btn - block')[0].click();");
                MessageBox.Show(getJob(chromeDriver));

            });
            thread.IsBackground = true;
            thread.Start();
        }
        private bool IsElementPresent(By by, ChromeDriver driver)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        private void Login(ChromeDriver chromeDriver)
        {
            var input = chromeDriver.FindElementsByClassName("form-control");
            input[0].SendKeys("tienanh511");
            input[1].SendKeys("Nguyen123");
            chromeDriver.ExecuteScript("document.getElementsByClassName('btn bg-gradient-1 py-2 border-0 text-light btn-block')[0].click();");
        }
        private void loginfb(ChromeDriver chromeDriver)
        {
            var email = chromeDriver.FindElementByName("email");
            email.SendKeys("0932563378");
            var pass = chromeDriver.FindElementByName("pass");
            pass.SendKeys("Matkhau123");
            if(IsElementPresent(By.Name("login"),chromeDriver))
            {
                var submit = chromeDriver.FindElementByName("login");
                submit.Click();
            }
            else
            {
                var login = chromeDriver.FindElementById("loginbutton");
                login.Click();
            }
            //chromeDriver.ExecuteScript("document.getElementsByClassName('sqdOP  L3NKy   y3zKF     ')[0].click();");
        }
        
        private string getJob(ChromeDriver chromeDriver)
        {
            IJavaScriptExecutor js = chromeDriver as IJavaScriptExecutor;
            var a = (string)js.ExecuteScript("var a = document.getElementsByClassName('ml-1 d400')[0].innerText;return a;");
            if(a == "")
            {
                MessageBox.Show("");
            }
            return a;
        }*/
    }
}
