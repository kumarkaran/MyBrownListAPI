using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using ExtensionMethods;
using System.Xml;
using System.Text;
using System.ServiceModel;

public partial class _Default : System.Web.UI.Page 
{
    int complaintId = 3;
    int accountId = 6;
    int entityId = 4;
    int locationTypeId = 1;
    int postingId = 2;
    int postingType = 1;
    int parentPostingId = 1;
    string location = "Test Location";
    string subject = "Test Subject";
    string description = "Test Description";
    string firstName = "Karan10";
    string lastName = "Kumar10";
    string alias = "karan10";
    string email = "karan10@yahoo.com";
    string password = "karan1234";
    string confirmPassword = "karan1234";
    string phone = "1234567890";
    string address = "Test Address 10";
    string imagePath = @"";
    string question1 = "Test Question1";
    string answer1 = "Test Answer1";
    string question2 = "Test Question2";
    string answer2 = "Test Answer2";

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnGet_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Account/Login?email=" + email + "&password=" + password;
        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "GET";

        HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
        Stream GETResponseStream = GETResponse.GetResponseStream();
        StreamReader sr = new StreamReader(GETResponseStream);

        lblResult.Text = sr.ReadToEnd();
    }

    protected void btnPost_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Account/SignUp";


            UserDetail userDetail = new UserDetail
            {
                FirstName = firstName,
                LastName = lastName,
                Alias = alias,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword,
                Phone = phone,
                Address = address,
                Question1 =  question1,
                Answer1 = answer1,
                Question2 = question2,
                Answer2 = answer2
            };

            string jsonUserDetail = userDetail.ToJSON();

            HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
            GETRequest.Method = "POST";
        
            using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonUserDetail);
                streamWriter.Flush();
                streamWriter.Close();

                HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
                Stream GETResponseStream = GETResponse.GetResponseStream();
                StreamReader sr = new StreamReader(GETResponseStream);

                lblResult.Text = sr.ReadToEnd();
            }   
        
    }

    protected void btnPostImage_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Account/ImageUpload";

        string jsonUserFile = ParseImage(imagePath).ToJSON();

        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
        {
            streamWriter.Write(jsonUserFile);
            streamWriter.Flush();
            streamWriter.Close();

            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            lblResult.Text = sr.ReadToEnd();
        }

    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Account/Delete";

        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "DELETE";

        using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
        {
            streamWriter.Write(accountId);
            streamWriter.Flush();
            streamWriter.Close();

            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            lblResult.Text = sr.ReadToEnd();
        }
    }

    protected void btnPostComplaint_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Complaint/New";


        ComplaintDetail complaintDetail = new ComplaintDetail
        {
            AccountId = accountId,
            EntityId = entityId,
            LocationTypeId = locationTypeId,
            Location = location,
            Subject = subject,
            Description = description,
            UserFile = ParseImage(imagePath)
        };

        string jsonComplaintDetail = complaintDetail.ToJSON();

        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
        {
            streamWriter.Write(jsonComplaintDetail);
            streamWriter.Flush();
            streamWriter.Close();

            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            lblResult.Text = sr.ReadToEnd();
        }   
    }

    protected void btnPostComplaintMeToo_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Complaint/MeToo";

        ComplaintDetail complaintDetail = new ComplaintDetail
        {
            ComplaintId = complaintId,
            AccountId = accountId
        };

        string jsonComplaintDetail = complaintDetail.ToJSON();

        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
        {
            streamWriter.Write(jsonComplaintDetail);
            streamWriter.Flush();
            streamWriter.Close();

            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            lblResult.Text = sr.ReadToEnd();
        }
    }

    protected void btnPostComplaintSupport_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Complaint/Support";

        ComplaintDetail complaintDetail = new ComplaintDetail
        {
            ComplaintId = complaintId,
            AccountId = accountId
        };

        string jsonComplaintDetail = complaintDetail.ToJSON();

        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
        {
            streamWriter.Write(jsonComplaintDetail);
            streamWriter.Flush();
            streamWriter.Close();

            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            lblResult.Text = sr.ReadToEnd();
        }
    }

    protected void btnPostComplaintFollow_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Complaint/Follow";

        ComplaintDetail complaintDetail = new ComplaintDetail
        {
            ComplaintId = complaintId,
            AccountId = accountId
        };

        string jsonComplaintDetail = complaintDetail.ToJSON();

        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
        {
            streamWriter.Write(jsonComplaintDetail);
            streamWriter.Flush();
            streamWriter.Close();

            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            lblResult.Text = sr.ReadToEnd();
        }
    }

    protected void btnPostPosting_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Posting/New";

        PostingDetail postingDetail = new PostingDetail
        {
            PostingType = postingType,
            ComplaintId = complaintId,
            ParentPostingId = parentPostingId,
            AccountId = accountId,
            EntityId = entityId,
            Description = description
        };

        string jsonPostingDetail = postingDetail.ToJSON();

        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
        {
            streamWriter.Write(jsonPostingDetail);
            streamWriter.Flush();
            streamWriter.Close();

            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            lblResult.Text = sr.ReadToEnd();
        }
    }

    protected void btnPostPostingLike_Click(object sender, EventArgs e)
    {
        string url = "http://localhost:54106/1.0/Posting/Like";

        PostingDetail postingDetail = new PostingDetail
        {
            PostingId = postingId,
            AccountId = accountId
        };

        string jsonPostingDetail = postingDetail.ToJSON();

        HttpWebRequest GETRequest = (HttpWebRequest)WebRequest.Create(url);
        GETRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(GETRequest.GetRequestStream()))
        {
            streamWriter.Write(jsonPostingDetail);
            streamWriter.Flush();
            streamWriter.Close();

            HttpWebResponse GETResponse = (HttpWebResponse)GETRequest.GetResponse();
            Stream GETResponseStream = GETResponse.GetResponseStream();
            StreamReader sr = new StreamReader(GETResponseStream);

            lblResult.Text = sr.ReadToEnd();
        }
    }

    private UserFile ParseImage(string imagePath)
    {
        UserFile userFile = new UserFile();
        try
        {
            System.Drawing.Image imageToConvert = System.Drawing.Image.FromFile(imagePath);
            using (MemoryStream ms = new MemoryStream())
            {
                imageToConvert.Save(ms, imageToConvert.RawFormat);

                userFile.FileName = Path.GetFileNameWithoutExtension(imagePath);
                userFile.Extension = Path.GetExtension(imagePath);
                userFile.FileContent = ms.ToArray();
                userFile.AccountId = Convert.ToInt16(accountId);
                userFile.TimeStamp = DateTime.Now;
            }
        }
        catch (Exception) { userFile = null; }
        
        return userFile;
    }
}

public class UserDetail
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Alias { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Question1 { get; set; }
    public string Answer1 { get; set; }
    public string Question2 { get; set; }
    public string Answer2 { get; set; }
}

public class UserFile
{
    public string FileName { get; set; }
    public string Extension { get; set; }
    public byte[] FileContent { get; set; }
    public int AccountId { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class ComplaintDetail
{
    public int ComplaintId { get; set; }
    public int AccountId { get; set; }
    public int EntityId { get; set; }
    public int LocationTypeId { get; set; }
    public string Location { get; set; }
    public string Subject {get; set; }
    public string Description { get; set; }
    public UserFile UserFile { get; set; }
}

public class PostingDetail
{
    public int PostingId { get; set; }
    public int PostingType { get; set; }
    public int ComplaintId { get; set; }
    public int ParentPostingId { get; set; }
    public int AccountId { get; set; }
    public int EntityId { get; set; }
    public string Description { get; set; }
}

namespace ExtensionMethods
{
    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 2147483647;
            return serializer.Serialize(obj);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 2147483647;
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(obj);
        }
    }
}