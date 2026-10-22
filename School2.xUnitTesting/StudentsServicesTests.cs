using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using Xunit;

namespace School2.xUnitTesting;

public class StudentsServicesTests : TestBase
{
    /// <summary>
    ///     Test, mis lisab uue õpilase ja kontrollib tema andmeid.
    /// </summary>
    [Fact]
    public async Task Should_AddNewStudent_WhenResultIsReturned()
    {
        //ülesseade
        var dto = MockStudentDTOData();

        //tegevus
        var result = await Svc<IStudentsServices>().Create( dto );

        //kontroll
        Assert.NotNull( result );

        Assert.NotEmpty( result.FirstName );
        Assert.NotEmpty( result.LastName );
        Assert.NotEmpty( result.PhoneNumber );
        Assert.NotEmpty( result.Email );

        Assert.Matches( dto.FirstName, result.FirstName );
        Assert.Matches( dto.LastName, result.LastName );
        Assert.Matches( dto.PhoneNumber, result.PhoneNumber );
        Assert.Matches( dto.Email, result.Email );
    }

    /// <summary>
    ///     Test, mis kontrollib õpilase andmeid teadaoleva Guid-iga.
    /// </summary>
    [Fact]
    public async Task Should_ReturnStudentDetails_WhenGuidIsNotNull()
    {
        //ülesseade
        var createdStudent = await AddStudentToDb();

        //tegevus
        var result = await Svc<IStudentsServices>().DetailsAsync( createdStudent.Id );

        //kontroll
        Assert.NotNull( result );

        Assert.Equal( result.Id, createdStudent.Id );
        Assert.True( result.Id == createdStudent.Id );

        Assert.Equal( result, createdStudent );

        Assert.Matches( createdStudent.FirstName, result.FirstName );
        Assert.Matches( createdStudent.LastName, result.LastName );
        Assert.Matches( createdStudent.PhoneNumber, result.PhoneNumber );
        Assert.Matches( createdStudent.Email, result.Email );
    }

    /// <summary>
    ///     Test, mis muudab õpilase andmeid ja kontrollib uusi väärtusi.
    /// </summary>
    [Fact]
    public async Task Should_UpdateStudentWithNewData_WhenDataIsDifferentFromDb()
    {
        //ülesseade
        var dto = MockStudentDTOData();

        var createResult = await Svc<IStudentsServices>().Create( dto );
        dto.Id = createResult.Id;
        dto.FirstName = "Testnimi";
        dto.LastName = "Testnimi2";
        dto.PhoneNumber = "22222222";
        dto.Email = "test2@test2.com";
        dto.CreatedAt = createResult.CreatedAt;
        dto.ModifiedAt = DateTime.UtcNow;

        // tegevus
        var result = await Svc<IStudentsServices>().Update( dto );

        // kontroll
        Assert.NotNull( result );

        Assert.Equal( dto.Id, result.Id );
        Assert.Equal( dto.PhoneNumber, result.PhoneNumber );
        Assert.Matches( dto.FirstName, result.FirstName );
        Assert.Matches( dto.LastName, result.LastName );
        Assert.Matches( dto.PhoneNumber, result.PhoneNumber );
        Assert.Matches( dto.Email, result.Email );
    }

    /// <summary>
    ///     Test, mis kustutab õpilase ja kontrollib, et teda enam andmebaasis ei ole.
    /// </summary>
    [Fact]
    public async Task Should_DeleteDataFromDb_WhenValidIdIsGiven()
    {
        // ülesseade
        var createdStudent = await AddStudentToDb();

        //tegevus
        var deletedStudent = await Svc<IStudentsServices>().Delete( createdStudent.Id );
        var result = await Svc<IStudentsServices>().DetailsAsync( createdStudent.Id );

        // kontroll
        Assert.Null( result );
        Assert.NotNull( deletedStudent );

        Assert.Equal( createdStudent, deletedStudent );
        Assert.Equal( createdStudent.Id, deletedStudent.Id );
        Assert.NotEqual( deletedStudent, result );
    }

    /// <summary>
    ///     Test, mis kontrollib, et ainult tühikutest koosneva ees- ja perekonnanimega õpilast ei lisata.
    /// </summary>
    [Fact]
    public async Task ShouldNot_AddStudentToDb_WhenFirstNameAndLastNameIsWhiteSpace()
    {
        /* ülesseade */
        var dto = MockStudentDTOData();
        dto.FirstName = "   ";
        dto.LastName = "   ";

        /* tegevus */
        var createResult = await Svc<IStudentsServices>().Create( dto );

        /* kontroll */
        Assert.Null( createResult );

        Assert.True( string.IsNullOrWhiteSpace( dto.FirstName ) );
        Assert.True( string.IsNullOrWhiteSpace( dto.LastName ) );
    }

