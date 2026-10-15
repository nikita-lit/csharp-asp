using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace School2.SeleniumTesting;

// Järjesti klass, mis omab omadusi mille järgi teste saab järjestada,
// väärtusi saab omistada ainult läbi konstruktori argumentide.
public class OrderAttribute(int value) : Attribute
{
    public int Value { get; } = value; // väärtus
}

// xUnit järjesti mis implementeerib ITestCaseOrderer liidasest "OrderTestCases" meetodi,
// ning see meetod on see, mida me kasutame testide käivitamisel nende testide õigesti järjestamiseks.
// Järjestatakse vastavalt OrderAttribute omadusele
public class PriorityOrderer : ITestCaseOrderer
{
    // meetod võtab sisse kõik test cased
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        // toimub sorteerimistegevus kus järjestatakse arendaja kirjutatud järjesti järgi.
        var result = testCases.OrderBy(test =>
        {
            // otsitakse järjesti omadusena
            var method =
                test.TestMethod.Method
                    .GetCustomAttributes(typeof(OrderAttribute))
                    .FirstOrDefault();

            // kui ei leita
            if (method == null)
            {
                // antakse tagasi maksimaalne täisarv, ehk test teostatakse kohe,
                // ega ei panda teiste potensiaalselt ÕIGES järjekorras olevate testide vahele
                return int.MaxValue;
            }
            
            // siis tagastatakse selle järjesti omaduse väärtus
            return method.GetNamedArgument<int>(nameof(OrderAttribute.Value));
        }).ToList();

        return result;
    }
}

[TestCaseOrderer("School2.SeleniumTesting.PriorityOrderer", "School2.SeleniumTesting")]
public class LanguageCourseFrontendTests
{
    public string ThisRunGuid { get; set; }
    
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
    
    private static string DataEntryLanguageCourse(IWebDriver driver, bool isOneOrTwo = false, bool updateName = true)
    {
        string generatedId = string.Empty;
        
        if (updateName)
        {
            generatedId = Guid.NewGuid().ToString();
            IWebElement nimetusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Nimetus"));
            nimetusEntry.SendKeys(generatedId);
        }
        
        if (!isOneOrTwo)
        {
            IWebElement keelEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            keelEntry.SendKeys("C# + Selenium & xUnit");
        
            IWebElement taseEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            taseEntry.SendKeys("3");
        
            IWebElement kirjeldusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            kirjeldusEntry.SendKeys("Kursusel õpime mis on automaattestimine, kuidas erineb manuaaltestimisest ning kuidas kirjutada backendi ja frontendi automaatteste.");
        }
        else
        {
            IWebElement keelEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Keel"));
            keelEntry.SendKeys("Mantis, Excel");
        
            IWebElement taseEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Tase"));
            taseEntry.SendKeys("3");
        
            IWebElement kirjeldusEntry =  driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus"));
            kirjeldusEntry.SendKeys("Kursusel õpime mis on manuaaltestimise, kuidas erineb automaattestimisest ning mis dokumentatsioon manuaaltestimise juures tähtis on ning kuidas neid kirjutada.");
        }

        return generatedId;
    }

    private static void DataEntryLanguageCourseClear(IWebDriver driver, bool saveName = false)
    {
        if (!saveName)
            driver.FindElement(By.Id("CU_LanguageCourse_Nimetus")).Clear();
        
        driver.FindElement(By.Id("CU_LanguageCourse_Keel")).Clear();
        driver.FindElement(By.Id("CU_LanguageCourse_Tase")).Clear();
        driver.FindElement(By.Id("CU_LanguageCourse_Kirjeldus")).Clear();
    }
    
    [Fact, Order(1)]
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
    
    [Fact, Order(5)]
    public void Should_NavigateToIndex_AddLanguageCourseWithUniqueData_ReturnToIndex()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex(driver);
        
        /* tegevus */
        IWebElement indexCreateBtn = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Btn"));
        indexCreateBtn.Click();
        
        string uniqueId = DataEntryLanguageCourse(driver);
        ThisRunGuid = uniqueId;

        IWebElement createBtn = driver.FindElement(By.Id("CU_Create_Btn"));
        createBtn.Click();
        
        /* kontroll */
        
        //kogune andmed
        string languageCourseNimetusData = driver.FindElement(By.Id("IndexTableCourseNimetus")).Text;
        string languageCourseKeelData = driver.FindElement(By.Id("IndexTableCourseKeel")).Text;
        
        Thread.Sleep(500);
        driver.Quit();
        
