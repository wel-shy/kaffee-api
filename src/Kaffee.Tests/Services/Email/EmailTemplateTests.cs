using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kaffee.Models;
using Kaffee.Services.Email;
using Xunit;

namespace Kaffee.Tests.Services.Email
{
    public class EmailTemplateTests
    {
        [Fact]
        public async Task TestGetFromFile()
        {
            var expectedContent = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN""""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
            <html xmlns=""http://www.w3.org/1999/xhtml"">
                <head>
                    <title>Welcome to Kaffee/title>
                </head>

                <body>
                    <h1>Welcome to Kaffee</h1>
                    <p>
                        Hey e@dwelsh.uk,<br />
                        Welcome to Kaffee! Please click <a href=""{LINK}"">here</a> to confirm your email address.
                    </p>
                </body>
            </html>";
            expectedContent = Regex.Replace(expectedContent, @"\s+", "");

            var stringContent = await EmailTemplateService.GetTemplate(EmailTemplate.Welcome, new Dictionary<string, string> {
                { "name", "e@dwelsh.uk" }
            });
            stringContent = Regex.Replace(stringContent, @"\s+", "");

            Assert.Equal(expectedContent, stringContent);
        }
    }
}