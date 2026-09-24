using School.Controllers;
using School.Models;

namespace School.xUnitTests;

public class RegisterUnitTests : TestBase
{
    [Fact]
    // 1. Kirjeldatakse ära, kas test on tavaline (peaks tegema/ei tohi) või negatiivne (ei tohi/peaks tegema)
    // 2. Kirjeldatakse ära, mida parasjagu üritatakse testialuse objektiga teha
    // 3. Mis tingimustel tulemust kontrollitakse peale tegevust
    //
    //              1            2               3
    //             \/           \/              \/
    public async Task ShouldNot_AddEmptyStudent_WhenResultIsReturned()
    {
        // ülesseade
        var controller = Controller<DashboardController>();
        var reg = new Registration()
        {
            Id = 1,
            TrainingId = 1,
            StudentUser = null,
        }
        
        // tegevus
        
        // kontroll
    }
}
