using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WallSwitch;

namespace WallSwitch.src.wallpaper
{
    class wallpaperLib
    {
       
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        public static void Set(string wpaper, string style)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == "Fill")
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == "Fit")
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == "Stretched")
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == "Centered")
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == "Tiled")
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            //string tempPath = "Resources\\" + wpaper;
            string tempPath = wpaper;
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public void SetWallpaper(string FileName)
        {
            Set(FileName, MySettings.Default.WPStyle);
        }

        public List<string> GetPicuresFromFiles()
        {
            //get the string values of the directories given
            string[] test2 = MySettings.Default.WPDirectories.Cast<string>().ToArray<string>();

            List<string> AllPictures = new List<string>();

            if (MySettings.Default.WPSearchThoughFolders == true)
            //A root folder was given and going to search though all the folders for pictures
            {
                for (int x = 0; x < MySettings.Default.WPDirectories.Count; x++)
                {
                    //Looking for file names of pictures per folder given by user
                    string[] PictureFilesJPG = Directory.GetFiles(test2[x], "*.jpg", SearchOption.AllDirectories);
                    string[] PictureFilesPNG = Directory.GetFiles(test2[x], "*.png", SearchOption.AllDirectories);

                    //take the values given and load them into the list
                    AllPictures.AddRange(PictureFilesJPG);
                    AllPictures.AddRange(PictureFilesPNG);

                }
                return AllPictures;
            }
            else
            {
                for (int x = 0; x < MySettings.Default.WPDirectories.Count; x++)
                {
                    //Looking for file names of pictures per folder given by user
                    string[] PictureFilesJPG = Directory.GetFiles(test2[x], "*.jpg");
                    string[] PictureFilesPNG = Directory.GetFiles(test2[x], "*.png");

                    //take the values given and load them into the list
                    AllPictures.AddRange(PictureFilesJPG);
                    AllPictures.AddRange(PictureFilesPNG);
                }
                return AllPictures;
            }
        }

        public List<string> GetTagsFromFiles(List<string> AllPictures)
        {
            //get the text values of the tags
            string[] StringUserTags = MySettings.Default.CoreActiveTags.Cast<string>().ToArray<string>();

            //Store the values found for pictures into this list for later parsing
            List<string> ListUserTags = new List<string>();

            for (int i = 0; i < MySettings.Default.CoreActiveTags.Count; i++)
            {
                var ArrayUserTags = AllPictures.Distinct().Where(f => f.Contains(StringUserTags[i].ToLower()));
                ListUserTags.AddRange(ArrayUserTags);
            }
            Debug.WriteLine(ListUserTags.ToString());

            //clean out all data in AllPictures given we now have new values we want to use
            AllPictures.Clear();

            //merge the new values we have to AllPictures so we dont have to change the logic later on
            AllPictures.AddRange(ListUserTags);

            //clear values from ListUserTags to free up some memory
            ListUserTags.Clear();

            return AllPictures;
        }

        public int GetRNGNumber(int min, int max)
        {
            Random RNG = new Random();

            int result = RNG.Next(min, max);

            return result;
        }

        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            int newWidth = 0;
            int newHeight = 0;

            if (image.Width != width)
            {
                newWidth = ResizeImageCalc(image.Width, width);
            }
            else
            {
                newWidth = width;
            }

            if (image.Height != height)
            {
                newHeight = ResizeImageCalc(image.Height, height);
            }
            else
            {
                newHeight = height;
            }

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                    //squishes the picture
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

                    //not squished but cuts the buttom off
                    //graphics.DrawImage(image, destRect, 0, 0, width, height, GraphicsUnit.Pixel, wrapMode);

                    //graphics.DrawImage(image, destRect, newWidth, newHeight, width, height, GraphicsUnit.Pixel, wrapMode);

                //graphics.DrawImage(image,destImage, 0, 0, width,)
                }
            }

            return destImage;
        }

        private int ResizeImageCalc(int img, int monitor)
        {

            int calc1 = img - monitor;
            int calc2 = calc1 / 2;

            return calc2;

        }

        public List<Monitors> GetNumberOfMonitors()
        {
            List<Monitors> listMonitors = new List<Monitors>();

            int NumberOfMonitors = 0;

            //check to see how many screens are on the system
            foreach (var screen in Screen.AllScreens)
            {
                //get the height and width values and store them in the list
                listMonitors.Add
                    (new Monitors
                    {
                        MonitorNumber = NumberOfMonitors,
                        MonitorHeight = screen.Bounds.Height,
                        MonitorWidth = screen.Bounds.Width
                    }
                    );

                NumberOfMonitors++;
            }

            return listMonitors;
        }

        private string OldDualScreenStitch(List<string> AllPictures)
        {
            int counter = MySettings.Default.WPInOrderCounter;
            List<Monitors> listMonitors = GetNumberOfMonitors();

            //checking to see if the counter needs to be reset to start the loop over again

            if (counter >= AllPictures.Count())
            {
                counter = 0;
            }
            string picture1 = AllPictures.ElementAt(counter);
            counter++;

            if (counter >= AllPictures.Count())
            {
                counter = 0;
            }
            string picture2 = AllPictures.ElementAt(counter);
            counter++;

            //convert to images from strings
            Image img1 = Image.FromFile(picture1);
            Image img2 = Image.FromFile(picture2);

            Bitmap bmp1 = new Bitmap(img1);
            bmp1.SetResolution(listMonitors[0].MonitorWidth, listMonitors[0].MonitorHeight);

            Bitmap bmp2 = new Bitmap(img2);
            bmp2.SetResolution(listMonitors[1].MonitorWidth, listMonitors[1].MonitorHeight);

            //calc width
            int PictureWidth = img1.Width + img2.Width;

            int TotalScreenWidth = listMonitors[0].MonitorWidth + listMonitors[1].MonitorHeight;

            //get the highest point on one of these two files
            //int MaxHeight = Math.Max(img1.Height, img2.Height);

            //pull the height from the monitors
            int MaxHeight = Math.Max(listMonitors[0].MonitorHeight, listMonitors[1].MonitorHeight);

            //set the new pictures dimentions based off values we pulled
            Bitmap img3 = new Bitmap(TotalScreenWidth, MaxHeight);

            //convert img3 to a graphic.. not sure why
            Graphics g = Graphics.FromImage(img3);

            //merge pictures
            g.Clear(Color.Black);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(bmp1, new Rectangle(0, 0, listMonitors[0].MonitorWidth, listMonitors[0].MonitorHeight));
            g.DrawImage(bmp2, new Rectangle(listMonitors[0].MonitorWidth, 0, listMonitors[1].MonitorWidth, listMonitors[1].MonitorHeight));


            //cleanup memory
            g.Dispose();
            img1.Dispose();
            img2.Dispose();
            bmp1.Dispose();
            bmp2.Dispose();

            //save picture
            string temp = System.IO.Path.GetTempPath();
            string SavedFile = temp + "\\merged.png";
            img3.Save(SavedFile, System.Drawing.Imaging.ImageFormat.Png);

            //cleanup memory
            img3.Dispose();

            //set the value of wpaper so it will display correctly
            //string wpaper = SavedFile;
            return SavedFile;
        }

        public string MultiMonitorStitch(string file1, string file2, string file3, string file4)
        {
            string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\";
            string SavedFile = null;

            List<Monitors> listMonitors = GetNumberOfMonitors();
            string fileMerged = FolderPath + "merged.png";

            //int widthTotal = Screen.GetBounds

            if (listMonitors.Count == 2)
            {
                Image img1 = Image.FromFile(file1);
                Image img2 = Image.FromFile(file2);

                Bitmap picture1 = ResizeImage(img1, listMonitors[0].MonitorWidth, listMonitors[0].MonitorHeight);
                Bitmap picture2 = ResizeImage(img2, listMonitors[1].MonitorWidth, listMonitors[1].MonitorHeight);

                Bitmap Merged =  BuildMergedFile(picture1, picture2, null, null, listMonitors);
                Merged.Save(fileMerged, ImageFormat.Png);
                SetWallpaper(fileMerged);

                //cleanup
                img1.Dispose();
                img2.Dispose();
                picture1.Dispose();
                picture2.Dispose();
                Merged.Dispose();
                DeleteFiles(file1, file2, null, null, fileMerged);

            }

            if (listMonitors.Count == 3)
            {
                Image img1 = Image.FromFile(file1);
                Image img2 = Image.FromFile(file2);
                Image img3 = Image.FromFile(file3);

                Bitmap picture1 = ResizeImage(img1, listMonitors[0].MonitorWidth, listMonitors[0].MonitorHeight);
                Bitmap picture2 = ResizeImage(img2, listMonitors[1].MonitorWidth, listMonitors[1].MonitorHeight);
                Bitmap picture3 = ResizeImage(img3, listMonitors[2].MonitorWidth, listMonitors[2].MonitorHeight);

                Bitmap Merged = BuildMergedFile(picture1, picture2, picture3, null, listMonitors);

                Merged.Save(fileMerged, ImageFormat.Png);
                //Merged.Save(FolderPath + "debug.png", ImageFormat.Png);

                SetWallpaper(fileMerged);

                //cleanup
                img1.Dispose();
                img2.Dispose();
                img3.Dispose();
                picture1.Dispose();
                picture2.Dispose();
                picture3.Dispose();
                Merged.Dispose();

                DeleteFiles(file1, file2, file3, null, fileMerged);

            }
            if (listMonitors.Count == 4)
            {
                Image img1 = Image.FromFile(file1);
                Image img2 = Image.FromFile(file2);
                Image img3 = Image.FromFile(file3);
                Image img4 = Image.FromFile(file4);

                Bitmap picture1 = ResizeImage(img1, listMonitors[0].MonitorWidth, listMonitors[0].MonitorHeight);
                Bitmap picture2 = ResizeImage(img2, listMonitors[1].MonitorWidth, listMonitors[1].MonitorHeight);
                Bitmap picture3 = ResizeImage(img3, listMonitors[2].MonitorWidth, listMonitors[2].MonitorHeight);
                Bitmap picture4 = ResizeImage(img4, listMonitors[3].MonitorWidth, listMonitors[3].MonitorHeight);

                Bitmap Merged = BuildMergedFile(picture1, picture2, picture3, picture4, listMonitors);

                Merged.Save(fileMerged, ImageFormat.Png);
                SetWallpaper(fileMerged);

                //cleanup
                img1.Dispose();
                img2.Dispose();
                img3.Dispose();
                img4.Dispose();
                picture1.Dispose();
                picture2.Dispose();
                picture3.Dispose();
                picture4.Dispose();
                Merged.Dispose();

                DeleteFiles(file1, file2, file3, file4, fileMerged);
            }

            //string wpaper = SavedFile;

            return SavedFile;
        }

        private Bitmap BuildMergedFile(Bitmap picture1, Bitmap picture2, Bitmap picture3, Bitmap picture4, List<Monitors> listMonitors)
        {
            if(listMonitors.Count == 4)
            {
                int TotalScreenWidth = listMonitors[0].MonitorWidth + listMonitors[1].MonitorWidth + listMonitors[2].MonitorWidth + listMonitors[3].MonitorWidth;

                int Monitor2Start = listMonitors[0].MonitorWidth;
                int Monitor3Start = listMonitors[0].MonitorWidth + listMonitors[1].MonitorWidth;
                int Monitor4Start = listMonitors[0].MonitorWidth + listMonitors[1].MonitorWidth + listMonitors[2].MonitorWidth;

                int MaxHeight = 1080;

                //stitch them together
                //set the new pictures dimentions based off values we pulled
                Bitmap img0 = new Bitmap(TotalScreenWidth, MaxHeight);

                //convert img3 to a graphic.. not sure why
                using (Graphics g = Graphics.FromImage(img0))
                {
                    //merge pictures
                    g.Clear(Color.Black);

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(picture1, new Rectangle(0, 0, listMonitors[0].MonitorWidth, listMonitors[0].MonitorHeight));
                    g.DrawImage(picture2, new Rectangle(Monitor2Start, 0, listMonitors[1].MonitorWidth, listMonitors[1].MonitorHeight));
                    g.DrawImage(picture3, new Rectangle(Monitor3Start, 0, listMonitors[2].MonitorWidth, listMonitors[2].MonitorHeight));
                    g.DrawImage(picture4, new Rectangle(Monitor4Start, 0, listMonitors[3].MonitorWidth, listMonitors[3].MonitorHeight));
                }

                return img0;
            }

            if(listMonitors.Count == 3)
            {
                int TotalScreenWidth = listMonitors[0].MonitorWidth + listMonitors[1].MonitorWidth + listMonitors[2].MonitorWidth;

                int Monitor2Start = listMonitors[0].MonitorWidth;
                int Monitor3Start = listMonitors[0].MonitorWidth + listMonitors[1].MonitorWidth;

                int MaxHeight = 1080;

                //stitch them together
                //set the new pictures dimentions based off values we pulled
                Bitmap img0 = new Bitmap(TotalScreenWidth, MaxHeight);

                //convert img3 to a graphic.. not sure why
                using (Graphics g = Graphics.FromImage(img0))
                {
                    //merge pictures
                    g.Clear(Color.Black);

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(picture1, new Rectangle(0, 0, listMonitors[0].MonitorWidth, listMonitors[0].MonitorHeight));
                    g.DrawImage(picture2, new Rectangle(Monitor2Start, 0, listMonitors[1].MonitorWidth, listMonitors[1].MonitorHeight));
                    g.DrawImage(picture3, new Rectangle(Monitor3Start, 0, listMonitors[2].MonitorWidth, listMonitors[2].MonitorHeight));
                }

                return img0;
            }

            if(listMonitors.Count == 2)
            {
                int TotalScreenWidth = listMonitors[0].MonitorWidth + listMonitors[1].MonitorWidth;

                int Monitor2Start = listMonitors[0].MonitorWidth;

                int MaxHeight = 1080;

                //stitch them together
                //set the new pictures dimentions based off values we pulled
                Bitmap img0 = new Bitmap(TotalScreenWidth, MaxHeight);

                //convert img3 to a graphic.. not sure why
                using (Graphics g = Graphics.FromImage(img0))
                {
                    //merge pictures
                    g.Clear(Color.Black);

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(picture1, new Rectangle(0, 0, listMonitors[0].MonitorWidth, listMonitors[0].MonitorHeight));
                    g.DrawImage(picture2, new Rectangle(Monitor2Start, 0, listMonitors[1].MonitorWidth, listMonitors[1].MonitorHeight));
                }

                return img0;
            }

            return null;

        }

        public void DeleteFiles(string file1, string file2, string file3, string file4, string fileMerged)
        {
            try
            {
                File.Delete(file1);
            }
            catch
            {

            }

            try
            {
                File.Delete(file2);
            }
            catch
            {

            }

            try
            {
                File.Delete(file3);
            }
            catch
            {

            }

            try
            {
                File.Delete(file4);
            }
            catch
            {

            }
            try
            {
                File.Delete(fileMerged);
            }
            catch
            {

            }
        }

    }
}
