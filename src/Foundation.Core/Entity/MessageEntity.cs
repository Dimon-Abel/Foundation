using Foundation.Core.Extensions;
using Foundation.Core.Utility;
using System.Collections.Generic;

namespace Foundation.Core.Entity
{
    public class MessageEntity
    {
        private string smtpServer;

        /// <summary>
        /// SMTP服务地址
        /// </summary>
        public string SmtpServer
        {
            get
            {
                if (smtpServer == null)
                    smtpServer = AppConfigManager.Get("Notify:Email:ServerName");
                return smtpServer;
            }
            set { smtpServer = value; }
        }

        private int serverPort;

        /// <summary>
        /// 服务端口号
        /// </summary>
        public int ServerPort
        {
            get
            {
                if (serverPort <= 0)
                    serverPort = int.Parse(AppConfigManager.Get("Notify:Email:serverPort"));
                return serverPort;
            }
            set { serverPort = value; }
        }

        private string userName;

        /// <summary>
        /// 邮箱登录用户名
        /// </summary>
        public string UserName
        {
            get
            {
                if (userName == null)
                    userName = AppConfigManager.Get("Notify:Email:FromName");
                return userName;
            }
            set { userName = value; }
        }

        private List<string> to;

        /// <summary>
        /// 接收邮箱集合
        /// </summary>
        public List<string> To
        {
            get { return to; }
            set { to = value; }
        }

        private List<string> cc;

        /// <summary>
        /// 抄送邮件集合
        /// </summary>
        public List<string> Cc
        {
            get { return cc; }
            set { cc = value; }
        }

        private string subject;

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        private string body;

        /// <summary>
        /// 邮件主体
        /// </summary>
        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        private List<string> attachmentList;

        /// <summary>
        /// 附件列表，需附件的绝对路径
        /// </summary>
        public List<string> AttachmentList
        {
            get { return attachmentList; }
            set { attachmentList = value; }
        }

        private bool cached = true;

        /// <summary>
        /// 是否缓存邮件（提高程序响应速度）
        /// </summary>
        public bool Cached
        {
            get { return cached; }
            set { cached = value; }
        }

        private bool isHTML = true;

        /// <summary>
        /// 邮件主体是否为HTML
        /// </summary>
        public bool IsHTML
        {
            get { return isHTML; }
            set { isHTML = value; }
        }

        private string sendPassword;

        /// <summary>
        /// 发送者密码
        /// </summary>
        public string SendPassword
        {
            get
            {
                if (sendPassword == null)
                    sendPassword = AppConfigManager.Get("Notify:Email:FromPass");
                return sendPassword;
            }
            set { sendPassword = value; }
        }

        /// <summary>
        /// 发送者邮件地址
        /// </summary>
        public string Email
        {
            get
            {
                if (email == null)
                {
                    email = AppConfigManager.Get("Notify:Email:FromEmail");
                }
                return email;
            }
            set { email = value; }
        }

        private string email;

        private string nickName;

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName
        {
            get
            {
                if (nickName == null)
                    nickName = AppConfigManager.Get("Notify:Email:FromNickName") == null ? AppConfigManager.Get("Notify:Email:FromEmail") : AppConfigManager.Get("Notify:Email:FromNickName");
                return nickName;
            }
            set { email = value; }
        }

        private bool isEnableSsl;

        public bool IsEnableSsl
        {
            get
            {
                isEnableSsl = AppConfigManager.Get("Notify:Email:EnableSsl").CastTo(true);
                return isEnableSsl;
            }
            set
            {
                isEnableSsl = value;
            }
        }
    }
}