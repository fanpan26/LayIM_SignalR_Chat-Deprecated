using LayIM.BLL;
using LayIM.Model;
using LayIM.Util;
using Macrosage.RabbitMQ.Server.Customer;
using Macrosage.RabbitMQ.Server.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LayIM.Queue
{
    /// <summary>
    /// 队列逻辑处理
    /// </summary>
   public class ChatQueue
    {
        const string QueueName = "LAYIM_CHAT_MSG_QUEUE";
        /// <summary>
        /// 接收到队列消息，进行处理
        /// </summary>
        public static void StartListeningChat()
        {
            IMessageCustomer customer = new MessageCustomer(QueueName);
            customer.StartListening();
            customer.ReceiveMessageCallback = message => {
                var msgModel = JsonHelper.DeserializeObject<ChatMessageResult>(message);
                UserBLL.AddMessage(msgModel);
                return true;
            };
        }

        /// <summary>
        /// 队列消息发布
        /// </summary>
        /// <param name="message"></param>
        public static void PublishMessage(ChatMessageResult message)
        {
            IMessageProduct product = new MessageProduct(QueueName);
            var strMessage = JsonHelper.SerializeObject(message);
            product.Publish(strMessage);
        }
    }
}
