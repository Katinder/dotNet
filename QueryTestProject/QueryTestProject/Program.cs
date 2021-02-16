using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using MySql.Data.MySqlClient;
using System.IO;

namespace QueryTestProject
{
    public class ApiOutput
    {

        public int log_id { get; set; }
        public int c1_pk { get; set; }
        public string c2_name { get; set; }
        public float c3_amount { get; set; }
        public DateTime c4_updated_at { get; set; }
        public string Operation { get; set; }
    }

    class Program
    {
        private static string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=root";
        private static MySqlConnection conn = new MySqlConnection(connectionString);

        static void ShowProduct(dynamic apiOut)
        {
            Console.WriteLine($"Name: {apiOut.c2_name}\namount:{apiOut.c3_amount}" +
                $"\nPk:{apiOut.c1_pk}\nlast updated at: {apiOut.c4_updated_at}\nOperation: {apiOut.Operation}\nKey: {apiOut.log_id}");
        }

        static void Main(string[] args)
        {
            string last_local_update_str = "2021-01-28 13:00:00";

            //DateTime last_local_time = Convert.ToDateTime(last_local_update_str);  //yy,mm,dd,hh,mm,ss

            
            string main_query2 = "select * from (select c1_pk, max(c4_updated_at) as last_update from schema1.log_table " +
                "group by c1_pk) as x inner join schema1.log_table as l on l.c1_pk=x.c1_pk and l.c4_updated_at=x.last_update;";

            string main_query = $"select * from (select c1_pk, max(c4_updated_at) as last_update " +
                $"from schema1.log_table group by c1_pk) as x " +
                $"inner join schema1.log_table as l on l.c1_pk=x.c1_pk and l.c4_updated_at=x.last_update " +
                $"where l.log_id in (select log_id from schema1.log_table where c4_updated_at>= '{last_local_update_str}');";

            


            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(main_query, conn);
                List<ApiOutput> outputList = new List<ApiOutput>();

                try
                {
                    MySqlDataReader queryOp = cmd.ExecuteReader();

                    while (queryOp.Read())
                    {
                        Console.WriteLine(queryOp[0] + " -- " + queryOp[1] + " -- " + queryOp[2]+" -- " + queryOp[3]+" -- " + queryOp[4]+" -- " + queryOp[5] + " -- " + queryOp[6] + " -- " + queryOp[7]);
                        //Console.WriteLine(queryOp.ToString());

                        ApiOutput newRecord = new ApiOutput();
                        newRecord.log_id = (int)queryOp["log_id"];
                        newRecord.c1_pk = (int)queryOp["c1_pk"];
                        newRecord.c2_name = (string)queryOp["c2_name"];
                        newRecord.c3_amount = (int)queryOp["c3_amount"];
                        newRecord.c4_updated_at = (DateTime)queryOp["c4_updated_at"];
                        newRecord.Operation = (string)queryOp["operation"];
                        //newRecord. = (int)queryOp["log_id"];

                        outputList.Add(newRecord);
                        //Console.WriteLine("-----------------------------------------");
                        //ShowProduct(newRecord);
                        //Console.WriteLine("-----------------------------------------");

                        /*
                        if (queryOp[7].ToString() == "I")
                        {
                            InsertToDb(newRecord);
                        }

                        else if (queryOp[7].ToString() == "U")
                        {
                            //check if item already in local db or is it an update after insert
                            var flag = CheckItem(newRecord);
                            if (flag)
                            {
                                UpdateDb(newRecord);
                            }
                            else //insert item
                            {
                                InsertToDb(newRecord);
                            }
                        }

                        else if (queryOp[7].ToString() == "D")
                        {
                            DeleteFromDb(newRecord);
                        }

                        Console.WriteLine("-----------------------------------------");*/
                       
                    }

                    queryOp.Close();

                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }

            CloseConnection();

            /*//SEARCH FOR POSITION
            List<int> pks_covered = new List<int>();
            foreach (var item in items)
            {
                if (Convert.ToDateTime(item.c4_updated_at) > last_local_time)
                {
                    //add to to-do
                    if (!pks_covered.Contains(item.c1_pk))
                    {
                        pks_covered.Add(item.c1_pk);
                        ShowProduct(item);

                        if (item.Operation == "I")
                        {
                            InsertToDb(item);
                        }

                        else if (item.Operation == "U")
                        {
                            //check if item already in local db or is it an update after insert
                            var flag = CheckItem(item);
                            if (flag)
                            {
                                UpdateDb(item);
                            }
                            else //insert item
                            {
                                InsertToDb(item);
                            }
                        }

                        else if (item.Operation == "D")
                        {
                            DeleteFromDb(item);
                        }

                        Console.WriteLine("-----------------------------------------");
                    }
                    else
                    {
                        continue;
                    }

                }

                else
                {
                    break;
                }

            } */

            Console.ReadKey();

        }

        private static bool CheckItem(ApiOutput item)
        {
            string check_query = $"SELECT COUNT(*) FROM schema1.local_table WHERE `pk`='{item.c1_pk}';";

            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(check_query, conn);
                try
                {
                    int queryOp = int.Parse(cmd.ExecuteScalar().ToString());
                    if (queryOp == 0)
                    {
                        Console.WriteLine("Row doesn't exist.");
                        CloseConnection();
                        return false;
                    }
                    else //already exists
                    {
                        Console.WriteLine("Row exists.");
                        CloseConnection();
                        return true;
                    }


                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

            else
                return false;
        }

        private static bool CloseConnection()
        {
            try
            {
                conn.Close();
                Console.WriteLine("Connection Closed.");
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return false;
            }
        }

        private static bool OpenConnection()
        {
            try
            {
                conn.Open();
                Console.WriteLine("Connection Opened.");
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return false;
            }
        }

        private static void DeleteFromDb(ApiOutput item)
        {
            string delete_query = $"DELETE FROM schema1.local_table WHERE `pk`='{item.c1_pk}';";

            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(delete_query, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Row Deleted.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                CloseConnection();
            }
        }

        private static void UpdateDb(ApiOutput item)
        {
            string update_query = $"UPDATE schema1.local_table SET `name`='{item.c2_name}', `amount`='{item.c3_amount}' WHERE `pk`= '{item.c1_pk}';";

            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(update_query, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Row Updated.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                CloseConnection();
            }
        }

        private static void InsertToDb(ApiOutput item)
        {

            string insert_query = $"INSERT INTO schema1.local_table (`pk`, `name`, `amount`) VALUES ('{item.c1_pk}','{item.c2_name}','{item.c3_amount}');"; //'2021-01-28 12:59:52'


            if (OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(insert_query, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("New Row Inserted.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                CloseConnection();
            }
        }
    }
}






