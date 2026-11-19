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

    /// <summary>
    ///     Test, mis tühistab kustutamise ja kontrollib, et õpilane koos andmetega säilib.
    /// </summary>
    [Fact, Order( 4 )]
    public void Should_KeepStudent_WhenDeletionIsCancelled()
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
                var button = tableRow.FindElement( By.Id( "IndexTableStudentDelete_Btn" ) );
                button.Click();
                break;
            }
        }

        /* tegevus */
        driver.FindElement( By.Id( "DD_CancelDelete_Btn" ) ).Click();

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
        Assert.Equal( "John", studentLastName );
        Assert.Equal( "1234567890", studentPhoneNumber );
        Assert.Equal( "good@mail.good", studentEmail );

        driver.Quit();
    }

    /// <summary>
    ///     Test, mis lahkub muutmisvormilt salvestamata ja kontrollib varasemate andmete säilimist.
    /// </summary>
    [Fact, Order( 5 )]
    public void Should_KeepPreviousData_WhenLeavingUpdateWithoutSaving()
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
        DataEntryStudentClear( driver );
        driver.FindElement( By.Id( "CU_Student_FirstName" ) ).SendKeys( "Salvestamata" );

        DataEntryStudent( driver );
        driver.FindElement( By.Id( "CU_Back_Btn" ) ).Click();

        var studentTable = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) );
        var studentRows = studentTable.FindElements( By.TagName( "tr" ) ).ToList();
        studentRows.RemoveAt( 0 );

        bool isObjectFound = false;
        foreach ( var studentRow in studentRows )
        {
            string firstName = studentRow.FindElement( By.Id( "IndexTableStudent_FirstName" ) ).Text;
            if ( firstName == ThisRunGuid )
            {
                isObjectFound = true;
                studentRow.FindElement( By.Id( "IndexTableStudentDetails_Btn" ) ).Click();
                break;
            }
        }

        Assert.True( isObjectFound );

        /* kontroll */
        Assert.NotEmpty( driver.FindElement( By.Id( "DD_StudentGuid" ) ).Text );
        Assert.Equal( ThisRunGuid, driver.FindElement( By.Id( "DD_StudentFirstName" ) ).Text );
        Assert.Equal( "John", driver.FindElement( By.Id( "DD_StudentLastName" ) ).Text );
        Assert.Equal( "1234567890", driver.FindElement( By.Id( "DD_StudentPhoneNumber" ) ).Text );
        Assert.Equal( "good@mail.good", driver.FindElement( By.Id( "DD_StudentEmail" ) ).Text );

        driver.Quit();
    }

    /// <summary>
    ///     Test, mis kontrollib kohustusliku eesnime veateadet ja teiste väljade säilimist vormil.
    /// </summary>
    [Fact, Order( 6 )]
    public void Should_ShowValidationError_WhenFirstNameIsEmpty()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex( driver, "Students_Btn" );
        driver.FindElement( By.Id( "IndexCreateNewStudent_Btn" ) ).Click();
        DataEntryStudent( driver );

        /* tegevus */
        driver.FindElement( By.Id( "CU_Create_Btn" ) ).Click();

        /* kontroll */
        var error = driver.FindElement( By.Id( "CU_Student_FirstName_Error" ) );

        Assert.True( error.Displayed );
        Assert.NotEmpty( error.Text );
        Assert.Equal( string.Empty, driver.FindElement( By.Id( "CU_Student_FirstName" ) ).GetAttribute( "value" ) );
        Assert.Equal( "Testttt", driver.FindElement( By.Id( "CU_Student_LastName" ) ).GetAttribute( "value" ) );
        Assert.Equal( "6666666666", driver.FindElement( By.Id( "CU_Student_PhoneNumber" ) ).GetAttribute( "value" ) );
        Assert.Equal( "evil@mail.evil", driver.FindElement( By.Id( "CU_Student_Email" ) ).GetAttribute( "value" ) );
        Assert.True( driver.FindElement( By.Id( "CU_Create_Btn" ) ).Displayed );

        driver.Quit();
    }

    /// <summary>
    ///     Test, mis kontrollib vigase e-posti veateadet ja seda, et õpilast ei lisata nimekirja.
    /// </summary>
    [Fact, Order( 7 )]
    public void Should_NotCreateStudent_WhenEmailIsInvalid()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex( driver, "Students_Btn" );
        driver.FindElement( By.Id( "IndexCreateNewStudent_Btn" ) ).Click();

        string uniqueName = DataEntryStudent( driver, updateName: true );
        driver.FindElement( By.Id( "CU_Student_Email" ) ).Clear();
        driver.FindElement( By.Id( "CU_Student_Email" ) ).SendKeys( "vigane-email" );

        /* tegevus */
        driver.FindElement( By.Id( "CU_Create_Btn" ) ).Click();

        /* kontroll */
        var error = driver.FindElement( By.Id( "CU_Student_Email_Error" ) );
        Assert.True( error.Displayed );
        Assert.NotEmpty( error.Text );
        Assert.Equal( uniqueName, driver.FindElement( By.Id( "CU_Student_FirstName" ) ).GetAttribute( "value" ) );
        Assert.Equal( "vigane-email", driver.FindElement( By.Id( "CU_Student_Email" ) ).GetAttribute( "value" ) );
        driver.FindElement( By.Id( "CU_Back_Btn" ) ).Click();
        var checkTable = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) );
        var checkRows = checkTable.FindElements( By.TagName( "tr" ) ).ToList();
        checkRows.RemoveAt( 0 );
        bool isObjectFound = false;

        foreach ( var checkRow in checkRows )
        {
            string firstName = checkRow.FindElement( By.Id( "IndexTableStudent_FirstName" ) ).Text;
            if ( firstName == uniqueName )
            {
                isObjectFound = true;
                break;
            }
        }

        Assert.False( isObjectFound );

        driver.Quit();
    }

    /// <summary>
    ///     Test, mis kustutab sama õpilase ja kontrollib tema kadumist nimekirjast.
    /// </summary>
    [Fact, Order( 8 )]
    public void Should_DeleteUniqueStudent_FromDeleteView()
    {
        /* ülesseade */
        var driver = CreateDriver();
        NavigateToIndex( driver, "Students_Btn" );

        int countBefore = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) )
            .FindElement( By.TagName( "tbody" ) ).FindElements( By.TagName( "tr" ) ).Count;

        var studentTable = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) );
        var studentRows = studentTable.FindElements( By.TagName( "tr" ) ).ToList();
        studentRows.RemoveAt( 0 );

        bool wasObjectFound = false;
        foreach ( var studentRow in studentRows )
        {
            string firstName = studentRow.FindElement( By.Id( "IndexTableStudent_FirstName" ) ).Text;
            if ( firstName == ThisRunGuid )
            {
                wasObjectFound = true;
                studentRow.FindElement( By.Id( "IndexTableStudentDelete_Btn" ) ).Click();
                break;
            }
        }

        Assert.True( wasObjectFound );
        Assert.NotEmpty( driver.FindElement( By.Id( "DD_StudentGuid" ) ).Text );

        /* tegevus */
        driver.FindElement( By.Id( "DD_Delete_Btn" ) ).Click();

        /* kontroll */
        Assert.True( driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) ).Displayed );

        var checkTable = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) );
        var checkRows = checkTable.FindElements( By.TagName( "tr" ) ).ToList();
        checkRows.RemoveAt( 0 );

        bool isObjectFound = false;

        foreach ( var checkRow in checkRows )
        {
            string firstName = checkRow.FindElement( By.Id( "IndexTableStudent_FirstName" ) ).Text;
            if ( firstName == ThisRunGuid )
            {
                isObjectFound = true;
                break;
            }
        }

        Assert.False( isObjectFound );

        int countAfter = driver.FindElement( By.Id( "IndexCreateNewStudent_Table" ) )
            .FindElement( By.TagName( "tbody" ) ).FindElements( By.TagName( "tr" ) ).Count;

        Assert.Equal( countBefore - 1, countAfter );

        driver.Quit();
    }
}