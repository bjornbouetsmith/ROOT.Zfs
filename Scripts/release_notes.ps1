param(
[string] $project="common",
[string] $repo="ROOT.Zfs" , 
[int] $count=10,
[string] $nltoken="####",
[string] $TagPrefix="R",
[string] $BuildNumber="$env:Build_BuildNumber")

$AESKey = Get-Content "$PSScriptRoot\key.txt"

$pass = get-content "$PSScriptRoot\cred.txt" | convertto-securestring -Key $AESKey

$credentials = new-object -typename System.Management.Automation.PSCredential -argumentlist "bbs@root.dom",$pass



$base = $env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI

if($base.Length -eq 0)
{
  $base ="http://build.root.dom/ROOT"
}
$refUrl = $uri=$base+[string]::Format("/{0}/_apis/git/repositories/{1}/refs?api-version=1.0",$project,$repo)

$refs = Invoke-WebRequest $uri -Method Get -Credential $credentials -ContentType "application/json" -UseBasicParsing | ConvertFrom-Json

$fullVer = $BuildNumber
$idx = $fullVer.LastIndexOf(" ")
if( $idx -gt -1)
{
    $current_tag = $fullVer.Substring($idx).Trim()
}
else
{
   
    $current_tag = $fullVer
}

$current_tag = "$TagPrefix$current_tag"
Write-Host "using $current_tag as tag for current release"
function GetTag([string] $commitId)
{
    $tag = $refs.value|where objectid -eq $commitId |Select-Object -ExpandProperty name
    if($tag -ne $null -and $tag -ne "")
    {
      return $tag.Split('/')[2]  
    }
    return "--------"
}
 
 
 
 $api = '/{0}/_apis/git/repositories/{1}/commits?$top={2}&$orderby=id%20desc';
   
 $url = [string]::Format($api,$project,$repo, $count*2)

 $complete = $base + $url
 $changeset = ""
 $outdir = $PSScriptRoot;
 Invoke-WebRequest -Uri $complete -OutFile "$outdir\history.txt" -Credential $credentials  -UseBasicParsing
 $content = Get-Content "$outdir\history.txt";
$tfsData = [System.Text.Encoding]::UTF8.GetString([System.Text.Encoding]::Default.GetBytes($content ))
$first = $true    
$x = ConvertFrom-Json -InputObject $tfsData| Foreach-Object -MemberName value | ForEach-Object -Process {
    $comment = $_.comment;
    
    $tag = GetTag -commitId $_.commitId

    if ($comment.Contains("**NO_CI**") -eq $false)
    {
        if(($tag -eq "--------"  -or $tag -eq "master" ) -and $first -eq $true)
        {
            $tag = $current_tag
            $first=$false
        }
         $changeset = $changeset+"$($(Get-Date -Date $_.author.date).ToString("yyyy-MM-dd"))  $tag  $($_.author.name.ToUpper())  $($comment.Replace("`n"," "))$nltoken"
    } 
 }

 Write-Host "ReleaseNotes:"
 Write-Host $changeset
 write-host "##vso[task.setvariable variable=ReleaseNotes]$changeset"
 Set-Content -Path "$outdir\ReleaseNotes.txt" -Value $changeset
 $env:ReleaseNotes = $changeset