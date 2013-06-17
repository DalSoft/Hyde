﻿using Mono.Options;
using Newtonsoft.Json;
using RazorEngine;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace Hyde
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = string.Empty;
            var layouts = string.Empty;
            var json = string.Empty;
            var output = string.Empty;
            var help = false;
            dynamic model = null;
            
            //TODO add base template args support
            var options = new OptionSet
                              {
                                      {"s|source=", "The folder with the .cshtml files you want to generate (required).", v => source = v},
                                      {"l|layouts=", "Layout file(s) to use, use | as a delimiter for multiple files (optional).", v => layouts = v},
                                      {"j|json=", "The model to use specified using JSON (optional).", v => json = v},
                                      {"o|output=", "The output folder where static site is generated (required).", v => output = v},
                                      {"h|help",  "Show this message and exit", v => help = true},
                                  };

            options.Parse(args);

            if (Debugger.IsAttached)
            {
                var projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                source = projectPath + @"\wwwtest";
                output = projectPath + @"\output";
                json = "{Title:'Test Hyde', H2:'Test Heading', IndexContent:'Some Test content for index', Page1Content:'Some Test content for page1', Page2Content:'Some Test content for page2'}";
                layouts = projectPath + @"\wwwtest\layout.cshtml|" + projectPath + @"\wwwtest\layout2.cshtml";
            }

            if (help)
            {
                options.WriteOptionDescriptions(Console.Out);
                if (Debugger.IsAttached) Console.ReadLine();

                Environment.Exit(0);
            }
            
            if (source == null && output == null)
            {
                Console.WriteLine("You need to provide -source and -output arguments. Use -help for usage.");
                if (Debugger.IsAttached) Console.ReadLine();
                Environment.Exit(1);
            }

            if (!Directory.Exists(source))
            {
                Console.WriteLine("-source folder " + source + " not found.");
                if (Debugger.IsAttached) Console.ReadLine();
                Environment.Exit(1);
            }

            if (json!=null)
            {
                try
                {
                    model = JsonConvert.DeserializeObject<ExpandoObject>(json);
                }catch  {
                    Console.WriteLine("-json problem deserializing JSON provided. Is the JSON valid?");
                    if (Debugger.IsAttached) Console.ReadLine();
                    Environment.Exit(1); 
                }
            }

            if (layouts != null) //Parse Layouts first.
            {   
                layouts.
                    Split("|".ToCharArray() , StringSplitOptions.RemoveEmptyEntries)
                    .Select(x=> new {filePath=x, cacheName = new FileInfo(x).Name})
                    .ToList()
                    .ForEach(x=> Razor.Compile(File.ReadAllText(x.filePath), x.cacheName));
                
                //Below doesn't work for some reason always returns stack empty
                //Razor.ParseMany(razorTemplates: layoutTemplates.Select(x=>File.ReadAllText(x.filePath)),cacheNames:layoutTemplates.Select(x=>x.cacheName).ToList(), parallel:true);
            }

            if (Directory.Exists(output)) Directory.Delete(output, true);
            Directory.CreateDirectory(output);

            var cshtml = Directory.GetFiles(source, "*.cshtml")
                    .Select(x=>x.ToLower())
                    .Except(layouts.ToLower().Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    .Select(x => new { filePath = x, razorTemplate = File.ReadAllText(x), cacheName = new FileInfo(x).Name, model })
                    .ToList();

            Razor.ParseMany(models: cshtml.Select(x => x.model), razorTemplates: cshtml.Select(x => x.razorTemplate), cacheNames: cshtml.Select(x => x.cacheName), parallel: true);

            foreach (var file in cshtml)
            {
                var outputFilePath = output + @"\" + new FileInfo(file.filePath).Name.Replace(".cshtml", ".html");
                
                using (var outputFile = File.CreateText(outputFilePath))
                {
                    outputFile.Write(Razor.Parse(file.razorTemplate, model ?? new { }, file.cacheName));
                }
                
            }

            Environment.Exit(0); 
        }

    }

}
