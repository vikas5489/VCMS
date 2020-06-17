using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VCMS.Services.Models;

namespace VCMS.Services.Controllers
{
    
    public class LoginController : ApiController
    {
        [HttpGet]
       // [Route("myapi/{login}")]         
        public string Get()
        {
            return "Welcome to Web Api";
        }


        static string con1 = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        [HttpPost]
        [Route("api/Login/AddCustomer")]
        public IHttpActionResult AddCustomer([FromBody]customer cs)

        {

            try

            {

                if (!ModelState.IsValid)

                {

                    return BadRequest(ModelState);

                }

                 Add_Customer(cs);

                return Ok("Success");

            }

            catch (Exception ex)
            {
                return Ok("Something went wrong, try later:: "+ ex );
                //return Ok("Something went wrong, try later");

            }

        }
        [HttpPost]
        [Route("api/Login/LoginCustomer")]
        public IHttpActionResult LoginCustomer([FromBody]customer cs)

        {

            try

            {

                if (!ModelState.IsValid)

                {

                    return BadRequest(ModelState);

                }

              bool result=  Login_Customer(cs);
                if (result)
                {
                    return Ok("Success");
                }
                else { return Ok("Incorrect User Name or Password"); }

            }

            catch (Exception ex)
            {
                return Ok("Something went wrong, try later:: " + ex);
                //return Ok("Something went wrong, try later");

            }

        }

        [Route("api/Login/GetAllCustomer")]
        [HttpGet]
        public IHttpActionResult GetCustomer()
        {
            try
            {

                if (!ModelState.IsValid)

                {

                    return BadRequest(ModelState);

                }


                return Ok(GetCustomer("0"));
                 
                

            }

            catch (Exception ex)
            {
                return Ok("Something went wrong, try later:: " + ex);
                 

            }

        }

        [Route("api/Login/GetCustomerById")]
        [HttpPost]
        public IHttpActionResult GetCustomer([FromBody]customer cs)
        {
            try
            {

                if (!ModelState.IsValid)

                {

                    return BadRequest(ModelState);

                }


                return Ok(GetCustomer(cs.Custimer_id.ToString()));



            }

            catch (Exception ex)
            {
                return Ok("Something went wrong, try later:: " + ex);


            }

        }

        private void Add_Customer(customer cs)

        {
            try
            {
                
                SqlConnection con = new SqlConnection(con1);
                SqlCommand com = new SqlCommand("Sp_Customer_Add", con);

                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@FullName", cs.Fullname);

                com.Parameters.AddWithValue("@Email", cs.Email);

                com.Parameters.AddWithValue("@Password", cs.Password);

                com.Parameters.AddWithValue("@Mobile_no", cs.Mobileno);

                //com.Parameters.AddWithValue("@Creatd_At", cs.Createdat);

                con.Open();

                com.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                throw;
            }



        }
        private bool Login_Customer(customer cs)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter();
            try
            {
                SqlConnection con = new SqlConnection(con1);
                SqlCommand cmd = new SqlCommand("LoginByUsernamePassword", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", cs.Custimer_id);
                cmd.Parameters.AddWithValue("@password", cs.Password);
                adp.SelectCommand = cmd;
                adp.Fill(dt);
                cmd.Dispose();
                if (dt.Rows.Count > 0)
                {

                    //Or in show messagebox using  ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", "alert('Login Successfull');", true);
                    //Or write using Response.Write("Login Successfull");

                    return true;
                    //Or redirect using Response.Redirect("Mypanel.aspx");
                }
                else
                {
                    return false;
                    //Or show in messagebox usingScriptManager.RegisterStartupScript(this, this.GetType(), "Message", "alert('Wrong Username/Password');", true);
                    //Or write using Response.Write("Wrong Username/Password");
                }
            }
            catch (Exception ex)
            {
                return false;
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", "alert('Oops!! following error occured : " + ex.Message.ToString() + "');", true);
                // Response.Write("Oops!! following error occured: " +ex.Message.ToString());           
            }
            finally
            {
                dt.Clear();
                dt.Dispose();
                adp.Dispose();
            }
        }

        // GET: User
        private List<customer> GetCustomer(string customerid)
        {            
            DataSet ds = new DataSet();
            List<customer> custlist = new List<customer>();
            using (SqlConnection con = new SqlConnection(con1))
            {
                using (SqlCommand cmd = new SqlCommand("GetCustomerListDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", customerid);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        customer custobj = new customer();
                        custobj.Custimer_id = Convert.ToInt32(ds.Tables[0].Rows[i]["Customer_ID"].ToString());
                        custobj.Fullname = ds.Tables[0].Rows[i]["FullName"].ToString();
                        custobj.Email = ds.Tables[0].Rows[i]["Email"].ToString();
                        custobj.Mobileno = ds.Tables[0].Rows[i]["Mobile_no"].ToString();
                        custobj.Createdat = ds.Tables[0].Rows[i]["Created_At"].ToString();
                        custlist.Add(custobj);
                    }
                     
                }
                con.Close();
            }
            return custlist;
        }

    }
}
