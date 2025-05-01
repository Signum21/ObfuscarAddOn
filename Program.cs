using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using System.Security.Cryptography;

namespace ObfuscarAddOn
{
    class Program
    {
        static void RandomizeGuid(ModuleDefinition _assembly)
        {
            string newGuid = Guid.NewGuid().ToString();
            var customAttribute = _assembly.Assembly.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.FullName == "System.Runtime.InteropServices.GuidAttribute");

            if (customAttribute != null)
            {
                customAttribute.ConstructorArguments[0] = new CustomAttributeArgument(_assembly.TypeSystem.String, newGuid);
                Console.WriteLine("Updated GUID to: " + newGuid);
            }
        }

        static void ObfuscatePublicParameters(ModuleDefinition _assembly, List<string> _functions)
        {
            foreach (var type in _assembly.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (_functions.Contains(method.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        for (int i = 0; i < method.Parameters.Count; i++)
                        {
                            var oldName = method.Parameters[i].Name;
                            
                            byte[] bytes = new byte[8];
                            RandomNumberGenerator.Create().GetBytes(bytes);
                            string rand = BitConverter.ToUInt32(bytes, 0).ToString();

                            method.Parameters[i].Name = rand;
                            Console.WriteLine($"Renamed parameter '{oldName}' to '{rand}' in {method.Name}");
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ObfuscarAddOn.exe <InputAssembly> <OutputAssembly> <OptionalFunctions>");
                return;
            }
            ModuleDefinition assembly = AssemblyDefinition.ReadAssembly(args[0]).MainModule;
            RandomizeGuid(assembly);

            if(args.Length > 2)
            {
                List<string> functions = args[2].Split(',').ToList();
                ObfuscatePublicParameters(assembly, functions);
            }
            string outputPath = args[1];
            assembly.Write(outputPath);
            Console.WriteLine("Modified assembly saved to: " + outputPath);
        }
    }
}
