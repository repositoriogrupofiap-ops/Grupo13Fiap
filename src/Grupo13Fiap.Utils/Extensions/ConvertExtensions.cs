using System.ComponentModel;

namespace Grupo13Fiap.Utils.Extensions
{
    public static class ConvertExtensions
    {
        public static T To<T>(this object? value)
        {
            if(value is null)
                return default!;

            return (T)value.To(typeof(T));
        }

        public static object To(this object value, Type conversionType)
        {
            if(conversionType == null)
                throw new ArgumentNullException("conversionType");

            if(conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if(value == null || Convert.IsDBNull(value))
                    return null;

                var nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            } else if(conversionType == typeof(Guid))
            {
                return new Guid($"{value}");
            } else if(conversionType.IsEnum && value is string)
            {
                return Enum.Parse(conversionType, (string)value);
            }

            if((value is string || value == null || value is DBNull) &&
                (conversionType == typeof(short) ||
                conversionType == typeof(int) ||
                conversionType == typeof(long) ||
                conversionType == typeof(double) ||
                conversionType == typeof(decimal) ||
                conversionType == typeof(float)))
            {
                if(!decimal.TryParse(value as string, out _))
                    value = "0";
            }

            if(conversionType == typeof(bool) && (value == null || value is DBNull))
            {
                value = 0;
            }

            if(conversionType == typeof(DateTime) && (value == null || value is DBNull))
            {
                value = DateTime.MinValue;
            }

            return Convert.ChangeType(value, conversionType);
        }
    }
}