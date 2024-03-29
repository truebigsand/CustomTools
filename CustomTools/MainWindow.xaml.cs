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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void SchoolDataButtonClick(object sender, RoutedEventArgs e)
        {
            SchoolDataWindow schoolDataWindow = new SchoolDataWindow();
            schoolDataWindow.Owner = this;
            schoolDataWindow.ShowDialog();
        }

        private void Kouyu100Button_Click(object sender, RoutedEventArgs e)
        {
            Kouyu100AutoFinishWindow kouyu100AutoFinishWindow = new Kouyu100AutoFinishWindow();
            kouyu100AutoFinishWindow.Owner = this;
            kouyu100AutoFinishWindow.ShowDialog();
        }
    }
}
