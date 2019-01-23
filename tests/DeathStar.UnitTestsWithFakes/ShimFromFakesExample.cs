using System;
using System.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeathStar.UnitTestsWithFakes
{
    /// <summary>
    /// Пример создания подделки для статического класса
    /// </summary>
    [TestClass]
    public class ShimFromFakesExample
    {
        [TestMethod]
        public void ShimTest()
        {
            // 1. создаем ShimContext - это обязательно
            using (ShimsContext.Create())
            {
                // 2. задаем нужное поведение
                // нюанс: к имени объекта добавляется префикс Shim
                ShimDateTime.NowGet = () => new DateTime(2000, 1, 1);

                // 3. а тут тестируем то, что нам надо
            }
        }
    }
}
