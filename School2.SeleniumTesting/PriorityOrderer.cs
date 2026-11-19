using Xunit.Abstractions;
using Xunit.Sdk;

namespace School2.SeleniumTesting;

// xUnit järjesti mis implementeerib ITestCaseOrderer liidasest "OrderTestCases" meetodi,
// ning see meetod on see, mida me kasutame testide käivitamisel nende testide õigesti järjestamiseks.
// Järjestatakse vastavalt OrderAttribute omadusele
public class PriorityOrderer : ITestCaseOrderer
{
    // meetod võtab sisse kõik test cased
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>( IEnumerable<TTestCase> testCases )
        where TTestCase : ITestCase
    {
        // toimub sorteerimistegevus kus järjestatakse arendaja kirjutatud järjesti järgi.
        var result = testCases.OrderBy( test =>
        {
            // otsitakse järjesti omadusena
            var method =
                test.TestMethod.Method
                    .GetCustomAttributes( typeof(OrderAttribute) )
                    .FirstOrDefault();

            // kui ei leita
            if ( method == null )
                // antakse tagasi maksimaalne täisarv, ehk test teostatakse kohe,
                // ega ei panda teiste potensiaalselt ÕIGES järjekorras olevate testide vahele
                return int.MaxValue;

            // siis tagastatakse selle järjesti omaduse väärtus
            return method.GetNamedArgument<int>( nameof(OrderAttribute.Value) );
        } ).ToList();

        return result;
    }
}