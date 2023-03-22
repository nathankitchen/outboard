namespace Outboard.Api.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Newtonsoft.Json;
    using Outboard.Api.Data;
    using Outboard.Api.Resources;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="HttpPostBuildTrigger" />.
    /// </summary>
    public class HttpPostBuildTriggerTests
    {
        [Fact]
        public async Task Run_Success()
        {
            var context = new DefaultHttpContext();

            var build = new BuildResource()
            {
                Version = "1.3.2",
                BuildDateUtc = DateTimeOffset.UtcNow.Subtract(new TimeSpan(1, 3, 5)),
                Changes = { 
                    new NoteResource() 
                    { 
                        Id = "402",
                        Type = "feature",
                        Title = "First feature",
                        Description = "Some description",
                        SupportingHtml = "Some <b>great</b> additional information",
                        IsHighlighted = false
                    },
                    new NoteResource() 
                    { 
                        Id = "192",
                        Type = "bug",
                        Title = "First bug",
                        Description = "Some fix description",
                        IsHighlighted = true
                    }
                }
            };

            string payload = JsonConvert.SerializeObject(build);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            context.Request.ContentType = "application/json";
            context.Request.Body = stream;

            var log = new Mock<ILogger>();
            var config = new Mock<IConfiguration>();
            var data = new Mock<IDataStore>();

            data.Setup(d => d.SaveBuild(It.IsAny<string>(), It.IsAny<BuildResource>())).Returns(Task.CompletedTask);

            var function = new HttpPostBuildTrigger(config.Object, data.Object);
            var response = await function.Run(context.Request, "test", log.Object);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            data.Verify(d => d.SaveBuild(
                It.Is<string>(t => t == "test"), 
                It.Is<BuildResource>(b => b.Version == build.Version)));
        }
    }
}