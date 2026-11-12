using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
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
        var dto = MockLecturerDTOData();
        dto.UserId = "test-user";

        //tegevus
        var result = await Svc<ILecturersServices>().Create( dto );

        //kontroll
        Assert.NotNull( result );
        Assert.NotEqual( Guid.Empty, result.Id );

        Assert.NotEmpty( result.FirstName );
        Assert.NotEmpty( result.LastName );
        Assert.NotEmpty( result.Qualifications );

        Assert.Equal( dto.FirstName, result.FirstName );
        Assert.Equal( dto.LastName, result.LastName );
        Assert.Equal( dto.Qualifications, result.Qualifications );
        Assert.Equal( dto.UserId, result.UserId );
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

        Assert.Equal( createdLecturer.Id, result.Id );
        Assert.Equal( createdLecturer.FirstName, result.FirstName );
        Assert.Equal( createdLecturer.LastName, result.LastName );
        Assert.Equal( createdLecturer.Qualifications, result.Qualifications );
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
        dto.UserId = "test-user";
        var createResult = await Svc<ILecturersServices>().Create( dto );
        Assert.NotNull( createResult );

        var createdAt = createResult.CreatedAt;
        var modifiedAt = createResult.ModifiedAt;
        dto.Id = createResult.Id;
        dto.FirstName = "Testnimi";
        dto.LastName = "Testnimi";
        dto.Qualifications = "Räägib nagu maakas";
        dto.CreatedAt = DateTime.MinValue;
        dto.ModifiedAt = DateTime.MinValue;

        // tegevus
        var result = await Svc<ILecturersServices>().Update( dto );
        Svc<School2Context>().ChangeTracker.Clear();
        var details = await Svc<ILecturersServices>().DetailsAsync( dto.Id );

        // kontroll
        Assert.NotNull( result );
        Assert.NotNull( details );

        Assert.Equal( dto.Id, result.Id );
        Assert.Equal( dto.Qualifications, result.Qualifications );
        Assert.Equal( dto.FirstName, result.FirstName );
        Assert.Equal( dto.LastName, result.LastName );
        Assert.Equal( dto.FirstName, details.FirstName );
        Assert.Equal( dto.LastName, details.LastName );
        Assert.Equal( dto.Qualifications, details.Qualifications );
        Assert.Equal( createdAt, details.CreatedAt );
        Assert.Equal( dto.UserId, details.UserId );
        Assert.True( details.ModifiedAt >= modifiedAt );
        Assert.NotEqual( dto.ModifiedAt, details.ModifiedAt );
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

    /// <summary>
    ///     Test, mis kontrollib iga kohustuslikku välja nulli, tühja teksti ja tühikutega.
    /// </summary>
    [Theory]
    [InlineData( null, "Test", "Test" )]
    [InlineData( "", "Test", "Test" )]
    [InlineData( "   ", "Test", "Test" )]
    [InlineData( "Test", null, "Test" )]
    [InlineData( "Test", "", "Test" )]
    [InlineData( "Test", "   ", "Test" )]
    [InlineData( "Test", "Test", null )]
    [InlineData( "Test", "Test", "" )]
    [InlineData( "Test", "Test", "   " )]
    public async Task ShouldNot_AddNewLecturer_WhenRequiredFieldIsEmpty(
        string? firstName, string? lastName, string? qualifications )
    {
        //ülesseade
        var dto = MockLecturerDTOData();
        dto.FirstName = firstName!;
        dto.LastName = lastName!;
        dto.Qualifications = qualifications!;
        var countBefore = Svc<School2Context>().Lecturers.Count();

        //tegevus
        var result = await Svc<ILecturersServices>().Create( dto );

        //kontroll
        Assert.Null( result );
        Assert.Equal( countBefore, Svc<School2Context>().Lecturers.Count() );
    }

    /// <summary>
    ///     Test, mis kontrollib, et null DTO-ga õppejõudu ei lisata.
    /// </summary>
    [Fact]
    public async Task ShouldNot_AddNewLecturer_WhenDtoIsNull()
    {
        //tegevus
        var result = await Svc<ILecturersServices>().Create( null );

        //kontroll
        Assert.Null( result );
    }

    /// <summary>
    ///     Test, mis kontrollib, et lisamisel eemaldatakse väljade algusest ja lõpust tühikud.
    /// </summary>
    [Fact]
    public async Task Should_TrimLecturerFields_WhenAddingNewLecturer()
    {
        //ülesseade
        var dto = MockLecturerDTOData();
        dto.FirstName = "  Test nimi  ";
        dto.LastName = "  Test perekonnanimi  ";
        dto.Qualifications = "  Eesti keele õpetaja  ";

        //tegevus
        var result = await Svc<ILecturersServices>().Create( dto );

        //kontroll
        Assert.NotNull( result );
        Assert.Equal( "Test nimi", result.FirstName );
        Assert.Equal( "Test perekonnanimi", result.LastName );
        Assert.Equal( "Eesti keele õpetaja", result.Qualifications );
    }

    /// <summary>
    ///     Test, mis kontrollib, et olematu Guid-iga õppejõu andmeid ei tagastata.
    /// </summary>
    [Fact]
    public async Task ShouldNot_ReturnDetails_WhenGuidIsIncorrect()
    {
        //ülesseade
        var createdLecturer = await AddLecturerToDb();
        var guid = Guid.NewGuid();

        //tegevus
        var result = await Svc<ILecturersServices>().DetailsAsync( guid );
        var details = await Svc<ILecturersServices>().DetailsAsync( createdLecturer.Id );

        //kontroll
        Assert.Null( result );
        Assert.NotNull( details );
        Assert.Equal( createdLecturer.Id, details.Id );
    }

    /// <summary>
    ///     Test, mis kontrollib, et vigase välja korral jäävad varasemad andmed alles.
    /// </summary>
    [Theory]
    [InlineData( null, "Testnimi", "Uus kvalifikatsioon" )]
    [InlineData( "", "Testnimi", "Uus kvalifikatsioon" )]
    [InlineData( "   ", "Testnimi", "Uus kvalifikatsioon" )]
    [InlineData( "Testnimi", null, "Uus kvalifikatsioon" )]
    [InlineData( "Testnimi", "", "Uus kvalifikatsioon" )]
    [InlineData( "Testnimi", "   ", "Uus kvalifikatsioon" )]
    [InlineData( "Testnimi", "Testnimi", null )]
    [InlineData( "Testnimi", "Testnimi", "" )]
    [InlineData( "Testnimi", "Testnimi", "   " )]
    public async Task ShouldNot_UpdateLecturer_WhenDataIsIncorrect(
        string? firstName, string? lastName, string? qualifications )
    {
        //ülesseade
        var dto = MockLecturerDTOData();
        var createResult = await Svc<ILecturersServices>().Create( dto );
        Assert.NotNull( createResult );
        var createdAt = createResult.CreatedAt;
        var modifiedAt = createResult.ModifiedAt;

        var updateDTO = new LecturerDTO
        {
            Id = createResult.Id,
            FirstName = firstName!,
            LastName = lastName!,
            Qualifications = qualifications!
        };

        //tegevus
        var result = await Svc<ILecturersServices>().Update( updateDTO );
        Svc<School2Context>().ChangeTracker.Clear();
        var details = await Svc<ILecturersServices>().DetailsAsync( createResult.Id );

        //kontroll
        Assert.Null( result );
        Assert.NotNull( details );
        Assert.Equal( createResult.Id, details.Id );
        Assert.Equal( dto.FirstName, details.FirstName );
        Assert.Equal( dto.LastName, details.LastName );
        Assert.Equal( dto.Qualifications, details.Qualifications );
        Assert.Equal( createdAt, details.CreatedAt );
        Assert.Equal( modifiedAt, details.ModifiedAt );
    }

    public async Task<Lecturer> AddLecturerToDb()
    {
        var dto = MockLecturerDTOData();
        var result = await Svc<ILecturersServices>().Create( dto );
        Assert.NotNull( result );
        return result;
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
