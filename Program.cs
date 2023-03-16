using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Threading;

namespace CookieChecker
{
    class Program
    {        
        static void Main(string[] args)
        {
            //Put the cookies you want to know about here!
            var approvedCookies = new List<string> { "French Toast", "Mystery Cookie" };
            var options = new ChromeOptions();
            var driver = new ChromeDriver();            
            var flavors = new List<string>();            

            Console.WriteLine("Opening browser...");
            driver.Navigate().GoToUrl("https://crumblcookies.com/");
            var ul = driver.FindElement(By.Id("weekly-cookie-flavors"));
            var webUl = ul.FindElements(By.XPath("./li"));

            Console.WriteLine("Gathering flavors...");
            foreach(var li in webUl)
            {
                var flavor = li.Text;
                var lines = Regex.Split(flavor, "\r\n");                
                flavors.Add(lines[0]);

            }

            Console.WriteLine("Flavors are:");
            foreach(var flavor in flavors)
            {
                Console.WriteLine(flavor);
            }

            SaveFlavors(flavors);

            var flavorMatches = approvedCookies.Intersect(flavors);
            if(flavorMatches.Count() > 0)
            {
                int attempts = 0;
                Console.WriteLine("Hot Dog! We have a winner!");
                try
                {
                    Notify(driver, flavorMatches);
                } catch
                {
                    attempts++;
                    if(attempts > 2)
                    {
                        Console.WriteLine("Notify failed.");
                    } else
                    {
                        Notify(driver, flavorMatches);
                    }

                }
            }
            
        }

        public static void Notify(ChromeDriver driver, IEnumerable<string> flavorMatches)
        {
            //Put your email address and password created in outlook that will do the sending here:
            var senderEmail = "thecookietracker@outlook.com";
            var senderPassword = "Suzuki12!";

            //Put the email address that you want to recieve notifications on here:
            var receiverEmail = "frazier.maxwell.t@gmail.com";

            driver.Navigate().GoToUrl("https://outlook.com/");
            Thread.Sleep(1000);
            var signInButton = driver.FindElement(By.XPath("//a[@data-task='signin']"));            
            signInButton.Click();            
            var emailBox = driver.FindElement(By.XPath("//input[@type='email']"));            
            emailBox.SendKeys(senderEmail);
            var nextButton = driver.FindElement(By.XPath("//input[@type='submit']"));
            nextButton.Click();
            Thread.Sleep(1000);
            var passwordBox = driver.FindElement(By.XPath("//input[@type='password']"));
            passwordBox.SendKeys(senderPassword);
            var otherSignInButton = driver.FindElement(By.XPath("//input[@type='submit']"));
            otherSignInButton.Click();
            var yesButton = driver.FindElement(By.XPath("//input[@type='submit']"));
            yesButton.Click();
            Thread.Sleep(2000);
            var newEmailButton = driver.FindElement(By.XPath("//button[@aria-label='New mail']"));
            int attempts = 0;                                 
            while(attempts < 2)
            {
                try
                {
                    newEmailButton.Click();
                    break;
                } catch(StaleElementReferenceException e)
                {
                    attempts++;
                }
            }            
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("window.focus();");
            var toLine = driver.FindElement(By.XPath("//div[@aria-label='To']"));
            toLine.SendKeys(receiverEmail);
            var subjectLine = driver.FindElement(By.XPath("//input[@aria-label='Add a subject']"));
            subjectLine.SendKeys("Your chosen flavor(s) are available at Crumbl Today!");
            var body = driver.FindElement(By.XPath("//div[@aria-label='Message body, press Alt+F10 to exit']"));
            body.SendKeys("The following flavors are available today: ");
            foreach(var flavor in flavorMatches)
            {
                if(flavorMatches.LastOrDefault() == flavor )
                {
                    body.SendKeys(flavor + ".");
                } else
                {
                    body.SendKeys(flavor + ", ");
                }
                
            }
            var sendButton = driver.FindElement(By.XPath("//button[@aria-label='Send']"));
            sendButton.Click();
            Thread.Sleep(1000);
            Console.WriteLine("Email sent successfully");
            driver.Close();
            Environment.Exit(0);
        }

        public static void SaveFlavors(List<string> flavors)
        {
            string filePath = @"D:\Cookies\" + DateTime.Now.ToString("d") + " Flavors.txt";
            filePath = filePath.Replace("/", "-");
            var sw = new StreamWriter(filePath, true);
            foreach(var flavor in flavors)
            {
                sw.WriteLine(flavor);
            }
            sw.Close();
        }
    }
}
