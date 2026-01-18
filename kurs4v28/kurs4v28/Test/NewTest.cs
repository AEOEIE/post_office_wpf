using NUnit.Framework;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace kurs4v28.Test
{
    [TestFixture]
    internal class NewTest
    {
        private MockRepository mockRepository;
        private string testFilePath;
        private string testDirectory;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            testDirectory = Path.Combine(Path.GetTempPath(), "Test");
            testFilePath = Path.Combine(testDirectory, "rememberme.dat");
            var field = typeof(RememberMe).GetField("FilePath",
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic);
            field.SetValue(null, testFilePath);
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }
        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }

            mockRepository.VerifyAll();
        }
        [Test]
        public void SaveCredentials_ShouldCreateFileWithEncodedData()
        {
            string login = "testuser";
            string password = "testpass123";
            RememberMe.SaveCredentials(login, password);
            Assert.IsTrue(File.Exists(testFilePath), "Файл должен быть создан");
            string encoded = File.ReadAllText(testFilePath);
            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            var parts = decoded.Split('|');
            Assert.AreEqual(2, parts.Length, "Должно быть 2 части данных");
            Assert.AreEqual(login, parts[0], "Логин должен сохраниться правильно");
            Assert.AreEqual(password, parts[1], "Пароль должен сохраниться правильно");
        }
        [Test]
        public void SaveCredentials_ShouldCreateDirectoryIfNotExists()
        {
            string login = "user1";
            string password = "pass1";
            RememberMe.SaveCredentials(login, password);
            Assert.IsTrue(Directory.Exists(testDirectory), "Директория должна быть создана");
            Assert.IsTrue(File.Exists(testFilePath), "Файл должен быть создан");
        }
        [Test]
        public void SaveCredentials_ShouldHandleExceptionGracefully()
        {
            string login = "user";
            string password = "pass";
            var field = typeof(RememberMe).GetField("FilePath",
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic);
            field.SetValue(null, "?:\\invalid\\path\\file.dat");
            Assert.DoesNotThrow(() => RememberMe.SaveCredentials(login, password));
            field.SetValue(null, testFilePath);
        }
        [Test]
        public void LoadCredentials_ShouldReturnNullWhenFileNotExists()
        {
            var result = RememberMe.LoadCredentials();
            Assert.IsNull(result.login, "Логин должен быть null при отсутствии файла");
            Assert.IsNull(result.password, "Пароль должен быть null при отсутствии файла");
        }

        [Test]
        public void LoadCredentials_ShouldReturnCredentialsWhenFileExists()
        {
            string login = "admin";
            string password = "admin123";
            RememberMe.SaveCredentials(login, password);
            var result = RememberMe.LoadCredentials();
            Assert.AreEqual(login, result.login, "Логин должен загрузиться правильно");
            Assert.AreEqual(password, result.password, "Пароль должен загрузиться правильно");
        }
        [Test]
        public void LoadCredentials_ShouldReturnNullForInvalidFileFormat()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(testFilePath));
            string invalidData = "not-base64-format";
            File.WriteAllText(testFilePath, invalidData);
            var result = RememberMe.LoadCredentials();
            Assert.IsNull(result.login, "Логин должен быть null при неверном формате");
            Assert.IsNull(result.password, "Пароль должен быть null при неверном формате");
        }
        [Test]
        public void LoadCredentials_ShouldReturnNullForInvalidDataFormat()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(testFilePath));
            string data = "only-one-part";
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
            File.WriteAllText(testFilePath, encoded);
            var result = RememberMe.LoadCredentials();
            Assert.IsNull(result.login, "Логин должен быть null при неверных данных");
            Assert.IsNull(result.password, "Пароль должен быть null при неверных данных");
        }
        [Test]
        public void LoadCredentials_ShouldHandleExceptionGracefully()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(testFilePath));
            File.WriteAllText(testFilePath, "test");
            using (var file = File.Open(testFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                var result = RememberMe.LoadCredentials();
                Assert.IsNull(result.login, "Должен вернуть null при ошибке");
                Assert.IsNull(result.password, "Должен вернуть null при ошибке");
            }
        }
        [Test]
        public void ClearCredentials_ShouldDeleteFileWhenExists()
        {
            RememberMe.SaveCredentials("user", "pass");
            Assert.IsTrue(File.Exists(testFilePath), "Файл должен существовать перед очисткой");
            RememberMe.ClearCredentials();
            Assert.IsFalse(File.Exists(testFilePath), "Файл должен быть удален");
        }
        [Test]
        public void ClearCredentials_ShouldNotThrowWhenFileNotExists()
        {
            Assert.IsFalse(File.Exists(testFilePath), "Файл не должен существовать");
            Assert.DoesNotThrow(() => RememberMe.ClearCredentials());
        }
        [Test]
        public void ClearCredentials_ShouldHandleExceptionGracefully()
        {
            RememberMe.SaveCredentials("user", "pass");
            using (var file = File.Open(testFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Assert.DoesNotThrow(() => RememberMe.ClearCredentials());
            }
        }
        [Test]
        public void IntegrationTest_SaveThenLoad_ShouldWorkCorrectly()
        {
            string login = "integrationuser";
            string password = "integrationpass!@#";
            RememberMe.SaveCredentials(login, password);
            var loaded = RememberMe.LoadCredentials();
            Assert.AreEqual(login, loaded.login, "Логин должен сохраниться и загрузиться");
            Assert.AreEqual(password, loaded.password, "Пароль должен сохраниться и загрузиться");
        }
        [Test]
        public void IntegrationTest_SaveClearLoad_ShouldReturnNull()
        {
            string login = "tempuser";
            string password = "temppass";
            RememberMe.SaveCredentials(login, password);
            RememberMe.ClearCredentials();
            var loaded = RememberMe.LoadCredentials();
            Assert.IsNull(loaded.login, "Логин должен быть null после очистки");
            Assert.IsNull(loaded.password, "Пароль должен быть null после очистки");
        }
        [Test]
        public void SaveCredentials_ShouldOverwriteExistingFile()
        {
            RememberMe.SaveCredentials("olduser", "oldpass");
            string originalContent = File.ReadAllText(testFilePath);
            RememberMe.SaveCredentials("newuser", "newpass");
            string newContent = File.ReadAllText(testFilePath);
            Assert.AreNotEqual(originalContent, newContent, "Файл должен быть перезаписан");
            var loaded = RememberMe.LoadCredentials();
            Assert.AreEqual("newuser", loaded.login, "Должен загрузиться новый логин");
            Assert.AreEqual("newpass", loaded.password, "Должен загрузиться новый пароль");
        }
        [Test]
        public void LoadCredentials_ShouldHandleEmptyFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(testFilePath));
            File.WriteAllText(testFilePath, "");
            var result = RememberMe.LoadCredentials();
            Assert.IsNull(result.login, "Должен вернуть null для пустого файла");
            Assert.IsNull(result.password, "Должен вернуть null для пустого файла");
        }
        [Test]
        public void SaveCredentials_ShouldHandleSpecialCharacters()
        {
            string login = "user@domain.com";
            string password = "p@ssw0rd!№;%:?*()";
            RememberMe.SaveCredentials(login, password);
            var loaded = RememberMe.LoadCredentials();
            Assert.AreEqual(login, loaded.login, "Спецсимволы в логине должны сохраниться");
            Assert.AreEqual(password, loaded.password, "Спецсимволы в пароле должны сохраниться");
        }
    }
}
