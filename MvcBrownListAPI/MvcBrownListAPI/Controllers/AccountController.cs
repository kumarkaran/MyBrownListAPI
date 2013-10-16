using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting.Web.Http;
using ExtensionMethods;
using System.IO;

namespace MvcBrownListAPI.Controllers
{
    public class AccountController : ApiController
    {
        #region Account
        [HttpGet]
        [HttpRoute("/1.0/Account/Login?email={email}&password={password}")]
        public string Login(string email, string password)
        {
            clsDAL objDAL = new clsDAL();
            return objDAL.checkLogin(email, password).ToJSON();
        }

        [HttpPost]
        [HttpRoute("/1.0/Account/SignUp")]
        public string SignUp()
        {
            //Retrieve POST data
            string jsonUserDetail = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.signUp(jsonUserDetail).ToJSON();
        }

        [HttpPost]
        [HttpRoute("/1.0/Account/ImageUpload")]
        public string ImageUpload()
        {
            //Retrieve POST data
            string imageData = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.imageUpload(imageData).ToJSON();
        }
        
        [HttpDelete]
        [HttpRoute("/1.0/Account/Delete")]
        public string DeleteAccount()
        {
            //Retrieve POST data
            string accountId = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.deleteAccount(accountId).ToJSON();
        }
        #endregion

        #region Complaint
        [HttpPost]
        [HttpRoute("/1.0/Complaint/New")]
        public string NewComplaint()
        {
            //Retrieve POST data
            string jsonComplaintDetail = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.newComplaint(jsonComplaintDetail).ToJSON();
        }

        [HttpPost]
        [HttpRoute("/1.0/Complaint/MeToo")]
        public string ComplaintMeToo()
        {
            //Retrieve POST data
            string jsonComplaintDetail = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.complaintMeToo(jsonComplaintDetail).ToJSON();
        }

        [HttpPost]
        [HttpRoute("/1.0/Complaint/Support")]
        public string ComplaintSupport()
        {
            //Retrieve POST data
            string jsonComplaintDetail = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.complaintSupport(jsonComplaintDetail).ToJSON();
        }

        [HttpPost]
        [HttpRoute("/1.0/Complaint/Follow")]
        public string ComplaintFollow()
        {
            //Retrieve POST data
            string jsonComplaintDetail = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.complaintFollow(jsonComplaintDetail).ToJSON();
        }
        #endregion

        #region Posting
        [HttpPost]
        [HttpRoute("/1.0/Posting/New")]
        public string NewPosting()
        {
            //Retrieve POST data
            string jsonComplaintDetail = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.newPosting(jsonComplaintDetail).ToJSON();
        }

        [HttpPost]
        [HttpRoute("/1.0/Posting/Like")]
        public string PostingLike()
        {
            //Retrieve POST data
            string jsonComplaintDetail = Request.Content.ReadAsStringAsync().Result;

            clsDAL objDAL = new clsDAL();
            return objDAL.postingLike(jsonComplaintDetail).ToJSON();
        }
        #endregion

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }
    }
}
