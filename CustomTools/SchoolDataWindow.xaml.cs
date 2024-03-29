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
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using Microsoft.Win32;

namespace CustomTools
{
    class Student
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public string StudentId { get; set; }
        public string EnrollmentYear { get; set; }
        public string Grade { get; set; }
        public string Class { get; set; }
        public string Sex { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    }
    /// <summary>
    /// SchoolDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SchoolDataWindow : Window
    {
        IDbConnection? connection = null;
        DatabaseType databaseType = DatabaseType.Sqlite;
        enum DatabaseType
        {
            Sqlite, SQL_Server
        }
        IDbConnection GetConnection()
        {
            if (databaseType == DatabaseType.Sqlite)
            {
                SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder();
                connectionStringBuilder.DataSource = ChooseFileTextBox.Text;
                return new SQLiteConnection(connectionStringBuilder.ToString());
            }
            if (databaseType == DatabaseType.SQL_Server)
            {
                return new SqlConnection(DatabaseConnectionStringTextBox.Text);
            }
            throw new ArgumentException("Unknown DatabaseType!");
        }
        public SchoolDataWindow()
        {
            InitializeComponent();
        }

        private void ExecuteSQLButtonClick(object sender, RoutedEventArgs e)
        {
            if (connection == null)
            {
                connection = GetConnection();
                connection.Open();
            }
            try
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = SQLCommandTextBox.Text;
                IDataReader dataReader = command.ExecuteReader();
                StudentsListBox.Items.Clear();
                while (dataReader.Read())
                {
                    StudentsListBox.Items.Add(new Student()
                    {
#pragma warning disable CS8601 // 引用类型赋值可能为 null。
                        StudentId = dataReader["Snum"].ToString(),
                        EnrollmentYear = dataReader["Syear"].ToString(),
                        Grade = dataReader["Sgrade"].ToString(),
                        Class = dataReader["Sclass"].ToString(),
                        Sex = dataReader["Sex"].ToString(),
                        Name = dataReader["Sname"].ToString(),
                        Password = dataReader["Spwd"].ToString()
#pragma warning restore CS8601 // 引用类型赋值可能为 null。
                    });
                }
                dataReader.Dispose();
                command.Dispose();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private void DatabaseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DatabaseComboBox.SelectedIndex == 0)
            {
                databaseType = DatabaseType.Sqlite;
                ChooseFileTextBox.Visibility = Visibility.Visible;
                DatabaseConnectionStringTextBox.Visibility = Visibility.Hidden;
                return;
            }
            if (DatabaseComboBox.SelectedIndex == 1)
            {
                databaseType = DatabaseType.SQL_Server;
                ChooseFileTextBox.Visibility = Visibility.Hidden;
                DatabaseConnectionStringTextBox.Visibility = Visibility.Visible;
                return;
            }
        }

        private void ChooseFileTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Please select your sqlite database file.";
            dialog.Filter = "Sqlite Database File(*.db,*.db3)|*.db;*.db3";
            dialog.InitialDirectory = @"C:\Users\唐知非\桌面\out";
            if (dialog.ShowDialog() == true)
            {
                ChooseFileTextBox.Text = dialog.FileName;
            }
            else
            {
                MessageBox.Show("未选择任何文件! ");
            }
            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }
    }
}
