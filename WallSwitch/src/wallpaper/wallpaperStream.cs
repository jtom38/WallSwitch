using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallSwitch.src.web;
using WallSwitch.src.sql;
using WallSwitch;

namespace WallSwitch.src.wallpaper
{
    class wallpaperStream
    {
        webStream _webStream = new webStream();
        wallpaperLib _wpLib = new wallpaperLib();
        sqlLib _sqlLib = new sqlLib();
        sqlKonachan _sqlKona = new sqlKonachan();
        sqlYandere _sqlYan = new sqlYandere();
        sqlWallhaven _sqlWH = new sqlWallhaven();

        public void StreamWallpaper()
        {
            //get the list of pictures in memory
           
            List<SQLData> t = GetDataFromDB();
           
            List<Monitors> listMonitors = _wpLib.GetNumberOfMonitors();

            //checking to see if we have more then one in the db
            if (t.Count >= 1)
            {
                if (MySettings.Default.WPMultiMonitor == true)
                {
                    //check for monitors
                    //int NumberOfMonitors = GetNumberOfMonitors();
                    if (listMonitors.Count >= 2)
                    {
                        //multimonitor setup
                        switch (listMonitors.Count)
                        {
                            case 2:
                                //search the db for a url that meets the requirements and send it back to us
                                List<SQLData> Monitor2_URL1 = PickPictureFromDBLogic(t, 0);

                                //download the file and return the file name
                                string Monitor2_File1 = _webStream.SQL_CacheFile(Monitor2_URL1);

                                //using this a failover... for some reason the list was being dumped after comming back from PickPictureFromDBLogic
                                //this will rebuild the list
                                if(t.Count == 0)
                                {
                                    t = GetDataFromDB();
                                }

                                List<SQLData> Monitor2_URL2 = PickPictureFromDBLogic(t, 1);
                                string Monitor2_File2 = _webStream.SQL_CacheFile(Monitor2_URL2);

                                //stitch the files together
                                _wpLib.MultiMonitorStitch(Monitor2_File1, Monitor2_File2, null, null);
                                break;
                            case 3:
                                List<SQLData> Monitor3_URL1 = PickPictureFromDBLogic(t, 0);
                                string Monitor3_File1 = _webStream.SQL_CacheFile(Monitor3_URL1);

                                if (t.Count == 0)
                                {
                                    t = GetDataFromDB();
                                }

                                List<SQLData> Monitor3_URL2 = PickPictureFromDBLogic(t, 1);
                                string Monitor3_File2 = _webStream.SQL_CacheFile(Monitor3_URL2);

                                if (t.Count == 0)
                                {
                                    t = GetDataFromDB();
                                }

                                List<SQLData> Monitor3_URL3 = PickPictureFromDBLogic(t, 2);
                                string Monitor3_File3 = _webStream.SQL_CacheFile(Monitor3_URL3);

                                _wpLib.MultiMonitorStitch(Monitor3_File1, Monitor3_File2, Monitor3_File3, null);
                                break;
                            case 4:
                                List<SQLData> Monitor4_URL1 = PickPictureFromDBLogic(t, 0);
                                string Monitor4_File1 = _webStream.SQL_CacheFile(Monitor4_URL1);

                                if (t.Count == 0)
                                {
                                    t = GetDataFromDB();
                                }

                                List<SQLData> Monitor4_URL2 = PickPictureFromDBLogic(t, 1);
                                string Monitor4_File2 = _webStream.SQL_CacheFile(Monitor4_URL2);

                                if (t.Count == 0)
                                {
                                    t = GetDataFromDB();
                                }

                                List<SQLData> Monitor4_URL3 = PickPictureFromDBLogic(t, 2);
                                string Monitor4_File3 = _webStream.SQL_CacheFile(Monitor4_URL3);

                                if (t.Count == 0)
                                {
                                    t = GetDataFromDB();
                                }

                                List<SQLData> Monitor4_URL4 = PickPictureFromDBLogic(t, 3);
                                string Monitor4_File4 = _webStream.SQL_CacheFile(Monitor4_URL4);

                                _wpLib.MultiMonitorStitch(Monitor4_File1, Monitor4_File2, Monitor4_File3, Monitor4_File4);
                                break;
                        }

                    }
                }
                else
                {
                    //single monitor setup
                    //search the db for a url that meets the requirements and send it back to us
                    List<SQLData> url = PickPictureFromDBLogic(t, 0);

                    //download the file and return the file name
                    string File1 = _webStream.SQL_CacheFile(url);

                    _wpLib.SetWallpaper(File1);

                    _wpLib.DeleteFiles(File1, null, null, null, null);
                }
            }
        }

