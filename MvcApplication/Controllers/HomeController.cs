using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using MvcApplication.Models;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;


namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        static List<interviewmodel> interviewList;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase FileUpload)
        {
            DataTable dt = new DataTable();

            if (FileUpload != null && FileUpload.ContentLength > 0)
            {

                string fileName = Path.GetFileName(FileUpload.FileName);
                string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);


                try
                {
                    FileUpload.SaveAs(path);

                    dt = ImportCSV(path);
                    ViewData["Feedback"] = ProcessBulkCopy(dt);
                    interviewList = GetData("select * from tbl_interview");
                }
                catch (Exception ex)
                {

                    ViewData["Feedback"] = ex.Message;
                }
            }
            else
            {

                ViewData["Feedback"] = "Please select a file";
            }

            dt.Dispose();
            return View();
        }

        private DataTable ImportCSV(string fileName)
        {
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();
            DataRow row;

            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            StreamReader sr = new StreamReader(fileName);

            line = sr.ReadLine();
            strArray = r.Split(line);
            Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));

            while ((line = sr.ReadLine()) != null)
            {
                strArray = r.Split(line);

                row = dt.NewRow();
                row.ItemArray = strArray;
                dt.Rows.Add(row);
            }

            sr.Dispose();
            sr.Close();

            return dt;
        }

        private static string ProcessBulkCopy(DataTable dt)
        {
            string Feedback = string.Empty;
            string connString = ConfigurationManager.ConnectionStrings["mvcapp_connection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (var copy = new SqlBulkCopy(conn))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"DELETE FROM tbl_interview;", conn);
                    cmd.ExecuteNonQuery();

                    copy.DestinationTableName = "tbl_interview";
                    copy.BatchSize = dt.Rows.Count;
                    try
                    {
                        copy.WriteToServer(dt);
                        Feedback = "Upload complete";
                    }
                    catch (Exception ex)
                    {
                        Feedback = ex.Message;
                    }

                    conn.Close();
                }
            }

            return Feedback;
        }

        private List<interviewmodel> SortByColumnWithOrder(string order, string orderDir, List<interviewmodel> data)
        {
            List<interviewmodel> lst = new List<interviewmodel>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Gender).ToList() : data.OrderBy(p => p.Gender).ToList();
                        break;
                    case "1":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Title).ToList() : data.OrderBy(p => p.Title).ToList();
                        break;
                    case "2":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Occupation).ToList() : data.OrderBy(p => p.Occupation).ToList();
                        break;
                    case "3":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Company).ToList() : data.OrderBy(p => p.Company).ToList();
                        break;
                    case "4":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.GivenName).ToList() : data.OrderBy(p => p.GivenName).ToList();
                        break;
                    case "5":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.MiddleInitial).ToList() : data.OrderBy(p => p.MiddleInitial).ToList();
                        break;
                    case "6":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Surname).ToList() : data.OrderBy(p => p.Surname).ToList();
                        break;
                    case "7":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.BloodType).ToList() : data.OrderBy(p => p.BloodType).ToList();
                        break;
                    case "8":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.EmailAddress).ToList() : data.OrderBy(p => p.EmailAddress).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return lst;
        }

        [HttpPost]
        public ActionResult LoadDataTable(string colOrder)
        {
            JsonResult result = new JsonResult();
            try
            {
                // Initialization.   
                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                string col0val = Convert.ToString(Request["columns[0][search][value]"]);
                string col1val = Convert.ToString(Request["columns[1][search][value]"]);
                string col2val = Convert.ToString(Request["columns[2][search][value]"]);
                string col3val = Convert.ToString(Request["columns[3][search][value]"]);
                string col4val = Convert.ToString(Request["columns[4][search][value]"]);
                string col5val = Convert.ToString(Request["columns[5][search][value]"]);
                string col6val = Convert.ToString(Request["columns[6][search][value]"]);
                string col7val = Convert.ToString(Request["columns[7][search][value]"]);
                string col8val = Convert.ToString(Request["columns[8][search][value]"]);

                string[] colOrderArr = colOrder.Split(',');
                //Debug.WriteLine(colOrder);

                // Loading.   
                List<interviewmodel> data;
                if (interviewList != null)
                    data = interviewList;
                else
                    data = GetData("select * from tbl_interview");

                // Total record count.   
                int totalRecords = data.Count;

                // Verification.   
                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    // use OR operation between column
                    data = data.Where(p => p.Gender.ToLower().Equals(search.ToLower()) ||
                        p.Title.ToLower().Equals(search.ToLower()) ||
                        p.Occupation.ToLower().Equals(search.ToLower()) ||
                        p.Company.ToLower().Equals(search.ToLower()) ||
                        p.GivenName.ToLower().Contains(search.ToLower()) ||
                        p.MiddleInitial.ToLower().Contains(search.ToLower()) ||
                        p.Surname.ToLower().Contains(search.ToLower()) ||
                        p.BloodType.ToLower().Equals(search.ToLower()) ||
                        p.EmailAddress.ToLower().Contains(search.ToLower())).ToList();
                }
                else // use AND operation between column
                {
                    if (!string.IsNullOrEmpty(col0val) && !string.IsNullOrWhiteSpace(col0val))
                    {
                        data = filter("Gender", col0val, data);
                    }
                    if (!string.IsNullOrEmpty(col1val) && !string.IsNullOrWhiteSpace(col1val))
                    {
                        data = filter("Title", col1val, data);
                    }
                    if (!string.IsNullOrEmpty(col2val) && !string.IsNullOrWhiteSpace(col2val))
                    {
                        data = filter("Occupation", col2val, data);
                    }
                    if (!string.IsNullOrEmpty(col3val) && !string.IsNullOrWhiteSpace(col3val))
                    {
                        data = filter("Company", col3val, data);
                    }
                    if (!string.IsNullOrEmpty(col4val) && !string.IsNullOrWhiteSpace(col4val))
                    {
                        data = filter("GivenName", col4val, data);
                    }
                    if (!string.IsNullOrEmpty(col5val) && !string.IsNullOrWhiteSpace(col5val))
                    {
                        data = filter("MiddleInitial", col5val, data);
                    }
                    if (!string.IsNullOrEmpty(col6val) && !string.IsNullOrWhiteSpace(col6val))
                    {
                        data = filter("Surname", col6val, data);
                    }
                    if (!string.IsNullOrEmpty(col7val) && !string.IsNullOrWhiteSpace(col7val))
                    {
                        data = filter("BloodType", col7val, data);
                    }
                    if (!string.IsNullOrEmpty(col8val) && !string.IsNullOrWhiteSpace(col8val))
                    {
                        data = filter("EmailAddress", col8val, data);
                    }
                }

                // Sorting.   
                if (!(string.IsNullOrEmpty(order) && string.IsNullOrEmpty(orderDir)))
                {
                    string index = Array.IndexOf(colOrderArr, order).ToString();
                    data = SortByColumnWithOrder(index, orderDir, data);
                }
                
                // Filter record count.   
                int recFilter = data.Count;

                // Apply pagination.   
                if (pageSize == -1)
                    data = data.Skip(startRec).Take(recFilter).ToList();
                else
                    data = data.Skip(startRec).Take(pageSize).ToList();

                // Loading drop down lists.   
                result = Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return result;
        }

        public List<interviewmodel> filter(string colName, string colVal, List<interviewmodel> data)
        {
            switch (colName)
            {
                case "Gender":
                    data = data.Where(p => p.Gender.ToLower().Equals(colVal.ToLower())).ToList();
                    break;
                case "Title":
                    data = data.Where(p => p.Title.ToLower().Equals(colVal.ToLower())).ToList();
                    break;
                case "Occupation":
                    data = data.Where(p => p.Occupation.ToLower().Equals(colVal.ToLower())).ToList();
                    break;
                case "Company":
                    data = data.Where(p => p.Company.ToLower().Equals(colVal.ToLower())).ToList();
                    break;
                case "GivenName":
                    data = data.Where(p => p.GivenName.ToLower().Contains(colVal.ToLower())).ToList();
                    break;
                case "MiddleInitial":
                    data = data.Where(p => p.MiddleInitial.ToLower().Contains(colVal.ToLower())).ToList();
                    break;
                case "Surname":
                    data = data.Where(p => p.Surname.ToLower().Contains(colVal.ToLower())).ToList();
                    break;
                case "BloodType":
                    data = data.Where(p => p.BloodType.ToLower().Equals(colVal.ToLower())).ToList();
                    break;
                case "EmailAddress":
                    data = data.Where(p => p.EmailAddress.ToLower().Contains(colVal.ToLower())).ToList();
                    break;
            }

            return data;
        }

        public List<interviewmodel> GetData(string query)
        {
            interviewList = new List<interviewmodel>();
            string strConnString = ConfigurationManager.ConnectionStrings["mvcapp_connection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(strConnString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        
                        interviewmodel inv;
                        
                        while (r.Read())
                        {
                            inv = new interviewmodel();
                            inv.Gender = r.GetString(0); 
                            inv.Title = r.GetString(1);
                            inv.Occupation = r.GetString(2);
                            inv.Company = r.GetString(3);
                            inv.GivenName = r.GetString(4);
                            inv.MiddleInitial = r.GetString(5);
                            inv.Surname = r.GetString(6);
                            inv.BloodType = r.GetString(7);
                            inv.EmailAddress = r.GetString(8);
                            interviewList.Add(inv);
                        }
                        
                    }

                }
            }

            return interviewList;
        }

        public DataTable proccessFilter(string filter)
        {
            dynamic item = JObject.Parse(filter);

            string GlobalSearch = item.GlobalSearch.Value;
            string Gender = item.Gender.Value;
            string Title = item.Title.Value;
            string Occupation = item.Occupation.Value;
            string Company = item.Company.Value;
            string GivenName = item.GivenName.Value;
            string MiddleInitial = item.MiddleInitial.Value;
            string Surname = item.Surname.Value;
            string BloodType = item.BloodType.Value;
            string EmailAddress = item.EmailAddress.Value;
            string ColumnOrder = item.ColumnOrder.Value;
            string[] colOrderArr = ColumnOrder.Split(',');
            string Order = item.SortInfo[0][0];
            string Direction = item.SortInfo[0][1];

            // handling filter
            string sqlWhere = "";
            if (!string.IsNullOrEmpty(GlobalSearch) && !string.IsNullOrWhiteSpace(GlobalSearch))
            {
                // use OR operation between column
                GlobalSearch = GlobalSearch.ToLower();
                sqlWhere = " WHERE     LOWER(Gender) = '" + GlobalSearch + "' or " +
                            "           LOWER(Title) = '" + GlobalSearch + "' or " +
                            "           LOWER(Occupation) = '" + GlobalSearch + "' or " +
                            "           LOWER(Company) = '" + GlobalSearch + "' or " +
                            "           LOWER(GivenName) LIKE '%" + GlobalSearch + "%' or " +
                            "           LOWER(MiddleInitial) LIKE '%" + GlobalSearch + "%' or " +
                            "           LOWER(Surname) LIKE '%" + GlobalSearch + "%' or " +
                            "           LOWER(BloodType) = '" + GlobalSearch + "' or " +
                            "           LOWER(EmailAddress) LIKE '%" + GlobalSearch + "%'  ";
            }
            else // use AND operation between column
            {
                if (!string.IsNullOrEmpty(Gender) && !string.IsNullOrWhiteSpace(Gender))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(Gender) = '" + Gender.ToLower() + "' ";
                }
                if (!string.IsNullOrEmpty(Title) && !string.IsNullOrWhiteSpace(Title))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(Title) = '" + Title.ToLower() + "' ";
                }
                if (!string.IsNullOrEmpty(Occupation) && !string.IsNullOrWhiteSpace(Occupation))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(Occupation) = '" + Occupation.ToLower() + "' ";
                }
                if (!string.IsNullOrEmpty(Company) && !string.IsNullOrWhiteSpace(Company))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(Company) = '" + Company.ToLower() + "' ";
                }
                if (!string.IsNullOrEmpty(GivenName) && !string.IsNullOrWhiteSpace(GivenName))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(GivenName) LIKE '%" + GivenName.ToLower() + "%' ";
                }
                if (!string.IsNullOrEmpty(MiddleInitial) && !string.IsNullOrWhiteSpace(MiddleInitial))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(MiddleInitial) LIKE '%" + MiddleInitial.ToLower() + "%' ";
                }
                if (!string.IsNullOrEmpty(Surname) && !string.IsNullOrWhiteSpace(Surname))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(Surname) LIKE '%" + Surname.ToLower() + "%' ";
                }
                if (!string.IsNullOrEmpty(BloodType) && !string.IsNullOrWhiteSpace(BloodType))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(BloodType) = '" + BloodType.ToLower() + "' ";
                }
                if (!string.IsNullOrEmpty(EmailAddress) && !string.IsNullOrWhiteSpace(EmailAddress))
                {
                    if (!sqlWhere.Equals("")) sqlWhere += " AND ";
                    sqlWhere += "LOWER(EmailAddress) LIKE '%" + EmailAddress.ToLower() + "%' ";
                }

                if (!sqlWhere.Equals("")) sqlWhere = " WHERE " + sqlWhere;
            }

            // handling sorting
            int index = Array.IndexOf(colOrderArr, Order);
            string colname = getColName(index);
            string sqlOrder = " ORDER BY " + colname + " " + Direction + " ";

            //handling column re-ordering
            int[] newOrderArr = new int[colOrderArr.Length];
            string columns = "";

            for (int i = 0; i < colOrderArr.Length; i++)
            {
                index = Array.IndexOf(colOrderArr, i.ToString());
                newOrderArr[i] = index;
            }

            for (int i = 0; i < newOrderArr.Length; i++)
            {
                colname = getColName(newOrderArr[i]);
                columns += colname + ",";
            }
            columns = columns.Remove(columns.Length - 1);

            //Get the data from database into datatable
            string strQuery = "select " + columns + " from tbl_interview " + sqlWhere + sqlOrder;

            Debug.WriteLine("strQuery:" + strQuery);

            SqlCommand cmd = new SqlCommand(strQuery);
            DataTable dt = GetData(cmd);

            return dt;
        }

        public void DownloadCSV(string filter)
        {
            DataTable dt = proccessFilter(filter);

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=DataTable.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";

            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                //add separator
                sb.Append(dt.Columns[k].ColumnName + ',');
            }
            //append new line
            sb.Append("\r\n");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    //add separator
                    sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                }
                //append new line
                sb.Append("\r\n");
            }
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        private string getColName(int idx)
        {
            string colname = "";
            switch (idx)
            {
                case 1:
                    colname = "Title";
                    break;
                case 2:
                    colname = "Occupation";
                    break;
                case 3:
                    colname = "Company";
                    break;
                case 4:
                    colname = "GivenName";
                    break;
                case 5:
                    colname = "MiddleInitial";
                    break;
                case 6:
                    colname = "Surname";
                    break;
                case 7:
                    colname = "BloodType";
                    break;
                case 8:
                    colname = "EmailAddress";
                    break;

                default:
                    colname = "Gender";
                    break;
            }
            return colname;
        }

        private DataTable GetData(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            string strConnString = ConfigurationManager.ConnectionStrings["mvcapp_connection"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            SqlDataAdapter sda = new SqlDataAdapter();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            try
            {
                con.Open();
                sda.SelectCommand = cmd;
                sda.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                sda.Dispose();
                con.Dispose();
            }
        }

        [HttpPost]
        public JsonResult AddInterview(interviewmodel inv)
        {
            string status = "";
            bool success = false;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["mvcapp_connection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO TBL_INTERVIEW(Gender,Title,Occupation,Company,GivenName," +
                                              "MiddleInitial,Surname,BloodType,EmailAddress) " +
                                              "VALUES( @Gender,@Title,@Occupation,@Company,@GivenName, " +
                                              "@MiddleInitial,@Surname,@BloodType,@EmailAddress)";

                        command.Parameters.AddWithValue("@Gender", inv.Gender);
                        command.Parameters.AddWithValue("@Title", inv.Title);
                        command.Parameters.AddWithValue("@Occupation", inv.Occupation);
                        command.Parameters.AddWithValue("@Company", inv.Company);
                        command.Parameters.AddWithValue("@GivenName", inv.GivenName);
                        command.Parameters.AddWithValue("@MiddleInitial", inv.MiddleInitial);
                        command.Parameters.AddWithValue("@Surname", inv.Surname);
                        command.Parameters.AddWithValue("@BloodType", inv.BloodType);
                        command.Parameters.AddWithValue("@EmailAddress", inv.EmailAddress);

                        command.ExecuteNonQuery();

                        if (interviewList == null) interviewList = new List<interviewmodel>();
                        interviewList.Add(inv);

                        status = "Save success";
                        success = true;
                    }
                }

            }
            catch (SqlException ex)
            {
                status = "Save failed. " + ex.Message;
                success = false;
            }
            catch (Exception ex)
            {
                status = "Save failed. " + ex.Message;
                success = false;
            }

            return new JsonResult { Data = new { status = status, success = success } };
        }

        public ActionResult PrintPreview(string filter)
        {
            dynamic item = JObject.Parse(filter);

            string GlobalSearch = item.GlobalSearch.Value;
            string Gender = item.Gender.Value;
            string Title = item.Title.Value;
            string Occupation = item.Occupation.Value;
            string Company = item.Company.Value;
            string GivenName = item.GivenName.Value;
            string MiddleInitial = item.MiddleInitial.Value;
            string Surname = item.Surname.Value;
            string BloodType = item.BloodType.Value;
            string EmailAddress = item.EmailAddress.Value;

            string activefilter = "";
            if (!string.IsNullOrEmpty(GlobalSearch) && !string.IsNullOrWhiteSpace(GlobalSearch))
            {
                activefilter = "Active filter = Global filter : " + GlobalSearch;
            }
            else // use AND operation between column
            {
                if (!string.IsNullOrEmpty(Gender) && !string.IsNullOrWhiteSpace(Gender))
                {
                    activefilter += "Gender : " + Gender + " | ";
                }
                if (!string.IsNullOrEmpty(Title) && !string.IsNullOrWhiteSpace(Title))
                {
                    activefilter += "Title : " + Title + " | ";
                }
                if (!string.IsNullOrEmpty(Occupation) && !string.IsNullOrWhiteSpace(Occupation))
                {
                    activefilter += "Occupation : " + Occupation + " | ";
                }
                if (!string.IsNullOrEmpty(Company) && !string.IsNullOrWhiteSpace(Company))
                {
                    activefilter += "Company : " + Company + " | ";
                }
                if (!string.IsNullOrEmpty(GivenName) && !string.IsNullOrWhiteSpace(GivenName))
                {
                    activefilter += "Given Name : " + GivenName + " | ";
                }
                if (!string.IsNullOrEmpty(MiddleInitial) && !string.IsNullOrWhiteSpace(MiddleInitial))
                {
                    activefilter += "Middle Initial : " + MiddleInitial + " | ";
                }
                if (!string.IsNullOrEmpty(Surname) && !string.IsNullOrWhiteSpace(Surname))
                {
                    activefilter += "Surname : " + Surname + " | ";
                }
                if (!string.IsNullOrEmpty(BloodType) && !string.IsNullOrWhiteSpace(BloodType))
                {
                    activefilter += "Blood Type : " + BloodType + " | ";
                }
                if (!string.IsNullOrEmpty(EmailAddress) && !string.IsNullOrWhiteSpace(EmailAddress))
                {
                    activefilter += "Email Address : " + EmailAddress + " | ";
                }

                if (!activefilter.Equals(""))
                {
                    activefilter = "Active filter = " + activefilter.Trim();
                    activefilter = activefilter.Remove(activefilter.Length - 1);
                }
                else activefilter = "No Filterting";
            }

            ViewBag.HeaderTitle = "Interview List";
            ViewBag.Filter = activefilter;

            DataTable dt = proccessFilter(filter);
            return View(dt);
        }
    }
}
