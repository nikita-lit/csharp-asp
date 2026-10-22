using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using Xunit;

namespace School2.xUnitTesting;

public class StudentsServicesTests : TestBase
{
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