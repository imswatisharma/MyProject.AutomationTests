using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace MyProject.AutomationTests
{
    [TestClass]
    public class ValidateLoginTest
    {
        private IWebDriver webDriver;

        //Get url and login details from input file
        private string url = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\ValidLoginInfo.txt")).First();
        private string validUsername = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\ValidLoginInfo.txt")).ElementAtOrDefault(1);
        private string validPassword = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\ValidLoginInfo.txt")).ElementAtOrDefault(2);
        private string invalidUsername = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\InvalidLoginInfo.txt")).ElementAtOrDefault(1);
        private string invalidPassword = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\InvalidLoginInfo.txt")).ElementAtOrDefault(2);

        private By cookies = By.ClassName("coi-banner__accept"); // To accept cookies
        private IWebElement BtnCookies => webDriver.FindElement(cookies);
        private By loginLink = By.ClassName("menulist-link"); // To go to login page
        private IWebElement LoginLink => webDriver.FindElement(loginLink);

        private By email = By.Id("Email"); // To get email text field
        private IWebElement Email => webDriver.FindElement(email);

        private By password = By.Id("password"); // To get password text field
        private IWebElement Password => webDriver.FindElement(password);

        private By loginSubmit = By.Id("login-submit"); // To get login submit button
        private IWebElement BtnLoginSubmit => webDriver.FindElement(loginSubmit);

        private By profile = By.CssSelector("a[href='/profile']"); // Upon successful login, profile is reached 

        [TestInitialize]
        public void Setup()
        {
            webDriver = new ChromeDriver(); // To launch Chrome
            webDriver.Manage().Window.Maximize(); // To maximize browser window
            webDriver.Navigate().GoToUrl(url); // To go to url
        }

        [TestMethod]
        public void TestValidLogin()
        {
            HandleCookiesAndGoToLoginPage();
            LoginWithDetails(validUsername, validPassword); // Login with valid login details
            Assert.IsTrue(Exists(profile)); // Test is success because profile element is found with valid login
        }

        [TestMethod]
        public void TestInvalidLogin()
        {
            HandleCookiesAndGoToLoginPage();
            LoginWithDetails(invalidUsername, invalidPassword); // Login with invalid login details
            Assert.IsFalse(Exists(profile)); // Test is success because profile element is not found with invalid login
        }

        private void HandleCookiesAndGoToLoginPage()
        {
            Thread.Sleep(4000); // To sleep so that cookies appear first to deal with
            BtnCookies.Click(); // Accept cookies
            Thread.Sleep(1000); // To sleep so that all required webelements appear
            LoginLink.Click(); // Go to login form
        }

        private void LoginWithDetails(string email, string password)
        {
            Email.SendKeys(email); // Fill email text field
            Password.SendKeys(password); // Fill password text field
            BtnLoginSubmit.Click(); // Click on login
        }

        private bool Exists(By by)
        {
            if (webDriver.FindElements(by).Count != 0) // To find if any element exists on the page
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [TestCleanup]
        public void Cleanup()
        {
            //webDriver.Quit(); // Can be enabled if we want to close browser windows & terminate session
        }

    }
}
