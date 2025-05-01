# ObfuscarAddOn
This project is designed to be used on top of `Obfuscar`. <br/>
It currently has two functionalities: `AssemblyInfo` editing (`Guid`, `AssemblyTitle`, `AssemblyProduct`) and obfuscation of selected functions parameters.

## Usage

```
.\ObfuscarAddOn.exe .\input.exe .\output.exe
.\ObfuscarAddOn.exe .\input.exe .\output.exe function
.\ObfuscarAddOn.exe .\input.exe .\output.exe function1,function2
```

## AssemblyInfo editing
The `Guid` is randomized and the properties `AssemblyTitle` and `AssemblyProduct` are changed to `Update`.

## Functions parameters obfuscation
When `Obfuscar` skips obfuscating a method, it also leaves its parameter names unobfuscated.<br/> 
I’ve submitted a pull request to add a setting that forces parameter obfuscation, but until it’s merged, this workaround can be used.
