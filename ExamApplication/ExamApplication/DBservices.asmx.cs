using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Configuration;

namespace ExamApplication
{
    /// <summary>
    /// Summary description for DBservices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DBservices : System.Web.Services.WebService
    {

        [WebMethod]
        public void GetBooksByCity()
        {
            //Input connectionString i web.config
            string connection = ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString;
            List<Book> books = new List<Book>();
            using (SqlConnection con = new SqlConnection(connection))
            {
                //Input SQL Commands
                SqlCommand cmd = new SqlCommand("SQL Command", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    Book book = new Book();
                    book.NameOrId = reader["Id"].ToString();
                    book.Title = reader["Name"].ToString();
                    book.Author = reader["Author"].ToString();
                    books.Add(book);
                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(books));
        }

        [WebMethod]
        public void GetCitiesByBook()
        {
            string connection = ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString;
            List<City> cities = new List<City>();
            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand("SQL Command", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    City city = new City();
                    city.AsciiName = reader["Name"].ToString();
                    city.Latitude = Convert.ToInt32(reader["Latitude"]);
                    city.Longitude = Convert.ToInt32(reader["Longitude"]);
                    cities.Add(city);
                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(cities));
        }

        [WebMethod]
        public void GetBooksAndCitiesByAuthor()
        {
            string connection = ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString;
            List<Book> books = new List<Book>();
            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand("SQL Command", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Book book = new Book();
                    book.NameOrId = reader["Id"].ToString();
                    book.Title = reader["Name"].ToString();
                    book.Author = reader["Author"].ToString();
                    books.Add(book);
                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(books));
        }

        [WebMethod]
        public void GetCitiesByLocation()
        {
            string connection = ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString;
            List<City> cities = new List<City>();
            using (SqlConnection con = new SqlConnection(connection))
            {
                SqlCommand cmd = new SqlCommand("SQL Command", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    City city = new City();
                    city.AsciiName = reader["Name"].ToString();
                    city.Latitude = Convert.ToInt32(reader["Latitude"]);
                    city.Longitude = Convert.ToInt32(reader["Longitude"]);
                    cities.Add(city);
                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(cities));
        }
    }
}
