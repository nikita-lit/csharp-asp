using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using Xunit;

namespace School2.xUnitTesting;

public class LecturersServicesTests : TestBase
{
    /// <summary>
    ///     Test, mis lisab uue objekti ja kontrollib, kas sellel on sama väärtus.
    /// </summary>
    [Fact]
    public async Task Should_AddNewLecturer_WhenResultIsReturned()
    {
        //ülesseade
        var dto = new LecturerDTO();
        dto.FirstName = "Test";
        dto.LastName = "Test";
        dto.Qualifications = "Test";

        //tegevus
        var result = await Svc<ILecturersServices>().Create( dto );

        //kontroll
        Assert.NotNull( result );

        Assert.NotEmpty( result.FirstName );
        Assert.NotEmpty( result.LastName );
        Assert.NotEmpty( result.Qualifications );

        Assert.Matches( dto.FirstName, result.FirstName );
        Assert.Matches( dto.LastName, result.LastName );
        Assert.Matches( dto.Qualifications, result.Qualifications );
    }

    // details test
    /// <summary>
    ///     Test, mis kontrollib Detailsist saadud väärtusi teadaoleva Guid-iga
    /// </summary>
    [Fact]
    public async Task Should_ReturnLecturerDetails_WhenGuidIsNotNull()
    {
        //ülesseade
        var createdLecturer = await AddLecturerToDb();

        //tegevus
        var result = await Svc<ILecturersServices>().DetailsAsync( createdLecturer.Id );

        //kontroll
        Assert.NotNull( result );

        Assert.Equal( result.Id, createdLecturer.Id );
        Assert.True( result.Id == createdLecturer.Id );

        Assert.Equal( result, createdLecturer );

        Assert.Matches( createdLecturer.FirstName, result.FirstName );
        Assert.Matches( createdLecturer.LastName, result.LastName );
        Assert.Matches( createdLecturer.Qualifications, result.Qualifications );
    }

    // update test
    /// <summary>
    ///     Test, mis muudab õppejõu andmeid ja kontrollib uusi väärtusi.
    /// </summary>
    [Fact]
    public async Task Should_UpdateLecturerWithNewData_WhenDataIsDifferentFromDb()
    {
        //ülesseade
        var dto = MockLecturerDTOData();
        var createResult = await Svc<ILecturersServices>().Create( dto );
        dto.Id = createResult.Id;
        dto.FirstName = "Testnimi";
        dto.LastName = "Testnimi";
        dto.Qualifications = "Räägib nagu maakas";
        dto.CreatedAt = createResult.CreatedAt;
        dto.ModifiedAt = DateTime.UtcNow;

        // tegevus
        var result = await Svc<ILecturersServices>().Update( dto );

        // kontroll
        Assert.NotNull( result );

        Assert.Equal( dto.Id, result.Id );
        Assert.Equal( dto.Qualifications, result.Qualifications );
        Assert.Matches( dto.FirstName, result.FirstName );
        Assert.Matches( dto.LastName, result.LastName );
    }

    // delete test
    /// <summary>
    ///     Test, mis kustutab õppejõu ja kontrollib, et teda enam andmebaasis ei ole.
    /// </summary>
    [Fact]
    public async Task Should_DeleteDataFromDb_WhenValidIdIsGiven()
    {
        // ülesseade
        var createdLecturer = await AddLecturerToDb();

        //tegevus
        var deletedLecturer = await Svc<ILecturersServices>().Delete( createdLecturer.Id );
        var result = await Svc<ILecturersServices>().DetailsAsync( createdLecturer.Id );

        // kontroll
        Assert.Null( result );
        Assert.NotNull( deletedLecturer );

        Assert.Equal( createdLecturer, deletedLecturer );
        Assert.Equal( createdLecturer.Id, deletedLecturer.Id );
        Assert.NotEqual( deletedLecturer, result );
    }

    public async Task<Lecturer> AddLecturerToDb()
    {
        var result = MockLecturerDTOData();
        return await Svc<ILecturersServices>().Create( result );
    }

    private LecturerDTO MockLecturerDTOData()
    {
        var dto = new LecturerDTO
        {
            FirstName = "Test",
            LastName = "Test",
            Qualifications = "Test"
        };

        return dto;
    }
}