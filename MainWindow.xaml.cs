using System;
using System.Windows;
using System.IO;
using System.Xml;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Media;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace DarkSouls3BackupTool {
    public partial class MainWindow : Window {

        #region Fields
        internal readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSoulsIII");
        
        internal string ConfigFilename {
            get {
                return Path.Combine(appDataFolder, "BackupConfig.xml");
            }
        }

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        System.Windows.Media.Color paleYellow, deepGrey;
        Dictionary<string, DateTime?> lastWriteTime = new Dictionary<string, DateTime?>();
        Config config;
        #endregion

        #region Init
        public MainWindow() {
            InitializeComponent();

            if (!File.Exists(ConfigFilename))
                CreateAppConfigFile();

            config = new Config(this);

            foreach (string profile in Directory.GetDirectories(appDataFolder))
                lastWriteTime.Add(profile.Substring(profile.LastIndexOf('\\') + 1), null);

            //0xFFF5EECF
            paleYellow = new System.Windows.Media.Color();
            paleYellow.A = 0xFF;
            paleYellow.R = 0xF5;
            paleYellow.G = 0xEE;
            paleYellow.B = 0xCF;

            //0xFF515151
            deepGrey = new System.Windows.Media.Color();
            deepGrey.A = 0xFF;
            deepGrey.R = 0x51;
            deepGrey.G = 0x51;
            deepGrey.B = 0x51;
            
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            if (config.AutoStart.Value) {
                btn_Start_Toggle(false);
                btn_start_Click(null, null);
            } else
                btn_Stop_Toggle(false);
        }
        #endregion

        #region Button Events
        void btn_start_Click(object sender, RoutedEventArgs e) {
            Log("Starting backup process...");
            Log("Creating a backup every " + config.Interval.Value + (config.Minutes.Value ? " minute" : " second") + (config.Interval.Value == 1 ? "." : "s."), false, true);
            
            if (config.Minutes.Value)
                dispatcherTimer.Interval = new TimeSpan(0, config.Interval.Value, 0);
            else
                dispatcherTimer.Interval = new TimeSpan(0, 0, config.Interval.Value);

            dispatcherTimer.Start();

            btn_Start_Toggle(false);
            btn_Stop_Toggle(true);
        }
        
        void btn_stop_Click(object sender, RoutedEventArgs e) {
            Log("Stopped backup process...");
            
            dispatcherTimer.Stop();

            btn_Stop_Toggle(false);
            btn_Start_Toggle(true);
        }

        void btn_restore_Click(object sender, RoutedEventArgs e) {
            System.Windows.MessageBox.Show("Restore UI still TODO!");
            //Restore s = new Restore();
            //s.Owner = this;
            //s.ShowDialog();

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

        void btn_help_Click(object sender, RoutedEventArgs e) {
            HelpAbout ha = new HelpAbout();
            ha.Owner = this;
            ha.ShowDialog();
        }
        
        void btn_saveFolder_Click(object sender, RoutedEventArgs e) {
            Process.Start(appDataFolder);
        }
        
        void btn_settings_Click(object sender, RoutedEventArgs e) {
            //Settings s = new Settings();
            //s.Owner = this;
            //s.ShowDialog();
        }

        void btn_clear_Click(object sender, RoutedEventArgs e) {
            txtBox_log.Text = "";
        }
        #endregion

        #region Start and Stop Button Toggles
        void btn_Start_Toggle(bool enable) {
            if (enable) {
                btn_start.FontWeight = FontWeights.Bold;
                btn_start.IsEnabled = true;
                btn_start.Background = new SolidColorBrush(deepGrey);
                btn_start.Foreground = new SolidColorBrush(paleYellow);
            } else {
                btn_start.FontWeight = FontWeights.Normal;
                btn_start.IsEnabled = false;
                btn_start.Background = new SolidColorBrush(Colors.Gray);
                btn_start.Foreground = new SolidColorBrush(Colors.LightGray);
            }
        }

        void btn_Stop_Toggle(bool enable) {
            if (enable) {
                btn_stop.FontWeight = FontWeights.Bold;
                btn_stop.IsEnabled = true;
                btn_stop.Background = new SolidColorBrush(deepGrey);
                btn_stop.Foreground = new SolidColorBrush(paleYellow);
            } else {
                btn_stop.FontWeight = FontWeights.Normal;
                btn_stop.IsEnabled = false;
                btn_stop.Background = new SolidColorBrush(Colors.Gray);
                btn_stop.Foreground = new SolidColorBrush(Colors.LightGray);
            }
        }
        #endregion

        #region Backup Timer
        void dispatcherTimer_Tick(object sender, EventArgs e) {
            if (!config.IfNotRunning.Value && Process.GetProcessesByName("DarkSoulsIII").Length == 0) {
                Log("Dark Souls 3 is not running, skipping backup creation...");
                return;
            }
                
            string dateOfBackupForFileName = DateTime.Now.ToString("MM/d/yy HH:mm:ss");

            foreach (string profile in Directory.GetDirectories(appDataFolder)) {
                try {
                    if (profile == "Backups")
                        continue;

                    string num = profile.Substring(profile.LastIndexOf('\\') + 1);

                    DateTime curWriteTime = File.GetLastWriteTime(Path.Combine(profile, "DS30000.sl2"));

                    if (lastWriteTime[num] != null && lastWriteTime[num] == curWriteTime && !config.IfNotChanged.Value) {
                        Log("Save file for " + num + " unchanged! Skipping backup");
                        continue;
                    }
                    
                    //Remove spaces, : and / from dateOfBackupForFileName and replace with dash
                    dateOfBackupForFileName = System.Text.RegularExpressions.Regex.Replace(dateOfBackupForFileName, @"[:|/|\s]", "-");

                    File.Copy(Path.Combine(profile, "DS30000.sl2"),
                                Path.Combine(profile, dateOfBackupForFileName + ".sl2"));

                    Log("Created a new backup for profile " + num);
                    lastWriteTime[num] = curWriteTime;
                    
                } catch (Exception ex) {
                    ErrorBox(ex.Message);

                    if (dispatcherTimer.IsEnabled) {
                        dispatcherTimer.Stop();
                    }
                }

                //Screenshot(Path.Combine(profile, dateOfBackupForFileName + ".png"));
            }
        }

        void Screenshot(string path) {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format64bppPArgb);
            var gfx = Graphics.FromImage(bmp);

            gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                
            bmp = ResizeImage(bmp, 512, 512);
            bmp.Save(path, ImageFormat.Png);
        }
        
        Bitmap ResizeImage(Image image, int width, int height) {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        #endregion

        #region Config
        void CreateAppConfigFile() {
            Log("BackupConfig.xml not found, Creating a new one...");

            if (!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            using (FileStream fs = new FileStream(ConfigFilename, FileMode.Create)) {
                using (StreamWriter sw = new StreamWriter(fs)) {
                    using (XmlTextWriter xw = new XmlTextWriter(sw)) {
                        //xw.Settings.Indent = true;
                        //xw.Formatting = Formatting.Indented;
                        //xw.Indentation = 4;
                        xw.WriteStartDocument();

                        xw.WriteStartElement("Config");

                        xw.WriteStartElement("TimeInterval");
                        xw.WriteAttributeString("immediate", "true");
                        xw.WriteAttributeString("minutes", "true");
                        xw.WriteAttributeString("value", "15");
                        xw.WriteEndElement();

                        xw.WriteStartElement("Max");
                        xw.WriteAttributeString("quantity", "100");
                        xw.WriteAttributeString("size", "1000");
                        xw.WriteEndElement();

                        xw.WriteStartElement("Misc");
                        xw.WriteAttributeString("backup-if-not-running", "false");
                        xw.WriteAttributeString("backup-if-not-changed", "false");
                        xw.WriteAttributeString("auto-start", "false");
                        xw.WriteAttributeString("log-to-file", "false");
                        xw.WriteEndElement();

                        xw.WriteEndElement();

                        xw.WriteEndDocument();
                        xw.Close();
                    }
                }
            }
        }

        internal class Config {
            XmlDocument doc;
            XmlNode timeNode, miscNode;
            MainWindow main;

            internal Attribute<int> Interval, MaxQuantity, MaxSize;
            internal Attribute<bool> Minutes, AutoStart, LogToFile,
                                     IfNotRunning, IfNotChanged;
            
            internal Config(MainWindow main) {
                this.main = main;

                doc = new XmlDocument();
                doc.Load(main.ConfigFilename);

                timeNode = doc.SelectSingleNode("Config/TimeInterval");
                miscNode = doc.SelectSingleNode("Config/Misc");

                Interval = new Attribute<int>("value", 15, this, timeNode);
                MaxQuantity = new Attribute<int>("quantity", 100, this, miscNode);
                MaxSize = new Attribute<int>("size", 1000, this, miscNode);
                Minutes = new Attribute<bool>("minutes", true, this, timeNode);
                AutoStart = new Attribute<bool>("auto-start", false, this, miscNode);
                IfNotRunning = new Attribute<bool>("backup-if-not-running", false, this, miscNode);
                IfNotChanged = new Attribute<bool>("backup-if-not-changed", false, this, miscNode);
            }

            public class Attribute<T> {
                public readonly string name;
                T value, defaultValue;
                Config config;
                XmlNode node;
                
                public T Value {
                    get {
                        try {
                            value = (T)Convert.ChangeType(node.Attributes[name].InnerText, typeof(T));
                        } catch {
                            config.main.Log(string.Format("'{0}' setting was corrupt, using default value '{1}'...", name, defaultValue));
                            Value = (T)Convert.ChangeType(defaultValue, typeof(T));
                        }

                        return value;
                    }
                    set {
                        this.value = value;
                        node.Attributes[name].InnerText = value.ToString();
                        config.doc.Save(config.main.ConfigFilename);
                    }
                }

                public Attribute(string name, T defaultValue, Config config, XmlNode node) {
                    this.name = name;
                    this.defaultValue = defaultValue;
                    this.config = config;
                    this.node = node;
                }
            }
        }
        #endregion

        #region Utils
        internal void Log(string message, bool timestamp = true, bool newLine = true) {
            if (timestamp)
                message = DateTime.Now.ToString("HH:mm:ss") + "\t" + message;
            else
                message = "\t" + message;

            if (newLine)
                message += Environment.NewLine;

            txtBox_log.AppendText(message);
            txtBox_log.ScrollToEnd();
        }
        
        void ErrorBox(string errorMessage) {
            System.Windows.MessageBox.Show(errorMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        #endregion
    }
}
