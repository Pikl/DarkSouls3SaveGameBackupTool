using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DarkSouls3SuperBackupTool {
    public partial class Restore : Window {


        public Restore() {
            InitializeComponent();

            LoadBackups();
        }

        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            LoadThumbnail();
        }

        void btn_restore_Click(object sender, RoutedEventArgs e) {
            if (ConfirmationBox() == MessageBoxResult.Yes)
                RestoreSelectedBackup();
        }

        void btn_close_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        MessageBoxResult ConfirmationBox() {
            string message = "Clicking yes will delete the current save file and replace" + Environment.NewLine + 
                             "it with the selected backup : " + listBox.SelectedItem.ToString();

            return MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo);
        }

        void LoadBackups() {
            listBox.Items.Clear();

            foreach (string profile in Directory.GetDirectories(MainWindow.appDataFolder)) {
                if (listBox.Items.Contains(profile))
                    continue;

                foreach(string backup in Directory.GetFiles(profile)) {
                    string name = Path.GetFileNameWithoutExtension(backup);
                    
                    if (listBox.Items.Contains(name) ||
                        Path.GetExtension(backup) != ".sl2" ||
                        name == "DS30000")
                        continue;
                    
                    listBox.Items.Add(Path.GetFileNameWithoutExtension(backup));
                }
            }
        }

        void LoadThumbnail() {
            string path = Path.Combine(MainWindow.ScreenshotFolder, listBox.SelectedItem.ToString() + ".png");

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path);
            bmp.EndInit();

            img_thumbnail.Source = bmp;
        }

        void RestoreSelectedBackup() {

        }

        //Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

        //fileDialog.Filter = "Dark Souls 3 Save Game Backups (.sl2)|*.sl2";
        //fileDialog.InitialDirectory = appDataFolder;

        //bool? result = fileDialog.ShowDialog();

        ////user selected a file to use as the backup
        //if (result == true) {
        //    string backupChoiceFileName = fileDialog.FileName;

        //    string deleteConfirmationMessage = "Are you sure? This will DELETE DS30000.sl2 (your current save file) and replace it with: "
        //                                     + Environment.NewLine + Path.GetFileName(backupChoiceFileName);

        //    string profile = Path.GetDirectoryName(backupChoiceFileName);

        //    MessageBoxResult messageBoxResult = MessageBox.Show(deleteConfirmationMessage, "Delete DS30000.sl2 Confirmation", MessageBoxButton.YesNo);

        //    if (messageBoxResult == MessageBoxResult.Yes) {
        //        try {
        //            //back up the original incase user error idea...
        //            //Directory.CreateDirectory(saveGameLocation + "RestoredSaveBackups
        //            //File.Copy(saveGameLocation + "DS30000.sl2", saveGameLocation + "\\RestoredSaveBackups\\" + "DeletedSave__DS30000.sl2.bak");

        //            File.Delete(Path.Combine(profile, "DS30000.sl2"));
        //            Log("Deleted DS30000.sl2...");

        //            File.Copy(backupChoiceFileName, Path.Combine(profile, "DS30000.sl2"));
        //            Log("Created DS30000.sl2 from backup...");

        //            CustomNotificationMessageBox("Back up has been successfully restored.");
        //        } catch (Exception ex) {
        //            ErrorBox(ex.ToString());
        //        }

        //    }

        //}
    }
}
