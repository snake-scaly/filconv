$file = Get-ChildItem .\FilConvWpf\Properties\AssemblyInfo.cs
$match = $file | Select-String -pattern "^\[assembly: AssemblyVersion\(`"(.*?)\.\d+`""
$version = $match.Matches[0].Groups[1].Value
dotnet publish FilConvWpf\FilConvWpf.csproj --output FilConv-$version --configuration Release -p:DebugType=None -p:DebugSymbols=false
