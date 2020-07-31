using System;
using NSubstitute;
using NUnit.Framework;

namespace LBFramework.Nunit
{
    public class FirstTest
    {
        public interface ICalculator  
        {
            int Add(int a, int b);
            string Mode { get; set; }
            event EventHandler PoweringUp;
        }
        [Test]
        public void Test_GetSubstitute()    //NSubstitute创建类型实例
        {
            ICalculator calculator = Substitute.For<ICalculator>();    //创建实例
        }
        [Test]
        public void Test_ReturnSpecifiedValue()
        {
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3);
             
            int actual = calculator.Add(1, 2);
            Assert.AreEqual(3, actual);
        }
    }
}
