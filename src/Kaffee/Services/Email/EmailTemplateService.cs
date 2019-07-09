using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kaffee.Models;

namespace Kaffee.Services.Email
{
    public class EmailTemplateService
    {
        public async static Task<string> GetTemplate(EmailTemplate template, Dictionary<string, string> fields)
        {
            switch (template)
            {
                case EmailTemplate.Welcome:
                    return await GetTemplateFromFile("Welcome.html", fields);
                default:
                    throw new NotImplementedException("Template does not exist");
            }
        }

        public async static Task<string> GetTemplateFromFile(string filename, Dictionary<string, string> fields)
        {
            var path = string.Format(@"./src/Kaffee/EmailTemplates/{0}", filename);
            var projectRoot = TryGetSolutionDirectoryInfo("./").FullName;
            path = Path.Combine(projectRoot, path);

            var template = await File.ReadAllTextAsync(path);
            foreach (var field in fields)
            {
                var key = string.Format("{{{0}}}", field.Key.ToUpper());
                Console.WriteLine(key);
                template = template.Replace(key, field.Value);
            }

            return template;
        }

        private static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }
    }
}