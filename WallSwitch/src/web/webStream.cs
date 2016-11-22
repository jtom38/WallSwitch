using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WallSwitch.src.sql;
using WallSwitch.src.ui;
using System.Threading;
using WallSwitch;

namespace WallSwitch.src.web
{
    class webStream
    {
        //this is used for the web requests to deal with the streaming function
        //by streaming the logic is as folllows
        //if userflag is 2 or 3 > search konachan and save all results to wallswitch.db > parse the db for files > download file in temp > if multi monitor stitch them together > set wallpaper

        //the sql classes are used for the streaming functions
        sqlLib _sqlLib = new sqlLib();
        sqlKonachan _sqlKC = new sqlKonachan();
        sqlWallhaven _sqlWH = new sqlWallhaven();
        sqlYandere _sqlYan = new sqlYandere();
        wallhavenLib _whLib = new wallhavenLib();
        webLib _webLib = new webLib();
        wsLib _wsLib = new wsLib();

        public bool BreakSiteLoop = false;
        bool threadAbort = false;
        public List<SQLData> MasterListFromTable = new List<SQLData>();

        int konachanCounter = 0;
        int yandereCounter = 0;
        int wallhavenCounter = 0;

        public void ThreadGetContent()
        {
            Thread webThread = new Thread(new ThreadStart(SQL_GetContent));
            if (threadAbort != true)
            {
                webThread.Start();
            }
            else
            {
                if(webThread.IsAlive)
                {
                    webThread.Join();
                }               
            }           
        }

        public void ParseKonachan()
        {
            MasterListFromTable.Clear();

            List<SQLData> l = _sqlKC.db_GetKonachanData();
            if (l.Count == 0)
            {
                //fresh db!!
                //set a flag to pull data later
            }
            else
            {
                MasterListFromTable.AddRange(l);
                //we dont need to touch the db anymore other then for updates!!!
            }

            //pull all pictures, no filter
            //going to loop though 1000 file gets
            for (int i = 0; i < 20; i++)
                {
                    int c = i;
                    c++;
                    string ParseValue = "https://www.konachan.com/post.json?limit=100&page=" + c;
                    SQL_RequestContent(ParseValue);
                    if (BreakSiteLoop == true)
                    {
                        i = 20;
                    }
                }
                _webLib.LogNumberOfUpdates("konachan.com", konachanCounter, yandereCounter, wallhavenCounter);

                //reset the flag
                BreakSiteLoop = false;
            konachanCounter = 0;
        }

        public void ParseYandere()
        {
            MasterListFromTable.Clear();

            List<SQLData> l = _sqlYan.db_GetYandereData();
            if (l.Count == 0)
            {
                //fresh db!!
                //set a flag to pull data later
            }
            else
            {
                MasterListFromTable.AddRange(l);
                //we dont need to touch the db anymore other then for updates!!!
            }

            //pull all pictures, no filter
            //going to loop though 1000 file gets
            for (int i = 0; i < 20; i++)
            {
                int c = i;
                c++;
                string ParseValue = "https://yande.re/post.json?limit=100&page=" + c;
                SQL_RequestContent(ParseValue);
                if (BreakSiteLoop == true)
                {
                    i = 20;
                }
            }
            _webLib.LogNumberOfUpdates("yande.re", konachanCounter, yandereCounter, wallhavenCounter);

            //reset the flag
            BreakSiteLoop = false;

            yandereCounter = 0;
        }

        public void ParseWallhaven()
        {
            MasterListFromTable.Clear();

            List<string> rating = new List<string>();
            rating.Add("s");
            rating.Add("q");

            List<SQLData> l = _sqlWH.db_GetWallHavenData();
            if (l.Count == 0)
            {
                //fresh db!!
                //set a flag to pull data later
            }
            else
            {
                MasterListFromTable.AddRange(l);
                //we dont need to touch the db anymore other then for updates!!!
            }

            //this is a counter to cycle though the rating
            for(int r= 0; r < rating.Count; r++)
            {
                //this is to cycle though page numbers
                for (int i = 0; i < 10; i++)
                {
                    //requests one page of data. page number is i.

                    List<wallhavenData> wallhavenList = _whLib.GetData(i, rating[r]);

                    //see if its been added to the db already
                    SQL_UpdateDBWallHaven(wallhavenList);

                    wallhavenList.Clear();

                    if (BreakSiteLoop == true)
                    {
                        i = 10;
                    }
                }
            }


            _webLib.LogNumberOfUpdates("wallhaven.cc", konachanCounter, yandereCounter, wallhavenCounter);

            BreakSiteLoop = false;

            wallhavenCounter = 0;
        }

