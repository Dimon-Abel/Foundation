using Foundation.Core.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Timers;

namespace Foundation.Core.Utility
{
    public class MailHelper
    {
        #region private static fields

        private static MailHelper instance;
        private static Timer timer;
        private static Queue<MessageEntity> msgQueue;
        private static object lockObject = new object();

        #endregion private static fields

        static MailHelper()
        {
            if (instance == null)
            {
                instance = new MailHelper();
            }
        }

        private MailHelper()
        {
        }

        public static void Instance()
        {
            if (instance == null)
            {
                instance = new MailHelper();
            }
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            int msgCount = msgQueue.Count;
            if (msgCount > 0)
            {
                while (msgCount-- > 0)
                {
                    MessageEntity entity = null;
                    try
                    {
                        entity = msgQueue.Dequeue();
                        if (entity != null)
                        {
                            smtpSend(entity);
                        }
                    }
                    catch (Exception ex)
                    {
                        string title = null;
                        string to = null;
                        if (entity != null)
                        {
                            title = entity.Subject;
                            to = entity.To.ToString();
                        }
                    }
                }
            }

            timer.Start();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="messageText">邮件内容</param>
        /// <param name="cached">是否缓存邮件（提高程序响应速度）</param>
        public static void Send(string from, string pwd, string to, string subject, string messageText, bool cached)
        {
            Send(from, pwd, to, null, null, subject, messageText, true, cached);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="messageText">邮件内容</param>
        /// <param name="cached">是否缓存邮件（提高程序响应速度）</param>
        public static void Send(string from, string pwd, string to, string cc, string subject, string messageText, bool cached)
        {
            Send(from, pwd, to, cc, null, subject, messageText, true, cached);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="messageText">邮件内容</param>
        /// <param name="cached">是否缓存邮件（提高程序响应速度）</param>
        public static void Send(string to, string subject, string messageText, bool cached)
        {
            Send(null, null, to, null, null, subject, messageText, true, cached);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="messageText">邮件内容</param>
        /// <param name="cached">是否缓存邮件（提高程序响应速度）</param>
        public static void Send(string to, List<string> attachList, string subject, string messageText, bool cached)
        {
            Send(null, null, to, null, attachList, subject, messageText, true, cached);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="cc">抄送者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="messageText">邮件内容</param>
        /// <param name="cached">是否缓存邮件（提高程序响应速度）</param>
        public static void Send(string to, string cc, string subject, string messageText, bool cached)
        {
            Send(null, null, to, cc, null, subject, messageText, true, cached);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="cc">抄送者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="attachList">附件集合</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="messageText">邮件内容</param>
        /// <param name="cached">是否缓存邮件（提高程序响应速度）</param>
        public static void Send(string to, string cc, List<string> attachList, string subject, string messageText, bool cached)
        {
            Send(null, null, to, cc, attachList, subject, messageText, true, cached);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="message">消息内容</param>
        public static void Send(string subject, string message)
        {
            var eamil = AppConfigManager.Get("Iduo:Notify:Email:AdminEmail");
            if (!string.IsNullOrWhiteSpace(eamil))
            {
                Send(eamil, subject, message, false);
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="from">发送方</param>
        /// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="cc">抄送者地址（可填多个地址，用英文分号“;”分割）</param>
        /// <param name="attachmentList">邮件附件列表，必须是绝对路径</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="messageText">邮件内容</param>
        /// <param name="cached">是否缓存邮件（提高程序响应速度）</param>
        public static void Send(string from, string sendPwd, string to, string cc, List<string> attachmentList, string subject, string body, bool isHTML, bool cached)
        {
            List<string> TO = null;
            List<string> CC = null;
            if (!string.IsNullOrEmpty(to))
            {
                string[] arrTo = to.Split(';');
                TO = new List<string>();
                for (int i = 0; i < arrTo.Length; i++)
                {
                    if (arrTo[i] != null && arrTo[i] != "")
                    {
                        TO.Add(arrTo[i]);
                    }
                }
            }
            if (!string.IsNullOrEmpty(cc))
            {
                string[] arrCC = cc.Split(';');
                CC = new List<string>();
                for (int i = 0; i < arrCC.Length; i++)
                {
                    if (arrCC[i] != null && arrCC[i] != "")
                    {
                        CC.Add(arrCC[i]);
                    }
                }
            }
            MessageEntity entity = new MessageEntity();
            entity.AttachmentList = attachmentList;
            entity.UserName = from;
            entity.To = TO;
            entity.Cc = CC;
            entity.Subject = subject;
            entity.Body = body;
            entity.Cached = cached;
            entity.SendPassword = sendPwd;
            entity.IsHTML = isHTML;
            Send(entity);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="entity">邮件配置类</param>
        public static void Send(MessageEntity entity)
        {
            if (entity.Cached)
            {
                lock (lockObject)
                {
                    if (msgQueue == null)
                    {
                        msgQueue = new Queue<MessageEntity>();
                    }
                    if (timer == null)
                    {
                        timer = new Timer(1000);
                        timer.Enabled = false;
                        timer.AutoReset = false;
                        timer.Elapsed += new ElapsedEventHandler(TimerOnElapsed);
                    }
                    if (!timer.Enabled)
                    {
                        timer.Enabled = true;
                    }
                }
                lock (msgQueue)
                {
                    msgQueue.Enqueue(entity);
                }
            }
            else
            {
                smtpSend(entity);
            }
        }

        private static void smtpSend(MessageEntity entity)
        {
            try
            {
                SmtpClient smtp = new SmtpClient(entity.SmtpServer, entity.ServerPort);
                smtp.EnableSsl = entity.IsEnableSsl;
                smtp.Credentials = new NetworkCredential(entity.UserName, entity.SendPassword);
                MailMessage mm = new MailMessage();
                //  MailMessage message = new MailMessage(entity.Email, entity.Email, entity.Subject, entity.Body);
                MailAddress mailAddress = new MailAddress(entity.Email, entity.NickName);
                mm.From = mailAddress;
                mm.Body = entity.Body;
                mm.Subject = entity.Subject;
                mm.IsBodyHtml = entity.IsHTML;
                foreach (string to in entity.To)
                {
                    if (!string.IsNullOrEmpty(to))
                        mm.To.Add(to);
                }
                if (entity.Cc != null && entity.Cc.Count > 0)
                {
                    foreach (string cc in entity.Cc)
                    {
                        if (!string.IsNullOrEmpty(cc))
                            mm.To.Add(new MailAddress(cc, entity.NickName));
                    }
                }
                if (entity.AttachmentList != null && entity.AttachmentList.Count > 0)
                {
                    foreach (string attach in entity.AttachmentList)
                    {
                        if (!string.IsNullOrEmpty(attach))
                            if (File.Exists(attach))
                                mm.Attachments.Add(new Attachment(attach));
                            else
                                throw new Exception("附件不存在！FilePath：" + attach);
                    }
                }

                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                throw new Exception($"邮件发送异常：{ex.Message}");
            }
        }
    }
}
