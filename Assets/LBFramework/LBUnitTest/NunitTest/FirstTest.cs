using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions;

namespace LBFramework.Nunit
{
    public class FirstTest
    {
        [Test]
        public void TS()
        {
            TTT(false);
        }

        public void TTT(bool b)
        {
            Console.WriteLine("LianBai");
        }
    }
}