        public void SQL_GetContent()
        {
            
            //talk to the table and get all the info and dump into MasterListFromTable so we can query it later, less sql action this way more memory action
            List<SQLData> l = _sqlLib.db_GetTablesInfo();
            if (l.Count == 0)
            {
                //fresh db!!
                //set a flag to pull data later
            }
            else
            {
                MasterListFromTable.AddRange(l);
                //we dont need to touch the db anymore other then for updates!!!
            }

            //Site values will be pulled from GUI
            Debug.WriteLine("Checking Sites");
            List<string> Sites = new List<string>();
            Sites.Add("https://www.konachan.com/post.json?");
            Sites.Add("https://yande.re/post.json?");
            Sites.Add("https://alpha.wallhaven.cc");

            //Adding this now to be able to fill the value in later
            string ParseValue = null;

            //If parsing from Konachan we can add the PictureWidth and PictureHeight values
            for (int x = 0; x < Sites.Count; x++)
            {
                Debug.WriteLine("Searching " + Sites[x]);

                //Figure out what sites we are working with
                if (Sites[x].ToString() == "https://www.konachan.com/post.json?" ||
                    Sites[x].ToString() == "https://yande.re/post.json?")
                {
                    //pull all pictures, no filter
                    //going to loop though 1000 file gets
                    for (int i = 0; i < 5; i++)
                    {
                        int c = i;
                        c++;
                        ParseValue = Sites[x] + "limit=100&page=" + c;
                        SQL_RequestContent(ParseValue);
                        if (BreakSiteLoop == true)
                        {
                            i = 20;
                        }
                    }
                    _webLib.LogNumberOfUpdates(Sites[x], konachanCounter, yandereCounter, wallhavenCounter);

                    //reset the flag
                    BreakSiteLoop = false;

                }
                if(Sites[x].ToString() == "https://alpha.wallhaven.cc")
                {
                    for(int i = 0; i < 1; i++)
                    {
                        //requests one page of data. page number is i.
                        List<wallhavenData> wallhavenList = _whLib.GetData(i, null);

                        //see if its been added to the db already
                        SQL_UpdateDBWallHaven(wallhavenList);

                        wallhavenList.Clear();

                        if(BreakSiteLoop == true)
                        {
                            i = 5;
                        }
                    }

                    _webLib.LogNumberOfUpdates(Sites[x], konachanCounter, yandereCounter, wallhavenCounter);

                    BreakSiteLoop = false;
                    
                }
            }


            //sets the flag to close the thread
            threadAbort = true;

            //go back to the function to close the thread with the new value 
            ThreadGetContent();

        }

        private void SQL_RequestContent(string ParseValue)
        {
            Debug.WriteLine("Starting parse on " + ParseValue);

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ParseValue);
                httpWebRequest.Method = WebRequestMethods.Http.Post;

                // This goes out and actually performs the web client request sorta thing	
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();

                // Get the stream associated with the response.
                // so the webpage ran, it did it's thing, now we need the stream of data obtained
                Stream receiveStream = response.GetResponseStream();
                
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    
                // We got the stream recieved!
                Debug.WriteLine("Response stream received.");
                // Let's ReadToEnd (meaning, let's read to the end of the stream, it outputs the whole stream obtained as a string)
                var json = readStream.ReadToEnd();
                Debug.WriteLine(json);
                    
                //var Values = JsonConvert.DeserializeObject<List<JSONValues>>(json);
                List<JSONValues> tempList = new List<JSONValues>();

