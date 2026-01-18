using kurs4v28;
using BCrypt.Net;
namespace Testiki
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        //тестики для Pass
        [Test]
        public void TestHashPasswordAndVerify()
        {
            string pas = "qwerty123";
            string hashPas = BCrypt.Net.BCrypt.HashPassword(pas, 12);
            bool isValid = BCrypt.Net.BCrypt.Verify(pas, hashPas);

            Console.WriteLine(hashPas);
            Console.WriteLine(isValid);
            Assert.IsTrue(isValid);
        }
        [Test]
        public void TestHashPasswordAndNotVerify()
        {
            string pas = "qwerty123";
            string hashPas = BCrypt.Net.BCrypt.HashPassword(pas, 12);
            bool isValid = BCrypt.Net.BCrypt.Verify("xz", hashPas);

            Console.WriteLine(hashPas);
            Console.WriteLine(isValid);
            Assert.IsFalse(isValid);
        }
        //тесты для Validation
        [Test]
        public void CreateUserValidation_DuplicateLogin_ReturnsError()
        {
            var result = Validation.CreateUserValidation("admin");

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Пользователь с таким логином уже существует!", result.error);
        }


    }
}