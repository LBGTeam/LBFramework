using System.Collections;
using UnityEngine;

namespace LBFramework.ResKit
{
    public interface IEnumeratorTask
    {
        //携程异步加载的接口
        IEnumerator DoLoadAsync(System.Action finishCallback);
    }
}