    /// <summary>
    ///     Test, mis kontrollib, et olematu Guid-iga õpilase andmeid ei tagastata.
    /// </summary>
    [Fact]
    public async Task ShouldNot_ReturnDetails_WhenGuidIsIncorrect()
    {
        /* ülesseade */
        var createdStudent = await AddStudentToDb();
        var guid = Guid.NewGuid();

        /* tegevus */
        var result = await Svc<IStudentsServices>().DetailsAsync( guid );

        /* kontroll */
        Assert.Null( result );

        Assert.NotEqual( guid, createdStudent.Id );
    }

    /// <summary>
    ///     Test, mis kontrollib, et vigaste andmetega uuendamisel jäävad õpilase varasemad andmed alles.
    /// </summary>
    [Fact]
    public async Task ShouldNot_UpdateStudent_WhenDataIsIncorrect()
    {
        /* ülesseade */
        var dto = MockStudentDTOData();
        var createResult = await Svc<IStudentsServices>().Create( dto );
        dto.Id = createResult.Id;

        var updateDTO = new StudentDTO
        {
            Id = createResult.Id,
            FirstName = "       ",
            LastName = "Testttt",
            PhoneNumber = "6666666666",
            Email = "evil@mail.evil",
            CreatedAt = createResult.CreatedAt,
            ModifiedAt = DateTime.UtcNow
        };

        /* tegevus */
        var result = await Svc<IStudentsServices>().Update( updateDTO );
        var details = await Svc<IStudentsServices>().DetailsAsync( createResult.Id );

        /* kontroll */
        Assert.Null( result );

        Assert.Equal( dto.Id, details.Id );
        Assert.Equal( dto.PhoneNumber, details.PhoneNumber );
        Assert.Matches( dto.FirstName, details.FirstName );
        Assert.Matches( dto.LastName, details.LastName );
        Assert.Matches( dto.PhoneNumber, details.PhoneNumber );
        Assert.Matches( dto.Email, details.Email );

        Assert.Equal( updateDTO.Id, details.Id );
        Assert.DoesNotMatch( updateDTO.FirstName, details.FirstName );
        Assert.DoesNotMatch( updateDTO.LastName, details.LastName );
    }

    /// <summary>
    ///     Test, mis kontrollib, et üle 100 tähemärgi pikkuse ees- ja perekonnanimega õpilast ei lisata.
    /// </summary>
    [Fact]
    public async Task ShouldNot_AddStudentToDb_WhenFirstNameAndLastNameHaveLengthMoreThan100Characters()
    {
        /* ülesseade */
        var dto = MockStudentDTOData();
        dto.FirstName = new string( 't', 101 );
        dto.LastName = new string( 'y', 101 );

        /* tegevus */
        var createResult = await Svc<IStudentsServices>().Create( dto );

        /* kontroll */
        Assert.Null( createResult );

        Assert.True( dto.FirstName.Length > 100 );
        Assert.True( dto.LastName.Length > 100 );
    }

    /// <summary>
    ///     Test, mis kontrollib, et õpilase teistkordne kustutamine tagastab nulli.
    /// </summary>
    [Fact]
    public async Task Should_ReturnNull_WhenDeletingStudentTwice()
    {
        /* ülesseade */
        var createdStudent = await AddStudentToDb();

        /* tegevus */
        var deletedStudent = await Svc<IStudentsServices>().Delete( createdStudent.Id );
        var deleted2Student = await Svc<IStudentsServices>().Delete( createdStudent.Id );

        var result = await Svc<IStudentsServices>().DetailsAsync( createdStudent.Id );

        /* kontroll */
        Assert.Null( result );
        Assert.NotNull( deletedStudent );
        Assert.Null( deleted2Student );

        Assert.Equal( createdStudent, deletedStudent );
        Assert.Equal( createdStudent.Id, deletedStudent.Id );
        Assert.NotEqual( deletedStudent, result );
    }

    public async Task<Student> AddStudentToDb()
    {
        var result = MockStudentDTOData();
        return await Svc<IStudentsServices>().Create( result );
    }

    private StudentDTO MockStudentDTOData()
    {
        var dto = new StudentDTO
        {
            FirstName = "Test",
            LastName = "Test",
            PhoneNumber = "11111111",
            Email = "test@test.com"
        };

        return dto;
    }
}