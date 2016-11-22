using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using WallSwitch;
using WallSwitch.src.web;

namespace WallSwitch.src.wallpaper
{
    class wallpaperSingle
    {
        //used for single screen setups
        webStream _webStream = new webStream();
        wallpaperLib _wpLib = new wallpaperLib();
        wallpaperStream _wpStream = new wallpaperStream();

        int counter = MySettings.Default.WPInOrderCounter;

        public void GetFileNames()
        {           
            if(MySettings.Default.CoreUseLocalFiles == true)
            {
                LocalWallpaperOnly();
            }

            if(MySettings.Default.CoreUseWebFiles == true)
            {
                //this isnt going to work without folders to parse.
                //HA new feature is to pull data from the internet!
                //Make a request to the local db that has cached some info to make the request!
                _wpStream.StreamWallpaper();
            }
        }

        private void LocalWallpaperOnly()
        {
            if (MySettings.Default.WPDirectories.Count != 0)
            {
                //look through the files and add them to the list
                List<string> AllPictures = _wpLib.GetPicuresFromFiles();

                if (MySettings.Default.WPDirectories.Count >= 1)
                {
                    _wpLib.GetTagsFromFiles(AllPictures);
                }

                Debug.WriteLine(AllPictures);

                //get the number of monitors
                List<Monitors> listMonitors = _wpLib.GetNumberOfMonitors();

                //tell us how they want pictures.  Different on each screen or the same on all
                //true = different on each screen
                //false = same on all

                //make the string wpaper ahead of time
                string wpaper = null;

                if (MySettings.Default.WPMultiMonitor == true)
                {
                    if (listMonitors.Count >= 2)
                    {
                        HasMultiMonitor(AllPictures);
                    }
                }
                else
                {
                    //this is used for single monitor
                    Debug.WriteLine("Selecting the wallpaper to display.");
                    if (MySettings.Default.WPShuffle == true)
                    {
                        //get a random number between 0 and the total number of files in AllPictures List
                        int RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        wpaper = AllPictures.ElementAt(RNGNumber);
                    }
                    else
                    {
                        //checking to see if the counter needs to be reset to start the loop over again
                        if (counter >= AllPictures.Count())
                        {
                            counter = 0;
                        }
                        //set the value of wpaper so it will display correctly
                        wpaper = AllPictures.ElementAt(counter);
                        counter++;
                    }

                }

                _wpLib.SetWallpaper(wpaper);

                //write the counter to the settings file to be pulled at a later time like next client load

                MySettings.Default.WPInOrderCounter = counter;
                MySettings.Default.Save();
                //Debug.WriteLine("Wallpaper was changed.");
            }
        }

