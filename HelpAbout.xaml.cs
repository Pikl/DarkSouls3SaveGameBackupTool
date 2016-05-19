using System.Windows;

namespace DarkSouls3SuperBackupTool {
    public partial class HelpAbout : Window {
        public HelpAbout() {
            InitializeComponent();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
