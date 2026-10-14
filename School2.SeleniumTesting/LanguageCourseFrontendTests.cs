using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Xunit;

namespace School2.SeleniumTesting;

public class LanguageCourseFrontendTests
{
    private static IWebDriver CreateDriver()
    {
        IWebDriver driver = new FirefoxDriver(); // brauserit juhtiv ja seal tegutsev draiver
        driver.Url = "http://localhost:5117/"; // navigeeritav aadress
        
        Thread.Sleep(1000);
        
        return driver;
    }

    private static void NavigateToIndex( IWebDriver driver )
    {
        IWebElement navigateToLanguageCourse = driver.FindElement(By.LinkText("Keelekursused")); // otsime tesitava osa lingi
        navigateToLanguageCourse.Click(); // simuleeritud tegevus elemendiga, antud juhul klikk
    }
    
    [Fact]
    public void Should_NavigateToIndex_AddLanguageCourseWithCorrectData_ReturnToIndex()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex(driver);
        
        /* tegevus */
        IWebElement indexCreateBtn = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Btn"));
        indexCreateBtn.Click();
        
        DataEntryLanguageCourse(driver);

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

    private static void DataEntryLanguageCourse(IWebDriver driver, bool isOneOrTwo = false)
    {
        if (!isOneOrTwo)
        {
            IWebElement nimetusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            nimetusEntry.SendKeys("Automaattestimise kursus");
        
            IWebElement keelEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            keelEntry.SendKeys("C# + Selenium & xUnit");
        
            IWebElement taseEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            taseEntry.SendKeys("3");
        
            IWebElement kirjeldusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            kirjeldusEntry.SendKeys("Kursusel õpime mis on automaattestimine, kuidas erineb manuaaltestimisest ning kuidas kirjutada backendi ja frontendi automaatteste.");
        }
        else
        {
            IWebElement nimetusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            nimetusEntry.Clear();
            nimetusEntry.SendKeys("Manuaaltestimise kursus");
        
            IWebElement keelEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            keelEntry.Clear();
            keelEntry.SendKeys("Mantis, Excel");
        
            IWebElement taseEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            taseEntry.Clear();
            taseEntry.SendKeys("3");
        
            IWebElement kirjeldusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            kirjeldusEntry.Clear();
            kirjeldusEntry.SendKeys("Kursusel õpime mis on manuaaltestimise, kuidas erineb automaattestimisest ning mis dokumentatsioon manuaaltestimise juures tähtis on ning kuidas neid kirjutada.");
        }
    }

    private static void DataEntryLanguageCourseClear(IWebDriver driver)
    {
        driver.FindElement(By.Id("CU_LanguageCourse_Nimetus")).Clear();
        driver.FindElement(By.Id("CU_LanguageCourse_Keel")).Clear();
        driver.FindElement(By.Id("CU_LanguageCourse_Tase")).Clear();
        driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus")).Clear();
    }

    [Fact]
    public void Should_NavigateToDetailsOfALanguageCourse_WithPreviouslyCorrectData()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex(driver);
        
        var table =  driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Table"));

        List<IWebElement> rowsOfTable = table.FindElements(By.TagName("tr")).ToList();
        rowsOfTable.RemoveAt(0);

        bool isObjectFound = false;
        
        /* tegevus */
        foreach (var row in rowsOfTable)
        {
            var nimetusElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            var keelElement = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
            if ((nimetusElement == "Automaattestimise kursus") && (keelElement == "C# + Selenium & xUnit"))
            {
                isObjectFound = true;
                var detailsBtn = row
                    .FindElement(By.Id("IndexTableCourseActions"))
                    .FindElement(By.Id("IndexTableCourseDetails_Btn"));
                
                detailsBtn.Click();
                
                break;
            }
        }
        
        Thread.Sleep(1000);
        
        string detailsId = driver.FindElement(By.Id("DD_LanguageCourseGuid")).Text;
        string detailsNimetus = driver.FindElement(By.Id("DD_LanguageCourseNimetus")).Text;
        string detailsKeel = driver.FindElement(By.Id("DD_LanguageCourseKeel")).Text;
        string detailsKirjeldus = driver.FindElement(By.Id("DD_LanguageCourseKirjeldus")).Text;
        string detailsTase = driver.FindElement(By.Id("DD_LanguageCourseTase")).Text;
        
        driver.FindElement(By.Id("DD_DetailsBack_Btn")).Click();
        Thread.Sleep(1000);
        driver.Quit();
        
        /* kontroll */
        Assert.True(isObjectFound);
        Assert.NotEmpty(detailsId);
        Assert.NotEmpty(detailsNimetus);
        Assert.NotEmpty(detailsKeel);
        Assert.NotEmpty(detailsKirjeldus);
        Assert.NotEmpty(detailsTase);
        Assert.Equal("Automaattestimise kursus", detailsNimetus);
        Assert.Equal("C# + Selenium & xUnit", detailsKeel);
    }

    [Fact]
    public void Should_UpdateDetailsOfLanguageCoures_WithNewData()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex(driver);
        
        var table =  driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Table"));

        List<IWebElement> rowsOfTable = table.FindElements(By.TagName("tr")).ToList();
        rowsOfTable.RemoveAt(0);

        bool isObjectFound = false;
        
        /* tegevus */
        foreach (var row in rowsOfTable)
        {
            var nimetusElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            var keelElement = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
            if ((nimetusElement == "Automaattestimise kursus") && (keelElement == "C# + Selenium & xUnit"))
            {
                isObjectFound = true;
                var updateBtn = row
                    .FindElement(By.Id("IndexTableCourseActions"))
                    .FindElement(By.Id("IndexTableCourseUpdate_Btn"));
                
                updateBtn.Click();
                
                break;
            }
        }

        DataEntryLanguageCourseClear(driver);
        DataEntryLanguageCourse(driver, true);
        Thread.Sleep(1000);
        
        driver.FindElement(By.Id("CU_Update_Btn")).Click();
        
        string detailsId = driver.FindElement(By.Id("CU_LanguageCourse_Guid")).GetAttribute("value");
        string detailsNimetus = driver.FindElement(By.Id("CU_LanguageCourse_Nimetus")).GetAttribute("value");
        string detailsKeel = driver.FindElement(By.Id("CU_LanguageCourse_Keel")).GetAttribute("value");
        string detailsKirjeldus = driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus")).GetAttribute("value");
        string detailsTase = driver.FindElement(By.Id("CU_LanguageCourse_Tase")).GetAttribute("value");
        
        Thread.Sleep(1000);
        driver.FindElement(By.Id("CU_Back_Btn")).Click();
        Thread.Sleep(1000);
        driver.Quit();
        
        /* kontroll */
        Assert.True(isObjectFound);
        Assert.NotNull(detailsId);
        Assert.NotEmpty(detailsId);
        Assert.Equal("Manuaaltestimise kursus", detailsNimetus);
        Assert.Equal("Mantis, Excel", detailsKeel);
        Assert.Equal("3", detailsTase);
        Assert.Equal("Kursusel õpime mis on manuaaltestimise, kuidas erineb automaattestimisest ning mis dokumentatsioon manuaaltestimise juures tähtis on ning kuidas neid kirjutada.", detailsKirjeldus);
    }
}