        //assertime
        Assert.Equal(uniqueId, languageCourseNimetusData);
        Assert.Equal("C# + Selenium & xUnit", languageCourseKeelData);
        Assert.True((uniqueId == languageCourseNimetusData) && ("C# + Selenium & xUnit" == languageCourseKeelData));
    }

    [Fact, Order(2)]
    public void Should_NavigateToDetailsOfALanguageCourse_WithPreviouslyCorrectData()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex(driver);
        
        var table =  driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Table"));

        List<IWebElement> rowsOfTable = table.FindElements(By.TagName("tr")).ToList();
        rowsOfTable.RemoveAt(0); // delete th

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

    [Fact, Order(6)]
    public void Should_NavigateToDetailsOfALanguageCourse_WithPreviouslyUniqueData()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex(driver);
        
        var table =  driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Table"));

        List<IWebElement> rowsOfTable = table.FindElements(By.TagName("tr")).ToList();
        rowsOfTable.RemoveAt(0); // delete th

        bool isObjectFound = false;
        
        /* tegevus */
        foreach (var row in rowsOfTable)
        {
            var nimetusElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            if (nimetusElement == ThisRunGuid)
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
        Assert.Equal(ThisRunGuid, detailsNimetus);
        Assert.Equal("C# + Selenium & xUnit", detailsKeel);
        Assert.Equal("Kursusel õpime mis on automaattestimine, kuidas erineb manuaaltestimisest ning kuidas kirjutada backendi ja frontendi automaatteste.", detailsKirjeldus);
        Assert.True(int.Parse(detailsTase) > -1);
    }
    
    [Fact, Order(3)]
    public void Should_UpdateDetailsOfLanguageCourses_WithNewData()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex(driver);
        
        var table =  driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Table"));

        List<IWebElement> rowsOfTable = table.FindElements(By.TagName("tr")).ToList();
        rowsOfTable.RemoveAt(0); // delete th

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
    
    [Fact, Order(7)]
    public void Should_UpdateDetailsOfUniqueLanguageCourses_WithNewData()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex(driver);
        
        var table =  driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Table"));

        List<IWebElement> rowsOfTable = table.FindElements(By.TagName("tr")).ToList();
        rowsOfTable.RemoveAt(0); // delete th

        bool isObjectFound = false;
        
        /* tegevus */
        foreach (var row in rowsOfTable)
        {
            var nimetusElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            if (nimetusElement == ThisRunGuid)
            {
                isObjectFound = true;
                var updateBtn = row
                    .FindElement(By.Id("IndexTableCourseActions"))
                    .FindElement(By.Id("IndexTableCourseUpdate_Btn"));
                
                updateBtn.Click();
                
                break;
            }
        }

        DataEntryLanguageCourseClear(driver, true);
        DataEntryLanguageCourse(driver, true, false);
        
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
        Assert.NotEmpty(detailsNimetus);
        Assert.Equal(ThisRunGuid, detailsNimetus);
        Assert.Equal("Mantis, Excel", detailsKeel);
        Assert.Equal("3", detailsTase);
        Assert.Equal("Kursusel õpime mis on manuaaltestimise, kuidas erineb automaattestimisest ning mis dokumentatsioon manuaaltestimise juures tähtis on ning kuidas neid kirjutada.", detailsKirjeldus);
    }
    
    //DELETE, DELETE, DELETE
    [Fact, Order(4)]
    public void Should_DeleteLanguageCourse_FromDeleteView()
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
            if ((nimetusElement == "Manuaaltestimise kursus") && (keelElement == "Mantis, Excel"))
            {
                isObjectFound = true;
                var detailsBtn = row
                    .FindElement(By.Id("IndexTableCourseActions"))
                    .FindElement(By.Id("IndexTableCourseDelete_Btn"));
                
                detailsBtn.Click();
                
                break;
            }
        }
        
        Thread.Sleep(1000);
        driver.FindElement(By.Id("DD_Delete_Btn")).Click();
        
        Thread.Sleep(1000);
        
        /* kontroll */
        isObjectFound = false;
        
        table = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Table"));
        rowsOfTable = table.FindElements(By.TagName("tr")).ToList();
        rowsOfTable.RemoveAt(0); // delete th
        
        foreach (var row in rowsOfTable)
        {
            var nimetusElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            var keelElement = row.FindElement(By.Id("IndexTableCourseKeel")).Text;
            if ((nimetusElement == "Manuaaltestimise kursus") && (keelElement == "Mantis, Excel"))
            {
                isObjectFound = true;
                break;
            }
        }
        
        Thread.Sleep(500);
        driver.Quit();

        Assert.False(isObjectFound);
        Assert.True(!isObjectFound);
    }    
    
    [Fact, Order(8)]
    public void Should_DeleteUniqueLanguageCourse_FromDeleteView()
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
            if (nimetusElement == ThisRunGuid)
            {
                isObjectFound = true;
                var detailsBtn = row
                    .FindElement(By.Id("IndexTableCourseActions"))
                    .FindElement(By.Id("IndexTableCourseDelete_Btn"));
                
                detailsBtn.Click();
                
                break;
            }
        }
        
        Thread.Sleep(1000);
        driver.FindElement(By.Id("DD_Delete_Btn")).Click();
        
        Thread.Sleep(1000);
        
        /* kontroll */
        isObjectFound = false;
        
        table = driver.FindElement(By.Id("IndexCreateNewLanguageCourse_Table"));
        rowsOfTable = table.FindElements(By.TagName("tr")).ToList();
        rowsOfTable.RemoveAt(0); // delete th
        
        foreach (var row in rowsOfTable)
        {
            var nimetusElement = row.FindElement(By.Id("IndexTableCourseNimetus")).Text;
            if (nimetusElement == ThisRunGuid)
            {
                isObjectFound = true;
                break;
            }
        }
        
        Thread.Sleep(500);
        driver.Quit();

        Assert.False(isObjectFound);
        Assert.True(!isObjectFound);
    }
}