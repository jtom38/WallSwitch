using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework.Components;
using MetroFramework;
using WallSwitch.src.sql;
using WallSwitch.src.web;
using WallSwitch.src.wallpaper;
using System.Diagnostics;

namespace WallSwitch.src.ui
{
    public partial class metroForm : MetroForm
    {

        //Wallpaper _wp = new Wallpaper();
        wallpaperSingle _wpSingle = new wallpaperSingle();
        wallpaperLib _wpLib = new wallpaperLib();
        sqlLib _sqlLib = new sqlLib();
        sqlKonachan _sqlKona = new sqlKonachan();
        sqlWallhaven _sqlWH = new sqlWallhaven();
        sqlYandere _sqlYan = new sqlYandere();
        sqlTags _sqlTags = new sqlTags(); 
        webStream _webStream = new webStream();
        wsLib _wsLib = new wsLib();

        MetroToolTip toolTip = new MetroToolTip();
        NotifyIcon notifyIcon = new NotifyIcon();

        BackgroundWorker ThreadWallpaper = new BackgroundWorker();
        int threadWallpaperCounter = 0;

        BackgroundWorker ThreadSQL = new BackgroundWorker();
        int threadSQLCounter = 0;

        bool ThreadWallpaperActive = true;

        bool ThreadSQLKonachan = false;
        bool ThreadSQLYandere = false;
        bool ThreadSQLWallhaven = false;
        bool ThreadSQLTags = false;

        public metroForm()
        {
            InitializeComponent();

            //enable metroStyleManager
            this.StyleManager = metroStyleManager1;

            //enable wallapperThread
            ThreadWallpaper.WorkerReportsProgress = true;
            ThreadWallpaper.DoWork += new DoWorkEventHandler(ThreadWallpaper_DoWork);
            ThreadWallpaper.ProgressChanged += new ProgressChangedEventHandler(ThreadWallpaper_ProgressChanged);

            //enable sqlThread
            ThreadSQL.WorkerReportsProgress = true;
            ThreadSQL.DoWork += new DoWorkEventHandler(ThreadSQL_DoWork);
            ThreadSQL.ProgressChanged += new ProgressChangedEventHandler(ThreadSQL_ProgressChanged);

            //set the programs icon
            this.Icon = WallSwitch.Properties.Resources.favicon_notification_v3;

            CoreNotifyIcon();

        }

        private void ThreadWallpaper_DoWork(object sender, DoWorkEventArgs e)
        {
            //throw new NotImplementedException();
            while (ThreadWallpaperActive == true)
            {
                try
                {
                    _wpSingle.GetFileNames();

                    int t = _wsLib.WallpaperSleepTimer() / 1000;

                    threadWallpaperCounter = _wsLib.WallpaperSleepTimer();

                    for (int i = 0; i < t; i++)
                    {
                        //sending 1 as the progress percent given we are not tracking this based off a percent but it will flag the next code

                        ThreadWallpaper.ReportProgress(1);

                        System.Threading.Thread.Sleep(1000);
                        threadWallpaperCounter = threadWallpaperCounter - 1000;
                    }
                }
                catch(Exception ex)
                {
                    _wsLib.LogWrite("Error", "bwWallpaperChanger", ex.ToString());
                }

            }
        }

        private void ThreadWallpaper_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            toolTip.SetToolTip(ThreadWallpaperChangePictureBox, "Wallpaper Change: " + _wsLib.ReportTimeLeft(threadWallpaperCounter));
        }

        private void ThreadSQL_DoWork(object sender, DoWorkEventArgs e)
        {
            //throw new NotImplementedException();
            bool ThreadSQLActive = true;
            while (ThreadSQLActive == true)
            {

                _sqlLib.CheckForDB();
                _sqlLib.OpenConnection();

                _webStream.ParseKonachan();
                ThreadSQLKonachan = true;
                ThreadSQL.ReportProgress(1);

                _webStream.ParseYandere();
                ThreadSQLYandere = true;
                ThreadSQL.ReportProgress(1);

                _webStream.ParseWallhaven();
                ThreadSQLWallhaven = true;
                ThreadSQL.ReportProgress(1);

                _wsLib.ParseTags();
                ThreadSQLTags = true;
                ThreadSQL.ReportProgress(1);

                //30m * 60s = 1800s
                threadSQLCounter = 1800000;
                for (int i = 0; i < 1800; i++)
                {
                    ThreadSQL.ReportProgress(1);
                    System.Threading.Thread.Sleep(1000);
                    threadSQLCounter = threadSQLCounter - 1000;
                }

                ThreadSQLKonachan = false;
                ThreadSQLYandere = false;
                ThreadSQLWallhaven = false;

            }

            //ThreadSQL.CancelAsync();

        }

