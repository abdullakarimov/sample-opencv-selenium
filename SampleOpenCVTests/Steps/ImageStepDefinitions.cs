using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow.NUnit;
using NUnit.Framework;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Net;

namespace SampleOpenCVTest.Steps
{
    [Binding]
    public class ImageStepDefinitions
    {
        private readonly ScenarioContext context;
        public ImageStepDefinitions(ScenarioContext scenarioContext)
        {
            context = scenarioContext;
        }
        private readonly string tdlUrl = "https://www.testdevlab.com/";
        IWebDriver driver;
        public void ClickAndWaitForPageToLoad(By elementLocator, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                var element = driver.FindElement(elementLocator);
                element.Click();                
                wait.Until(c => !element.Displayed);
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Element with locator: '" + elementLocator + "' was not found in current context page.");
                throw;
            }
        }

        [Given("that I am on the Main page")]
        public void GivenUserOnPage()
        {
            driver = new ChromeDriver()
            {
                Url = tdlUrl
            };
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);
            Assert.That(driver.Title.Contains("TestDevLab"));
        }

        [Then("I hover over the (.*) link")]
        public void HoverOverLink(string input)
        {
            Actions action = new Actions(driver);
            IWebElement linkElement = driver.FindElement(By.XPath("//div[@class='title no-select' and text()='" + input + "']"));
            action.MoveToElement(linkElement).Perform();
            Assert.That(driver.FindElement(By.XPath("//div[text()='Mobile application testing']")).Displayed);
        }

        [Then("I click the (.*) link")]
        public void ClickLink(string input)
        {
            Actions action = new Actions(driver);
            IWebElement linkElement = driver.FindElement(By.XPath("//div[text()='" + input + "']"));
            action.MoveToElement(linkElement).Perform();
            ClickAndWaitForPageToLoad(By.XPath("//div[text()='" + input + "']"));
        }

        [Then("I verify that the tab title contains (.*)")]
        public void VerifyTitle(string input)
        {
            Assert.That(driver.Title.Contains(input));
        }

        [Then("I compare the first image on the page with the base image")]
        public void CompareImages()
        {
            IWebElement imageElement = driver.FindElement(By.XPath("//div[contains(@class, 'manual-t-photo')]"));
            string imageUrl = imageElement.GetCssValue("background-image").Replace("url(\"", "").Replace("\")", "");
            string solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            WebClient webClient = new WebClient();
            try
            {
                webClient.DownloadFile(imageUrl, solutionDir + "\\Images\\temp.jpg");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Image<Gray, Byte> imgToCompare = new Image<Gray, byte>(solutionDir + "\\Images\\temp.jpg");
            Image<Gray, Byte> baseImg = new Image<Gray, byte>(solutionDir + "\\Images\\mobileqa.jpg");
            Assert.That(imgToCompare.Equals(baseImg));
        }

        [Then("I close the browser")]
        public void ThenCloseBrowser()
        {
            driver.Quit();
        }
    }
}
