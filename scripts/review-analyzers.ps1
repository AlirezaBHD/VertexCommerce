$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$out = Join-Path $root 'review-output'
New-Item -ItemType Directory -Force $out | Out-Null

dotnet build (Join-Path $root 'VertexCommerce.slnx') -p:StrictAnalyzers=false -t:Rebuild -v:q -nologo `
    "-flp:logfile=$out\build.log;verbosity=minimal;warningsonly" | Out-Null

$pattern = 'src[\\/]VertexCommerce\.[^\\/]+[\\/].*warning (\w+):'
$lines = Select-String -Path "$out\build.log" -Pattern 'warning (\w+):' |
    ForEach-Object { $_.Line.Trim() } | Sort-Object -Unique
$lines | Set-Content "$out\warnings.txt"

$lines | ForEach-Object { if ($_ -match 'warning (\w+):') { $Matches[1] } } |
    Group-Object | Sort-Object Count -Descending |
    ForEach-Object { '{0,6}  {1}' -f $_.Count, $_.Name } | Tee-Object "$out\summary.txt"

Write-Host "`nDetails: $out\warnings.txt"
