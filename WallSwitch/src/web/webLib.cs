using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallSwitch.src.ui;

namespace WallSwitch.src.web
{
    class webLib
    {
        wsLib _wsLib = new wsLib();

        public string GetSiteName(string url)
        {

            string siteName = "Unknown";

            if (url.Contains("konachan.com"))
            {
                siteName = "konachan.com";
            }

            if (url.Contains("yande.re"))
            {
                siteName = "yande.re";
            }

            if (url.Contains("wallhaven.cc"))
            {
                siteName = "wallhaven.cc";
            }

            return siteName;
        }

        public string GetFileType(string url)
        {
            string fileType = null;

            //figure out what file type the source is from JSON
            if (url.EndsWith(".jpg") == true)
            {
                fileType = ".jpg";
                //FullFileName = FolderPath + "\\" + SiteName + " - " + url[0].siteID + ".jpg";
                //Download.DownloadFile(url[0].jpeg_url, FullFileName);
            }

            if (url.EndsWith(".png") == true)
            {
                fileType = ".png";                
                //Download.DownloadFile(url[0].jpeg_url, FullFileName);
            }

            return fileType;
        }

        public void LogNumberOfUpdates(string site, int konachanCounter, int yandereCounter, int wallhavenCounter)
        {
            switch (site)
            {
                case "konachan.com":
                    if (konachanCounter != 0)
                    {
                        _wsLib.LogWrite("u", "db", "Added " + konachanCounter + " files to 'tbl.Konachan'.");
                        konachanCounter = 0;
                    }
                    else
                    {
                        _wsLib.LogWrite("i", "db", "No updates found on Konachan.com");
                    }
                    break;
                case "yande.re":
                    if (yandereCounter != 0)
                    {
                        _wsLib.LogWrite("u", "db", "Added " + yandereCounter + " files to 'tbl.Yandere'.");
                        yandereCounter = 0;
                    }
                    else
                    {
                        _wsLib.LogWrite("i", "db", "No updates found on Yande.re");
                    }
                    break;
                case "wallhaven.cc":
                    if (wallhavenCounter != 0)
                    {
                        _wsLib.LogWrite("u", "db", "Added " + wallhavenCounter + " files to 'tbl.Wallhaven'.");
                        wallhavenCounter = 0;
                    }
                    else
                    {
                        _wsLib.LogWrite("i", "db", "No updates found on Wallhaven.cc");
                    }
                    break;
            }
        }
    }
}