        private void ThreadSQL_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            if (ThreadSQLKonachan == true && ThreadSQLYandere == true && ThreadSQLWallhaven == true && ThreadSQLTags == true)
            {
                toolTip.SetToolTip(ThreadSQLStatusPictureBox, "Database Update: " + _wsLib.ReportTimeLeft(threadSQLCounter));
            }
            else
            {
                if (ThreadSQLTags == true)
                {
                    toolTip.SetToolTip(ThreadSQLStatusPictureBox, "Parse Konachan.com - Done\rParse Yande.re - Done\rParse Wallhaven.cc - Done");
                }
                else
                {
                    if (ThreadSQLWallhaven == true)
                    {
                        toolTip.SetToolTip(ThreadSQLStatusPictureBox, "Parse Konachan.com - Done\rParse Yande.re - Done\rParse Wallhaven.cc - Done");
                    }
                    else
                    {
                        if (ThreadSQLYandere == true)
                        {
                            toolTip.SetToolTip(ThreadSQLStatusPictureBox, "Parse Konachan.com - Done\rParse Yande.re - Done\rParse Wallhaven.cc - Running");
                        }
                        else
                        {
                            if (ThreadSQLKonachan == true)
                            {
                                toolTip.SetToolTip(ThreadSQLStatusPictureBox, "Parse Konachan.com - Done\rParse Yande.re - Running\rParse Wallhaven.cc - Pending");
                            }
                            else
                            {
                                toolTip.SetToolTip(ThreadSQLStatusPictureBox, "Parse Konachan.com - Pending\rParse Yande.re - Pending\rParse Wallhaven.cc - Pending");
                            }
                        }
                    }
                }
            }
        }

        private void formTest_Load(object sender, EventArgs e)
        {
            OnLoadTest();

            OnLoadDBConnection();
            OnLoadStartWPRotation();

            OnLoadCoreMinamized();
            OnLoadCoreNotifications();
            OnLoadCoreDarkMode();
            OnLoadCoreUseLocalFiles();
            OnLoadCoreUseWebFiles();

            OnLoadFolderPath();
            OnLoadWPShuffle();
            OnLoadWPFit();
            OnLoadWPChangeInterval();
            OnLoadWPRecursiveSearch();
            OnLoadMultiMonitor();

            OnLoadWebRating();
            OnLoadWebKonachan();
            OnLoadWebWallhaven();
            OnLoadWebYandere();

            OnLoadActiveTags();
            OnLoadBlockedTags();
        }

        private void OnLoadDBConnection()
        {
            if (MySettings.Default.CoreUseWebFiles == true)
            {
                toolTip.SetToolTip(ThreadSQLStatusPictureBox, "Parse Konachan.com - Running\rParse Yande.re - Pending\rParse Wallhaven.cc - Pending\rTag Refresh - Pending");

                ThreadSQL.RunWorkerAsync();
            }
        }

        private void OnLoadStartWPRotation()
        {
            //start auto cycle of pictures if folders are available
            //bool CancleThread = false;

            if(MySettings.Default.CoreUseLocalFiles == true || MySettings.Default.CoreUseWebFiles == true)
            {
                //_wpSingle.ThreadGetTimer(CancleThread);
                ThreadWallpaper.RunWorkerAsync();
            }

            if (MySettings.Default.CoreUseWebFiles == false)
            {
                //ask user if they want to enable web streaming
                DialogResult t = MetroMessageBox.Show(this, "I see you dont have any folders added.  We can stream pictures from the internet if you like?", "WallSwitch", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                //DialogResult streamResult = MessageBox.Show("I see you dont have any folders added.  We can stream pictures from Konachan.com if you like?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (t == DialogResult.Yes)
                {
                    MySettings.Default.CoreUseWebFiles = true;
                    MySettings.Default.Save();
                    OnLoadDBConnection();
                    //_wpSingle.ThreadGetTimer(CancleThread);
                    ThreadWallpaper.RunWorkerAsync();
                }
            }     
        }

        private void OnLoadCoreMinamized()
        {
            try
            {
                if(MySettings.Default.CoreStartMinamized == true)
                {
                    CoreMinamizedToggle.Checked = true;
                }
                else
                {
                    CoreMinamizedToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadCoreNotifications()
        {
            try
            {
                if(MySettings.Default.CoreNotifications == true)
                {
                    CoreNotificationsToggle.Checked = true;
                }
                else
                {
                    CoreNotificationsToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadCoreDarkMode()
        {
            try
            {
                if (MySettings.Default.CoreDarkMode == true)
                {
                    metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
                    CoreDarkModeToggle.Checked = true;
                }
                else
                {
                    metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
                    CoreDarkModeToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadCoreUseLocalFiles()
        {
            try
            {
                if(MySettings.Default.CoreUseLocalFiles == true)
                {
                    CoreUseLocalFilesToggle.Checked = true;
                }
                else
                {
                    CoreUseLocalFilesToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadCoreUseWebFiles()
        {
            try
            {
                if(MySettings.Default.CoreUseWebFiles == true)
                {
                    CoreUseWebFilesToggle.Checked = true;
                }
                else
                {
                    CoreUseWebFilesToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadFolderPath()
        {
            try
            {
                for (int i = 0; i < MySettings.Default.WPDirectories.Count; i++)
                {
                    WPLocalFoldersComboBox.Items.Add(MySettings.Default.WPDirectories[i]);
                }
                WPLocalFoldersComboBox.SelectedIndex = 0;

                WPLocalFoldersComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch
            {
                //log
            }

        }

        private void OnLoadWPShuffle()
        {
            try
            {
                if(MySettings.Default.WPShuffle == true)
                {
                    WPShuffleToggle.Checked = true;
                }
                else
                {
                    WPShuffleToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadWPFit()
        {
            try
            {
                WPFitComboBox.Text = MySettings.Default.WPStyle;
            }
            catch
            {
                //log
            }

            WPFitComboBox.SelectedText = MySettings.Default.WPStyle;
            WPFitComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void OnLoadWPChangeInterval()
        {
            try
            {
                WPChangeIntervalComboBox.Text = MySettings.Default.WPChangeInterval;
            }
            catch
            {
                //log
            }

            WPChangeIntervalComboBox.SelectedText = MySettings.Default.WPChangeInterval;
            WPChangeIntervalComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void OnLoadWPRecursiveSearch()
        {
            try
            {
                if (MySettings.Default.WPSearchThoughFolders == true)
                {
                    WPRecursiveSearchToggle.Checked = true;
                }
                if (MySettings.Default.WPSearchThoughFolders == false)
                {
                    WPRecursiveSearchToggle.Checked = false;
                }

                toolTip.SetToolTip(WPRecursiveSearchToggle, "On = Will search all folders under the root given.\r Off = Only search the root folder given.");
            }
            catch
            {
                //log
            }
        }

        private void OnLoadMultiMonitor()
        {
            try
            {
                List<Monitors> m = _wpLib.GetNumberOfMonitors();

                if(m.Count >= 2)
                {
                    WPMultiMonitorLabel.Visible = true;
                    WPMultiMonitorToggle.Visible = true;

                    if (MySettings.Default.WPMultiMonitor == true)
                    {
                        WPMultiMonitorToggle.Checked = true;
                    }
                    if (MySettings.Default.WPMultiMonitor == false)
                    {
                        WPMultiMonitorToggle.Checked = false;
                    }

                }
                else
                {
                    WPMultiMonitorLabel.Visible = true;
                    WPMultiMonitorToggle.Visible = true;
                }

                toolTip.SetToolTip(WPMultiMonitorToggle, "On = Different wallpaper per monitor.\rOff = Same wallpaper per monitor.");
            }
            catch
            {
                //log
            }
        }

        private void OnLoadWebRating()
        {
            switch (MySettings.Default.RSSRatings)
            {
                case "s":
                    WebRatingComboBox.Text = "Safe";
                    break;
                case "q":
                    WebRatingComboBox.Text = "Questionable";
                    break;
                case "e":
                    WebRatingComboBox.Text = "Explicit";
                    break;
            }

            WebRatingComboBox.SelectedText = MySettings.Default.RSSRatings;
            WebRatingComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void OnLoadWebKonachan()
        {
            try
            {
                if(MySettings.Default.rssSiteKonachan == true)
                {
                    WebKonachanToggle.Checked = true;
                }
                else
                {
                    WebKonachanToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadWebWallhaven()
        {
            try
            {
                if (MySettings.Default.rssSiteWallhaven == true)
                {
                    WebWallhavenToggle.Checked = true;
                }
                else
                {
                    WebWallhavenToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadWebYandere()
        {
            try
            {
                if (MySettings.Default.rssSiteYandre == true)
                {
                    WebYandereToggle.Checked = true;
                }
                else
                {
                    WebYandereToggle.Checked = false;
                }
            }
            catch
            {
                //log
            }
        }

        private void OnLoadActiveTags()
        {
            try
            {
                ActiveTagsComboBox.Items.AddRange(MySettings.Default.CoreActiveTags.Cast<string>().ToArray<string>());
                ActiveTagsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch
            {
                //log
            }
        }

        private void OnLoadBlockedTags()
        {
            try
            {
                BlockedTagsComboBox.Items.AddRange(MySettings.Default.CoreBlockedTags.Cast<string>().ToArray<string>());
                BlockedTagsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch
            {
                //log
            }
        }

        private void OnLoadTest()
        {
            //_wsLib.ParseTags();
        }

        private void CoreNotifyIcon()
        {
            //set trayIcon
            notifyIcon.Icon = WallSwitch.Properties.Resources.favicon_notification_v3_x32;
            notifyIcon.Text = "WallSwitch";
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += new System.EventHandler(trayIcon_DoubleClick);

            Container components = new Container();
            ContextMenu contextMenu1 = new ContextMenu();
            MenuItem menuItem1 = new MenuItem();
            MenuItem menuItem2 = new MenuItem();

            // Initialize contextMenu1
            contextMenu1.MenuItems.AddRange(new MenuItem[] { menuItem1, menuItem2 });

            // Initialize menuItem1
            menuItem1.Index = 0;
            menuItem1.Text = "E&xit";
            menuItem1.Click += new System.EventHandler(trayIcon_Exit);

            
            menuItem2.Index = 1;
            menuItem2.Text = "Next Wallpaper";
            menuItem2.Click += trayIcon_NextWallpaper;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon.ContextMenu = contextMenu1;

        }

        private void trayIcon_NextWallpaper(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            _wpSingle.GetFileNames();

        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //Show();
            WindowState = FormWindowState.Normal;
        }

        private void trayIcon_Exit(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Application.Exit();
        }

        private void CoreDBView(string function, string site)
        {
            dbGrid.Visible = true;

            string[] siteColumns = { "P_ID", "SiteID", "Tags", "rating", "preview_url", "jpeg_url", "jpeg_width", "jpeg_height", "date_added", "favorite" };
            string[] localColumns = { "" };
            string[] tagColumns = { "P_ID", "Tags", "Site" };
          
            ///loop though and remove all the data before we add new data
            try
            {
                //remove
                dbGrid.Rows.Clear();

                for (int i = 0; i < siteColumns.Length; i++)
                {
                    dbGrid.Columns.Remove(siteColumns[i]);
                }

                for(int i = 0; i < tagColumns.Length; i++)
                {
                    dbGrid.Columns.Remove(tagColumns[i]);
                }

            }
            catch
            {
                //log
            }

            switch (function)
            {
                case "site":
                    ///based on what value that was sent we will place the correct columns in place
                    try
                    {
                        dbGrid.Visible = false;
                        for (int i = 0; i < siteColumns.Length; i++)
                        {
                            dbGrid.Columns.Add(siteColumns[i], siteColumns[i]);
                        }

                        List<SQLData> listData = _wsLib.CoreDBViewSites(site);

                        for (int i = 0; i < listData.Count; i++)
                        {
                            dbGrid.Rows.Add(listData[i].id, listData[i].siteID, listData[i].tags, listData[i].rating, listData[i].preview_url, listData[i].jpeg_url, listData[i].jpeg_width, listData[i].jpeg_height, listData[i].date_added, listData[i].favorite);
                        }
                        dbGrid.Visible = true;
                    }
                    catch
                    {
                        //log
                    }
                    break;

                case "local":
                    dbGrid.Visible = false;
                    MetroMessageBox.Show(this, "Oh no\rCurrently not working\rSorry about that :(", "WallSwitch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show("This is currently not working.");
                    break;
                case "tags":                   
                    try
                    {
                        dbGrid.Visible = false;
                        for (int i = 0; i < tagColumns.Length; i++)
                        {
                            dbGrid.Columns.Add(tagColumns[i], tagColumns[i]);
                        }

                        List<tagsData> tagList = _sqlTags.db_GetTagsData();

                        for(int i = 0; i < tagList.Count; i++)
                        {
                            dbGrid.Rows.Add(tagList[i].id, tagList[i].tag, tagList[i].site);
                        }
                        dbGrid.Visible = true;
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    break;
            }

        }

        private void CoreDarkModeToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CoreDarkModeToggle.Text == "On")
                {
                    MySettings.Default.CoreDarkMode = true;
                    metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
                    ThreadWallpaperChangePictureBox.Image = WallSwitch.Properties.Resources.Image_26x26_gray;
                    ThreadSQLStatusPictureBox.Image = WallSwitch.Properties.Resources.Database_32x_gray;
                }
                else
                {
                    MySettings.Default.CoreDarkMode = false;
                    metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
                    ThreadWallpaperChangePictureBox.Image = WallSwitch.Properties.Resources.Image_26x26_transparent;
                    ThreadSQLStatusPictureBox.Image = WallSwitch.Properties.Resources.Database_32x_black;
                }

                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void CoreMinamizedToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CoreMinamizedToggle.Text == "On")
                {
                    MySettings.Default.CoreStartMinamized = true;
                }
                else
                {
                    MySettings.Default.CoreStartMinamized = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void CoreNotificationsToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CoreNotificationsToggle.Text == "On")
                {
                    MySettings.Default.CoreNotifications = true;
                }
                else
                {
                    MySettings.Default.CoreNotifications = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void CoreUseLocalFilesToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CoreUseLocalFilesToggle.Text == "On")
                {
                    MySettings.Default.CoreUseLocalFiles = true;
                }
                else
                {
                    MySettings.Default.CoreUseLocalFiles = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void CoreUseWebFilesToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CoreUseWebFilesToggle.Text == "On")
                {
                    MySettings.Default.CoreUseWebFiles = true;
                }
                else
                {
                    MySettings.Default.CoreUseWebFiles = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WebYandereToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (WebYandereToggle.Text == "On")
                {
                    MySettings.Default.rssSiteYandre = true;
                }
                else
                {
                    MySettings.Default.rssSiteYandre = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WebWallhavenToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (WebWallhavenToggle.Text == "On")
                {
                    MySettings.Default.rssSiteWallhaven = true;
                }
                else
                {
                    MySettings.Default.rssSiteWallhaven = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WebKonachanToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (WebKonachanToggle.Text == "On")
                {
                    MySettings.Default.rssSiteKonachan = true;
                }
                else
                {
                    MySettings.Default.rssSiteKonachan = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WebRatingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (WebRatingComboBox.Text)
            {
                case "Safe":
                    MySettings.Default.RSSRatings = "s";
                    break;
                case "Questionable":
                    MySettings.Default.RSSRatings = "q";
                    break;
                case "Explicit":
                    MySettings.Default.RSSRatings = "e";
                    break;
            }
            MySettings.Default.Save();
        }

        private void WPMultiMonitorToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (WPMultiMonitorToggle.Text == "On")
                {
                    MySettings.Default.WPMultiMonitor = true;
                }
                else
                {
                    MySettings.Default.WPMultiMonitor = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WPRecursiveSearchToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (WPRecursiveSearchToggle.Text == "On")
                {
                    MySettings.Default.WPSearchThoughFolders = true;
                }
                if (WPRecursiveSearchToggle.Text == "Off")
                {
                    MySettings.Default.WPSearchThoughFolders = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WPChangeIntervalComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MySettings.Default.WPChangeInterval = WPChangeIntervalComboBox.Text;
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WPFitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MySettings.Default.WPStyle = WPFitComboBox.Text;
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WPShuffleToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (WPShuffleToggle.Checked == true)
                {
                    MySettings.Default.WPShuffle = true;
                }
                else
                {
                    MySettings.Default.WPShuffle = false;
                }
                MySettings.Default.Save();
            }
            catch
            {
                //log
            }
        }

        private void WPAddFolderButton_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog newFolder = new FolderBrowserDialog();

                int indexCount = WPLocalFoldersComboBox.Items.Count;

                newFolder.ShowNewFolderButton = true;
                newFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;

                //call the window
                DialogResult result = newFolder.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //this code will run when the user his OK or OPEN
                    string FolderName = newFolder.SelectedPath;
                    WPLocalFoldersComboBox.Items.Add(FolderName);

                    //Save the value for later in settings
                    MySettings.Default.WPDirectories.Insert(indexCount, FolderName);
                    MySettings.Default.Save();
                }

                //Update list of availabe tags
                //GetTagsFromFileNames();

                //index the files to find out what ID's are known
                //GetIDValuesFromFiles();

                //Now that a folder has been given, start the rotation
                //bool CancleThread = false;
                //_wp.ThreadGetTimer(CancleThread);
            }
            catch
            {
                //log
            }
        }

        private void dbKonachanLink_Click(object sender, EventArgs e)
        {
            CoreDBView("site", "k");
        }

        private void DBWallHavenLink_Click(object sender, EventArgs e)
        {
            CoreDBView("site", "w");
        }

        private void DBYandereLink_Click(object sender, EventArgs e)
        {
            CoreDBView("site", "y");
        }

        private void DBLocalLink_Click(object sender, EventArgs e)
        {
            CoreDBView("local", null);
        }

        private void DBTagsLink_Click(object sender, EventArgs e)
        {
            CoreDBView("tags", null);
        }

        private void ActiveTagsAddButton_Click(object sender, EventArgs e)
        {
            if(ActiveTagsTextBox.Text == "")
            {
                MetroMessageBox.Show(this, "Unable to add a null value :)", "WallSwitch", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                try
                {
                    MySettings.Default.CoreActiveTags.Insert(MySettings.Default.CoreActiveTags.Count, ActiveTagsTextBox.Text);
                    MySettings.Default.Save();

                    ActiveTagsComboBox.Items.Clear();
                    ActiveTagsComboBox.Items.AddRange(MySettings.Default.CoreActiveTags.Cast<string>().ToArray<string>());

                    ActiveTagsTextBox.Clear();
                }
                catch
                {
                    //log
                }
                
            }
        }

        private void ActiveTagsRemoveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(ActiveTagsComboBox.Text == "")
                {
                    MetroMessageBox.Show(this, "Unable to remove a null value :)", "WallSwitch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult t = MetroMessageBox.Show(this, "Are you sure you want to remove the tag '" + ActiveTagsComboBox.Text + "'?", "WallSwitch", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (t == DialogResult.Yes)
                    {
                        MySettings.Default.CoreActiveTags.Remove(ActiveTagsComboBox.Text);
                        MySettings.Default.Save();

                        ActiveTagsComboBox.Items.Clear();
                        ActiveTagsComboBox.Items.AddRange(MySettings.Default.CoreActiveTags.Cast<string>().ToArray<string>());
                    }
                }
            }
            catch
            {
                //log
            }
        }

        private void BlockedTagsAddButton_Click(object sender, EventArgs e)
        {
            if (BlockedTagsTextBox.Text == "")
            {
                MetroMessageBox.Show(this, "Unable to add a null value :)", "WallSwitch", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                try
                {
                    MySettings.Default.CoreBlockedTags.Insert(MySettings.Default.CoreBlockedTags.Count, BlockedTagsTextBox.Text);
                    MySettings.Default.Save();

                    BlockedTagsComboBox.Items.Clear();
                    BlockedTagsComboBox.Items.AddRange(MySettings.Default.CoreBlockedTags.Cast<string>().ToArray<string>());

                    BlockedTagsTextBox.Clear();
                }
                catch
                {
                    //log
                }

            }
        }

        private void BlockedRemoveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (BlockedTagsComboBox.Text == "")
                {
                    MetroMessageBox.Show(this, "Unable to remove a null value :)", "WallSwitch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult t = MetroMessageBox.Show(this, "Are you sure you want to remove the tag '" + BlockedTagsComboBox.Text + "'?", "WallSwitch", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (t == DialogResult.Yes)
                    {
                        MySettings.Default.CoreBlockedTags.Remove(BlockedTagsTextBox.Text);
                        MySettings.Default.Save();

                        BlockedTagsComboBox.Items.Clear();
                        BlockedTagsComboBox.Items.AddRange(MySettings.Default.CoreBlockedTags.Cast<string>().ToArray<string>());
                    }
                }
            }
            catch
            {
                //log
            }
        }


    }
}
