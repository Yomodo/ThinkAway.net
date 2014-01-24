using System;
using System.Collections.Generic;
using ThinkAway.Data;
using ThinkAway.Data.SQLite;

namespace ThinkAway.Plus.Modem
{
    class DBQueue : ISMSQueue
    {
        private readonly IDbHelper _dbHelper; 

        private const string SMS_SEND = "SMS_SEND";

        internal DBQueue()
        {
            _dbHelper = new SQLite("rtx.db",false,false);
        }

        /// <summary>
        /// 创建表结构 如果不存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool CreateTable(string tableName)
        {
            string sql = string.Format("select count(*) from sqlite_master  where type='table' and name='{0}';",tableName);
            int result = _dbHelper.ExecSql(sql);
            if (result == 0)
            {
                sql = string.Format("CREATE TABLE {0}([SID] INTEGER PRIMARY KEY NOT NULL,[COM] VARCHAR(20), [PHONE] VARCHAR(20),[CONTENT] VARCHAR(500),[DATETIME] LONG,[STATE] INTEGER);",tableName);
                result = _dbHelper.ExecSql(sql);
            }
            return result > 0;
        }


        #region ISMSQueue 成员

        public bool CheckQueue(string com)
        {
            string sql = String.Format("select count([sid]) from {0} where [com] = '{1}' and [dateTime] <= {2} and [state] = {3}",
               SMS_SEND, com, DateTime.Now.Ticks, 0);
            return _dbHelper.ExecSql(sql) > 0;
        }

        public long Add(SMSSendInfo smsInfo)
        {
            const int defaultNum = 0;
            DataValues contentValues = new DataValues();
            contentValues.Add("SID",null);
            contentValues.Add("COM",smsInfo.Com);
            contentValues.Add("PHONE",smsInfo.Phone);
            contentValues.Add("CONTENT",smsInfo.Message);
            contentValues.Add("DATETIME",smsInfo.DateTime);
            contentValues.Add("STATE",defaultNum);
            int result = _dbHelper.Insert(SMS_SEND, contentValues);
            return result;
        }

        public void Set(SMSSendInfo sendInfo, int state)
        {
            DataValues contentValues = new DataValues();
            contentValues.Add("state", state);
            DataValues whereArgs = new DataValues();
            whereArgs.Add("SID", sendInfo.Id);
            _dbHelper.Update(SMS_SEND, contentValues, "SID = ?", whereArgs);
        }

        public List<SMSSendInfo> Get(string com)
        {
            List<SMSSendInfo> list = new List<SMSSendInfo>();
            DataValues whereArgs = new DataValues();
            whereArgs.Add("com",com);
            whereArgs.Add("dateTime",DateTime.Now.Ticks);
            whereArgs.Add("state", 0);
            using (Cursor cursor = _dbHelper.Query(SMS_SEND, null, "com = ?,dateTime < ?,state = ?", null))
            {
                while (cursor.Next())
                {
                    int sid = cursor.GetInt("sid");
                    string phone = cursor.GetString("phone");
                    string content = cursor.GetString("content");
                    //int state = Convert.ToInt32(row["state"]);
                    //model
                    SMSSendInfo sendInfo = new SMSSendInfo
                    {
                        Id = sid,
                        Com = com,
                        Phone = phone,
                        Message = content,
                        DateTime = DateTime.Now.Ticks,
                    };
                    list.Add(sendInfo);
                    Set(sendInfo, 1);
                }
            }
            return list;
        }

        #endregion
    }
}
