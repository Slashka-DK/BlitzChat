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

namespace BlitzChat
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

        }

        private void sliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(lblOpacity != null)
            lblOpacity.Content = Convert.ToInt32(sliderOpacity.Value) + "%";
        }

        public void setSlider(double d) {
            sliderOpacity.Value = d;
            lblOpacity.Content = Convert.ToInt32(d * 100) + "%";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Xceed.Wpf.Toolkit.ColorPicker picker = new Xceed.Wpf.Toolkit.ColorPicker();
        }

        private void txtTwitch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void frmSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void bttnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
