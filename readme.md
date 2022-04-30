# Sharp.ValueObject

## Usage

### 1. Use as "extended" enum type

You can define a type that will resemble an enum, but it will also be possible to add additional functionality to it.

The following example shows how you can define a `Direction` enum based on the `int` type:

```cs
    public class Direction : SingleValueObject<int, Direction>
    {
        public static Constant Up { get; } = new(1);
        public static Constant Down { get; } = new(2);
        public static Constant Left { get; } = new(3);
        public static Constant Right { get; } = new(4);

        protected Direction(int value) : base(value) { }
    }
```

An important point, the constructor should not be public, otherwise, it will be possible to create an object with a value that was not defined as available. In the examples you see `protected` to prevent Visual Studio from showing a warning due to unused constructor.

If the enum type must be based on the `string` type, then it is better to use `StringValueObject` type insted:

```cs
    public class Direction : StringValueObject<Direction>
    {
        public static Constant Up { get; } = new("Up");
        public static Constant Down { get; } = new("Down");
        public static Constant Left { get; } = new("Left");
        public static Constant Right { get; } = new("Right");

        protected Direction(string value) : base(value) { }
    }
```

Examples:  
- [Direction](Examples/Example_001_StringEnum)

### 2. Use as some specialized type with one value

Sometimes you need to define a type that will contain some single value such as temperature, distance, weight, speed, etc. Such types can also contain constants, methods and properties with any logic, and value constraints can be defined in the constructor:

```cs
    public class Temperature : SingleValueObject<float, Temperature>
    {
        public static Constant AbsoluteZero { get; } = new(-273.15F);
        public static Constant WaterFreeze { get; } = new(0.0F);
        public static Constant WaterEvaporation { get; } = new(100.0F);

        public Temperature(float value) : base(value)
        {
            if (value < AbsoluteZero.Value)
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        public bool IsWaterFreeze => Value <= WaterFreeze.Value;
        public bool IsWaterEvaporation => Value >= WaterEvaporation.Value;
    }
```

In this example, the constructor is defined as public, which allows you to set any float value.

In some cases it is required to extend the base class of constants, for example, to add new fields to them, it can be used to show some list of available values:

```cs
    public class Temperature : SingleValueObject<float, Temperature>
    {
        public static Constant AbsoluteZero { get; } = new("Absolute zero", -273.15F);
        public static Constant WaterFreeze { get; } = new("Water freeze", 0.0F);
        public static Constant WaterEvaporation { get; } = new("Water Evaporation", 100.0F);

        public Temperature(float value) : base(value)
        {
            if (value < AbsoluteZero.Value)
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        public bool IsWaterFreeze => Value <= WaterFreeze.Value;
        public bool IsWaterEvaporation => Value >= WaterEvaporation.Value;

        public new class Constant : SingleValueObject<float, Temperature>.Constant
        {
            public Constant(string name, float value) : base(value)
            {
                Name = name;
            }

            public string Name { get; set; }
        }
    }
```

Examples:
- [Temperature](Examples/Example_002_ValueObject)

## Support in frameworks and libraries
| Framework/Library      | Package                               |                                                                                                                            |
|------------------------|---------------------------------------|----------------------------------------------------------------------------------------------------------------------------|
| Entity Framework Core  | Sharp.ValueObject.EntityFrameworkCore | Correct reading and saving SingleValueObject from/to DbContext                                                             |
| System.Text.Json       | Sharp.ValueObject.Json                | Serialization and deserialization of SingleValueObject as a simple value (string, int, etc.)                               |
| Swashbuckle.AspNetCore | Sharp.ValueObject.Swashbuckle         | If System.Text.Json is used, allows json models to be rendered in Swagger based on serialization and deserialization logic |
