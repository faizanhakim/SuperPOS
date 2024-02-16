using Project.Models;
using MessagePack;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.Connections;

namespace Project.Services
{   
    public class UserAuthentication
    {
        string connectionString = @"Data Source=DESKTOP-B2IDPUK\HALANISQL;Initial Catalog=""SuperMarket System"";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection;
        
        public UserAuthentication()
        {
            connection = new SqlConnection(connectionString);
        }

        public bool VerifyUser(UserModel user)
        {
            string query = "Select Id, UserName, Password, Employee_id From Users where UserName = @user and Password = @pass";
            
            SqlCommand command = new SqlCommand(query,connection);
            command.Parameters.Add("@user", System.Data.SqlDbType.VarChar,50).Value = user.UserName;
            command.Parameters.Add("@pass", System.Data.SqlDbType.VarChar,50).Value = user.Password;
            

            try
            {
                connection.Open();
                SqlDataReader dataReader = command.ExecuteReader();
           
                if(dataReader.HasRows)
                {
                    dataReader.Read();
                    user.Id = dataReader.GetInt32(0);
                   // user.Employee_Id = dataReader.GetInt32(3);
                
                    dataReader.Close();
                    connection.Close();

                    return true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return false;
        }

        public bool MakeUser(UserModel user)
        {
            string query = "INSERT INTO USERS (UserName, Password) VALUES(@user, @pass)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.Add("@user", System.Data.SqlDbType.VarChar,50).Value = user.UserName;
            command.Parameters.Add("@pass", System.Data.SqlDbType.VarChar, 50).Value = user.Password;
            //command.Parameters.Add("@e", System.Data.SqlDbType.Int).Value = user.Employee_Id;

            connection.Open();
            try
            {
                command.ExecuteScalar();
                connection.Close();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            connection.Close();

            return false;
        }
         


    }
}
