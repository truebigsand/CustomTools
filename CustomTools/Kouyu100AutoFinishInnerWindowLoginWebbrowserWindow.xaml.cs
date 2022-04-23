using System;
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

namespace CustomTools
{
    /// <summary>
    /// Kouyu100AutoFinishInnerWindowLoginWebbrowserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Kouyu100AutoFinishInnerWindowLoginWebbrowserWindow : Window
    {
        /// <summary>
        /// 设置浏览器静默，不弹错误提示框，
        /// 来自https://blog.csdn.net/jumtre/article/details/40184505
        /// </summary>
        /// <param name="webBrowser">要设置的WebBrowser控件浏览器</param>
        /// <param name="silent">是否静默</param>
        private void SetWebBrowserSilent(WebBrowser webBrowser, bool silent)
        {
            System.Reflection.FieldInfo fi = typeof(WebBrowser).GetField("_axIWebBrowser2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (fi != null)
            {
                object browser = fi.GetValue(webBrowser);
                if (browser != null)
                    browser.GetType().InvokeMember("Silent", System.Reflection.BindingFlags.SetProperty, null, browser, new object[] { silent });
            }
        }
        public Kouyu100AutoFinishInnerWindowLoginWebbrowserWindow()
        {
            InitializeComponent();
            SetWebBrowserSilent(MainWebbrowser, true);
        }
        public string GetAuthToken()
        {
            string cookieString = Application.GetCookie(new Uri("https://028.kouyu100.com/njjlzxhx/index.jsp"));
            for (int i = 0; i < cookieString.Length; i++)
            {
                if (cookieString[i] == ' ')
                {
                    cookieString.Remove(i);
                }
            }
            string? authTokenLine = cookieString
                .Split(';')
                .ToList()
                .Find(str => str.Contains("authToken"));
            if (authTokenLine == null)
            {
                MessageBox.Show("内置登录失败! 未成功获取Cookie->authToken! ");
                return "";
            }
            return authTokenLine.Split('=')[1];
        }
    }
}
