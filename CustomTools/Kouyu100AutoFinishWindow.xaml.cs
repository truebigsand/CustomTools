﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
//using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.ComponentModel;

namespace CustomTools
{
    public class Homework
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public int ExamId { get; set; }
        public int HomeworkId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
    /// <summary>
    /// Kouyu100.xaml 的交互逻辑
    /// </summary>
    public partial class Kouyu100AutoFinishWindow : Window
    {
        public Kouyu100AutoFinishWindow()
        {
            InitializeComponent();
        }
        public string Kouyu100HttpGet(string Url)
        {
            //Create
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            AuthTokenTextBox.Dispatcher.Invoke(delegate
            {
                request.Headers.Add("Cookie", $"authToken={AuthTokenTextBox.Text}");
            });
            //Receive
            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
            string resultString = streamReader.ReadToEnd();
            //Clean up
            streamReader.Close();
            responseStream.Close();
            return resultString;
        }
        //引用自https://www.jb51.net/article/177025.htm
        public string Kouyu100HttpPost(string Url, string postDataStr)
        {
            //Create
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            AuthTokenTextBox.Dispatcher.Invoke(delegate
            {
                request.Headers.Add("Cookie", $"authToken={AuthTokenTextBox.Text}");
            });
            Encoding encoding = Encoding.UTF8;
            byte[] postData = encoding.GetBytes(postDataStr);
            request.ContentLength = postData.Length;
            //Send
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            //Receive
            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, encoding);
            string resultString = streamReader.ReadToEnd();
            //Clean up
            requestStream.Close();
            streamReader.Close();
            responseStream.Close();
            return resultString;
        }
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        static bool isOver = true;
        private void GetHomeworkListButton_Click(object sender, RoutedEventArgs e)
        {
            isOver = false;
            Task.Run(delegate
            {
                GetHomeworkListButton.Dispatcher.BeginInvoke(new Action(delegate
                {
                    GetHomeworkListButton.IsEnabled = false;
                }));
                HomeworkListBox.Dispatcher.BeginInvoke(new Action(delegate
                {
                    HomeworkListBox.Items.Clear();
                }));
                try
                {
                    string httpResult = Kouyu100HttpPost("http://028.kouyu100.com/njjlzxhx/findHomeWorkListAll.action", "goToPage=1&type=all&state=0");
                    JObject responseJson = (JObject)JsonConvert.DeserializeObject(httpResult);
                    int pageNumber = (int)responseJson["totalPage"];
                    for (int i = 1; i <= pageNumber; i++)
                    {
                        try
                        {
                            string result = Kouyu100HttpPost("http://028.kouyu100.com/njjlzxhx/findHomeWorkListAll.action", $"goToPage={i}&type=all&state=0");
                            JObject json = (JObject)JsonConvert.DeserializeObject(result);
                            foreach (JObject j in json["info"])
                            {
                                string url = (string)j["url"];
                                var splitResult1 = url.Split('=');
                                int ExamId = int.Parse(splitResult1[1].Split('&')[0]);
                                int HomeworkId = int.Parse(splitResult1[2]);
                                string Content = (string)j["content"];
                                HomeworkListBox.Dispatcher.BeginInvoke(new Action(delegate
                                {
                                    HomeworkListBox.Items.Add(new Homework()
                                    {
                                        ExamId = ExamId,
                                        HomeworkId = HomeworkId,
                                        Name = Content.Split('\'')[1],
                                        Status = (int)j["status"] == 2 ? "已完成" : "未完成"
                                    });
                                }));
                            }
                        }
                        catch (FormatException ex)
                        {
                            // 暂不支持的作业类型
                        }
                        catch (OverflowException ex)
                        {
                            // 暂不支持的作业类型
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                GetHomeworkListButton.Dispatcher.BeginInvoke(new Action(delegate
                {
                    GetHomeworkListButton.IsEnabled = true;
                }));

            });
            isOver = true;
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning restore CS8602 // 解引用可能出现空引用。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
        }

        private void HomeworkListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Kouyu100AutoFinishInnerWindow kouyu100AutoFinishInnerWindow = new Kouyu100AutoFinishInnerWindow((Homework)HomeworkListBox.SelectedItem, $"authToken={AuthTokenTextBox.Text}");
            if (kouyu100AutoFinishInnerWindow.PrepareSuccessfully)
            {
                kouyu100AutoFinishInnerWindow.Owner = this;
                kouyu100AutoFinishInnerWindow.ShowDialog();
            }
        }

        private void InnerWebbrowserLoginButton_Click(object sender, RoutedEventArgs e)
        {
            Kouyu100AutoFinishInnerWindowLoginWebbrowserWindow kouyu100AutoFinishInnerWindowLoginWebbrowserWindow = new Kouyu100AutoFinishInnerWindowLoginWebbrowserWindow();
            kouyu100AutoFinishInnerWindowLoginWebbrowserWindow.Owner = this;
            MessageBox.Show("请在接下来弹出的窗口内登录, 登陆成功后关闭即可");
            kouyu100AutoFinishInnerWindowLoginWebbrowserWindow.ShowDialog();
            AuthTokenTextBox.Text = kouyu100AutoFinishInnerWindowLoginWebbrowserWindow.GetAuthToken();
            if (AuthTokenTextBox.Text != "")
            {
                AuthTokenTextBox.IsEnabled = false;
                GetHomeworkListButton.IsEnabled = true;
                GetHomeworkListButton.Opacity = 1;
            }
        }

        private void Clear_state_Click(object sender, RoutedEventArgs e)
        {
            if (isOver)
            {
                AuthTokenTextBox.IsEnabled = true;
                GetHomeworkListButton.IsEnabled = false;
                GetHomeworkListButton.Opacity = 0.5;
                for (int index = 0; index < 10; index++)
                {
                    for (int i = 0; i < HomeworkListBox.Items.Count; i++)
                    {
                        HomeworkListBox.Items.RemoveAt(i);
                    }
                }

            }
            else
            {
                MessageBox.Show("正在读取中，无法关闭");
            }
        }
    }
}