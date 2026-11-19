using OpenQA.Selenium;
using Xunit;

namespace School2.SeleniumTesting;

[TestCaseOrderer( "School2.SeleniumTesting.PriorityOrderer", "School2.SeleniumTesting" )]
public class StudentFrontendTests : FrontendTestBase
{
    public static string ThisRunGuid { get; set; } = string.Empty;

    /// <summary>
    ///     Meetod, mis täidab õpilase vormi algsete või muudetud andmetega.
    /// </summary>
    private static string DataEntryStudent( IWebDriver driver, bool isOneOrTwo = false, bool updateName = false )
    {
        string generatedId = string.Empty;

        if ( updateName )
        {
            generatedId = Guid.NewGuid().ToString();
            driver.FindElement( By.Id( "CU_Student_FirstName" ) ).SendKeys( generatedId );
        }

        if ( !isOneOrTwo )
        {
            var lastNameEntry = driver.FindElement( By.Id( "CU_Student_LastName" ) );
            lastNameEntry.SendKeys( "Testttt" );

            var phoneEntry = driver.FindElement( By.Id( "CU_Student_PhoneNumber" ) );
            phoneEntry.SendKeys( "6666666666" );

            var emailEntry = driver.FindElement( By.Id( "CU_Student_Email" ) );
            emailEntry.SendKeys( "evil@mail.evil" );
        }
        else
        {
            var lastNameEntry = driver.FindElement( By.Id( "CU_Student_LastName" ) );
            lastNameEntry.SendKeys( "John" );

            var phoneEntry = driver.FindElement( By.Id( "CU_Student_PhoneNumber" ) );
            phoneEntry.SendKeys( "1234567890" );

            var emailEntry = driver.FindElement( By.Id( "CU_Student_Email" ) );
            emailEntry.SendKeys( "good@mail.good" );
        }

        return generatedId;
    }

    /// <summary>
    ///     Meetod, mis puhastab õpilase vormi väljad ja vajadusel säilitab eesnime.
    /// </summary>
    private static void DataEntryStudentClear( IWebDriver driver, bool saveName = false )
    {
        if ( !saveName )
            driver.FindElement( By.Id( "CU_Student_FirstName" ) ).Clear();

        driver.FindElement( By.Id( "CU_Student_LastName" ) ).Clear();
        driver.FindElement( By.Id( "CU_Student_PhoneNumber" ) ).Clear();
        driver.FindElement( By.Id( "CU_Student_Email" ) ).Clear();
    }

    /// <summary>
    ///     Test, mis lisab uue õpilase ja kontrollib tema andmeid õpilaste nimekirjas.
    /// </summary>
    [Fact, Order( 1 )]
    public void Should_NavigateToIndex_AddStudentWithUniqueData_ReturnToIndex()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex( driver, "Students_Btn" );

        /* tegevus */
        driver.FindElement( By.Id( "IndexCreateNewStudent_Btn" ) ).Click();
        ThisRunGuid = DataEntryStudent( driver, updateName: true );

        driver.FindElement( By.Id( "CU_Create_Btn" ) ).Click();