                var Values = JsonConvert.DeserializeObject<List<JSONValues>>(json);
                tempList.AddRange(Values);
                SQL_UpdateDB(tempList, ParseValue);
            }
            catch
            {
                Debug.WriteLine("Error: Failed to parse on " + ParseValue + ". Site could be down.");
                _wsLib.LogWrite("e", "web", "Failed to parse on " + ParseValue + ". Site could be down.");
            }

        }

        private void SQL_UpdateDB(List<JSONValues> SiteData, string parseValue)
        {
            for (int i = 0; i < SiteData.Count; i++)
            {

                //check the db to see if we find a match for the ID... though we could have a match with the two sites
                //string site = GetSiteNameFromURL(SiteData[i].preview_url);

                int tempID = SiteData[i].id;

                //List<SQLData> SQLResults = dbCode.CheckForDupeID(t[i].id, site);
                var SQLResults = MasterListFromTable.Find(x => x.siteID == tempID);

                string site = _webLib.GetSiteName(SiteData[i].jpeg_url);

                if(site == "konachan.com")
                {
                    if (SQLResults == null)
                    {
                        _sqlKC.db_AddKonachanValue(
                            SiteData[i].id,
                            SiteData[i].tags,
                            SiteData[i].rating,
                            SiteData[i].preview_url,
                            SiteData[i].jpeg_url,
                            SiteData[i].jpeg_width,
                            SiteData[i].jpeg_height,
                            DateTime.Now.ToString("s"));
                        konachanCounter++;
                    }
                    else
                    {
                        //send a break command to stop parsing that site
                        BreakSiteLoop = true;

                        //this will force the loop to go to the max counter and break out
                        i = SiteData.Count;
                    }
                }

                if (site == "yande.re")
                {
                    if (SQLResults == null)
                    {
                        _sqlYan.db_AddYandereValue(
                            SiteData[i].id,
                            SiteData[i].tags,
                            SiteData[i].rating,
                            SiteData[i].preview_url,
                            SiteData[i].jpeg_url,
                            SiteData[i].jpeg_width,
                            SiteData[i].jpeg_height,
                            DateTime.Now.ToString("s"));
                        yandereCounter++;
                    }
                    else
                    {
                        
                        //send a break command to stop parsing that site
                        BreakSiteLoop = true;

                        //this will force the loop to go to the max counter and break out
                        i = SiteData.Count;
                    }
                }
            }
        }

        private void SQL_UpdateDBWallHaven(List<wallhavenData> SiteData)
        {
            for (int i = 0; i < SiteData.Count; i++)
            {
                //check the db to see if we find a match for the ID... though we could have a match with the two sites
                //string site = GetSiteNameFromURL(SiteData[i].jpeg_url);

                int tempID = SiteData[i].id;

                //List<SQLData> SQLResults = dbCode.CheckForDupeID(t[i].id, site);
                var SQLResults = MasterListFromTable.Find(x => x.siteID == tempID);

                if (SQLResults == null)
                {
                    _sqlWH.db_AddWallhavenValue(
                        SiteData[i].id,
                        SiteData[i].tags,
                        SiteData[i].rating,
                        SiteData[i].preview_url,
                        SiteData[i].jpeg_url,
                        SiteData[i].jpeg_width,
                        SiteData[i].jpeg_height,
                        DateTime.Now.ToString("s"));
                    wallhavenCounter++;
                }
                else
                {
                    
                    //send a break command to stop parsing that site
                    BreakSiteLoop = true;

                    //this will force the loop to go to the max counter and break out
                    i = SiteData.Count;
                }
            }
        }

        public string SQL_CacheFile(List<SQLData> url)
        {
            string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\";
            string FolderPathPortable = ".\\";
            string FullFileName = null;

            if(MySettings.Default.CorePortableMode == true)
            {
                using (WebClient Download = new WebClient())
                {
                    //declare the site we are pulling from
                    string SiteName = _webLib.GetSiteName(url[0].jpeg_url);

                    string fileType = _webLib.GetFileType(url[0].jpeg_url);

                    //figure out what file type the source is from JSON
                    FullFileName = FolderPathPortable + SiteName + " - " + url[0].siteID + fileType;
                    Download.DownloadFile(url[0].jpeg_url, FullFileName);
                }
            }
            else
            {
                using (WebClient Download = new WebClient())
                {
                    //declare the site we are pulling from
                    string SiteName = _webLib.GetSiteName(url[0].jpeg_url);

                    string fileType = _webLib.GetFileType(url[0].jpeg_url);

                    //figure out what file type the source is from JSON
                    FullFileName = FolderPath + SiteName + " - " + url[0].siteID + fileType;
                    Download.DownloadFile(url[0].jpeg_url, FullFileName);
                }
            }



            _wsLib.LogWrite("i", "web", "File downloaded " + url[0].jpeg_url);

            return FullFileName;
        }

    }
}
