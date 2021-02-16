using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroToMySQL
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string connectionString = "server=127.0.0.1;port=3306;uid=root;pwd=root;database=test";
            string select_query = "select * from schema1.table1;";
            string insert_query = "insert into schema1.table1 (id, name, pass) values (6, 'i am number 6', '6');";
            MySqlConnection conn= new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                Console.WriteLine("Opened");
                

                MySqlCommand cmd = new MySqlCommand(select_query, conn);

                //Execute command
                MySqlDataReader queryOp= cmd.ExecuteReader();
                
                while (queryOp.Read())
                {
                    Console.WriteLine(queryOp[0] + " -- " + queryOp[1] + " -- " + queryOp[2]);
                }
                queryOp.Close();

                //Console.WriteLine(queryOp);
                MySqlCommand cmd2 = new MySqlCommand(insert_query, conn);
                cmd2.ExecuteNonQuery();

                //cmd = new MySqlCommand(select_query, conn);
                MySqlDataReader queryOp2 = cmd.ExecuteReader();

                while (queryOp2.Read())
                {
                    Console.WriteLine(queryOp2[0] + " -- " + queryOp2[1] + " -- " + queryOp2[2]);
                }
                queryOp2.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //close connection
                conn.Close();
                Console.WriteLine("Closed");
            }

            Console.ReadKey();

        }
    }
}
