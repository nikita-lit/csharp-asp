namespace School2.SeleniumTesting;

// Järjesti klass, mis omab omadusi mille järgi teste saab järjestada,
// väärtusi saab omistada ainult läbi konstruktori argumentide.
public class OrderAttribute( int value ) : Attribute
{
    public int Value { get; } = value; // väärtus
}