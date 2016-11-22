using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallSwitch.src.sql
{
    class sqlTags
    {
        public List<tagsData> db_GetTagsData()
        {
            string workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\WallSwitch.db";

            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    string Query = "SELECT * FROM 'lu.Tags'";

                    SQLiteCommand myCommand = new SQLiteCommand(Query, m_dbConection);
                    SQLiteDataReader reader = myCommand.ExecuteReader();

                    List<tagsData> NewList = new List<tagsData>();

                    while (reader.Read())
                    {
                        NewList.Add(new tagsData
                        {
                            id = reader.GetInt32(0),
                            tag = reader.GetString(1),
                            site = reader.GetString(2),
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

        public List<tagsData> db_GetTagsDataSite(string site)
        {
            string workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\WallSwitch.db";

            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    string Query = "SELECT * FROM 'lu.Tags' WHERE site LIKE '%" + site + "%' ";

                    SQLiteCommand myCommand = new SQLiteCommand(Query, m_dbConection);
                    SQLiteDataReader reader = myCommand.ExecuteReader();

                    List<tagsData> NewList = new List<tagsData>();

                    while (reader.Read())
                    {
                        NewList.Add(new tagsData
                        {
                            id = reader.GetInt32(0),
                            tag = reader.GetString(1),
                            site = reader.GetString(2),
                        });
                    }
                    reader.Close();
                    m_dbConection.Close();
                    return NewList;
                }
                catch
                {
                    //Debug.WriteLine(ex);
                }
            }
            return null;
        }

        public void db_AddTagsValue(string tag, string site)
        {
            string workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\WallSwitch.db";

            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    using (var sqlCommand = new SQLiteCommand("INSERT INTO 'lu.Tags' (tag, site) values (@tag, @site)", m_dbConection))
                    { 
                        sqlCommand.Parameters.AddWithValue("tag", tag);
                        sqlCommand.Parameters.AddWithValue("site", site);

                        sqlCommand.ExecuteNonQuery();
                    }

                    m_dbConection.Close();

                }
                catch
                {
                    //MessageBox.Show("Unable to open file " + SQLiteFilePath + ". Generated Error: " + ex.Message);
                }
            }
        }
    }
}
