using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;


namespace WallSwitch.src.sql
{
    class sqlYandere
    {

        public List<SQLData> db_GetYandereData()
        {
            string workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\WallSwitch.db";

            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    //string Query = "SELECT * FROM [aspTest].[dbo].[tbl.Index] WHERE rating LIKE 's' ORDER BY id DESC";
                    string Query = "SELECT * FROM 'tbl.Yandere'";

                    SQLiteCommand myCommand = new SQLiteCommand(Query, m_dbConection);
                    SQLiteDataReader reader = myCommand.ExecuteReader();

                    List<SQLData> NewList = new List<SQLData>();

                    while (reader.Read())
                    {
                        NewList.Add(new SQLData
                        {
                            id = reader.GetInt32(0),
                            siteID = reader.GetInt32(1),
                            tags = reader.GetString(2),
                            rating = reader.GetString(3),
                            preview_url = reader.GetString(4),
                            jpeg_url = reader.GetString(5),
                            jpeg_width = reader.GetInt32(6),
                            jpeg_height = reader.GetInt32(7),
                            date_added = reader.GetString(8),
                            favorite = reader.GetInt32(9)
                        });
                    }
                    reader.Close();
                    m_dbConection.Close();
                    return NewList;
                }
                catch(SQLiteException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return null;
        }

        public void db_AddYandereValue(int SiteID, string tags, string rating, string preview_url, string jpeg_url, int jpeg_width, int jpeg_height, string DateTimeAdded)
        {
            string workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\WallSwitch.db";

            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    using (var sqlCommand = new SQLiteCommand("INSERT INTO 'tbl.Yandere' (ID, tags, rating, preview_url, jpeg_url, jpeg_width, jpeg_height, date_added, favorite) values (@ID, @tags, @rating, @preview_url, @jpeg_url, @jpeg_width, @jpeg_height, @date_added, @favorite)", m_dbConection))
                    {
                        sqlCommand.Parameters.AddWithValue("ID", SiteID);
                        sqlCommand.Parameters.AddWithValue("tags", tags);
                        sqlCommand.Parameters.AddWithValue("rating", rating);
                        sqlCommand.Parameters.AddWithValue("preview_url", preview_url);
                        sqlCommand.Parameters.AddWithValue("jpeg_url", jpeg_url);
                        sqlCommand.Parameters.AddWithValue("jpeg_width", jpeg_width);
                        sqlCommand.Parameters.AddWithValue("jpeg_height", jpeg_height);
                        sqlCommand.Parameters.AddWithValue("date_added", DateTimeAdded);
                        sqlCommand.Parameters.AddWithValue("favorite", 0);

                        sqlCommand.ExecuteNonQuery();
                    }

                    m_dbConection.Close();

                }
                catch(SQLiteException ex)
                {
                    Debug.WriteLine(ex);
                    //MessageBox.Show("Unable to open file " + SQLiteFilePath + ". Generated Error: " + ex.Message);
                }
            }
        }
    }
}
