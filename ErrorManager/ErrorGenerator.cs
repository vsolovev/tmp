using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

[Generator]
public class ErrorCodeSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Use AdditionalTextsProvider to access additional files      
        var additionalFilesProvider = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith("errors.json"))
            .Select((file, cancellationToken) => file.GetText(cancellationToken)?.ToString());

        context.RegisterSourceOutput(additionalFilesProvider, (sourceProductionContext, jsonText) =>
        {
            if (string.IsNullOrEmpty(jsonText)) return;

            try
            {
                var jsonDoc = JsonDocument.Parse(jsonText);
                var sb = new StringBuilder();
                sb.AppendLine("using ErrTest;");
                sb.AppendLine("public static class ErrorManager {");

                GenerateCode(jsonDoc.RootElement, sb, "    ", "");

                sb.AppendLine("}");

                sourceProductionContext.AddSource("ErrorManager.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            }
            catch (FileNotFoundException ex)
            {
                // Log or handle missing assembly exception    
                throw new InvalidOperationException("Error loading assembly dependencies", ex);
            }
        });
    }

    private void GenerateCode(JsonElement element, StringBuilder sb, string indent, string path)
    {
        foreach (var prop in element.EnumerateObject())
        {
            var name = ToPascalCase(prop.Name);
            var fullPath = string.IsNullOrEmpty(path) ? name : $"{path}.{name}";

            if (prop.Value.ValueKind == JsonValueKind.Object &&
                prop.Value.TryGetProperty("Description", out var desc) &&
                prop.Value.TryGetProperty("StatusCode", out var code))
            {
                sb.AppendLine($"{indent}public static readonly ErrorCode {name} = new ErrorCode(\"{fullPath}\", \"{desc.GetString()}\", {code.GetInt32()});");
            }
            else
            {
                sb.AppendLine($"{indent}public static class {name} {{");
                GenerateCode(prop.Value, sb, indent + "    ", fullPath);
                sb.AppendLine($"{indent}}}");
            }
        }
    }

    private string ToPascalCase(string name) =>
        string.Join("", name.Split('.', '_').Select(s => char.ToUpper(s[0]) + s.Substring(1)));
}
