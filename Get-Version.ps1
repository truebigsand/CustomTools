$ReleaseVersion = Get-Content ./RELEASE_VERSION
Add-Content -Path $env:GITHUB_ENV -Value "ReleaseVersion=${ReleaseVersion}"