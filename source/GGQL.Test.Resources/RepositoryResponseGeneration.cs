namespace GGQL.Test.Resources
{
    //We use emebedded Resource to provide (file based) Test Data
    //using single files causes lot of problems during deployment to CI-Test Servers. embedded into a dll/assembly makes the life more easier!
    //downside: compiler takes time to compile/embedd this files.
    //solution: we use a separate assembly ONLY for that Resources and WITHOUT Dependencies to other Assemblies so that incremental Build can work and we avoid a lot of re-compiles
    //or we can unload the csproj inside the solution))

    public enum RepositoryResponseGeneration
    {
        July,
    }
}
