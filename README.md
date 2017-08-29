# GGQL
GitHub GraphQL Playground

## Idea
Try to implement some Feature that is not directly available via GitHub Api.

Some idea and requirement from [https://github.com/karelz/GitHubIssues](https://github.com/karelz/GitHubIssues)

## Technology
- pure dotnet core (2.0) 
- GraphQL
- SQLite
- MarkDig

## Architecture
* Console Worker that creates Snapshots from Github Issue Data
* Differ that compares Snapshots and create Diff-Information
* Create Markdown Reports from Diff
* Convert Markdown into html and send via email (in this case )


This code is used to generate the following Blogger Feeds:

Owner|Repository|Feed
---|---|---
dotnet|cli|[https://dotnet-cli-issues.blogspot.co.at/](https://dotnet-cli-issues.blogspot.co.at/)
dotnet|corefx|[https://dotnet-corefx-issues.blogspot.co.at/](https://dotnet-corefx-issues.blogspot.co.at/)
dotnet|coreclr|[https://dotnet-coreclr-issues.blogspot.co.at/](https://dotnet-coreclr-issues.blogspot.co.at/)

User can aggregate Repositories by Feed with the preferred News aggregator.

