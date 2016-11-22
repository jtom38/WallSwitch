using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using WallSwitch.src.sql;
using System.Diagnostics;

namespace WallSwitch.src.ui
{
    class wsLib
    {
        sqlKonachan _sqlKona = new sqlKonachan();
        sqlWallhaven _sqlWH = new sqlWallhaven();
        sqlYandere _sqlYan = new sqlYandere();
        sqlTags _sqlTags = new sqlTags();

        //public string logFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\log.txt";
        public string logFilePortable = ".\\log.txt";
        public string logFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\log.txt";

        public string databaseFilePortable = ".\\WallSwitch.db";
        public string databaseFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\WallSwitch.db";

        public void LogWrite(string logType, string logScope, string logLine)
        {
            string ReturnedType = GetLogType(logType);

            string ReturnedScope = GetLogScope(logScope);

            //take the logline and add the return key
            string newLine = ReturnedType + " - " + ReturnedScope + " - " + DateTime.Now.ToString("s") + " - " + logLine;

            if(MySettings.Default.CorePortableMode == true)
            {
                LogWritePortableTrue(newLine);
            }
            else
            {
                LogWritePortableFalse(newLine);
            }
            
        }

        private void LogWritePortableTrue(string newLine)
        {

            if (!File.Exists(logFilePortable))
            {
                using (StreamWriter file = File.CreateText(logFilePortable))
                {
                    file.WriteLine(newLine);
                }
            }

            using (StreamWriter file = File.AppendText(logFilePortable))
            {
                file.WriteLine(newLine);
            }
        }

        private void LogWritePortableFalse(string newLine)
        {
            if (!File.Exists(logFile))
            {
                using (StreamWriter file = File.CreateText(logFile))
                {
                    file.WriteLine(newLine);
                }
            }

            using (StreamWriter file = File.AppendText(logFile))
            {
                file.WriteLine(newLine);
            }
        }

        private string GetLogType(string logType)
        {
            string ReturnedType = null; 
            switch (logType)
            {
                case "i":
                    ReturnedType = "Infomation";
                    break;
                case "e":
                    ReturnedType = "Error";
                    break;
                case "u":
                    ReturnedType = "Update";
                    break;
                case "n":
                    ReturnedType = "New";
                    break;
            }

            return ReturnedType;
        }

        private string GetLogScope(string logScope)
        {
            string ReturnedScope = null;

            switch (logScope)
            {
                case "db":
                    ReturnedScope = "Database";
                    break;
                case "wp":
                    ReturnedScope = "Wallpaper";
                    break;
                case "sys":
                    ReturnedScope = "System";
                    break;
                case "web":
                    ReturnedScope = "Web";
                    break; 
            }

            return ReturnedScope;
        }

        public void ParseTags()
        {
            try
            {
                List<SQLData> konaData = _sqlKona.db_GetKonachanData();
                List<string> tags = new List<string>();

                for (int i = 0; i < konaData.Count; i++)
                {
                    //remove file extention
                    string[] temp = konaData[i].tags.Split(' ');
                    tags.AddRange(temp);
                }

                //remove dupes
                var noDupesKona = tags.Distinct().ToList();

                //get existing tags in the db to make sure we dont write dupes
                List<tagsData> tagsKona = _sqlTags.db_GetTagsDataSite("konachan");

                for (int i = 0; i < noDupesKona.Count; i++)
                {
                    //make sure the 
                    var SQLResult = tagsKona.Find(x => x.tag == noDupesKona[i]);

                    if(SQLResult == null)
                    {
                        //add lines to db
                        _sqlTags.db_AddTagsValue(noDupesKona[i], "konachan");
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error wsLib.ParseTags.konaData. ex: " + ex);
            }


            try
            {
                List<string> tags = new List<string>();
                List<SQLData> yanData = _sqlYan.db_GetYandereData();
                for (int i = 0; i < yanData.Count; i++)
                {
                    string[] yanTemp = yanData[i].tags.Split(' ');
                    tags.AddRange(yanTemp);
                }

                var noDupesYan = tags.Distinct().ToList();

                //get existing tags in the db to make sure we dont write dupes
                List<tagsData> tagsYandere = _sqlTags.db_GetTagsDataSite("yandere");

                for (int i = 0; i < noDupesYan.Count; i++)
                {
                    var SQLResult = tagsYandere.Find(x => x.tag == noDupesYan[i]);

                    if(SQLResult == null)
                    {
                        _sqlTags.db_AddTagsValue(noDupesYan[i], "yandere");
                    }
                    
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error wsLib.ParseTags.yanData. ex: " + ex);
            }

            try
            {
                List<string> tags = new List<string>();
                List<SQLData> WHData = _sqlWH.db_GetWallHavenData();
                for (int i = 0; i < WHData.Count; i++)
                {
                    string[] whTemp = WHData[i].tags.Split(' ');
                    tags.AddRange(whTemp);
                }

                var noDupesWH = tags.Distinct().ToList();

                //get existing tags in the db to make sure we dont write dupes
                List<tagsData> tagsWallhaven = _sqlTags.db_GetTagsDataSite("wallhaven");

                for (int i = 0; i < noDupesWH.Count; i++)
                {
                    var SQLResult = tagsWallhaven.Find(x => x.tag == noDupesWH[i]);

                    if(SQLResult == null)
                    {
                        _sqlTags.db_AddTagsValue(noDupesWH[i], "wallhaven");
                    }

                }
            }
            catch(Exception ex)
            {
                //log
                Debug.WriteLine("Error wsLib.ParseTags.whData. ex: " + ex);
            }

        }

        public List<SQLData> CoreDBViewSites(string site)
        {
            List<SQLData> listData = new List<SQLData>();

            if (site == "k")
            {
                listData = _sqlKona.db_GetKonachanData();
            }
            if (site == "w")
            {
                listData = _sqlWH.db_GetWallHavenData();
            }
            if (site == "y")
            {
                listData = _sqlYan.db_GetYandereData();
            }

            return listData;
        }

        public int WallpaperSleepTimer()
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
        
        public string ReportTimeLeft(int counter)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(counter);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);

            return answer;
        }
    }
}
