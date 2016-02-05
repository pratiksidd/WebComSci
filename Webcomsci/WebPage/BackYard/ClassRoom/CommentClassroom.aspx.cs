﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace Webcomsci.WebPage.BackYard.ClassRoom
{
    public partial class CommentClassroom : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                showLIstview();

            }

        }

        protected object postID
        {
            get
            {
                return ViewState["postID"];
            }
            set
            {
                ViewState["postID"] = value;
            }
        }




        private void showLIstview()
        {
            try
            {
                string usertype = Session["userType"].ToString();
                int classid = Convert.ToInt32(Request.QueryString["classid"]);
                int dtchID = Convert.ToInt32(Request.QueryString["dchID"]);
                string userid = Session["userid"].ToString();

                DataTable dtPostClass = new DataTable();
                dtPostClass = BLL.ClassRoom.selectCommentPostClassRoom(usertype, userid, dtchID);

                ListViewShowPost.DataSource = dtPostClass;
                ListViewShowPost.DataBind();
            }
            catch (Exception)
            {

            }
        }



        protected DataTable getCommentsForPost(string thePost)
        {
            int clickPostID = 0;

            try
            {
                clickPostID = Convert.ToInt32(postID);
            }
            catch (Exception)
            {
                postID = 0;
            }


            DataTable dt = new DataTable();
            if (clickPostID == (Convert.ToInt32(thePost)))
            {
                dt = BLL.ClassRoom.getCommentsForPost(Convert.ToInt32(thePost), clickPostID);



            }
            else
            {
                dt = BLL.ClassRoom.getCommentsForPost(Convert.ToInt32(thePost), 0);

            }

            return dt;

        }


        public void ShowMessageWeb(string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("alert('");
            sb.Append(msg.Replace("\n", "\\n").Replace("\r", "").Replace("'", "\\'"));
            sb.Append("');");
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showalert", sb.ToString(), true);

        }

        protected void btnPost_Click(object sender, EventArgs e)
        {

            /**********************************
             * 
             * ปุ่มโพสต์
             * 
             * ******************************/
            string postTxtbo = TextAreaPostClassRoom.InnerHtml.ToString();
            if (!postTxtbo.Equals(""))
            {

                string userid = Session["userid"].ToString();
                string usertype = Session["userType"].ToString();
                string post = TextAreaPostClassRoom.InnerHtml;//txtPost.Text.ToString();
                int classID = Convert.ToInt32(Request.QueryString["classid"]);

                bool re = BLL.ClassRoom.postInClassRoom(userid, usertype, post, classID);
                if (re)
                {
                    //  txtPost = "";
                    TextAreaPostClassRoom.InnerHtml = "";

                    showLIstview();
                }
            }
        }


        protected void btnComment_Click(object sender, EventArgs e)
        {

            /**********************************
             * 
             * ปุ่มคอมเม้น
             * 
             * ******************************/
            Button objImage = (Button)sender;

            string[] commandArgs = objImage.CommandArgument.ToString().Split(new char[] { ',' });
            string id = commandArgs[0];
            string userid = Session["userid"].ToString();
            string usertype = Session["userType"].ToString();


            TextBox tbcomment = (TextBox)objImage.FindControl("textarea");

            string commentvalue = tbcomment.Text.ToString();
            if (!commentvalue.Equals(""))
            {

                bool re = BLL.ClassRoom.insertComment(id, tbcomment.Text, userid, usertype);

                if (re)
                {
                    tbcomment.Text = "";

                    showLIstview();
                }

                postID = id;
                tbcomment.Focus();
            }

            /*url:Ref :http://stackoverflow.com/questions/9822827/how-to-pass-my-multiple-values-through-command-argument-c*/
        }

        protected void ListViewShowKmPost_ItemDataBound(object sender, ListViewItemEventArgs e)
        {

            try
            {
                /**************************
                 * 
                 * เช็คการเปิดปุ่มลบของการโพสต์
                 * 
                 ************************** */
                string sessionuserid = Session["userid"].ToString();
                if (e.Item.ItemType == ListViewItemType.DataItem)
                {

                    Label user = (Label)e.Item.FindControl("lblPostUserID");
                    Label posID = (Label)e.Item.FindControl("lblpostClassID");
                    ImageButton imgPostClose = (ImageButton)e.Item.FindControl("imgBtnClosePost");

                    LinkButton linkShowAllComment = (LinkButton)e.Item.FindControl("lblshowAllComment");
                    Image imgShowAllcom = (Image)e.Item.FindControl("imgPicAllcom");

                    string userid = user.Text;

                    int classid = Convert.ToInt32(Request.QueryString["classid"]);

                    string addminUserid = BLL.ClassRoom.selectAdminUseridClassRoom(classid);

                    bool checkOpenShowAllcomment = BLL.ClassRoom.selectShowAllComment(posID.Text);
                    if (checkOpenShowAllcomment)
                    {
                        linkShowAllComment.Visible = true;
                        imgShowAllcom.Visible = true;
                    }

                    if (sessionuserid.Equals(userid) || sessionuserid.Equals(addminUserid))
                    {
                        imgPostClose.Visible = true;

                    }

                    ///***************imageuser*******************************/

                    //string usertype = Session["userType"].ToString();
                    //string path = BLL.mainManage.selectPathPictureUser(sessionuserid, usertype);
                    //ImageButton imgOwn = (ImageButton)e.Item.FindControl("imgOwn");
                    //imgOwn.ImageUrl = path;

                    ///***********************************************/
                }
            }
            catch (Exception ex) { }

        }

        protected void lvwComments_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            /**************************
             * 
             * เช็คการเปิดปุ่มลบของการคอมเม้น
             * 
             ************************** */
            string sessionuserid = Session["userid"].ToString();
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {

                Label user = (Label)e.Item.FindControl("lblcomuseridID");
                ImageButton imgPostClose = (ImageButton)e.Item.FindControl("imgBtnCloseComment");

                string userid = user.Text;

                int classid = Convert.ToInt32(Request.QueryString["classid"]);

                string addminUserid = BLL.ClassRoom.selectAdminUseridClassRoom(classid);
                if (sessionuserid.Equals(userid) || sessionuserid.Equals(addminUserid))
                {
                    imgPostClose.Visible = true;
                }
            }
        }




        protected void DataPagerPost_PreRender(object sender, EventArgs e)
        {
            showLIstview();
        }



        protected void lblshowAllComment_Click(object sender, EventArgs e)
        {
            /*********************************
             * 
             * แสดงคอมเม้นทั้งหมด
             * 
             * *****************************/
            LinkButton objImage = (LinkButton)sender;

            string[] commandArgs = objImage.CommandArgument.ToString().Split(new char[] { ',' });
            string id = commandArgs[0];

            LinkButton lblshowAll = (LinkButton)objImage.FindControl("lblshowAllComment");
            lblshowAll.Visible = false;



            TextBox tbcomment = (TextBox)objImage.FindControl("textarea");

            postID = id;
            tbcomment.Focus();
        }

        protected void imgBtnClosePost_Click(object sender, ImageClickEventArgs e)
        {
            /********************************
             * 
             * ลบข้อมูลโพสต์
             * 
             * *****************************/
            ImageButton objImage = (ImageButton)sender;

            string[] commandArgs = objImage.CommandArgument.ToString().Split(new char[] { ',' });
            string posid = commandArgs[0];
            string userid = commandArgs[1];

            bool delPost = BLL.ClassRoom.closePost(posid);
            if (delPost)
            {
                showLIstview();
            }

        }

        protected void imgBtnCloseComment_Click(object sender, ImageClickEventArgs e)
        {
            /********************************
             * 
             * ลบข้อมูลคอมเม้น
             * 
             * *****************************/

            ImageButton objImage = (ImageButton)sender;

            string[] commandArgs = objImage.CommandArgument.ToString().Split(new char[] { ',' });
            string commentid = commandArgs[0];
            string userid = commandArgs[1];


            bool delPost = BLL.ClassRoom.closeComment(commentid);
            Label tbcomment = (Label)objImage.FindControl("lbldetailComment");
            if (delPost)
            {
                showLIstview();
                tbcomment.Focus();
            }




        }







    }
}