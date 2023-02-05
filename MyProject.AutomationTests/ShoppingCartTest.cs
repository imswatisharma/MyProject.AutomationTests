using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.AutomationTests
{
    [TestClass]
    public class ShoppingCartTest
    {
        private IWebDriver webDriver;

        //Get url, login details and product keyword to search from input file
        private string url = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\ValidLoginInfo.txt")).First();
        private string validUsername = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\ValidLoginInfo.txt")).ElementAtOrDefault(1);
        private string validPassword = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\ValidLoginInfo.txt")).ElementAtOrDefault(2);
        private string productKeywordToSearch = File.ReadLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\ProductToSearch.txt")).First();

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
        private IWebElement Profile => webDriver.FindElement(profile);

        private By query = By.Id("query"); // To get search text box
        private IWebElement Query => webDriver.FindElement(query);

        private By suggestions = By.CssSelector(".col-xs-12.col-md-8.br-r-md.mv5.ng-scope"); // To get parent element of suggested items when you write something in search textbox
        private By firstSuggestion = By.XPath("//*[@id=\"commonController\"]/div[2]/header/div/div[3]/div/div[2]/div[1]"); // To get first suggested item
        private IWebElement FirstSuggestion => webDriver.FindElement(firstSuggestion);
        private By span = By.TagName("span"); // To get product name which you can use to verify later
        string firstSuggestedProductName; // To save the first suggested product name which you're adding and need later to validate with the cart content
        private By addToCartButton = By.CssSelector(".fas.fa-shopping-basket"); // To get the Add to cart button
        private IWebElement BtnAddToCart => FirstSuggestion.FindElement(addToCartButton);
        private By primaryTextAfterAddition = By.CssSelector(".h2.text-primary.mn"); // To get the webelement containing the message after addition of item in the cart
        private IWebElement PrimaryTextAfterAddition => webDriver.FindElement(primaryTextAfterAddition);
        private By goToCartPage = By.CssSelector(".btn.btn-primary.btn-lg"); // To get the webelement to go to cart page
        private IWebElement GoToCartPage => webDriver.FindElement(goToCartPage);
        private By orderedItemCount = By.CssSelector(".btn.btn-default.btn-sm.ng-binding"); // To get the number of items in the cart
        private IWebElement OrderedItemCount => webDriver.FindElement(orderedItemCount);
        private By productInfoInCart = By.CssSelector(".inline-block.mn.mr20-sm"); // To get the webelement related to item info in the cart
        private IWebElement ProductInfoInCart => webDriver.FindElement(productInfoInCart);
        private By removeItemFromCart = By.CssSelector(".remove-btn.lh24.ml30"); // To get the button to remove item from the cart
        private IWebElement BtnRemoveItemFromCart => webDriver.FindElement(removeItemFromCart);
        private By cartEmpty = By.ClassName("cart_empty"); // To get the class which appears only if cart is empty
        private IWebElement CartEmpty => webDriver.FindElement(cartEmpty);
        private By cartEmptyTextElement = By.XPath("//*[@id=\"order\"]/main/div[2]/div/h3"); // To get the webelement with message related to empty cart
        private IWebElement CartEmptyTextElement => webDriver.FindElement(cartEmptyTextElement);

        [TestInitialize]
        public void Setup()
        {
            webDriver = new ChromeDriver(); // To launch Chrome
            webDriver.Manage().Window.Maximize(); // To maximize browser window
            webDriver.Navigate().GoToUrl(url); // To go to url
            HandleCookiesAndGoToLoginPage();
            LoginWithDetails(validUsername, validPassword); // Login with valid details
        }

        [TestMethod]
        public void ValidateCart()
        {
            Assert.IsTrue(Exists(profile)); // Verify that login was successful before proceeding
            Query.SendKeys(productKeywordToSearch); // Searching for an item from a keyword in the input file
            Thread.Sleep(4000); // Either Sleep or make webDriver wait until suggestions come for the entered keyword
            Assert.IsTrue(Exists(suggestions)); // Validate that some suggestions come before proceeding
            Assert.IsTrue(Exists(firstSuggestion)); // Validate that you can get the first suggested item
            firstSuggestedProductName = FirstSuggestion.FindElement(span).Text; // To get name of first suggested product
            BtnAddToCart.Click(); // Add item to cart
            Thread.Sleep(4000);// Either Sleep or make webDriver Wait until item added to cart before validating next items
            Assert.IsTrue(Exists(primaryTextAfterAddition));
            Assert.IsTrue(PrimaryTextAfterAddition.Text.Equals(firstSuggestedProductName + " er tilføjet til din kurv"));
            // To validate that item addition to cart was successful, verified that ' er tilføjet til din kurv' comes on webpage
            // as well as the same product is added to the cart for which 'add to cart' was clicked by comparing product name
            GoToCartPage.Click();
            Thread.Sleep(4000);
            Assert.IsTrue(OrderedItemCount.Text.Equals("1")); // Validating that cart contains only 1 item
            Assert.IsTrue(firstSuggestedProductName.Equals(ProductInfoInCart.FindElement(span).Text));
            //Validating that the name of product in cart is the same as product which was added
            BtnRemoveItemFromCart.Click(); // Remove item from cart
            Thread.Sleep(4000); // Sleep required to get the item deleted from cart and generate required webelements
            Assert.IsTrue(Exists(cartEmpty)); // Validating that the cart is empty because this webelement exists only when cart is empty
            Assert.IsTrue(CartEmptyTextElement.Text.Equals("Din kurv er tom")); // Also validating this message on the cartEmpty webelement
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