        private List<SQLData> GetDataFromDB()
        {
            List<SQLData> t = new List<SQLData>();
            List<SQLData> temp = new List<SQLData>();

            if (MySettings.Default.rssSiteKonachan == true)
            {
                temp = _sqlKona.db_GetKonachanData();
                t.AddRange(temp);

                temp.Clear();
            }


            if (MySettings.Default.rssSiteWallhaven == true)
            {
                temp = _sqlWH.db_GetWallHavenData();
                t.AddRange(temp);

                temp.Clear();
            }


            if (MySettings.Default.rssSiteYandre == true)
            {
                temp = _sqlYan.db_GetYandereData();
                t.AddRange(temp);

                temp.Clear();
            }

            return t;
        }

        public List<SQLData> PickPictureFromDBLogic(List<SQLData> dbIndex, int MonitorNumber)
        {
            List<SQLData> RefinedList = dbIndex;

            List<Monitors> listMonitors = _wpLib.GetNumberOfMonitors();

            //need to filter some data

            //blacklist
            List<SQLData> listBlacklist = new List<SQLData>();
            if (MySettings.Default.CoreBlockedTags.Count != 0)
            {
                List<SQLData> listBlackList =  getBlacklist(dbIndex);
            }
            else
            {
                listBlacklist = RefinedList;
            }

            //whitelist
            List<SQLData> listWhiteList = new List<SQLData>();
            if (MySettings.Default.CoreActiveTags.Count != 0)
            {
                listWhiteList = getWhiteList(listBlacklist);
            }
            else
            {
                listWhiteList = RefinedList;
            }

            //select width
            List<SQLData> listMatchWidth = getWidth(listWhiteList, listMonitors, MonitorNumber);

            //select height
            List<SQLData> listMatchHeight = GetHeight(listMatchWidth, listMonitors, MonitorNumber);

            //select rating
            List<SQLData> listMatchRating = GetRating(listMatchHeight);

            //select file based off rng or in order
            List<SQLData> listFilePicked = GetFile(listMatchRating);

            //cleanup
            //RefinedList.Clear();
            listWhiteList.Clear();
            listMatchWidth.Clear();
            listMatchHeight.Clear();
            listMatchRating.Clear();
            listMatchRating.Clear();

            return listFilePicked;
        }

        private List<SQLData> getBlacklist(List<SQLData> RefinedList)
        {
            List<SQLData> tempList = new List<SQLData>();

            for (int i = 0; i < MySettings.Default.CoreBlockedTags.Count; i++)
            {

                var BlacklistResults = RefinedList.FindAll(x => x.tags.Contains(MySettings.Default.CoreBlockedTags[i]));

                //tempList.RemoveAll(BlacklistResults)
                //tempList.AddRange(BlacklistResults);
                RefinedList.RemoveAll(x => x.tags.Contains(MySettings.Default.CoreBlockedTags[i]));

                //BlacklistResults.Clear();

            }
            //move the tempList to refinedList
            //RefinedList.AddRange(tempList);

            //clear out any data if any
            //tempList.Clear();

            return RefinedList;

        }

        private List<SQLData> getWhiteList(List<SQLData> RefinedList)
        {
            List<SQLData> tempList = new List<SQLData>();

            //whitelist
            for (int i = 0; i < MySettings.Default.CoreActiveTags.Count; i++)
            {
                //look though the list and find all results that contain the tag listed in settings
                var TagResults = RefinedList.FindAll(x => x.tags.Contains(MySettings.Default.CoreActiveTags[i]));

                //then add it to the master list
                tempList.AddRange(TagResults);

                TagResults.Clear();
            }

            //move the tempList to refinedList
            RefinedList.Clear();
            RefinedList.AddRange(tempList);

            return RefinedList;
        }

