# onion-project-users-microservice
A microservice implemeting all users functionalities like preregister to an account, authenticate, etc

# Pre requisites

This project is build with .NET 6 and prepared to run as an AWS Lambda.

As a pre-requisite, you'll need to install [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and then install the [AWS Lambda tools for .NET](https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html)

If you're in a hurry, this should do the trick:

```sh
$ dotnet new -i Amazon.Lambda.Templates
```

## Running the tests

```sh
$ cd test/Otis.Users.Tests/
$ dotnet test
```

## Deploying the application

```sh
$ cd src/Otis.Users/
$ dotnet lambda package
```

Then upload the file `src/Otis.Users/bin/Release/net6.0/Otis.Users.zip` to the AWS lambda function on the AWS Web Console.
