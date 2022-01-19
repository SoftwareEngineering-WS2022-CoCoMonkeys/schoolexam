using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SchoolExam.Web.Serialization;

/// <summary>
/// <para>
/// A base JSON converter that allows objects accepted or returned by the API using a base type or interface to be converted to a derived type. The derived type is
/// specified in the object's JSON in a field named $typeName. The actual format of this value is up to the classes that derive from this class.
/// </para>
/// <para>
/// Create a derived converter class for each base type or interface for which derived types will be passed to or received from the API methods. Each derived converter class
/// should 1) override <see cref="DerivedTypeJsonConverter{TBase}.NameToType(string)"/>, which receives the value of the $typeName field, and should instantiate objects based
/// on the $typeName value, and 2) override <see cref="DerivedTypeJsonConverter{TBase}.TypeToName(Type)"/> which returns the $typeName value to use based on the object's type.
/// </para>
/// </summary>
/// <typeparam name="TBase"></typeparam>
/// <remarks>
/// <para>This is almost what JSON.Net's "type name handling" does. However, type naming handling emits the object's type along with its namespace and assembly.</para>
/// </remarks>
public abstract class DerivedTypeJsonConverter<TBase> : JsonConverter<TBase>
{
	/// <summary>
	/// Returns the value to use for the $type property.
	/// </summary>
	/// <returns></returns>
	protected abstract string TypeToName(Type type);

	/// <summary>
	/// Returns the type that corresponds to the specified $type value.
	/// </summary>
	/// <param name="typeName"></param>
	/// <returns></returns>
	protected abstract Type NameToType(string typeName);

	/// <summary>
	/// The name of the "type" property in JSON.
	/// </summary>
	private const string TypePropertyName = "type";

	public override bool CanConvert(Type objectType)
	{
		return typeof(TBase) == objectType;
	}
	
	public override TBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		// get the $type value by parsing the JSON string into a JsonDocument
		var jsonDocument = JsonDocument.ParseValue(ref reader);
		jsonDocument.RootElement.TryGetProperty(TypePropertyName, out JsonElement typeNameElement);
		var typeName = typeNameElement.ValueKind == JsonValueKind.String ? typeNameElement.GetString() : null;
		if (string.IsNullOrWhiteSpace(typeName))
		{
			throw new InvalidOperationException($"Missing or invalid value for {TypePropertyName} (base type {typeof(TBase).FullName}).");
		}

		// get the JSON text that was read by the JsonDocument
		string json;
		using (var stream = new MemoryStream())
		using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Encoder = options.Encoder })) {
			jsonDocument.WriteTo(writer);
			writer.Flush();
			json = Encoding.UTF8.GetString(stream.ToArray());
		}

		// deserialize the JSON to the type specified by $type
		try {
			return (TBase) JsonSerializer.Deserialize(json, NameToType(typeName), options)!;
		}
		catch (Exception ex) {
			throw new InvalidOperationException("Invalid JSON in request.", ex);
		}
	}

	public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
	{
		// create an ExpandoObject from the value to serialize so we can dynamically add a $type property to it
		ExpandoObject expando = ToExpandoObject(value);
		expando.TryAdd(TypePropertyName, TypeToName(value!.GetType()));

		// serialize the expando
		JsonSerializer.Serialize(writer, expando, options);
	}

	/// <summary>
	/// Returns an <see cref="ExpandoObject"/> whose values are copied from the specified object's public properties.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	private static ExpandoObject ToExpandoObject(object? obj)
	{
		var expando = new ExpandoObject();
		if (obj != null) {
			// copy all public properties
			foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead))
			{
				// make sure that property name starts with a lower case letter
				var propertyName = property.Name[0].ToString().ToLower() + property.Name.Substring(1);
				expando.TryAdd(propertyName, property.GetValue(obj));
			}
		}

		return expando;
	}
}