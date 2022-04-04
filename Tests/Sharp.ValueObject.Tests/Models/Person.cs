using System;

namespace Sharp.ValueObject.Tests.Models
{
    public class Person : ComplexValueObject<Person>
    {
        public Person(PersonName name, PersonAge age, string note)
        {
            Name = name;
            Age = age;
            Note = note;
        }

        public PersonName Name { get; }

        public PersonAge Age { get; }

        public string Note { get; }
    }

    public class PersonName : ComplexValueObject<PersonName>
    {
        public PersonName(string firstName, string surname)
        {
            FirstName = firstName;
            Surname = surname;
        }

        public string FirstName { get; }

        public string Surname { get; }
    }

    public class PersonAge : SingleValueObject<int, PersonAge>
    {
        public PersonAge(int value) : base(value)
        {
            if (value is < 0 or > 150)
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}
