using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Mono.Cecil;

namespace ObfuscarAddOn
{
    class Program
    {
        internal static void UpdateAssemblyMetadata(ModuleDefinition _assembly, string property, string newValue)
        {
            var customAttribute = _assembly.Assembly.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.FullName == property);

            if (customAttribute != null)
            {
                string oldValue = customAttribute.ConstructorArguments[0].Value.ToString();
                customAttribute.ConstructorArguments[0] = new CustomAttributeArgument(_assembly.TypeSystem.String, newValue);
                Console.WriteLine($"Updated {customAttribute.AttributeType.Name} from {oldValue} to: {newValue}");
            }
        }

        internal static void ObfuscatePublicParameters(ModuleDefinition _assembly, List<string> _functions)
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

        internal static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ObfuscarAddOn.exe <InputAssembly> <OutputAssembly> <Functions>");
                return;
            }
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(args[0]);
            string newName = "Update";

            // Modified by IlRepack
            //assembly.Name.Name = newName;
            //assembly.MainModule.Name = newName + ".exe";

            UpdateAssemblyMetadata(assembly.MainModule, "System.Runtime.InteropServices.GuidAttribute", Guid.NewGuid().ToString());
            UpdateAssemblyMetadata(assembly.MainModule, "System.Reflection.AssemblyTitleAttribute", newName);
            UpdateAssemblyMetadata(assembly.MainModule, "System.Reflection.AssemblyProductAttribute", newName);

            if (args.Length > 2)
            {
                List<string> functions = args[2].Split(',').ToList();
                ObfuscatePublicParameters(assembly.MainModule, functions);
            }
            string outputPath = args[1];
            assembly.Write(outputPath);
            Console.WriteLine("Modified assembly saved to: " + outputPath);
        }
    }
}