        /* kontroll */
        var studentTable = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) );
        var studentRows = studentTable.FindElements( By.TagName( "tr" ) ).ToList();
        studentRows.RemoveAt( 0 );

        bool isObjectFound = false;
        string studentFirstName = string.Empty;
        string studentLastName = string.Empty;
        string studentPhoneNumber = string.Empty;
        string studentEmail = string.Empty;

        foreach ( var studentRow in studentRows )
        {
            string firstName = studentRow.FindElement( By.Id( "IndexTableStudent_FirstName" ) ).Text;
            if ( firstName == ThisRunGuid )
            {
                isObjectFound = true;
                studentFirstName = studentRow.FindElement( By.Id( "IndexTableStudent_FirstName" ) ).Text;
                studentLastName = studentRow.FindElement( By.Id( "IndexTableStudent_LastName" ) ).Text;
                studentPhoneNumber = studentRow.FindElement( By.Id( "IndexTableStudent_PhoneNumber" ) ).Text;
                studentEmail = studentRow.FindElement( By.Id( "IndexTableStudent_Email" ) ).Text;
                break;
            }
        }

        Assert.True( isObjectFound );
        Assert.Equal( ThisRunGuid, studentFirstName );
        Assert.Equal( "Testttt", studentLastName );
        Assert.Equal( "6666666666", studentPhoneNumber );
        Assert.Equal( "evil@mail.evil", studentEmail );

        driver.Quit();
    }

    /// <summary>
    ///     Test, mis avab sama õpilase detailvaate ja kontrollib kõiki sisestatud andmeid.
    /// </summary>
    [Fact, Order( 2 )]
    public void Should_NavigateToStudentDetails_WithPreviouslyUniqueData()
    {
        /* ülesseade */
        var driver = CreateDriver();

        /* tegevus */
        NavigateToIndex( driver, "Students_Btn" );

        var table = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) );
        var rowsOfTable = table.FindElements( By.TagName( "tr" ) ).ToList();
        rowsOfTable.RemoveAt( 0 );

        foreach ( var tableRow in rowsOfTable )
        {
            string firstName = tableRow.FindElement( By.Id( "IndexTableStudent_FirstName" ) ).Text;
            if ( firstName == ThisRunGuid )
            {
                var button = tableRow.FindElement( By.Id( "IndexTableStudentDetails_Btn" ) );
                button.Click();
                break;
            }
        }

        /* kontroll */
        Assert.NotEmpty( driver.FindElement( By.Id( "DD_StudentGuid" ) ).Text );
        Assert.Equal( ThisRunGuid, driver.FindElement( By.Id( "DD_StudentFirstName" ) ).Text );
        Assert.Equal( "Testttt", driver.FindElement( By.Id( "DD_StudentLastName" ) ).Text );
        Assert.Equal( "6666666666", driver.FindElement( By.Id( "DD_StudentPhoneNumber" ) ).Text );
        Assert.Equal( "evil@mail.evil", driver.FindElement( By.Id( "DD_StudentEmail" ) ).Text );

        driver.FindElement( By.Id( "DD_DetailsBack_Btn" ) ).Click();
        Assert.True( driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) ).Displayed );

        driver.Quit();
    }

    /// <summary>
    ///     Test, mis muudab sama õpilase andmeid ja kontrollib neid pärast vormi värskendamist.
    /// </summary>
    [Fact, Order( 3 )]
    public void Should_UpdateDetailsOfUniqueStudent_WithNewData()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex( driver, "Students_Btn" );

        var table = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) );
        var rowsOfTable = table.FindElements( By.TagName( "tr" ) ).ToList();
        rowsOfTable.RemoveAt( 0 );

        foreach ( var tableRow in rowsOfTable )
        {
            string firstName = tableRow.FindElement( By.Id( "IndexTableStudent_FirstName" ) ).Text;
            if ( firstName == ThisRunGuid )
            {
                var button = tableRow.FindElement( By.Id( "IndexTableStudentUpdate_Btn" ) );
                button.Click();
                break;
            }
        }

        /* tegevus */
        DataEntryStudentClear( driver, true );
        DataEntryStudent( driver, true );

        driver.FindElement( By.Id( "CU_Update_Btn" ) ).Click();

        Thread.Sleep( 500 );

        string? detailsId = driver.FindElement( By.Id( "CU_Student_Guid" ) ).GetAttribute( "value" );
        string? detailsFirstName = driver.FindElement( By.Id( "CU_Student_FirstName" ) ).GetAttribute( "value" );
        string? detailsLastName = driver.FindElement( By.Id( "CU_Student_LastName" ) ).GetAttribute( "value" );
        string? detailsPhone = driver.FindElement( By.Id( "CU_Student_PhoneNumber" ) ).GetAttribute( "value" );
        string? detailsEmail = driver.FindElement( By.Id( "CU_Student_Email" ) ).GetAttribute( "value" );

        /* kontroll */
        Assert.NotNull( detailsId );
        Assert.NotEmpty( detailsId );
        Assert.Equal( ThisRunGuid, detailsFirstName );
        Assert.Equal( "John", detailsLastName );
        Assert.Equal( "1234567890", detailsPhone );
        Assert.Equal( "good@mail.good", detailsEmail );

        driver.Quit();
    }
}