using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using NUnit.Framework;

namespace kurs4v28.Test.RealizationAllFeatures
{
    [Binding]
    public class AuthFlow
    {
        private List<TestUser> _users = new List<TestUser>();
        private string _currentWindow = "MainWindow";
        private bool _createUserWindowOpened = false;
        private bool _createUserWindowClosed = false;
        public class TestUser
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }
        [Given(@"пользователь admin с паролем 12345678 существует в системе")]
        public void GivenAdminUserExists()
        {
            _users.Add(new TestUser
            {
                Login = "admin",
                Password = "12345678",
                Role = "admin"
            });
        }
        [When(@"я вхожу с логином ""(.*)"" и паролем ""(.*)""")]
        public void WhenILoginWithCredentials(string login, string password)
        {
            var user = _users.FirstOrDefault(u => u.Login == login);

            if (user != null && user.Password == password)
            {
                _currentWindow = "AdminWindow";
            }
            else
            {
                _currentWindow = "MainWindow";
            }
        }

        [Then(@"открывается окно AdminWindow")]
        public void ThenAdminWindowOpens()
        {
            Assert.AreEqual("AdminWindow", _currentWindow, "Должно открыться окно AdminWindow!");
        }

        [When(@"я нажимаю кнопку ""(.*)""")]
        public void WhenIClickButton(string buttonName)
        {
            if (buttonName == "Добавить пользователя" && _currentWindow == "AdminWindow")
            {
                _createUserWindowOpened = true;
                _currentWindow = "CreateUserWindow";
            }
            else if (buttonName == "Выйти" && _currentWindow == "AdminWindow")
            {
                _currentWindow = "MainWindow";
            }
        }

        [Then(@"открывается окно CreateUserWindow")]
        public void ThenCreateUserWindowOpens()
        {
            Assert.IsTrue(_createUserWindowOpened, "Окно создания пользователя должно открыться!");
            Assert.AreEqual("CreateUserWindow", _currentWindow);
        }

        [When(@"я закрываю окно CreateUserWindow")]
        public void WhenICloseCreateUserWindow()
        {
            if (_currentWindow == "CreateUserWindow")
            {
                _createUserWindowClosed = true;
                _currentWindow = "AdminWindow";
            }
        }

        [Then(@"я возвращаюсь в окно AdminWindow")]
        public void ThenIReturnToAdminWindow()
        {
            Assert.IsTrue(_createUserWindowClosed, "Окно создания пользователя должно закрыться!");
            Assert.AreEqual("AdminWindow", _currentWindow);
        }
        [When(@"я нажимаю кнопку ""(.*)"" в AdminWindow")]
        public void WhenЯНажимаюКнопкуВAdminWindow(string buttonName)
        {
            if (buttonName == "Выйти" && _currentWindow == "AdminWindow")
            {
                _currentWindow = "MainWindow";
            }
        }
        [Then(@"открывается окно MainWindow")]
        public void ThenMainWindowOpens()
        {
            Assert.AreEqual("MainWindow", _currentWindow, "Должно открыться главное окно!");
        }
    }
}
