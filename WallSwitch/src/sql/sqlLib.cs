using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using WallSwitch.src.ui;

namespace WallSwitch.src.sql
{
    class sqlLib
    {

        sqlKonachan _sqlKC = new sqlKonachan();
        sqlWallhaven _sqlWH = new sqlWallhaven();
        sqlYandere _sqlYan = new sqlYandere();
        sqlLocal _sqlLocal = new sqlLocal();
        wsLib _wsLib = new wsLib();

        public string workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\WallSwitch.db";

        public void CheckForDB()
        {
            string t = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (Directory.Exists(t + "\\WallSwitch") == false)
            {
                Directory.CreateDirectory(t + "\\WallSwitch");
            }
            if (File.Exists(workingDirectory) == false)
            {
                MakeDBFile();

                MakeTableKonachan();

                MakeTableWallhaven();

                MakeTableYandere();

                MakeTableTags();
            }
        }

        private void MakeDBFile()
        {
            //make the file
            SQLiteConnection.CreateFile(workingDirectory);
            _wsLib.LogWrite("i", "db", "File created %appdata%\\WallSwitch\\WallSwitch.db");
        }

        private void MakeTableKonachan()
        {
            //build the connector
            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    //build the command
                    string sqlCommand = "CREATE TABLE 'tbl.Konachan' (P_ID INTEGER PRIMARY KEY AUTOINCREMENT, ID INTEGER, tags STRING, rating STRING, preview_url STRING, jpeg_url STRING, jpeg_width INTEGER, jpeg_height INTEGER, date_added DATETIME, favorite INTEGER)";

                    //issue command to make the table
                    SQLiteCommand command = new SQLiteCommand(sqlCommand, m_dbConection);

                    command.ExecuteNonQuery();
                    m_dbConection.Close();
                    _wsLib.LogWrite("i", "db", "Table 'tbl.Konachan' was added to WallSwitch.db");
                }
                catch
                {

                }
            }
        }

        private void MakeTableWallhaven()
        {
            //build the connector
            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    //build the command
                    string sqlCommand = "CREATE TABLE 'tbl.Wallhaven' (P_ID INTEGER PRIMARY KEY AUTOINCREMENT, ID INTEGER, tags STRING, rating STRING, preview_url STRING, jpeg_url STRING, jpeg_width INTEGER, jpeg_height INTEGER, date_added DATETIME, favorite INTEGER)";

                    //issue command to make the table
                    SQLiteCommand command = new SQLiteCommand(sqlCommand, m_dbConection);

                    command.ExecuteNonQuery();
                    m_dbConection.Close();
                    _wsLib.LogWrite("i", "db", "Table 'tbl.Wallhaven' was added to WallSwitch.db");
                }
                catch
                {

                }
            }
        }

        private void MakeTableYandere()
        {
            //build the connector
            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    //build the command
                    string sqlCommand = "CREATE TABLE 'tbl.Yandere' (P_ID INTEGER PRIMARY KEY AUTOINCREMENT, ID INTEGER, tags STRING, rating STRING, preview_url STRING, jpeg_url STRING, jpeg_width INTEGER, jpeg_height INTEGER, date_added DATETIME, favorite INTEGER)";

                    //issue command to make the table
                    SQLiteCommand command = new SQLiteCommand(sqlCommand, m_dbConection);

                    command.ExecuteNonQuery();
                    m_dbConection.Close();
                    _wsLib.LogWrite("i", "db", "Table 'tbl.Yandere' was added to WallSwitch.db");
                }
                catch
                {

                }
            }
        }

        private void MakeTableLocal()
        {
            //build the connector
            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    //build the command
                    string sqlCommand = "CREATE TABLE 'tbl.Local' (P_ID INTEGER PRIMARY KEY AUTOINCREMENT, ID INTEGER, tags STRING, rating STRING, preview_url STRING, jpeg_url STRING, jpeg_width INTEGER, jpeg_height INTEGER, date_added DATETIME, favorite INTEGER)";

                    //issue command to make the table
                    SQLiteCommand command = new SQLiteCommand(sqlCommand, m_dbConection);

                    command.ExecuteNonQuery();
                    m_dbConection.Close();
                    _wsLib.LogWrite("i", "db", "Table 'tbl.Local' was added to WallSwitch.db");
                }
                catch
                {

                }
            }
        }

        private void MakeTableTags()
        {
            //build the connector
            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    //build the command
                    string sqlCommand = "CREATE TABLE 'lu.Tags' (P_ID INTEGER PRIMARY KEY AUTOINCREMENT, tag STRING, site STRING)";

                    //issue command to make the table
                    SQLiteCommand command = new SQLiteCommand(sqlCommand, m_dbConection);

                    command.ExecuteNonQuery();
                    m_dbConection.Close();
                    _wsLib.LogWrite("i", "db", "Table 'lu.Tags' was added to WallSwitch.db");
                }
                catch
                {

                }
            }
        }

        public void OpenConnection()
        {
            //CheckForDB();

            using (SQLiteConnection m_dbConection = new SQLiteConnection("Data Source=" + workingDirectory + "; Version=3;"))
            {
                try
                {
                    m_dbConection.Open();

                    m_dbConection.Close();
                }
                catch
                {
                    //MessageBox.Show("Unable to open file " + SQLiteFilePath + ". Generated Error: " + ex.Message);
                }
            }
        }

        public List<SQLData> db_GetTablesInfo()
        {
            string workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WallSwitch\\WallSwitch.db";

            List<SQLData> t = new List<SQLData>();

            try
            {
                if (MySettings.Default.CoreUseWebFiles == true)
                {
                    List<SQLData> KonachanData = _sqlKC.db_GetKonachanData();
                    List<SQLData> WallhavenData = _sqlWH.db_GetWallHavenData();
                    List<SQLData> YandereData = _sqlYan.db_GetYandereData();

                    t.AddRange(KonachanData);
                    t.AddRange(WallhavenData);
                    t.AddRange(YandereData);

                    KonachanData.Clear();
                    WallhavenData.Clear();
                    YandereData.Clear();
                }

                if (MySettings.Default.CoreUseLocalFiles == true)
                {
                    List<SQLData> LocalData = _sqlLocal.db_GetLocalData();

                    t.AddRange(LocalData);

                    LocalData.Clear();
                }
            }
            catch
            {
                //log
            }
            return t;
        }
    }
}
