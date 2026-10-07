using School2.Core.DTO;
using School2.Core.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using School2.Core.Domain;
using Xunit;

namespace School2.xUnitTesting
{
    public class LanguageCoursesServicesTests : TestBase
    {
        // [Fact] //Käsusõna, mida testrunner tunneb, et aru saada mis on test, ja mis ei ole
        // 1 - Kirjeldatakse ära, kas test on tavaline (peaks/ei tohi teha), või negatiivne (ei tohi/peaks tegema)
        // 2 - Mida parasjagu testiga testitakse.
        // 3 - Mis tingimustel tulemust kontrollitakse, peale tegevust
        //                  1           2           3
        //                  \/          \/          \/
        // public async Task Should_AddNewCourse_WhenResultIsReturned()
        // {
        //     //ülesseade
        //     LanguageCourseDTO newCourseDTO = new LanguageCourseDTO();
        //     newCourseDTO.Nimetus = "TestKursus";
        //     newCourseDTO.Keel = "Eesti keel";
        //     newCourseDTO.Tase = "Algtase";
        //     newCourseDTO.Kirjeldus = "A0 tasemel eesti keele \"õpe\", tule ja raiska aega";
        //
        //     //tegevus
        //     var result = await Svc<ILanguageCoursesServices>().Create(newCourseDTO);
        //
        //     //kontroll
        //     Assert.NotNull(result);
        //     /*
        //      Assert on klass mille abil saab kontrollita andmete eri tingimusi, kujusid, olekuid jne.
        //     Antud juhul kontrollitakse eelnevat objekti ühe kontrolliga - et ei oleks tühi.
        //     Aga, kui meie meetod pärast selle sisu arendamist hakkab juba tagastama mingisugust objekti, 
        //     tuleks testi täiendada, täpsemate tingimustega, mis kontrollib näiteks, kas on samasugune, 
        //     sisaldab kindlal kujul andmeid, andmed on mingit kindlat tüüpi jne. Võimalusi mida kontrollida on palju,
        //     ning viise kuidas teste kirjutada veelgi rohkem.
        //      */
        // }

        [Fact]
        public async Task ShouldNot_AddNewCourse_WhenFieldsEmpty()
        {
            //ülesseade
            LanguageCourseDTO newCourseDTO = MockLanguageCourseDTOData();
            newCourseDTO.Keel = string.Empty;
            newCourseDTO.Nimetus = string.Empty;
            
            //tegevus
            var result = await Svc<ILanguageCoursesServices>().Create(newCourseDTO);

            //kontroll
            Assert.Null(result); // kontrollime et teenus lükkaks objekti lisamise tagasi

            if (result != null)
            {
                // kontrollime et keel oleks juurde lisatud, ja mitte tühi
                Assert.NotNull(result.Keel);
                Assert.NotNull(result.Nimetus);

                // kontrollime et keeles oleks midagi lisatud
                Assert.True(result.Keel.Length > 0); 
                Assert.False(result.Nimetus.Length < 1);
            
                // kontrollime et teenus ei kaota ära vahepeal andmeid mis me sisestasime
                Assert.Equal(newCourseDTO.Keel, result.Keel); 
                Assert.Equal(newCourseDTO.Nimetus, result.Nimetus);
            }
        }

        [Fact]
        public async Task Should_ReturnCourseDetails_WhenGuidIsNotNull()
        {
            var createdCourse = await AddObjectToDb();
            var result = await Svc<ILanguageCoursesServices>().DetailsAsync(createdCourse.Id);
            
            Assert.NotNull(result);
            
            Assert.Equal(result.Id, createdCourse.Id);
            Assert.True(result.Id == createdCourse.Id);
            
            Assert.Equal(result, createdCourse);
        }

        // test peab kontrollima et andmete muutmisel õigesti andmed ka lisatakse
        [Fact]
        public async Task Should_UpdateNimetusWithNewData_WhenDataIsDifferentFromDb()
        {
            //ülesseade
            LanguageCourseDTO dto = MockLanguageCourseDTOData();
            var createResult = await Svc<ILanguageCoursesServices>().Create(dto);
            dto.Id = createResult.Id;
            dto.Keel = "Eesti (Võro)";
            dto.Nimetus = "Võro kieli";
            dto.Kirjeldus = "räägi nagu maakas";
            dto.Tase = "C6";
            dto.CreatedAt = createResult.CreatedAt;
            dto.ModifiedAt = DateTime.UtcNow;
            
            // tegevus
            var result = await Svc<ILanguageCoursesServices>().Update(dto);
            
            // kontroll
            Assert.NotNull(result);
            
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.Keel, result.Keel);
            Assert.Equal(dto.Kirjeldus, result.Kirjeldus);
            Assert.Matches(dto.Tase, result.Tase);
            Assert.Matches(dto.Nimetus, result.Nimetus);
        }

        [Fact]
        public async Task Should_DeleteDataFromDb_WhenValidIdIsGiven()
        {
            // ülesseade
            var createdCourse = await AddObjectToDb();
            
            //tegevus
            var deletedCourse = await Svc<ILanguageCoursesServices>().Delete(createdCourse.Id);
            var result = await Svc<ILanguageCoursesServices>().DetailsAsync(createdCourse.Id);
            
            // kontroll
            Assert.Null(result);
            Assert.NotNull(deletedCourse);
            
            Assert.Equal(createdCourse, deletedCourse);
            Assert.Equal(createdCourse.Id, deletedCourse.Id);
            Assert.NotEqual(deletedCourse, result);
        }

        public async Task<LanguageCourse> AddObjectToDb()
        {
            var result = MockLanguageCourseDTOData();
            return await Svc<ILanguageCoursesServices>().Create(result);
        }

        private LanguageCourseDTO MockLanguageCourseDTOData()
        {
            var dto = new LanguageCourseDTO
            {
                Nimetus = "TestKursus",
                Keel = "Eesti keel",
                Tase = "Algtase",
                Kirjeldus = "A0 tasemel eesti keele \"õpe\", tule ja raiska aega"
            };

            return dto;
        }
    }
}
