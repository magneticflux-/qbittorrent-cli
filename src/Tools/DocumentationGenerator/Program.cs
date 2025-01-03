using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace DocumentationGenerator
{
    internal class Program
    {

        [Option("-i|--input <ASSEMBLY>", "Input assembly path", CommandOptionType.SingleValue)]
        [Required]
        [FileExists]
        public string InputAssembly { get; set; }

        [Option("-o|--out-dir <PATH>", "Output directory", CommandOptionType.SingleValue)]
        [Required]
        [DirectoryExists]
        public string OutputDir { get; set; }

        [Option("-r|--root-command <CLASS>", "Root command class", CommandOptionType.SingleValue)]
        [DefaultValue("Program")]
        public string RootCommand { get; set; }

        [Option("-n|--name <ROOT_COMMAND_NAME>", "", CommandOptionType.SingleValue)]
        public string? RootCommandName { get; set; }

        private static int Main(string[] args)
        {
            var app = new CommandLineApplication<Program>();
            try
            {
                app.Conventions.UseDefaultConventions();
                var code = app.Execute(args);
                return code;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            var assembly = Assembly.LoadFrom(InputAssembly);
            var rootCommandTypeName = string.IsNullOrEmpty(RootCommand) ? "Program" : RootCommand;
            var rootCommandType = assembly.GetExportedTypes().FirstOrDefault(t => t.FullName == rootCommandTypeName)
                ?? assembly.GetExportedTypes().Single(t => t.Name == rootCommandTypeName);
            var appType = typeof(CommandLineApplication<>).MakeGenericType(rootCommandType);
            var root = (CommandLineApplication)Activator.CreateInstance(appType, [true])!;
            root.Conventions.UseDefaultConventions();
            root.Name = RootCommandName ?? assembly.GetName().Name;

            var outDir = new DirectoryInfo(OutputDir);
            foreach (var item in outDir.GetFileSystemInfos())
            {
                item.Delete();
            }

            var helpTextGenerator = new MarkdownHelpTextGenerator();
            Generate(root, root.Name!);

            using (var writer = new StreamWriter(Path.Combine(OutputDir, "command-reference.md"), false))
            {
                helpTextGenerator.GenerateCommandIndex(root, writer);
            }

            return 0;

            void Generate(CommandLineApplication command, string name)
            {
                using (var writer = new StreamWriter(Path.Combine(OutputDir, name + ".md"), false))
                {
                    helpTextGenerator.Generate(command, writer);
                }

                foreach (var subCommand in command.Commands)
                {
                    Generate(subCommand, $"{name}-{subCommand.Name}");
                }
            }
        }
    }
}
