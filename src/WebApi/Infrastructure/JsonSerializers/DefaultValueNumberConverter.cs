using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Infrastructure.JsonSerializers;

public class DefaultValueNumberConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle null values explicitly
        if (reader.TokenType == JsonTokenType.Null)
        {
            // If the type is nullable, return null; otherwise, return default(T)
            return Nullable.GetUnderlyingType(typeof(T)) != null ? (T)(object)null : default;
        }

        try
        {
            // Handle numeric types
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (typeof(T) == typeof(int) || Nullable.GetUnderlyingType(typeof(T)) == typeof(int))
                    return (T)(object)(int)Math.Truncate(reader.GetDouble()); // Truncate for int
                if (typeof(T) == typeof(long) || Nullable.GetUnderlyingType(typeof(T)) == typeof(long))
                    return (T)(object)(long)Math.Truncate(reader.GetDouble()); // Truncate for long
                if (typeof(T) == typeof(double) || Nullable.GetUnderlyingType(typeof(T)) == typeof(double))
                    return (T)(object)reader.GetDouble();
                if (typeof(T) == typeof(float) || Nullable.GetUnderlyingType(typeof(T)) == typeof(float))
                    return (T)(object)(float)reader.GetDouble(); // Convert to float
                if (typeof(T) == typeof(decimal) || Nullable.GetUnderlyingType(typeof(T)) == typeof(decimal))
                    return (T)(object)(decimal)reader.GetDouble(); // Convert to decimal
            }

            // Handle string representations of numbers
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    // If the type is nullable, return null; otherwise, return default(T)
                    return Nullable.GetUnderlyingType(typeof(T)) != null ? (T)(object)null : default;
                }

                // Parse based on the target type
                if (typeof(T) == typeof(int) || Nullable.GetUnderlyingType(typeof(T)) == typeof(int))
                {
                    if (int.TryParse(stringValue, out var intValue))
                        return (T)(object)intValue;
                    else if (double.TryParse(stringValue, out var doubleValue))
                        return (T)(object)(int)Math.Truncate(doubleValue); // Truncate fractional part
                }
                if (typeof(T) == typeof(long) || Nullable.GetUnderlyingType(typeof(T)) == typeof(long))
                {
                    if (long.TryParse(stringValue, out var longValue))
                        return (T)(object)longValue;
                    else if (double.TryParse(stringValue, out var doubleValue))
                        return (T)(object)(long)Math.Truncate(doubleValue); // Truncate fractional part
                }
                if (typeof(T) == typeof(double) || Nullable.GetUnderlyingType(typeof(T)) == typeof(double))
                {
                    if (double.TryParse(stringValue, out var doubleValue))
                        return (T)(object)doubleValue;
                    else if (int.TryParse(stringValue, out var intValue))
                        return (T)(object)(double)intValue; // Convert to double
                }
                if (typeof(T) == typeof(float) || Nullable.GetUnderlyingType(typeof(T)) == typeof(float))
                {
                    if (float.TryParse(stringValue, out var floatValue))
                        return (T)(object)floatValue;
                    else if (int.TryParse(stringValue, out var intValue))
                        return (T)(object)(float)intValue; // Convert to float
                }
                if (typeof(T) == typeof(decimal) || Nullable.GetUnderlyingType(typeof(T)) == typeof(decimal))
                {
                    if (decimal.TryParse(stringValue, out var decimalValue))
                        return (T)(object)decimalValue;
                    else if (int.TryParse(stringValue, out var intValue))
                        return (T)(object)(decimal)intValue; // Convert to decimal
                }
            }
        }
        catch (Exception ex)
        {
            throw;
            // In case of any parsing errors, return null for nullable types or default(T) otherwise
        }

        // Fallback to null for nullable types or default(T) otherwise
        return Nullable.GetUnderlyingType(typeof(T)) != null ? (T)(object)null : default;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Write null if the value is null
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Write the numeric value directly
        switch (value)
        {
            case int intValue:
                writer.WriteNumberValue(intValue);
                break;
            case long longValue:
                writer.WriteNumberValue(longValue);
                break;
            case double doubleValue:
                writer.WriteNumberValue(doubleValue);
                break;
            case float floatValue:
                writer.WriteNumberValue(floatValue);
                break;
            case decimal decimalValue:
                writer.WriteNumberValue(decimalValue);
                break;
            default:
                throw new JsonException($"Unsupported numeric type: {typeof(T)}");
        }
    }
}