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

namespace DarkSouls3SuperBackupTool {
    public partial class Settings : Window {
        public Settings() {
            InitializeComponent();
        }

        #region Button Events
        void btn_cancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        void btn_save_Click(object sender, RoutedEventArgs e) {
            Close();
        }
        #endregion
    }
}