        private List<SQLData> getWidth(List<SQLData> RefinedList, List<Monitors> listMonitors, int MonitorNumber)
        {
            //find the results that meet the min
            var WidthResult = RefinedList.FindAll(x => x.jpeg_width >= listMonitors[MonitorNumber].MonitorWidth);

            RefinedList.Clear();

            RefinedList.AddRange(WidthResult);

            WidthResult.Clear();

            return RefinedList;
        }

        private List<SQLData> GetHeight(List<SQLData> RefinedList, List<Monitors> listMonitors, int MonitorNumber)
        {
            //find the results that meet the min
            var HeightResult = RefinedList.FindAll(x => x.jpeg_height >= listMonitors[MonitorNumber].MonitorHeight);

            //clear out the list to update the info
            RefinedList.Clear();

            //update the list with the new data
            RefinedList.AddRange(HeightResult);

            //cleanup even though garbage collection will do it anyway
            HeightResult.Clear();

            return RefinedList;
        }

        private List<SQLData> GetRating(List<SQLData> RefinedList)
        {
            switch (MySettings.Default.RSSRatings)
            {
                case "e":
                    //we dont need to do anything
                    break;
                case "q":
                    var RatingQ = RefinedList.FindAll(x => x.rating == "q");
                    var RatingQ2 = RefinedList.FindAll(x => x.rating == "s");
                    RefinedList.Clear();
                    RefinedList.AddRange(RatingQ);
                    RefinedList.AddRange(RatingQ2);

                    break;
                case "s":
                    var RatingS = RefinedList.FindAll(x => x.rating == "s");
                    RefinedList.Clear();
                    RefinedList.AddRange(RatingS);
                    break;
            }

            return RefinedList;
        }

        private List<SQLData> GetFile(List<SQLData> listMatchRating)
        {
            List<SQLData> tempList = new List<SQLData>();
            bool RerollPicture = true;

            if (MySettings.Default.WPShuffle == true)
            {
                //rng pick
                int RNGNumber = 0;
                while (RerollPicture == true)
                {
                    //picks the number
                    RNGNumber = _wpLib.GetRNGNumber(0, listMatchRating.Count());

                    //take the height and width and make sure its not a poster
                    RerollPicture = GetAspectRatio(listMatchRating[RNGNumber].jpeg_height, listMatchRating[RNGNumber].jpeg_width);
                }

                tempList.Clear();

                tempList.Add(
                    new SQLData
                    {
                        jpeg_url = listMatchRating[RNGNumber].jpeg_url,
                        siteID = listMatchRating[RNGNumber].siteID,

                    }
                );

            }
            else
            {
                //rng off
                //check to make sure the counter is still valid for this rotation
                if(MySettings.Default.WPInOrderCounter >= listMatchRating.Count)
                {
                    MySettings.Default.WPInOrderCounter = 0;
                    MySettings.Default.Save();
                }

                while(RerollPicture == true)
                {
                    RerollPicture = GetAspectRatio(listMatchRating[MySettings.Default.WPInOrderCounter].jpeg_height, listMatchRating[MySettings.Default.WPInOrderCounter].jpeg_width);
                    MySettings.Default.WPInOrderCounter++;
                    MySettings.Default.Save();
                }

                tempList.Clear();

                tempList.Add(
                    new SQLData
                    {
                        jpeg_url = listMatchRating[MySettings.Default.WPInOrderCounter].jpeg_url,
                        siteID = listMatchRating[MySettings.Default.WPInOrderCounter].siteID,

                    }
                );
            }
            
            return tempList;
        }

        private bool GetAspectRatio(int height, int width)
        {
            /// <summary>
            ///posters
            ///w2319 x h3300 
            ///where height > width calculate out with a 1.42~
            ///
            ///16:9 = .5625
            ///
            ///16:10 = 0.625
            /// </summary>

            bool RerollPicture = false;

            decimal result = height / width;
            if(result < 1)
            {
                //should be either a .5 or .6 for a valid picture
                RerollPicture = false;
            }
            else
            {
                //should be at least a 1, so its a poster we need to reroll
                RerollPicture = true;
            }
            //float result = 

            return RerollPicture;
        }
    }
}
