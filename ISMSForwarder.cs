using System;

namespace XamarinAndroid.SmsForward
{
    internal interface ISMSForwarder : IDisposable
    {
        public void Send(string message);
    }
}