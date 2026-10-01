using School2.Core.DTO;
using School2.Core.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace School2.xUnitTesting
{
    public class LanguageCoursesServicesTests : TestBase
    {
        [Fact] //Käsusõna, mida testrunner tunneb, et aru saada mis on test, ja mis ei ole
        // 1 - Kirjeldatakse ära, kas test on tavaline (peaks/ei tohi teha), või negatiivne (ei tohi/peaks tegema)
        // 2 - Mida parasjagu testiga testitakse.
        // 3 - Mis tingimustel tulemust kontrollitakse, peale tegevust
        //                  1           2           3
        //                  \/          \/          \/
        public async Task Should_AddNewCourse_WhenResultIsReturned()
        {
            //ülesseade
            LanguageCourseDTO newCourseDTO = new LanguageCourseDTO();
            newCourseDTO.Nimetus = "TestKursus";
            newCourseDTO.Keel = "Eesti keel";
            newCourseDTO.Tase = "Algtase";
            newCourseDTO.Kirjeldus = "A0 tasemel eesti keele \"õpe\", tule ja raiska aega";

            //tegevus
            var result = await Svc<ILanguageCoursesServices>().Create(newCourseDTO);

            //kontroll
            Assert.NotNull(result);
            /*
             Assert on klass mille abil saab kontrollita andmete eri tingimusi, kujusid, olekuid jne.
            Antud juhul kontrollitakse eelnevat objekti ühe kontrolliga - et ei oleks tühi.
            Aga, kui meie meetod pärast selle sisu arendamist hakkab juba tagastama mingisugust objekti, 
            tuleks testi täiendada, täpsemate tingimustega, mis kontrollib näiteks, kas on samasugune, 
            sisaldab kindlal kujul andmeid, andmed on mingit kindlat tüüpi jne. Võimalusi mida kontrollida on palju,
            ning viise kuidas teste kirjutada veelgi rohkem.
             */
        }
    }
}
