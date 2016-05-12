using System.Windows;

namespace DarkSouls3BackupTool {
    public partial class HelpAbout : Window {
        public HelpAbout() {
            InitializeComponent();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
