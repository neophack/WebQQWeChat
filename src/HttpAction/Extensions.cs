﻿using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FxUtility.Extensions;
using HttpAction.Action;
using HttpAction.Core;
using HttpAction.Event;

namespace HttpAction
{
    public static class Extensions
    {
        public static string GetRequestHeader(this HttpRequestItem request, CookieCollection cookieCollection)
        {
            var sb = new StringBuilder();
            sb.AppendLineIf($"{HttpConstants.Referrer}: { request.Referrer}", !request.Referrer.IsNullOrEmpty());
            sb.AppendLineIf($"{HttpConstants.ContentType}: {request.ContentType}", !request.ContentType.IsNullOrEmpty());
            var cookies = cookieCollection.OfType<Cookie>();
            sb.AppendLine($"{HttpConstants.Cookie}: {string.Join("; ", cookies)}");
            return sb.ToString();
        }

        public static string GetRequestHeader(this HttpRequestItem request, CookieContainer cookieContainer)
        {
            return GetRequestHeader(request, cookieContainer.GetCookies(new Uri(request.RawUrl)));
        }

        public static IAction CreateAction<T>(this IActionFactory factory, params object[] args) where T : IAction
        {
            return factory.CreateAction(typeof(T), args);
        }

        public static Task<ActionEvent> ExecuteAsync(this IActor actor)
        {
            return actor.ExecuteAsync(CancellationToken.None);
        }

        public static Task<ActionEvent> ExecuteAsync(this IActor actor, int seconds)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(seconds * 1000);
            return actor.ExecuteAsync(cts.Token);
        }

        /// <summary>
        /// 当结果是重试或重复的时候自动再次执行
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static async Task<ActionEvent> ExecuteAsyncAuto(this IActor actor)
        {
            ActionEvent result;
            do
            {
                result = await actor.ExecuteAsync();
            } while (result.Type == ActionEventType.EvtRepeat || result.Type == ActionEventType.EvtRetry);
            return result;
        }
    }
}
