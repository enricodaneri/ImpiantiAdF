$nugetPackageFile = "packages.config"

$files = Get-ChildItem -Path $solutionFolder -Filter $nugetPackageFile -Recurse

foreach ($file in $files)
{
    [xml]$xml = Get-Content $file.FullName
    $nodes = Select-Xml "/packages/package/@id" $xml
    $packageIds = @{}

    foreach ($node in $nodes) {
        $packageId = $node.Node.'#text'
        try
        {
            $packageIds.Add($packageId, $packageId)
        }
        Catch [System.ArgumentException]
        {
            Write-Host "Found duplicate package in " $file.FullName ". Duplicate package: $packageId"
        }
    }
}
