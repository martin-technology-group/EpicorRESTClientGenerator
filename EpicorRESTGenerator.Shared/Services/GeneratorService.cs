using EpicorRESTGenerator.Shared.Models;
using Newtonsoft.Json;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EpicorRESTGenerator.Shared.Services
{
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2007/app")]
    [XmlRoot("service", Namespace = "http://www.w3.org/2007/app", IsNullable = false)]
    public partial class GeneratorService
    {
        [XmlElement("workspace")]
        public ServiceWorkspace Workspace { get; set; }

        public static async Task<bool> GenerateCode(GeneratorService services, GeneratorOptions options)
        {
            using (WebClient client = WebClientService.GetWebClient(
                string.IsNullOrEmpty(options.Username) ? string.Empty : options.Username,
                string.IsNullOrEmpty(options.Password) ? string.Empty : options.Password))
            {
                foreach (ServiceWorkspaceCollection service in services.Workspace.Collection)
                {
                    string name = service.Href.Replace(".", string.Empty).Replace("-", string.Empty);
                    try
                    {
                        string response = client.DownloadString(options.APIURL + service.Href);

                        dynamic responseJson = JsonConvert.DeserializeObject(response);
                        if (!options.APIURL.Contains("baq"))
                        {
                            foreach (dynamic j in responseJson["paths"])
                            {
                                dynamic post = j.First["post"];
                                if (post != null)
                                {
                                    dynamic postOpID = j.First["post"]["operationId"];
                                    if (postOpID != null)
                                    {
                                        j.First["post"]["operationId"] = j.Name.Replace(@"\", "").Replace("/", string.Empty);
                                    }
                                }
                            }
                        }

                        string output = JsonConvert.SerializeObject(responseJson, Formatting.Indented);

                        SwaggerDocument document = await SwaggerDocument.FromJsonAsync(output);
                        SwaggerToCSharpClientGeneratorSettings settings = new SwaggerToCSharpClientGeneratorSettings()
                        {
                            ClassName = name,
                            OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator()
                        };

                        SwaggerToCSharpClientGenerator generator = new SwaggerToCSharpClientGenerator(document, settings);

                        if (options.UseBaseClass)
                        {
                            generator.Settings.ClientBaseClass = options.BaseClass;
                        }

                        generator.Settings.CSharpGeneratorSettings.Namespace = options.UseNamespace ? "MyNamespace" : options.Namespace;
                        generator.Settings.UseHttpClientCreationMethod = true;
                        generator.Settings.AdditionalNamespaceUsages = new[] { "Newtonsoft.Json", "Newtonsoft.Json.Linq" };
                        generator.Settings.DisposeHttpClient = false;

                        string code = generator.GenerateFile();
                        code = code
                            .Replace("MyNamespace", options.Namespace + "." + service.Href.Replace("-", ""))
                            .Replace("var client_ = await CreateHttpClientAsync(cancellationToken).ConfigureAwait(false);",
                            "var client_ = CreateHttpClient();")
                            .Replace("#pragma warning disable // Disable all warnings", "")
                            .Replace("<Key>k", "Keyk")
                            .Replace("<Value>k", "Valuek")
                            .Replace("_tLÐ¡TotalCost", "_tLDTotalCost")
                            .Replace("TLÐ¡TotalCost", "TLDTotalCost")
                            .Replace("private System.Collections.Generic.IDictionary<string, string> _additionalProperties = " +
                            "new System.Collections.Generic.Dictionary<string, string>();",
                            "private System.Collections.Generic.IDictionary<string, JToken> _additionalProperties = " +
                            "new System.Collections.Generic.Dictionary<string, JToken>();")
                            .Replace("public System.Collections.Generic.IDictionary<string, string> AdditionalProperties",
                            " public System.Collections.Generic.IDictionary<string, JToken> AdditionalProperties")
                            .Replace(", Required = Newtonsoft.Json.Required.Always)]", ", Required = Newtonsoft.Json.Required.AllowNull)]")
                            .Replace("[System.ComponentModel.DataAnnotations.Required]", "")
                            .Replace(@"public string BaseUrl", "public string ServiceUrl")
                            .Replace(@"public string BaseUrl", "public string ServiceUrl")
                            .Replace(@"get { return _baseUrl; }", "get { return base.BaseUrl + _serviceUrl; }")
                            .Replace(@"set { _baseUrl = value; }", "set { _serviceUrl = value; }")
                            .Replace(@"urlBuilder_.Append(BaseUrl)", "urlBuilder_.Append(ServiceUrl)")
                            // Convert doubles and longs to strings (IEEE754Compatible)
                            .Replace("private double?", "private string")
                            .Replace("public double?", "public string")
                            .Replace("private long?", "private string")
                            .Replace("public long?", "public string")
                        ;

                        // String URL parameters must preserve single quotes, while
                        // other URL paramater data types must not include single quotes
                        code = Regex.Replace(code,
                            "(urlBuilder_\\.Replace\\(\"{(?:.*?)}\", System\\.Uri\\.EscapeDataString\\(" +
                            "System.Convert.ToString\\()((?:.*?))(, System\\.Globalization\\.CultureInfo\\.InvariantCulture\\)\\)\\);)",
                            delegate (Match match)
                            {
                                string matchedVariable = match.Groups[2].Value;
                                if (matchedVariable == "company" || matchedVariable == "salesRepCode")
                                {
                                    return match.Groups[1].Value + "\"'\" + " + matchedVariable + " + \"'\"" + match.Groups[3].Value;
                                }
                                else
                                {
                                    return match.ToString();
                                }
                            });

                        code = Regex.Replace(code,
                            "(private string _baseUrl = \")(.*?)(\";)",
                            delegate (Match match)
                            {
                                return "private string _serviceUrl = \"" + service.Href.Replace("-", "") + match.Groups[3].Value;
                            });

                        File.WriteAllText(Path.GetDirectoryName(options.Project) + "\\" + service.Href + ".cs", code);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{1} : <------> {0}", ex, name);
                        string directory = AppDomain.CurrentDomain.BaseDirectory + @"/Logs/";
                        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                        File.AppendAllText(directory + DateTime.Now.ToString("MMDDYYYY_hhmmssfffff") + ".txt", name + Environment.NewLine + ex);
                    }
                }
            }
            return true;
        }

        public static GeneratorService GetEpicorServices(string serviceURL, GeneratorOptions details)
        {
            using (WebClient client = WebClientService.GetWebClient(
                string.IsNullOrEmpty(details.Username) ? "" : details.Username,
                string.IsNullOrEmpty(details.Password) ? "" : details.Password))
            {
                GeneratorService services = new GeneratorService();
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(GeneratorService));
                string xml = client.DownloadString(serviceURL);

                using (StringReader reader = new StringReader(xml))
                {
                    services = (GeneratorService)serializer.Deserialize(reader);
                }

                return services;
            }
        }
    }
}
