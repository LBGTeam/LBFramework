using System.Collections;
using LBFramework.ResKit;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LBFramework.NUnitTest
{
    public class ResKitTest
    {
        [Test]
        public void LoadAssetResSyncTest()
        {
            ResMgr.Init();
            
            var resLoader = ResLoader.Allocate();

            var coinGetClip = resLoader.LoadSync<AudioClip>("resources://coin_get");

            Assert.IsTrue(coinGetClip);

            resLoader.UnloadAllInstantiateRes(true);

            resLoader = null;
        }


        [UnityTest]
        public IEnumerator LoadAssetResAsyncTest()
        {
            /*var resLoader = new ResLoader();

            var loadDone = false;*/
            
            /*resLoader.LoadAsync("coin_get", "coin_get", (succeed, res) =>
            {
                var coinGetClip = res.Asset as AudioClip;

                Assert.IsTrue(coinGetClip);

                loadDone = succeed;
            });*/

            /*hile (!loadDone)
            {
                yield return null;
            }

            resLoader.UnloadAllInstantiateRes(true);

            resLoader = null;*/
            yield return null;
        }
    }
}