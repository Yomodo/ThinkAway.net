using System.Transactions;
using NUnit.Framework;
using ThinkAway.Data;
using ThinkAway.Data.MSSQL;
using ThinkAway.IO.Log;

namespace ThinkAway.Test
{
    class DbTest
    {
        private readonly ILog _logger;

        [Test]
        public void MsSqlTest()
        {
            IDbHelper dbHelper = new MsSQL("test");
            //dbHelper.CreateTable("TEST", tableValues);

            for (int i = 0; i < 10; i++)
            {
                DataValues contentValues = new DataValues();
                contentValues.Add("Name", "lsong");
                contentValues.Add("Password", "123456");
                int insert = dbHelper.Insert("myTable", contentValues);
                Assertion.Assert(insert <= 0);
            }

            using (TransactionScope transactionScope = new TransactionScope())
            {
                DataValues contentValues = new DataValues();
                dbHelper.Update("myTable", contentValues, null, null);

                transactionScope.Complete();
            }
            using (var cursor = dbHelper.Query("myTable", null, null, null))
            {
                while (cursor.Next())
                {
                    System.Console.Write(cursor[0] + "\t");
                    System.Console.Write(cursor[1] + "\t");
                    System.Console.Write(cursor[2] + "\t");
                    System.Console.WriteLine();
                }
            }
        }
    }
}