        private void HasMultiMonitor(List<string> AllPictures)
        {

            //make the string wpaper ahead of time
            string wpaper = null;

            //get the number of monitors
            List<Monitors> listMonitors = _wpLib.GetNumberOfMonitors();

            //so we know the system has at least two screens
            switch (listMonitors.Count)
            {
                case 2:
                    //Debug.WriteLine("Selecting the wallpaper to display.");
                    if (MySettings.Default.WPShuffle == true)
                    {

                        int RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture1 = AllPictures.ElementAt(RNGNumber);

                        //get picture 2
                        RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture2 = AllPictures.ElementAt(RNGNumber);

                        wpaper = _wpLib.MultiMonitorStitch(picture1, picture2, null, null);
                    }
                    else
                    {
                        string picture1 = AllPictures.ElementAt(counter);
                        counter++;

                        //get picture 2
                        string picture2 = AllPictures.ElementAt(counter);
                        counter++;

                        wpaper = _wpLib.MultiMonitorStitch(picture1, picture2, null, null);
                        MySettings.Default.WPInOrderCounter = counter;
                        MySettings.Default.Save();
                    }
                    break;
                case 3:
                    if (MySettings.Default.WPShuffle == true)
                    {
                        int RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture1 = AllPictures.ElementAt(RNGNumber);

                        //get picture 2
                        RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture2 = AllPictures.ElementAt(RNGNumber);

                        RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture3 = AllPictures.ElementAt(RNGNumber);

                        wpaper = _wpLib.MultiMonitorStitch(picture1, picture2, picture3, null);
                    }
                    else
                    {
                        string picture1 = AllPictures.ElementAt(counter);
                        counter++;

                        //get picture 2
                        string picture2 = AllPictures.ElementAt(counter);
                        counter++;

                        string picture3 = AllPictures.ElementAt(counter);
                        counter++;

                        wpaper = _wpLib.MultiMonitorStitch(picture1, picture2, picture3, null);
                        MySettings.Default.WPInOrderCounter = counter;
                        MySettings.Default.Save();
                    }

                    break;
                case 4:
                    if (MySettings.Default.WPShuffle == true)
                    {
                        int RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture1 = AllPictures.ElementAt(RNGNumber);

                        //get picture 2
                        RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture2 = AllPictures.ElementAt(RNGNumber);

                        RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture3 = AllPictures.ElementAt(RNGNumber);

                        RNGNumber = _wpLib.GetRNGNumber(0, AllPictures.Count());
                        string picture4 = AllPictures.ElementAt(RNGNumber);

                        wpaper = _wpLib.MultiMonitorStitch(picture1, picture2, picture3, picture4);
                    }
                    else
                    {
                        string picture1 = AllPictures.ElementAt(counter);
                        counter++;

                        //get picture 2
                        string picture2 = AllPictures.ElementAt(counter);
                        counter++;

                        string picture3 = AllPictures.ElementAt(counter);
                        counter++;

                        string picture4 = AllPictures.ElementAt(counter);
                        counter++;

                        wpaper = _wpLib.MultiMonitorStitch(picture1, picture2, picture3, picture4);
                        MySettings.Default.WPInOrderCounter = counter;
                        MySettings.Default.Save();
                    }
                    break;
            }
        }

        private int ThreadTimer()
        {
            //build in the stopwatch.
            int UserTime = 0;
            switch (MySettings.Default.WPChangeInterval)
            {
                case "5 Minutes":
                    UserTime = 300000;
                    break;
                case "15 Minutes":
                    UserTime = 900000;
                    break;
                case "30 Minutes":
                    UserTime = 1800000;
                    break;
                case "1 Hour":
                    UserTime = 3600000;
                    break;
                case "1 Day":
                    UserTime = 86400000;
                    break;
                default:
                    UserTime = 300000;
                    break;
            }

            return UserTime;
        }

        public void ThreadGetTimer(bool CancleThread)
        {
            //using System.Threading.Timer for this so its already running in its own thread.
            Thread Process = new Thread(GetTimer);

            //check first to see if the command to turn the thread off is passed
            if (CancleThread == true)
            {
                //check to see if the process is alive to close it.
                if (Process.IsAlive)
                {
                    Process.Join();
                    Debug.WriteLine("Thread should be closed.");
                }
            }
            else
            {
                //asuming the cancle command isnt sent and the thread is not on
                Process.Start();
                Debug.WriteLine("Process was started");
                GetFileNames();
            }
        }

        public void GetTimer()
        {

            //build in the stopwatch.
            int UserTime = 0;
            switch (MySettings.Default.WPChangeInterval)
            {
                case "5 Minutes":
                    UserTime = 300000;
                    break;
                case "15 Minutes":
                    UserTime = 900000;
                    break;
                case "30 Minutes":
                    UserTime = 1800000;
                    break;
                case "1 Hour":
                    UserTime = 3600000;
                    break;
                case "1 Day":
                    UserTime = 86400000;
                    break;
                default:
                    UserTime = 300000;
                    break;
            }

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = UserTime;
            aTimer.Enabled = true;
            Debug.WriteLine("UserTime: " + UserTime);
            Debug.WriteLine("Timer started");
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //MessageBox.Show("Time is up");
            Debug.WriteLine("Moving to GetFileNames()");

            GetFileNames();
        }
    }
}
