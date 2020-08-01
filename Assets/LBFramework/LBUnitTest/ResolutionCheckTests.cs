using LBFramework.LBUtils;
using NUnit.Framework;
using UnityEngine;

namespace LBFramework.NUnitTest
{
    public class ResolutionCheckTests
    {
        [Test]
        public void ResolutionCheck_LandscapeTests()
        {
            Debug.LogFormat("是否是横屏:{0}", ResolutionCheck.IsLandScape);
            Assert.AreEqual(ResolutionCheck.IsLandScape, Screen.width > Screen.height);
        }
        
        [Test]
        public void ResolutionCheck_4_3_Tests()
        {
            Debug.LogFormat("是否是4:3分辨率？{0}", ResolutionCheck.IsPad);
        }
        
        [Test]
        public void ResolutionCheck_16_9_Tests()
        {
            Debug.LogFormat("是否是16:9分辨率？{0}", ResolutionCheck.IsPhone16_9);
        }

        [Test]
        public void ResolutionCheck_Other_Tests()
        {
            Debug.LogFormat("是否是750:1334分辨率？{0}", ResolutionCheck.IsRatito(750,1334));
            Debug.LogFormat("是否是1024:768分辨率？{0}", ResolutionCheck.IsRatito(1024,768));
        }
    }
}