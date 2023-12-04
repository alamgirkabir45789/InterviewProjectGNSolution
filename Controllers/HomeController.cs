using InterviewProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InterviewProject.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace InterviewProject.Controllers
{

    public class JsonRequest
    {
        public string CustomerName { get; set; }
        public string MeetingAgenda { get; set; }
        public DateTime MeetingDate { get; set; }
        public string MeetingTime { get; set; }
        public string AttendsFromClientSide { get; set; }
        public string AttendsFromHostSide { get; set; }
        public string MeetingDiscussion { get; set; }
        public string MeetingDecision { get; set; }
        public string MeetingPlace { get; set; }
    }

    public class ServiceDetails
    {
        public string ServiceName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
    }

    public class Common
    {
        public JsonRequest info { get; set; }
        public List<ServiceDetails> agreement { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        [Route("api/PostMetting")]
        [HttpPost]
        public IActionResult PostMetting([FromBody] Common requestData)
        {

            var connString = _configuration.GetConnectionString("dbConnection");
            string Meeting_Minutes_Master_Tbl_ID = Guid.NewGuid().ToString();
            
            SqlTransaction transaction = null;

            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    con.Open();
                    transaction = con.BeginTransaction("MasterDetailsTransaction");


                    SqlCommand cmd = new SqlCommand($"exec Meeting_Minutes_Master_Save_SP '{Meeting_Minutes_Master_Tbl_ID}','{requestData.info.CustomerName}','{requestData.info.MeetingAgenda}','{Convert.ToDateTime(requestData.info.MeetingDate) }','{requestData.info.MeetingTime}','{requestData.info.AttendsFromClientSide}','{requestData.info.AttendsFromHostSide}','{requestData.info.MeetingDiscussion}','{requestData.info.MeetingDecision}', '{requestData.info.MeetingPlace}'",
                             con, transaction);
                    cmd.ExecuteNonQuery();


                    foreach (var item in requestData.agreement)
                    {
                        if (item.ServiceName[0].ToString() != "")
                        {
                            SqlCommand cmdTwo =
                  new SqlCommand($"exec Meeting_Minutes_Details_Save_SP '{item.ServiceName}','{item.Unit}','{Convert.ToInt16(item.Quantity)}','{Meeting_Minutes_Master_Tbl_ID}' ",
                      con, transaction);
                            cmdTwo.ExecuteNonQuery();

                        }


                    }

                    transaction.Commit();
                    con.Close();

                   
                }
                catch (Exception exception)
                {
                    if (transaction != null)
                        transaction.Rollback();
                    return BadRequest(exception.Message);
                }



            }

            return Json(new { res = true });
        }

        [Route("api/GetAllCorporateCustomer")]
        [HttpGet]
        public JsonResult GetAllCorporateCustomer()
        {
            var corporateCustomer = _context.Corporate_Customer_Tbl.ToList();
            return Json(corporateCustomer);
        }

        [Route("api/GetAllIndividualCustomer")]
        [HttpGet]
        public JsonResult GetAllIndividualCustomer()
        {
            var individualCustomer = _context.Individual_Customer_Tbl.ToList();
            return Json(individualCustomer);
        }

        [Route("api/GetAllProductAndService")]
        [HttpGet]
        public JsonResult GetAllProductAndService()
        {
            var productService = _context.Products_Service_Tbl.ToList();
            return Json(productService);
        }

        [Route("api/GetUnitByService")]
        [HttpGet]
        public JsonResult GetUnitByService(string serviceName)
        {
            var unit = _context.Products_Service_Tbl.FirstOrDefault(x => x.ServiceName == serviceName);
            return Json(unit);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}

