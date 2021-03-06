﻿using System.Threading;
using System.Threading.Tasks;
using HttpAction.Event;
using WebWeChat.Im.Action;
using WebWeChat.Im.Core;
using WebWeChat.Im.Module.Interface;
using HttpAction;

namespace WebWeChat.Im.Module.Impl
{
    public class ContactModule : WeChatModule, IContactModule
    {
        public Task<ActionEvent> GetContact(ActionEventListener listener = null)
        {
            // 如果直接new一个Action并执行的话也可以，但是不能自动重试
            return new WeChatActionFuture(Context, listener)
                .PushAction<GetContactAction>()
                .ExecuteAsync();
        }

        public Task<ActionEvent> GetGroupMember(ActionEventListener listener = null)
        {
            return new WeChatActionFuture(Context, listener)
               .PushAction<BatchGetContactAction>()
               .ExecuteAsync();
        }

        public ContactModule(IWeChatContext context) : base(context)
        {
        }
    }
}
