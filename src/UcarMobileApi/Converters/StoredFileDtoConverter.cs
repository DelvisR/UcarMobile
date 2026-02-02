using System.Reflection;
using Newtonsoft.Json;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Files;

namespace UcarMobileApi.Converters;

/// <summary>
/// A custom Newtonsoft.Json converter for <see cref="StoredFileDto"/> that generates a presigned download URL for the file.
/// </summary>
public class StoredFileDtoNewtonsoftConverter(IStorageService storageService) : JsonConverter<StoredFileDto>
{
    /// <summary>
    /// Writes the JSON representation of a <see cref="StoredFileDto"/> object, generating a presigned download URL for the file.
    /// </summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="StoredFileDto"/> value to write.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, StoredFileDto? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();

        var type = value.GetType();
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            // Skip JsonIgnore
            if (prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                continue;

            var propName = prop.Name;
            var propValue = prop.GetValue(value);

            if (propName == "Url" && !string.IsNullOrEmpty(value.Key) && !string.IsNullOrEmpty(value.Bucket))
            {
                propValue = storageService.GenerateDownloadPresignedUrl(value.Key, value.Bucket);
            }

            writer.WritePropertyName(propName);
            serializer.Serialize(writer, propValue);
        }

        writer.WriteEndObject();
    }

    /// <summary>
    /// Reads the JSON representation of a <see cref="StoredFileDto"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="hasExistingValue">Whether there is an existing value.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The deserialized <see cref="StoredFileDto"/> object.</returns>
    public override StoredFileDto? ReadJson(JsonReader reader, Type objectType, StoredFileDto? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Use default deserialization
        return serializer.Deserialize<StoredFileDto>(reader);
    }

    /// <summary>
    /// Gets a value indicating whether this converter can read JSON.
    /// </summary>
    public override bool CanRead => true;
}
