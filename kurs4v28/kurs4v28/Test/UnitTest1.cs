
using kurs4v28;
using kurs4v28.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Testiki
{
    [TestFixture]
    public class Tests
    {
        //private post_officeEntities2 context => ModelDB.GetContext();
        //private IDbContext context => ModelDB.GetContext();
        private Mock<post_officeEntities2> _mockContext;
        employee emp = new employee();
        Role role = new Role();
        Sposob_otpravki sposob = new Sposob_otpravki();
        services service = new services();
        Client client = new Client();
        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<post_officeEntities2>();
            SetupTestData(); 
        }
        private void SetupTestData()
        {
            var testUsers = new List<employee>
            {
                new employee { id = 1, login = "admin", password = Pass.HashPasswordWithBcrypt("12345678w"), role_id = 1 },
                new employee { id = 2, login = "service", password = Pass.HashPasswordWithBcrypt("12345678w"), role_id = 4 },
                new employee { id = 3, login = "operator", password =Pass.HashPasswordWithBcrypt("12345678w"), role_id = 3 }
            }.AsQueryable();

            var testRoles = new List<Role>
            {
                new Role { id = 1, role_name = "admin" },
                new Role { id = 3, role_name = "operator" },
                new Role { id = 4, role_name = "service_manager" }
            }.AsQueryable();

            var testServices = new List<services>
            {
                new services { id = 1, name = "Отправка письма" },
                new services { id = 2, name = "Отправка посылки" }
            }.AsQueryable();

            var testSposob = new List<Sposob_otpravki>
            {
                new Sposob_otpravki { id = 1, name = "Поезд" },
                new Sposob_otpravki { id = 2, name = "Курьер" }
            }.AsQueryable();

            var testClients = new List<Client>
            {
                new Client { id = 1, name = "Михаил", phone = "79062345487", passport_seria_number = "3456 6554" },
                new Client { id = 2, name = "Анна", phone = "79165554433", passport_seria_number = "1234 5678" }
            }.AsQueryable();

            // Мокаем DbSet'ы для post_officeEntities2
            MockDbSet(_mockContext, testUsers, c => c.employee);
            MockDbSet(_mockContext, testRoles, c => c.Role);
            MockDbSet(_mockContext, testServices, c => c.services);
            MockDbSet(_mockContext, testSposob, c => c.Sposob_otpravki);
            MockDbSet(_mockContext, testClients, c => c.Client);
        }
        private void MockDbSet<T>(Mock<post_officeEntities2> context, IQueryable<T> data,
           System.Linq.Expressions.Expression<Func<post_officeEntities2, DbSet<T>>> expression) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            context.Setup(expression).Returns(mockSet.Object);
        }
       
        //тестики для Pass
        [Test]
        public void TestHashPasswordAndNotVerify()
        {
            string password = "qwerty123";
            string hash = Pass.HashPasswordWithBcrypt(password);
            bool isValid = Pass.VerifyBcryptPassword("ssss", hash);
            Console.WriteLine(isValid);
            Assert.IsFalse(isValid);
        }
        [Test]
        public void Password_HashAndVerify_ShouldWork()
        {
            string password = "qwerty123";
            string hash = Pass.HashPasswordWithBcrypt(password);
            bool isValid = Pass.VerifyBcryptPassword(password, hash);

            Assert.IsTrue(isValid);
        }
        //тесты для Validation
        [Test]
        public void CreateUserValidation_DuplicateLogin_ReturnsError()
        {
            var result = Validation.CreateUserValidation("admin", "12345678e", _mockContext.Object);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Пользователь с таким логином уже существует!", result.error);
        }
        [Test]
        public void CreateUserValidationLoginNew()
        {
            var result = Validation.CreateUserValidation("admin69", "12345678e", _mockContext.Object);

            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void CreateUserValidation_PasswordMenshe_ReturnsError()
        {
            var result = Validation.CreateUserValidation("admin1", "123456e", _mockContext.Object);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Должно быть не меньше 8 символов", result.error);
        }
        [Test]
        public void CreateUserValidation_PasswordPasswordOnlyNums_ReturnsError()
        {
            var result = Validation.CreateUserValidation("admin1", "12345678", _mockContext.Object);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Пароль должен содержать хотя бы одну букву!", result.error);
        }
        [Test]
        public void CreateUserValidation_PasswordPasswordOnlyLetters_ReturnsError()
        {
            var result = Validation.CreateUserValidation("admin1", "qwertyui", _mockContext.Object);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Пароль должен содержать хотя бы одну цифру!", result.error);
        }

        [Test]
        public void EditUserValidation_DuplicateLogin_ReturnsError()
        {
            emp = _mockContext.Object.employee.FirstOrDefault(e => e.login == "admin");
            role = _mockContext.Object.Role.FirstOrDefault(r => r.role_name == "admin");
            var result = Validation.EditUserValidation("service", "12345678e", role, emp, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Пользователь с таким логином уже существует!", result.error);
        }
        [Test]
        public void EditUserValidation_ChangeAdminRole_ReturnsError()
        {
            emp = _mockContext.Object.employee.FirstOrDefault(e => e.login == "admin");
            role = _mockContext.Object.Role.FirstOrDefault(r => r.role_name == "operator");
            var result = Validation.EditUserValidation("admin", "", role, emp, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Вы не можете менять роль админа!", result.error);
        }
        [Test]
        public void EditUserValidation_ChangeNotAdminToAdminRole_ReturnsError()
        {
            emp = _mockContext.Object.employee.FirstOrDefault(e => e.login == "service");
            role = _mockContext.Object.Role.FirstOrDefault(r => r.role_name == "admin");
            var result = Validation.EditUserValidation("service", "", role, emp, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Вы не можете сделать этого пользователя админом!", result.error);
        }
        [Test]
        public void EditUserValidation_ChangeNotAdminRole_ReturnsError()
        {
            emp = _mockContext.Object.employee.FirstOrDefault(e => e.login == "service");
            role = _mockContext.Object.Role.FirstOrDefault(r => r.role_name == "operator");
            var result = Validation.EditUserValidation("service", "", role, emp, _mockContext.Object);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void EditUserValidation_ChangeLogin_ReturnsError()
        {
            emp = _mockContext.Object.employee.FirstOrDefault(e => e.login == "service");
            role = _mockContext.Object.Role.FirstOrDefault(r => r.role_name == "operator");
            var result = Validation.EditUserValidation("qwera","", role, emp, _mockContext.Object);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void EditUserValidation_PasswordMenshe_ReturnsError()
        {
            emp = _mockContext.Object.employee.FirstOrDefault(e => e.login == "service");
            role = _mockContext.Object.Role.FirstOrDefault(r => r.role_name == "operator");
            var result = Validation.EditUserValidation("qwera", "134567e", role, emp, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Должно быть не меньше 8 символов", result.error);
        }
        [Test]
        public void EditUserValidation_PasswordOnlyNums_ReturnsError()
        {
            emp = _mockContext.Object.employee.FirstOrDefault(e => e.login == "service");
            role = _mockContext.Object.Role.FirstOrDefault(r => r.role_name == "operator");
            var result = Validation.EditUserValidation("qwera", "12345678", role, emp, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Пароль должен содержать хотя бы одну букву!", result.error);
        }
        [Test]
        public void EditUserValidation_PasswordOnlyLetters_ReturnsError()
        {
            emp = _mockContext.Object.employee.FirstOrDefault(e => e.login == "service");
            role = _mockContext.Object.Role.FirstOrDefault(r => r.role_name == "operator");
            var result = Validation.EditUserValidation("qwera", "qwertyuio", role, emp, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Пароль должен содержать хотя бы одну цифру!", result.error);
        }
        [Test]
        public void CreateSposobValidation_DuplicateName_ReturnsError()
        {
            var result = Validation.CreateSposobValidation("Поезд", _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Такой способ доставки уже существует!", result.error);
        }
        [Test]
        public void CreateSposobValidation_NewName_ReturnsError()
        {
            var result = Validation.CreateSposobValidation("что-то там", _mockContext.Object);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void EditSposobValidation_NewName_ReturnsError()
        {
            sposob = _mockContext.Object.Sposob_otpravki.FirstOrDefault(s => s.name=="Курьер");
            var result = Validation.EditSposobValidation("Отправка как-то", sposob, _mockContext.Object);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void EditSposobValidation_DuplicateName_ReturnsError()
        {
            sposob = _mockContext.Object.Sposob_otpravki.FirstOrDefault(s => s.name == "Курьер");
            var result = Validation.EditSposobValidation("Поезд", sposob, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Такой способ доставки уже существует!", result.error);
        }
        [Test]
        public void CreateServiceValidation_NewName_ReturnsError()
        {
            var result = Validation.CreateServiceValidation("что-то там", _mockContext.Object);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void CreateServiceValidation_duplicateName_ReturnsError()
        {
            var result = Validation.CreateServiceValidation("Отправка письма", _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Такая услуга уже существует!", result.error);
        }
        [Test]
        public void EditServiceValidation_duplicateName_ReturnsError()
        {
            service = _mockContext.Object.services.FirstOrDefault(ser => ser.name == "Отправка письма");
            var result = Validation.EditServiceValidation("Отправка посылки", service, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Такая услуга уже существует!", result.error);
        }
        [Test]
        public void EditServiceValidation_NewName_ReturnsError()
        {
            service = _mockContext.Object.services.FirstOrDefault(ser => ser.name == "Отправка письма");
            var result = Validation.EditServiceValidation("аааааааааааааааааа", service, _mockContext.Object);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void CreateClientValidation_NewPhonePassport_ReturnsError()
        {
            var result = Validation.CreateClientValidation("123", "00000", _mockContext.Object);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void CreateClientValidation_NewPhoneDuplicatePassport_ReturnsError()
        {
            var result = Validation.CreateClientValidation("123", "3456 6554", _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Клиент с такими паспортными данными уже существует!", result.error);
        }
        [Test]
        public void CreateClientValidation_NewPassportDuplicatePhone_ReturnsError()
        {
            var result = Validation.CreateClientValidation("79062345487", "1111110", _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Клиент с таким номером телефона уже существует!", result.error);
        }
        [Test]
        public void EditClientValidation_NewPhonePassport_ReturnsError()
        {
            client = _mockContext.Object.Client.FirstOrDefault(c => c.name == "Михаил");
            var result = Validation.EditClientValidation("123", "00000", client, _mockContext.Object);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.error);
        }
        [Test]
        public void EditClientValidation_NewPhoneDuplicatPassport_ReturnsError()
        {
            client = _mockContext.Object.Client.FirstOrDefault(c => c.name == "Михаил");
            var result = Validation.EditClientValidation("123", "1234 5678", client, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Клиент с такими паспортными данными уже существует!", result.error);
        }
        [Test]
        public void EditClientValidation_NewPassportDuplicatePhone_ReturnsError()
        {
            client = _mockContext.Object.Client.FirstOrDefault(c => c.name == "Михаил");
            var result = Validation.EditClientValidation("79165554433", "3412 0000", client, _mockContext.Object);
            Assert.IsFalse(result.isValid);
            Assert.AreEqual("Клиент с таким номером телефона уже существует!", result.error);
        } 
    }
}