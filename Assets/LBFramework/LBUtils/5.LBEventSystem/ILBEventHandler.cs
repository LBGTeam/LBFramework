using UnityEngine;

namespace LBFramework.LBUtils
{
    public interface ILBEventHandler
    {
        void HandleEvent(int eventId, LBEventComArg arg = null);
    }
}