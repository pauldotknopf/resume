﻿using System;
using System.IO;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PowerArgs;
using Resume.Config;
using Statik.Embedded;
using Statik.Files;
using Statik.Mvc;
using Statik.Web;
using YamlDotNet.Serialization;

namespace Resume
{
    class Program
    {
        private static IWebBuilder _webBuilder;
        private static string _contentDirectory = Directory.GetCurrentDirectory();
        
        static int Main(string[] args)
        {
            try
            {
                _webBuilder = Statik.Statik.GetWebBuilder();
                _webBuilder.RegisterMvcServices();
                _webBuilder.RegisterServices(services =>
                {
                    services.Configure<RazorViewEngineOptions>(options =>
                    {
                        //options.FileProviders.Add(new EmbeddedFileProvider(typeof(Program).Assembly, "Resume.Resources"));
                        options.FileProviders.Add(new PhysicalFileProvider("/Users/pknopf/git/resume/statik-resume-generator/src/Resume/Resources"));
                    });
                });
                
                RegisterPages();
                RegisterResources();
                RegisterConfig();
                
                try
                {
                    Args.InvokeAction<Program>(args);
                }
                catch (ArgException ex)
                {
                    Console.WriteLine(ex.Message);
                    ArgUsage.GenerateUsageFromTemplate<Program>().WriteLine();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            return 0;
        }

        private static void RegisterResources()
        {
            //_webBuilder.RegisterFileProvider(new EmbeddedFileProvider(typeof(Program).Assembly, "Resume.Resources.wwwroot"));
            _webBuilder.RegisterFileProvider(new PhysicalFileProvider("/Users/pknopf/git/resume/statik-resume-generator/src/Resume/Resources/wwwroot"));
            var staticDirectory = Path.Combine(_contentDirectory, "static");
            if (Directory.Exists(staticDirectory))
            {
                _webBuilder.RegisterDirectory(staticDirectory);
            }
        }
        
        private static void RegisterConfig()
        {
            var config = new ResumeConfig();
            config.Personal = new DeserializerBuilder().Build()
                .Deserialize<Personal>(File.ReadAllText(Path.Combine(_contentDirectory, "personal.yml")));
            config.Resume = new DeserializerBuilder().Build()
                .Deserialize<Config.Resume>(File.ReadAllText(Path.Combine(_contentDirectory, "resume.yml")));
            config.General = new DeserializerBuilder().Build()
                .Deserialize<General>(File.ReadAllText(Path.Combine(_contentDirectory, "general.yml")));

            var markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            config.Resume.Career = Markdown.ToHtml(config.Resume.Career);
            foreach (var experience in config.Resume.Experience)
            {
                experience.Description = Markdown.ToHtml(experience.Description, markdownPipeline);
            }
            foreach (var project in config.Resume.Projects)
            {
                project.Description = Markdown.ToHtml(project.Description, markdownPipeline);
            }
            
            _webBuilder.RegisterServices(services => services.AddSingleton(config));

            if (!string.IsNullOrEmpty(config.General.CName))
            {
                _webBuilder.Register("/CNAME",
                    context => context.Response.WriteAsync(config.General.CName),
                    extractExactPath: true);
            }
        }

        private static void RegisterPages()
        {
            _webBuilder.RegisterMvc("/", new
            {
                controller = "Resume",
                action = "Index"
            });
            _webBuilder.RegisterMvc("/template", new
            {
                controller = "Resume",
                action = "Template"
            });
            _webBuilder.RegisterMvc("/resume.pdf", new
            {
                controller = "Resume",
                action = "Pdf"
            });
        }
        
        [ArgActionMethod, ArgIgnoreCase]
        public void Serve()
        {
            Console.WriteLine("serve");
            using (var host = _webBuilder.BuildWebHost(port: 8000))
            {
                host.Listen();
                Console.WriteLine("Listening on port 8000...");
                Console.ReadLine();
            }
        }
        
        public class BuildArgs
        {
            [ArgDefaultValue("output"), ArgShortcut("o")]
            public string Output { get; set; }
        }
        
        [ArgActionMethod, ArgIgnoreCase]
        public async Task Build(BuildArgs args)
        {
            using (var host = _webBuilder.BuildVirtualHost())
            {
                await Statik.Statik.ExportHost(host, args.Output);
            }
        }
    }
}
