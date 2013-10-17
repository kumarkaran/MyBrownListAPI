using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using EncryptString;

namespace MvcBrownListAPI
{
    public class clsDAL
    {
        #region Variables
        string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection cnn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor call
        /// </summary>
        public clsDAL()
        {
            cnn = new SqlConnection(conString);
        }

        /// <summary>
        /// Authenticating Login credentials and return Success/Failed
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public RequestStatus checkLogin(string email, string password)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                if (cnn.State == ConnectionState.Closed)
                    cnn.Open();

                cmd.CommandText = "Select * From account Where email='" + email + "' And password='" + StringCipher.Encrypt(password) + "'";
                cmd.Connection = cnn;

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows) //validate if user exist or not
                {
                    requestStatus.Code = AppConstants.SUCCESS_CODE;
                    requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                }
                else
                {
                    requestStatus.Code = AppConstants.FAILED_CODE;
                    requestStatus.Message = AppConstants.FAILED_MESSAGE;
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "checkLogin function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// SignUp new users
        /// </summary>
        /// <param name="jsonStreamUserDetail"></param>
        /// <returns></returns>
        public RequestStatus signUp(string jsonUserDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                UserDetail userDetail = null;

                if (!string.IsNullOrEmpty(jsonUserDetail))
                    userDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<UserDetail>(jsonUserDetail);
                #endregion

                if (userDetail != null)
                {
                    #region Basic Validations
                    if (string.IsNullOrEmpty(userDetail.FirstName) || string.IsNullOrEmpty(userDetail.LastName) || string.IsNullOrEmpty(userDetail.Email) ||
                        string.IsNullOrEmpty(userDetail.Password) || string.IsNullOrEmpty(userDetail.Question1) || string.IsNullOrEmpty(userDetail.Answer1) ||
                        string.IsNullOrEmpty(userDetail.Question2) || string.IsNullOrEmpty(userDetail.Answer2))
                    {
                        requestStatus.Code = AppConstants.REQUIRED_FIELDS_CODE;
                        requestStatus.Message = AppConstants.REQUIRED_FIELDS_MESSAGE;

                        return requestStatus;
                    }

                    if (!IsValidMail(userDetail.Email))
                    {
                        requestStatus.Code = AppConstants.INVALID_EMAIL_CODE;
                        requestStatus.Message = AppConstants.INVALID_EMAIL_MESSAGE;

                        return requestStatus;
                    }

                    if (userDetail.Password != userDetail.ConfirmPassword)
                    {
                        requestStatus.Code = AppConstants.PASSWORD_MISMATCH_CODE;
                        requestStatus.Message = AppConstants.PASSWORD_MISMATCH_MESSAGE;

                        return requestStatus;
                    }
                    #endregion

                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Account_New", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firstname", userDetail.FirstName);
                    cmd.Parameters.AddWithValue("@lastname", userDetail.LastName);
                    cmd.Parameters.AddWithValue("@alias", userDetail.Alias);
                    cmd.Parameters.AddWithValue("@email", userDetail.Email);
                    cmd.Parameters.AddWithValue("@password", StringCipher.Encrypt(userDetail.Password));
                    cmd.Parameters.AddWithValue("@phone", userDetail.Phone);
                    cmd.Parameters.AddWithValue("@address", userDetail.Address);
                    cmd.Parameters.AddWithValue("@question1", userDetail.Question1);
                    cmd.Parameters.AddWithValue("@answer1", StringCipher.Encrypt(userDetail.Answer1));
                    cmd.Parameters.AddWithValue("@question2", userDetail.Question2);
                    cmd.Parameters.AddWithValue("@answer2", StringCipher.Encrypt(userDetail.Answer2));

                    var insertStatus = cmd.ExecuteScalar();

                    if (insertStatus != null)
                    {
                        string statusString = (string)insertStatus;

                        if (statusString.Contains("SUCCESS")) //successfully created account
                        {
                            requestStatus.Code = AppConstants.SUCCESS_CODE;
                            requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                        }
                        else if (statusString.Contains("FAIL")) //Email or Alias already exist
                        {
                            requestStatus.Code = AppConstants.EMAIL_EXIST_CODE;
                            requestStatus.Message = AppConstants.EMAIL_EXIST_MESSAGE;
                        }
                        else
                        {
                            requestStatus.Code = AppConstants.FAILED_CODE;
                            requestStatus.Message = AppConstants.FAILED_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "signUp function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// To upload image for Account
        /// </summary>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public RequestStatus imageUpload(string imageData)
        {
            RequestStatus requestStatus = new RequestStatus();
            int image_id = 0;

            try
            {
                #region Convertion from JSON to Generic Object
                UserFile userFile = null;

                if (!string.IsNullOrEmpty(imageData))
                    userFile = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<UserFile>(imageData);
                #endregion

                if (userFile != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd.CommandText = "Insert Into upload(filename, extension, file_content, account_id, timestamp) Values(@filename, @extension, @file_content, @account_id, @current_datetime); Select CAST(scope_identity() AS int)";

                    cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = userFile.FileName;
                    cmd.Parameters.Add("@extension", SqlDbType.VarChar).Value = userFile.Extension;
                    cmd.Parameters.Add("@file_content", SqlDbType.VarBinary).Value = userFile.FileContent;
                    cmd.Parameters.Add("@account_id", SqlDbType.Int).Value = userFile.AccountId;
                    cmd.Parameters.Add("@current_datetime", SqlDbType.DateTime).Value = userFile.TimeStamp;

                    cmd.Connection = cnn;

                    int imageInsertId = (int)cmd.ExecuteScalar();

                    if (imageInsertId > 0) //successfully added image entry
                    {
                        image_id = imageInsertId;

                        cmd = new SqlCommand();
                        cmd.CommandText = "Update account Set image_id = " + imageInsertId + " Where id=" + userFile.AccountId;
                        cmd.Connection = cnn;

                        int updateStatus = cmd.ExecuteNonQuery();

                        if (updateStatus > 0) //successfully updated account
                        {
                            requestStatus.Code = AppConstants.SUCCESS_CODE;
                            requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                        }
                        else
                        {
                            requestStatus.Code = AppConstants.FAILED_CODE;
                            requestStatus.Message = AppConstants.FAILED_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "imageUpload function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// Delete Account with related information from other tables
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RequestStatus deleteAccount(string id)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                if (cnn.State == ConnectionState.Closed)
                    cnn.Open();

                cmd = new SqlCommand("sp_Account_Delete", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);

                var deleteStatus = cmd.ExecuteScalar();

                if (deleteStatus != null)
                {
                    string statusString = (string)deleteStatus;

                    if (statusString.Contains("SUCCESS")) //successfully deleted account
                    {
                        requestStatus.Code = AppConstants.SUCCESS_CODE;
                        requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                    }
                    else 
                    {
                        requestStatus.Code = AppConstants.FAILED_CODE;
                        requestStatus.Message = AppConstants.FAILED_MESSAGE;
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Data = "deleteAccount function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// POST a new complaint
        /// </summary>
        /// <param name="jsonComplaintDetail"></param>
        /// <returns></returns>
        public RequestStatus newComplaint(string jsonComplaintDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                ComplaintDetail complaintDetail = null;

                if (!string.IsNullOrEmpty(jsonComplaintDetail))
                    complaintDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ComplaintDetail>(jsonComplaintDetail);
                #endregion

                if (complaintDetail != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Complaint_New", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@accountId", complaintDetail.AccountId);
                    cmd.Parameters.AddWithValue("@entityId", complaintDetail.EntityId);
                    cmd.Parameters.AddWithValue("@locationTypeId", complaintDetail.LocationTypeId);
                    cmd.Parameters.AddWithValue("@location", complaintDetail.Location);
                    cmd.Parameters.AddWithValue("@subject", complaintDetail.Subject);
                    cmd.Parameters.AddWithValue("@description", complaintDetail.Description);

                    if (complaintDetail.UserFile != null) //if user also pass image/file detail then upload image/file as "should also allow uploaded files to be added."
                    {
                        cmd.Parameters.AddWithValue("@fileName", complaintDetail.UserFile.FileName);
                        cmd.Parameters.AddWithValue("@extension", complaintDetail.UserFile.Extension);
                        cmd.Parameters.AddWithValue("@fileContent", complaintDetail.UserFile.FileContent);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@fileName", DBNull.Value);
                        cmd.Parameters.AddWithValue("@extension", DBNull.Value);
                        cmd.Parameters.Add("@fileContent", SqlDbType.VarBinary, -1);
                        cmd.Parameters["@fileContent"].Value = DBNull.Value;
                    }

                    int complaintInsertId = (int)cmd.ExecuteScalar();

                    if (complaintInsertId > 0) //successfully added entity entry
                    {
                        requestStatus.Code = AppConstants.SUCCESS_CODE;
                        requestStatus.Data = complaintInsertId.ToString();
                        requestStatus.Message = AppConstants.SUCCESS_MESSAGE;

                        //New Posting - it must have a Posting (of type 1 [solution]) with it
                        cmd = new SqlCommand("sp_Posting_New", cnn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@postingType", 1); //1 - solution, 2 - Comment, 3 - Response
                        cmd.Parameters.AddWithValue("@complaintId", complaintInsertId);
                        cmd.Parameters.AddWithValue("@parentPostingId", DBNull.Value); //If it is null, that means the comment/posting is associated with a complaint (complaint_id)
                        cmd.Parameters.AddWithValue("@accountId", complaintDetail.AccountId);
                        cmd.Parameters.AddWithValue("@entityId", complaintDetail.EntityId);
                        cmd.Parameters.AddWithValue("@Description", "");

                        int postingInsertId = (int)cmd.ExecuteScalar();
                    }
                    else
                    {
                        requestStatus.Code = AppConstants.FAILED_CODE;
                        requestStatus.Message = AppConstants.FAILED_MESSAGE;
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "newComplaint function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// POST a complaint me too
        /// </summary>
        /// <param name="jsonComplaintDetail"></param>
        /// <returns></returns>
        public RequestStatus complaintMeToo(string jsonComplaintDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                ComplaintDetail complaintDetail = null;

                if (!string.IsNullOrEmpty(jsonComplaintDetail))
                    complaintDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ComplaintDetail>(jsonComplaintDetail);
                #endregion

                if (complaintDetail != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Complaint_MeToo", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@complaintId", complaintDetail.ComplaintId);
                    cmd.Parameters.AddWithValue("@accountId", complaintDetail.AccountId);

                    var insertStatus = cmd.ExecuteScalar();

                    if (insertStatus != null)
                    {
                        string statusString = (string)insertStatus;

                        if (statusString.Contains("SUCCESS")) //successfully added complaint me too entry
                        {
                            requestStatus.Code = AppConstants.SUCCESS_CODE;
                            requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                        }
                        else
                        {
                            requestStatus.Code = AppConstants.FAILED_CODE;
                            requestStatus.Message = AppConstants.FAILED_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "complaintMeToo function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// POST a complaint support
        /// </summary>
        /// <param name="jsonComplaintDetail"></param>
        /// <returns></returns>
        public RequestStatus complaintSupport(string jsonComplaintDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                ComplaintDetail complaintDetail = null;

                if (!string.IsNullOrEmpty(jsonComplaintDetail))
                    complaintDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ComplaintDetail>(jsonComplaintDetail);
                #endregion

                if (complaintDetail != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Complaint_Support", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@complaintId", complaintDetail.ComplaintId);
                    cmd.Parameters.AddWithValue("@accountId", complaintDetail.AccountId);

                    var insertStatus = cmd.ExecuteScalar();

                    if (insertStatus != null)
                    {
                        string statusString = (string)insertStatus;

                        if (statusString.Contains("SUCCESS")) //successfully added complaint support entry
                        {
                            requestStatus.Code = AppConstants.SUCCESS_CODE;
                            requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                        }
                        else
                        {
                            requestStatus.Code = AppConstants.FAILED_CODE;
                            requestStatus.Message = AppConstants.FAILED_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "complaintSupport function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// POST a complaint follow
        /// </summary>
        /// <param name="jsonComplaintDetail"></param>
        /// <returns></returns>
        public RequestStatus complaintFollow(string jsonComplaintDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                ComplaintDetail complaintDetail = null;

                if (!string.IsNullOrEmpty(jsonComplaintDetail))
                    complaintDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ComplaintDetail>(jsonComplaintDetail);
                #endregion

                if (complaintDetail != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Complaint_Follow", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@complaintId", complaintDetail.ComplaintId);
                    cmd.Parameters.AddWithValue("@accountId", complaintDetail.AccountId);

                    var insertStatus = cmd.ExecuteScalar();

                    if (insertStatus != null)
                    {
                        string statusString = (string)insertStatus;

                        if (statusString.Contains("SUCCESS")) //successfully added complaint support entry
                        {
                            requestStatus.Code = AppConstants.SUCCESS_CODE;
                            requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                        }
                        else
                        {
                            requestStatus.Code = AppConstants.FAILED_CODE;
                            requestStatus.Message = AppConstants.FAILED_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "complaintFollow function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// To upload image/file based on Complaint
        /// </summary>
        /// <param name="jsonComplaintDetail"></param>
        /// <returns></returns>
        public RequestStatus complaintFileUpload(string jsonComplaintDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                ComplaintDetail complaintDetail = null;

                if (!string.IsNullOrEmpty(jsonComplaintDetail))
                    complaintDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ComplaintDetail>(jsonComplaintDetail);
                #endregion

                if (complaintDetail != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Complaint_FileUpload", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@complaintId", complaintDetail.ComplaintId);
                    cmd.Parameters.AddWithValue("@fileName", complaintDetail.UserFile.FileName);
                    cmd.Parameters.AddWithValue("@extension", complaintDetail.UserFile.Extension);
                    cmd.Parameters.AddWithValue("@fileContent", complaintDetail.UserFile.FileContent);

                    var insertStatus = cmd.ExecuteScalar();

                    if (insertStatus != null)
                    {
                        string statusString = (string)insertStatus;

                        if (statusString.Contains("SUCCESS")) //successfully added complaint file entry
                        {
                            requestStatus.Code = AppConstants.SUCCESS_CODE;
                            requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                        }
                        else
                        {
                            requestStatus.Code = AppConstants.FAILED_CODE;
                            requestStatus.Message = AppConstants.FAILED_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "complaintFileUpload function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// POST a new posting
        /// </summary>
        /// <param name="jsonPostingDetail"></param>
        /// <returns></returns>
        public RequestStatus newPosting(string jsonPostingDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                PostingDetail postingDetail = null;

                if (!string.IsNullOrEmpty(jsonPostingDetail))
                    postingDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<PostingDetail>(jsonPostingDetail);
                #endregion

                if (postingDetail != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Posting_New", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@postingTypeId", postingDetail.PostingTypeId);
                    cmd.Parameters.AddWithValue("@complaintId", postingDetail.ComplaintId);

                    if (postingDetail.ParentPostingId == 0) //handling null value
                        cmd.Parameters.AddWithValue("@parentPostingId", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@parentPostingId", postingDetail.ParentPostingId);

                    cmd.Parameters.AddWithValue("@accountId", postingDetail.AccountId);

                    if (postingDetail.EntityId == 0) //handling null value
                        cmd.Parameters.AddWithValue("@entityId", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@entityId", postingDetail.EntityId);

                    cmd.Parameters.AddWithValue("@Description", postingDetail.Description);

                    int postingInsertId = (int)cmd.ExecuteScalar();

                    if (postingInsertId > 0) //successfully added posting entry
                    {
                        requestStatus.Code = AppConstants.SUCCESS_CODE;
                        requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                    }
                    else
                    {
                        requestStatus.Code = AppConstants.FAILED_CODE;
                        requestStatus.Message = AppConstants.FAILED_MESSAGE;
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "newPosting function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// POST a posting like
        /// </summary>
        /// <param name="jsonPostingDetail"></param>
        /// <returns></returns>
        public RequestStatus postingLike(string jsonPostingDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                PostingDetail postingDetail = null;

                if (!string.IsNullOrEmpty(jsonPostingDetail))
                    postingDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<PostingDetail>(jsonPostingDetail);
                #endregion

                if (postingDetail != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Posting_Like", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@postingId", postingDetail.PostingId);
                    cmd.Parameters.AddWithValue("@accountId", postingDetail.AccountId);

                    var insertStatus = cmd.ExecuteScalar();

                    if (insertStatus != null)
                    {
                        string statusString = (string)insertStatus;

                        if (statusString.Contains("SUCCESS")) //successfully added posting like entry
                        {
                            requestStatus.Code = AppConstants.SUCCESS_CODE;
                            requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                        }
                        else
                        {
                            requestStatus.Code = AppConstants.FAILED_CODE;
                            requestStatus.Message = AppConstants.FAILED_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "postingLike function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// To upload image/file based on Posting
        /// </summary>
        /// <param name="jsonPostingDetail"></param>
        /// <returns></returns>
        public RequestStatus postingFileUpload(string jsonPostingDetail)
        {
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                #region Convertion from JSON to Generic Object
                PostingDetail postingDetail = null;

                if (!string.IsNullOrEmpty(jsonPostingDetail))
                    postingDetail = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<PostingDetail>(jsonPostingDetail);
                #endregion

                if (postingDetail != null)
                {
                    if (cnn.State == ConnectionState.Closed)
                        cnn.Open();

                    cmd = new SqlCommand("sp_Posting_FileUpload", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@postingId", postingDetail.PostingId);
                    cmd.Parameters.AddWithValue("@fileName", postingDetail.UserFile.FileName);
                    cmd.Parameters.AddWithValue("@extension", postingDetail.UserFile.Extension);
                    cmd.Parameters.AddWithValue("@fileContent", postingDetail.UserFile.FileContent);

                    var insertStatus = cmd.ExecuteScalar();

                    if (insertStatus != null)
                    {
                        string statusString = (string)insertStatus;

                        if (statusString.Contains("SUCCESS")) //successfully added posting file entry
                        {
                            requestStatus.Code = AppConstants.SUCCESS_CODE;
                            requestStatus.Message = AppConstants.SUCCESS_MESSAGE;
                        }
                        else
                        {
                            requestStatus.Code = AppConstants.FAILED_CODE;
                            requestStatus.Message = AppConstants.FAILED_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.Code = AppConstants.FATAL_ERROR_CODE;
                requestStatus.Message = AppConstants.FATAL_ERROR_MESSAGE;
                requestStatus.Data = "postingFileUpload function : " + ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            return requestStatus;
        }

        /// <summary>
        /// Check if the mail is valid.
        /// </summary>
        /// <param name="emailInput">emailInput</param>
        /// <returns>boolean value</returns>
        internal static bool IsValidMail(string emailInput)
        {
            Regex emailExpression = new Regex(AppConstants.EMAIL_MATCH_PATTERN, RegexOptions.IgnoreCase);
            bool isValidEmail = false;
            if (emailInput != null)
            {
                isValidEmail = emailExpression.IsMatch(emailInput);
            }
            return isValidEmail;
        }

        #endregion
    }
}