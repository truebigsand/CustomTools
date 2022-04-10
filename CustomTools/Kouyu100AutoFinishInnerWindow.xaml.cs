using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CustomTools
{
    /// <summary>
    /// Kouyu100AutoFinishInnerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Kouyu100AutoFinishInnerWindow : Window
    {
        private readonly Homework currentHomework;
        string cookieString;
        int ScoreId;
        public Kouyu100AutoFinishInnerWindow(Homework currentHomework, string cookieString)
        {
            InitializeComponent();
            this.currentHomework = currentHomework;
            this.cookieString = cookieString;
            HomeworkTitleTextBlock.Text = $"标题: {currentHomework.Name}";
            ExamIdTextBlock.Text = $"ExamId: {currentHomework.ExamId}";
            HomeworkIdTextBlock.Text = $"HwId: {currentHomework.HomeworkId}";
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            ScoreId = (int)((
                (JObject)JsonConvert.DeserializeObject(
                    Kouyu100HttpGet($"http://028.kouyu100.com/njjlzxhx/getTimeAndAnswer.action?examId={currentHomework.ExamId}&homeWork.id={currentHomework.HomeworkId}")
                )
            )["listenExamScore"]["id"]);
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8602 // 解引用可能出现空引用。
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
        }
        public string Kouyu100HttpGet(string Url)
        {
            //Create
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.AllowAutoRedirect = false;
            request.Headers.Add("Cookie", cookieString);
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
        public string Kouyu100HttpPost(string Url, string postDataStr)
        {
            //Create
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Cookie", cookieString);
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
        // 修改自https://www.cnblogs.com/hofmann/p/11347007.html
        static public string DictionaryToQueryString(IDictionary<string, string> parameters)
        {
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            StringBuilder query = new StringBuilder();
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append('=').Append(value).Append('&');
                }
            }
            string content = query.ToString().Substring(0, query.Length - 1);

            return content;
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                /*  Python Code:
                 *  def upload_answer(group_id, choose_id, answer, exam_id, homework_id, score_id):
                        data = {
                            'chooseAnswer': f'{group_id}@@@{choose_id}@@@{answer}@@@1',
                            'listenExamAnswer.quesType': 1,
                            'listenExamAnswer.orgId': 10206,
                            'listenExamAnswer.orgType': 1,
                            'listenExamAnswer.managerId': 255505,
                            'listenExamAnswer.examId': exam_id,
                            'listenExamAnswer.homeWorkId': homework_id,
                            'listenExamScore.id': score_id
                        }
                        return req.post('https://028.kouyu100.com/njjlzxhx/saveSingleExamAnswer.action',data=data,cookies=cookies)
                    response = req.get(f'http://028.kouyu100.com/njjlzxhx/getListenGroupsByExamId.action?examId={exam_id}',cookies=cookies)
                    group_list = response.json()['groupList']
                    answer_list = []
                    for i in group_list:
                        for j in i['chooseList']:
                            answer_list.append({
                                'group_id': i['id'],
                                'choose_id': j['id'],
                                'answer': j['answers']
                            })
                    for i in answer_list:
                        group_id, choose_id, answer = i['group_id'], i['choose_id'], i['answer']
                        print(upload_answer(group_id, choose_id, answer, exam_id, homework_id, score_id).text)
                        # print(answer, end='')

                    req.get(f'https://028.kouyu100.com/njjlzxhx/endExamAnswer.action?examId={exam_id}&homeWork.id={homework_id}&listenExamScore.id={score_id}')

                    if req.get(f'http://028.kouyu100.com/njjlzxhx/findStudentExamInfo.action?examId={exam_id}&hwId={homework_id}').text != '{"scoreList":[]}':
                        print('ok!')
                    else:
                        print('failed!')
                 */
                // 构造答案列表
                string groupListResultString = Kouyu100HttpGet($"http://028.kouyu100.com/njjlzxhx/getListenGroupsByExamId.action?examId={currentHomework.ExamId}");
                JArray groupList = (JArray)((JObject)JsonConvert.DeserializeObject(groupListResultString))["groupList"];
                List<Tuple<int, int, string>> answerList = new List<Tuple<int, int, string>>();
                foreach(JObject group in groupList)
                {
                    foreach (JObject choose in (JArray)group["chooseList"])
                    {
                        answerList.Add(new Tuple<int, int, string>((int)group["id"], (int)choose["id"], (string)choose["answers"]));
                    }
                }
                int count = 0;
                double total = answerList.ToArray().Length;
                // 遍历答案列表上传答案
                foreach (Tuple<int, int, string> tuple in answerList)
                {
                    int groupId = tuple.Item1;
                    int chooseId = tuple.Item2;
                    string answer = tuple.Item3;
                    Dictionary<string, string> data = new Dictionary<string, string>()
                    {
                        { "chooseAnswer", $"{groupId}@@@{chooseId}@@@{answer}@@@1" },
                        { "listenExamAnswer.quesType", "1" },
                        { "listenExamAnswer.orgId", "10206" },
                        { "listenExamAnswer.orgType", "1" },
                        { "listenExamAnswer.managerId", "255505" },
                        { "listenExamAnswer.examId", currentHomework.ExamId.ToString() },
                        { "listenExamAnswer.homeWorkId", currentHomework.HomeworkId.ToString() },
                        { "listenExamScore.id", ScoreId.ToString() }
                    };
                    // 上传单个答案并检查
                    if (Kouyu100HttpPost("https://028.kouyu100.com/njjlzxhx/saveSingleExamAnswer.action", DictionaryToQueryString(data)) != "1.0")
                    {
                        throw new WebException("上传单个答案API调用错误! ");
                    }
                    count++;
                    UploadProgressBar.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        UploadProgressBar.Value = count / total * 100;
                    }));
                }
                // 结束上传
                _ = Kouyu100HttpGet($"https://028.kouyu100.com/njjlzxhx/endExamAnswer.action?examId={currentHomework.ExamId}&homeWork.id={currentHomework.HomeworkId}&listenExamScore.id={ScoreId}");
                // 检查结果
                string result = Kouyu100HttpGet($"http://028.kouyu100.com/njjlzxhx/findStudentExamInfo.action?examId={currentHomework.ExamId}&hwId={currentHomework.HomeworkId}");
                if (result!= "{\"scoreList\":[]}")
                {
                    UploadProgressBar.Dispatcher.Invoke(delegate
                    {
                        UploadProgressBar.Value = 100;
                    });
                    MessageBox.Show("成功! ");
                }
                else
                {
                    MessageBox.Show("失败! ");
                    UploadProgressBar.Dispatcher.Invoke(delegate
                    {
                        UploadProgressBar.Value = 0;
                    });
                }
            });
        }
    }
}
