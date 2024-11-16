using System;

namespace _Scripts.UI.Architectural
{
    public interface IContent
    {
        void SetCulling();
        void OnClickListen(Action callBack);
        void OnClickListen(string buttonName,Action callBack);
        void OnClickListen(ref Action<string> callBack,string contentId);
        void OnClickListen(string buttonName, ref Action<string> callBack, string contentId);
    }
}