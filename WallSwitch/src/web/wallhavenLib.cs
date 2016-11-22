using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WallSwitch;

/// <summary>
/// This file is more or less a library.  Add the file to whats needed and add the list data
/// then all you need to do is call getData(Pages) and get back the info from the site.
/// You will get a list with all the data in it
/// </summary>

namespace WallSwitch.src.web
{
    class wallhavenLib
    {
        public static List<wallhavenData> _wallhavenList = new List<wallhavenData>();

        public List<wallhavenData> GetData(int pages, string rating)
        {
            for(int i = 0; i < 1; i++)
            {
                string urlAddress = GetURL(pages, rating);

                Debug.WriteLine("Wallhaven.cc - Rating " + rating + " Page " + pages);                

                string pictureRating = GetRating(urlAddress);

                var data = _loadHtmlContent(urlAddress);

                var doc = new HtmlDocument();
                doc.LoadHtml(data);
                //For some weird reason you have to save it for it to work properly
                doc.Save("test.html");

                //int c = 0;

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[contains(@class,'preview')]"))
                {
                    var att = link.Attributes["href"];

                    var url = att.Value;

                    var picHtml = _loadHtmlContent(url);

                    doc.LoadHtml(picHtml);
                    //For some weird reason you have to save it for it to work properly
                    doc.Save("pic.html");

                    var picture = doc.DocumentNode.SelectSingleNode("//img[contains(@id,'wallpaper')]");

                    var pictureLink = picture.Attributes["src"].Value;
                    var pictureData = picture.Attributes["alt"].Value;
                    var pictureWidth = picture.Attributes["data-wallpaper-width"].Value;
                    var pictureHeight = picture.Attributes["data-wallpaper-height"].Value;
                    var pictureName = pictureLink.Replace("//wallpapers.wallhaven.cc/wallpapers/full/wallhaven-", "");

                    pictureLink = "https:" + pictureLink;

                    DataCleanup(pictureLink, pictureData, pictureName, pictureWidth, pictureHeight, pictureRating);

                    //Debug.WriteLine("Wallhaven.cc - Parsed page " + pages + " picture " + c);
                    //c++;
                }

                File.Delete("test.html");
                File.Delete("pic.html");

                Debug.WriteLine("Wallhaven.cc - Finished page " + pages);
            }
            return _wallhavenList;
        }

        private string GetURL(int pageNumber,string rating)
        {
            //autoincrament the page number given the loop starts at 0 but we need the first page of the site
            pageNumber++;
            
            string urlAddress = null;
            switch (rating)
            {
                case "s":
                    urlAddress = "https://alpha.wallhaven.cc/search?categories=010&purity=100&sorting=date_added&order=desc&page="+pageNumber;
                    break;
                case "q":
                    urlAddress = "https://alpha.wallhaven.cc/search?categories=010&purity=010&sorting=date_added&order=desc&page="+pageNumber;
                    break;
                case "e":
                    //urlAddress = "https://alpha.wallhaven.cc/search?categories=010&purity=001&sorting=date_added&order=desc&page="+pageNumber;
                    urlAddress = "https://alpha.wallhaven.cc/search?categories=010&purity=010&sorting=date_added&order=desc&page=" + pageNumber;
                    break;
                default:
                    urlAddress = "https://alpha.wallhaven.cc/search?categories=010&purity=100&sorting=date_added&order=desc&page=" + pageNumber;
                    break;
            }

            return urlAddress;
        }

        private string GetRating(string urlAddress)
        {

            //used for the index
            string pictureRating = null;

            if (urlAddress.Contains("purity=100"))
            {
                pictureRating = "s";
            }

            if (urlAddress.Contains("purity=010"))
            {
                pictureRating = "q";
            }

            if (urlAddress.Contains("purity=001"))
            {
                pictureRating = "e";
            }

            return pictureRating;
        }

        private string _loadHtmlContent(string urlAddress)
        {
            string data = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }

            return data;
        }

        private void DataCleanup(string pictureLink, string pictureData, string pictureName, string pictureWidth, string pictureHeight, string pictureRating)
        {

            //pull url for full page url... might not be needed given we want the picture url
            //pull pictureLink = full picture url
            //pull pictureData = catagory, picture size, tags
            //pull pictureName = ID and file type, can drop the file type and just keep the tag
            //pictureWidth 
            //pictureHeight
            //pictureRating = this will be set based off the url purity... best way to pull it atm

            //removeing the file extention
            string pictureID = pictureName.Remove(pictureName.Length - 4, 4);

            string picturePreview = "https://alpha.wallhaven.cc/wallpapers/thumb/small/th-" + pictureID + ".jpg";

            //clean pictureData.  Need to remove the word anime and the res of the picture
            string pictureTags = pictureData.Remove(0, 6);

            _wallhavenList.Add(
                new wallhavenData
                {
                    id = int.Parse(pictureID),
                    tags = pictureData,
                    preview_url = picturePreview,
                    jpeg_url = pictureLink,
                    rating = pictureRating,
                    jpeg_width = int.Parse(pictureWidth),
                    jpeg_height = int.Parse(pictureHeight)
                }
            );
        }
    }
}
