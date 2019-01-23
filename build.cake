///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// EXTERNAL NUGET TOOLS
//////////////////////////////////////////////////////////////////////

// запуск тестов
#Tool "xunit.runner.console"

var testResultsDir = artifactsDir + Directory("test-results");
var solutionPath = "./UnitTests.BugsStrikesBack.sln";
var framework = "netstandard2.0";

Task("Clean")
    .Does(() => 
    {            
        DotNetCoreClean(solutionPath);        
        DirectoryPath[] cleanDirectories = new DirectoryPath[] {
            binsDir,
            testResultsDir,
        };
    
        CleanDirectories(cleanDirectories);
    
        foreach(var path in cleanDirectories) { EnsureDirectoryExists(path); }
    
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => 
    {
        var settings = new DotNetCoreBuildSettings
          {
              Configuration = configuration
          };
          
        DotNetCoreBuild(
            solutionPath,
            settings);
    });


    
Task("Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {        
        Information("UnitTests task...");
        var projects = GetFiles("./tests/DeathStar.UnitTests/*csproj");
        foreach(var project in projects)
        {
            Information(project);
            
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = false
                });
        }
    });    

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Tests");
    
RunTarget(target);
