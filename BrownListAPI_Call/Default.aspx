<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    Account
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnGet" runat="server" Text="Get/Login" onclick="btnGet_Click" />
                </td>
                <td>
                    <asp:Button ID="btnPost" runat="server" Text="Post/SignUp" onclick="btnPost_Click" />
                </td>
                <td>
                    <asp:Button ID="btnPostImage" runat="server" Text="Post/Image" onclick="btnPostImage_Click" />
                </td>
                <td>
                    <asp:Button ID="btnDelete" runat="server" Text="Delete/Account" onclick="btnDelete_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    Complaint
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnPostComplaint" runat="server" Text="Post/Complaint" onclick="btnPostComplaint_Click" />
                </td>
                <td>
                    <asp:Button ID="btnPostComplaintMeToo" runat="server" Text="Post/ComplaintMeToo" onclick="btnPostComplaintMeToo_Click" />
                </td>
                <td>
                    <asp:Button ID="btnPostComplaintSupport" runat="server" Text="Post/ComplaintSupport" onclick="btnPostComplaintSupport_Click" />
                </td>
                <td>
                    <asp:Button ID="btnPostComplaintFollow" runat="server" Text="Post/ComplaintFollow" onclick="btnPostComplaintFollow_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    Posting
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnPostPosting" runat="server" Text="Post/Posting" onclick="btnPostPosting_Click" />
                </td>
                <td>
                    <asp:Button ID="btnPostPostingLike" runat="server" Text="Post/PostingLike" onclick="btnPostPostingLike_Click" />
                </td>
            </tr>
        </table>
        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
