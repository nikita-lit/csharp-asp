using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace School2.SeleniumTesting;

public abstract class FrontendTestBase
{
    public const string Url = "http://localhost:5117/";

    /// <summary>
    ///     Ülesseade meetod mis tekitab draiveri ja navigeerib aadressile
    /// </summary>
    /// <param name="driver"></param>
    protected IWebDriver CreateDriver()
    {
        var driver = new FirefoxDriver(); // brauserit juhtiv ja seal tegutsev draiver
        driver.Url = Url; // navigeeritav aadress

        Thread.Sleep( 1000 );

        return driver;
    }

    /// <summary>
    ///     Ülesseade meetod mis vajutab keeltekursuste lingile
    /// </summary>
    /// <param name="driver"></param>
    protected void NavigateToIndex( IWebDriver driver, string btnId )
    {
        var navigateToLanguageCourse =
            driver.FindElement( By.Id( btnId ) ); // otsime tesitava osa lingi
        navigateToLanguageCourse.Click(); // simuleeritud tegevus elemendiga, antud juhul klikk
    }
}