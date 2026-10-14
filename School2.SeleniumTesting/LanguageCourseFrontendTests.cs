using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Xunit;

namespace School2.SeleniumTesting;

public class LanguageCourseFrontendTests
{
    [Fact]
    public void Should_NavigateToIndex_AddLanguageCourseWithCorrectData_ReturnToIndex()
    {
        /* ülesseade */
        IWebDriver driver = new FirefoxDriver(); // brauserit juhtiv ja seal tegutsev draiver
        driver.Url = "http://localhost:5117/"; // navigeeritav aadress

        Thread.Sleep(3000);
        
        IWebElement navigateToLanguageCourse = driver.FindElement(By.LinkText("Keelekursused")); // otsime tesitava osa lingi
        navigateToLanguageCourse.Click(); // simuleeritud tegevus elemendiga, antud juhul klikk
        
        /* tegevus */
        IWebElement indexCreateBtn = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Btn"));
        indexCreateBtn.Click();
        
        IWebElement nimetusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
        nimetusEntry.SendKeys("Automaattestimise kursus");
        
        IWebElement keelEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
        keelEntry.SendKeys("C# + Selenium & xUnit");
        
        IWebElement taseEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
        taseEntry.SendKeys("3");
        
        IWebElement kirjeldusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
        kirjeldusEntry.SendKeys("Kursusel õpime mis on automaattestimine, kuidas erineb manuaaltestimisest ning kuidas kirjutada backendi ja frontendi automaatteste.");
        
        IWebElement createBtn = driver.FindElement(By.Id("CU_Create_Btn"));
        createBtn.Click();
        
        /* kontroll */
        
        //kogune andmed
        string languageCourseNimetusData = driver.FindElement(By.Id("IndexTableCourseNimetus")).Text;
        string languageCourseKeelData = driver.FindElement(By.Id("IndexTableCourseKeel")).Text;
        
        Thread.Sleep(500);
        driver.Quit();
        
        //assertime
        Assert.Equal("Automaattestimise kursus", languageCourseNimetusData);
        Assert.Equal("C# + Selenium & xUnit", languageCourseKeelData);
        Assert.True(("Automaattestimise kursus" == languageCourseNimetusData) && ("C# + Selenium & xUnit" == languageCourseKeelData));
    }
}